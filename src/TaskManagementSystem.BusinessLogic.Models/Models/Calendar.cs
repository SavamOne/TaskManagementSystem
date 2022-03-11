using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record Calendar
{
    public Calendar(Guid id, string name, string description, DateTime creationDateUtc)
    {
        Id = id;
        Name = name.AssertNotNullOrWhiteSpace();
        Description = description.AssertNotNullOrWhiteSpace();
        CreationDateUtc = creationDateUtc;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateTime CreationDateUtc { get; }
}