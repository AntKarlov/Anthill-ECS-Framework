namespace Anthill.Extensions
{
	using UnityEngine;
	
	public static class Vector2Extension
	{
		#region Public Methods

		/// <summary>
		/// Returns a copy of the vector with its X set to the given value.
		/// </summary>
		public static Vector2 SetX(this Vector2 aVector, float aValue)
		{
			aVector.x = aValue;
			return aVector;
		}
		/// <summary>
		/// Returns a copy of the vector with its Y set to the given value.
		/// </summary>
		public static Vector2 SetY(this Vector2 aVector, float aValue)
		{
			aVector.y = aValue;
			return aVector;
		}

		/// <summary>
		/// Returns a copy of the vector with its values rounded to integers, using Mathf.Round.
		/// </summary>
		public static Vector2 Round(this Vector2 aVector)
		{
			return new Vector2(Mathf.Round(aVector.x), Mathf.Round(aVector.y));
		}

		/// <summary>
		/// Returns a copy of the vector with its values rounded to integers, using a fast int cast.
		/// </summary>
		public static Vector2 Floor(this Vector2 aVector)
		{
			return new Vector2((int) aVector.x, (int) aVector.y);
		}

		#endregion
	}
}