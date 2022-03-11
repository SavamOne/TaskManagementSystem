using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class CalendarInfo
{
    public CalendarInfo(Guid id, string name, string description, DateTimeOffset creationDate)
    {
        Id = id;
        Name = name.AssertNotNullOrWhiteSpace();
        Description = description.AssertNotNullOrWhiteSpace();
        CreationDate = creationDate;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateTimeOffset CreationDate { get; }
}