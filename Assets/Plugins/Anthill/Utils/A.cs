using System.Text;
using UnityEngine;

/// <summary>
///	Helper for logging messages into console.
///
/// Examples of use.
/// A.Log("Its a number", value);
/// A.Error("<color=red>Fatal Error:</color>: File", fileName, "not found");
/// A.Error("{0}: File `{1}` not found!, A.Colored("Fatal Error", A.Red), fileName);
/// A.Warning("Warning: `{0}` argument not found in `{1}`.", valueName, rootObject);
/// </summary>
public static class A
{
	public enum Color
	{
		Aqua,
		Black,
		Blue,
		Brown,
		Darkblue,
		Fuchsia,
		Green,
		Grey,
		Lightblue,
		Lime,
		Maroon,
		Navy,
		Olive,
		Orange,
		Purple,
		Red,
		Silver,
		Teal,
		White,
		Yellow
	}

	#region Public Methods

	/// <summary>
	/// Outputs simple message to the console.
	/// </summary>
	/// <param name="aArgs">Array of the arguments.</param>
	public static void Log(params object[] aArgs)
	{
		Debug.Log(Message(aArgs));
	}

	/// <summary>
	/// Outputs warning message to the console.
	/// </summary>
	/// <param name="aArgs">Array of the arguments.</param>
	public static void Warning(params object[] aArgs)
	{
		Debug.LogWarning(Message(aArgs));
	}

	/// <summary>
	/// Outputs error message to the console.
	/// </summary>
	/// <param name="aArgs">Array of the arguments.</param>
	public static void Error(params object[] aArgs)
	{
		Debug.LogError(Message(aArgs));
	}

	/// <summary>
	/// Outputs assert message to the console.
	/// </summary>
	/// <param name="aCondition">Condition of the assertion. If true, then method throws error.</param>
	/// <param name="aMessage">Message.</param>
	/// <param name="aArgs">Optional arguments for the message.</param>
	public static void Assert(bool aCondition, string aMessage, params object[] aArgs)
	{
		if (aCondition)
		{
			var args = new object[aArgs.Length + 1];
			args[0] = string.Concat("Assert Failed! ", aMessage);
			for (int i = 0, n = aArgs.Length; i < n; i++)
			{
				args[i + 1] = aArgs[i];
			}
			string str = Message(args);
			Debug.Assert(!aCondition, str);
			Debug.Break();
		}
	}

	public static void Assert(bool aCondition, string aMessage)
	{
		if (aCondition)
		{
			Debug.Assert(!aCondition, aMessage);
			Debug.Break();
		}
	}

	/// <summary>
	/// Adds color tags to the message text.
	/// </summary>
	/// <param name="aText">Text.</param>
	/// <param name="aColor">Color.</param>
	/// <returns>String with colored tags.</returns>
	public static string Colored(string aText, Color aColor)
	{
		return string.Format("<color={0}>{1}</color>", aColor, aText);
	}

	/// <summary>
	/// Adds color tags to the message text.
	/// </summary>
	/// <param name="aText">Text.</param>
	/// <param name="aColor">Color.</param>
	/// <returns>String with colored tags.</returns>
	public static string Colored(string aText, string aColorName)
	{
		return string.Format("<color={0}>{1}</color>", aColorName, aText);
	}

	/// <summary>
	/// Adds size tags to the message.
	/// </summary>
	/// <param name="aText">Text.</param>
	/// <param name="aSize">Size.</param>
	/// <returns>String with size tags.</returns>
	public static string Sized(string aText, int aSize)
	{
		return string.Format("<size={0}>{1}</size>", aSize, aText);
	}

	/// <summary>
	/// Adds bold tags to the message.
	/// </summary>
	/// <param name="aText">Text.</param>
	/// <returns>String with the bold tags.</returns>
	public static string Bold(string aText)
	{
		return string.Format("<b>{0}</b>", aText);
	}

	/// <summary>
	/// Adds italic tags to the message.
	/// </summary>
	/// <param name="aText">Text.</param>
	/// <returns>String with the italic tags.</returns>
	public static string Italic(string aText)
	{
		return string.Format("<i>{0}</i>", aText);
	}

	#endregion
	#region Private Methods
	
	private static string Message(params object[] aArgs)
	{
		string result = null;
		if (aArgs[0] is string && CountOfBrackets((string) aArgs[0]) == aArgs.Length - 1)
		{
			var args = new object[aArgs.Length - 1];
			for (int i = 1, n = aArgs.Length; i < n; i++)
			{
				args[i - 1] = aArgs[i];
			}
			result = string.Format((string) aArgs[0], args);
		}

		if (result == null)
		{
			var sb = new StringBuilder();
			for (int i = 0, n = aArgs.Length; i < n; i++)
			{
				if (aArgs[i] != null)
				{
					sb.Append(aArgs[i].ToString());
				}
				else
				{
					sb.Append("Null");
				}
				sb.Append(" ");
			}
			result = sb.ToString();
		}

		return result;
	}

	private static int CountOfBrackets(string aStr)
	{
		int count = 0;
		bool opened = false;
		for (int i = 0, n = aStr.Length; i < n; i++)
		{
			switch (aStr[i])
			{
				case '{' : 
					opened = true;
					break;

				case '}' :
					if (opened)
					{
						count++;
						opened = false;
					}
					break;
			}
		}

		return count;
	}
	
	#endregion
}
