using UnityEngine;
using UnityEditor;
using System;

namespace Anthill.Core
{
	[CustomEditor(typeof(AntEntity))]
	public class AntEntityEditor : UnityEditor.Editor
	{
	#region Private Variables

		private AntEntity _self;
		private bool _isFoldout;

	#endregion
	
	#region Unity Calls
	
		private void OnEnable()
		{
			_self = (AntEntity) target;
		}
	
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			GUILayout.Space(10.0f);
			EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
			GUI.enabled = false;
			EditorGUILayout.Toggle($"Added To Engine", _self.IsAddedToEngine);
			GUI.enabled = true;

			if (_self.IsAddedToEngine)
			{
				Type removeType = null;
				_isFoldout = EditorGUILayout.Foldout(_isFoldout, $"Components ({_self.Components.Count})", true);
				if (_isFoldout)
				{
					EditorGUILayout.BeginVertical();
					EditorGUI.indentLevel++;
					var componentType = typeof(Component);
					for (int i = 0, n = _self.Components.Count; i < n; i++)
					{
						var type = _self.Components[i].GetType();
						var isUnity = type.IsSubclassOf(componentType);
						var prefix = isUnity ? "●" : "○";
						var postfix = isUnity ? "(Mono)" : "";
						GUILayout.Label($"{prefix} {type.Name} {postfix}", EditorStyles.miniLabel);
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}

				if (removeType != null)
				{
					_self.Remove(removeType);
				}
			}
		}
	
	#endregion
	}
}