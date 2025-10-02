namespace CareSync.Domain.Events;

using System.ComponentModel.DataAnnotations.Schema;

// EF Core was attempting to map DomainEvent (and its derived records) as entities because
// they are exposed via aggregate navigation properties (IReadOnlyCollection<DomainEvent> DomainEvents).
// Marking the abstract base with NotMapped prevents EF from treating these as entity types.
[NotMapped]
public abstract record DomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
