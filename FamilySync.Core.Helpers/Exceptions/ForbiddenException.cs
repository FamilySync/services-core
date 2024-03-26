namespace FamilySync.Core.Helpers.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message)
        : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ForbiddenException(Type type, long id)
        : base($"Forbidden action for {type.Name} with id {id}")
    {
    }

    public ForbiddenException(Type type, string id)
        : base($"Forbidden action for {type.Name} with id {id}")
    {
    }

    public ForbiddenException(Type entityType, Type idType, string id)
        : base($"Forbidden action for {entityType.Name} for type {idType.Name} with id {id}")
    {
    }

    public ForbiddenException(Type entityType, Type idType, long id)
        : base($"Forbidden action for {entityType.Name} for type {idType.Name} with id {id}")
    {
    }
}