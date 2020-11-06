namespace Anthill.Inject
{
	using System;
	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class InjectAttribute : Attribute
	{
		public string Key { get; set; }

		public InjectAttribute()
		{
			// ..
		}

		public InjectAttribute(string aKey)
		{
			Key = aKey;
		}
	}
}