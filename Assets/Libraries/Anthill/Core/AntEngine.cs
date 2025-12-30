using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public class AntEngine
	{
		// -----------------------------------------------------
		// Public Variables
		// -----------------------------------------------------

		public static readonly int InitialEntityListSize = 128;
		public static readonly int InitialNodeListSize = 128;

		// -----------------------------------------------------
		// Private Variables
		// -----------------------------------------------------

		private static readonly Dictionary<Type, IFamily> _families = new();
		private static readonly List<IEntity> _entities = new(InitialEntityListSize);
		private static AntScenario _scenario = new();
		private static int _currentAvailPriority = 0;

		// -----------------------------------------------------
		// Getters / Setters
		// -----------------------------------------------------

		public static AntScenario Scenario
		{
			get => _scenario;
			set => _scenario = value;
		}

		public static List<IEntity> Entities => _entities;

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		/// <summary>
		/// Adds all entities from existing GameObject on the scene.
		/// GameObject will be found by name.
		/// </summary>
		/// <param name="transformName">Name of the GameObject on scene.</param>
		public static void AddEntitiesFromHierarchy(string transformName)
		{
			if (GameObject.Find(transformName) is GameObject go)
			{
				AddEntitiesFromHierarchy(go.transform);
			}
			else
			{
				A.Assert(true, $"Can't find `{transformName}` object on scene.");
			}
		}

		/// <summary>
		/// Adds all entities from specified GameObject.
		/// </summary>
		/// <param name="parent">Transform of the GameObject.</param>
		public static void AddEntitiesFromHierarchy(Transform parent)
		{
			if (parent.TryGetComponent<AntEntity>(out var entity) && entity.allowToAddFromHierachy)
			{
				AddEntity(entity);
			}

			Transform child;
			for (int i = 0, n = parent.childCount; i < n; i++)
			{
				child = parent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					if (child.childCount > 0)
					{
						AddEntitiesFromHierarchy(child);
					}
					else
					{
						if (child.TryGetComponent(out entity) && entity.allowToAddFromHierachy)
						{
							AddEntity(entity);
						}
					}
				}
			}
		}

		/// <summary>
		/// Removes all entities from existing GameObject on the scene.
		/// Transform will be found by name.
		/// </summary>
		/// <param name="transformName">Name of the GameObject on scene.</param>
		public static void RemoveEntitiesFromHierarchy(string transformName)
		{
			if (GameObject.Find(transformName) is GameObject go)
			{
				RemoveEntitiesFromHierarchy(go.transform);
			}
			else
			{
				A.Assert(true, $"Can't find `{transformName}` object on scene.");
			}
		}

		/// <summary>
		/// Removes all entities from specified GameObject.
		/// </summary>
		/// <param name="aParent">Transform of the GameObject.</param>
		public static void RemoveEntitiesFromHierarchy(Transform parent)
		{
			if (parent.TryGetComponent<AntEntity>(out var entity))
			{
				RemoveEntity(entity);
			}

			Transform child;
			for (int i = 0, n = parent.childCount; i < n; i++)
			{
				child = parent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					if (child.TryGetComponent(out entity))
					{
						RemoveEntity(entity);
					}

					if (child.childCount > 0)
					{
						RemoveEntitiesFromHierarchy(child);
					}
				}
			}
		}

		/// <summary>
		/// Adds GameObject entity to the ECS.
		/// </summary>
		/// <param name="gameObject">GameObject with Entity component to add.</param>
		/// <param name="includeChildren">Checks and adds children entities if true.</param>
		public static void AddEntity(GameObject gameObject, bool includeChildren = false)
		{
			AddEntity(gameObject.transform, includeChildren);
		}

		/// <summary>
		/// Adds Transform entity to the ECS.
		/// </summary>
		/// <param name="transform">Transform with Entity component to add.</param>
		/// <param name="includeChildren">Checks and adds children entities if true.</param>
		public static void AddEntity(Transform transform, bool includeChildren = false)
		{
			if (transform.TryGetComponent<AntEntity>(out var entity))
			{
				AddEntity(entity);
			}

			if (includeChildren && transform.childCount > 0)
			{
				for (int i = 0, n = transform.childCount; i < n; i++)
				{
					AddEntity(transform.GetChild(i), includeChildren);
				}
			}
		}

		/// <summary>
		/// Adds specified entity to the ECS.
		/// </summary>
		/// <param name="entity">Entity component.</param>
		public static void AddEntity(IEntity entity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityAdded(entity);
			}

			entity.EventComponentAdded += OnComponentAdded;
			entity.EventComponentRemoved += OnComponentRemoved;
			_entities.Add(entity);
			entity.OnAddedToEngine();
		}

		/// <summary>
		/// Removes specified entity from the ECS.
		/// </summary>
		/// <param name="entity">Entity Component.</param>
		public static void RemoveEntity(IEntity entity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityRemoved(entity);
			}

			entity.EventComponentAdded -= OnComponentAdded;
			entity.EventComponentRemoved -= OnComponentRemoved;
			_entities.Remove(entity);
			entity.OnRemovedFromEngine();
		}

		/// <summary>
		/// Returns list of nodes by Type.
		/// </summary>
		/// <typeparam name="T">Type of the node list.</typeparam>
		/// <returns>List of nodes by type.</returns>
		public static AntNodeList<T> GetNodes<T>()
		{
			var type = typeof(T);
			AntFamily<T> family;
			if (!_families.ContainsKey(type))
			{
				family = new AntFamily<T>();
				_families[type] = family;
				for (int i = 0, n = _entities.Count; i < n; i++)
				{
					family.EntityAdded(_entities[i]);
				}
			}
			else
			{
				family = (AntFamily<T>)_families[type];
			}

			return family.Nodes;
		}

		/// <summary>
		/// Releases list of nodes by Type.
		/// </summary>
		/// <param name="nodes">List of nodes for relese.</param>
		/// <typeparam name="T">Type of nodes.</typeparam>
		public static void ReleaseNodes<T>()
		{
			var type = typeof(T);
			if (_families.ContainsKey(type))
			{
				_families.Remove(type);
			}
		}

		/// <summary>
		/// Creates and adds new system into engine with priority.
		/// </summary>
		/// <param name="priority">Value of the priority (lower value means higher priority).</param>
		/// <typeparam name="T">Type of the system.</typeparam>
		/// <returns>Reference on the new system.</returns>
		public static T Add<T>(int priority = -1) where T : ISystem
		{
			return Scenario.Add<T>(priority == -1 ? _currentAvailPriority++ : priority);
		}

		/// <summary>
		/// Adds new system into engine with priority.
		/// </summary>
		/// <param name="system">Reference on the system.</param>
		/// <param name="priority">Value of the priority (lower value means higher priority).</param>
		/// <returns>Reference on the added system.</returns>
		public static ISystem Add(ISystem system, int priority = -1)
		{
			return Scenario.Add(system, priority == -1 ? _currentAvailPriority++ : priority);
		}

		/// <summary>
		/// Removes system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to remove.</typeparam>
		/// <returns>Reference on the removed system.</returns>
		public static T Remove<T>() where T : ISystem
		{
			return Scenario.Remove<T>();
		}

		/// <summary>
		/// Removes system from the scenario.
		/// </summary>
		/// <param name="aSystem">Reference on the system that need to remove.</param>
		/// <returns>Reference on the removed system.</returns>
		public static ISystem Remove(ISystem system)
		{
			return Scenario.Remove(system);
		}

		/// <summary>
		/// Gets system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to extract.</typeparam>
		/// <returns>Reference on the system.</returns>
		public static T Get<T>() where T : ISystem
		{
			return Scenario.Get<T>();
		}

		/// <summary>
		/// Trying to get system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to extract.</typeparam>
		/// <typeparam name="aResult">Extracted system..</typeparam>
		/// <returns>True if system is extracted or false if not found.</returns>
		public static bool TryGet<T>(out T system) where T : ISystem
		{
			return Scenario.TryGet(out system);
		}

		/// <summary>
		/// Initializes all IInitializeSystem systems.
		/// </summary>
		public static void Initialize()
		{
			Scenario.Initialize();
		}

		/// <summary>
		/// Deinitializes all IDeinitializeSystem systems.
		/// </summary>
		public static void Deinitialize()
		{
			Scenario.Deinitialize();
		}

		/// <summary>
		/// Executes all IExecuteSystem systems.
		/// </summary>
		public static void Execute()
		{
			Scenario.Execute();
		}

		/// <summary>
		/// Executes all IExecuteFixedSystem systems.
		/// </summary>
		public static void ExecuteFixed()
		{
			Scenario.ExecuteFixed();
		}

		/// <summary>
		/// Executes all IExecuteLateSystem systems.
		/// </summary>
		public static void ExecuteLate()
		{
			Scenario.ExecuteLate();
		}

		/// <summary>
		/// Cleanups all ICleanupSystem systems.
		/// </summary>
		public static void Cleanup()
		{
			Scenario.Cleanup();
		}

		/// <summary>
		/// Disable all IDisableSystem systems.
		/// </summary>
		public static void Disable()
		{
			Scenario.Disable();
		}

		/// <summary>
		/// Enable all IEnableSystem systems.
		/// </summary>
		public static void Enable()
		{
			Scenario.Enable();
		}

		/// <summary>
		/// Reset all IResetSystem systems.
		/// </summary>
		public static void Reset()
		{
			Scenario.Reset();
		}

		// -----------------------------------------------------
		// Event Handlers
		// -----------------------------------------------------

		private static void OnComponentAdded(IEntity entity, Type component)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentAdded(entity, component);
			}
		}

		private static void OnComponentRemoved(IEntity entity, Type component)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentRemoved(entity, component);
			}
		}
	}
}