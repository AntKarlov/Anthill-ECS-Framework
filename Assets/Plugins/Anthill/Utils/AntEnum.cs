namespace Anthill.Utils
{
	using System;

// #if UNITY_EDITOR
// 	using UnityEditor;
// #endif

	public static class AntEnum
	{
		public static T Parse<T>(string aValue)
		{
			return (T) Enum.Parse(typeof(T), aValue);
		}

		public static T Parse<T>(string aValue, T aDefaultValue)
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

		public static string[] GetStringValues<T>()
		{
			var list = GetValues<T>();
			var result = new string[list.Length];
			for (int i = 0, n = list.Length; i < n; i++)
			{
				result[i] = list[i].ToString();
			}
			return result;
		}

		public static T[] GetValues<T>()
		{
			if (typeof(T).BaseType != typeof(Enum))
			{
				throw new ArgumentException("T must be of type System.Enum");
			}
			return (T[]) Enum.GetValues(typeof(T));
		}

		public static T GetRandomValue<T>()
		{
			if (typeof(T).BaseType != typeof(Enum))
			{
				throw new ArgumentException("T must be of type System.Enum");
			}

			var list = GetValues<T>();
			int index = AntMath.RandomRangeInt(0, list.Length - 1);
			return list[index];
		}

// #if UNITY_EDITOR
// 		public static string EnumField<T>(string aValue, string aDefValue)
// 		{
// 			if (typeof(T).BaseType == typeof(Enum))
// 			{
// 				string[] options = AntEnum.GetStringValues<T>();
// 				int index = Array.IndexOf(options, aValue);
// 				if (index >= 0 && index < options.Length)
// 				{
// 					index = EditorGUILayout.Popup(index, options);
// 					return AntEnum.Parse<T>(options[index]).ToString();
// 				}
// 			}
// 			return aDefValue;
// 		}
// #endif
	}
}