namespace Anthill.Pools
{
	using System.Collections.Generic;
	using UnityEngine;

	public class AntPoolContainer
	{
		public string name;

		protected List<AntPoolable> _objects;
		protected Object _samplePrefab;
		protected Transform _parent;
		protected int _currentIndex;

		protected bool _isGrow;
		protected bool _isLimitCapactiy;
		protected int _maxCapacity;

	#region Getters / Setters

		public int GrowCount { get; private set; } = 0;

	#endregion

	#region Public Methods

		public AntPoolContainer(AntPoolPreset.Item aSource, Transform aParent = null)
		{
			name = aSource.prefab.name;
			_objects = new List<AntPoolable>();
			_samplePrefab = aSource.prefab;
			_parent = aParent;
			_currentIndex = -1;

			_isGrow = aSource.isGrow;
			_isLimitCapactiy = aSource.isLimitCapacity;
			_maxCapacity = aSource.maxCapacity;

			for (int i = 0; i < aSource.initialSize; i++)
			{
				Add(Populate());
			}
		}

		public AntPoolable Populate()
		{
			GrowCount++;

			var go = GameObject.Instantiate((GameObject) _samplePrefab);
			go.name = _samplePrefab.name;

			var po = go.GetComponent<AntPoolable>();
			if (po != null)
			{
				po.Pool = this;
				if (_parent != null)
				{
					po.transform.SetParent(_parent);
				}
				return po;
			}

			A.Editor.Warning($"Component `AntPoolable` not found for the prefab `{go.name}`.", this);
			return null;
		}

		public void Add(AntPoolable aPoolable)
		{
			if (aPoolable == null)
			{
				A.Editor.Warning($"Can't add object into pool because it is null!", this);
				return;
			}

			aPoolable.ReturnedToPool();

			_currentIndex++;
			if (_currentIndex < _objects.Count)
			{
				_objects[_currentIndex] = aPoolable;
			}
			else
			{
				_objects.Add(aPoolable);
			}
		}

		public T Get<T>()
		{
			return Get().GetComponent<T>();
		}

		public AntPoolable Get()
		{
			AntPoolable result;
			if (_currentIndex > -1)
			{
				result = _objects[_currentIndex];
				_objects[_currentIndex] = null;
				_currentIndex--;

				result.ExtractedFromPool();
				return result;
			}

			if (_isGrow)
			{
				if (_isLimitCapactiy && GrowCount >= _maxCapacity)
				{
					return null;
				}
				
				result = Populate();
				result.ExtractedFromPool();
				return result;
			}

			return null;
		}

		public bool Has<T>()
		{
			if (_currentIndex <= -1)
			{
				Add(Populate());
			}

			return (_currentIndex >= 0 && _currentIndex < _objects.Count)
				? (_objects[_currentIndex].GetComponent<T>() != null)
				: false;
		}

	#endregion
	}
}