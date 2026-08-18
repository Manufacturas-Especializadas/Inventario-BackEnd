namespace PPEInventory.Application.Common.Models;

public class PaginationParameters
{
    public const int DefaultPageSize = 25;

    public const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;
}