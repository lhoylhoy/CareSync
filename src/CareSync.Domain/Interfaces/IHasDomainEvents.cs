using CareSync.Domain.Events;

namespace CareSync.Domain.Interfaces;

public interface IHasDomainEvents
{
    public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    public void ClearDomainEvents();
}
