namespace Anthill.Json
{
	using UnityEngine;
	
	public class AntJSONData : AntJSONItem
	{
		public AntJSONData(string aValue)
		{
			Value = aValue;
		}

		public AntJSONData(int aValue)
		{
			AsInt = aValue;
		}

		public AntJSONData(float aValue)
		{
			AsFloat = aValue;
		}

		public AntJSONData(double aValue)
		{
			AsDouble = aValue;
		}

		public AntJSONData(bool aValue)
		{
			AsBool = aValue;
		}

		public AntJSONData(Vector2 aValue)
		{
			AsVector2 = aValue;
		}

		public AntJSONData(Vector3 aValue)
		{
			AsVector3 = aValue;
		}

		public AntJSONData(Quaternion aValue)
		{
			AsQuaternion = aValue;
		}

		public AntJSONData(Color aValue)
		{
			AsColor = aValue;
		}

		public override string ToString()
		{
			var tmp = new AntJSONData("");

			tmp.AsInt = AsInt;
			if (tmp.Value == _value)
			{
				return AsInt.ToString();
			}

			return "\"" + Escape(Value) + "\"";
		}

		public override string Export(string aPrefix = "")
		{
			return ToString();
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			var tmp = new AntJSONData("");

			tmp.AsInt = AsInt;
			if (tmp.Value == Value)
			{
				aWriter.Write((byte) AntJSONBinaryTag.IntValue);
				aWriter.Write(AsInt);
				return;
			}

			tmp.AsFloat = AsFloat;
			if (tmp.Value == Value)
			{
				aWriter.Write((byte) AntJSONBinaryTag.FloatValue);
				aWriter.Write(AsFloat);
				return;
			}

			tmp.AsDouble = AsDouble;
			if (tmp.Value == Value)
			{
				aWriter.Write((byte) AntJSONBinaryTag.DoubleValue);
				aWriter.Write(AsDouble);
				return;
			}

			tmp.AsBool = AsBool;
			if (tmp.Value == Value)
			{
				aWriter.Write((byte) AntJSONBinaryTag.BoolValue);
				aWriter.Write(AsBool);
				return;
			}

			aWriter.Write((byte) AntJSONBinaryTag.Value);
			aWriter.Write((Value != null) ? Value : "");
		}
	}
}