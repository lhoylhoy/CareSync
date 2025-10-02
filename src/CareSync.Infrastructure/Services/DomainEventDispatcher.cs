using CareSync.Domain.Events;
using CareSync.Domain.Interfaces;

namespace CareSync.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    // TODO: Integrate with MediatR or message bus later (Outbox pattern)
    public Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
    {
        // Intentionally noop for now
        return Task.CompletedTask;
    }
}
