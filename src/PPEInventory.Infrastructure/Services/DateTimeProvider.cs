using PPEInventory.Application.Interfaces;

namespace PPEInventory.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}