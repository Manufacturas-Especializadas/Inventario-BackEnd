namespace PPEInventory.Api.Models;

public record BootstrapAdminRequest(
    string EmployeeNumber,
    string Username,
    string Password,
    string BootstrapKey);