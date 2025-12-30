using System;
using System.Collections.Generic;

namespace Anthill.Core
{
	public class AntNodePool<T>
	{
		// -----------------------------------------------------
		// Private Variables
		// -----------------------------------------------------

		private readonly List<T> _availList;

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public AntNodePool()
		{
			_availList = new List<T>(AntEngine.InitialNodeListSize);
		}

		public void Add(T node)
		{
			_availList.Add(node);
		}

		public T Get()
		{
			T result;
			if (_availList.Count > 0)
			{
				result = _availList[^1];
				_availList.RemoveAt(_availList.Count - 1);
			}
			else
			{
				result = Activator.CreateInstance<T>();
			}
			return result;
		}
	}
}