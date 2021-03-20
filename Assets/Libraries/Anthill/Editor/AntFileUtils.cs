namespace Anthill.Editor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	public static class AntFileUtils
	{
		/// <summary>Path slash for AssetDatabase format</summary>
		public static readonly string adbPathSlash = "/";

		/// <summary>Path slash to replace for AssetDatabase format</summary>
		public static readonly string adbPathSlashToReplace = "\\";

		/// <summary>Current OS path slash</summary>
		public static readonly string pathSlash;
		
		/// <summary>Path slash to replace on current OS</summary>
		public static readonly string pathSlashToReplace;

		/// <summary>
		/// Full path to project directory, without final slash.
		/// </summary>
		public static string projectPath
		{
			get
			{
				if (_fooProjectPath == null)
				{
					_fooProjectPath = Application.dataPath;
					_fooProjectPath = _fooProjectPath.Substring(0, _fooProjectPath.LastIndexOf(adbPathSlash));
					_fooProjectPath = _fooProjectPath.Replace(adbPathSlash, pathSlash);
				}
				return _fooProjectPath;
			}
		}

		/// <summary>
		/// Full path to project's Assets directory, without final slash.
		/// </summary>
		public static string assetsPath
		{
			get => projectPath + pathSlash + "Assets";
		}

		private static readonly StringBuilder _strb = new StringBuilder();
		private static readonly Char[] _validFilenameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-()!.$&+- ".ToCharArray();
		private static string _fooProjectPath;

	#region Constructor

		static AntFileUtils()
		{
			bool useWindowsSlashes = (Application.platform == RuntimePlatform.WindowsEditor);
			pathSlash = useWindowsSlashes ? "\\" : "/";
			pathSlashToReplace = useWindowsSlashes ? "/" : "\\";
		}

	#endregion

	#region Public Methods

		/// <summary>
		/// Returns TRUE if the given path is an absolute path.
		/// </summary>
		public static bool IsFullPath(string aPath)
		{
			return (aPath[1] == ':');
		}

		/// <summary>
		/// Returns TRUE if the given path is an AssetDatabase path.
		/// </summary>
		public static bool IsAdbPath(string aPath)
		{
			return aPath.StartsWith("Assets");
		}

		/// <summary>
		/// Returns TRUE if the given GUID refers to a valid and existing project folder.
		/// </summary>
		public static bool IsProjectFolder(string aAssetGuid)
		{
			if (string.IsNullOrEmpty(aAssetGuid))
			{
				return false;
			}

			string adbPath = AssetDatabase.GUIDToAssetPath(aAssetGuid);
			return (!string.IsNullOrEmpty(adbPath) && Directory.Exists(AntFileUtils.AdbPathToFullPath(adbPath)));
		}

		/// <summary>
		/// Converts the given project-relative path to a full path.
		/// </summary>
		public static string AdbPathToFullPath(string aAdbPath)
		{
			aAdbPath = aAdbPath.Replace(adbPathSlash, pathSlash);
			return projectPath + pathSlash + aAdbPath;
		}

		/// <summary>
		/// Converts the given full path to a project-relative path.
		/// </summary>
		public static string FullPathToADBPath(string fullPath)
		{
			return fullPath.Substring(projectPath.Length + 1).Replace(adbPathSlashToReplace, adbPathSlash);
		}

		/// <summary>
		/// Returns TRUE if the file/directory at the given path exists.
		/// </summary>
		/// <param name="adbPath">Path, relative to Unity's project folder.</param>
		public static bool AssetExists(string aAdbPath)
		{
			string fullPath = AdbPathToFullPath(aAdbPath);
			return (File.Exists(fullPath) || Directory.Exists(fullPath));
		}

		/// <summary>
		/// Validates the string as a valid fileName
		/// (uses commonly accepted characters an all systems instead of system-specific ones).<para/>
		/// BEWARE: doesn't check for reserved words
		/// </summary>
		/// <param name="s">string to replace</param>
		/// <param name="minLength">Minimum length for considering the string valid</param>
		public static bool IsValidFileName(string aStr, int aMinLength = 2)
		{
			if (string.IsNullOrEmpty(aStr) || aStr.Length < aMinLength)
			{
				return false;
			}

			foreach (char c in aStr)
			{
				if (Array.IndexOf(_validFilenameChars, c) == -1)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns TRUE if the given filepath is within this Unity project Assets folder.
		/// </summary>
		/// <param name="aFullFilePath">Full file path.</param>
		public static bool FilePathIsWithinUnityProjectAssets(string aFullFilePath)
		{
			return ApplySystemDirectorySeparators(aFullFilePath)
				.StartsWith(assetsPath);
		}

		/// <summary>
		/// Returns the given string stripped of any invalid filename characters.<para/>
		/// BEWARE: doesn't check for reserved words
		/// </summary>
		/// <param name="aStr">string to replace</param>
		/// <param name="aReplaceWith">Character to use as replacement for invalid ones</param>
		public static string ConvertToValidFilename(string aStr, char aReplaceWith = '_')
		{
			_strb.Length = 0;
			char[] schars = aStr.ToCharArray();
			foreach (char c in schars)
			{
				_strb.Append(Array.IndexOf(_validFilenameChars, c) == -1 ? aReplaceWith : c);
			}
			return _strb.ToString();
		}

		/// <summary>
		/// Returns the given path with all slashes converted to the correct ones used by the system.
		/// </summary>
		public static string ApplySystemDirectorySeparators(string aPath)
		{
			return aPath.Replace(pathSlashToReplace, pathSlash);
		}

		/// <summary>
		/// Returns the asset path of the given GUID (relative to Unity project's folder),
		/// or an empty string if either the GUID is invalid or the related path doesn't exist.
		/// </summary>
		public static string GUIDToExistingAssetPath(string aGuid)
		{
			if (string.IsNullOrEmpty(aGuid))
			{
				return "";
			}

			string assetPath = AssetDatabase.GUIDToAssetPath(aGuid);
			if (string.IsNullOrEmpty(assetPath))
			{
				return "";
			}

			if (AssetExists(assetPath))
			{
				return assetPath;
			}
			return "";
		}

		public static void CreateScriptableObjectInCurrentFolder<T>() where T : ScriptableObject
		{
			if (Selection.activeObject == null)
			{
				return;
			}

			string currAdbFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (currAdbFolder == "")
			{
				currAdbFolder = "Assets";
			}
			else if (Path.GetExtension(currAdbFolder) != "")
			{
				currAdbFolder = currAdbFolder.Substring(0, currAdbFolder.IndexOf('.'));
			}

			if (!Directory.Exists(AntFileUtils.AdbPathToFullPath(currAdbFolder)))
			{
				A.Warning("No valid project folder selected.", "AntFileUtils");
				return;
			}

			string name = typeof(T).ToString();
			int dotIndex = name.LastIndexOf('.');
			if (dotIndex != -1)
			{
				name = name.Substring(dotIndex + 1);
			}

			string adbPath = AssetDatabase.GenerateUniqueAssetPath(currAdbFolder + string.Format("/New {0}.asset", name));
			T instance = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(instance, adbPath);
		}

		/// <summary>
		/// Checks if the given directory (full path) is empty or not.
		/// </summary>
		public static bool IsEmpty(string aDir)
		{
			DirectoryInfo dInfo = new DirectoryInfo(aDir);
			if (dInfo.GetFiles().Length > 0)
			{
				return false;
			}

			if (dInfo.GetDirectories().Length > 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Deletes all files and subdirectories from the given directory.
		/// </summary>
		public static void MakeEmpty(string dir)
		{
			DirectoryInfo dInfo = new DirectoryInfo(dir);
			foreach (FileInfo f in dInfo.GetFiles())
			{
				if (f.Extension != "meta")
				{
					AssetDatabase.DeleteAsset(FullPathToADBPath(f.ToString()));
				}
			}

			foreach (DirectoryInfo d in dInfo.GetDirectories())
			{
				AssetDatabase.DeleteAsset(FullPathToADBPath(d.ToString()));
			}
		}

		/// <summary>
		/// Returns the adb path to the given ScriptableObject.
		/// </summary>
		public static string MonoInstanceADBPath(ScriptableObject aScriptableObj)
		{
			MonoScript ms = MonoScript.FromScriptableObject(aScriptableObj);
			return AssetDatabase.GetAssetPath(ms);
		}

		/// <summary>
		/// Returns the adb path to the given MonoBehaviour.
		/// </summary>
		public static string MonoInstanceADBPath(MonoBehaviour aMonoBeh)
		{
			MonoScript ms = MonoScript.FromMonoBehaviour(aMonoBeh);
			return AssetDatabase.GetAssetPath(ms);
		}

		/// <summary>
		/// Returns the adb directory that contains the given ScriptableObject without final slash.
		/// </summary>
		public static string MonoInstanceADBDir(ScriptableObject aScriptableObj)
		{
			MonoScript ms = MonoScript.FromScriptableObject(aScriptableObj);
			string res = AssetDatabase.GetAssetPath(ms);
			return res.Substring(0, res.LastIndexOf(adbPathSlash));
		}

		/// <summary>
		/// Returns the adb directory that contains the given MonoBehaviour without final slash.
		/// </summary>
		public static string MonoInstanceADBDir(MonoBehaviour aMonoBeh)
		{
			MonoScript ms = MonoScript.FromMonoBehaviour(aMonoBeh);
			string res = AssetDatabase.GetAssetPath(ms);
			return res.Substring(0, res.LastIndexOf(adbPathSlash));
		}

		/// <summary>
		/// Returns the adb paths to the selected folders in the Project panel, or NULL if there is none.
		/// Contrary to Selection.activeObject, which only returns folders selected in the right side of the panel,
		/// this method also works with folders selected in the left side.
		/// </summary>
		public static List<string> SelectedAdbDirs()
		{
			List<string> selectedPaths = null;
			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) 
			{
				string adbPath = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(adbPath) && Directory.Exists(AdbPathToFullPath(adbPath))) 
				{
					if (selectedPaths == null)
					{
						selectedPaths = new List<string>();
					}
					selectedPaths.Add(adbPath);
				}
			}
			return selectedPaths;
		}

		/// <summary>
		/// Sets the script execution order of the given MonoBehaviour.
		/// </summary>
		public static void SetScriptExecutionOrder(MonoBehaviour aMonoBeh, int aOrder)
		{
			MonoScript ms = MonoScript.FromMonoBehaviour(aMonoBeh);
			MonoImporter.SetExecutionOrder(ms, aOrder);
		}

		/// <summary>
		/// Gets the script execution order of the given MonoBehaviour.
		/// </summary>
		public static int GetScriptExecutionOrder(MonoBehaviour aMonoBeh)
		{
			MonoScript ms = MonoScript.FromMonoBehaviour(aMonoBeh);
			return MonoImporter.GetExecutionOrder(ms);
		}

	#endregion
	}
}