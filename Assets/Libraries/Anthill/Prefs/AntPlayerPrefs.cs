namespace Anthill.Prefs
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Globalization;
	using UnityEngine;
	using Anthill.Extensions;

	public static class AntPlayerPrefs
	{
	#region Public Methods

		/// <summary>
		/// Saves data to the player prefs from the object fields marked as PlayerPrefs attributes.
		/// </summary>
		/// <param name="aObject">Object for serialization.</param>
		/// <typeparam name="T">Type of the object.</typeparam>
		public static void Serialize<T>(object aObject)
		{
			Type t = typeof(T);
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var attr = (PlayerPrefsAttribute) field.GetCustomAttributes(typeof(PlayerPrefsAttribute), true)
					.FirstOrDefault();
				
				if (attr != null)
				{
					FieldInfo f = t.GetField(field.Name);

					// Support of string.
					// ------------------
					if (f.FieldType == typeof(string)) 
					{
						PlayerPrefs.SetString(attr.Key, (string) field.GetValue(aObject));
					}

					// Support of int.
					// ---------------
					else if (f.FieldType == typeof(Int32))
					{
						PlayerPrefs.SetInt(attr.Key, (int) field.GetValue(aObject));
					}

					// Support of float.
					// -----------------
					else if (f.FieldType == typeof(Single))
					{
						PlayerPrefs.SetFloat(attr.Key, (float) field.GetValue(aObject));
					}

					// Support of bool.
					// ----------------
					else if (f.FieldType == typeof(Boolean))
					{
						PlayerPrefs.SetInt(attr.Key, ((bool) field.GetValue(aObject)) ? 1 : 0);
					}

					// Support of Vector2.
					// -------------------
					else if (f.FieldType == typeof(Vector2))
					{
						PlayerPrefs.SetString(attr.Key, Vec2ToStr((Vector2) field.GetValue(aObject)));
					}

					// Support of Vector3.
					// -------------------
					else if (f.FieldType == typeof(Vector3))
					{
						PlayerPrefs.SetString(attr.Key, Vec3ToStr((Vector3) field.GetValue(aObject)));
					}

					// Support of Quaternion.
					// ----------------------
					else if (f.FieldType == typeof(Quaternion))
					{
						PlayerPrefs.SetString(attr.Key, QuatToStr((Quaternion) field.GetValue(aObject)));
					}

					// Support of Color.
					// -----------------
					else if (f.FieldType == typeof(Color))
					{
						PlayerPrefs.SetString(attr.Key, ((Color) field.GetValue(aObject)).ToHex());
					}

					// Support of Enum.
					// ----------------
					else if (f.FieldType.BaseType == typeof(Enum))
					{
						PlayerPrefs.SetString(attr.Key, ((Enum) field.GetValue(aObject)).ToString());
					}

					// Support of float[].
					// -------------------
					else if (f.FieldType == typeof(float[]))
					{
						float[] values = (float[]) field.GetValue(aObject);
						PlayerPrefs.SetString(attr.Key, string.Join(" ", values));
					}

					// Support of int[].
					// -----------------
					else if (f.FieldType == typeof(int[]))
					{
						int[] values = (int[]) field.GetValue(aObject);
						PlayerPrefs.SetString(attr.Key, string.Join(" ", values));
					}
				}
			}
		}

		/// <summary>
		/// Loads data from the player prefs to the object fields marked as PlayerPrefs attributes.
		/// </summary>
		/// <param name="aObject">Object for deserialization.</param>
		/// <typeparam name="T">Type of the object.</typeparam>
		public static void Deserialize<T>(object aObject)
		{
			Type t = typeof(T);
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var attr = (PlayerPrefsAttribute) field.GetCustomAttributes(typeof(PlayerPrefsAttribute), true)
					.FirstOrDefault();

				if (attr != null)
				{
					FieldInfo f = t.GetField(field.Name);

					// Support of string.
					// ------------------
					if (f.FieldType == typeof(string))
					{
						field.SetValue(aObject, PlayerPrefs.GetString(attr.Key, (string) field.GetValue(aObject)));
					}

					// Support of int.
					// ---------------
					else if (f.FieldType == typeof(Int32))
					{
						field.SetValue(aObject, PlayerPrefs.GetInt(attr.Key, (int) field.GetValue(aObject)));
					}

					// Support of float.
					// -----------------
					else if (f.FieldType == typeof(Single))
					{
						field.SetValue(aObject, PlayerPrefs.GetFloat(attr.Key, (float) field.GetValue(aObject)));
					}

					// Support of bool.
					// ----------------
					else if (f.FieldType == typeof(Boolean))
					{
						field.SetValue(aObject, PlayerPrefs.GetInt(attr.Key, (bool) field.GetValue(aObject) ? 1 : 0) > 0);
					}

					// Support of Vector2.
					// -------------------
					else if (f.FieldType == typeof(Vector2))
					{
						field.SetValue(
							aObject, 
							StrToVec2(PlayerPrefs.GetString(attr.Key, Vec2ToStr((Vector2) field.GetValue(aObject))))
						);
					}

					// Support Vector3.
					// ----------------
					else if (f.FieldType == typeof(Vector3)) 
					{
						field.SetValue(
							aObject, 
							StrToVec3(PlayerPrefs.GetString(attr.Key, Vec3ToStr((Vector3) field.GetValue(aObject))))
						);
					}

					// Support of Quaternion.
					// ----------------------
					else if (f.FieldType == typeof(Quaternion)) 
					{
						field.SetValue(
							aObject, 
							StrToQuat(PlayerPrefs.GetString(attr.Key, QuatToStr((Quaternion) field.GetValue(aObject))))
						);
					}

					// Support of Color.
					// -----------------
					else if (f.FieldType == typeof(Color)) 
					{
						field.SetValue(
							aObject, 
							PlayerPrefs.GetString(attr.Key, ((Color) field.GetValue(aObject)).ToHex()).ToColor()
						);
					}

					// Support of enum.
					// ----------------
					else if (f.FieldType.BaseType == typeof(Enum))
					{
						try
						{
							var eStr = PlayerPrefs.GetString(attr.Key, ((Enum) field.GetValue(aObject)).ToString());
							field.SetValue(aObject, Enum.Parse(f.FieldType, eStr));
						}
						catch (Exception)
						{
							// ..
						}
					}

					// Support of string[].
					// --------------------
					else if (f.FieldType == typeof(string[]))
					{
						var defaultValues = (string[]) field.GetValue(aObject);
						var value = PlayerPrefs.GetString(attr.Key, string.Join(" ", defaultValues));
						field.SetValue(aObject, value.Split('|'));
					}

					// Support of int[].
					// -----------------
					else if (f.FieldType == typeof(int[])) // int[]
					{
						// Default value.
						var defaultValues = (int[]) field.GetValue(aObject);
						var value = PlayerPrefs.GetString(attr.Key, string.Join(" ", defaultValues));

						// Parse read value.
						var strValues = value.Split(' ');
						var intValues = new int[strValues.Length];
						int v = 0;
						for (int i = 0, n = strValues.Length; i < n; i++)
						{
							intValues[i] = (Int32.TryParse(strValues[i], out v)) ? v : 0;
						}
						field.SetValue(aObject, intValues);
					}

					// Support of float[].
					// -------------------
					else if (f.FieldType == typeof(float[]))
					{
						// Default value.
						float[] defaultValues = (float[]) field.GetValue(aObject);
						var value = PlayerPrefs.GetString(attr.Key, string.Join(" ", defaultValues));

						// Parse read value.
						var strValues = value.Split(' ');
						var floatValues = new float[strValues.Length];
						for (int i = 0, n = strValues.Length; i < n; i++)
						{
							floatValues[i] = ParseFloat(strValues[i]);
						}
						field.SetValue(aObject, floatValues);
					}
				}
			}
		}

	#endregion

	#region Private Methods

		private static string Vec2ToStr(Vector2 aValue)
		{
			return $"{aValue.x} {aValue.y}";
		}

		private static Vector2 StrToVec2(string aValue)
		{
			var v = Vector2.zero;
			string[] arr = aValue.Split(' ');
			if (arr.Length >= 2)
			{
				v.x = ParseFloat(arr[0]);
				v.y = ParseFloat(arr[1]);
			}
			return v;
		}

		private static string Vec3ToStr(Vector3 aValue)
		{
			return $"{aValue.x} {aValue.y} {aValue.z}";
		}

		private static Vector3 StrToVec3(string aValue)
		{
			var v = Vector3.zero;
			string[] arr = aValue.Split(' ');
			if (arr.Length >= 3)
			{
				v.x = ParseFloat(arr[0]);
				v.y = ParseFloat(arr[1]);
				v.z = ParseFloat(arr[2]);
			}
			return v;
		}

		private static string QuatToStr(Quaternion aValue)
		{
			return $"{aValue.x} {aValue.y} {aValue.z} {aValue.w}";
		}

		private static Quaternion StrToQuat(string aValue)
		{
			Quaternion v = Quaternion.identity;
			string[] arr = aValue.Split(' ');
			if (arr.Length >= 4)
			{
				v.x = ParseFloat(arr[0]);
				v.y = ParseFloat(arr[1]);
				v.z = ParseFloat(arr[2]);
				v.w = ParseFloat(arr[3]);
			}
			return v;
		}

		private static float ParseFloat(string aValue)
		{
			NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			float result = 0.0f;
			return (float.TryParse(aValue.Replace(',', '.'), style, culture, out result)) ? result : 0.0f;
		}

		private static double ParseDouble(string aValue)
		{
			NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			double result = 0.0;
			return (double.TryParse(aValue.Replace(',', '.'), style, culture, out result)) ? result : 0.0;
		}

	#endregion
	}
}