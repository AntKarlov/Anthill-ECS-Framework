namespace Anthill.Pools
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;

	[CustomEditor(typeof(AntPoolPreset))]
	public class AntPoolPresetEditor : Editor
	{
	#region Private Variables

		private AntPoolPreset _self;
		private string _searchQuery = string.Empty;
		private int _removeIndex = -1;
		private bool _confirmRemove;

	#endregion

	#region Unity Calls

		private void OnEnable()
		{
			_self = (AntPoolPreset) target;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();

			DrawPoolList();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_self);
				serializedObject.ApplyModifiedProperties();
			}
		}

	#endregion

	#region Private Methods

		private void DrawPoolList()
		{
			GUILayout.BeginHorizontal();
			{
				_searchQuery = GUILayout.TextField(_searchQuery, GUI.skin.FindStyle("ToolbarSeachTextField")/*, GUILayout.MinWidth(150.0f)*/);
				if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")/*, GUILayout.MaxWidth(150.0f)*/))
				{
					_searchQuery = string.Empty;
					GUI.FocusControl(null);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			{
				GameObject newObject = null;
				newObject = (GameObject) EditorGUILayout.ObjectField("Drop to Add", newObject, typeof(GameObject), false);
				if (newObject != null)
				{
					if (!IsExists(newObject))
					{
						AntArray.Add(ref _self.items, new AntPoolPreset.Item
						{
							prefab = newObject,
							initialSize = 5,
							isGrow = true,
							isLimitCapacity = false,
							maxCapacity = 5,
							isOpened = true
						});
					}
					else
					{
						EditorUtility.DisplayDialog("Oops!", $"Object `{newObject.name}` already exists in the pool list!", "Ok");
					}
				}
			}
			GUILayout.EndVertical();
			
			if (_self.items.Length == 0)
			{
				EditorGUILayout.LabelField("No Poolable objects in the list.", EditorStyles.centeredGreyMiniLabel);
			}
			else
			{
				GUILayout.BeginVertical();
				{
					Color c = GUI.color;
					AntPoolPreset.Item curItem;
					for (int i = _self.items.Length - 1; i >= 0; i--)
					{
						curItem = _self.items[i];
						if (curItem.prefab == null || (curItem.prefab != null && curItem.prefab.name.ToLower().Contains(_searchQuery.ToLower())))
						{
							EditorGUI.indentLevel++;
							if (curItem.prefab == null)
							{
								GUI.color = Color.red;
							}
							GUILayout.BeginVertical(EditorStyles.helpBox);
							GUI.color = c;

							GUILayout.BeginHorizontal();
							{
								curItem.isOpened = (curItem.prefab != null)
									? EditorGUILayout.Foldout(curItem.isOpened, curItem.prefab.name, true)
									: curItem.isOpened = EditorGUILayout.Foldout(curItem.isOpened, "<Missed Prefab>", true);
								
								EditorGUILayout.LabelField(string.Format("{0}", curItem.initialSize), GUILayout.MaxWidth(60.0f));

								if (_removeIndex == i && !_confirmRemove)
								{
									GUI.color = Color.red;
									_confirmRemove = GUILayout.Button("Delete?", GUILayout.MaxWidth(70.0f), GUILayout.MaxHeight(18.0f));

									GUI.color = Color.green;
									if (GUILayout.Button("X", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(18.0f)))
									{
										_removeIndex = -1;
									}

									GUI.color = c;
								}
								else
								{
									if (GUILayout.Button("X", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(18.0f)))
									{
										_removeIndex = i;
									}
								}
								
								GUI.color = c;
							}
							GUILayout.EndHorizontal();

							if (curItem.isOpened)
							{
								GUILayout.Space(2.0f);
								EditorGUI.indentLevel++;

								curItem.prefab = (GameObject) EditorGUILayout.ObjectField("Prefab", curItem.prefab, typeof(GameObject), false);
								curItem.initialSize = EditorGUILayout.IntField("Initial Size", curItem.initialSize);
								curItem.isGrow = EditorGUILayout.Toggle("Is Growing", curItem.isGrow);
								GUI.enabled = curItem.isGrow;
								curItem.isLimitCapacity = EditorGUILayout.Toggle("Limit Growing", curItem.isLimitCapacity);
								GUI.enabled = curItem.isLimitCapacity;
								curItem.maxCapacity = EditorGUILayout.IntField("Max Capacity", curItem.maxCapacity);
								GUI.enabled = true;

								EditorGUI.indentLevel--;
								GUILayout.Space(2.0f);
							}

							_self.items[i] = curItem;
							EditorGUI.indentLevel--;
							GUILayout.EndVertical();
						}
					}
				}
				GUILayout.EndVertical();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(string.Concat("Total Objects: ", _self.items.Length.ToString()));
					if (GUILayout.Button("Generate Enum of all Pools"))
					{
						var str = GenerateEnum();
						var te = new TextEditor();
						te.text = str;
						te.SelectAll();
						te.Copy();

						Debug.Log("Enum copied into clip board!");
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			if (_removeIndex != 1 && _confirmRemove)
			{
				AntArray.RemoveAt(ref _self.items, _removeIndex);
				_confirmRemove = false;
				_removeIndex = -1;
			}
		}

		private bool IsExists(GameObject aObject)
		{
			int index = System.Array.FindIndex(_self.items, x => System.Object.ReferenceEquals(x.prefab, aObject));
			return (index >= 0 && index < _self.items.Length);
		}

		private string GenerateEnum()
		{
			var str = "public enum Poolable\n{\n";
			var obs = Resources.FindObjectsOfTypeAll<AntPoolPreset>();
			for (int i = 0, n = obs.Length; i < n; i++)
			{
				var poolAsset = obs[i];
				for (int j = 0, jn = poolAsset.items.Length; j < jn; j++)
				{
					str = string.Concat(str, "\t", poolAsset.items[j].prefab.name, ",\n");
				}
			}
			str = string.Concat(str, "}");
			return str;
		}

	#endregion
	}
}