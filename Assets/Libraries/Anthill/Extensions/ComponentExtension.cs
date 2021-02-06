namespace Anthill.Extensions
{
	using System;
	using System.Reflection;
	using UnityEngine;
	using Object = UnityEngine.Object;

	public static class ComponentExtension
	{
		public enum CopyMode
		{
			Add,         // Adds the component even if another component of the same type already exists on the target.
			AddIfMissing // Adds the component only if the same component doesn't already exist on the target.
		}

		/// <summary>
		/// Copies a component with all its public and private dynamic fields, and adds it to the given target.
		/// </summary>
		/// <param name="aOriginal">Component to copy</param>
		/// <param name="aTo">GameObject to add the component to</param>
		/// <param name="aCopyMode">Copy mode</param>
		/// <param name="aRemoveOriginalComponent">If TRUE, removes the original components after copying it.</param>
		public static T CopyTo<T>(
			this T aOriginal, GameObject aTo, CopyMode aCopyMode = CopyMode.AddIfMissing, 
			bool aRemoveOriginalComponent = false) where T : Component
		{
			Type type = aOriginal.GetType();
			Component copy = (aCopyMode == CopyMode.AddIfMissing) 
				? aTo.GetComponent<T>() 
				: null;
			
			if (copy == null)
			{
				copy = aTo.AddComponent(type);
			}

			FieldInfo[] fInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo fi in fInfos)
			{
				fi.SetValue(copy, fi.GetValue(aOriginal));
			}

			if (aRemoveOriginalComponent)
			{
				Object.Destroy(aOriginal);
			}

			return (T) copy;
		}

		/// <summary>
		/// Returns ONLY the Components in the children, and ignores the parent.
		/// </summary>
		/// <param name="aIncludeInactive">If TRUE also includes inactive children.</param>
		public static T[] GetOnlyComponentsInChildren<T>(
			this MonoBehaviour aMonoBeh, bool aIncludeInactive = false) where T : Component
		{
			return aMonoBeh.gameObject.GetOnlyComponentsInChildren<T>(aIncludeInactive);
		}

		/// <summary>
		/// Returns the Component only if it's in a child, and ignores the parent.
		/// </summary>
		/// <param name="aIncludeInactive">If TRUE also searches inactive children</param>
		public static T GetOnlyComponentInChildren<T>(
			this MonoBehaviour aMonoBeh, bool aIncludeInactive = false) where T : Component
		{
			return aMonoBeh.gameObject.GetOnlyComponentInChildren<T>(aIncludeInactive);
		}

		/// <summary>
		/// Finds the component in the given MonoBehaviour or its parents, with options to choose 
		/// if ignoring inactive objects or not.
		/// </summary>
		/// <param name="aIncludeInactive">If TRUE also searches inactive parents.</param>
		public static T GetComponentInParentExtended<T>(
			this MonoBehaviour aMonoBeh, bool aIncludeInactive = false) where T : Component
		{
			return aMonoBeh.gameObject.GetComponentInParentExtended<T>(aIncludeInactive);
		}
	}
}