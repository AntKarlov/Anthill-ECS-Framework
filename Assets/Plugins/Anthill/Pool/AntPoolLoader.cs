namespace Anthill.Pool
{
	using UnityEngine;
	using System.Collections;

	public class AntPoolLoader : MonoBehaviour
	{
		public delegate void PoolLoaderDelegate(AntPoolLoader aLoader, float aProgress);
		public event PoolLoaderDelegate EventStart;
		public event PoolLoaderDelegate EventProcess;
		public event PoolLoaderDelegate EventComplete;

		[HideInInspector]
		public AntPoolList[] pools;

		[HideInInspector]
		public int countPerStep = 5;

		private AntPoolList _currentPool; // Текущий пул.
		private int _currentPoolIndex;    // Индекс текущего пула в списке пулов.
		private int _currentItemIndex;    // Индекс текущего айтема в текущем списке пулов.
		private int _current;             // Текущий индекс всего процесса.
		private int _count;               // Кол-во обрабатываемых вещей.

		#region Public Methods

		public void StartLoading()
		{
			A.Assert((pools == null || pools.Length == 0), "Pool Loader is empty!");

			var root = new GameObject();
			root.name = "Pools";
			AntPoolManager.SetParent(root.transform);

			_count = 0;
			_current = 0;
			for (int i = 0, n = pools.Length; i < n; i++)
			{
				_count += pools[i].items.Length;
			}

			_currentPoolIndex = 0;
			_currentPool = pools[_currentPoolIndex];

			EventStart?.Invoke(this, 0.0f);
			StartCoroutine(DoProgress());
		}

		#endregion
		#region Private Methods
		
		private IEnumerator DoProgress()
		{
			while (true)
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

				if (_current < _count)
				{
					yield return null;
				}
				else
				{
					break;
				}
			}
			
			EventComplete?.Invoke(this, 1.0f);
		}

		#endregion

	}
}