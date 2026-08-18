namespace PPEInventory.Application.Interfaces;

public interface IUnitOfWork
{
    Task<IAppTransaction> BeginSerializableTransactionAsync(
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}