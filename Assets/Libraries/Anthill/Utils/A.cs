using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

public enum Verbosity
{
	None,              // No logs.
	Verbose,           // Log everything, verbose logs included.
	Normal,            // Log everything except logs marked as verbose.
	ErrorsAndWarnings, // Log only errors and warnings.
	Errors             // Log only errors.
}

public static class A
{
	private enum LogKind
	{
		Normal,
		Warning,
		Error
	}

	private struct LogResult
	{
		public bool success;
		public string message;
	}

	/// <summary>
	/// Sets the runtime verbosity value, which indicates the type of messages that will be logged.
	/// </summary>
	public static Verbosity verbosity = Verbosity.Normal;

	/// <summary>
	/// Sets the editor verbosity value, witch indicates the type of messages that will be logged.
	/// </summary>
	public static Verbosity editorVerbosity = Verbosity.Normal;

	/// <summary>
	/// If FALSE doesn't log anything, nothing in editor and runtime.
	/// </summary>
	public static bool enabled = true;

	/// <summary>
	/// If TRUE strips all HTML tags from the logs.
	/// </summary>
	public static bool stripHtmlTags = false;

	private const string _editorPrefix = "[Editor] ";
	private const string _verboseColor = "<color=#666666>";
	private const string _senderColor = "<color=#6b854a>";
	private const string _senderColorImportant = "<color=#785814>";
	private const string _verboseSenderColor = "<color=#6b854a>";
	private static readonly Regex _tagRegex = new Regex(@"<[^>]*>", RegexOptions.Multiline);
	private static readonly StringBuilder _stringBuilder = new StringBuilder();

#region Public Methods

	/// <summary>
	/// Logs the given message.
	/// </summary>
	public static void Log(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
	{
		var result = CompileMessage(-1, false, LogKind.Normal, aSender, aMessage, false, aHexColor, false);
		if (result.success)
		{
			Debug.Log(result.message, aContext);
		}
	}

	/// <summary>
	/// Logs the given message. Force message will log if <see cref="verbosity"/> is set to None
	/// but force message won't log if the logger is not enabled.
	/// </summary>
	public static void LogForce(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
	{
		var result = CompileMessage(-1, false, LogKind.Normal, aSender, aMessage, false, aHexColor, true);
		if (result.success)
		{
			Debug.Log(result.message, aContext);
		}
	}

	/// <summary>
	/// Logs the given message.
	/// </summary>
	public static void Verbose(object aMessage, object aSender = null, Object aContext = null)
	{
		var result = CompileMessage(-1, false, LogKind.Normal, aSender, aMessage, true, null, false);
		if (result.success)
		{
			Debug.Log(result.message, aContext);
		}
	}

	/// <summary>
	/// Logs the given warning message.
	/// </summary>
	public static void Warning(object aMessage, object aSender = null, Object aContext = null)
	{
		var result = CompileMessage(-1, false, LogKind.Warning, aSender, aMessage, false, null, false);
		if (result.success)
		{
			Debug.LogWarning(result.message, aContext);
		}
	}
	
	/// <summary>
	/// Logs the given error message.
	/// </summary>
	public static void Error(object aMessage, object aSender = null, Object aContext = null)
	{
		var result = CompileMessage(-1, false, LogKind.Error, aSender, aMessage, false, null, false);
		if (result.success)
		{
			Debug.LogError(result.message, aContext);
		}
	}
	
	/// <summary>
	/// Logs checks condition and log assertion if condition is TRUE.
	/// </summary>
	/// <param name="aCondition"></param>
	/// <param name="aMessage"></param>
	/// <param name="aBreak"></param>
	public static void Assert(bool aCondition, string aMessage, object aSender = null, Object aContext = null, bool aBreak = true)
	{
		if (aCondition)
		{
			if (!enabled)
			{
				return;
			}

			if (aSender != null)
			{
				_stringBuilder.Append("<b>")
					.Append(aSender)
					.Append("</b>")
					.Append(" ► ");
			}

			_stringBuilder.Append(aMessage);
			string finalLog = (stripHtmlTags)
				? _tagRegex.Replace(_stringBuilder.ToString(), "")
				: _stringBuilder.ToString();

			_stringBuilder.Length = 0;
			Debug.Assert(!aCondition, finalLog, aContext);

			if (aBreak)
			{
				Debug.Break();
			}
		}
	}

#endregion

#region Private Methods

	private static LogResult CompileMessage(
		int aImportance, bool aIsEditor, LogKind aLogKind, object aSender, object aMessage,
		bool aIsVerbose, string aHexColor, bool aForce)
	{
		// Logging is disabled.
		if (!enabled)
		{
			return new LogResult { success = false, message = null };
		}

		// Logging is disabled for editor.
		if (!aForce && aIsEditor && editorVerbosity == Verbosity.None)
		{
			return new LogResult { success = false, message = null };
		}

		// Logging is disabled for production.
		if (!aForce && !aIsEditor && verbosity == Verbosity.None)
		{
			return new LogResult { success = false, message = null };
		}

		Verbosity targetVerbosity = (aIsEditor) ? editorVerbosity : verbosity;
		switch (targetVerbosity)
		{
			case Verbosity.Errors :
				if (!aForce && aLogKind != LogKind.Error)
				{
					return new LogResult { success = false, message = null };
				}
				break;

			case Verbosity.ErrorsAndWarnings :
				if (!aForce && aLogKind != LogKind.Error && aLogKind != LogKind.Warning)
				{
					return new LogResult { success = false, message = null };
				}
				break;

			case Verbosity.Normal :
				if (aIsVerbose)
				{
					return new LogResult { success = false, message = null };
				}
				break;
		}

		bool isImportant = (aImportance > -1);
		bool isColored = (aIsVerbose || !string.IsNullOrEmpty(aHexColor));
		string importantLogEnd = "";
		if (isImportant)
		{
			_stringBuilder.Append("<b><color=#f4c560><size=14>★ </size></color></b>");
			switch (aImportance)
			{
				case 1 :
					_stringBuilder.Append("<b>");
					importantLogEnd = "</b>";
					break;

				default :
					_stringBuilder.Append("<size=14><b>");
					importantLogEnd = "</b></size>";
					break;
			}
		}

		if (aIsVerbose)
		{
			_stringBuilder.Append(_verboseColor);
		}
		else if (isColored)
		{
			_stringBuilder.Append("<color=");
			if (aHexColor[0] != '#')
			{
				_stringBuilder.Append('#');
			}
			_stringBuilder.Append(aHexColor).Append(">");
		}

		if (aIsEditor)
		{
			_stringBuilder.Append(_editorPrefix);
		}

		if (aSender != null)
		{
			_stringBuilder.Append((aIsVerbose)
				? _verboseSenderColor 
				: (isImportant)
					? _senderColorImportant
					: _senderColor)
				.Append(aSender)
				.Append("</color> ► ");
		}

		_stringBuilder.Append(aMessage);
		if (isColored)
		{
			_stringBuilder.Append("</color>");
		}

		if (isImportant)
		{
			_stringBuilder.Append(importantLogEnd);
		}

		string finalLog = (stripHtmlTags)
			? _tagRegex.Replace(_stringBuilder.ToString(), "")
			: _stringBuilder.ToString();

		_stringBuilder.Length = 0;
		return new LogResult { success = true, message = finalLog };
	}

#endregion

#region Internal Classes

	public static class Important
	{
		/// <summary>
		/// Logs the important message.
		/// </summary>
		public static void Log(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(1, false, LogKind.Normal, aSender, aMessage, false, aHexColor, false);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the important message. Force message will log if <see cref="verbosity"/> is set to None
		/// but force message won't log if the logger is not enabled.
		/// </summary>
		public static void LogForce(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(1, false, LogKind.Normal, aSender, aMessage, false, aHexColor, true);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the very important message.
		/// </summary>
		public static void High(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(0, false, LogKind.Normal, aSender, aMessage, false, aHexColor, false);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the very important message. Force message will log if <see cref="verbosity"/> is set to None
		/// but force message won't log if the logger is not enabled.
		/// </summary>
		public static void HighForce(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(0, false, LogKind.Normal, aSender, aMessage, false, aHexColor, true);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}
	}

	public static class Editor
	{
		/// <summary>
		/// Logs the given editor-only message.
		/// </summary>
		public static void Log(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(-1, true, LogKind.Normal, aSender, aMessage, false, aHexColor, false);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the given editor-only message with the give options. Force message will log 
		/// if <see cref="verbosity"/> is set to None but force message won't log if the logger is not enabled.
		/// </summary>
		public static void LogForce(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(-1, true, LogKind.Normal, aSender, aMessage, false, aHexColor, true);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the given editor-only message.
		/// </summary>
		public static void Verbose(object aMessage, object aSender = null, Object aContext = null, string aHexColor = null)
		{
			var result = A.CompileMessage(-1, true, LogKind.Normal, aSender, aMessage, true, aHexColor, false);
			if (result.success)
			{
				Debug.Log(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the given editor-only warning message.
		/// </summary>
		public static void Warning(object aMessage, object aSender = null, Object aContext = null)
		{
			var result = A.CompileMessage(-1, true, LogKind.Warning, aSender, aMessage, false, null, false);
			if (result.success)
			{
				Debug.LogWarning(result.message, aContext);
			}
		}

		/// <summary>
		/// Logs the given editor-only error message.
		/// </summary>
		public static void Error(object aMessage, object aSender = null, Object aContext = null)
		{
			var result = A.CompileMessage(-1, true, LogKind.Error, aSender, aMessage, false, null, false);
			if (result.success)
			{
				Debug.LogError(result.message, aContext);
			}
		}
	}
	
#endregion
}
