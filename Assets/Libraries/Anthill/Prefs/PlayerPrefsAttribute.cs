namespace Anthill.Prefs
{
	using System;
	
	[AttributeUsage(AttributeTargets.All)]
	public class PlayerPrefsAttribute : System.Attribute
	{
		private string _key;

		public PlayerPrefsAttribute(string aKey)
		{
			_key = aKey;
		}

		public string Key { get => _key; }
	}
}