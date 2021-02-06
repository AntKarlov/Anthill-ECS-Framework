namespace Anthill.Extensions
{
	using UnityEngine;

	public static class TransformExtension
	{
		#region Public Methods

		public static void SetX(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.position;
			v.x = aValue;
			aTransform.position = v;
		}

		public static void SetY(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.position;
			v.y = aValue;
			aTransform.position = v;
		}

		public static void SetZ(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.position;
			v.z = aValue;
			aTransform.position = v;
		}

		public static void SetLocalX(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localPosition;
			v.x = aValue;
			aTransform.localPosition = v;
		}

		public static void SetLocalY(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localPosition;
			v.y = aValue;
			aTransform.localPosition = v;
		}

		public static void SetLocalZ(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localPosition;
			v.z = aValue;
			aTransform.localPosition = v;
		}

		public static void SetLocalScale(this Transform aTransform, float aValue)
		{
			aTransform.localScale = new Vector3(aValue, aValue, aValue);
		}

		public static void SetLocalScaleX(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localScale;
			v.x = aValue;
			aTransform.localScale = v;
		}
		public static void SetLocalScaleY(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localScale;
			v.y = aValue;
			aTransform.localScale = v;
		}

		public static void SetLocalScaleZ(this Transform aTransform, float aValue)
		{
			Vector3 v = aTransform.localScale;
			v.z = aValue;
			aTransform.localScale = v;
		}

		#endregion
	}
}