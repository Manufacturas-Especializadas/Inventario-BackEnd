# Corrección UTC de timestamps de PPERequest

Fecha: 2026-10-27. Cambio aplicado únicamente al código local, sobre los cambios existentes. Sin commit, push, despliegue, modificación de datos existentes ni aplicación de migraciones.

## Causa raíz y reproducción previa

Se rastrearon creación, aprobación, rechazo, cancelación, entrega, pendientes, detalle por folio y delivery-history. Todos los handlers generan sus timestamps desde `IDateTimeProvider.UtcNow`, cuya implementación utiliza `DateTime.UtcNow`.

`PPERequest` tiene cinco propiedades de instante: `CreatedAt`, `ApprovedAt`, `RejectedAt`, `DeliveredAt` y `CancelledAt`. Antes del cambio, su configuración EF no restauraba `DateTime.Kind` al leer los campos SQL `datetime2`. Los mappings y las proyecciones de `PPERequestRepository` copian esos valores directamente. La API usa `AddControllers()` y System.Text.Json sin conversor de fechas personalizado. Axios tampoco transforma las fechas recibidas.

Se reprodujo antes de modificar el código de producto con Microsoft.Data.SqlClient, SQL Server LocalDB y el ApplicationDbContext real:

```text
Input UTC:      2026-10-27T15:42:00.0000000Z
SQL roundtrip:  2026-10-27T15:42:00.0000000
Kind:          Unspecified
Ticks iguales: True
EF Kind:       Unspecified
```

JSON obtenido antes:

```json
{"createdAt":"2026-10-27T15:42:00","approvedAt":"2026-10-27T15:42:00","rejectedAt":"2026-10-27T15:42:00","deliveredAt":"2026-10-27T15:42:00","cancelledAt":"2026-10-27T15:42:00"}
```

JavaScript interpreta un ISO datetime sin zona como hora local. Por eso las 15:42 almacenadas como UTC se mostraban como 15:42 locales. El caso se reprodujo con la configuración local y datos sintéticos; no se consultó la base ni la API de producción.

## Cambio implementado

Único archivo de producto modificado en esta tarea:

`src/PPEInventory.Infrastructure/Persistence/Configurations/PPERequestConfiguration.cs`

Se aplica el mismo `ValueConverter<DateTime, DateTime>` a los cinco campos:

```csharp
var utcDateTimeConverter = new ValueConverter<DateTime, DateTime>(
    value => value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
```

La escritura conserva el valor. La lectura restaura únicamente la marca UTC, sin cambiar ticks ni desplazar horas. EF maneja `null` fuera del converter, por lo que la misma instancia sirve para `DateTime` y `DateTime?`; se comprobó con campos nulos y no nulos. Es el comportamiento documentado en [Microsoft Learn: Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions#specify-the-datetimekind-when-reading-dates).

No se cambió DateTimeProvider, entidades, DTOs, mappings, proyecciones, reglas de negocio ni filtros. No se aplicó un converter global.

JSON obtenido después, leyendo de SQL y serializando con las opciones web estándar:

```json
{"createdAt":"2026-10-27T15:42:00Z","approvedAt":"2026-10-27T15:42:00Z","rejectedAt":"2026-10-27T15:42:00Z","deliveredAt":"2026-10-27T15:42:00Z","cancelledAt":"2026-10-27T15:42:00Z"}
```

## Regresión ejecutada

Prueba equivalente al recorrido de datos de la API: SQL Server LocalDB → EF Core real → repositorio/mapping real → System.Text.Json con `JsonSerializerDefaults.Web`. Se usaron tablas temporales `#Utc_*` y datos sintéticos, con un interceptor de prueba que redirige únicamente los nombres de tabla. Las tablas desaparecen al cerrar la conexión. No se crearon tablas persistentes ni se ejecutaron migraciones.

| Caso | Resultado |
|---|---|
| Solicitud, escritura y lectura | ✅ `SaveChangesAsync` y lectura nueva: `CreatedAt` conserva 15:42 y tiene Kind UTC. |
| Eventos aún no ocurridos | ✅ ApprovedAt, RejectedAt, DeliveredAt y CancelledAt conservan `null`. |
| Valores SQL de los cinco campos | ✅ Lectura directa con SqlClient confirma que todos siguen en 15:42; no hay desplazamiento en escritura. |
| Pending | ✅ `GetPendingAsync` real proyecta CreatedAt y ApprovedAt y ambos se serializan con Z. |
| Detalle por folio | ✅ `GetByFolioAsync` y `ToDto()` reales conservan UTC en CreatedAt, ApprovedAt, DeliveredAt y CancelledAt. |
| Aprobación | ✅ ApprovedAt recuperado de SQL tiene UTC; también se verificó la proyección de pendientes de aprobación. |
| Entrega | ✅ DeliveredAt recuperado tiene UTC, tanto en entidad como en DTO. |
| Historial de entregas | ✅ `GetDeliveryHistoryAsync` real devuelve CreatedAt y DeliveredAt con Z, incluyendo la proyección `DeliveredAt.Value`. |
| Cancelación | ✅ CancelledAt recuperado de SQL y serializado con Z. |
| Rechazo | ✅ RejectedAt recuperado de SQL y serializado con Z mediante el DTO de resultado de rechazo. |
| Lectura para actualizar | ✅ `GetByFolioForUpdateAsync`, que utiliza FromSql, también restaura UTC. |

Estas pruebas verifican persistencia y serialización de fechas. No simulan nuevas aprobaciones, reservas o entregas de negocio en producción ni equivalen a interacción manual con Swagger/navegador. Los handlers de eventos se revisaron y permanecen usando UtcNow. El DTO general de detalle ya omitía RejectedAt; se mantiene ese contrato, y la prueba cubre la entidad y el DTO de rechazo que sí lo expone.

Se consumió el JSON obtenido en JavaScript/Intl usando el mismo patrón del frontend (`new Date(value)` y DateTimeFormat sin opción timeZone). Se ejecutaron procesos de prueba con zonas locales distintas:

| Zona del proceso de prueba | Resultado para los cinco campos |
|---|---|
| America/Mexico_City | ✅ 27 oct 2026, 9:42 a.m. |
| Europe/Madrid | ✅ 27 oct 2026, 4:42 p.m. |
| Asia/Tokyo | ✅ 28 oct 2026, 12:42 a.m. |

No se cambió la zona del equipo, App Service ni frontend. Las zonas se establecieron exclusivamente en el entorno de los procesos de prueba.

## Build, frontend y migración

- `dotnet build PPEInventory.sln --artifacts-path .artifacts/managed-units --no-restore -m:1`: **0 errores**. Una advertencia NU1900 de acceso al índice de vulnerabilidades de NuGet, ajena al cambio.
- `dotnet ef migrations has-pending-model-changes --project src/PPEInventory.Infrastructure --startup-project src/PPEInventory.Api --no-build`, apuntando a esos artefactos: **No changes have been made to the model since the last migration.**
- No se necesita migración: SQL type, nulabilidad y estructura física no cambian. No se creó migración vacía ni se modificó el snapshot. La migración de unidades administradas que ya existía permanece independiente y sin aplicar.
- Frontend: **sin modificaciones en esta tarea**. Sus formateadores ya son correctos. `npm run build` no se repitió porque no hubo cambios frontend.
- `git diff --check`: correcto.

Artefactos temporales de comprobación, fuera del código de producto:

- `obj/utc-regression/Program.cs` y `UtcRegression.csproj`.
- `obj/utc-regression/verified-timestamps.json`.
- `obj/utc-regression/check-local-time.cjs`.
- `obj/utc-regression/check-date-range.cjs`.

## Filtro por rango de fechas: segundo problema, sin cambios

`PPERequestRepository.GetDeliveryHistoryAsync` usa `DateOnly.ToDateTime(TimeOnly.MinValue)` y `DateTime.SpecifyKind(..., Utc)` para el inicio y el día siguiente exclusivo. Esto etiqueta medianoche como UTC; no convierte una medianoche de negocio local a UTC.

Para consultar el 27 de octubre de 2026:

```text
Rango actual:      [2026-10-27T00:00Z, 2026-10-28T00:00Z)
En Ciudad México: [2026-10-26 18:00,   2026-10-27 18:00)

Día local completo en America/Mexico_City:
                  [2026-10-27T06:00Z, 2026-10-28T06:00Z)
```

La UI genera sus fechas predeterminadas con el calendario local del navegador, por lo que, si el usuario espera el día local completo, puede incluir entregas de la tarde anterior y excluir entregas de la tarde seleccionada. Los filtros DateFrom/DateTo de pending usan el mismo patrón.

No se modificaron. Hace falta decidir si el día de negocio corresponde a la planta/almacén o a la zona del usuario y expresar esa zona en el contrato/configuración. Una corrección posterior debe usar una zona geográfica y sus reglas, no sumar/restar seis horas fijas.

## Riesgos y otros timestamps

La corrección presupone que los valores persistidos de PPERequest representan UTC, como indican todos los escritores actuales revisados. No se auditaron importaciones ni datos históricos de producción; si alguna carga externa introdujo hora local, etiquetarla UTC no corrige esa carga.

Otros campos generados con UtcNow y sin converter de fecha podrían presentar el mismo problema: `User.CreatedAt`, `UpdatedAt`, `LastLoginAt`; `InventoryMovement.CreatedAt`; `AuditLog.CreatedAt`; `InventoryAdjustment.CreatedAt`; `InventoryCount.CreatedAt` y `SubmittedAt`, además de timestamps de mantenimiento de catálogos. Solo se identificaron estáticamente, sin modificación ni prueba de roundtrip específica.

Campos como `PurchaseOrder.OrderDate` o `ConfirmedDeliveryDate` requieren análisis independiente de su semántica; esta tarea no los trata como instantes UTC.
