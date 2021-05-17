namespace Anthill.Pools
{
	using UnityEngine;
	using System.Collections.Generic;

	using Anthill.Core;

	public class AntPoolLoaderSystem : ISystem, IExecuteSystem
	{
		public delegate void PoolLoaderDelegate(AntPoolLoaderSystem aLoader);
		public event PoolLoaderDelegate EventStart;
		public event PoolLoaderDelegate EventComplete;

		public delegate void PoolLoaderProgressDelegate(AntPoolLoaderSystem aLoader, float aProgress);
		public event PoolLoaderProgressDelegate EventProcess;

		public List<AntPoolPreset> pools;
		public int countPerStep = 5;

		private AntPoolPreset _currentPool; // Текущий пул.
		private int _currentPoolIndex;      // Индекс текущего пула в списке пулов.
		private int _currentItemIndex;      // Индекс текущего айтема в текущем списке пулов.
		private int _current;               // Текущий индекс всего процесса.
		private int _count;                 // Кол-во обрабатываемых вещей.
		private bool _isStarted;

		private PoolLoaderDelegate _startCallback;
		private PoolLoaderDelegate _completeCallback;

	#region Public Methods

		public AntPoolLoaderSystem()
		{
			pools = new List<AntPoolPreset>();
			_isStarted = false;
		}

		public AntPoolLoaderSystem AddPreset(string aFileName)
		{
			var list = (AntPoolPreset) Resources.Load(aFileName);
			A.Assert(list == null, $"Can't load `{aFileName}` pool asset!", this);
			pools.Add(list);
			return this;
		}

		public AntPoolLoaderSystem StartLoading()
		{
			if (_isStarted)
			{
				A.Editor.Warning("Already started!", this);
				return this;
			}

			A.Assert((pools == null || pools.Count == 0), "Nothig for loading!", this);

			var root = new GameObject();
			root.name = "Pools";
			AntPool.SetParent(root.transform);

			_count = 0;
			_current = 0;
			for (int i = 0, n = pools.Count; i < n; i++)
			{
				_count += pools[i].items.Length;
			}

			_currentPoolIndex = 0;
			_currentPool = pools[_currentPoolIndex];

			EventStart?.Invoke(this);
			_startCallback?.Invoke(this);
			_startCallback = null;

			_isStarted = true;
			return this;
		}

		public AntPoolLoaderSystem OnStart(PoolLoaderDelegate aCallback)
		{
			_startCallback = aCallback;
			return this;
		}

		public AntPoolLoaderSystem OnComplete(PoolLoaderDelegate aCallback)
		{
			_completeCallback = aCallback;
			return this;
		}

	#endregion

	#region ISystem Implementation
		
		public void AddedToEngine()
		{
			// ..
		}

		public void RemovedFromEngine()
		{
			// ..
		}

	#endregion

	#region IExecuteSystem Implementation

		public void Execute()
		{
			if (_isStarted)
			{
				int n = (_current + countPerStep > _count) 
					? _count - _current 
					: countPerStep; 
				
				for (int i = 0; i < n; i++)
				{
					if (_currentItemIndex == _currentPool.items.Length)
					{
						_currentPoolIndex++;
						_currentPool = pools[_currentPoolIndex];
						_currentItemIndex = 0;
					}
					
					AntPool.Add(_currentPool.items[_currentItemIndex]);
					_currentItemIndex++;
					_current++;
				}

				EventProcess?.Invoke(this, (float) _current / (float) _count);

				if (_current >= _count)
				{
					_isStarted = false;
					EventComplete?.Invoke(this);
					_completeCallback?.Invoke(this);
					_completeCallback = null;
				}
			}
		}

	#endregion
	}
}