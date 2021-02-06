namespace Anthill.Json
{
	public class AntJSONFactory : AntJSONItem
	{
		private AntJSONItem _parentItem;

		public AntJSONFactory(AntJSONItem aParentItem)
		{
			_key = null;
			_parentItem = aParentItem;
		}

		public AntJSONFactory(string aKey, AntJSONItem aParentItem)
		{
			_key = aKey;
			_parentItem = aParentItem;
		}

		private void Set(AntJSONItem aItem)
		{
			if (_key != null)
			{
				_parentItem.Add(_key, aItem);
			}
			else
			{
				_parentItem.Add(aItem);
			}

			_parentItem = null;
		}

		public override AntJSONItem this[string aKey]
		{
			get
			{
				return new AntJSONFactory(aKey, this);
			}
			set
			{
				var tmp = new AntJSONObject();
				tmp.Add(this);
				Set(tmp);
			}
		}

		public override AntJSONItem this[int aIndex]
		{
			get
			{
				return new AntJSONFactory(this);
			}
			set
			{
				var tmp = new AntJSONArray();
				tmp.Add(this);
				Set(tmp);
			}
		}

		public override void Add(AntJSONItem aItem)
		{
			var tmp = new AntJSONArray();
			tmp.Add(aItem);
			Set(tmp);
		}

		public override void Add(string aKey, AntJSONItem aItem)
		{
			var tmp = new AntJSONObject();
			tmp.Add(aKey, aItem);
			Set(tmp);
		}

		public override string ToString()
		{
			return "AntJSONFactory";
		}

		public override string Export(string aPrefix = "")
		{
			return string.Empty;
		}

		public static bool operator ==(AntJSONFactory aItemA, object aItemB)
		{
			return (aItemB != null) 
				? System.Object.ReferenceEquals(aItemA, aItemB) 
				: true;
		}

		public static bool operator !=(AntJSONFactory aItemA, object aItemB)
		{
			return !(aItemA == aItemB);
		}

		public override bool Equals(object aItem)
		{
			return (aItem != null) 
				? System.Object.ReferenceEquals(this, aItem) 
				: true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string Value
		{
			get
			{
				Set(new AntJSONData(""));
				return "";
			}
			set
			{
				Set(new AntJSONData(value));
			}
		}

		public override int AsInt
		{
			get
			{
				Set(new AntJSONData(0));
				return 0;
			}
			set
			{
				Set(new AntJSONData(value));
			}
		}

		public override float AsFloat
		{
			get
			{
				Set(new AntJSONData(0.0f));
				return 0.0f;
			}
			set
			{
				Set(new AntJSONData(value));
			}
		}

		public override double AsDouble
		{
			get
			{
				Set(new AntJSONData(0.0));
				return 0.0;
			}
			set
			{
				Set(new AntJSONData(value));
			}
		}

		public override bool AsBool
		{
			get
			{
				Set(new AntJSONData(false));
				return false;
			}
			set
			{
				Set(new AntJSONData(value));
			}
		}

		public override AntJSONArray AsArray
		{
			get
			{
				var tmp = new AntJSONArray();
				Set(tmp);
				return tmp;
			}
		}

		public override AntJSONObject AsObject
		{
			get
			{
				var tmp = new AntJSONObject();
				Set(tmp);
				return tmp;
			}
		}
	}
}