namespace Anthill.Fsm
{
	using UnityEngine;
	using System.Collections.Generic;

	public class AntFsmAgent : MonoBehaviour
	{
		private struct Pair
		{
			public int from;
			public int to;
		}

		public AntFsmPreset finiteStateMachinePreset;
		public bool debugLog;

		private int _currentStateIndex;
		private int _defaultStateIndex;
		private List<AntFsmState> _states;

		private AntFsmState _currentState;
		private List<Pair> _transitions;

		#region Getters / Setters

		public int DefaultStateIndex { get => _defaultStateIndex; }
		public int CurrentStateIndex { get => _currentStateIndex; }

		public AntFsmState CurrentState
		{
			get => _currentState;
		}

		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_currentStateIndex = -1;

			if (finiteStateMachinePreset == null)
			{
				A.Warning($"AntFsmAgent `{gameObject.name}` have no FSM preset.");
				return;
			}

			// 1. Find Default State.
			// ----------------------
			_defaultStateIndex = finiteStateMachinePreset.states.FindIndex(x => x.isDefault);
			_currentStateIndex = (_defaultStateIndex >= 0 && _defaultStateIndex < finiteStateMachinePreset.states.Count)
				? _defaultStateIndex
				: -1;

			// 2. Initialize States.
			// ------------------
			_states = new List<AntFsmState>();
			for (int i = 0, n = finiteStateMachinePreset.states.Count; i < n; i++)
			{
				A.Assert(
					finiteStateMachinePreset.states[i].prefab == null, 
					$"AntFsmAgent `{finiteStateMachinePreset.name}` have no state prefab for `{finiteStateMachinePreset.states[i].name}` state!"
				);

				var go = GameObject.Instantiate(finiteStateMachinePreset.states[i].prefab);
				go.transform.SetParent(gameObject.transform);
				go.name = finiteStateMachinePreset.states[i].name;
				
				var state = go.GetComponent<AntFsmState>();
				
				A.Assert(
					state == null, 
					$"GameObject `{go.name}` have no `AntFsmState` component!"
				);

				state.Create(gameObject);
				state.gameObject.SetActive(false);
				AddState(state, finiteStateMachinePreset.states[i].isDefault);
			}

			// 3. Initialize transitions.
			// --------------------------
			_transitions = new List<Pair>();
			for (int i = 0, n = finiteStateMachinePreset.transitions.Count; i < n; i++)
			{
				AddTransition(
					finiteStateMachinePreset.transitions[i].fromStateIndex, 
					finiteStateMachinePreset.transitions[i].toStateIndex
				);
			}
		}

		private void Update()
		{
			if (_currentStateIndex > -1)
			{
				_currentState.Execute();
			}
		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Removes all states and all transitions.
		/// </summary>
		public void Clear()
		{
			_currentStateIndex = -1;
			_currentState = null;

			for (int i = 0, n = _states.Count; i < n; i++)
			{
				GameObject.Destroy(_states[i].gameObject);
			}

			_states.Clear();
			_transitions.Clear();
		}

		/// <summary>
		/// Adds new state to the state list.
		/// </summary>
		/// <param name="aState">State component.</param>
		/// <param name="aIsDefault">If true then this state immediatly will be activated.</param>
		public void AddState(AntFsmState aState, bool aIsDefault)
		{
			if (!Contains(aState))
			{
				_states.Add(aState);
				if (aIsDefault)
				{
					_currentStateIndex = _states.Count - 1;
					_currentState = aState;
					_currentState.gameObject.SetActive(true);
					_currentState.Enter();
				}
			}
			else if (debugLog)
			{
				A.Warning($"Can't to add state `{aState.name}` because it is already exists!");
			}
		}

		/// <summary>
		/// Check state in the state list.
		/// </summary>
		/// <param name="aState">State component.</param>
		/// <returns>True if state already added.</returns>
		public bool Contains(AntFsmState aState)
		{
			return _states.Contains(aState);
		}

		/// <summary>
		/// Removes state from the state list.
		/// </summary>
		/// <param name="aState">State component.</param>
		public void RemoveState(AntFsmState aState)
		{
			int stateIndex = _states.IndexOf(aState);
			if (stateIndex >= 0 && stateIndex < _states.Count)
			{
				GameObject.Destroy(_states[stateIndex].gameObject);
				_states.RemoveAt(stateIndex);
				
				Pair p;
				for (int i = _transitions.Count - 1; i >= 0; i--)
				{
					p = _transitions[i];
					if (p.from == stateIndex || p.to == stateIndex)
					{
						_transitions.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// Adds transition between states.
		/// </summary>
		/// <param name="aFromStateIndex">From State Index.</param>
		/// <param name="aToStateIndex">To State Index.</param>
		public void AddTransition(int aFromStateIndex, int aToStateIndex)
		{
			if (debugLog && ContainsTransition(aFromStateIndex, aToStateIndex))
			{
				A.Warning($"Transition from `{_states[aFromStateIndex].name}` to `{_states[aToStateIndex].name} already exists!");
			}

			_transitions.Add(
				new Pair 
				{
					from = aFromStateIndex,
					to = aToStateIndex
				}
			);
		}

		/// <summary>
		/// Checks if transition between states exists.
		/// </summary>
		/// <param name="aFromStateIndex">From State Index.</param>
		/// <param name="aToStateIndex">To State Index.</param>
		/// <returns>Returns true if transition exists.</returns>
		public bool ContainsTransition(int aFromStateIndex, int aToStateIndex)
		{
			int index = _transitions.FindIndex(x => x.from == aFromStateIndex && x.to == aToStateIndex);
			return (index >= 0 && index < _transitions.Count);
		}

		/// <summary>
		/// Removes transition between states.
		/// </summary>
		/// <param name="aFromStateIndex">From State Index.</param>
		/// <param name="aToStateIndex">To State Index.</param>
		public void RemoveTransition(int aFromStateIndex, int aToStateIndex)
		{
			int index = _transitions.FindIndex(x => x.from == aFromStateIndex && x.to == aToStateIndex);
			if (index >= 0 && index < _transitions.Count)
			{
				_transitions.RemoveAt(index);
			}
		}

		/// <summary>
		/// Switches current state to specified state if possible.
		/// </summary>
		/// <param name="aToStateIndex">Index of the next state.</param>
		/// <param name="aForce">If true then state will be switched anyway, if no possible transitions.</param>
		/// <returns>Returns true if state is switched.</returns>
		public bool PerformState(int aToStateIndex, bool aForce = false)
		{
			int transitionIndex = _transitions.FindIndex(x => x.from == _currentStateIndex && x.to == aToStateIndex);
			if ((transitionIndex >= 0 && transitionIndex < _transitions.Count) || aForce)
			{
				_currentState.Exit();
				_currentState.gameObject.SetActive(false);

				_currentState = _states[aToStateIndex];
				_currentStateIndex = aToStateIndex;
				
				_currentState.gameObject.SetActive(true);
				_currentState.Enter();

				return true;
			}
			else if (debugLog)
			{
				A.Warning($"Can't find transition from `{_states[_currentStateIndex].name}` to `{_states[aToStateIndex].name}` state!");
			}

			return false;
		}

		#endregion
	}
}