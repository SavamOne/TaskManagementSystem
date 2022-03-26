namespace TaskManagementSystem.Client.Helpers.Implementations;

public static class StringHelper
{
	public static string GetHexColorForText(string? text)
	{
		int hash = ( text ?? string.Empty ).Aggregate(0, (current, c) => c + ( ( current << 5 ) - current ));

		byte[] array =
		{
			(byte)( hash & 0xFF ),
			(byte)( ( hash >> 8 ) & 0xFF ),
			(byte)( ( hash >> 16 ) & 0xFF )
		};

		return "#" + Convert.ToHexString(array);
	}
}