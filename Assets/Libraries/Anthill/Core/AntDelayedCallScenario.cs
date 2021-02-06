namespace Anthill.Core
{
	using System.Collections.Generic;
	using UnityEngine;

	public class AntDelayedCallScenario : AntScenario
	{
		private List<DelayedCall> _delayedCalls;

		public AntDelayedCallScenario() : base("Delayed Call Scenario")
		{
			// ..
		}

		public override void AddedToEngine()
		{
			base.AddedToEngine();
			_delayedCalls = new List<DelayedCall>();
		}

		public override void RemovedFromEngine()
		{
			base.RemovedFromEngine();
			_delayedCalls.Clear();
			_delayedCalls = null;
		}

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

		public void Add(DelayedCall aDelayedCall)
		{
			_delayedCalls.Add(aDelayedCall);
		}
	}
}