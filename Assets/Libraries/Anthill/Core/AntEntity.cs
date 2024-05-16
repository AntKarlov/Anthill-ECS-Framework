using System;
using UnityEngine;

namespace Anthill.Core
{
	public class AntEntity : MonoBehaviour
	{
	#region Public Variables
	
		public delegate void ComponentChangeDelegate(AntEntity aEntity, Type aComponentType);
		public delegate void EntityDelegate(AntEntity aEntity);

		/// <summary>
		/// Called when added new component on the entity.
		/// </summary>
		public event ComponentChangeDelegate EventComponentAdded;

		/// <summary>
		/// Called when removed component from the entity.
		/// </summary>
		public event ComponentChangeDelegate EventComponentRemoved;

		/// <summary>
		/// Called when entity added to the ECS engine.
		/// </summary>
		public event EntityDelegate EventAddedToEngine;

		/// <summary>
		/// Called when entity removed from the ECS engine.
		/// </summary>
		public event EntityDelegate EventRemovedFromEngine;

		public bool allowToAddFromHierachy = true;
	
	#endregion

	#region Private Variables

		protected Transform _cachedTransform;
		protected bool _isInit;

	#endregion

	#region Getters Setters

		/// <summary>
		/// Determines is added into ECS engine.
		/// </summary>
		public bool IsAddedToEngine { get; private set; }

		/// <summary>
		/// Cashed transform of the entity.
		/// </summary>
		public Transform Transform
		{
			get 
			{
				Init();
				return _cachedTransform;
			}
		}

		/// <summary>
		/// Position of the entity transform.
		/// </summary>
		public Vector3 Position
		{
			get
			{
				Init();
				return _cachedTransform.position;
			}
			set
			{
				Init();
				_cachedTransform.position = value;
			}
		}

		/// <summary>
		/// Rotation of the entity transform.
		/// </summary>
		public Quaternion Rotation
		{
			get
			{
				Init();
				return _cachedTransform.rotation;
			}
			set
			{
				Init();
				_cachedTransform.rotation = value;
			}
		}

	#endregion

	#region Unity Calls

		// ..

	#endregion

	#region Public Methods

		/// <summary>
		/// Checks specified component on entity.
		/// </summary>
		/// <param name="aType">Type of the component.</param>
		/// <returns>True if component exists.</returns>
		public bool Has(Type aType)
		{
			return gameObject.GetComponent(aType) != null;
		}

		/// <summary>
		/// Cheks specified component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>True if component exists.</returns>
		public bool Has<T>() where T : Component
		{
			return gameObject.TryGetComponent<T>(out var component);
		}

		/// <summary>
		/// Returns specified component on the entity.
		/// </summary>
		/// <param name="aType">Type of the component.</param>
		/// <returns>Component.</returns>
		public object Get(Type aType)
		{
			return gameObject.GetComponent(aType);
		}

		/// <summary>
		/// Returns specified component on the entity.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>() where T : Component
		{
			return gameObject.TryGetComponent<T>(out var component) ? component : null;
		}

		/// <summary>
		/// Adds specified component to the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>Returns reference on the added component.</returns>
		public T Add<T>() where T : Component
		{
			var comp = gameObject.AddComponent<T>();
			EventComponentAdded?.Invoke(this, typeof(T));
			return comp;
		}

		/// <summary>
		/// Removes specified component on the entity.
		/// </summary>
		/// <param name="aComponent">Component.</param>
		/// <returns>Returns reference on the removed component.</returns>
		public Component Remove(Component aComponent)
		{
			DestroyComponent(aComponent);
			EventComponentRemoved?.Invoke(this, aComponent.GetType());
			return aComponent;
		}

		/// <summary>
		/// Removes specified component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>Returns reference on the removed component.</returns>
		public T Remove<T>() where T : Component
		{
			var comp = gameObject.GetComponent<T>();
			DestroyComponent(comp);
			EventComponentRemoved?.Invoke(this, comp.GetType());
			return comp;
		}

	#endregion

	#region Protected Methods

		internal void Init()
		{
			if (_isInit) return;
			_isInit = true;
			_cachedTransform = GetComponent<Transform>();
		}

		internal void OnAddedToEngine()
		{
			IsAddedToEngine = true;
			EventAddedToEngine?.Invoke(this);
		}

		internal void OnRemovedFromEngine()
		{
			IsAddedToEngine = false;
			EventRemovedFromEngine?.Invoke(this);
		}

		protected virtual void DestroyComponent(Component aComponent)
		{
			Destroy(aComponent);
		}

	#endregion
	}
}