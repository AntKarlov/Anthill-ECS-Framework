namespace Anthill.Utils
{
	using System;

	public static class AntEnum
	{
		/// <summary>
		/// Converts string into Enum value.
		/// </summary>
		/// <param name="aValue">String value for converting.</param>
		/// <typeparam name="T">Type of the Enum.</typeparam>
		/// <returns>Enum value.</returns>
		public static T Parse<T>(string aValue) where T : Enum
		{
			return (T) Enum.Parse(typeof(T), aValue);
		}

		/// <summary>
		/// Converts string into Enum value with default value.
		/// </summary>
		/// <param name="aValue">String value for converting.</param>
		/// <param name="aDefaultValue">Default value if convert is impossible.</param>
		/// <typeparam name="T">Type of the Enum.</typeparam>
		/// <returns>Enum value.</returns>
		public static T Parse<T>(string aValue, T aDefaultValue) where T : Enum
		{
			try
			{
				return Parse<T>(aValue);
			}
			catch
			{
				return aDefaultValue;
			}
		}

		/// <summary>
		/// Extracts all Enum values as array of the strings.
		/// </summary>
		/// <typeparam name="T">Type of the Enum.</typeparam>
		/// <returns>Array of the strings.</returns>
		public static string[] GetStringValues<T>() where T : Enum
		{
			var list = GetValues<T>();
			var result = new string[list.Length];
			for (int i = 0, n = list.Length; i < n; i++)
			{
				result[i] = list[i].ToString();
			}
			return result;
		}

		/// <summary>
		/// Extracts all Enum values as array.
		/// </summary>
		/// <typeparam name="T">Type of the Enum.</typeparam>
		/// <returns>Array of the enums.</returns>
		public static T[] GetValues<T>() where T : Enum
		{
			// if (typeof(T).BaseType != typeof(Enum))
			// {
			// 	throw new ArgumentException("T must be of type System.Enum");
			// }
			return (T[]) Enum.GetValues(typeof(T));
		}

		/// <summary>
		/// Extracts random value of Enum.
		/// </summary>
		/// <typeparam name="T">Type of the Enum.</typeparam>
		/// <returns>Random value of the enum.</returns>
		public static T GetRandomValue<T>() where T : Enum
		{
			// if (typeof(T).BaseType != typeof(Enum))
			// {
			// 	throw new ArgumentException("T must be of type System.Enum");
			// }
			var list = GetValues<T>();
			int index = AntRandom.Range(0, list.Length - 1);
			return list[index];
		}
	}
}