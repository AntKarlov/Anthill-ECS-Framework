namespace Anthill.Pool
{
	using System.Collections.Generic;
	using UnityEngine;

	public class AntPool
	{
		public string name;

		protected List<AntPoolObject> _objects;
		protected Object _samplePrefab;
		protected Transform _parent;
		protected int _currentIndex;
		protected int _growCount;

		#region Getters / Setters

		public int GrowCount { get => _growCount; }

		#endregion
		#region Public Methods

		public AntPool(Object aSamplePrefab, int aCapacity, Transform aParent = null)
		{
			name = aSamplePrefab.name;
			_objects = new List<AntPoolObject>();
			_samplePrefab = aSamplePrefab;
			_parent = aParent;
			_currentIndex = -1;
			_growCount = 0;

			AntPoolObject po;
			for (int i = 0; i < aCapacity; i++)
			{
				po = CreateNew();
				if (po != null)
				{
					AddObject(po);
				}
			}
		}

		public AntPoolObject CreateNew()
		{
			var go = GameObject.Instantiate((GameObject) _samplePrefab);
			go.name = _samplePrefab.name;

			var po = go.GetComponent<AntPoolObject>();
			if (po != null)
			{
				po.Pool = this;
				if (_parent != null)
				{
					po.transform.SetParent(_parent);
				}
				return po;
			}

			A.Warning($"Component `AntPoolObject` not found for the prefab `{go.name}`.");
			return null;
		}

		public void AddObject(AntPoolObject aObject, bool aCheckCopy = false)
		{
			if (aCheckCopy)
			{
				int index = _objects.IndexOf(aObject);
				if (index >= 0 && index < _objects.Count)
				{
					A.Warning($"Object `{aObject.name}` already added to the pool!");
					return;
				}
			}

			aObject.PlacedToPool();
			_currentIndex++;
			if (_currentIndex < _objects.Count)
			{
				_objects[_currentIndex] = aObject;
			}
			else
			{
				_objects.Add(aObject);
			}
		}

		public T GetObject<T>()
		{
			return GetObject().GetComponent<T>();
		}

		public AntPoolObject GetObject()
		{
			AntPoolObject result;
			if (_currentIndex > -1)
			{
				result = _objects[_currentIndex];
				_objects[_currentIndex] = null;
				result.ExtractedFromPool();
				_currentIndex--;
				return result;
			}

			result = CreateNew();
			result.ExtractedFromPool();
			_growCount++;
			return result;
		}

		public bool HasComponent<T>()
		{
			AntPoolObject result;
			if (_currentIndex > -1)
			{
				result = _objects[_currentIndex];
			}
			else
			{
				result = CreateNew();
				AddObject(result);
			}

			return (result.GetComponent<T>() != null);
		}

		#endregion
	}
}