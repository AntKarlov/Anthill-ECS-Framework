namespace Anthill.Fsm
{
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Custom editor for the AntFsmPreset scriptable object.
	/// </summary>
	[CustomEditor(typeof(AntFsmPreset))]
	public class AntFsmPresetEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Open FSM Workbench", GUILayout.MinHeight(40.0f)))
			{
				AntFsmWorkbench.OpenPreset(target.name);
			}
		}
	}
}