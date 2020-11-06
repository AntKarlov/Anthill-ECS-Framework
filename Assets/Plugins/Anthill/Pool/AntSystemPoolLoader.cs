namespace Anthill.Pool
{
	using UnityEngine;
	using System.Collections.Generic;

	using Anthill.Core;

	public class AntSystemPoolLoader : ISystem, IExecuteSystem
	{
		public delegate void PoolLoaderDelegate(AntSystemPoolLoader aLoader, float aProgress);
		public event PoolLoaderDelegate EventStart;
		public event PoolLoaderDelegate EventProcess;
		public event PoolLoaderDelegate EventComplete;

		public List<AntPoolList> pools;
		public int countPerStep = 5;

		private AntPoolList _currentPool; // Текущий пул.
		private int _currentPoolIndex;    // Индекс текущего пула в списке пулов.
		private int _currentItemIndex;    // Индекс текущего айтема в текущем списке пулов.
		private int _current;             // Текущий индекс всего процесса.
		private int _count;               // Кол-во обрабатываемых вещей.
		private bool _isStarted;

		#region Public Methods

		public AntSystemPoolLoader()
		{
			pools = new List<AntPoolList>();
			_isStarted = false;
		}

		public void AddPoolList(string aFileName)
		{
			var list = (AntPoolList) Resources.Load(aFileName);
			A.Assert(list == null, $"Can't load `{aFileName}` pool list!", true);
			pools.Add(list);
		}

		public void StartLoading()
		{
			A.Assert((pools == null || pools.Count == 0), "Pool Loader is empty!", true);

			var root = new GameObject();
			root.name = "Pools";
			AntPoolManager.SetParent(root.transform);

			_count = 0;
			_current = 0;
			for (int i = 0, n = pools.Count; i < n; i++)
			{
				_count += pools[i].items.Length;
			}

			_currentPoolIndex = 0;
			_currentPool = pools[_currentPoolIndex];

			if (EventStart != null)
			{
				EventStart(this, 0.0f);
			}

			_isStarted = true;
		}

		#endregion
		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			// ..
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			// ..
		}

		#endregion
		#region IExecuteSystem Implementation

		public void Execute()
		{
			if (_isStarted)
			{
				int n = (_current + countPerStep > _count) ? _count - _current : countPerStep; 
				for (int i = 0; i < n; i++)
				{
					if (_currentItemIndex == _currentPool.items.Length)
					{
						_currentPoolIndex++;
						_currentPool = pools[_currentPoolIndex];
						_currentItemIndex = 0;
					}
					
					AntPoolManager.AddPool(_currentPool.items[_currentItemIndex].prefab, _currentPool.items[_currentItemIndex].initialSize);
					_currentItemIndex++;
					_current++;
				}

				EventProcess?.Invoke(this, (float) _current / (float) _count);

				if (_current >= _count)
				{
					_isStarted = false;
					EventComplete?.Invoke(this, 1.0f);
				}
			}
		}

		#endregion
	}
}