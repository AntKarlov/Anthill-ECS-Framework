namespace Anthill.Extensions
{
	using UnityEngine;

	public static class Vector3Extension
	{
		#region Public Methods

		/// <summary>
		/// Rotates the given vector around the given pivot by the given angles
		/// </summary>
		public static Vector3 RotateAroundPivot(this Vector3 aVector, Vector3 aPivot, Vector3 aRotation)
		{
			return aVector.RotateAroundPivot(aPivot, Quaternion.Euler(aRotation));
		}
		/// <summary>
		/// Rotates the given vector around the given pivot by the given angles.
		/// </summary>
		public static Vector3 RotateAroundPivot(this Vector3 aVector, Vector3 aPivot, Quaternion aRotation)
		{
			return aRotation * (aVector - aPivot) + aPivot;
		}

		/// <summary>
		/// Returns a copy of the vector with its X set to the given value.
		/// </summary>
		public static Vector3 SetX(this Vector3 aVector, float aValue)
		{
			aVector.x = aValue;
			return aVector;
		}

		/// <summary>
		/// Returns a copy of the vector with its Y set to the given value.
		/// </summary>
		public static Vector3 SetY(this Vector3 aVector, float aValue)
		{
			aVector.y = aValue;
			return aVector;
		}
		/// <summary>
		/// Returns a copy of the vector with its Z set to the given value.
		/// </summary>
		public static Vector3 SetZ(this Vector3 aVector, float aValue)
		{
			aVector.z = aValue;
			return aVector;
		}

		/// <summary>
        /// Returns the Y angle in degrees of the given vector
		/// (0 to 180, negative if on left, positive if on right), based on world coordinates.
        /// </summary>
        public static float AngleY(this Vector3 aVector)
        {
            return AngleY(Vector3.forward, aVector);
        }

        /// <summary>
        /// Returns the Y angle in degrees between from and to 
		/// (0 to 180, negative if to is on left, positive if to is on right).
        /// </summary>
        public static float AngleY(Vector3 aFrom, Vector3 aTo)
        {
            Vector2 a = new Vector2(aFrom.x, aFrom.z).normalized;
            Vector2 b = new Vector2(aTo.x, aTo.z).normalized;
            float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(a, b), -1.0f, 1.0f)) * 57.29578f;
            float cross = b.x * a.y - b.y * a.x;
            return (cross < 0.0f) ? -angle : angle;
        }

		#endregion
	}
}