namespace Anthill.Extensions
{
	using System;
	using UnityEngine;

	public static class GameObjectExtension
	{
		#region Public Methods

		/// <summary>
		/// Activates then immediately deactivates the target gameObject.
		/// Useful when wanting to call Awake before deactivating a gameObject.
		/// </summary>
		public static void AwakeAndDeactivate(this GameObject aGo)
		{
			aGo.SetActive(true);
			aGo.SetActive(false);
		}

		/// <summary>
		/// Returns TRUE if the gameObject is a child of an object with the given Component.
		/// </summary>
		public static bool IsChildOfComponent<T>(this GameObject aGo) where T : Component
		{
			return HasOrIsChildOfComponentInternal<T>(aGo, false);
		}

		/// <summary>
		/// Returns TRUE if the gameObject has or is a child of an object with the given Component.
		/// </summary>
		public static bool HasOrIsChildOfComponent<T>(this GameObject aGo) where T : Component
		{
			return HasOrIsChildOfComponentInternal<T>(aGo, true);
		}

		/// <summary>
		/// Returns TRUE if the gameObject is a child of the given tag.
		/// </summary>
		public static bool IsChildOfTag(this GameObject aGo, string aTag)
		{
			return IsChildOfTagInternal(aGo, aTag, false);
		}

		/// <summary>
		/// Returns TRUE if the gameObject has or is a child of the given tag.
		/// </summary>
		public static bool HasOrIsChildOfTag(this GameObject aGo, string aTag)
		{
			return IsChildOfTagInternal(aGo, aTag, true);
		}

		/// <summary>
		/// Returns ONLY the Components in the children, and ignores the parent.
		/// </summary>
		/// <param name="aIncludeInactive">If TRUE also includes inactive children.</param>
		public static T[] GetOnlyComponentsInChildren<T>(
			this GameObject aGo, bool aIncludeInactive = false) where T : Component
		{
			T[] components = aGo.GetComponentsInChildren<T>(aIncludeInactive);
			int len = components.Length;
			if (len == 0)
			{
				return components;
			}

			T thisT = aGo.GetComponent<T>();
			if (thisT == null)
			{
				return components;
			}

			T lastT = components[components.Length - 1];
			len--;

			Array.Resize(ref components, len);
			
			bool requiresShifting = false;
			for (int i = 0; i < len; ++i)
			{
				T f = components[i];
				if (f == thisT)
				{
					requiresShifting = true;
				}

				if (requiresShifting)
				{
					if (i < len - 1)
					{
						components[i] = components[i + 1];
					}
					else
					{
						components[i] = lastT;
					}
				}
			}
			return components;
		}

		/// <summary>
		/// Returns the Component only if it's in a child, and ignores the parent.
		/// </summary>
		/// <param name="aIncludeInactive">If TRUE also searches inactive children.</param>
		public static T GetOnlyComponentInChildren<T>(
			this GameObject aGo, bool aIncludeInactive = false) where T : Component
		{
			T component = aGo.GetComponentInChildren<T>(aIncludeInactive);
			if (component.transform == aGo.transform)
			{
				T[] components = aGo.GetComponentsInChildren<T>(aIncludeInactive);
				return components.Length <= 1 ? null : components[1];
			}
			return component;
		}

		/// <summary>
		/// Finds the component in the given MonoBehaviour or its parents, with options to 
		/// choose if ignoring inactive objects or not.
		/// </summary>
		/// <param name="includeInactive">If TRUE also searches inactive parents</param>
		public static T GetComponentInParentExtended<T>(
			this GameObject aGo, bool aIncludeInactive = false) where T : Component
		{
			T result = null;
			Transform target = aGo.transform;
			while (target != null && result == null)
			{
				if (aIncludeInactive || target.gameObject.activeInHierarchy)
				{
					result = target.gameObject.GetComponent<T>();
				}
				target = target.parent;
			}
			return result;
		}

		#endregion
		#region Private Methods

		private static bool HasOrIsChildOfComponentInternal<T>(
			GameObject aGo, bool aIncludeSelf = false) where T : Component
		{
			if (aIncludeSelf && aGo.GetComponent<T>() != null)
			{
				return true;
			}

			Transform t = aGo.transform;
			while (t.parent != null)
			{
				t = t.parent;
				if (t.GetComponent<T>() != null)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsChildOfTagInternal(GameObject aGo, string aTag, bool aIncludeSelf = false)
		{
			if (aIncludeSelf && aGo.tag.Equals(aTag))
			{
				return true;
			}

			Transform t = aGo.transform;
			while (t.parent != null)
			{
				t = t.parent;
				if (t.tag.Equals(aTag))
				{
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}