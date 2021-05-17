namespace Anthill.Pools
{
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	[CustomEditor(typeof(AntPoolLoader))]
	public class AntPoolLoaderEditor : Editor
	{
		private AntPoolLoader _self;

		private void OnEnable()
		{
			_self = (AntPoolLoader) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				AntPoolPreset newPool = null;
				newPool = (AntPoolPreset) EditorGUILayout.ObjectField("Add Pool", newPool, typeof(AntPoolPreset), true);
				if (newPool != null)
				{
					if (!IsExists(newPool))
					{
						AntArray.Add(ref _self.pools, newPool);
					}
					else
					{
						EditorUtility.DisplayDialog("Oops!", $"Object `{newPool.name}` already added to the list!", "Ok");
					}
				}

				GUILayout.Space(3.0f);
				if (_self.pools.Length > 0)
				{
					EditorGUI.indentLevel++;
					int delIndex = -1;
					for (int i = 0, n = _self.pools.Length; i < n; i++)
					{
						GUILayout.BeginHorizontal();
						
						_self.pools[i] = (AntPoolPreset) EditorGUILayout.ObjectField(_self.pools[i], typeof(AntPoolPreset), true);

						if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
						{
							delIndex = i;
						}

						GUILayout.EndHorizontal();
					}

					if (delIndex > -1)
					{
						AntArray.RemoveAt(ref _self.pools, delIndex);
					}
					EditorGUI.indentLevel--;
				}
				else
				{
					EditorGUILayout.LabelField("No Presets for loading.", EditorStyles.centeredGreyMiniLabel);
				}
			}
			GUILayout.Space(3.0f);
			GUILayout.EndVertical();

			GUILayout.Space(10.0f);
			EditorGUILayout.LabelField("Initialize Settings", EditorStyles.boldLabel);
			_self.loadOnStart = EditorGUILayout.Toggle("Load On Start", _self.loadOnStart);
			_self.countPerStep = EditorGUILayout.IntField("Populate Per Step", _self.countPerStep);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}
		}

		private bool IsExists(AntPoolPreset aObject)
		{
			int index = System.Array.FindIndex(_self.pools, x => System.Object.ReferenceEquals(x, aObject));
			return (index >= 0 && index < _self.pools.Length);
		}
	}
}