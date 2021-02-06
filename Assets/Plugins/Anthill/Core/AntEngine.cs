namespace Anthill.Core
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class AntEngine : AntScenario
	{
		private readonly Dictionary<Type, IFamily> _families;
		private List<AntEntity> _entities;
		private List<DelayedCall> _delayedCalls;

		#region Getters / Setters
		
		public static AntEngine Current { get; private set; }
		
		#endregion
		#region Public Methods

		public AntEngine()
		{
			Current = this;
			_families = new Dictionary<Type, IFamily>();
			_entities = new List<AntEntity>();
			_delayedCalls = new List<DelayedCall>();
			_engine = this;
		}

		/// <summary>
		/// Adds all entities from existing GameObject on the scene.
		/// GameObject will be found by name.
		/// </summary>
		/// <param name="aTransformName">Name of the GameObject on scene.</param>
		public void AddEntitiesFromHierarchy(string aTransformName)
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
		public void AddEntitiesFromHierarchy(Transform aParent)
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
		public void RemoveEntitiesFromHierarchy(string aTransformName)
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
		public void RemoveEntitiesFromHierarchy(Transform aParent)
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
		public void AddEntity(GameObject aGameObject, bool aIncludeChildren = false)
		{
			AddEntity(aGameObject.transform, aIncludeChildren);
		}

		/// <summary>
		/// Adds Transform entity to the ECS.
		/// </summary>
		/// <param name="aGameObject">Transform with Entity component to add.</param>
		/// <param name="aIncludeChildren">Checks and adds children entities if true.</param>
		public void AddEntity(Transform aTransform, bool aIncludeChildren = false)
		{
			if (aTransform.gameObject.activeSelf)
			{
				AntEntity entity = aTransform.GetComponent<AntEntity>();
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
		public void AddEntity(AntEntity aEntity)
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
		public void RemoveEntity(AntEntity aEntity)
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
		/// Returns node list.
		/// </summary>
		/// <typeparam name="T">Type of the node list.</typeparam>
		/// <returns>List of nodes by type.</returns>
		public AntNodeList<T> GetNodes<T>()
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
		/// Releases node list.
		/// </summary>
		/// <param name="aNodes">List of nodes for relese.</param>
		/// <typeparam name="T">Type of nodes.</typeparam>
		public void ReleaseNodes<T>(AntNodeList<T> aNodes)
		{
			var type = typeof(T);
			if (_families.ContainsKey(type))
			{
				_families.Remove(type);
			}
		}

		/// <summary>
		/// Processing of all entities.
		/// </summary>
		public override void Execute()
		{
			base.Execute();
			float dt = Time.deltaTime;
			for (int i = _delayedCalls.Count - 1; i >= 0; i--)
			{
				if (_delayedCalls[i].Update(dt))
				{
					_delayedCalls.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Sets delayed call of specified action without arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		public void DelayedCall(float aDelay, Action aFunc)
		{
			var call = new DelayedCall();
			call.SetProcess(aFunc);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}
		
		/// <summary>
		/// Sets delayed call of specified action with one argument.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument for method.</param>
		/// <typeparam name="T1">Type of the argument.</typeparam>
		public void DelayedCall<T1>(float aDelay, Action<T1> aFunc, T1 aArg1)
		{
			var call = new DelayedCall<T1>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}
		
		/// <summary>
		/// Sets delayed call of specified action with two arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		public void DelayedCall<T1, T2>(float aDelay, Action<T1, T2> aFunc, T1 aArg1, T2 aArg2)
		{
			var call = new DelayedCall<T1, T2>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		/// <summary>
		/// Sets delayed call of specified action with three arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <param name="aArg3"></param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns></returns>
		public void DelayedCall<T1, T2, T3>(float aDelay, Action<T1, T2, T3> aFunc, T1 aArg1, T2 aArg2, T3 aArg3)
		{
			var call = new DelayedCall<T1, T2, T3>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		/// <summary>
		/// Sets delayed call of specified action with four arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <param name="aArg3">Argument three.</param>
		/// <param name="aArg4">Argument four.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument.</typeparam>
		/// <returns></returns>
		public void DelayedCall<T1, T2, T3, T4>(float aDelay, Action<T1, T2, T3, T4> aFunc, T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			var call = new DelayedCall<T1, T2, T3, T4>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3, aArg4);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		#endregion
		#region Event Handlers

		private void OnComponentAdded(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentAdded(aEntity, aComponent);
			}
		}

		private void OnComponentRemoved(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentRemoved(aEntity, aComponent);
			}
		}

		#endregion
	}
}