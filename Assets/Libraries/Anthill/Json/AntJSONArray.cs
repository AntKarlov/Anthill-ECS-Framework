namespace Anthill.Json
{
	using System.Collections;
	using System.Collections.Generic;

	public class AntJSONArray : AntJSONItem, IEnumerable
	{
		private readonly List<AntJSONItem> _list;

		public AntJSONArray()
		{
			_list = new List<AntJSONItem>();
		}

		public override void Add(string aKey, AntJSONItem aItem)
		{
			_list.Add(aItem);
		}

		public override void Add(string aKey, string aValue)
		{
			_list.Add(new AntJSONData(aValue));
		}

		public override AntJSONItem this[string aKey]
		{
			get { return new AntJSONFactory(aKey, this); }
			set { _list.Add(value); }
		}

		public override AntJSONItem this[int aIndex]
		{
			get
			{
				if (aIndex >= 0 && aIndex < _list.Count)
				{
					return _list[aIndex];
				}

				return new AntJSONFactory(this);
			}
			set
			{
				if (aIndex >= 0 && aIndex < _list.Count)
				{
					_list[aIndex] = value;
				}

				_list.Add(value);
			}
		}

		public override AntJSONItem Remove(string aKey)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				if (string.Equals(_list[i].Key, aKey))
				{
					var tmp = _list[i];
					_list.RemoveAt(i);
					return tmp;
				}
			}

			return null;
		}

		public override AntJSONItem Remove(int aIndex)
		{
			if (aIndex >= 0 && aIndex < _list.Count)
			{
				var tmp = _list[aIndex];
				_list.RemoveAt(aIndex);
				return tmp;
			}

			return null;
		}

		public override AntJSONItem Remove(AntJSONItem aItem)
		{
			var index = _list.IndexOf(aItem);
			if (index >= 0 && index < _list.Count)
			{
				_list.RemoveAt(index);
				return aItem;
			}

			return null;
		}

		public override bool HasChildren
		{
			get { return NumChildren > 0; }
		}

		public override int NumChildren
		{
			get { return _list.Count; }
		}

		public override IEnumerable<AntJSONItem> Children
		{
			get
			{
				foreach (AntJSONItem item in _list)
				{
					yield return item;
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (AntJSONItem item in _list)
			{
				yield return item;
			}
		}

		public override string ToString()
		{
			string result = "[ ";
			foreach (AntJSONItem item in _list)
			{
				if (result.Length > 2)
				{
					result += ", ";
				}

				result += item.ToString();
			}

			result += " ]";
			return result;
		}

		public override string Export(string aPrefix = "")
		{
			string result = "[";
			foreach (AntJSONItem item in _list)
			{
				if (result.Length > 2)
				{
					result += ", ";
				}

				result += "\n" + aPrefix + "   ";
				result += item.Export(aPrefix + "   ");
			}
			result += "\n" + aPrefix + "]";
			return result;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)AntJSONBinaryTag.Array);
			aWriter.Write(_list.Count);
			for (int i = 0; i < _list.Count; i++)
			{
				_list[i].Serialize(aWriter);
			}
		}
	}
}