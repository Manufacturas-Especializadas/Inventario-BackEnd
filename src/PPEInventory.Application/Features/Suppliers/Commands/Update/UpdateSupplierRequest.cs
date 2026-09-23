namespace PPEInventory.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierRequest
{
    public string Name { get; set; } = string.Empty;

    public string? ContactName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
}