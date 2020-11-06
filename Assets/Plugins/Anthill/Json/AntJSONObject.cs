namespace Anthill.Json
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class AntJSONObject : AntJSONItem, IEnumerable
	{
		protected Dictionary<string, AntJSONItem> _dict;

		public AntJSONObject()
		{
			_dict = new Dictionary<string, AntJSONItem>();
		}

		public override void Add(string aKey, AntJSONItem aItem)
		{
			if (!string.IsNullOrEmpty(aKey))
			{
				aItem.SetKey(aKey);
				if (_dict.ContainsKey(aKey))
				{
					_dict[aKey] = aItem;
				}
				else
				{
					_dict.Add(aKey, aItem);
				}
			}
			else
			{
				aItem.SetKey(Guid.NewGuid().ToString());
				_dict.Add(aItem.Key, aItem);
			}
		}

		public override void Add(string aKey, string aValue)
		{
			Add(aKey, new AntJSONData(aValue));
		}

		public override AntJSONItem Remove(string aKey)
		{
			if (_dict.ContainsKey(aKey))
			{
				var tmp = _dict[aKey];
				_dict.Remove(aKey);
				return tmp;
			}

			return null;
		}

		public override AntJSONItem this[string aKey]
		{
			get
			{
				return (_dict.ContainsKey(aKey))
					? _dict[aKey]
					: new AntJSONFactory(aKey, this);
			}
			set
			{
				value.SetKey(aKey);
				if (_dict.ContainsKey(aKey))
				{
					_dict[aKey] = value;
				}
				else
				{
					_dict.Add(aKey, value);
				}
			}
		}

		public override AntJSONItem this[int aIndex]
		{
			get
			{
				return (aIndex >= 0 && aIndex < _dict.Count)
					? _dict.ElementAt(aIndex).Value
					: new AntJSONFactory(this);
			}
			set
			{
				if (aIndex >= 0 && aIndex < _dict.Count)
				{
					string key = _dict.ElementAt(aIndex).Key;
					value.SetKey(key);
					_dict[key] = value;
				}
			}
		}

		public bool ContainsKey(string aKey)
		{
			return _dict.ContainsKey(aKey);
		}

		public override bool HasChildren
		{
			get { return NumChildren > 0; }
		}

		public override int NumChildren
		{
			get { return _dict.Count; }
		}

		#region Implementing IEnumerable

		public override IEnumerable<AntJSONItem> Children
		{
			get
			{
				foreach (KeyValuePair<string, AntJSONItem> item in _dict)
				{
					yield return item.Value;
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, AntJSONItem> item in _dict)
			{
				yield return item;
			}
		}
		
		#endregion

		public override string ToString()
		{
			string result = "{ ";
			foreach (KeyValuePair<string, AntJSONItem> item in _dict)
			{
				if (result.Length > 2)
				{
					result += ", ";
				}

				result += "\"" + Escape(item.Key) + "\": " + item.Value;
			}

			result += " }";
			return result;
		}

		public override string Export(string aPrefix = "")
		{
			string result = "{";
			foreach (KeyValuePair<string, AntJSONItem> item in _dict)
			{
				if (result.Length > 2)
				{
					result += ", ";
				}

				result += "\n" + aPrefix + "   ";
				result += "\"" + Escape(item.Key) + "\": " + item.Value.Export(aPrefix + "   ");
			}

			result += "\n" + aPrefix + "}";
			return result;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)AntJSONBinaryTag.Object);
			aWriter.Write(_dict.Count);
			foreach (var k in _dict)
			{
				aWriter.Write(k.Key);
				_dict[k.Key].Serialize(aWriter);
			}
		}
	}
}