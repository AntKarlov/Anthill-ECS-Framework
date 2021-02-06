#define Use_FileIO
namespace Anthill.Json
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using UnityEngine;
	using Anthill.Extensions;

	public class AntJSONItem
	{
		protected string _key = "";
		protected string _value = "";

		#region Interface implementation

		public virtual void Add(AntJSONItem aItem)
		{
			Add("", aItem);
		}

		public virtual void Add(string aKey, string aValue)
		{
			Add(aKey, new AntJSONData(aValue));
		}

		public virtual void Add(string aKey, AntJSONItem aItem)
		{
			// ...
		}

		public virtual AntJSONItem Remove(string aKey)
		{
			return null;
		}

		public virtual AntJSONItem Remove(int aIndex)
		{
			return null;
		}

		public virtual AntJSONItem Remove(AntJSONItem aItem)
		{
			return null;
		}

		public virtual AntJSONItem this[string aKey]
		{
			get { return null; }
			set { }
		}

		public virtual AntJSONItem this[int aIndex]
		{
			get { return null; }
			set { }
		}

		public virtual IEnumerable<AntJSONItem> Children
		{
			get
			{
				yield break;
			}
		}

		public virtual bool QueryCheck(string aQuery)
		{
			string[] list = aQuery.Split('.');
			AntJSONItem current = this;
			int index;
			for (int i = 0; i < list.Length; i++)
			{
				current = (int.TryParse(list[i], out index)) ? current[index] : current[list[i]];
				if (current == null)
				{
					return false;
				}
			}

			return true;
		}

		public virtual AntJSONItem Query(string aQuery)
		{
			string[] list = aQuery.Split('.');
			AntJSONItem current = this;
			int index;
			for (int i = 0; i < list.Length; i++)
			{
				current = (int.TryParse(list[i], out index)) ? current[index] : current[list[i]];
				if (current == null)
				{
					return null;
				}
			}

			return current;
		}

		public override string ToString()
		{
			return "AntJSONItem";
		}

		public virtual string Export(string aPrefix = "")
		{
			return "AntJSONItem";
		}

		internal void SetKey(string aValue)
		{
			_key = aValue;
		}

		public virtual string Key
		{
			get { return _key; }
		}

		public virtual string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public virtual bool HasChildren
		{
			get { return false; }
		}

		public virtual int NumChildren
		{
			get { return 0; }
		}

		#endregion
		#region Typecasting

		protected float ParseFloat(string aValue)
		{
			NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			float result = 0.0f;
			return (float.TryParse(aValue.Replace(',', '.'), style, culture, out result)) ? result : 0.0f;
		}

		protected double ParseDouble(string aValue)
		{
			NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			double result = 0.0;
			return (double.TryParse(aValue.Replace(',', '.'), style, culture, out result)) ? result : 0.0;
		}

		public virtual int AsInt
		{
			get
			{
				int v;
				return (int.TryParse(Value, out v)) ? v : 0;
			}
			set { Value = value.ToString(); }
		}

		public virtual float AsFloat
		{
			get { return ParseFloat(Value); }
			set { Value = value.ToString(); }
		}

		public virtual double AsDouble
		{
			get { return ParseDouble(Value); }
			set { Value = value.ToString(); }
		}

		public virtual bool AsBool
		{
			get
			{
				bool v;
				return (bool.TryParse(Value, out v)) ? v : false;
			}
			set { Value = value.ToString(); }
		}

		public virtual Vector2 AsVector2
		{
			get
			{
				var v = Vector2.zero;
				string[] arr = Value.Split(' ');
				if (arr.Length >= 2)
				{
					v.x = ParseFloat(arr[0]);
					v.y = ParseFloat(arr[1]);
				}
				return v;
			}
			set { Value = string.Format("{0} {1}", value.x, value.y); }
		}

		public virtual Vector3 AsVector3
		{
			get
			{
				var v = Vector3.zero;
				string[] arr = Value.Split(' ');
				if (arr.Length >= 3)
				{
					v.x = ParseFloat(arr[0]);
					v.y = ParseFloat(arr[1]);
					v.z = ParseFloat(arr[2]);
				}
				return v;
			}
			set { Value = string.Format("{0} {1} {2}", value.x, value.y, value.z); }
		}

		public virtual Quaternion AsQuaternion
		{
			get
			{
				Quaternion v = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				string[] arr = Value.Split(' ');
				if (arr.Length >= 4)
				{
					v.x = ParseFloat(arr[0]);
					v.y = ParseFloat(arr[1]);
					v.z = ParseFloat(arr[2]);
					v.w = ParseFloat(arr[3]);
				}
				return v;
			}
			set { Value = string.Format("{0} {1} {2} {3}", value.x, value.y, value.z, value.w); }
		}

		public virtual Color AsColor
		{
			get => Value.ToColor();
			set => Value = value.ToHex(true);
		}

		public virtual AntJSONObject AsObject
		{
			get { return this as AntJSONObject; }
		}

		public virtual AntJSONArray AsArray
		{
			get { return this as AntJSONArray; }
		}

		#endregion
		#region Operators

		/*public static implicit operator AntJSONItem(string aData)
		{
			return new AntJSONData(aData);
		}

		public static implicit operator string(AntJSONItem aItem)
		{
			return (aItem == null) ? null : aItem.Value;
		}*/

		public static bool operator ==(AntJSONItem aItemA, object aItemB)
		{
			if (aItemA is AntJSONFactory && aItemB == null)
			{
				return true;
			}

			return System.Object.ReferenceEquals(aItemA, aItemB);
		}

		public static bool operator !=(AntJSONItem aItemA, object aItemB)
		{
			return !(aItemA == aItemB);
		}

		public override bool Equals(object aItem)
		{
			return System.Object.ReferenceEquals(this, aItem);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
		#region Serialization

		public virtual void Serialize(BinaryWriter aWriter)
		{
			// ...
		}

		public static AntJSONItem Deserialize(BinaryReader aReader)
		{
			var tag = (AntJSONBinaryTag) aReader.ReadByte();
			int childCount;
			if (tag == AntJSONBinaryTag.Array)
			{
				childCount = aReader.ReadInt32();
				var arr = new AntJSONArray();
				for (int i = 0; i < childCount; i++)
				{
					arr.Add(Deserialize(aReader));
				}
				return arr;
			}

			if (tag == AntJSONBinaryTag.Object)
			{
				childCount = aReader.ReadInt32();
				var obj = new AntJSONObject();
				for (int i = 0; i < childCount; i++)
				{
					string key = aReader.ReadString();
					var val = Deserialize(aReader);
					obj.Add(key, val);
				}
				return obj;
			}

			if (tag == AntJSONBinaryTag.Value)
			{
				return new AntJSONData(aReader.ReadString());
			}

			if (tag == AntJSONBinaryTag.IntValue)
			{
				return new AntJSONData(aReader.ReadInt32());
			}

			if (tag == AntJSONBinaryTag.FloatValue)
			{
				return new AntJSONData(aReader.ReadSingle());
			}

			if (tag == AntJSONBinaryTag.DoubleValue)
			{
				return new AntJSONData(aReader.ReadDouble());
			}

			if (tag == AntJSONBinaryTag.BoolValue)
			{
				return new AntJSONData(aReader.ReadBoolean());
			}

			throw new Exception("AntJSON Deserialize: Unknown tag " + tag);
		}

		public void SaveToStream(Stream aData)
		{
			Serialize(new BinaryWriter(aData));
		}

		public static AntJSONItem LoadFromStream(Stream aStream)
		{
			using (var reader = new BinaryReader(aStream))
			{
				return Deserialize(reader);
			}
		}

		public void SaveToFileAsText(string aFileName)
		{
			#if Use_FileIO
			var dir = (new FileInfo(aFileName)).Directory.FullName;
			if (!File.Exists(dir))
				Directory.CreateDirectory(dir);

			var file = File.CreateText(aFileName);
			file.WriteLine(Export());
			file.Close();
			#else
			throw new Exception("AntJSON SaveToFile: Can't use File IO in web-player!");
			#endif
		}

		public static AntJSONItem LoadFromFileAsText(string aFileName)
		{
		#if Use_FileIO
			if (File.Exists(aFileName))
			{
				string data = "";
				var file = File.OpenText(aFileName);
				var line = file.ReadLine();
				data += line;
				while (line != null)
				{
					line = file.ReadLine();
					if (line != null)
					{
						data += line;
					}
				}

				return AntJSON.Parse(data);
			}
			
			throw new Exception("AntJSONItem LoadFromTextFile: File `" + aFileName + "` not found.");
		#else
			throw new Exception("AntJSON SaveToFile: Can't use File IO in web-player!");
		#endif
		}

		public void SaveToFileAsStream(string aFileName)
		{
		#if Use_FileIO
			Directory.CreateDirectory((new FileInfo(aFileName)).Directory.FullName);
			using (var file = File.OpenWrite(aFileName))
			{
				SaveToStream(file);
			}
		#else
			throw new Exception("AntJSON SaveToFile: Can't use File IO in web-player!");
		#endif
		}

		public static AntJSONItem LoadFromFileAsStream(string aFileName)
		{
		#if Use_FileIO
			using (var file = File.OpenRead(aFileName))
			{
				return LoadFromStream(file);
			}
		#else
			throw new Exception("AntJSON LoadFromFile: Can't use File IO in web-player!");
		#endif
		}

		public string SaveToBase64()
		{
			using (var stream = new MemoryStream())
			{
				SaveToStream(stream);
				stream.Position = 0;
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public static AntJSONItem LoadFromBase64(string aBase64)
		{
			var tmp = Convert.FromBase64String(aBase64);
			var stream = new MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}

		protected static string Escape(string aText)
		{
			string result = "";
			if (aText != null)
			{
				foreach (char c in aText)
				{
					switch (c)
					{
						case '\\' : 
							result += "\\\\";
							break;

						case '\"' : 
							result += "\\\"";
							break;

						case '\n' : 
							result += "\\n"; 
							break;

						case '\r' : 
							result += "\\r"; 
							break;

						case '\t' : 
							result += "\\t"; 
							break;

						case '\b' : 
							result += "\\b"; 
							break;

						case '\f' : 
							result += "\\f";
							break;

						default : 
							result += c; 
							break;
					}
				}
			}

			return result;
		}
		
		#endregion
	}
}