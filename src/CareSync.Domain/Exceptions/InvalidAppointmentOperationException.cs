namespace CareSync.Domain.Exceptions;

public class InvalidAppointmentOperationException : DomainException
{
    public InvalidAppointmentOperationException(string message) : base(message)
    {
    }
}
