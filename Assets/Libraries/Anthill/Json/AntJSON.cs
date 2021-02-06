namespace Anthill.Json
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;

	public static class AntJSON
	{
		public static AntJSONItem Parse(string aJSON)
		{
			var stack = new Stack<AntJSONItem>();
			AntJSONItem current = null;
			int i = 0;
			string token = "";
			string tokenName = "";
			bool quoteMode = false;
			while (i < aJSON.Length)
			{
				switch (aJSON[i])
				{
					case '{' :
						if (quoteMode)
						{
							token += aJSON[i];
							break;
						}

						stack.Push(new AntJSONObject());
						if (current != null)
						{
							tokenName = tokenName.Trim();
							current.Add(tokenName, stack.Peek());
						}

						token = string.Empty;
						tokenName = string.Empty;
						current = stack.Peek();
						break;

					case '[' :
						if (quoteMode)
						{
							token += aJSON[i];
							break;
						}

						stack.Push(new AntJSONArray());
						if (current != null)
						{
							tokenName = tokenName.Trim();
							current.Add(tokenName, stack.Peek());
						}

						token = string.Empty;
						tokenName = string.Empty;
						current = stack.Peek();
						break;

					case '}' :
					case ']' :
						if (quoteMode)
						{
							token += aJSON[i];
							break;
						}

						if (stack.Count == 0)
						{
							throw new Exception("AntJSON Parse: To many closing brackets.");
						}

						stack.Pop();
						if (!token.Equals(string.Empty))
						{
							if (current != null)
							{
								tokenName = tokenName.Trim();
								current.Add(tokenName, new AntJSONData(token));
							}
						}

						token = string.Empty;
						tokenName = string.Empty;
						if (stack.Count > 0)
						{
							current = stack.Peek();
						}
						break;

					case ':' :
						if (quoteMode)
						{
							token += aJSON[i];
							break;
						}

						tokenName = token;
						token = string.Empty;
						break;

					case '"' :
						quoteMode = !quoteMode;
						break;

					case ',' :
						if (quoteMode)
						{
							token += aJSON[i];
							break;
						}

						if (!token.Equals(string.Empty) && current != null)
						{
							current.Add(tokenName, new AntJSONData(token));
						}

						token = string.Empty;
						tokenName = string.Empty;
						break;

					case '\n' :
					case '\r' :
						break;

					case ' ' :
					case '\t' :
						if (quoteMode)
						{
							token += aJSON[i];
						}
						break;

					case '\\' :
						i++;
						if (quoteMode)
						{
							char c = aJSON[i];
							switch (c)
							{
								case 't' : 
									token += "\t";
									break;

								case 'r' :
									token += "\r";
									break;

								case 'n' : 
									token += "\n"; 
									break;

								case 'b' : 
									token += "\b"; 
									break;

								case 'f' : 
									token += "\f"; 
									break;

								case 'u' :
									string s = aJSON.Substring(i + 1, 4);
									token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
									i += 4;
									break;

								default : 
									token += c; 
									break;
							}
						}
						break;

					case '1' :
					case '2' :
					case '3' :
					case '4' :
					case '5' :
					case '6' :
					case '7' :
					case '8' :
					case '9' :
					case '0' :
						token += aJSON[i];
						break;

					default:
						if (quoteMode)
						{
							token += aJSON[i];
						}
						break;
				}
				i++;
			}

			if (quoteMode)
			{
				throw new Exception("AntJSON Parse: Quotation marks seems to be missed up.");
			}

			return current;
		}

		public static AntJSONObject Serialize<T>(object aObject)
		{
			Type t = typeof(T);
			var result = new AntJSONObject();
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var json = (JsonFieldAttribute) field.GetCustomAttributes(typeof(JsonFieldAttribute), true).FirstOrDefault();
				if (json != null)
				{
					FieldInfo f = t.GetField(field.Name);
					if (f.FieldType == typeof(string)) // string
					{
						result[json.Key].Value = (string) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Int32)) // int
					{
						result[json.Key].AsInt = (int) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Single)) // float
					{
						result[json.Key].AsFloat = (float) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Boolean)) // bool
					{
						result[json.Key].AsBool = (bool) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Vector2)) // Vector2
					{
						result[json.Key].AsVector2 = (Vector2) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Vector3)) // Vector3
					{
						result[json.Key].AsVector3 = (Vector3) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Quaternion)) // Quaternion
					{
						result[json.Key].AsQuaternion = (Quaternion) field.GetValue(aObject);
					}
					else if (f.FieldType == typeof(Color)) // Color
					{
						result[json.Key].AsColor = (Color) field.GetValue(aObject);
					}
					else if (f.FieldType.BaseType == typeof(Enum)) // Enum
					{
						result[json.Key].Value = ((Enum) field.GetValue(aObject)).ToString();
					}
					else if (f.FieldType == typeof(LayerMask)) // LayerMask
					{
						string[] mask = AntLayerMask.MaskToNames((LayerMask) field.GetValue(aObject));
						var arr = new AntJSONArray();
						for (int i = 0, n = mask.Length; i < n; i++)
						{
							arr[i].Value = mask[i];
						}
						result[json.Key] = arr;
					}
					else if (f.FieldType == typeof(float[])) // float []
					{
						float[] values = (float[]) field.GetValue(aObject);
						var arr = new AntJSONArray();
						for (int i = 0, n = values.Length; i < n; i++)
						{
							arr[i].AsFloat = values[i];
						}
						result[json.Key] = arr;
					}
					else if (f.FieldType == typeof(int[])) // int[]
					{
						int[] values = (int[]) field.GetValue(aObject);
						var arr = new AntJSONArray();
						for (int i = 0, n = values.Length; i < n; i++)
						{
							arr[i].AsInt = values[i];
						}
						result[json.Key] = arr;
					}
				}
			}
			return result;
		}

		public static object Deserialize<T>(AntJSONObject aData, object aObject)
		{
			Type t = typeof(T);
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var json = (JsonFieldAttribute) field.GetCustomAttributes(typeof(JsonFieldAttribute), true).FirstOrDefault();
				if (json != null)
				{
					FieldInfo f = t.GetField(field.Name);
					if (f.FieldType == typeof(string)) // string
					{
						field.SetValue(aObject, aData[json.Key].Value);
					}
					else if (f.FieldType == typeof(Int32)) // int
					{
						field.SetValue(aObject, aData[json.Key].AsInt);
					}
					else if (f.FieldType == typeof(Single)) // float
					{
						field.SetValue(aObject, aData[json.Key].AsFloat);
					}
					else if (f.FieldType == typeof(Boolean)) // bool
					{
						field.SetValue(aObject, aData[json.Key].AsBool);
					}
					else if (f.FieldType == typeof(Vector2)) // Vector2
					{
						field.SetValue(aObject, aData[json.Key].AsVector2);
					}
					else if (f.FieldType == typeof(Vector3)) // Vector3
					{
						field.SetValue(aObject, aData[json.Key].AsVector3);
					}
					else if (f.FieldType == typeof(Quaternion)) // Quaternion
					{
						field.SetValue(aObject, aData[json.Key].AsQuaternion);
					}
					else if (f.FieldType == typeof(Color)) // Color
					{
						field.SetValue(aObject, aData[json.Key].AsColor);
					}
					else if (f.FieldType.BaseType == typeof(Enum)) // Enum
					{
						try
						{
							field.SetValue(aObject, Enum.Parse(f.FieldType, aData[json.Key].Value));
						}
						catch (Exception)
						{
							// ..
						}
					}
					else if (f.FieldType == typeof(LayerMask)) // LayerMask
					{
						AntJSONArray arr = aData[json.Key].AsArray;
						var mask = new string[arr.NumChildren];
						for (int i = 0, n = mask.Length; i < n; i++)
						{
							mask[i] = arr[i].Value;
						}
						field.SetValue(aObject, AntLayerMask.NamesToMask(mask));
					}
					else if (f.FieldType == typeof(int[])) // int[]
					{
						AntJSONArray arr = aData[json.Key].AsArray;
						var values = new int[arr.NumChildren];
						for (int i = 0, n = values.Length; i < n; i++)
						{
							values[i] = arr[i].AsInt;
						}
						field.SetValue(aObject, values);
					}
					else if (f.FieldType == typeof(float[])) // float[]
					{
						AntJSONArray arr = aData[json.Key].AsArray;
						var values = new float[arr.NumChildren];
						for (int i = 0, n = values.Length; i < n; i++)
						{
							values[i] = arr[i].AsFloat;
						}
						field.SetValue(aObject, values);
					}
				}
			}
			return aObject;
		}

		public static Type GetType(string aTypeName)
		{
			Type type = Type.GetType(aTypeName);
			if (type != null)
			{
				return type;
			}

			if (aTypeName.Contains("."))
			{
				var assemblyName = aTypeName.Substring(0, aTypeName.IndexOf('.'));
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(aTypeName);
					if (type != null)
					{
						return type;
					}
				}
			}

			var currentAssembly = Assembly.GetExecutingAssembly();
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach (var assemblyName in referencedAssemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(aTypeName);
					if (type != null)
					{
						return type;
					}
				}
			}

			return null;
		}

		public static AntJSONItem LoadFromAsStream(TextAsset aAsset)
		{
			return AntJSONItem.LoadFromStream(new MemoryStream(aAsset.bytes));
		}

		public static AntJSONItem LoadFromAsText(TextAsset aAsset)
		{
			return AntJSON.Parse(aAsset.text);
		}

		public static AntJSONItem LoadFromResourcesAsStream(string aFileName)
		{
			var data = (TextAsset) Resources.Load(aFileName);
			return AntJSONItem.LoadFromStream(new MemoryStream(data.bytes));
		}

		public static AntJSONItem LoadFromResourcesAsText(string aFileName)
		{
			var data = (TextAsset) Resources.Load(aFileName);
			return AntJSON.Parse(data.text);
		}

		public static AntJSONItem LoadFromFileAsStream(string aFileName)
		{
			return AntJSONItem.LoadFromFileAsStream(aFileName);
		}

		public static AntJSONItem LoadFromFileAsText(string aFileName)
		{
			return AntJSONItem.LoadFromFileAsText(aFileName);
		}
	}
}