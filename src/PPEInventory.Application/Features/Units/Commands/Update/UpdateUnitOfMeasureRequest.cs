namespace PPEInventory.Application.Features.Units.Commands.Update;

public class UpdateUnitOfMeasureRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Symbol { get; set; }
}