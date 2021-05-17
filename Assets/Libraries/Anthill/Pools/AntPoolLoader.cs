namespace Anthill.Pools
{
	using UnityEngine;
	using System.Collections;

	public class AntPoolLoader : MonoBehaviour
	{
		public delegate void PoolLoaderDelegate(AntPoolLoader aLoader);
		public event PoolLoaderDelegate EventStart;
		public event PoolLoaderDelegate EventComplete;

		public delegate void PoolLoaderProgressDelegate(AntPoolLoader aLoader, float aProgress);
		public event PoolLoaderProgressDelegate EventProcess;

		[HideInInspector]
		public AntPoolPreset[] pools = new AntPoolPreset[0];

		[HideInInspector]
		public int countPerStep = 5;

		[HideInInspector]
		public bool loadOnStart;

		private AntPoolPreset _currentPool; // Текущий пул.
		private int _currentPoolIndex;      // Индекс текущего пула в списке пулов.
		private int _currentItemIndex;      // Индекс текущего айтема в текущем списке пулов.
		private int _current;               // Текущий индекс всего процесса.
		private int _count;                 // Кол-во обрабатываемых вещей.
		private bool _isStarted;

		private PoolLoaderDelegate _startCallback;
		private PoolLoaderDelegate _completeCallback;

	#region Unity Calls
		
		private void Start()
		{
			if (loadOnStart)
			{
				StartLoading();
			}
		}
		
	#endregion

	#region Public Methods

		public AntPoolLoader StartLoading()
		{
			if (_isStarted)
			{
				return this;
			}

			A.Assert((pools == null || pools.Length == 0), "Nothing for pooling!", this);

			var root = new GameObject();
			root.name = "Pools";
			AntPool.SetParent(root.transform);

			_count = 0;
			_current = 0;
			for (int i = 0, n = pools.Length; i < n; i++)
			{
				_count += pools[i].items.Length;
			}

			_currentPoolIndex = 0;
			_currentPool = pools[_currentPoolIndex];

			StartCoroutine(DoProgress());
			_isStarted = true;
			return this;
		}

		public AntPoolLoader OnStart(PoolLoaderDelegate aCallback)
		{
			_startCallback = aCallback;
			return this;
		}

		public AntPoolLoader OnComplete(PoolLoaderDelegate aCallback)
		{
			_completeCallback = aCallback;
			return this;
		}

	#endregion

	#region Private Methods
		
		private IEnumerator DoProgress()
		{
			yield return null;

			EventStart?.Invoke(this);
			_startCallback?.Invoke(this);
			_startCallback = null;

			while (true)
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

				if (_current < _count)
				{
					yield return null;
				}
				else
				{
					break;
				}
			}
			
			EventComplete?.Invoke(this);
			_completeCallback?.Invoke(this);
			_completeCallback = null;
			_isStarted = false;
		}

	#endregion
	}
}