namespace Anthill.Builds
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEditor;
	using UnityEditor.Build.Reporting;
	using UnityEngine;
	using Anthill.Editor;
	using Anthill.Extensions;

	public class BuildManagerEditor : EditorWindow
	{
		public enum OnWillBuildResult
		{
			Continue, // Continue
			Cancel,   // Cancel this build
			CancelAll // Cancel all builds in queue
		}

		private enum BuildResult
		{
			Success,
			Failed,
			Canceled,
			CancelAll
		}

		/// <summary>
		/// Called when assigning the build's name. Hook to this to modify it and return the one you want.<para/>
		/// Must return a string with the name to use.
		/// </summary>
		public static event OnBuildNameRequestHandler OnBuildNameRequest;
		public delegate string OnBuildNameRequestHandler(BuildTarget aBuildTarget, string aBuildName);

		private static string Dispatch_OnBuildNameRequest(BuildTarget aBuildTarget, string aBuildName)
		{
			return (OnBuildNameRequest == null) ? aBuildName : OnBuildNameRequest(aBuildTarget, aBuildName);
		}

		/// <summary>
		/// Called before a build starts.<para/>
		/// Must return an <code>OnWillBuildResult</code> indicating if you wish to continue
		/// </summary>
		public static event OnWillBuildHandler OnWillBuild;
		public delegate OnWillBuildResult OnWillBuildHandler(BuildTarget aBuildTarget, bool aIsFirstBuildOfQueue);

		private static OnWillBuildResult Dispatch_OnWillBuild(BuildTarget aBuildTarget, bool aIsFirstBuildOfQueue)
		{
			return (OnWillBuild == null) ? OnWillBuildResult.Continue : OnWillBuild(aBuildTarget, aIsFirstBuildOfQueue);
		}
		
		[MenuItem("Tools/Anthill/Build Manager")]
		private static void ShowWindow()
		{
			GetWindow(typeof(BuildManagerEditor), false, "Build Manager");
		}

	#region Private Variables
		
		private const string _srcAdbFilePath = "Assets/BuildManagerData.asset";
		static readonly StringBuilder _strb = new StringBuilder();
		static readonly StringBuilder _strbAlt = new StringBuilder();

		private BuildManagerData _src;
		private Vector2 _scrollPos;
		private const int _labelWidth = 116;
		private string _buildFolderComment;
		private string[] _buildPathsLabels;
		private Color _delColor = "#FF6E2A".HexToColor();
	
	#endregion

	#region Unity Calls

		private void OnEnable()
		{
			if (_src == null)
			{
				_src = AntEditorHelpers.ConnectToSourceAsset<BuildManagerData>(_srcAdbFilePath, true);
			}

			RefreshBuildPathsLabels();
			_buildFolderComment = string.Format(
				"The build folder is relative to your Unity's project folder:\n\n\"{0}/\"\n\nYou can use \"../\" to navigate backwards",
				AntFileUtils.projectPath.Replace('\\', '/')
			);

			Undo.undoRedoPerformed += Repaint;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= Repaint;
		}

		private void OnFocus()
		{
			if (_src == null)
			{
				_src = AntEditorHelpers.ConnectToSourceAsset<BuildManagerData>(_srcAdbFilePath, true);
			}

			RefreshBuildPathsLabels();
		}

		private void OnGUI()
		{
			if (_src == null)
			{
				_src = AntEditorHelpers.ConnectToSourceAsset<BuildManagerData>(_srcAdbFilePath, true);
			}

			_scrollPos = GUILayout.BeginScrollView(_scrollPos);
			Undo.RecordObject(_src, "Build Manager");
			
			// Main toolbar.
			// —————————————
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				var caption = (_src.settingsFoldout)
					? $"▼ Settings"
					: $"► Settings";

				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				_src.settingsFoldout = GUILayout.Toggle(_src.settingsFoldout, caption, style);

				GUI.color = Color.yellow;
				if (GUILayout.Button("BUILD ALL ENABLED", EditorStyles.toolbarButton, GUILayout.Width(140.0f)))
				{
					BuildAllEnabled();
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			if (_src.settingsFoldout)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				{
					_src.version = EditorGUILayout.TextField("Version", _src.version);

					EditorGUILayout.BeginHorizontal();
					{
						_src.build = EditorGUILayout.IntField("Build", _src.build);
						if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(18.0f), GUILayout.Height(18.0f)))
						{
							_src.build += 1;
						}

						if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(18.0f), GUILayout.Height(18.0f)))
						{
							_src.build -= 1;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				GUILayout.Space(10.0f);
			}

			// Affixes.
			// ————————
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				var caption = (_src.prefixesFoldout)
					? $"▼ Prefixes"
					: $"► Prefixes";

				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				_src.prefixesFoldout = GUILayout.Toggle(_src.prefixesFoldout, caption, style);

				if (GUILayout.Button("+ Add Prefix", EditorStyles.toolbarButton, GUILayout.Width(80)))
				{
					_src.prefixesFoldout = true;
					_src.prefixes.Add(new BuildManagerData.Affix());
					GUI.changed = true;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (_src.prefixesFoldout && _src.prefixes.Count > 0)
			{
				DrawAffixes(_src.prefixes);
			}

			// Suffixes.
			// —————————
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				var caption = (_src.suffixesFoldout)
					? $"▼ Suffixes"
					: $"► Suffixes";

				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				_src.suffixesFoldout = GUILayout.Toggle(_src.suffixesFoldout, caption, style);

				if (GUILayout.Button("+ Add Suffix", EditorStyles.toolbarButton, GUILayout.Width(80)))
				{
					_src.suffixesFoldout = true;
					_src.suffixes.Add(new BuildManagerData.Affix());
					GUI.changed = true;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (_src.suffixesFoldout && _src.suffixes.Count > 0)
			{
				DrawAffixes(_src.suffixes);
			}

			if (GUI.changed)
			{
				RefreshBuildPathsLabels();
			}
			
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				GUILayout.Label("Builds", EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button("+ Add platform", EditorStyles.toolbarButton))
				{
					AddPlatform();
				}

				if (GUILayout.Button("▲", EditorStyles.toolbarButton))
				{
					foreach (BuildManagerData.Build build in _src.builds)
					{
						build.foldout = false;
					}
					GUI.changed = true;
				}

				if (GUILayout.Button("▼", EditorStyles.toolbarButton))
				{
					foreach (BuildManagerData.Build build in _src.builds)
					{
						build.foldout = true;
					}
					GUI.changed = true;
				}
			}
			EditorGUILayout.EndHorizontal();

			// Builds.
			// ———————
			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < _src.builds.Count; ++i)
			{
				if (!DrawBuild(i))
				{
					i--;
					GUI.changed = true;
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				GUI.changed = true;
			}

			GUILayout.EndScrollView();
			if (GUI.changed)
			{
				RefreshBuildPathsLabels();
				EditorUtility.SetDirty(_src);
			}
		}

		private void DrawAffixes(List<BuildManagerData.Affix> aAffixes)
		{
			var c = GUI.color;
			EditorGUILayout.BeginVertical();

			for (int i = 0; i < aAffixes.Count; ++i)
			{
				BuildManagerData.Affix affix = aAffixes[i];
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{	
					GUI.color = (affix.enabled) ? Color.green : Color.white;
					affix.enabled = GUILayout.Toggle(affix.enabled, "On", EditorStyles.toolbarButton, GUILayout.Width(30));
					GUI.color = c;

					affix.text = EditorGUILayout.TextField(affix.text, GUI.skin.FindStyle("ToolbarTextField"), GUILayout.ExpandWidth(true));
					if (!string.IsNullOrEmpty(affix.text) && affix.text.StartsWith("@"))
					{
						if (GUILayout.Button("Test", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
						{
							Debug.Log(affix.GetText(true, _src));
						}
					}

					GUI.enabled = affix.enabled;
					affix.enabledForInnerExecutable = GUILayout.Toggle(
						affix.enabledForInnerExecutable,
						new GUIContent("Win/Linux Filename", "If toggled applies this also to the filename on WIN/Linux builds only."),
						EditorStyles.toolbarButton,
						GUILayout.Width(121)
					);
					GUI.enabled = true;

					// GUILayout.FlexibleSpace();
					GUI.color = _delColor;
					if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(16.0f)))
					{
						aAffixes.RemoveAt(i);
						--i;
						GUI.changed = true;
					}
					GUI.color = c;
				}
				EditorGUILayout.EndHorizontal();
			}

			// EditorGUILayout.HelpBox(
			// 	"If you start an affix with \"@\" it will be evaluated as a static string property/field via Reflection.",
			// 	MessageType.Info
			// );
			// GUILayout.Space(10.0f);
			EditorGUILayout.EndVertical();
		}

		// Returns FALSE if the given build was deleted
		private bool DrawBuild(int aIndex)
		{
			BuildManagerData.Build build = _src.builds[aIndex];

			// Toolbar.
			// ————————
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				GUI.color = (build.enabled) ? Color.green : Color.white;
				build.enabled = GUILayout.Toggle(build.enabled, "On", EditorStyles.toolbarButton, GUILayout.Width(30));
				GUI.color = Color.white;
				
				var caption = (build.foldout)
					? $"▼ {build.buildName} ({build.buildTarget.ToString()})"
					: $"► {build.buildName} ({build.buildTarget.ToString()})";

				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				build.foldout = GUILayout.Toggle(build.foldout, caption, style);

				// GUILayout.FlexibleSpace();
				if (GUILayout.Button("BUILD NOW", EditorStyles.toolbarButton, GUILayout.Width(80)))
				{
					Build(build);
				}

				GUI.color = _delColor;
				if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(16.0f)))
				{
					_src.builds.RemoveAt(aIndex);
					return false;
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			// Data.
			// —————
			if (build.foldout)
			{
				GUI.enabled = build.enabled;
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					GUIStyle style = new GUIStyle(EditorStyles.miniBoldLabel);
					style.richText = true;
					EditorGUILayout.LabelField(_buildPathsLabels[aIndex], style);
					if (!build.BuildsDirectlyToFile() || build.buildTarget == BuildTarget.StandaloneOSX) 
					{
						build.clearBuildFolder = GUILayout.Toggle(
							build.clearBuildFolder,
							new GUIContent(
								"Clear At Build",
								"If selected and a build with the same name already exists, deletes the build contents before creating the new build"
							),
							EditorStyles.toolbarButton,
							GUILayout.Width(_labelWidth)
						);
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				{
					// Special Android/iOS behaviour
					// bool hasIncreaseInternalBuildNumberOption = false;
					// string increaseInternalBuildNumberOptionButton = "";
					// string increaseInternalBuildNumberOptionDescr = "";

					// switch (build.buildTarget)
					// {
					// 	case BuildTarget.Android:
					// 		// hasIncreaseInternalBuildNumberOption = true;
					// 		increaseInternalBuildNumberOptionButton = "Auto-increase bundleVersionCode";
					// 		increaseInternalBuildNumberOptionDescr = "If selected auto-increases the Android \"bundleVersionCode\" at each build";
					// 		break;
						
					// 	case BuildTarget.iOS:
					// 		// hasIncreaseInternalBuildNumberOption = true;
					// 		increaseInternalBuildNumberOptionButton = "Auto-increase buildNumber";
					// 		increaseInternalBuildNumberOptionDescr = "If selected auto-increases the iOS \"buildNumber\" at each build";
					// 		break;
					// }

					// if (hasIncreaseInternalBuildNumberOption)
					// {
					// 	build.increaseInternalBuildNumber = GUILayout.Toggle(
					// 		build.increaseInternalBuildNumber,
					// 		new GUIContent(increaseInternalBuildNumberOptionButton, increaseInternalBuildNumberOptionDescr),
					// 		GUILayout.ExpandWidth(false)
					// 	);
					// }

					build.buildFolder = EditorGUILayout.TextField(new GUIContent("Build Folder", _buildFolderComment), build.buildFolder);

					GUI.color = (string.IsNullOrEmpty(build.buildName)) ? Color.red : Color.white;
					build.buildName = EditorGUILayout.TextField("Build Name", build.buildName);

					GUI.color = string.IsNullOrEmpty(build.bundleIdentifier) ? Color.red : Color.white;
					build.bundleIdentifier = EditorGUILayout.TextField("Bundle Identifier", build.bundleIdentifier);
					
					// Special Android/iOS behaviour
					switch (build.buildTarget)
					{
						case BuildTarget.Android:
							build.key = EditorGUILayout.TextField("Key Alias Password", build.key);
							build.buildAppBundle = EditorGUILayout.Toggle("Build App Bundle", build.buildAppBundle);
							break;
					}

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Define Symbols", EditorStyles.boldLabel);
						if (GUILayout.Button("", "OL Plus", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(18.0f)))
						{
							build.defineSymbols.Add(new BuildManagerData.Build.DefineSymbol
							{
								kind = BuildManagerData.Build.DefineKind.AddIfNotExists,
								value = ""
							});
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginVertical();
					{
						if (build.defineSymbols == null)
						{
							build.defineSymbols = new List<BuildManagerData.Build.DefineSymbol>();
						}

						if (build.defineSymbols.Count > 0)
						{
							int delSymbolIndex = -1;
							for (int i = 0, n = build.defineSymbols.Count; i < n; i++)
							{
								var item = build.defineSymbols[i];
								EditorGUILayout.BeginHorizontal();
								{
									item.kind = (BuildManagerData.Build.DefineKind) EditorGUILayout.EnumPopup(item.kind);
									item.value = EditorGUILayout.TextField(item.value);
									if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(18.0f)))
									{
										delSymbolIndex = i;
									}
									build.defineSymbols[i] = item;
								}
								EditorGUILayout.EndHorizontal();
							}

							if (delSymbolIndex > -1)
							{
								build.defineSymbols.RemoveAt(delSymbolIndex);
							}
						}
						else
						{
							EditorGUILayout.LabelField("No Define Symbols", EditorStyles.centeredGreyMiniLabel);
						}
						EditorGUILayout.Space(2.0f);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndVertical();
				GUILayout.Space(10.0f);
				GUI.enabled = true;
			}

			return true;
		}

	#endregion

	#region Private Methods

		private void AddPlatform()
		{
			GenericMenu menu = new GenericMenu();
			foreach (BuildTarget buildTarget in BuildManagerData.AllowedBuildTargets) 
			{
				menu.AddItem(new GUIContent(buildTarget.ToString()), false, AddPlatformApply, buildTarget);
			}
			menu.ShowAsContext();
		}

		private void AddPlatformApply(object aBuildTargetObj)
		{
			BuildTarget buildTarget = (BuildTarget) aBuildTargetObj;
			_src.builds.Add(new BuildManagerData.Build(buildTarget));
			EditorUtility.SetDirty(_src);
			RefreshBuildPathsLabels();
		}

		private void BuildAllEnabled()
		{
			EditorUtility.DisplayProgressBar("Build All", "Preparing...", 0.2f);
			// Use delayed call to prevent Unity GUILayout bug.
			AntEditorUtils.ClearAllDelayedCalls();
			AntEditorUtils.DelayedCall(0.1f, () => DoBuildAllEnabled());
		}

		private void DoBuildAllEnabled()
		{
			int totEnabled = 0;
			foreach (BuildManagerData.Build build in _src.builds)
			{
				if (build.enabled)
				{
					totEnabled++;
				}
			}

			if (totEnabled == 0)
			{
				EditorUtility.DisplayDialog("Build All", "0 platforms enabled, nothing to build", "Ok");
				EditorUtility.ClearProgressBar();
				return;
			}

			bool proceed = EditorUtility.DisplayDialog(
				"Build All",
				$"Build for {totEnabled} platforms?",
				"Yes", "Cancel"
			);

			if (!proceed)
			{
				EditorUtility.ClearProgressBar();
				return;
			}

			for (int i = 0; i < _src.builds.Count; i++)
			{
				BuildManagerData.Build build = _src.builds[i];
				if (!build.enabled)
				{
					continue;
				}
				
				// Hook ► Verify if build should continue
				OnWillBuildResult onWillBuildResult = Dispatch_OnWillBuild(build.buildTarget, i == 0);

				switch (onWillBuildResult)
				{
					case OnWillBuildResult.Cancel:
						EditorUtility.ClearProgressBar();
						continue;

					case OnWillBuildResult.CancelAll:
						EditorUtility.ClearProgressBar();
						return;
				}
				
				BuildResult result = DoBuild(build);
				
				if (result == BuildResult.Success)
				{
					Debug.Log("BuildManager ► All builds is successfully done!");
				}
				else if (result == BuildResult.Failed)
				{
					Debug.LogWarning("BuildManager ► Build failure! Something went wrong.");
				}

				if (result == BuildResult.CancelAll)
				{
					Debug.LogWarning("BuildManager ► Build process is canceled.");
					return;
				}
			}

			EditorUtility.ClearProgressBar();
		}

		private void Build(BuildManagerData.Build aBuild)
		{
			// Hook ► Verify if build should continue
			OnWillBuildResult onWillBuildResult = Dispatch_OnWillBuild(aBuild.buildTarget, true);
			if (onWillBuildResult != OnWillBuildResult.Continue)
			{
				return;
			}

			// EditorUtility.DisplayProgressBar($"Build ({aBuild.buildTarget})", "Preparing...", 0.2f);
			// Use delayed call to prevent Unity GUILayout bug
			AntEditorUtils.ClearAllDelayedCalls();
			AntEditorUtils.DelayedCall(0.1f, () => DoBuild(aBuild));
		}

		/// <summary>
		/// Returns TRUE if all builds in queue should be canceled.
		/// </summary>
		/// <param name="aBuild"></param>
		/// <returns></returns>
		private BuildResult DoBuild(BuildManagerData.Build aBuild)
		{
			string dialogTitle = $"Build ({aBuild.buildTarget})";

			if (string.IsNullOrEmpty(aBuild.buildFolder))
			{
				bool cancelAll = !EditorUtility.DisplayDialog(
					dialogTitle, 
					"Build folder can't be empty!", "Ok", "Cancel All"
				);

				EditorUtility.ClearProgressBar();
				return (cancelAll) ? BuildResult.CancelAll : BuildResult.Canceled;
			}

			if (string.IsNullOrEmpty(aBuild.bundleIdentifier))
			{
				bool cancelAll = !EditorUtility.DisplayDialog(
					dialogTitle, 
					"Bundle Identifier can't be empty!", "Ok", "Cancel All"
				);

				EditorUtility.ClearProgressBar();
				return (cancelAll) ? BuildResult.CancelAll : BuildResult.Canceled;
			}

			string buildFolder = Path.GetFullPath(AntFileUtils.projectPath + "/" + aBuild.buildFolder);

			if (!Directory.Exists(buildFolder))
			{
				bool cancelAll = !EditorUtility.DisplayDialog(
					dialogTitle, 
					$"Build folder doesn't exist!\n\n\"{buildFolder}\"", 
					"Ok", "Cancel All"
				);

				EditorUtility.ClearProgressBar();
				return (cancelAll) ? BuildResult.CancelAll : BuildResult.Canceled;
			}

			bool buildIsSingleFile = aBuild.BuildsDirectlyToFile();
			string completeBuildFolder = buildIsSingleFile
				? buildFolder
				: Path.GetFullPath(buildFolder + AntFileUtils.pathSlash + GetFullBuildName(aBuild, false));

			string buildFilePath = aBuild.buildTarget == BuildTarget.iOS || aBuild.buildTarget == BuildTarget.WebGL
				? completeBuildFolder
				: completeBuildFolder + AntFileUtils.pathSlash + GetFullBuildName(aBuild, true);

			if (!buildIsSingleFile)
			{
				if (aBuild.clearBuildFolder && Directory.Exists(completeBuildFolder))
				{
					// Clear build folder
					string[] files = Directory.GetFiles(completeBuildFolder);
					for (int i = 0; i < files.Length; ++i)
					{
						File.Delete(files[i]);
					}

					string[] subdirs = Directory.GetDirectories(completeBuildFolder);
					for (int i = 0; i < subdirs.Length; ++i)
					{
						Directory.Delete(subdirs[i], true);
					}
				}
			}
			else if (aBuild.clearBuildFolder)
			{
				// Clear build file if it's a directory (OSX)
				if (aBuild.buildTarget == BuildTarget.StandaloneOSX && Directory.Exists(buildFilePath))
				{
					Directory.Delete(buildFilePath, true);
				}
			}

			// Set define symbols
			for (int i = 0, n = aBuild.defineSymbols.Count; i < n; i++)
			{
				if (aBuild.defineSymbols[i].kind == BuildManagerData.Build.DefineKind.AddIfNotExists)
				{
					AntEditorUtils.AddGlobalDefine(aBuild.defineSymbols[i].value);
				}
				else if (aBuild.defineSymbols[i].kind == BuildManagerData.Build.DefineKind.RemoveIfExists)
				{
					AntEditorUtils.RemoveGlobalDefine(aBuild.defineSymbols[i].value);
				}
			}

			// Build
			PlayerSettings.bundleVersion = _src.version;
			switch (aBuild.buildTarget)
			{
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneOSX:
				case BuildTarget.StandaloneLinux64:
					// todo: set build number for standalone
					PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, aBuild.bundleIdentifier);
					break;

				case BuildTarget.Android:
					PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, aBuild.bundleIdentifier);
					// _src.build = _src.build + (aBuild.increaseInternalBuildNumber ? 1 : 0);
					PlayerSettings.Android.bundleVersionCode = _src.build;
					// PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + (aBuild.increaseInternalBuildNumber ? 1 : 0);
					PlayerSettings.Android.keystorePass = PlayerSettings.Android.keyaliasPass = aBuild.key;
					EditorUserBuildSettings.buildAppBundle = aBuild.buildAppBundle;
					break;

				case BuildTarget.iOS:
					PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, aBuild.bundleIdentifier);
					// _src.build = _src.build + (aBuild.increaseInternalBuildNumber ? 1 : 0);
					// PlayerSettings.iOS.buildNumber = PlayerSettings.iOS.buildNumber + (aBuild.increaseInternalBuildNumber ? 1 : 0);
					PlayerSettings.iOS.buildNumber = _src.build.ToString();
					break;

				case BuildTarget.WebGL:
					PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, aBuild.bundleIdentifier);
					break;
			}

			BuildPlayerOptions buildOptions = new BuildPlayerOptions();
			string[] scenes = new string[EditorBuildSettings.scenes.Length];
			for (int i = 0; i < scenes.Length; ++i)
			{
				scenes[i] = EditorBuildSettings.scenes[i].path;
			}

			buildOptions.scenes = scenes;
			buildOptions.locationPathName = buildFilePath;
			buildOptions.target = aBuild.buildTarget;
			if (EditorUserBuildSettings.development)
			{
				buildOptions.options = buildOptions.options | BuildOptions.Development;
			}

			if (EditorUserBuildSettings.allowDebugging)
			{
				buildOptions.options = buildOptions.options | BuildOptions.AllowDebugging;
			}

			BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

			return (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) 
				? BuildResult.Success 
				: BuildResult.Failed;
		}

		private void RefreshBuildPathsLabels()
		{
			_buildPathsLabels = new string[_src.builds.Count];
			for (int i = 0, n = _src.builds.Count; i < n; ++i)
			{
				_buildPathsLabels[i] = GetFullBuildPathLabel(_src.builds[i]);
			}
		}

		private bool ValidateBuildData(BuildManagerData.Build aBuild)
		{
			return !string.IsNullOrEmpty(aBuild.buildFolder) &&
				!string.IsNullOrEmpty(aBuild.buildName) &&
				!string.IsNullOrEmpty(aBuild.bundleIdentifier);
		}

		private string GetFullBuildPathLabel(BuildManagerData.Build aBuild)
		{
			_strb.Length = 0;
			_strb.Append(AntFileUtils.projectPath);
			if (!ValidateBuildData(aBuild))
			{
				_strb.Replace('\\', '/');
				_strb.Insert(0, "<b>");
				_strb.Append("</b>");
				return _strb.ToString();
			}

			_strb.Append(AntFileUtils.pathSlash).Append(aBuild.buildFolder);
			string fullPath = Path.GetFullPath(_strb.ToString());
			_strb.Length = 0;
			_strb.Append(fullPath).Append(AntFileUtils.pathSlash);
			
			_strb.Append("<b><color=#008000>").Append(GetFullBuildName(aBuild, aBuild.BuildsDirectlyToFile())).Append("</color></b>");
			if (aBuild.HasInnerExecutable())
			{
				_strb.Append('/').Append(GetFullBuildName(aBuild, true));
			}

			_strb.Replace('\\', '/');
			return _strb.ToString();
		}

		private string GetFullBuildName(BuildManagerData.Build aBuild, bool withExtension)
		{
			bool isInnerExecutable = (withExtension && aBuild.HasInnerExecutable());
			_strbAlt.Length = 0;
			foreach (BuildManagerData.Affix affix in _src.prefixes)
			{
				if (affix.enabled && (!isInnerExecutable || affix.enabledForInnerExecutable))
				{
					_strbAlt.Append(affix.GetText(false, _src));
				}
			}

			_strbAlt.Append(aBuild.buildName);
			foreach (BuildManagerData.Affix affix in _src.suffixes)
			{
				if (affix.enabled && (!isInnerExecutable || affix.enabledForInnerExecutable))
				{
					_strbAlt.Append(affix.GetText(false, _src));
				}
			}

			string result = _strbAlt.ToString();
			if (!isInnerExecutable)
			{
				string prevResult = result;
				result = Dispatch_OnBuildNameRequest(aBuild.buildTarget, result);
				if (string.IsNullOrEmpty(result))
				{
					result = prevResult;
				}
			}

			if (withExtension)
			{
				switch (aBuild.buildTarget)
				{
					case BuildTarget.StandaloneWindows64 :
					case BuildTarget.StandaloneWindows :
						result += ".exe";
						break;

					case BuildTarget.StandaloneOSX :
						result += ".app";
						break;
					
					case BuildTarget.Android :
						result += (aBuild.buildAppBundle) ? ".aab" : ".apk";
						break;
				}
			}
			return result;
		}

	#endregion
	}
}