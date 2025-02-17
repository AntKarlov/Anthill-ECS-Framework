using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	[AddComponentMenu("Anthill/Core/AntEntity")]
	public class AntEntity : MonoBehaviour, IEntity
	{
	#region Public Variables

		[Tooltip("If true, then entity can be added into ECS from AddFromHiearachy methods.")]
		public bool allowToAddFromHierachy = true;
	
	#endregion

	#region Private Variables

		private bool _isMonoInitialized;
		private List<object> _components = new();

	#endregion

	#region Getters Setters

		/// <summary>
		/// Return reference to the list with all components.
		/// </summary>
		public List<object> Components
		{
			get
			{
				SetupMonoComponents();
				return _components;
			}
		}

		/// <summary>
		/// Returns component by index from the component list.
		/// </summary>
		public object this[int index]
		{
			get
			{
				SetupMonoComponents();
				return index >= 0 && index < _components.Count ? _components[index] : null;
			}
		} 

	#endregion

	#region IEntity Implementation

		/// <summary>
		/// Called when added new component on the entity.
		/// </summary>
		public event ComponentDelegate EventComponentAdded;

		/// <summary>
		/// Called when removed component from the entity.
		/// </summary>
		public event ComponentDelegate EventComponentRemoved;

		/// <summary>
		/// Called when entity added to the ECS engine.
		/// </summary>
		public event EntityDelegate EventAddedToEngine;

		/// <summary>
		/// Called when entity removed from the ECS engine.
		/// </summary>
		public event EntityDelegate EventRemovedFromEngine;

		/// <summary>
		/// Determines if entity can be added into engine from AddFromHiearachy methods (Mono).
		/// </summary>
		public bool AllowToAddFromHierachy => allowToAddFromHierachy;

		/// <summary>
		/// Determines is added into ECS engine.
		/// </summary>
		public bool IsAddedToEngine { get; private set; }

		/// <summary>
		/// Checks specified component on entity.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns>True if component exists.</returns>
		public bool Has(Type componentType)
		{
			SetupMonoComponents();
			return GetComponentIndex(componentType) >= 0;
		}

		/// <summary>
		/// Cheks specified component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>True if component exists.</returns>
		public bool Has<T>() where T : class
		{
			SetupMonoComponents();
			return GetComponentIndex<T>() >= 0;
		}

		/// <summary>
		/// Returns specified component on the entity or null if component not exists.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns>Reference on component or null if component not exists.</returns>
		public object Get(Type componentType)
		{
			SetupMonoComponents();
			int index = GetComponentIndex(componentType);
			return index >= 0 && index < _components.Count ? _components[index] : null;
		}

		/// <summary>
		/// Trying to get component on the entity.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="component">Getted component or null if component not exists.</param>
		/// <returns>Return true if component exists.</returns>
		public bool TryGet(Type componentType, out object component)
		{
			component = Get(componentType);
			return component != null;
		}

		/// <summary>
		/// Returns specified component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>Returns component or null if component not exists.</returns>
		public T Get<T>() where T : class
		{
			SetupMonoComponents();
			int index = GetComponentIndex<T>();
			return index >= 0 && index < _components.Count ? (T) _components[index] : null;
		}

		/// <summary>
		/// Trying to get component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <param name="component">Getted component or null if component not exists.</param>
		/// <returns>Return true if component exists.</returns>
		public bool TryGet<T>(out T component) where T : class
		{
			component = Get<T>();
			return component != null;
		}

		/// <summary>
		/// Adds simple component to the entity if they don't have components with same types.
		/// If component with same type already existing, then returns reference on the existing component.
		/// </summary>
		/// <param name="component">Component to add.</param>
		/// <returns>Returns reference to the added or existing component.</returns>
		public object Add(object component)
		{
			SetupMonoComponents();
			var type = component.GetType();
			int index = GetComponentIndex(type);
			if (index >= 0 && index < _components.Count)
			{
				return _components[index];
			}

			if (type.IsSubclassOf(typeof(Component)))
			{
				var monoComponent = gameObject.AddComponent(type);
				_components.Add(monoComponent);
				EventComponentAdded?.Invoke(this, type);
				return monoComponent;
			}
			else
			{
				_components.Add(component);
				EventComponentAdded?.Invoke(this, type);
				return component;
			}
		}

		/// <summary>
		/// Adds simple component to the entity if they don't have components with same types.
		/// If component with same type already existing, then returns reference on the existing component. 
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>Returns reference on the added or existing component.</returns>
		public T Add<T>() where T : class
		{
			SetupMonoComponents();
			int index = GetComponentIndex<T>();
			if (index >= 0 && index < _components.Count)
			{
				return (T) _components[index];
			}

			var type = typeof(T);
			if (type.IsSubclassOf(typeof(Component)))
			{
				var monoComponent = gameObject.AddComponent(type);
				_components.Add(monoComponent);
				EventComponentAdded?.Invoke(this, type);
				return monoComponent as T;
			}
			else
			{
				var component = Activator.CreateInstance<T>();
				_components.Add(component);
				EventComponentAdded?.Invoke(this, type);
				return component;
			}
		}

		/// <summary>
		/// Removes component on the entity.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Returns reference on the removed component or null if component not existing.</returns>
		public object Remove(object component)
		{
			SetupMonoComponents();
			object result = null;
			var type = component.GetType();
			var index = GetComponentIndex(type);
			if (index >= 0 && index < _components.Count)
			{
				if (component is Component monoComponent)
				{
					DestroyComponent(monoComponent);
					result = monoComponent;
				}
				else
				{
					result = component;
				}

				_components.RemoveAt(index);
				EventComponentRemoved?.Invoke(this, type);
			}

			return result;
		}

		/// <summary>
		/// Removes component on the entity.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <returns>Returns reference on the removed component or null if component not existing.</returns>
		public T Remove<T>() where T : class
		{
			SetupMonoComponents();
			int index = GetComponentIndex<T>();
			if (index >= 0 && index < _components.Count)
			{
				var component = _components[index];
				if (component is Component monoComponent)
				{
					DestroyComponent(monoComponent);
				}

				_components.RemoveAt(index);
				EventComponentRemoved?.Invoke(this, component.GetType());
				return (T) component;
			}

			return default;
		}

		/// <summary>
		/// Removes component on the entity if component existing.
		/// </summary>
		/// <typeparam name="T">Type of component to remove.</typeparam>
		/// <param name="component">Reference on removed component or null if component not exists.</param>
		/// <returns>True if component was removed.</returns>
		public bool TryRemove<T>(out object component) where T : class
		{
			component = Remove<T>();
			return component != null;
		}

		/// <summary>
		/// Called when entity is added to the engine.
		/// </summary>
		public void OnAddedToEngine()
		{
			IsAddedToEngine = true;
			EventAddedToEngine?.Invoke(this);
		}

		/// <summary>
		/// Called when entity is removed from the engine.
		/// </summary>
		public void OnRemovedFromEngine()
		{
			IsAddedToEngine = false;
			EventRemovedFromEngine?.Invoke(this);
		}

	#endregion

	#region Protected Methods

		private void SetupMonoComponents()
		{
			if (_isMonoInitialized) return;
			_isMonoInitialized = true;

			var monoComponents = GetComponents<Component>();
			_components = new List<object>();

			for (int i = 0, n = monoComponents.Length; i < n; i++)
			{
				_components.Add(monoComponents[i]);
			}

			// Add GameObject as component in the list.
			_components.Add(gameObject);
		}

		protected virtual void DestroyComponent(Component component)
		{
			Destroy(component);
		}

		private int GetComponentIndex<T>()
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				if (_components[i] is T || typeof(T).IsAssignableFrom(_components[i].GetType()))
				{
					return i;
				}
			}

			return -1;
		}

		private int GetComponentIndex(Type componentType)
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				var type = _components[i].GetType();
				if (type.Equals(componentType) || componentType.IsAssignableFrom(type))
				{
					return i;
				}
			}

			return -1;
		}

	#endregion
	}
}