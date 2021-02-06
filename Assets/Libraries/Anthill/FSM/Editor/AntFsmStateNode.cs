namespace Anthill.Fsm
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	public class AntFsmStateNode
	{
		public delegate void StateNodeDelegate(AntFsmStateNode aNode);
		public event StateNodeDelegate EventSettedAsDefault;
		public event StateNodeDelegate EventDeleteState;

		public Rect rect;
		public bool isDragged;
		public bool isSelected;
		public bool isAddTransitionMode;

		public List<AntFsmStateNode> links;

		private bool _isDefault;
		private GUIStyle _currentStyle;

		private GUIStyle _normalStyle;
		private GUIStyle _defaultStyle;
		private GUIStyle _normalSelectedStyle;
		private GUIStyle _defaultSelectedStyle;

		private GUIStyle _titleStyle;
		private GUIStyle _bodyStyle;

		private AntFsmPreset _preset;

		#region Getters Setters

		public int StateIndex { get; set; }
		public string Name { get => _preset.states[StateIndex].name; }

		public bool IsDefault
		{
			get => _isDefault;
			set
			{
				_isDefault = value;
				var st = _preset.states[StateIndex];
				st.isDefault = _isDefault;
				_preset.states[StateIndex] = st;
				UpdateVisual();
			}
		}

		public Vector2 Position
		{
			get => new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);
		}

		#endregion
		#region Public Methods

		public AntFsmStateNode(AntFsmPreset aPreset, int aIndex)
		{
			_titleStyle = new GUIStyle();
			_titleStyle.fontStyle = FontStyle.Bold;
			_bodyStyle = CreateStyle("IN BigTitle");

			_preset = aPreset;

			StateIndex = aIndex;
			_isDefault = _preset.states[StateIndex].isDefault;

			rect = new Rect(
				_preset.states[StateIndex].position.x,
				_preset.states[StateIndex].position.y, 200.0f, 95.0f
			);

			links = new List<AntFsmStateNode>();

			_normalStyle = CreateNodeStyle("node0.png", new Color(0.639f, 0.65f, 0.678f));
			_normalSelectedStyle = CreateNodeStyle("node0 on.png", new Color(0.639f, 0.65f, 0.678f));

			_defaultStyle = CreateNodeStyle("node5.png", Color.black);
			_defaultSelectedStyle = CreateNodeStyle("node5 on.png", Color.black);

			UpdateVisual();
		}

		public void AddLink(AntFsmStateNode aNode, bool aChangeAsset = true)
		{
			if (!links.Contains(aNode))
			{
				links.Add(aNode);
				if (aChangeAsset)
				{
					_preset.transitions.Add(
						new AntFsmPreset.TransitionItem
						{
							fromStateIndex = StateIndex,
							toStateIndex = aNode.StateIndex
						}
					);
				}
			}
		}

		public void RemoveLink(AntFsmStateNode aNode)
		{
			for (int i = _preset.transitions.Count - 1; i >= 0; i--)
			{
				if (_preset.transitions[i].fromStateIndex == StateIndex &&
					_preset.transitions[i].toStateIndex == aNode.StateIndex)
				{
					_preset.transitions.RemoveAt(i);
				}
			}
			
			links.Remove(aNode);
		}

		public Vector2 GetOutputPoint(Vector2 aToPosition)
		{
			var pos = Position;
			float ang = AntAngle.BetweenDeg(pos, aToPosition);
			return new Vector2(
				pos.x + 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y + 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

		public Vector2 GetOutputPoint(AntFsmStateNode aToNode)
		{
			var pos = Position;
			float ang = AntAngle.BetweenDeg(pos, aToNode.Position);
			return new Vector2(
				pos.x + 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y + 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

		public Vector2 GetInputPoint(AntFsmStateNode aFromNode)
		{
			var pos = Position;
			float ang = AntAngle.BetweenDeg(pos, aFromNode.Position);
			return new Vector2(
				pos.x - 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y - 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

		public void Drag(Vector2 aDelta)
		{
			rect.position += aDelta;
			var st = _preset.states[StateIndex];
			st.position = new Vector2(rect.x, rect.y);
			_preset.states[StateIndex] = st;
		}

		public void Draw()
		{
			EditorGUI.BeginChangeCheck();
			var state = _preset.states[StateIndex];

			GUI.Box(rect, "", _currentStyle);
			
			// Title
			GUI.Label(
				new Rect(rect.x + 12.0f, rect.y + 12.0f, rect.y + 12.0f, rect.width - 24.0f), 
				$"{StateIndex}. {state.name}", 
				_titleStyle
			);

			var content = new Rect(rect.x + 7, rect.y + 30.0f, rect.width - 14.0f, rect.height - 40.0f);
			content.x = rect.x + 7.0f;
			content.y = rect.y + 30.0f;
			content.width = rect.width - 14.0f;
			content.height = rect.height - 50.0f;
			GUI.Box(content, "", _bodyStyle);

			content.y += 4.0f;
			GUILayout.BeginArea(content);
			{
				EditorGUIUtility.labelWidth = 40.0f;
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(2.0f);
					state.name = EditorGUILayout.TextField("Name", state.name);
					GUILayout.Space(2.0f);
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(2.0f);
					state.prefab = (GameObject) EditorGUILayout.ObjectField("State", state.prefab, typeof(GameObject), false);
					if (state.prefab != null && state.prefab.GetComponent<AntFsmState>() == null)
					{
						A.Warning($"Object `{state.prefab.name}` have no the `AntFsmState` script.");
					}
					GUILayout.Space(2.0f);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			_preset.states[StateIndex] = state;
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_preset);
			}
		}

		public bool ProcessEvents(Event aEvent)
		{
			switch (aEvent.type)
			{
				case EventType.MouseDown :
					if (aEvent.button == 0)
					{
						if (rect.Contains(aEvent.mousePosition))
						{
							isDragged = true;
							isSelected = true;
							GUI.changed = true;
						}
						else
						{
							isSelected = false;
							GUI.changed = true;
						}

						UpdateVisual();
					}

					if (aEvent.button == 1 && isSelected &&
						rect.Contains(aEvent.mousePosition))
					{
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Set As Default"), _isDefault, OnSetAsDefault);
						menu.AddItem(new GUIContent("Add Transition"), false, OnAddTransition);
						menu.AddSeparator("");
						menu.AddItem(new GUIContent("Delete"), false, OnDeleteState);
						menu.ShowAsContext();
						aEvent.Use();
					}
					break;

				case EventType.MouseUp :
					isDragged = false;
					break;

				case EventType.MouseDrag :
					if (aEvent.button == 0 && isDragged)
					{
						Drag(aEvent.delta);
						aEvent.Use();
						return true;
					}
					break;
			}

			return false;
		}

		#endregion
		#region Private Methods

		protected GUIStyle CreateStyle(string aTextureName)
		{
			var style = new GUIStyle(aTextureName);
			style.border = new RectOffset(12, 12, 12, 12);
			style.padding = new RectOffset(12, 0, 10, 0);
			return style;
		}

		private GUIStyle CreateNodeStyle(string aTextureName, Color aTextColor)
		{
			var style = new GUIStyle();
			style.normal.background = (EditorGUIUtility.isProSkin)
				? (Texture2D) EditorGUIUtility.Load($"builtin skins/darkskin/images/{aTextureName}")
				: (Texture2D) EditorGUIUtility.Load($"builtin skins/lightskin/images/{aTextureName}");
			style.border = new RectOffset(12, 12, 12, 12);
			style.richText = true;
			style.padding = new RectOffset(12, 12, 12, 12);
			style.normal.textColor = aTextColor;
			style.alignment = TextAnchor.MiddleCenter;
			return style;
		}

		private void UpdateVisual()
		{
			if (isSelected)
			{
				_currentStyle = (_isDefault)
					? _defaultSelectedStyle
					: _normalSelectedStyle;
			}
			else
			{
				_currentStyle = (_isDefault)
					? _defaultStyle
					: _normalStyle;
			}
		}

		#endregion
		#region Event Handlers

		private void OnSetAsDefault()
		{
			IsDefault = !IsDefault;
			EventSettedAsDefault?.Invoke(this);
		}

		private void OnAddTransition()
		{
			isAddTransitionMode = true;
		}

		private void OnDeleteState()
		{
			EventDeleteState?.Invoke(this);
		}

		#endregion
	}
}