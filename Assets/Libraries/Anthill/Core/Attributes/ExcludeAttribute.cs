using System;

namespace Anthill.Core
{
	public class ExcludeAttribute : Attribute
	{
		public Type[] excludeTypes;

		public ExcludeAttribute(Type excludeType)
		{
			excludeTypes = new [] { excludeType };
		}

		public ExcludeAttribute(Type excludeType1, Type excludeType2)
		{
			excludeTypes = new [] { excludeType1, excludeType2 };
		}

		public ExcludeAttribute(Type excludeType1, Type excludeType2, Type excludeType3)
		{
			excludeTypes = new [] { excludeType1, excludeType2, excludeType3 };
		}

		public ExcludeAttribute(Type excludeType1, Type excludeType2, Type excludeType3, Type excludeType4)
		{
			excludeTypes = new [] { excludeType1, excludeType2, excludeType3, excludeType4 };
		}

		public ExcludeAttribute(Type excludeType1, Type excludeType2, Type excludeType3, Type excludeType4, Type excludeType5)
		{
			excludeTypes = new [] { excludeType1, excludeType2, excludeType3, excludeType4, excludeType5 };
		}

		public ExcludeAttribute(Type excludeType1, Type excludeType2, Type excludeType3, Type excludeType4, Type excludeType5, Type excludeType6)
		{
			excludeTypes = new [] { excludeType1, excludeType2, excludeType3, excludeType4, excludeType5, excludeType6 };
		}
	}
}