using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManagementSystem.Shared.Models.Options;

public static class ApplicationJsonOptions
{
	public static readonly JavaScriptEncoder Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

	public static readonly JsonIgnoreCondition DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

	public static readonly JsonNamingPolicy PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

	public static readonly bool AllowTrailingCommas = true;

	public static readonly bool PropertyNameCaseInsensitive = true;

	public static IList<JsonConverter> Converters = new List<JsonConverter>
	{
		new JsonStringEnumConverter()
	};

	public static readonly JsonSerializerOptions Options = new()
	{
		Encoder = Encoder,
		AllowTrailingCommas = AllowTrailingCommas,
		PropertyNameCaseInsensitive = PropertyNameCaseInsensitive,
		PropertyNamingPolicy = PropertyNamingPolicy,
		DefaultIgnoreCondition = DefaultIgnoreCondition,
		Converters =
		{
			new JsonStringEnumConverter()
		}
	};
}