using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public EfUnitOfWork(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IAppTransaction>
        BeginSerializableTransactionAsync(
            CancellationToken cancellationToken = default)
    {
        var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        return new EfAppTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(
                "The inventory was modified by another operation. Please retry.",
                ex);
        }
    }

    private sealed class EfAppTransaction
        : IAppTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfAppTransaction(
            IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return _transaction.CommitAsync(
                cancellationToken);
        }

        public Task RollbackAsync(
            CancellationToken cancellationToken = default)
        {
            return _transaction.RollbackAsync(
                cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }
}