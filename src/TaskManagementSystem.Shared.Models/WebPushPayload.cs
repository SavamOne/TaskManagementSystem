namespace TaskManagementSystem.Shared.Models;

public class WebPushPayload
{
	public WebPushPayload(string message, string url)
	{
		Message = message;
		Url = url;
	}

	public string Message { get; }

	public string Url { get; }
}