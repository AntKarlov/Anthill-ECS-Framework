using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Anthill.Core
{
	public class AntBaseScenario : ISystem, IInitializeSystem, IDeinitializeSystem, IExecuteSystem,
		IExecuteFixedSystem, IExecuteLateSystem, ICleanupSystem, IEnableSystem, IDisableSystem, IResetSystem
	{
	#region Private Variables

		protected enum PendingChange
		{
			Add,
			Remove
		}

		private int _lockCount;
		protected bool _enabled;

		protected List<AntPriorityPair<ISystem>> _systems;
		protected List<AntPriorityPair<IInitializeSystem>> _initializeSystems;
		protected List<AntPriorityPair<IDeinitializeSystem>> _deinitializeSystems;
		protected List<AntPriorityPair<IExecuteSystem>> _executeSystems;
		protected List<AntPriorityPair<IExecuteFixedSystem>> _executeFixedSystems;
		protected List<AntPriorityPair<IExecuteLateSystem>> _executeLateSystems;
		protected List<AntPriorityPair<IEnableSystem>> _enableSystems;
		protected List<AntPriorityPair<IDisableSystem>> _disableSystems;
		protected List<AntPriorityPair<ICleanupSystem>> _cleanupSystems;
		protected List<AntPriorityPair<IResetSystem>> _resetSystems;
		protected List<KeyValuePair<AntPriorityPair<ISystem>, PendingChange>> _pending;

	#endregion

	#region Getters Setters

		/// <summary>
		/// Name of the scenario, uses for the debug states.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Count of the locks.
		/// </summary>
		public bool IsLocked => _lockCount > 0;

	#endregion

	#region Public Methods

		public AntBaseScenario(string name)
		{
			Name = name;

			_systems = new List<AntPriorityPair<ISystem>>();
			_initializeSystems = new List<AntPriorityPair<IInitializeSystem>>();
			_deinitializeSystems = new List<AntPriorityPair<IDeinitializeSystem>>();
			_executeSystems = new List<AntPriorityPair<IExecuteSystem>>();
			_executeFixedSystems = new List<AntPriorityPair<IExecuteFixedSystem>>();
			_executeLateSystems = new List<AntPriorityPair<IExecuteLateSystem>>();
			_enableSystems = new List<AntPriorityPair<IEnableSystem>>();
			_disableSystems = new List<AntPriorityPair<IDisableSystem>>();
			_cleanupSystems = new List<AntPriorityPair<ICleanupSystem>>();
			_resetSystems = new List<AntPriorityPair<IResetSystem>>();
			_pending = new List<KeyValuePair<AntPriorityPair<ISystem>, PendingChange>>();

			_enabled = true;
		}

		/// <summary>
		/// Checks if system already added into scenario.
		/// </summary>
		/// <typeparam name="T">Type of the system.</typeparam>
		/// <returns>Returns true if system already added into scenario.</returns>
		public virtual bool Has<T>() where T : ISystem
		{
			int index = _systems.FindIndex(x => x.System is T);
			return index >= 0 && index < _systems.Count;
		}

		/// <summary>
		/// Creates and adds new system into engine with priority.
		/// </summary>
		/// <param name="priority">Value of the priority (lower value means higher priority).</param>
		/// <typeparam name="T">Type of the system.</typeparam>
		/// <returns>Reference on the new system.</returns>
		public virtual T Add<T>(int priority = 0) where T : ISystem
		{
			Type type = typeof(T);
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			var system = (ISystem) constructor.Invoke(null);
			// A.Assert(system == null, $"Class `{type.ToString()}` not implemented <b>ISystem</b> interface!");
			return (T) Add(system, priority);
		}

		/// <summary>
		/// Adds new system into engine with priority.
		/// </summary>
		/// <param name="system">Reference on the system.</param>
		/// <param name="priority">Value of the priority (lower value means higher priority).</param>
		/// <returns>Reference on the added system.</returns>
		public virtual ISystem Add(ISystem system, int priority = 0)
		{
			if (IsLocked)
			{
				_pending.Add(
					new KeyValuePair<AntPriorityPair<ISystem>, PendingChange>(
						new AntPriorityPair<ISystem>(system, priority), 
						PendingChange.Add
					)
				);

				return system;
			}

			_systems.Add(new AntPriorityPair<ISystem>(system, priority));
			_systems = _systems.OrderBy(x => x.Priority).ToList();

			if (system is IInitializeSystem initializeSystem)
			{
				_initializeSystems.Add(new AntPriorityPair<IInitializeSystem>(initializeSystem, priority));
				_initializeSystems = _initializeSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IDeinitializeSystem deinitializeSystem)
			{
				_deinitializeSystems.Add(new AntPriorityPair<IDeinitializeSystem>(deinitializeSystem, priority));
				_deinitializeSystems = _deinitializeSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IDisableSystem disableSystem)
			{
				_disableSystems.Add(new AntPriorityPair<IDisableSystem>(disableSystem, priority));
				_disableSystems = _disableSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IEnableSystem enableSystem)
			{
				_enableSystems.Add(new AntPriorityPair<IEnableSystem>(enableSystem, priority));
				_enableSystems = _enableSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IExecuteSystem executeSystem)
			{
				_executeSystems.Add(new AntPriorityPair<IExecuteSystem>(executeSystem, priority));
				_executeSystems = _executeSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IExecuteFixedSystem executeFixedSystem)
			{
				_executeFixedSystems.Add(new AntPriorityPair<IExecuteFixedSystem>(executeFixedSystem, priority));
				_executeFixedSystems = _executeFixedSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IExecuteLateSystem executeLateSystem)
			{
				_executeLateSystems.Add(new AntPriorityPair<IExecuteLateSystem>(executeLateSystem, priority));
				_executeLateSystems = _executeLateSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is ICleanupSystem cleanupSystem)
			{
				_cleanupSystems.Add(new AntPriorityPair<ICleanupSystem>(cleanupSystem, priority));
				_cleanupSystems = _cleanupSystems.OrderBy(x => x.Priority).ToList();
			}

			if (system is IResetSystem resetSystem)
			{
				_resetSystems.Add(new AntPriorityPair<IResetSystem>(resetSystem, priority));
				_resetSystems = _resetSystems.OrderBy(x => x.Priority).ToList();
			}
			
			system.AddedToEngine();
			return system;
		}

		/// <summary>
		/// Removes system from the scenario.
		/// </summary>
		/// <param name="system">Reference on the system that need to remove.</param>
		/// <returns>Reference on the removed system.</returns>
		public virtual ISystem Remove(ISystem system)
		{
			if (IsLocked)
			{
				_pending.Add(
					new KeyValuePair<AntPriorityPair<ISystem>, PendingChange>(
						new AntPriorityPair<ISystem>(system, 0),
						PendingChange.Remove
					)
				);
				return system;
			}

			_systems.RemoveAll(x => Object.ReferenceEquals(x.System, system));

			if (system is IInitializeSystem initializeSystem)
			{
				_initializeSystems.RemoveAll(x => Object.ReferenceEquals(x.System, initializeSystem));
			}
			
			if (system is IDeinitializeSystem deinitializeSystem)
			{
				_deinitializeSystems.RemoveAll(x => Object.ReferenceEquals(x.System, deinitializeSystem));
			}

			if (system is IDisableSystem disableSystem)
			{
				_disableSystems.RemoveAll(x => Object.ReferenceEquals(x.System, disableSystem));
			}

			if (system is IEnableSystem enableSystem)
			{
				_enableSystems.RemoveAll(x => Object.ReferenceEquals(x.System, enableSystem));
			}

			if (system is IExecuteSystem executeSystem)
			{
				_executeSystems.RemoveAll(x => Object.ReferenceEquals(x.System, executeSystem));
			}

			if (system is IExecuteFixedSystem executeFixedSystem)
			{
				_executeFixedSystems.RemoveAll(x => Object.ReferenceEquals(x.System, executeFixedSystem));
			}

			if (system is IExecuteLateSystem executeLateSystem)
			{
				_executeLateSystems.RemoveAll(x => Object.ReferenceEquals(x.System, executeLateSystem));
			}

			if (system is ICleanupSystem cleanupSystem)
			{
				_cleanupSystems.RemoveAll(x => Object.ReferenceEquals(x.System, cleanupSystem));
			}

			if (system is IResetSystem resetSystem)
			{
				_resetSystems.RemoveAll(x => Object.ReferenceEquals(x.System, resetSystem));
			}

			system.RemovedFromEngine();
			return system;
		}

		/// <summary>
		/// Removes system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to remove.</typeparam>
		/// <returns>Reference on the removed system.</returns>
		public T Remove<T>() where T : ISystem
		{
			var system = Get<T>();
			if (system is ISystem sys) Remove(sys);
			return system;
		}

		/// <summary>
		/// Gets system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to extract.</typeparam>
		/// <returns>Reference on the system.</returns>
		public T Get<T>() where T : ISystem
		{
			int index = _systems.FindIndex(x => x.System is T);
			return (index >= 0 && index < _systems.Count)
				? (T) _systems[index].System
				: default;
		}

		/// <summary>
		/// Try to get system from the scenario by Type.
		/// </summary>
		/// <typeparam name="T">Type of the system that need to extract.</typeparam>
		/// <typeparam name="aResult">Referense on the extracted system.</typeparam>
		/// <returns>True if the system is extracted or false if not found.</returns>
		public bool TryGet<T>(out T component) where T : ISystem
		{
			int index = _systems.FindIndex(x => x.System is T);
			if (index >= 0 && index < _systems.Count)
			{
				component = (T) _systems[index].System;
				return true;
			}
			
			component = default;
			return false;
		}

	#endregion

	#region Protected Methods

		private void Lock()
		{
			_lockCount++;
		}

		private void Unlock()
		{
			_lockCount--;
			if (_lockCount <= 0)
			{
				_lockCount = 0;
				ApplyPending();
			}
		}

		private void ApplyPending()
		{
			KeyValuePair<AntPriorityPair<ISystem>, PendingChange> pair;
			for (int i = 0, n = _pending.Count; i < n; i++)
			{
				pair = _pending[i];
				if (pair.Value == PendingChange.Add)
				{
					Add(pair.Key.System, pair.Key.Priority);
				}
				else
				{
					Remove(pair.Key.System);
				}
			}

			_pending.Clear();
		}

	#endregion

	#region ISystem Implementation

		public virtual void AddedToEngine()
		{
			// ..
		}

		public virtual void RemovedFromEngine()
		{
			// ..
		}

	#endregion

	#region IInitializeSystem Implementation

		/// <summary>
		/// Initializes all IInitializeSystem systems.
		/// </summary>
		public virtual void Initialize()
		{
			for (int i = _initializeSystems.Count - 1; i >= 0; i--)
			{
				_initializeSystems[i].System.Initialize();
			}
		}

	#endregion

	#region IDeinitializeSystem Implementation

		/// <summary>
		/// Deinitializes all IDeinitializeSystem systems.
		/// </summary>
		public virtual void Deinitialize()
		{
			for (int i = _deinitializeSystems.Count - 1; i >= 0; i--)
			{
				_deinitializeSystems[i].System.Deinitialize();
			}
		}

	#endregion

	#region IExecuteSystem Implementation
		
		/// <summary>
		/// Executes all IExecuteSystem systems.
		/// </summary>
		public virtual void Execute()
		{
			if (_enabled)
			{
				for (int i = _executeSystems.Count - 1; i >= 0; i--)
				{
					_executeSystems[i].System.Execute();
				}
			}
		}

	#endregion

	#region IExecuteFixed Implementation
		
		/// <summary>
		/// Executes all IExecuteFixedSystem systems.
		/// </summary>
		public virtual void ExecuteFixed()
		{
			if (_enabled)
			{
				for (int i = _executeFixedSystems.Count - 1; i >= 0; i--)
				{
					_executeFixedSystems[i].System.ExecuteFixed();
				}
			}
		}
		
	#endregion

	#region IExecuteLate Implementation

		/// <summary>
		/// Executes all IExecuteLateSystem systems.
		/// </summary>
		public virtual void ExecuteLate()
		{
			if (_enabled)
			{
				for (int i = _executeLateSystems.Count - 1; i >= 0; i--)
				{
					_executeLateSystems[i].System.ExecuteLate();
				}
			}
		}

	#endregion

	#region ICleanupSystem Implementation

		/// <summary>
		/// Cleanups all ICleanupSystem systems.
		/// </summary>
		public virtual void Cleanup()
		{
			if (_enabled)
			{
				for (int i = _cleanupSystems.Count - 1; i >= 0; i--)
				{
					_cleanupSystems[i].System.Cleanup();
				}
			}
		}

	#endregion

	#region IDisableSystem Implementation

		/// <summary>
		/// Disable all IDisableSystem systems.
		/// </summary>
		public virtual void Disable()
		{
			for (int i = _disableSystems.Count - 1; i >= 0; i--)
			{
				_disableSystems[i].System.Disable();
			}

			_enabled = false;
		}

	#endregion

	#region IEnableSystem Implementation

		/// <summary>
		/// Enable all IEnableSystem systems.
		/// </summary>
		public virtual void Enable()
		{
			for (int i = _enableSystems.Count - 1; i >= 0; i--)
			{
				_enableSystems[i].System.Enable();
			}

			_enabled = true;
		}

	#endregion

	#region IResetSystem Implementation
		
		/// <summary>
		/// Reset all IResetSystem systems.
		/// </summary>
		public virtual void Reset()
		{
			for (int i = _resetSystems.Count - 1; i >= 0; i--)
			{
				_resetSystems[i].System.Reset();
			}
		}
		
	#endregion
	}
}