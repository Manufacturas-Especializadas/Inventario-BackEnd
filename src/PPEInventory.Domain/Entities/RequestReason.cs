namespace PPEInventory.Domain.Entities;

public class RequestReason
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<PPERequest> PPERequests { get; set; }
        = new List<PPERequest>();
}