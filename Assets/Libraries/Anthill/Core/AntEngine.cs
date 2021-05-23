namespace Anthill.Core
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class AntEngine
	{
	#region Private Variables

		private static readonly Dictionary<Type, IFamily> _families = new Dictionary<Type, IFamily>();
		private static List<AntEntity> _entities = new List<AntEntity>();
		private static AntScenario _scenario = new AntScenario();

	#endregion

	#region Getters / Setters
		
		public static AntScenario Scenario
		{
			get => _scenario;
			set => _scenario = value;
		}
		
	#endregion

	#region Public Methods

		/// <summary>
		/// Adds all entities from existing GameObject on the scene.
		/// GameObject will be found by name.
		/// </summary>
		/// <param name="aTransformName">Name of the GameObject on scene.</param>
		public static void AddEntitiesFromHierarchy(string aTransformName)
		{
			var go = GameObject.Find(aTransformName);
			A.Assert(go == null, $"Can't find `{aTransformName}` object on scene.");
			if (go != null)
			{
				AddEntitiesFromHierarchy(go.transform);
			}
		}

		/// <summary>
		/// Adds all entities from specified GameObject.
		/// </summary>
		/// <param name="aParent">Transform of the GameObject.</param>
		public static void AddEntitiesFromHierarchy(Transform aParent)
		{
			Transform child;
			AntEntity entity;
			for (int i = 0, n = aParent.childCount; i < n; i++)
			{
				child = aParent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					entity = child.GetComponent<AntEntity>();
					if (entity != null)
					{
						AddEntity(entity);
					}

					if (child.childCount > 0)
					{
						AddEntitiesFromHierarchy(child);
					}
				}
			}
		}

		/// <summary>
		/// Removes all entities from existing GameObject on the scene.
		/// Transform will be found by name.
		/// </summary>
		/// <param name="aTransformName">Name of the GameObject on scene.</param>
		public static void RemoveEntitiesFromHierarchy(string aTransformName)
		{
			var go = GameObject.Find(aTransformName);
			A.Assert(go == null, $"Can't find `{aTransformName}` object on scene.");
			if (go != null)
			{
				RemoveEntitiesFromHierarchy(go.transform);
			}
		}

		/// <summary>
		/// Removes all entities from specified GameObject.
		/// </summary>
		/// <param name="aParent">Transform of the GameObject.</param>
		public static void RemoveEntitiesFromHierarchy(Transform aParent)
		{
			Transform child;
			AntEntity entity;
			for (int i = 0, n = aParent.childCount; i < n; i++)
			{
				child = aParent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					entity = child.GetComponent<AntEntity>();
					if (entity != null)
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
		/// <param name="aGameObject">GameObject with Entity component to add.</param>
		/// <param name="aIncludeChildren">Checks and adds children entities if true.</param>
		public static void AddEntity(GameObject aGameObject, bool aIncludeChildren = false)
		{
			AddEntity(aGameObject.transform, aIncludeChildren);
		}

		/// <summary>
		/// Adds Transform entity to the ECS.
		/// </summary>
		/// <param name="aGameObject">Transform with Entity component to add.</param>
		/// <param name="aIncludeChildren">Checks and adds children entities if true.</param>
		public static void AddEntity(Transform aTransform, bool aIncludeChildren = false)
		{
			if (aTransform.gameObject.activeSelf)
			{
				var entity = aTransform.GetComponent<AntEntity>();
				if (entity != null)
				{
					AddEntity(entity);
					if (aIncludeChildren && aTransform.childCount > 0)
					{
						for (int i = 0, n = aTransform.childCount; i < n; i++)
						{
							AddEntity(aTransform.GetChild(i), aIncludeChildren);
						}
					}
				}
			}
		}

		/// <summary>
		/// Adds specified entity to the ECS.
		/// </summary>
		/// <param name="aEntity">Entity component.</param>
		public static void AddEntity(AntEntity aEntity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityAdded(aEntity);
			}

			aEntity.EventComponentAdded += OnComponentAdded;
			aEntity.EventComponentRemoved += OnComponentRemoved;
			_entities.Add(aEntity);
			aEntity.OnAddedToEngine();
		}

		/// <summary>
		/// Removes specified entity from the ECS.
		/// </summary>
		/// <param name="aEntity">Entity Component.</param>
		public static void RemoveEntity(AntEntity aEntity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityRemoved(aEntity);
			}

			aEntity.EventComponentAdded -= OnComponentAdded;
			aEntity.EventComponentRemoved -= OnComponentRemoved;
			_entities.Remove(aEntity);
			aEntity.OnRemovedFromEngine();
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
				family = (AntFamily<T>) _families[type];
			}

			return family.Nodes;
		}

		/// <summary>
		/// Releases list of nodes by Type.
		/// </summary>
		/// <param name="aNodes">List of nodes for relese.</param>
		/// <typeparam name="T">Type of nodes.</typeparam>
		public static void ReleaseNodes<T>(AntNodeList<T> aNodes)
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
		/// <param name="aPriority">Value of the priority (lower value means higher priority).</param>
		/// <typeparam name="T">Type of the system.</typeparam>
		/// <returns>Reference on the new system.</returns>
		public static T Add<T>(int aPriority = 0) where T : ISystem
		{
			return Scenario.Add<T>(aPriority);
		}

		/// <summary>
		/// Adds new system into engine with priority.
		/// </summary>
		/// <param name="aSystem">Reference on the system.</param>
		/// <param name="aPriority">Value of the priority (lower value means higher priority).</param>
		/// <returns>Reference on the added system.</returns>
		public static ISystem Add(ISystem aSystem, int aPriority = 0)
		{
			return (ISystem) Scenario.Add(aSystem, aPriority);
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
		public static ISystem Remove(ISystem aSystem)
		{
			return Scenario.Remove(aSystem);
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

	#endregion

	#region Event Handlers

		private static void OnComponentAdded(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentAdded(aEntity, aComponent);
			}
		}

		private static void OnComponentRemoved(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentRemoved(aEntity, aComponent);
			}
		}

	#endregion
	}
}