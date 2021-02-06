namespace Anthill.Fsm
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "NewFiniteStateMachine", menuName = "Anthill/Finite State Machine")]
	public class AntFsmPreset : ScriptableObject
	{
		[HideInInspector]
		public List<StateItem> states = new List<StateItem>();

		[HideInInspector]
		public List<TransitionItem> transitions = new List<TransitionItem>();

		public List<PropItem> properties = new List<PropItem>();

		[Serializable]
		public struct StateItem
		{
			public string name;
			public GameObject prefab;
			public Vector2 position;
			public bool isDefault;
		}

		[Serializable]
		public struct TransitionItem
		{
			public int fromStateIndex;
			public int toStateIndex;
		}

		[Serializable]
		public struct PropItem
		{
			public string name;
			public PropKind kind;
			public bool boolValue;
			public int intValue;
			public float floatValue;
		}

		public enum PropKind
		{
			Bool,
			Int,
			Float,
			Trigger
		}
	}
}