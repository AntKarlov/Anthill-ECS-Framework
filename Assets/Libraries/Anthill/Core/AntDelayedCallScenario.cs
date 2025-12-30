using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public class AntDelayedCallScenario : AntScenario
	{
		// -----------------------------------------------------
		// Private Variables
		// -----------------------------------------------------

		private int _lockCount;
		private List<DelayedCall> _delayedCalls;
		private List<KeyValuePair<DelayedCall, PendingChange>> _callPending;

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public AntDelayedCallScenario() : base("Delayed Call Scenario")
		{
			// ..
		}

		public override void AddedToEngine()
		{
			base.AddedToEngine();
			_delayedCalls = new List<DelayedCall>();
			_callPending = new List<KeyValuePair<DelayedCall, PendingChange>>();
		}

		public override void RemovedFromEngine()
		{
			base.RemovedFromEngine();
			_callPending.Clear();
			_callPending = null;

			_delayedCalls.Clear();
			_delayedCalls = null;
		}

		public override void Execute()
		{
			base.Execute();
			Lock();

			float dt = Time.deltaTime;
			float undt = Time.unscaledDeltaTime;
			DelayedCall call;
			for (int i = _delayedCalls.Count - 1; i >= 0; i--)
			{
				call = _delayedCalls[i];
				if (call.IsUnscaledTime)
				{
					if (_delayedCalls[i].Update(undt))
					{
						_delayedCalls.RemoveAt(i);
					}
				}
				else if (_delayedCalls[i].Update(dt))
				{
					_delayedCalls.RemoveAt(i);
				}
			}

			Unlock();
		}

		public override void Deinitialize()
		{
			base.Deinitialize();
			for (int i = _delayedCalls.Count - 1; i >= 0; i--)
			{
				if (_delayedCalls[i].IsKillOnDeinitialize)
				{
					_delayedCalls[i].Kill();
				}
			}
		}

		public void KillAll()
		{
			for (int i = _delayedCalls.Count - 1; i >= 0; i--)
			{
				_delayedCalls[i].Kill();
			}
		}

		/// <summary>
		/// Adds new DelayedCall into list.
		/// </summary>
		/// <param name="delayedCall">Delayed call.</param>
		public void Add(DelayedCall delayedCall)
		{
			if (_lockCount > 0)
			{
				_callPending.Add(
					new KeyValuePair<DelayedCall, PendingChange>(
						delayedCall,
						PendingChange.Add
					)
				);
			}
			else
			{
				_delayedCalls.Add(delayedCall);
			}
		}

		/// <summary>
		/// Removes DelaydCall from the list.
		/// </summary>
		/// <param name="aDelayedCall"></param>
		public void Remove(DelayedCall delayedCall)
		{
			if (_lockCount > 0)
			{
				_callPending.Add(
					new KeyValuePair<DelayedCall, PendingChange>(
						delayedCall,
						PendingChange.Remove
					)
				);
			}
			else
			{
				if (_delayedCalls.Contains(delayedCall))
				{
					_delayedCalls.Remove(delayedCall);
				}
			}
		}

		/// <summary>
		/// Clears all exisitng DelayedCalls.
		/// </summary>
		public void Clear()
		{
			_delayedCalls.Clear();
		}

		// -----------------------------------------------------
		// Private Methods
		// -----------------------------------------------------

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
			KeyValuePair<DelayedCall, PendingChange> pair;
			for (int i = 0, n = _callPending.Count; i < n; i++)
			{
				pair = _callPending[i];
				if (pair.Value == PendingChange.Add)
				{
					Add(pair.Key);
				}
				else
				{
					Remove(pair.Key);
				}
			}

			_callPending.Clear();
		}
	}
}