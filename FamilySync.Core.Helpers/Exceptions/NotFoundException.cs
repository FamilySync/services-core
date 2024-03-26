namespace FamilySync.Core.Helpers.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(Type type, long id)
        : base($"Could not find entity of type {type.Name} with id {id}")
    {
    }

    public NotFoundException(Type type, string id)
        : base($"Could not find entity of type {type.Name} with id {id}")
    {
    }

    public NotFoundException(Type entityType, Type idType, string id)
        : base($"Could not find entity of type {entityType.Name} for type {idType.Name} with id {id}")
    {
    }

    public NotFoundException(Type entityType, Type idType, long id)
        : base($"Could not find entity of type {entityType.Name} for type {idType.Name} with id {id}")
    {
    }
}