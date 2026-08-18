namespace PPEInventory.Application.Common.Models;

public record JwtTokenResult(
    string Token,
    DateTime ExpiresAt);