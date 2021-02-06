namespace Anthill.Filters
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(AntShockwavePreset))]
	public class AntShockwavePresetEditor : Editor
	{
		private AntShockwavePreset _self;

		private void OnEnable()
		{
			_self = (AntShockwavePreset) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUI.enabled = Application.isPlaying;
			if (GUILayout.Button("Test!"))
			{
				if (AntShockWaveFilter.Current == null)
				{
					A.Warning("AntShockwaveFilter not found!");
					return;
				}

				AntShockWaveFilter.Current.Animate(_self, AntShockWaveFilter.Current.transform.position);
			}
			GUI.enabled = true;

			if (!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Testing is available only in PlayMode.", MessageType.Info);
			}
		}
	}
}