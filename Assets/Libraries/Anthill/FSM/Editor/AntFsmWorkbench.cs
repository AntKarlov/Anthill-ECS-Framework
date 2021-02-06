namespace Anthill.Fsm
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	public class AntFsmWorkbench : EditorWindow
	{
		private struct Connection
		{
			public Vector2 from;
			public Vector2 to;
			public Vector2 center;
			public Vector2 arrowA;
			public Vector2 arrowB;
			public AntFsmStateNode fromNode;
			public AntFsmStateNode toNode;
		}

		private AntFsmPreset[] _presets;
		private List<AntFsmStateNode> _nodes;

		private Vector2 _offset;
		private Vector2 _drag;
		private Vector2 _totalDrag;
		private bool _deleteMode;
		private int _delIndex;

		private Vector2 _mousePosition;
		private List<Connection> _conections;
		private AntFsmPreset _currentPreset;

		private GUIStyle _titleStyle;
		private GUIStyle _labelStyle;
		private GUIStyle _smallLabelStyle;
		
		[MenuItem("Tools/Anthill/FSM Workbench")]
		public static AntFsmWorkbench ShowWindow()
		{
			var window = (AntFsmWorkbench) EditorWindow.GetWindow(typeof(AntFsmWorkbench), false, "FSM Workbench");
			window.autoRepaintOnSceneChange = true;
			return window;
		}

		public static void OpenPreset(string aName)
		{
			ShowWindow().OnSelectPreset(aName);
		}

		public static T[] GetAllInstances<T>() where T : ScriptableObject
		{
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			T[] a = new T[guids.Length];
			for(int i = 0, n = guids.Length; i < n; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}

			return a;
		}

		#region Unity Calls

		private void OnEnable()
		{
			_nodes = new List<AntFsmStateNode>();
			_conections = new List<Connection>();
			_presets = GetAllInstances<AntFsmPreset>();

			_titleStyle = new GUIStyle();
			_titleStyle.fontSize = 22;
			_titleStyle.normal.textColor = Color.gray;

			_labelStyle = new GUIStyle();
			_labelStyle.fontSize = 14;
			_labelStyle.normal.textColor = Color.gray;

			_smallLabelStyle = new GUIStyle();
			_smallLabelStyle.normal.textColor = Color.gray;
		}

		private void OnGUI()
		{
			if (_currentPreset != null)
			{
				Handles.DrawSolidRectangleWithOutline(
					new Rect(0.0f, 0.0f, position.width, position.height), 
					new Color(0.184f, 0.184f, 0.184f), 
					new Color(0.184f, 0.184f, 0.184f)
				);

				DrawGrid(20, Color.gray, 0.05f);
				DrawGrid(100, Color.gray, 0.05f);

				ProcessEvents(Event.current);

				DrawLinks();
				DrawNodes();

				Repaint();

				GUI.Label(
					new Rect(20.0f, 30.0f, 200.0f, 50.0f),
					"Hold `Ctrl` and click on Transition to delete.",
					_smallLabelStyle
				);
			}
			else
			{
				if (Event.current.type == EventType.Repaint)
				{
					// GUI.Label(
					// 	new Rect(10.0f, 10.0f, 200.0f, 50.0f), 
					// 	"Finite State Machine Preset is not selected.",
					// 	_titleStyle
					// );

					GUI.Label(
						new Rect(20.0f, 30.0f, 200.0f, 50.0f),
						"Create and edit preset:",
						_labelStyle
					);

					GUI.Label(
						new Rect(30.0f, 60.0f, 200.0f, 50.0f),
						"1. Open context menu in the Project window.",
						_smallLabelStyle
					);

					GUI.Label(
						new Rect(30.0f, 80.0f, 200.0f, 50.0f),
						"2. Select `Create > Anthill > Finite State Machine`.",
						_smallLabelStyle
					);

					GUI.Label(
						new Rect(30.0f, 100.0f, 200.0f, 50.0f),
						"3. Enter the name and select new preset.",
						_smallLabelStyle
					);

					GUI.Label(
						new Rect(30.0f, 120.0f, 200.0f, 50.0f),
						"4. Open context menu and select `Add State`.",
						_smallLabelStyle
					);

					GUI.Label(
						new Rect(30.0f, 140.0f, 200.0f, 50.0f),
						"5. Remove State, Set As Default and Add Transitions between states also via context menu.",
						_smallLabelStyle
					);

					GUI.Label(
						new Rect(30.0f, 160.0f, 200.0f, 50.0f),
						"6. For remove Transitions between states, press Ctrl and click on transition.",
						_smallLabelStyle
					);
				}
			}

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical(GUILayout.MinWidth(200.0f), GUILayout.MaxWidth(200.0f));
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					if (GUILayout.Button("Hide Props", EditorStyles.toolbarButton))
					{

					}

					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add", EditorStyles.toolbarDropDown))
					{
						var menu = new GenericMenu();
						var props = AntEnum.GetStringValues<AntFsmPreset.PropKind>();
						for (int i = 0, n = props.Length; i < n; i++)
						{
							menu.AddItem(new GUIContent(props[i]), false, OnAddProperty, props[i]);
						}
						menu.DropDown(new Rect(0.0f, 12.0f, 0.0f, 0.0f));
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				{
					EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
					EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
					EditorGUILayout.EndHorizontal();

					if (_currentPreset != null)
					{
						int delIndex = -1;
						for (int i = 0, n = _currentPreset.properties.Count; i < n; i++)
						{
							var prop = _currentPreset.properties[i];
							EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
							{
								prop.name = EditorGUILayout.TextField(prop.name);
								EditorGUILayout.LabelField(prop.kind.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(50.0f));
								if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
								{
									delIndex = i;
								}
							}
							EditorGUILayout.EndHorizontal();
							_currentPreset.properties[i] = prop;
						}

						if (delIndex > -1)
						{
							_currentPreset.properties.RemoveAt(delIndex);
						}
					}
				}
				GUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();
		
				GUILayout.BeginVertical();
				{
					EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
					{
						string currentName = (_currentPreset != null) ? _currentPreset.name : "Open";
						if (GUILayout.Button(currentName, EditorStyles.toolbarDropDown))
						{
							_presets = GetAllInstances<AntFsmPreset>();
							var menu = new GenericMenu();
							if (_presets != null && _presets.Length > 0)
							{
								for (int i = 0, n = _presets.Length; i < n; i++)
								{
									bool isSelected = (_currentPreset != null && _currentPreset.name.Equals(_presets[i].name));
									menu.AddItem(new GUIContent(_presets[i].name), isSelected, OnSelectPreset, _presets[i].name);
								}
							}
							else
							{
								menu.AddDisabledItem(new GUIContent("<No FSM presets>"));
							}

							// menu.AddSeparator("");
							// menu.AddItem(new GUIContent("Update FSM presets list"), false, OnFindAllPresets);
							// todo: сдвигать позицию меню в зависимости от того отображаются ли проперти
							menu.DropDown(new Rect(0.0f, 12.0f, 0.0f, 0.0f));
						}

						if (_currentPreset != null)
						{
							if (GUILayout.Button("Export States to Enum", EditorStyles.toolbarButton))
							{
								var str = "public enum FsmStates\n{\n";
								for (int i = 0, n = _nodes.Count; i < n; i++)
								{
									str = string.Concat(str, $"{_nodes[i].Name} = {_nodes[i].StateIndex}");
									str = (i + 1 == _nodes.Count)
										? string.Concat(str, "\n}")
										: string.Concat(str, ",\n");
								}

								var te = new TextEditor();
								te.text = str;
								te.SelectAll();
								te.Copy();
							}
						}

						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		public void OnAddProperty(object aPropertyKind)
		{
			_currentPreset.properties.Add(new AntFsmPreset.PropItem
			{
				name = "<Unnamed>",
				kind = AntEnum.Parse<AntFsmPreset.PropKind>(aPropertyKind.ToString(), AntFsmPreset.PropKind.Bool),
				boolValue = false,
				intValue = 0,
				floatValue = 0.0f
			});
		}

		public void OnSelectPreset(object aPresetName)
		{
			AntFsmPreset selectedPreset = System.Array.Find(_presets, x => x.name.Equals(aPresetName.ToString()));
			if (selectedPreset != null)
			{
				ClearNodes();
				_currentPreset = null;
			}

			_currentPreset = selectedPreset;
			ClearNodes();

			AntFsmStateNode node;
			for (int i = 0, n = _currentPreset.states.Count; i < n; i++)
			{
				node = new AntFsmStateNode(_currentPreset, i);
				node.EventSettedAsDefault += OnSetNodeAsDefault;
				node.EventDeleteState += OnDeleteState;
				_nodes.Add(node);
			}

			AntFsmStateNode fromNode;
			AntFsmStateNode toNode;
			for (int i = 0, n = _currentPreset.transitions.Count; i < n; i++)
			{
				fromNode = GetNodeByIndex(_currentPreset.transitions[i].fromStateIndex);
				toNode = GetNodeByIndex(_currentPreset.transitions[i].toStateIndex);
				fromNode.AddLink(toNode, false);
			}

			/*
			if (Selection.objects.Length == 1 && Selection.objects[0] is AntFsmPreset)
			{
				var t = (AntFsmPreset) Selection.objects[0];
				if (t == null)
				{
					ClearNodes();
					_currentPreset = null;
				}
				else if (!System.Object.ReferenceEquals(t, _currentPreset))
				{
					_currentPreset = t;
					ClearNodes();

					AntFsmStateNode node;
					for (int i = 0, n = _currentPreset.states.Count; i < n; i++)
					{
						node = new AntFsmStateNode(_currentPreset, i);
						node.EventSettedAsDefault += OnSetNodeAsDefault;
						node.EventDeleteState += OnDeleteState;
						_nodes.Add(node);
					}

					AntFsmStateNode fromNode;
					AntFsmStateNode toNode;
					for (int i = 0, n = _currentPreset.transitions.Count; i < n; i++)
					{
						fromNode = GetNodeByIndex(_currentPreset.transitions[i].fromStateIndex);
						toNode = GetNodeByIndex(_currentPreset.transitions[i].toStateIndex);
						fromNode.AddLink(toNode, false);
					}
				}
			}
			else
			{
				ClearNodes();
				_currentPreset = null;
			}
			//*/
		}

		// private void OnFindAllPresets()
		// {
		// 	_presets = GetAllInstances<AntFsmPreset>();
		// }

		private void OnAddState()
		{
			_currentPreset.states.Add(
				new AntFsmPreset.StateItem
				{
					name = "<Unnamed>",
					position = _mousePosition,
					isDefault = (_currentPreset.states.Count == 0)
				}
			);

			var node = new AntFsmStateNode(_currentPreset, _currentPreset.states.Count - 1);
			node.EventSettedAsDefault += OnSetNodeAsDefault;
			node.EventDeleteState += OnDeleteState;
			_nodes.Add(node);
		}

		private void OnSetNodeAsDefault(AntFsmStateNode aNode)
		{
			AntFsmStateNode node;
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				node = _nodes[i];
				if (node.IsDefault && node.StateIndex != aNode.StateIndex)
				{
					node.IsDefault = false;
				}
			}
		}

		private void OnDeleteState(AntFsmStateNode aNode)
		{
			int delIndex = -1;
			AntFsmStateNode node;

			// 1. Remove links from other nodes on deleting node.
			// ------------------------------------------------
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				node = _nodes[i];
				for (int j = node.links.Count - 1; j >= 0; j--)
				{
					if (node.links[j].StateIndex == aNode.StateIndex)
					{
						node.RemoveLink(aNode);
					}
				}

				if (node.StateIndex == aNode.StateIndex)
				{
					delIndex = i;
				}
			}

			// 2. Remove all links from deleting node to other nodes.
			// ------------------------------------------------------
			for (int i = aNode.links.Count - 1; i >= 0; i--)
			{
				aNode.RemoveLink(aNode.links[i]);
			}

			// 3. Remove node.
			// ---------------
			_nodes.RemoveAt(delIndex);

			// 4. Remove state from preset.
			// ----------------------------
			int removeStateIndex = aNode.StateIndex;
			_currentPreset.states.RemoveAt(removeStateIndex);
			
			// Update links in preset.
			AntFsmPreset.TransitionItem transitionItem;
			for (int i = _currentPreset.transitions.Count - 1; i >= 0; i--)
			{
				transitionItem = _currentPreset.transitions[i];
				if (transitionItem.fromStateIndex > removeStateIndex)
				{
					transitionItem.fromStateIndex--;
				}

				if (transitionItem.toStateIndex > removeStateIndex)
				{
					transitionItem.toStateIndex--;
				}

				_currentPreset.transitions[i] = transitionItem;
			}

			// 5. Update node indexes.
			// -----------------------
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				if (_nodes[i].StateIndex > removeStateIndex)
				{
					_nodes[i].StateIndex--;
				}
			}
		}

		#endregion
		#region Private Methods

		private void ClearNodes()
		{
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				_nodes[i].EventSettedAsDefault -= OnSetNodeAsDefault;
			}
			_nodes.Clear();
		}

		private void ProcessEvents(Event aEvent)
		{
			_drag = Vector2.zero;

			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				_nodes[i].ProcessEvents(aEvent);
			}

			_deleteMode = (aEvent.control);

			switch (aEvent.type)
			{
				case EventType.MouseDown :
					if (aEvent.button == 1)
					{
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Add State"), false, OnAddState);
						_mousePosition = aEvent.mousePosition;
						menu.ShowAsContext();
					}

					if (aEvent.button == 0)
					{
						if (_deleteMode)
						{
							// Delete connection
							if (_delIndex >= 0 && _delIndex < _conections.Count)
							{
								_conections[_delIndex].fromNode.RemoveLink(_conections[_delIndex].toNode);
								EditorUtility.SetDirty(_currentPreset);
							}
							_deleteMode = false;
						}
						else
						{
							// Create connection
							var fromNode = GetNodeWithTransition();
							if (fromNode != null)
							{
								fromNode.isAddTransitionMode = false;
								var toNode = GetNodeByMousePosition(aEvent.mousePosition);
								if (toNode != null)
								{
									fromNode.AddLink(toNode);
									EditorUtility.SetDirty(_currentPreset);
								}
							}
						}
					}
					break;

				case EventType.MouseDrag :
					if (aEvent.button == 2)
					{
						_totalDrag += aEvent.delta;
						_drag = aEvent.delta;

						for (int i = 0, n = _nodes.Count; i < n; i++)
						{
							_nodes[i].Drag(_drag);
						}

						GUI.changed = true;
						EditorUtility.SetDirty(_currentPreset);
					}
					break;
			}
		}

		private AntFsmStateNode GetNodeByMousePosition(Vector2 aMousePosition)
		{
			int index = _nodes.FindIndex(x => x.rect.Contains(aMousePosition));
			return (index >= 0 && index < _nodes.Count) ? _nodes[index]	: null;
		}

		private AntFsmStateNode GetNodeWithTransition()
		{
			int index = _nodes.FindIndex(x => x.isAddTransitionMode);
			return (index >= 0 && index < _nodes.Count) ? _nodes[index] : null;
		}

		private AntFsmStateNode GetNodeByName(string aName)
		{
			int index = _nodes.FindIndex(x => x.Name.Equals(aName));
			return (index >= 0 && index < _nodes.Count) ? _nodes[index] : null;
		}

		private AntFsmStateNode GetNodeByIndex(int aStateIndex)
		{
			int index = _nodes.FindIndex(x => x.StateIndex == aStateIndex);
			return (index >= 0 && index < _nodes.Count) ? _nodes[index] : null;
		}

		private void DrawNodes()
		{
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				_nodes[i].Draw();
			}
		}

		private void DrawLinks()
		{
			AntFsmStateNode node;
			AntFsmStateNode toNode;
			Vector2 outPosition;
			Vector2 inPosition;

			_conections.Clear();
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				node = _nodes[i];
				for (int j = 0, nj = node.links.Count; j < nj; j++)
				{
					toNode = node.links[j];
					outPosition = node.GetOutputPoint(toNode);
					inPosition = toNode.GetInputPoint(node);
					// AntDrawer.DrawSolidLine(outPosition, inPosition, Color.white);

					float dist = AntMath.Distance(outPosition, inPosition);
					float ang = AntAngle.BetweenRad(outPosition, inPosition);
					var pos = new Vector2(
						outPosition.x + dist * 0.5f * Mathf.Cos(ang),
						outPosition.y + dist * 0.5f * Mathf.Sin(ang));
					var posA = new Vector2(
						pos.x - 10f * Mathf.Cos(ang + 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang + 35.0f * Mathf.Deg2Rad));
					var posB = new Vector2(
						pos.x - 10f * Mathf.Cos(ang - 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang - 35.0f * Mathf.Deg2Rad));

					// AntDrawer.DrawSolidLine(pos, posA, Color.white);
					// AntDrawer.DrawSolidLine(pos, posB, Color.white);

					_conections.Add(
						new Connection 
						{
							from = outPosition,
							to = inPosition,
							center = pos,
							arrowA = posA,
							arrowB = posB,
							fromNode = node,
							toNode = toNode
						}
					);
				}

				if (node.isAddTransitionMode)
				{
					outPosition = node.GetOutputPoint(Event.current.mousePosition);
					inPosition = Event.current.mousePosition;
					AntDrawer.DrawSolidLine(outPosition, inPosition, Color.white);

					float dist = AntMath.Distance(outPosition, inPosition);
					float ang = AntAngle.BetweenRad(outPosition, inPosition);
					var pos = new Vector2(
						outPosition.x + dist * 0.5f * Mathf.Cos(ang),
						outPosition.y + dist * 0.5f * Mathf.Sin(ang)
					);
					var posA = new Vector2(
						pos.x - 10f * Mathf.Cos(ang + 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang + 35.0f * Mathf.Deg2Rad)
					);
					var posB = new Vector2(
						pos.x - 10f * Mathf.Cos(ang - 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang - 35.0f * Mathf.Deg2Rad)
					);

					AntDrawer.DrawSolidLine(pos, posA, Color.white);
					AntDrawer.DrawSolidLine(pos, posB, Color.white);
				}
			}

			if (_deleteMode)
			{
				float minDistance = float.MaxValue;
				for (int i = 0, n = _conections.Count; i < n; i++)
				{
					float dist = AntMath.Distance(_conections[i].center, Event.current.mousePosition);
					if (dist < minDistance)
					{
						_delIndex = i;
						minDistance = dist;
					}
				}
			}
			else
			{
				_delIndex = -1;
			}

			Connection con;
			Color c;
			for (int i = 0, n = _conections.Count; i < n; i++)
			{
				con = _conections[i];
				c = (i == _delIndex)
					? Color.red
					: (con.fromNode.isSelected)
						? Color.white
						: Color.gray;
				AntDrawer.DrawSolidLine(con.from, con.to, c);
				AntDrawer.DrawSolidLine(con.center, con.arrowA, c);
				AntDrawer.DrawSolidLine(con.center, con.arrowB, c);
			}
		}

		private void DrawGrid(float aCellSize, Color aColor, float aOpacity)
		{
			int cols = Mathf.CeilToInt(position.width / aCellSize);
			int rows = Mathf.CeilToInt(position.height / aCellSize);

			Handles.BeginGUI();
			Color c = Handles.color;
			Handles.color = new Color(aColor.r, aColor.g, aColor.b, aOpacity);

			_offset += _drag * 0.5f;
			var newOffset = new Vector3(_offset.x % aCellSize, _offset.y % aCellSize, 0.0f);

			for (int i = 0; i < cols; i++)
			{
				Handles.DrawLine(
					new Vector3(aCellSize * i, -aCellSize, 0.0f) + newOffset,
					new Vector3(aCellSize * i, position.height, 0.0f) + newOffset
				);
			}

			for (int i = 0; i < rows; i++)
			{
				Handles.DrawLine(
					new Vector3(-aCellSize, aCellSize * i, 0.0f) + newOffset,
					new Vector3(position.width, aCellSize * i, 0.0f) + newOffset
				);
			}

			Handles.color = c;
			Handles.EndGUI();
		}

		#endregion
	}
}