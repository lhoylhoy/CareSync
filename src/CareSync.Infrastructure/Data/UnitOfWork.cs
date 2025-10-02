using CareSync.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Data;

public class UnitOfWork(CareSyncDbContext context, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Capture domain events before saving (only for aggregates implementing IHasDomainEvents)
        var domainEventHolders = context.ChangeTracker.Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await context.SaveChangesAsync(cancellationToken);

        if (domainEventHolders.Count > 0)
        {
            var events = domainEventHolders.SelectMany(e => e.DomainEvents).ToList();
            await dispatcher.DispatchAsync(events, cancellationToken);
            domainEventHolders.ForEach(e => e.ClearDomainEvents());
        }

        return result;
    }
}
