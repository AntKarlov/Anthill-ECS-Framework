using System.Collections.Generic;
using Anthill.Core;
using UnityEngine;

namespace Anthill.Tweening
{
	public class AntTweeningScenario : AntScenario
	{
	#region Private Variables

		private List<ITween> _tweens;

	#endregion

	#region Public Methods

		public AntTweeningScenario() : base("Tweening Scenario")
		{
			// ..
		}

		public override void AddedToEngine()
		{
			base.AddedToEngine();
			_tweens = new List<ITween>();
		}

		public override void RemovedFromEngine()
		{
			base.RemovedFromEngine();
			_tweens.Clear();
			_tweens = null;
		}

		public override void Execute()
		{
			base.Execute();
			float dt = Time.deltaTime;
			float udt = Time.unscaledDeltaTime;

			ITween tween;
			for (int i = _tweens.Count - 1; i >= 0; i--)
			{
				tween = _tweens[i];
				tween.Execute(tween.IsUnscaledTime ? udt : dt);
			}
		}

		public override void Deinitialize()
		{
			base.Deinitialize();
			for (int i = _tweens.Count - 1; i >= 0; i--)
			{
				if (_tweens[i].IsKillOnDeinitialize)
				{
					_tweens[i].Kill();
				}
			}
		}

		public void KillAll()
		{
			for (int i = _tweens.Count - 1; i >= 0; i--)
			{
				_tweens[i].Kill();
			}
		}

		public void Add(ITween aTween)
		{
			_tweens.Add(aTween);
			// ConsoleProDebug.Watch("Active Tweens:", $"{_tweens.Count}");
		}

		public void Remove(ITween aTween)
		{
			_tweens.Remove(aTween);
			// ConsoleProDebug.Watch("Active Tweens:", $"{_tweens.Count}");
		}

	#endregion

	#region Private Methods
		
		// ..
		
	#endregion
	}
}