namespace TaskManagementSystem.Shared.Helpers;

public static class CollectionExtensions
{
	public static void ClearIfPossible<T>(this ICollection<T> collection)
	{
		if (collection.IsReadOnly || !collection.Any())
		{
			return;
		}

		collection.Clear();
	}
}