using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Anthill.Core
{
	public class AntFamily<T> : IFamily<T>
	{
		// -----------------------------------------------------
		// Private Variables
		// -----------------------------------------------------

		private readonly AntNodeList<T> _nodes;
		private readonly Dictionary<IEntity, T> _entities;
		private readonly Dictionary<Type, PropertyInfo> _components;
		private readonly Type[] _excludeTypes;
		private readonly AntNodePool<T> _pool;

		// -----------------------------------------------------
		// Getters / Setters
		// -----------------------------------------------------

		/// <summary>
		/// List of nodes.
		/// </summary>
		public AntNodeList<T> Nodes => _nodes;

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public AntFamily(AntNodePool<T> pool = null)
		{
			_nodes = new AntNodeList<T>();
			_entities = new Dictionary<IEntity, T>();
			_pool = pool ?? new AntNodePool<T>();

			var type = typeof(T);
			var excludeAttr = type.GetCustomAttribute<ExcludeAttribute>();
			_excludeTypes = excludeAttr != null ? excludeAttr.excludeTypes : Array.Empty<Type>();

			_components = type.GetProperties()
				.ToDictionary(propInfo => propInfo.PropertyType, propInfo => propInfo);
		}

		public void ComponentAdded(IEntity entity, Type componentType)
		{
			if (_entities.ContainsKey(entity) && HasExclude(componentType))
			{
				RemoveEntity(entity);
				return;
			}

			if (!_entities.ContainsKey(entity))
			{
				AddEntity(entity);
			}
		}

		public void ComponentRemoved(IEntity entity, Type componentType)
		{
			if (_entities.ContainsKey(entity) && HasComponent(componentType))
			{
				RemoveEntity(entity);
				return;
			}

			if (!_entities.ContainsKey(entity) && HasExclude(componentType))
			{
				AddEntity(entity);
			}
		}

		public void EntityAdded(IEntity entity)
		{
			if (!_entities.ContainsKey(entity))
			{
				AddEntity(entity);
			}
		}

		public void EntityRemoved(IEntity entity)
		{
			if (_entities.ContainsKey(entity))
			{
				RemoveEntity(entity);
			}
		}

		// -----------------------------------------------------
		// Private Methods
		// -----------------------------------------------------

		private void AddEntity(IEntity entity)
		{
			foreach (var excludeComponent in _excludeTypes)
			{
				if (entity.Has(excludeComponent))
				{
					return;
				}
			}

			foreach (var pair in _components)
			{
				if (!entity.Has(pair.Key))
				{
					return;
				}
			}

			var node = _pool.Get();
			_entities[entity] = node;

			foreach (var pair in _components)
			{
				pair.Value.SetValue(node, entity.Get(pair.Key), null);
			}

			_nodes.Add(node);
		}

		private void RemoveEntity(IEntity entity)
		{
			var node = _entities[entity];
			_pool.Add(node);
			_entities.Remove(entity);
			_nodes.Remove(node);
		}

		private bool HasComponent(Type componentType)
		{
			foreach (var pair in _components)
			{
				if (pair.Key.Equals(componentType) || pair.Key.IsAssignableFrom(componentType))
				{
					return true;
				}
			}

			return false;
		}

		private bool HasExclude(Type componentType)
		{
			for (int i = 0, n = _excludeTypes.Length; i < n; i++)
			{
				if (_excludeTypes[i].Equals(componentType))
				{
					return true;
				}
			}

			return false;
		}
	}
}