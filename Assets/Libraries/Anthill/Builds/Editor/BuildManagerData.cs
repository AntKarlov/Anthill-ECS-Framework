namespace Anthill.Builds
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using Anthill.Extensions;

	[Serializable]
	public class BuildManagerData : ScriptableObject
	{
		public bool settingsFoldout = false;
		public string version = "1.0";
		public int build = 1;

		public bool prefixesFoldout = false;
		public bool suffixesFoldout = false;
		public List<Affix> prefixes = new List<Affix>();
		public List<Affix> suffixes = new List<Affix>();
		public List<Build> builds = new List<Build>();

		public static readonly BuildTarget[] AllowedBuildTargets = new []
		{
			BuildTarget.StandaloneWindows64,
			BuildTarget.StandaloneOSX,
			BuildTarget.StandaloneLinux64,
			BuildTarget.Android,
			BuildTarget.iOS,
			BuildTarget.WebGL
		};

		[Serializable]
		public class Affix
		{
			public bool enabled = true;

			// If TRUE also uses this affix when determining the fileName on standalone platforms
			public bool enabledForInnerExecutable = false;
			public string text;

			// Returns the right text value, evaluating it as a static string property in case it starts with a @
			public string GetText(bool aLogErrors, BuildManagerData aData)
			{
				if (aData != null && !string.IsNullOrEmpty(text))
				{
					if (text.Contains("{version}"))
					{
						return aData.version;
					}
					else if (text.Contains("{build}"))
					{
						return aData.build.ToString();
					}
				}

				return (string.IsNullOrEmpty(text) || !text.StartsWith("@"))
					? text
					: text.Substring(1).EvalAsProperty<string>(null, aLogErrors);
			}
		}
		
		[Serializable]
		public class Build
		{
			public enum DefineKind
			{
				AddIfNotExists,
				RemoveIfExists
			}

			[Serializable]
			public struct DefineSymbol
			{
				public DefineKind kind;
				public string value;
			}

			public BuildTarget buildTarget;
			public bool foldout = true;
			public bool enabled = true;

			// Relative to project directory
			public string buildFolder;

			// Folder within buildFolder where the build will be created (this one is created if it doesn't exist) 
			public string buildName;

			public bool clearBuildFolder = true;
			public string bundleIdentifier;

			public List<DefineSymbol> defineSymbols = new List<DefineSymbol>();

			// Android/iOS only
			public string key;
			public bool buildAppBundle = false;
			// public bool increaseInternalBuildNumber = false;

			public Build(BuildTarget buildTarget)
			{
				this.buildTarget = buildTarget;
			}

			public bool HasInnerExecutable()
			{
				switch (buildTarget)
				{
					case BuildTarget.StandaloneWindows:
					case BuildTarget.StandaloneWindows64:
					case BuildTarget.StandaloneLinux64:
						return true;
					
					default:
						return false;
				}
			}

			public bool BuildsDirectlyToFile()
			{
				switch (buildTarget)
				{
					case BuildTarget.StandaloneOSX:
					case BuildTarget.Android:
						return true;
					
					default:
						return false;
				}
			}
		}
	}
}