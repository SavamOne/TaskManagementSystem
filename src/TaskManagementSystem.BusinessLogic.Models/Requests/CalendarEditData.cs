namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class CalendarEditData
{
    public CalendarEditData(Guid editorId, Guid calendarId, string? name, string? description)
    {
        EditorId = editorId;
        CalendarId = calendarId;
        Name = name;
        Description = description;
    }

    public Guid EditorId { get; }

    public Guid CalendarId { get; }
    
    public string? Name { get; }
    
    public string? Description { get; }
}