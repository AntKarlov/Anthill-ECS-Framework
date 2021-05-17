namespace Anthill.Pools
{
	using UnityEngine;
	
	[AddComponentMenu("Anthill/Pools/Poolable")]
	public class AntPoolable : MonoBehaviour
	{
		public delegate void PoolableDelegate();

		/// <summary>
		/// Called when object extracted from the pool.
		/// </summary>
		public PoolableDelegate EventExtractedFromPool;

		/// <summary>
		/// Called when object placed to the pool.
		/// </summary>
		public PoolableDelegate EventPlacedToPool;

		/// <summary>
		/// Called when object returned to the pool after using.
		/// </summary>
		public PoolableDelegate EventReturnedToPool;

		private PoolableDelegate _extractedCallback;
		private PoolableDelegate _returnedCallback;

	#region Getters / Setters

		public AntPoolContainer Pool { get; set; }

	#endregion

	#region Unity Calls

		private void OnDestroy()
		{
			Pool = null;
		}

	#endregion

	#region Public Methods

		public AntPoolable ReturnToPool()
		{
			if (Pool == null)
			{
				A.Editor.Warning($"Object `{name}` is not polled and trying to returning to the pool.", this);
				return this;
			}

			Pool.Add(this);
			return this;
		}

		public AntPoolable OnExtracted(PoolableDelegate aCallback)
		{
			_extractedCallback = aCallback;
			return this;
		}

		public AntPoolable OnReturned(PoolableDelegate aCallback)
		{
			_returnedCallback = aCallback;
			return this;
		}

	#endregion

	#region Protected Methods

		internal void ExtractedFromPool()
		{
			_extractedCallback?.Invoke();
			_extractedCallback = null;

			EventExtractedFromPool?.Invoke();
			gameObject.SetActive(true);
		}

		internal void ReturnedToPool()
		{
			_returnedCallback?.Invoke();
			_returnedCallback = null;

			EventPlacedToPool?.Invoke();
			gameObject.SetActive(false);
		}

	#endregion
	}
}