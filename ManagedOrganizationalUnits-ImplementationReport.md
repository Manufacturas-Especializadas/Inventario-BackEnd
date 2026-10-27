# Unidades administradas por Production — informe local

Fecha: 2026-10-27. Se trabajó sobre los estados locales de `PPEInventory` y `PPEInventory-FrontEnd`, conservando los cambios publicados y no publicados que ya existían. No se hizo commit ni push. No se aplicó ninguna migración a ninguna base de datos.

## Diseño y comportamiento anterior

En `CreatePPERequestCommandHandler.Handle`, la rama Production resolvía el empleado mediante `_currentUser.EmployeeId`, tomaba su `Employee.OrganizationalUnitId` y rechazaba un `EmployeeNumber` diferente o una unidad de consumo distinta. `ProductionPPERequestForm` mostraba la identidad del solicitante y omitía ambos campos en el POST. Administrator ya exigía ambos campos explícitos y tenía precedencia en cuentas con ambos roles.

La nueva entidad `UserManagedOrganizationalUnit` se guarda en `UserManagedOrganizationalUnits`, con PK compuesta `(UserId, OrganizationalUnitId)`, FK a `Users` con Cascade, FK a `OrganizationalUnits` con Restrict e índice por `OrganizationalUnitId`. Sigue el patrón de `UserRoleConfiguration`. La navegación `User.ManagedOrganizationalUnits` representa la responsabilidad; `Employee.OrganizationalUnitId` sigue siendo la adscripción principal individual.

`PPERequest.RequestedForOrganizationalUnitId` sigue representando el destino de consumo. No se cambió ninguna de estas dos columnas ni solicitudes históricas.

No se expanden permisos a descendientes. La jerarquía existente resuelve el camino hacia la raíz para límites; no había una regla de autorización heredada que justificara ampliar el alcance. Cada unidad debe asignarse explícitamente.

## Migración y backfill

Migración: `20261027144613_AddUserManagedOrganizationalUnits`, con su Designer y actualización del snapshot.

Después de crear la tabla e índice, ejecuta este backfill dentro de la migración:

```sql
INSERT INTO [UserManagedOrganizationalUnits] ([UserId], [OrganizationalUnitId])
SELECT u.[Id], e.[OrganizationalUnitId]
FROM [Users] u
INNER JOIN [Employees] e ON e.[Id] = u.[EmployeeId]
INNER JOIN [OrganizationalUnits] ou ON ou.[Id] = e.[OrganizationalUnitId]
WHERE e.[OrganizationalUnitId] IS NOT NULL
  AND EXISTS (
      SELECT 1 FROM [UserRoles] ur
      INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId]
      WHERE ur.[UserId] = u.[Id] AND r.[Name] = 'Production'
  )
  AND NOT EXISTS (
      SELECT 1 FROM [UserManagedOrganizationalUnits] managed
      WHERE managed.[UserId] = u.[Id]
        AND managed.[OrganizationalUnitId] = e.[OrganizationalUnitId]
  );
```

Incluye cuentas y unidades inactivas para conservar la relación si posteriormente se reactivan. Eso no permite solicitarlas mientras estén inactivas: las consultas y creación verifican el estado activo. También incluye usuarios que tengan Production junto con otros roles. No usa IDs de roles codificados.

Se revisaron la migración, el diff del snapshot y los scripts SQL generados de subida/bajada. La subida crea exclusivamente la relación y su backfill; no elimina columnas ni modifica `Employees` o `PPERequests`. `Down()` elimina únicamente `UserManagedOrganizationalUnits`; EF elimina además el registro correspondiente de su historial de migraciones. No hay cambios pendientes entre el modelo y la última migración según EF.

Migración aplicada en producción: **NO**. Aplicada en local: **NO**. El SQL del backfill fue revisado, no ejecutado.

## Backend

| Operación | Acceso y comportamiento |
|---|---|
| `GET /api/users/{id}/managed-organizational-units` | Administrator. Devuelve las asignaciones, incluidas las inactivas para poder administrarlas. |
| `PUT /api/users/{id}/managed-organizational-units` | Administrator. Body `{ "organizationalUnitIds": [1, 2, 4] }`. Reemplaza el conjunto y responde 204. Lista vacía revoca las asignaciones. |
| `GET /api/ppe-requests/form-options` | Conserva almacenes/motivos y agrega `managedOrganizationalUnits` con las unidades activas asignadas al Production actual. |
| `GET /api/ppe-requests/eligible-employees?search=Pedro&pageNumber=1&pageSize=25` | PPERequester. Búsqueda por nombre/nómina, siempre restringida al usuario autenticado, empleados activos y unidades asignadas activas. Máximo 100 registros por página; el frontend pide 25. |
| `POST /api/ppe-requests` | Mismo contrato. Cambia solamente la resolución/validación del receptor y unidad para Production. |

La operación de administración verifica usuario existente, unidades existentes y activas, IDs positivos y sin duplicados mediante FluentValidation. Reemplaza únicamente relaciones agregadas/retiradas; repetir el mismo conjunto no crea duplicados. Usa `BeginSerializableTransactionAsync`, un `SaveChangesAsync`, commit y rollback. Controller y handler restringen la modificación a Administrator. El cambio de roles no elimina asignaciones.

Production carga al usuario actual desde BD, verifica que esté activo, resuelve al receptor y exige que esté activo y que su unidad principal esté entre las asignaciones activas del usuario. Independientemente, verifica que la unidad de consumo esté en ese mismo conjunto. Un empleado de L1 puede consumir en L2 si el encargado administra ambas. Las violaciones de alcance lanzan `ForbiddenException` (403 mediante el middleware existente).

Compatibilidad: omitir `EmployeeNumber` usa el empleado actualmente asociado al usuario en BD; omitir la unidad usa la principal del receptor resuelto, después de verificar alcance. No se infieren permisos a partir de esa adscripción: sin relación explícita, la solicitud se rechaza. El frontend nuevo envía ambos valores.

Administrator conserva las mismas reglas y su precedencia sobre Production. Su creación no consulta ni exige asignaciones. Warehouse y Viewer no adquieren permisos. No se cambió JWT ni políticas/roles globales.

Se eligió una consulta paginada de empleados porque no existe una cota de plantilla que haga seguro cargar todos en `form-options`. El filtro se aplica en SQL antes de contar/paginar y el DTO expone solamente identidad y adscripción, sin campos administrativos adicionales.

### Archivos creados

- `src/PPEInventory.Domain/Entities/UserManagedOrganizationalUnit.cs`
- `src/PPEInventory.Infrastructure/Persistence/Configurations/UserManagedOrganizationalUnitConfiguration.cs`
- `src/PPEInventory.Infrastructure/Persistence/Migrations/20261027144613_AddUserManagedOrganizationalUnits.cs`
- `src/PPEInventory.Infrastructure/Persistence/Migrations/20261027144613_AddUserManagedOrganizationalUnits.Designer.cs`
- `src/PPEInventory.Application/Features/Users/ManagedOrganizationalUnitDto.cs`
- `src/PPEInventory.Application/Features/Users/Commands/SetManagedOrganizationalUnits/SetUserManagedOrganizationalUnitsCommand.cs`
- `src/PPEInventory.Application/Features/Users/Commands/SetManagedOrganizationalUnits/SetUserManagedOrganizationalUnitsCommandHandler.cs`
- `src/PPEInventory.Application/Features/Users/Commands/SetManagedOrganizationalUnits/SetUserManagedOrganizationalUnitsCommandValidator.cs`
- `src/PPEInventory.Application/Features/Users/Commands/SetManagedOrganizationalUnits/SetUserManagedOrganizationalUnitsRequest.cs`
- `src/PPEInventory.Application/Features/Users/Queries/GetManagedOrganizationalUnits/GetUserManagedOrganizationalUnitsQuery.cs`
- `src/PPEInventory.Application/Features/Users/Queries/GetManagedOrganizationalUnits/GetUserManagedOrganizationalUnitsQueryHandler.cs`
- `src/PPEInventory.Application/Features/PPERequests/Queries/GetEligibleEmployees/GetEligiblePPERequestEmployeesQuery.cs`
- `src/PPEInventory.Application/Features/PPERequests/Queries/GetEligibleEmployees/GetEligiblePPERequestEmployeesQueryHandler.cs`
- `src/PPEInventory.Application/Features/PPERequests/Queries/GetEligibleEmployees/GetEligiblePPERequestEmployeesQueryValidator.cs`

### Archivos modificados por esta tarea

- `src/PPEInventory.Domain/Entities/User.cs`
- `src/PPEInventory.Infrastructure/Persistence/ApplicationDbContext.cs`
- `src/PPEInventory.Infrastructure/Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`
- `src/PPEInventory.Infrastructure/Repositories/UserRepository.cs`
- `src/PPEInventory.Infrastructure/Repositories/EmployeeRepository.cs`
- `src/PPEInventory.Application/Interfaces/IUserRepository.cs`
- `src/PPEInventory.Application/Interfaces/IEmployeeRepository.cs`
- `src/PPEInventory.Application/Features/PPERequests/Commands/Create/CreatePPERequestCommandHandler.cs`
- `src/PPEInventory.Application/Features/PPERequests/PPERequestFormOptionsDto.cs`
- `src/PPEInventory.Application/Features/PPERequests/Queries/GetFormOptions/GetPPERequestFormOptionsQueryHandler.cs`
- `src/PPEInventory.Api/Controllers/UsersController.cs`
- `src/PPEInventory.Api/Controllers/PPERequestsController.cs`

Otros cambios que aparecen en Git, entre ellos el historial de entregas, ya estaban presentes al iniciar y no son de esta implementación.

## Frontend

UsersPage ofrece “Líneas / áreas” a Administrator para cuentas Production. Abre un modal que reutiliza `CatalogFormModal` y el servicio del catálogo organizacional existente. Permite consultar, buscar, agregar/quitar y guardar asignaciones; muestra errores, carga, guardado y unidades inactivas asignadas. No crea ni edita unidades del catálogo. Guardar sin selección indica expresamente que revoca el alcance.

ProductionPPERequestForm incorpora búsqueda paginada y selección de empleado, su unidad habitual y la línea/área de consumo. Al elegir receptor preselecciona su unidad principal. Solo aparecen asignaciones activas; con una sola unidad se muestra como valor fijo y se envía explícitamente. Sin asignaciones no permite enviar. Se conserva la selección de almacén, motivo, productos, cantidades y notas. Solicitudes concurrentes de búsqueda descartan respuestas anteriores.

Archivos creados en `PPEInventory-FrontEnd`:

- `src/components/users/UserManagedOrganizationalUnitsModal.tsx`
- `src/hooks/useUserManagedOrganizationalUnits.ts`
- `src/hooks/useEligiblePPERequestEmployees.ts`

Archivos modificados:

- `src/pages/UsersPage.tsx`
- `src/components/ppe-requests/ProductionPPERequestForm.tsx`
- `src/api/services/UsersService.ts`
- `src/api/services/PPERequestsService.ts`
- `src/types/types.ts`

No se modificó DeliveriesPage ni el formulario administrativo de solicitudes. Los cambios previos de tipos y servicios se conservaron; antes de aplicar los archivos se verificaron hashes del estado local inspeccionado.

## Regresión y alcance de las pruebas

No había proyecto de pruebas ni framework de frontend. Se ejecutó un programa temporal sin framework ni dependencias nuevas, en `obj/managed-units-smoke`, que llama a los handlers reales con repositorios/transacciones simulados y valida el modelo EF sin conexión. Resultado: **34 comprobaciones aprobadas**. No equivale a integración HTTP, persistencia SQL, concurrencia real ni prueba de navegador.

| Escenario | Resultado y evidencia |
|---|---|
| A. Production existente, una línea | ✅ Validado en handler con asignación equivalente al backfill: campos omitidos, Pending y reserva de 2. El backfill real no se ejecutó. |
| B. Production con varias líneas | ✅ Validado: administra L1/L2/L4; empleado de L1, consumo L2, encargado adscrito a otra unidad. La entidad y respuesta mantienen L2. |
| C. Unidad fuera de alcance | ✅ Validado: L5 rechazada, sin solicitud ni reserva, rollback solicitado. |
| D. Empleado fuera de alcance | ✅ Validado: empleado de L5 rechazado. |
| E. Empleado inactivo | ✅ Validado: rechazado. También se verificaron usuario y unidades inactivos. |
| F. Administrator | ✅ Validado: solicita para empleado/unidad fuera de asignaciones, incluso sin asignaciones y con ambos roles; sigue exigiendo campos explícitos. |
| H. Normal | ✅ Validado: Pending y reserva inmediata. Stock insuficiente rechazado. |
| G. Excepcional | ✅ Validado por separado Damage, Lost y Other: PendingApproval sin reserva ni límites normales. |
| Aprobación | ✅ Validado: transición a Pending y reserva; mantiene L2 aunque cambien la adscripción del receptor y las asignaciones del creador. |
| Reserva | ✅ Validado en memoria: normal reserva al crear; excepcional al aprobar; entrega libera la reserva. No se probaron bloqueos/concurrencia SQL. |
| J. Entrega | ✅ Validado en handler con rol Warehouse: Delivered, OnHand de 10 a 8, Reserved a 0, movimiento de -2, sin parámetro de unidad. |
| I. Límite | ✅ Validado: path y argumento de consumo comprometido usan L2; aplica límite de L2, rechaza exceso y hereda límite del ancestro cuando falta el de L2. La consulta SQL del ciclo se revisó estáticamente y no se modificó. |
| K. Historial | ⚠️ No validado de extremo a extremo. El mapping del DTO tras entregar sí se probó con L2; consulta, filtros, listado y detalle se revisaron y siguen usando RequestedForOrganizationalUnit, no la adscripción del empleado. |
| L. Seguridad | ✅ Validado en handlers: unidad/empleado externo rechazados; Warehouse/Viewer no crean; Production/Warehouse/Viewer no administran asignaciones. Políticas HTTP revisadas estáticamente. |
| Administración | ✅ Validado en handlers/validador: reemplazo, repetición sin duplicados, vaciado, usuario/unidad inexistentes, unidad inactiva, IDs duplicados/null/no positivos, commit/rollback solicitados. |
| Opciones | ✅ Validado en handler: solo unidades asignadas activas. Búsqueda SQL/paginación y UI revisadas estáticamente. |

La lógica de `GetCommittedNormalQuantitiesInCycleAsync` se conserva: filtra por unidad guardada, excluye motivos excepcionales y cuenta pendientes más entregados dentro del ciclo vigente. No se alteraron aprobación, rechazo, cancelación, entrega ni movimientos de inventario. La cancelación sigue autorizando al creador, por lo que no requiere que creador y receptor sean el mismo empleado.

## Validaciones técnicas

- `dotnet build PPEInventory.sln --artifacts-path .artifacts/managed-units --no-restore -m:1`: compilación correcta, **0 errores**, después de los bloques de backend y migración. La restauración inicial con ese directorio se hizo sin `--no-restore`. Se usó salida aislada para evitar interferir con la API/Visual Studio.
- Advertencias NU1900 por consulta de vulnerabilidades de NuGet en las compilaciones; sin cambios de paquetes del producto. El programa temporal se restauró con `NuGetAudit=false`, sin modificar los proyectos.
- `dotnet test`: no aplica; no había proyectos de pruebas. No se creó framework ni se agregó proyecto a la solución. El programa temporal ejecutó 34 comprobaciones reales de handlers/modelo.
- `npm run build`: correcto, TypeScript y Vite sin errores.
- `npm run lint`: **falló con los mismos tres errores iniciales**, todos `react-hooks/set-state-in-effect`, en `ProductWarehouseAssignmentModal.tsx:189`, `useAuditLogs.ts:196`, `useRequestReasons.ts:67`. Ninguno se modificó.
- ESLint de los ocho archivos de esta tarea: **0 errores**.
- ESLint de los 18 archivos TypeScript existentes modificados/nuevos en el estado local completo, incluidos cambios previos: **0 errores**.
- `git diff --check`: correcto en ambos proyectos.
- EF `migrations has-pending-model-changes`: sin cambios pendientes.
- SQL de subida y bajada generado y revisado; **no ejecutado**. Archivos temporales `obj/managed-units-up.sql` y `obj/managed-units-down.sql`.
- Código/resultados temporales: `obj/managed-units-smoke/Program.cs`, `ManagedUnitsSmoke.csproj`, `results.txt`. Ejecución local: `dotnet .artifacts/managed-units/bin/ManagedUnitsSmoke/debug/ManagedUnitsSmoke.dll`.

## Riesgos y pendientes de integración

- La migración debe aplicarse mediante el procedimiento de despliegue habitual antes de servir el backend nuevo; sin la tabla, Production y los endpoints nuevos fallarán. Esta tarea no la aplica.
- Falta validar migración/backfill, guardado EF, consultas SQL, concurrencia y pantallas contra una base de pruebas. No se accedió a datos reales ni se iniciaron solicitudes de negocio.
- El backfill preserva la adscripción principal vigente al migrar. Usuarios Production nuevos requieren asignación explícita. Cambiar posteriormente la adscripción del empleado no cambia automáticamente las unidades que administra: son conceptos independientes.
- El formulario conserva el caché de opciones existente durante su montaje; tras una reasignación administrativa puede necesitar recargarse. El backend vuelve a verificar las asignaciones al crear y rechaza selecciones revocadas.
- Los límites continúan acumulándose por la unidad de consumo guardada. Solicitar para más receptores puede alcanzar antes el límite compartido de esa unidad; no se amplió ni cambió su cálculo.
- Una unidad inactivada después de crear una excepcional sigue bloqueando su aprobación, conforme a la regla existente. Reasignar responsables no cambia unidades de solicitudes ya creadas.

No hubo cambios de compras, proveedores, costos, conteos, ajustes, autenticación ni roles globales; tampoco commit, push o despliegue.
