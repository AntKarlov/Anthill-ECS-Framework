namespace Anthill.Task
{
	using System.Collections.Generic;
	using Anthill.Core;

	public class AntTaskManagerScenario : AntScenario
	{
		// protected List<KeyValuePair<AntTaskManager, PendingChange>> _taskPending;
		private List<AntTaskManager> _managers;
		// private int _lockCount = 0;

		#region Getters / Setters
		
		// public bool IsTaskManagerLocked { get => _lockCount > 0; }
		
		#endregion
		#region Public Methods

		public AntTaskManagerScenario() : base("Task Manager Scenario")
		{
			// ..
		}

		public override void AddedToEngine()
		{
			base.AddedToEngine();
			_managers = new List<AntTaskManager>();
		}

		public override void RemovedFromEngine()
		{
			base.RemovedFromEngine();
			_managers.Clear();
			_managers = null;
		}

		public override void Execute()
		{
			base.Execute();
			for (int i = _managers.Count - 1; i >= 0; i--)
			{
				_managers[i].Execute();
			}
		}

		public void KillAll()
		{
			for (int i = _managers.Count - 1; i >= 0; i--)
			{
				_managers[i].Kill();
			}
		}

		public void Add(AntTaskManager aTaskManager)
		{
			// if (IsLocked)
			// {
			// 	_taskPending.Add(
			// 		new KeyValuePair<AntTaskManager, PendingChange>(
			// 			aTaskManager, 
			// 			PendingChange.Add
			// 		)
			// 	);
			// 	return;
			// }

			_managers.Add(aTaskManager);
		}

		public void Remove(AntTaskManager aTaskManager)
		{
			// if (IsLocked)
			// {
			// 	_taskPending.Add(
			// 		new KeyValuePair<AntTaskManager, PendingChange>(
			// 			aTaskManager, 
			// 			PendingChange.Remove
			// 		)
			// 	);
			// 	return;
			// }

			_managers.Remove(aTaskManager);
		}

		#endregion
		#region Private Methods
		
		// private void LockScenario()
		// {
		// 	_lockCount++;
		// }

		// private void UnlockScenario()
		// {
		// 	_lockCount--;
		// 	if (_lockCount <= 0)
		// 	{
		// 		_lockCount = 0;
		// 		ApplyPending();
		// 	}
		// }

		// private void ApplyPending()
		// {
		// 	KeyValuePair<AntTaskManager, PendingChange> pair;
		// 	for (int i = 0, n = _taskPending.Count; i < n; i++)
		// 	{
		// 		pair = _taskPending[i];
		// 		if (pair.Value == PendingChange.Add)
		// 		{
		// 			Add(pair.Key);
		// 		}
		// 		else
		// 		{
		// 			Remove(pair.Key);
		// 		}
		// 	}

		// 	_taskPending.Clear();
		// }
		
		#endregion
	}
}