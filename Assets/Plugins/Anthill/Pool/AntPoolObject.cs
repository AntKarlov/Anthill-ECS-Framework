namespace Anthill.Pool
{
	using UnityEngine;
	
	public class AntPoolObject : MonoBehaviour
	{
		public delegate void PoolActionDelegate();

		/// <summary>
		/// Called when object extracted from the pool.
		/// </summary>
		public PoolActionDelegate EventExtractedFromPool;

		/// <summary>
		/// Called when object placed to the pool
		/// </summary>
		public PoolActionDelegate EventPlacedToPool;

		/// <summary>
		/// Called when object returned to the pool after using.
		/// </summary>
		public PoolActionDelegate EventReturnedToPool;

		#region Getters / Setters

		public AntPool Pool { get; set; }

		#endregion
		#region Unity Calls

		private void OnDestroy()
		{
			Pool = null;
		}

		#endregion
		#region Public Methods

		public void ReturnToPool()
		{
			if (Pool != null)
			{
				Pool.AddObject(this);
				EventReturnedToPool?.Invoke();
			}
			else
			{
				A.Warning($"Object `{name}` is not polled and trying to returning to the pool.");
			}
		}

		#endregion
		#region Protected Methods

		internal void ExtractedFromPool()
		{
			gameObject.SetActive(true);
			EventExtractedFromPool?.Invoke();
		}

		internal void PlacedToPool()
		{
			gameObject.SetActive(false);
			EventPlacedToPool?.Invoke();
		}

		#endregion
	}
}