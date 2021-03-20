namespace Anthill.Tasks
{
	using System.Collections.Generic;
	using Anthill.Core;

	public class AntTaskManagerScenario : AntScenario
	{
		private List<AntTaskManager> _managers;

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
			_managers.Add(aTaskManager);
		}

		public void Remove(AntTaskManager aTaskManager)
		{
			_managers.Remove(aTaskManager);
		}

	#endregion

	#region Private Methods
		
		// ..
		
	#endregion
	}
}