namespace Anthill.Extensions
{
	using System;
	using System.Globalization;
	using System.Text;
	using UnityEngine;

	public static class StringExtension
	{
		private static readonly StringBuilder _stringBuilder = new StringBuilder();

		#region Public Methods

		/// <summary>
		/// Returns TRUE if the string is null or empty.
		/// </summary>
		/// <param name="trimSpaces">If TRUE (default) and the string contains only spaces, considers it empty</param>
		public static bool IsNullOrEmpty(this string aStr, bool aTrimSpaces = true)
		{
			if (aStr == null)
			{
				return true;
			}

			return (aTrimSpaces) 
				? (aStr.Trim().Length == 0)
				: (aStr.Length == 0);
		}

		/// <summary>
		/// Converts a HEX color to a Unity Color and returns it.
		/// </summary>
		/// <param name="hex">The HEX color, either with or without the initial # 
		/// (accepts both regular and short format).</param>
		public static Color HexToColor(this string aHex)
		{
			if (aHex[0] == '#')
			{
				aHex = aHex.Substring(1);
			}

			int len = aHex.Length;
			bool isShortFormat = (len < 6);
			if (isShortFormat)
			{
				float r = (HexToInt(aHex[0]) + HexToInt(aHex[0]) * 16f) / 255f;
				float g = (HexToInt(aHex[1]) + HexToInt(aHex[1]) * 16f) / 255f;
				float b = (HexToInt(aHex[2]) + HexToInt(aHex[2]) * 16f) / 255f;
				float a = len == 4 ? (HexToInt(aHex[3]) + HexToInt(aHex[3]) * 16f) / 255f : 1;
				return new Color(r, g, b, a);
			}
			else
			{
				float r = (HexToInt(aHex[1]) + HexToInt(aHex[0]) * 16f) / 255f;
				float g = (HexToInt(aHex[3]) + HexToInt(aHex[2]) * 16f) / 255f;
				float b = (HexToInt(aHex[5]) + HexToInt(aHex[4]) * 16f) / 255f;
				float a = len == 8 ? (HexToInt(aHex[7]) + HexToInt(aHex[6]) * 16f) / 255f : 1;
				return new Color(r, g, b, a);
			}
		}

		/// <summary>
		/// Nicifies a string, replacing underscores with spaces, and adding a space before Uppercase letters 
		/// (except the first character).
		/// </summary>
		public static string Nicify(this string aStr)
		{
			if (string.IsNullOrEmpty(aStr))
			{
				return "";
			}

			_stringBuilder.Length = 0;
			_stringBuilder.Append(aStr[0].ToString().ToUpper());
			for (int i = 1, n = aStr.Length; i < n; i++)
			{
				char curr = aStr[i];
				char prev = aStr[i - 1];
				if (curr == '_')
				{
					// Replace underscores with spaces
					_stringBuilder.Append(' ');
				}
				else
				{
					// Add spaces before numbers and uppercase letters.
					if (curr != ' ' && 
						(char.IsUpper(curr) && (prev != ' ' && prev != '_') || 
						char.IsNumber(curr) && prev != ' ' && prev != '_' && 
						!char.IsNumber(prev)))
					{
						_stringBuilder.Append(' ');
					}
					_stringBuilder.Append(curr);
				}
			}
			return _stringBuilder.ToString();
		}

		/// <summary>
		/// Converts the string to the given enum value.
		/// Throws an exception if the string can't be converted.
		/// If the enum value can't be found, returns the 0 indexed value.
		/// NOTE: doesn't use try/catch (TryParse) since on some platforms that won't work.
		/// </summary>
		public static T ToEnum<T>(this string aStr, T? aDefaultValue = null) where T : struct, IConvertible
		{
			Type tType = typeof(T);
			if (!tType.IsEnum)
			{
				throw new ArgumentException("T must be of type Enum.");
			}

			if (Enum.IsDefined(tType, aStr))
			{
				return (T) Enum.Parse(tType, aStr);
			}

			if (aDefaultValue == null)
			{
				throw new ArgumentException("Value not found and defaultValue not set.");
			}

			return (T) aDefaultValue;
		}

		/// <summary>
		/// Converts the given string to a float and returns it.
		/// Returns <code>defaultValue</code> if the conversion fails or the string is empty.
		/// </summary>
		/// <param name="defaultValue">Default value to return if conversion fails.</param>
		public static float ToFloat(this string aStr, float aDefaultValue = 0)
		{
			if (string.IsNullOrEmpty(aStr))
			{
				return aDefaultValue;
			}

			float result;
			float.TryParse(aStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out result);
			return result;
		}

		/// <summary>
		/// Compares a version string (in format #.#.###) with another of the same format,
		/// and return TRUE if this one is minor. Boths trings must have the same number of dot separators.
		/// </summary>
		public static bool VersionIsMinorThan(this string aStr, string aVersion)
		{
			string[] thisV = aStr.Split('.');
			string[] otherV = aVersion.Split('.');
			if (thisV.Length != otherV.Length)
			{
				throw new ArgumentException("Invalid");
			}

			for (int i = 0, n = thisV.Length; i < n; ++i)
			{
				int thisInt = Convert.ToInt32(thisV[i]);
				int otherInt = Convert.ToInt32(otherV[i]);
				if (i == thisV.Length - 1)
				{
					return thisInt < otherInt;
				}
				else if (thisInt == otherInt)
				{
					continue;
				}
				else if (thisInt < otherInt)
				{
					return true;
				}
				else if (thisInt > otherInt)
				{
					return false;
				}
			}

			throw new ArgumentException("Invalid");
		}

		#endregion

		#region Private Methods

		private static int HexToInt(char aHexValue)
		{
			return int.Parse(aHexValue.ToString(), System.Globalization.NumberStyles.HexNumber);
		}

		#endregion
	}
}