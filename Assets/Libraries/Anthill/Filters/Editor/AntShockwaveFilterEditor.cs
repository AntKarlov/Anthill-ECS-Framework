namespace Anthill.Filters
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	// using Anthill.Editor;

	[CustomEditor(typeof(AntShockWaveFilter))]
	public class AntShockwaveFilterEditor : Editor
	{
		private AntShockWaveFilter _self;
		private ReorderableList _reorderableList;
		private int _currentPickerWindow;

		#region Unity Calls

		private void OnEnable()
		{
			_self = (AntShockWaveFilter) target;
			if (_self.presets != null)
			{
				_reorderableList = new ReorderableList(_self.presets, typeof(AntShockwavePreset), true, true, true, true);
				_reorderableList.drawHeaderCallback += DrawHeaderHandler;
				_reorderableList.drawElementCallback += DrawElementHandler;
				_reorderableList.onAddCallback += AddItemHandler;
				_reorderableList.onRemoveCallback += RemoveItemHandler;
				_reorderableList.onSelectCallback += SelectElementHandler;
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			// if (GUILayout.Button("Create Shockwave Preset"))
			// {
			// 	_self.presets.Add(ScriptableObjectUtility.CreateAsset<AntShockwavePreset>("ShockwavePreset"));
			// }

			_reorderableList.DoLayoutList();

			if (Event.current.commandName == "ObjectSelectorUpdated" && 
				EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindow)
			{
				var preset = (AntShockwavePreset) EditorGUIUtility.GetObjectPickerObject();
				if (preset != null && !_self.presets.Contains(preset))
				{
					_self.presets.Add(preset);
				}
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			_self.createByMouseClick = EditorGUILayout.BeginToggleGroup("Create By Mouse Click", _self.createByMouseClick);
			if (_self.createByMouseClick)
			{
				EditorGUILayout.HelpBox("Select Shockwave Preset in the Presets List and click on the screen during GamePlay mode.", MessageType.Info);
			}
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.EndVertical();
		}

		#endregion
		#region Event Handlers

		private void SelectElementHandler(ReorderableList aList)
		{
			_self.selectedPreset = aList.index;
		}
		
		private void DrawHeaderHandler(Rect aRect)
		{
			GUI.Label(aRect, "Shockwave Presets");
		}

		private void DrawElementHandler(Rect aRect, int aIndex, bool aActive, bool aFocused)
		{
			EditorGUI.BeginChangeCheck();
			string presetName = (_self.presets[aIndex] != null)
				? _self.presets[aIndex].name
				: "<none>";

			// Name of preset.
			EditorGUI.LabelField(
				new Rect(
					aRect.x, 
					aRect.y, 
					aRect.width * 0.5f, 
					EditorGUIUtility.singleLineHeight * 1.1f), 
				presetName);

			// Edit button.
			if (GUI.Button(
				new Rect(
					aRect.x + 2.0f * aRect.width / 4.0f + 5.0f,
					aRect.y,
					aRect.width / 4.0f - 5.0f,
					EditorGUIUtility.singleLineHeight * 1.1f), 
				"Edit"))
			{
				Selection.activeObject = _self.presets[aIndex];
			}

			// Test button.
			GUI.enabled = Application.isPlaying;
			if (GUI.Button(
				new Rect(
					aRect.x + 3.0f * aRect.width / 4.0f + 5.0f,
					aRect.y,
					aRect.width / 4.0f - 5.0f,
					EditorGUIUtility.singleLineHeight * 1.1f),
				"Test!"))
			{
				_self.Animate(_self.presets[aIndex], _self.transform.position);
			}
			GUI.enabled = true;

			EditorGUI.EndChangeCheck();
		}

		private void AddItemHandler(ReorderableList aList)
		{
			_currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
			EditorGUIUtility.ShowObjectPicker<AntShockwavePreset>(null, false, "", _currentPickerWindow);
		}

		private void RemoveItemHandler(ReorderableList aList)
		{
			_self.presets.RemoveAt(aList.index);
		}
		
		#endregion
	}
}