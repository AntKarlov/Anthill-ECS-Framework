namespace Anthill.Cameras
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	// using Anthill.Editor;

	[CustomEditor(typeof(AntCameraShake))]
	public class AntCameraShakeEditor : Editor
	{
		private AntCameraShake _self;
		private ReorderableList _reorderableList;
		private int _currentPickerWindow;

		#region Unity Calls

		private void OnEnable()
		{
			_self = (AntCameraShake) target;
			if (_self.presets != null)
			{
				_reorderableList = new ReorderableList(_self.presets, typeof(AntCameraShakePreset), true, true, true, true);
				_reorderableList.drawHeaderCallback += DrawHeaderHandler;
				_reorderableList.drawElementCallback += DrawElementHandler;
				_reorderableList.onAddCallback += AddItemHandler;
				_reorderableList.onRemoveCallback += RemoveItemHandler;
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			// if (GUILayout.Button("Create ShakePreset"))
			// {
			// 	_self.presets.Add(ScriptableObjectUtility.CreateAsset<AntCameraShakePreset>("ShakePreset"));
			// }

			_reorderableList.DoLayoutList();

			if (Event.current.commandName == "ObjectSelectorUpdated" && 
				EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindow)
			{
				var preset = (AntCameraShakePreset) EditorGUIUtility.GetObjectPickerObject();
				if (preset != null && !_self.presets.Contains(preset))
				{
					_self.presets.Add(preset);
				}
			}
		}

		#endregion
		#region Event Handlers
		
		private void DrawHeaderHandler(Rect aRect)
		{
			GUI.Label(aRect, "Shake Presets");
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

			// Shake button.
			GUI.enabled = Application.isPlaying;
			if (GUI.Button(
				new Rect(
					aRect.x + 3.0f * aRect.width / 4.0f + 5.0f,
					aRect.y,
					aRect.width / 4.0f - 5.0f,
					EditorGUIUtility.singleLineHeight * 1.1f),
				"Shake!"))
			{
				_self.Shake(_self.presets[aIndex]);
			}
			GUI.enabled = true;

			EditorGUI.EndChangeCheck();
		}

		private void AddItemHandler(ReorderableList aList)
		{
			_currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
			EditorGUIUtility.ShowObjectPicker<AntCameraShakePreset>(null, false, "", _currentPickerWindow);
		}

		private void RemoveItemHandler(ReorderableList aList)
		{
			_self.presets.RemoveAt(aList.index);
		}
		
		#endregion
	}
}