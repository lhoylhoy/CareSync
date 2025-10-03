using CareSync.Domain.Events;

namespace CareSync.Domain.Interfaces;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
}
