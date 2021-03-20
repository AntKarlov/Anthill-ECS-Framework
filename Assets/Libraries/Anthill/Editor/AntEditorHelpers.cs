namespace Anthill.Editor
{
	using System.Reflection;
	using UnityEditorInternal;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public static class AntEditorHelpers
	{
		private static Dictionary<EditorWindow, GUIContent> _winTitleContentByEditor;
		private static FieldInfo _fi_editorWindowParent;
		private static MethodInfo _miRepaintCurrentEditor;

		/// <summary>
		/// Retuns array of sorting layers in the editor.
		/// </summary>
		/// <returns>Array of sorting layers.</returns>
		public static string[] GetSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty(
				"sortingLayerNames",
				BindingFlags.Static | BindingFlags.NonPublic
			);
			return (string[]) sortingLayersProperty.GetValue(null, new object[0]);
		}

		/// <summary>
		/// Returns of the indexes of sorting layers in the editor.
		/// </summary>
		/// <returns>Array of indexes of sorting layers.</returns>
		public static int[] GetSortingLayerUniqueIDs()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty(
				"sortingLayerUniqueIDs",
				BindingFlags.Static | BindingFlags.NonPublic
			);
			return (int[]) sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}

		/// <summary>
		/// Connects to a <see cref="ScriptableObject"/> asset.
		/// If the asset already exists at the given path, loads it and returns it.
		/// Otherwise, depending on the given parameters, either returns NULL or automatically creates it before loading and returning it.
		/// </summary>
		/// <typeparam name="T">Asset type</typeparam>
		/// <param name="aAdbFilePath">File path (relative to Unity's project folder).</param>
		/// <param name="createIfMissing">If TRUE and the requested asset doesn't exist, forces its creation.</param>
		/// <param name="createFoldersIfMissing">If TRUE also creates the path folders if they don't exist.</param>
		public static T ConnectToSourceAsset<T>(
			string aAdbFilePath, bool aCreateIfMissing = false, bool aCreateFoldersIfMissing = false
		) where T : ScriptableObject
		{
			if (!AntFileUtils.AssetExists(aAdbFilePath))
			{
				if (aCreateIfMissing)
				{
					CreateScriptableAsset<T>(aAdbFilePath, aCreateFoldersIfMissing);
				}
				else
				{
					return null;
				}
			}

			T source = (T) AssetDatabase.LoadAssetAtPath(aAdbFilePath, typeof(T));
			if (source == null)
			{
				// Source changed (or editor file was moved from outside of Unity): overwrite it.
				CreateScriptableAsset<T>(aAdbFilePath, aCreateFoldersIfMissing);
				source = (T) AssetDatabase.LoadAssetAtPath(aAdbFilePath, typeof(T));
			}

			return source;
		}

		/// <summary>
		/// Check if the <see cref="ScriptableObject"/> at the given path exists and eventually if it's available
		/// </summary>
		/// <param name="aAdbFilePath">File path (relative to Unity's project folder)</param>
		/// <param name="aCheckIfAvailable">If TRUE also check if the file is available
		/// (file can be unavailable if it was deleted outside Unity, or if Unity is just starting)</param>
		public static bool SourceAssetExists<T>(string aAdbFilePath, bool aCheckIfAvailable = true) where T : ScriptableObject
		{
			if (!AntFileUtils.AssetExists(aAdbFilePath))
			{
				return false;
			}

			if (!aCheckIfAvailable)
			{
				return true;
			}

			T source = (T) AssetDatabase.LoadAssetAtPath(aAdbFilePath, typeof(T));
			return source != null;
		}

		/// <summary>
		/// Returns TRUE if the given <see cref="EditorWindow"/> is dockable, FALSE if instead it's a utility window.
		/// </summary>
		// public static bool IsDockableWindow(EditorWindow aEditor)
		// {
		// 	if (_fi_editorWindowParent == null)
		// 	{
		// 		_fi_editorWindowParent = aEditor.GetType()
		// 			.GetField("m_Parent", BindingFlags.NonPublic | BindingFlags.Instance);
		// 	}
		// 	object parent = _fi_editorWindowParent.GetValue(aEditor);
		// 	if (parent == null)
		// 	{
		// 		A.Error("Parent is NULL, you should call this after the first GUI call happened.", "AntEditorHelpers");
		// 		return false;
		// 	}

		// 	return parent.GetType().ToString().Equals("UnityEditor.DockArea");
		// }

		/// <summary>
		/// Sets the icon and title of an editor window. Works with older versions of Unity, where the titleContent 
		/// property wasn't available.
		/// </summary>
		/// <param name="editor">Reference to the editor panel whose icon to set.</param>
		/// <param name="icon">Icon to apply.</param>
		/// <param name="title">Title. If NULL doesn't change it.</param>
		public static void SetWindowTitle(EditorWindow aEditor, Texture aIcon, string aTitle = null)
		{
			GUIContent titleContent;
			if (_winTitleContentByEditor == null)
			{
				_winTitleContentByEditor = new Dictionary<EditorWindow, GUIContent>();
			}

			if (_winTitleContentByEditor.ContainsKey(aEditor))
			{
				titleContent = _winTitleContentByEditor[aEditor];
				if (titleContent != null)
				{
					if (titleContent.image != aIcon)
					{
						titleContent.image = aIcon;
					}
					
					if (aTitle != null && titleContent.text != aTitle)
					{
						titleContent.text = aTitle;
					}
					return;
				}

				_winTitleContentByEditor.Remove(aEditor);
			}

			titleContent = GetWinTitleContent(aEditor);
			if (titleContent != null)
			{
				if (titleContent.image != aIcon)
				{
					titleContent.image = aIcon;
				}
				
				if (aTitle != null && titleContent.text != aTitle)
				{
					titleContent.text = aTitle;
				}

				_winTitleContentByEditor.Add(aEditor, titleContent);
			}
		}

		/// <summary>
		/// Repaints the currently focues editor.
		/// </summary>
		public static void RepaintCurrentEditor()
		{
			if (_miRepaintCurrentEditor == null)
			{
				_miRepaintCurrentEditor = typeof(EditorGUIUtility).GetMethod("RepaintCurrentWindow", BindingFlags.Static | BindingFlags.NonPublic);
			}

			_miRepaintCurrentEditor.Invoke(null, null);
		}

		#region Private Methods

		private static void CreateScriptableAsset<T>(string aAdbFilePath, bool aCreateFoldersIfMissing) where T : ScriptableObject
		{
			T data = ScriptableObject.CreateInstance<T>();
			if (aCreateFoldersIfMissing)
			{
				string[] folders = aAdbFilePath.Split(AntFileUtils.adbPathSlash.ToCharArray()[0]);
				string path = "Assets";
				for (int i = 1; i < folders.Length - 1; ++i)
				{
					string folder = folders[i];
					if (!AntFileUtils.AssetExists(path + AntFileUtils.adbPathSlash + folder))
					{
						AssetDatabase.CreateFolder(path, folder);
					}
					path = path + AntFileUtils.adbPathSlash + folder;
				}
			}
			AssetDatabase.CreateAsset(data, aAdbFilePath);
		}

		private static GUIContent GetWinTitleContent(EditorWindow aEditor)
		{
			const BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			PropertyInfo p = typeof(EditorWindow).GetProperty("cachedTitleContent", bFlags);
			return (p != null) 
				? (GUIContent) p.GetValue(aEditor, null)
				: null;
		}

		#endregion
	}
}