namespace Anthill.Cameras
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(AntCameraShakePreset))]
	public class AntCameraShakePresetEditor : UnityEditor.Editor
	{
		private AntCameraShakePreset _self;

		private void OnEnable()
		{
			_self = (AntCameraShakePreset) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUI.enabled = Application.isPlaying;
			if (GUILayout.Button("Test Shake!"))
			{
				if (AntCameraShake.Current != null)
				{
					AntCameraShake.Current.Shake(_self);
				}
				else
				{
					A.Warning("AntCameraShake not found!");
				}
			}

			if (GUILayout.Button("Stop Shaking"))
			{
				if (AntCameraShake.Current != null)
				{
					AntCameraShake.Current.CancelShake();
				}
				else
				{
					A.Warning("AntCameraShake not found!");
				}
			}
			GUI.enabled = true;

			if (!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Testing is available only in PlayMode.", MessageType.Info);
			}
		}
	}
}