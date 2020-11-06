namespace Anthill.Test
{
	using UnityEngine;
	using Anthill.Fsm;
	
	public class Logic : MonoBehaviour
	{
		public LogicState state;

		private AntFsmAgent _agent;
	
		#region Unity Calls
	
		private void Awake()
		{
			_agent = GetComponent<AntFsmAgent>();
		}
	
		private void Start()
		{
			// ...
		}
	
		private void Update()
		{
			if (_agent.CurrentStateIndex != (int) state)
			{
				_agent.PerformState((int) state);
			}
		}
	
		#endregion
	}

	public enum LogicState
	{
		Idle = 0,
		Walk = 1,
		Shoot = 2
	}
}