namespace Anthill.Scenes
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.SceneManagement;
	using UnityEditor.SceneManagement;
	using Anthill.Utils;

	// ■█

	public class ScenesManagerEditor : EditorWindow
	{
		private enum ScenesPanelState
		{
			Default,
			Info
		}

		private const float _btnHeight = 20.0f;
		private const int _btnPadding = 4;

		private int _startIndex;
		private int _dragIndex;
		private int _lastDragIndex;
		private bool _isMouseDown;
		private int[] _indexList;
		private string _dragSceneName;

		private Vector2 _scrollPos;
		private ScenesPanelState _state = ScenesPanelState.Default;
		private bool _enableDelete;
		private readonly StringBuilder _strb = new StringBuilder();

		[MenuItem("Tools/Anthill/Scenes Manager")]
		private static void ShowWindow()
		{
			var window = GetWindow(typeof(ScenesManagerEditor), false, "Scenes Manager");
			window.autoRepaintOnSceneChange = true;
		}

		#region Unity Calls

		private void OnHierarchyChange()
		{
			Repaint();
		}

		private void OnGUI()
		{
			if (Event.current.type == EventType.ValidateCommand && 
				Event.current.commandName == "UndoRedoPerformed")
			{
				Repaint();
			}

			_scrollPos = GUILayout.BeginScrollView(_scrollPos);

			if (!Application.isPlaying && _state == ScenesPanelState.Info)
			{
				DrawHelpState();
			}
			else
			{
				DrawDefaultState();
			}

			GUILayout.EndScrollView();
		}

		#endregion

		#region Private Methods

		private void DrawDefaultState()
		{
			var c = GUI.color;

			if (!Application.isPlaying)
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					_enableDelete = GUILayout.Toggle(_enableDelete, "Enable Remove", EditorStyles.toolbarButton);
					if (GUILayout.Button("Info", EditorStyles.toolbarButton, GUILayout.Width(46)))
					{
						_state = ScenesPanelState.Info;
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			int len = EditorBuildSettings.scenes.Length;

			// Get and show total enabled + disabled scenes.
			// —————————————————————————————————————————————
			int totEnabled = 0;
			int totDisabled = 0;
			for (var i = 0; i < len; i++)
			{
				if (EditorBuildSettings.scenes[i].enabled)
				{
					totEnabled++;
				}
				else
				{
					totDisabled++;
				}
			}

			_strb.Length = 0;
			_strb.Append("Scenes In Build: ").Append(totEnabled).Append("/").Append(totEnabled + totDisabled);
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField(_strb.ToString(), EditorStyles.miniBoldLabel);
			EditorGUILayout.EndHorizontal();

			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
			EditorBuildSettingsScene[] scenesMod = null;

			// Droping of the dragging item.
			// —————————————————————————————
			if (_isMouseDown && Event.current.type == EventType.MouseUp)
			{
				_isMouseDown = false;
				int toIndex = Mathf.RoundToInt(Mathf.Abs(_dragIndex - _startIndex));
				if (toIndex > 0)
				{
					int dir = (_dragIndex - _startIndex) / Mathf.Abs(_dragIndex - _startIndex);
					for (int i = 0; i < toIndex; i++)
					{
						if (dir > 0)
						{
							AntArray.Swap(ref scenes, _startIndex + i + 1, _startIndex + i);
							GUI.changed = true;
						}
						else if (dir < 0)
						{
							AntArray.Swap(ref scenes, _startIndex - i - 1, _startIndex - i);
							GUI.changed = true;
						}
					}
				}
			}

			// Draw list of scenes.
			// ————————————————————
			int sceneIndex = -1;
			for (int i = 0; i < len; i++)
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				EditorBuildSettingsScene scene = (_isMouseDown) ? scenes[_indexList[i]] : scenes[i];
				if (Application.isPlaying && !scene.enabled)
				{
					continue;
				}

				if (scene.enabled)
				{
					sceneIndex++;
				}

				string sceneName = Path.GetFileNameWithoutExtension(scene.path);
				bool isCurrent = (SceneManager.sceneCount > 1) 
					? IsAdditiveSceneLoaded(scene.path) 
					: SceneManager.GetActiveScene().path == scene.path;
				
				Color bgShade = (isCurrent) ? Color.green : Color.white;
				Color labelColor = (isCurrent) ? Color.yellow : Color.black;

				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				
				if (_enableDelete)
				{
					GUI.color = Color.red;
					if (GUILayout.Button("", "OL Minus", GUILayout.Width(16.0f)))
					{
						// Remove scene from build.
						scenesMod = CloneAndRemove(scenes, i);
						GUI.changed = true;
					}
					GUI.color = c;
					GUILayout.Space(2.0f);
				}

				if (Application.isPlaying)
				{
					GUILayout.Button(sceneIndex.ToString(), EditorStyles.toolbarButton, GUILayout.Width(20.0f));
				}
				else
				{
					GUI.color = (scene.enabled) ? Color.green : Color.white;
					var caption = (scene.enabled) ? sceneIndex.ToString() : "-";
					if (_isMouseDown && i == _dragIndex)
					{
						GUI.color = Color.gray;
						caption = "";
					}

					if (GUILayout.Button(caption, EditorStyles.toolbarButton, GUILayout.Width(20.0f)))
					{
						scene.enabled = !scene.enabled;
					}
					GUI.color = c;
				}

				EditorGUI.EndDisabledGroup();
				
				GUI.color = bgShade;
				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				
				if (_isMouseDown && i == _dragIndex)
				{
					GUI.color = Color.gray;
					sceneName = "";
				}

				if (GUILayout.Button(sceneName, style))
				{
					if (Application.isPlaying)
					{
						SceneManager.LoadScene(sceneIndex);
					}
					else
					{
						if (Event.current.button == 1)
						{
							// Right-click: ping scene in Project panel and store its name in the clipboard
							Object sceneObj = AssetDatabase.LoadAssetAtPath<Object>(scene.path);
							EditorGUIUtility.PingObject(sceneObj);
							EditorGUIUtility.systemCopyBuffer = sceneName;
						}
						else
						{
							if (Event.current.shift)
							{
								// Shift click: open scene additive
								EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
							}
							else
							{
								// Left-click: open scene
								if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
								{
									EditorSceneManager.OpenScene(scene.path);
								}
							}
						}
					}
				}
				GUI.color = c;

				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				
				// Begin of dragging scene.
				// ————————————————————————
				style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleCenter;
				style.fixedWidth = 16.0f;
				GUIContent buttonText = new GUIContent("≡");
				Rect buttonRect = GUILayoutUtility.GetRect(buttonText, style); 
				if (Event.current.isMouse && buttonRect.Contains(Event.current.mousePosition))
				{ 
					if (!_isMouseDown && Event.current.type == EventType.MouseDown)
					{
						_isMouseDown = true;
						_dragIndex = i;
						_startIndex = _dragIndex;
						_lastDragIndex = i;
						_dragSceneName = sceneName;
						_indexList = new int[len];
						for (int j = 0; j < len; j++)
						{
							_indexList[j] = j;
						}
					}
					else if (Event.current.type == EventType.MouseUp)
					{
						_isMouseDown = false;
					}
				} 
				
				GUI.Button(buttonRect, buttonText, style);

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
			}

			// Drawing of reordering scenes.
			// —————————————————————————————
			if (_isMouseDown)
			{
				// Draw dragging item.
				Rect btnPos = new Rect(Event.current.mousePosition.x - Screen.width * 0.5f,
					Event.current.mousePosition.y - _btnHeight * 0.5f,
					Screen.width - 10.0f, _btnHeight);

				GUI.color = Color.cyan;
				GUI.Button(btnPos, _dragSceneName, new GUIStyle(EditorStyles.toolbarButton));
				GUI.color = c;

				// Get index for the new position.
				int index = Mathf.FloorToInt((Event.current.mousePosition.y - 42.0f) / (_btnHeight + (float) _btnPadding));
				index = (index >= len) ? len - 1 : (index < 0) ? 0 : index;

				if (_dragIndex != index)
				{
					_dragIndex = index;
				}

				if (_dragIndex != _lastDragIndex)
				{
					int last = _indexList[_lastDragIndex];
					_indexList[_lastDragIndex] = _indexList[_dragIndex];
					_indexList[_dragIndex] = last;
					_lastDragIndex = _dragIndex;
				}
			}

			// Updating of build settings.
			// ———————————————————————————
			if (GUI.changed)
			{
				EditorBuildSettings.scenes = (scenesMod == null) 
					? scenes 
					: scenesMod;
			}

			// Drag drop area.
			// ———————————————
			if (!Application.isPlaying)
			{
				DrawDragDropSceneArea();
			}
		}

		private void DrawHelpState()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();
			var toggle = (_state == ScenesPanelState.Info);
			var prev = toggle;
			toggle = GUILayout.Toggle(toggle, "Info", EditorStyles.toolbarButton);
			if (toggle != prev)
			{
				_state = (toggle)
					? ScenesPanelState.Info
					: ScenesPanelState.Default;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space(4.0f);
			EditorGUILayout.LabelField("SHIFT + click on a scene", EditorStyles.miniBoldLabel);
			EditorGUILayout.LabelField("Open scene additive.", EditorStyles.miniLabel);

			EditorGUILayout.Space(4.0f);
			EditorGUILayout.LabelField("Right-click on a scene", EditorStyles.miniBoldLabel);
			EditorGUILayout.LabelField("Ping it and copy it's name in the clipboard.", EditorStyles.miniLabel);
			EditorGUILayout.EndVertical();
		}

		private void DrawDragDropSceneArea()
		{
			Event e = Event.current;
			Rect dropRect = GUILayoutUtility.GetRect(0, 44, GUILayout.ExpandWidth(true));
			dropRect.x += 3;
			dropRect.y += 3;
			dropRect.width -= 6;
			EditorGUI.HelpBox(dropRect, "Drop Scenes here to add them to the build list.", MessageType.Info);

			switch (e.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!dropRect.Contains(e.mousePosition))
					{
						return;
					}

					bool isValid = true;

					// Verify if drop is valid (contains only scenes)
					foreach (Object dragged in DragAndDrop.objectReferences)
					{
						if (!dragged.ToString().EndsWith(".SceneAsset)"))
						{
							// Invalid
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
							isValid = false;
							break;
						}
					}

					if (!isValid)
					{
						return;
					}

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if (e.type == EventType.DragPerform)
					{
						// Add scenes
						DragAndDrop.AcceptDrag();
						EditorBuildSettingsScene[] currScenes = EditorBuildSettings.scenes;
						List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(currScenes.Length + DragAndDrop.objectReferences.Length);

						foreach (EditorBuildSettingsScene s in currScenes)
						{
							newScenes.Add(s);
						}

						foreach (Object dragged in DragAndDrop.objectReferences)
						{
							EditorBuildSettingsScene scene = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(dragged), true);
							newScenes.Add(scene);
						}

						EditorBuildSettings.scenes = newScenes.ToArray();
					}
					break;
			}
		}

		private static EditorBuildSettingsScene[] CloneAndRemove(EditorBuildSettingsScene[] scenes, int index)
		{
			EditorBuildSettingsScene[] res = new EditorBuildSettingsScene[scenes.Length - 1];
			int diff = 0;
			for (int i = 0; i < res.Length; ++i)
			{
				if (i == index)
				{
					diff++;
				}
				res[i] = scenes[i + diff];
			}
			return res;
		}

		private static bool IsAdditiveSceneLoaded(string scenePath)
		{
			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				if (SceneManager.GetSceneAt(i).path == scenePath)
				{
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}