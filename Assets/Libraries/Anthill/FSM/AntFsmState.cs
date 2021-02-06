namespace Anthill.Fsm
{
	using UnityEngine;
	
	public abstract class AntFsmState : MonoBehaviour
	{
		/// <summary>
		/// Called when object with this state is created,
		/// </summary>
		/// <param name="aGameObject">Game object with AntFsmAgent component.</param>
		public virtual void Create(GameObject aGameObject)
		{
			// ..
		}

		/// <summary>
		/// Called before entering this state.
		/// </summary>
		public virtual void Enter()
		{
			// ..
		}

		/// <summary>
		/// Calling every time when state is active.
		/// </summary>
		public virtual void Execute()
		{
			// ..
		}

		/// <summary>
		/// Calling before exiting from this state.
		/// </summary>
		public virtual void Exit()
		{
			// ..
		}
	}
}