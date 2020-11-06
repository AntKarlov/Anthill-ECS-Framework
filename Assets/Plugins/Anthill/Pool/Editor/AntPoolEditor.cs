namespace Anthill.Pool
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;

	[CustomEditor(typeof(AntPoolList))]
	public class AntPoolEditor : Editor
	{
		private AntPoolList _self;
		private string _searchQuery = string.Empty;
		private int _removeIndex = -1;
		private bool _confirmRemove;

		#region Unity Calls

		private void OnEnable()
		{
			_self = (AntPoolList) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawPoolList();

			EditorUtility.SetDirty(_self);
			serializedObject.ApplyModifiedProperties();
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
						AntArray.Add<AntPoolList.Item>(ref _self.items, new AntPoolList.Item
						{
							prefab = newObject,
							initialSize = 5,
							isGrow = true,
							maxGrowing = 20,
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
				EditorGUILayout.LabelField("The Pool List is Empty.");
			}
			else
			{
				GUILayout.BeginVertical();
				{
					Color c = GUI.color;
					AntPoolList.Item curItem;
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
									_confirmRemove = GUILayout.Button("Delete?", GUILayout.MaxWidth(70.0f), GUILayout.MaxHeight(16.0f));

									GUI.color = Color.green;
									if (GUILayout.Button("X", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
									{
										_removeIndex = -1;
									}

									GUI.color = c;
								}
								else
								{
									if (GUILayout.Button("X", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
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

								curItem.prefab = (GameObject) EditorGUILayout.ObjectField("Object Prefab", curItem.prefab, typeof(GameObject), false);
								curItem.initialSize = EditorGUILayout.IntField("Initial Size", curItem.initialSize);

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

				EditorGUILayout.LabelField(string.Concat("Total Objects: ", _self.items.Length.ToString()));
			}

			if (_removeIndex != 1 && _confirmRemove)
			{
				AntArray.RemoveAt<AntPoolList.Item>(ref _self.items, _removeIndex);
				_confirmRemove = false;
				_removeIndex = -1;
			}
		}

		private bool IsExists(GameObject aObject)
		{
			int index = System.Array.FindIndex(_self.items, x => System.Object.ReferenceEquals(x.prefab, aObject));
			return (index >= 0 && index < _self.items.Length);
		}

		#endregion
	}
}