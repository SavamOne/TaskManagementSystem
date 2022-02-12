using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class Toast
{
    public Toast(string author, string description, string text)
    {
        Id = Guid.NewGuid();
        Author = author.AssertNotNullOrWhiteSpace();
        Description = description.AssertNotNullOrWhiteSpace();
        Text = text.AssertNotNullOrWhiteSpace();
    }
    
    public Guid Id { get; }

    public string Author { get; }
    
    public string Description { get; }
    
    public string Text { get; }
}