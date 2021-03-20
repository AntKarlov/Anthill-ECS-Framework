namespace Anthill.Core
{
	using System;
	using System.Collections.Generic;

	public class AntNodePool<T>
	{
	#region Private Variables

		private List<T> _availList;

	#endregion
	
	#region Public Methods

		public AntNodePool()
		{
			_availList = new List<T>();
		}

		public void Add(T aNode)
		{
			_availList.Add(aNode);
		}

		public T Get()
		{
			T result;
			if (_availList.Count > 0)
			{
				result = _availList[_availList.Count - 1];
				_availList.RemoveAt(_availList.Count - 1);
			}
			else
			{
				result = Activator.CreateInstance<T>(); 
			}
			return result;
		}

	#endregion
	}
}