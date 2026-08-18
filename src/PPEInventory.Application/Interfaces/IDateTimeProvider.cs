namespace PPEInventory.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}