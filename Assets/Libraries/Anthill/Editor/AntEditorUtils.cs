namespace Anthill.Editor
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	public static class AntEditorUtils
	{
	#region Private Variables
		
		private static readonly List<DelayedCall> _delayedCalls = new List<DelayedCall>();
		private static MethodInfo _clearConsoleMI;
		private static readonly List<GameObject> _rootGOs = new List<GameObject>(500);
		private static readonly StringBuilder _strb = new StringBuilder();
		private static float _editorUiScaling = -1;
		private static MethodInfo _miGetTargetStringFromBuildTargetGroup;
		private static MethodInfo _miGetPlatformNameFromBuildTargetGroup;
		private static MethodInfo _miGetAnnotations;
		private static MethodInfo _miSetGizmoEnabled;
		private static int _miSetGizmoEnabledTotParms;
		private static MethodInfo _miSetIconEnabled;
		
	#endregion
		
	#region Public Methods

		/// <summary>
		/// Calls the given action after the given delay.
		/// </summary>
		public static DelayedCall DelayedCall(float aDelay, Action aCallback)
		{
			DelayedCall res = new DelayedCall(aDelay, aCallback);
			_delayedCalls.Add(res);
			return res;
		}

		public static void ClearAllDelayedCalls()
		{
			for (int i = 0, n = _delayedCalls.Count; i < n; i++)
			{
				_delayedCalls[i].Clear();
			}
			_delayedCalls.Clear();
		}

		public static void ClearDelayedCall(DelayedCall aCall)
		{
			aCall.Clear();
			int index = _delayedCalls.IndexOf(aCall);
			if (index >= 0 && index < _delayedCalls.Count) 
			{
				_delayedCalls.Remove(aCall);
			}
		}

		/// <summary>
		/// Return the size of the editor game view, eventual extra bars excluded 
		/// (meaning the true size of the game area).
		/// </summary>
		public static Vector2 GetGameViewSize()
		{
			return Handles.GetMainGameViewSize();
		}

		/// <summary>
		/// Returns a value from 1 to N (2 for 200% scaling) indicating the UI Scaling of Unity's editor.
		/// The first time this is called it will store the scaling and keep it without refreshing,
		/// since you need to restart Unity in order to apply a scaling change
		/// </summary>
		public static float GetEditorUiScaling()
		{
			if (_editorUiScaling < 0)
			{
				// EditorPrefs method: I prefer Reflection method because I'm not sure on OSX, 
				// where you can't set UI scaling manually, this is used 
				// _editorUiScaling = EditorPrefs.GetInt("CustomEditorUIScale") * 0.01f;
				PropertyInfo p = typeof(GUIUtility).GetProperty(
					"pixelsPerPoint", 
					BindingFlags.Static | BindingFlags.NonPublic
				);

				if (p != null)
				{
					_editorUiScaling = (float) p.GetValue(null, null);
				}
				else
				{
					_editorUiScaling = 1;
				}
			}
			return _editorUiScaling;
		}

		/// <summary>
		/// Clears all logs from Unity's console.
		/// </summary>
		public static void ClearConsole()
		{
			if (_clearConsoleMI == null)
			{
				Type logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
				if (logEntries != null)
				{
					_clearConsoleMI = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
				}

				if (_clearConsoleMI == null)
				{
					return;
				}
			}

			_clearConsoleMI.Invoke(null,null);
		}

		/// <summary>
		/// Adds the given global define (if it's not already present) to all the <see cref="BuildTargetGroup"/>
		/// or only to the given <see cref="BuildTargetGroup"/>, depending on passed parameters,
		/// and returns TRUE if it was added, FALSE otherwise.<para/>
		/// NOTE: when adding to all of them some legacy warnings might appear, which you can ignore.
		/// </summary>
		/// <param name="aId"></param>
		/// <param name="aBuildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to add to all of them.</param>
		public static bool AddGlobalDefine(string aId, BuildTargetGroup? aBuildTargetGroup = null)
		{
			bool added = false;
			BuildTargetGroup[] targetGroups = (aBuildTargetGroup == null)
				? (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup))
				: new[] { (BuildTargetGroup) aBuildTargetGroup };

			foreach (BuildTargetGroup btg in targetGroups)
			{
				if (!IsValidBuildTargetGroup(btg))
				{
					continue;
				}

				string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
				string[] singleDefs = defs.Split(';');
				if (System.Array.IndexOf(singleDefs, aId) != -1)
				{
					// Already present
					continue; 
				}

				added = true;
				defs += defs.Length > 0 ? ";" + aId : aId;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, defs);
			}
			return added;
		}

		/// <summary>
		/// Removes the given global define (if present) from all the <see cref="BuildTargetGroup"/>
		/// or only from the given <see cref="BuildTargetGroup"/>, depending on passed parameters,
		/// and returns TRUE if it was removed, FALSE otherwise.<para/>
		/// NOTE: when removing from all of them some legacy warnings might appear, which you can ignore.
		/// </summary>
		/// <param name="aId"></param>
		/// <param name="aBuildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to remove from all of them.</param>
		public static bool RemoveGlobalDefine(string aId, BuildTargetGroup? aBuildTargetGroup = null)
		{
			bool removed = false;
			BuildTargetGroup[] targetGroups = (aBuildTargetGroup == null)
				? (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup))
				: new[] { (BuildTargetGroup) aBuildTargetGroup };

			foreach (BuildTargetGroup btg in targetGroups)
			{
				if (!IsValidBuildTargetGroup(btg))
				{
					continue;
				}

				string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
				string[] singleDefs = defs.Split(';');
				if (System.Array.IndexOf(singleDefs, aId) == -1)
				{
					// Not present
					continue; 
				}

				removed = true;
				_strb.Length = 0;
				for (int i = 0, n = singleDefs.Length; i < n; ++i)
				{
					if (singleDefs[i] == aId)
					{
						continue;
					}

					if (_strb.Length > 0)
					{
						_strb.Append(';');
					}

					_strb.Append(singleDefs[i]);
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, _strb.ToString());
			}

			_strb.Length = 0;
			return removed;
		}

		/// <summary>
		/// Returns TRUE if the given global define is present in all the <see cref="BuildTargetGroup"/>
		/// or only in the given <see cref="BuildTargetGroup"/>, depending on passed parameters.<para/>
		/// </summary>
		/// <param name="aId"></param>
		/// <param name="aBuildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to check in all of them.</param>
		public static bool HasGlobalDefine(string aId, BuildTargetGroup? aBuildTargetGroup = null)
		{
			BuildTargetGroup[] targetGroups = (aBuildTargetGroup == null)
				? (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup))
				: new[] { (BuildTargetGroup) aBuildTargetGroup };

			foreach(BuildTargetGroup btg in targetGroups)
			{
				if (!IsValidBuildTargetGroup(btg))
				{
					continue;
				}

				string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
				string[] singleDefs = defs.Split(';');
				if (System.Array.IndexOf(singleDefs, aId) != -1)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Sets the gizmos icon visibility in the Scene and Game view for the given class names
		/// Uses code from Zwer99 on UnityAnswers (thank you): https://answers.unity.com/questions/851470/how-to-hide-gizmos-by-script.html
		/// </summary>
		/// <param name="aVisible">Visibility</param>
		/// <param name="aClassNames">Class names (no namespace), as many as you want separated by a comma</param>
		public static void SetGizmosIconVisibility(bool aVisible, params string[] aClassNames)
		{
			if (!StoreAnnotationsReflectionMethods())
			{
				return;
			}

			int setValue = (aVisible) ? 1 : 0;
			var annotations = _miGetAnnotations.Invoke(null, null);
			foreach (object annotation in (IEnumerable)annotations)
			{
				Type annotationType = annotation.GetType();
				FieldInfo fiClassId = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
				FieldInfo fiScriptClass = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
				if (fiClassId == null || fiScriptClass == null)
				{
					continue;
				}

				string scriptClass = (string)fiScriptClass.GetValue(annotation);
				bool found = false;
				for (int i = 0; i < aClassNames.Length; ++i)
				{
					if (aClassNames[i] != scriptClass)
					{
						continue;
					}
					found = true;
					break;
				}

				if (!found)
				{
					continue;
				}

				int classId = (int) fiClassId.GetValue(annotation);
				if (_miSetGizmoEnabledTotParms == 4)
				{
					// Unity 2019 or newer
					_miSetGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, setValue, true });
				}
				else
				{
					// Unity 2018 or older
					_miSetGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, setValue });
				}

				_miSetIconEnabled.Invoke(null, new object[] { classId, scriptClass, setValue });
			}
		}

		/// <summary>
		/// Sets the gizmos icon visibility in the Scene and Game view for all custom icons
		/// (for example icons created with HOTools)
		/// Uses code from Zwer99 on UnityAnswers (thank you): https://answers.unity.com/questions/851470/how-to-hide-gizmos-by-script.html
		/// </summary>
		/// <param name="aVisible">Visibility</param>
		public static void SetGizmosIconVisibilityForAllCustomIcons(bool aVisible)
		{
			// Note: works by checking class ID being 114 (otherwise I could check if scriptClass is not nullOrEmpty
			if (!StoreAnnotationsReflectionMethods())
			{
				return;
			}

			int setValue = (aVisible) ? 1 : 0;
			var annotations = _miGetAnnotations.Invoke(null, null);
			foreach (object annotation in (IEnumerable) annotations)
			{
				Type annotationType = annotation.GetType();
				FieldInfo fiClassId = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
				FieldInfo fiScriptClass = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
				if (fiClassId == null || fiScriptClass == null)
				{
					continue;
				}

				int classId = (int) fiClassId.GetValue(annotation);
				if (classId != 114)
				{
					continue;
				}

				string scriptClass = (string)fiScriptClass.GetValue(annotation);
				if (_miSetGizmoEnabledTotParms == 4)
				{
					// Unity 2019 or newer
					_miSetGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, setValue, true });
				}
				else
				{
					// Unity 2018 or older
					_miSetGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, setValue });
				}

				_miSetIconEnabled.Invoke(null, new object[] { classId, scriptClass, setValue });
			}
		}

	#endregion

	#region Private Methods

		private static bool IsValidBuildTargetGroup(BuildTargetGroup group)
		{
			if (group == BuildTargetGroup.Unknown)
			{
				return false;
			}

			if (_miGetTargetStringFromBuildTargetGroup == null)
			{
				Type moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
				// MethodInfo miIsPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
				
				_miGetTargetStringFromBuildTargetGroup = moduleManager.GetMethod(
					"GetTargetStringFromBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic
				);

				_miGetPlatformNameFromBuildTargetGroup = typeof(PlayerSettings).GetMethod(
					"GetPlatformName", BindingFlags.Static | BindingFlags.NonPublic
				);
			}

			string targetString = (string) _miGetTargetStringFromBuildTargetGroup.Invoke(null, new object[] { group });
			string platformName = (string) _miGetPlatformNameFromBuildTargetGroup.Invoke(null, new object[] { group });

			// Group is valid if at least one betweeen targetString and platformName is not empty.
			// This seems to me the safest and more reliant way to check,
			// since ModuleManager.IsPlatformSupportLoaded dosn't work well with BuildTargetGroup (only BuildTarget)
			return (!string.IsNullOrEmpty(targetString) || !string.IsNullOrEmpty(platformName));
		}

		/// <summary>
		/// Returns FALSE if the annotations API weren't found.
		/// </summary>
		private static bool StoreAnnotationsReflectionMethods()
		{
			if (_miGetAnnotations != null)
			{
				return true;
			}

			Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.AnnotationUtility");
			if (type == null)
			{
				return false;
			}

			_miGetAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
			_miSetGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
			if (_miSetGizmoEnabled != null)
			{
				_miSetGizmoEnabledTotParms = _miSetGizmoEnabled.GetParameters().Length;
			}

			_miSetIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
			return true;
		}

	#endregion
	}

	public class DelayedCall
	{
		public float delay;
		public Action callback;
		private readonly float _startupTime;

		public DelayedCall(float aDelay, Action aCallback)
		{
			delay = aDelay;
			callback = aCallback;
			_startupTime = Time.realtimeSinceStartup;
			EditorApplication.update += UpdateHandler;
		}

		public void Clear()
		{
			if (EditorApplication.update != null)
			{
				EditorApplication.update -= UpdateHandler;
			}
			callback = null;
		}

		private void UpdateHandler()
		{
			if (Time.realtimeSinceStartup - _startupTime >= delay)
			{
				if (EditorApplication.update != null)
				{
					EditorApplication.update -= UpdateHandler;
				}
				
				if (callback != null)
				{
					callback();
				}
			}
		}
	}
}