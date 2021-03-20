namespace Anthill.Extensions
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using UnityEngine;
	// using Object = UnityEngine.Object;

	public static class StringExtension
	{
	#region Private Variables

		private static readonly StringBuilder _stringBuilder = new StringBuilder();

	#endregion
	
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
        /// If the given string is a directory path, returns its parent
        /// with or without final slash depending on the original directory format.
        /// </summary>
        public static string Parent(this string aDir)
        {
            if (aDir.Length <= 1)
			{
				return aDir;
			}

            string slashType = aDir.IndexOf("/") == -1 ? "\\" : "/";
            int index = aDir.LastIndexOf(slashType);
            if (index == -1)
			{
				// Not a directory path.
				return aDir; 
			}

            if (index == aDir.Length - 1)
			{
                // Had final slash
                index = aDir.LastIndexOf(slashType, index - 1);
                return (index == -1) ? aDir : aDir.Substring(0, index + 1);
            }

            // No final slash
            return aDir.Substring(0, index);
        }

        /// <summary>
        /// If the string is a directory, returns the directory name,
        /// if instead it's a file returns its name without extension.
        /// </summary>
        public static string FileOrDirectoryName(this string aPath)
        {
            if (aPath.Length <= 1) return aPath;
            int slashIndex = aPath.LastIndexOfAnySlash();
            int dotIndex = aPath.LastIndexOf('.');
            if (dotIndex != -1 && dotIndex > slashIndex)
			{
				// Remove extension if present.
				aPath = aPath.Substring(0, dotIndex); 
			}

            if (slashIndex == -1)
			{
				return aPath;
			}

            if (slashIndex == aPath.Length - 1)
			{
				// Remove final slash.
                aPath = aPath.Substring(0, slashIndex); 
                slashIndex = aPath.LastIndexOfAnySlash();
                if (slashIndex == -1)
				{
					return aPath;
				}
            }

            return aPath.Substring(slashIndex + 1);
        }

        /// <summary>
        /// Evaluates the string as a property or field and returns its value.
        /// </summary>
        /// <param name="aObj">If NULL considers the string as a static property, 
		/// otherwise uses obj as the starting instance.</param>
        public static T EvalAsProperty<T>(this string aStr, object aObj = null, bool aLogErrors = false)
        {
            try
			{
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                string[] split = aStr.Split('.');
                if (aObj == null)
				{
                    // Static.
                    string typeS = split[0];
                    for (int i = 1, n = split.Length - 1; i < n; ++i)
					{
						typeS += '.' + split[i];
					}

                    Type t = null;
                    for (int i = 0, n = assemblies.Length; i < n; ++i)
					{
                        t = assemblies[i].GetType(typeS);
                        if (t != null)
						{
							break;
						}
                    }

                    if (t == null)
					{
						throw new NullReferenceException($"Type {typeS} could not be found in any of the domain assemblies");
					}

                    PropertyInfo pInfo = t.GetProperty(split[split.Length - 1]);
                    if (pInfo != null)
					{
                        return (T)pInfo.GetValue(
                            null, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                            null, null, CultureInfo.InvariantCulture
                        );
                    }
					else
					{
                        return (T)t.GetField(split[split.Length - 1]).GetValue(null);
                    }
                }
				else
				{    
                    foreach (string part in split)
					{
                        Type t = aObj.GetType();
                        PropertyInfo pInfo = t.GetProperty(part);
                        if (pInfo != null)
						{
							aObj = pInfo.GetValue(aObj, null);
						}
                        else
						{
							aObj = t.GetField(part).GetValue(aObj);
						}
                    }
                    return (T) aObj;
                }
            }
			catch (Exception aException)
			{
                if (aLogErrors)
				{
					A.Error($"EvalAsProperty error ({aException.Message})\n{aException.StackTrace}", "StringExtension");
				}
                return default(T);
            }
        }

	#endregion

	#region Private Methods

		private static int HexToInt(char aHexValue)
		{
			return int.Parse(aHexValue.ToString(), System.Globalization.NumberStyles.HexNumber);
		}

		/// <summary>
		/// Returns the last index of any slash occurrence, either \ or /.
		/// </summary>
        static int LastIndexOfAnySlash(this string aStr)
        {
            int index = aStr.LastIndexOf('/');
            return (index == -1)
				? aStr.LastIndexOf('\\')
				: index;
        }

	#endregion
	}
}