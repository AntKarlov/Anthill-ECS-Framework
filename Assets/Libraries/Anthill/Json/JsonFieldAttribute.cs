namespace Anthill.Json
{
	using System;
	
	[AttributeUsage(AttributeTargets.All)]
	public class JsonFieldAttribute : System.Attribute
	{
		private string _key;

		public JsonFieldAttribute(string aKey)
		{
			_key = aKey;
		}

		public string Key
		{
			get { return _key; }
		}
	}
}