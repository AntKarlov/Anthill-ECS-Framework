namespace Anthill.Test
{
	using UnityEngine;
	using Anthill.Fsm;
	
	public class ShootState : AntFsmState
	{
		/// <summary>
		/// Called when object with this state is created,
		/// </summary>
		/// <param name="aGameObject">Game object with AntFsmAgent component.</param>
		public override void Create(GameObject aGameObject)
		{
			A.Log("Shoot state is created!");
		}

		/// <summary>
		/// Called before entering this state.
		/// </summary>
		public override void Enter()
		{
			A.Log("Shoot state is entered!");
		}

		/// <summary>
		/// Calling every time when state is active.
		/// </summary>
		public override void Execute()
		{
			// ..
		}

		/// <summary>
		/// Calling before exiting from this state.
		/// </summary>
		public override void Exit()
		{
			A.Log("Shoot state is exit!");
		}
	}
}