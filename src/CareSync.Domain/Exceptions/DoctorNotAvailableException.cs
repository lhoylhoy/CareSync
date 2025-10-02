namespace CareSync.Domain.Exceptions;

public class DoctorNotAvailableException : DomainException
{
    public DoctorNotAvailableException(string message) : base(message)
    {
    }
}
