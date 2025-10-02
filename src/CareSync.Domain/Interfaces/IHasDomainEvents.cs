using CareSync.Domain.Events;

namespace CareSync.Domain.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
