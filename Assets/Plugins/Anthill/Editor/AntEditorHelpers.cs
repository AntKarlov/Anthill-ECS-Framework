namespace Anthill.Editor
{
	using System.Reflection;
	using UnityEditorInternal;

	public static class AntEditorHelpers
	{
		/// <summary>
		/// Retuns array of sorting layers in the editor.
		/// </summary>
		/// <returns>Array of sorting layers.</returns>
		public static string[] GetSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty(
				"sortingLayerNames",
				BindingFlags.Static | BindingFlags.NonPublic
			);
			return (string[]) sortingLayersProperty.GetValue(null, new object[0]);
		}

		/// <summary>
		/// Returns of the indexes of sorting layers in the editor.
		/// </summary>
		/// <returns>Array of indexes of sorting layers.</returns>
		public static int[] GetSortingLayerUniqueIDs()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty(
				"sortingLayerUniqueIDs",
				BindingFlags.Static | BindingFlags.NonPublic
			);
			return (int[]) sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}
	}
}