namespace Anthill.Utils
{
	using UnityEngine;

	public static class AntAngle
	{
		public const float RadToDeg = 180.0f / Mathf.PI;
		public const float DegToRad = Mathf.PI / 180.0f;
		
		/// <summary>
		/// Normalize any angle as -180 to 180 in degress.
		/// </summary>
		/// <param name="aValue">Angle in the degrees.</param>
		/// <returns>Normalized angle from -180 to 180 deg.</returns>
		public static float Normalize(float aValue)
		{
			const float ang = 180.0f;
			bool inv = aValue < 0.0f;
			
			aValue = (inv ? -aValue : aValue) % 360;
			
			if (aValue > ang)			
			{
				aValue = -ang + (aValue - ang);
			}
			
			return (inv ? -aValue : aValue);			
		}

		/// <summary>
		/// Calcs difference between two angles.
		/// </summary>
		/// <param name="aA">First angle.</param>
		/// <param name="aB">Second angle.</param>
		/// <returns>Difference in the radians.</returns>
		public static float Difference(float aA, float aB)
		{
			return Mathf.Atan2(Mathf.Sin(aA - aB), Mathf.Cos(aA - aB));
		}
		
		/// <summary>
		/// Determines whether the angle falls into the specified area.
		/// </summary>
		/// <param name="aAngleDeg">Angle to check in degrees.</param>
		/// <param name="aTarget">Rotation angle of the area in degrees.</param>
		/// <param name="aArea">Area size to both sides from target angle.</param>
		/// <returns>True if angle in in the area.</returns>
		public static bool Area(float aAngleDeg, float aTarget, float aArea)
		{
			return Mathf.Abs(Difference(aAngleDeg, aTarget) * RadToDeg) <= aArea;
		}

		/// <summary>
		/// Inverse angle.
		/// </summary>
		/// <param name="aValue">Angle in degrees.</param>
		/// <returns>Returns inversed angle.</returns>
		public static float InvertDeg(float aValue)
		{
			return Normalize(aValue) + 180.0f;
		}
		
		/// <summary>
		/// Converts a negative angle value to a positive one.
		/// </summary>
		/// <param name="aValue">Any angle in degrees.</param>
		/// <returns>Positive angle in degrees.</returns>
		public static float AbsDeg(float aValue)
		{
			return (aValue < 0.0f) ? 180.0f + (180.0f + aValue) : aValue;
		}

		/// <summary>
		/// Change the value of the angle with a given coefficient.
		/// </summary>
		/// <param name="aLower">Current angle in degrees.</param>
		/// <param name="aUpper">Target angle in degrees.</param>
		/// <param name="aCoef">Change coefficient.</param>
		/// <returns>The new angle value taking into account the coefficient.</returns>
		public static float LerpDeg(float aLower, float aUpper, float aCoef)
		{
			if (Mathf.Abs(aLower - aUpper) > 180.0f)
			{
				if (aLower > aUpper)
				{
					aUpper += 360.0f;
				}
				else
				{
					aUpper -= 360.0f;
				}
			}

			aLower += (aUpper - aLower) * aCoef;
			return Normalize(aLower);
		}

		/// <summary>
		/// Limits the value of angle.
		/// </summary>
		/// <param name="aDegrees">Angle in degrees.</param>
		/// <param name="aLimit">Limit angle.</param>
		/// <returns>New value of the angle if angle outside the limit.</returns>
		public static float LimitDeg(float aDegrees, float aLimit)
		{
			if (aDegrees > aLimit)
			{
				aDegrees = aLimit;
			}
			else if (aDegrees < -aLimit)
			{
				aDegrees = -aLimit;
			}
			
			return aDegrees;
		}

		/// <summary>
		/// Calcs angle between two points.
		/// </summary>
		/// <param name="aA">First point.</param>
		/// <param name="aB">Second point.</param>
		/// <returns>Angle between points in radians.</returns>
		public static float BetweenRad(Vector2 aA, Vector2 aB)
		{
			return Mathf.Atan2(aB.y - aA.y, aB.x - aA.x);
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aX">X pos.</param>
		/// <param name="aY">Y pos.</param>
		/// <returns>Angle in radians.</returns>
		public static float BetweenRad(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX);
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aPoint">Target point.</param>
		/// <returns>Angle in radians.</returns>
		public static float BetweenRad(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.y, aPoint.x);
		}

		/// <summary>
		/// Calcs angle between two points.
		/// </summary>
		/// <param name="aA">First point.</param>
		/// <param name="aB">Second point.</param>
		/// <returns>Angle between points in degrees.</returns>
		public static float BetweenDeg(Vector2 aA, Vector2 aB)
		{
			return Mathf.Atan2(aB.y - aA.y, aB.x - aA.x) * RadToDeg;
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aX">X pos.</param>
		/// <param name="aY">Y pos.</param>
		/// <returns>Angle in degrees.</returns>
		public static float BetweenDeg(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX) * RadToDeg;
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aPoint">Target point.</param>
		/// <returns>Angle in degrees.</returns>
		public static float BetweenDeg(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.y, aPoint.x) * RadToDeg;
		}

		/// <summary>
		/// Normalizes angle in the degrees.
		/// </summary>
		/// <param name="aDegrees">Angle in degrees.</param>
		/// <returns>Normalized angle in degrees.</returns>
		// public static float NormalizeDeg(float aDegrees)
		// {
		// 	if (aDegrees < 0.0f)
		// 	{
		// 		aDegrees = 360.0f + aDegrees;
		// 	}
		// 	else if (aDegrees >= 360.0f)
		// 	{
		// 		aDegrees = aDegrees - 360.0f;
		// 	}

		// 	return aDegrees;
		// }

		/// <summary>
		/// Normalizes angle in the radians.
		/// </summary>
		/// <param name="aRadians">Angle in radians.</param>
		/// <returns>Normalized angle in radians.</returns>
		// public static float NormalizeRad(float aRadians)
		// {
		// 	if (aRadians < 0.0f)
		// 	{
		// 		aRadians = Mathf.PI * 2.0f + aRadians;
		// 	}
		// 	else if (aRadians >= Mathf.PI * 2.0f)
		// 	{
		// 		aRadians = aRadians - Mathf.PI * 2.0f;
		// 	}

		// 	return aRadians;
		// }

		/// <summary>
		/// Rotates the point around pivot.
		/// </summary>
		/// <param name="aPoint">Rotating point.</param>
		/// <param name="aPivot">Pivot point.</param>
		/// <param name="aAngle">Rotation angle in degrees.</param>
		/// <returns>Point poisition after rotation.</returns>
		public static Vector2 RotateAroundDeg(Vector2 aPoint, Vector2 aPivot, float aDegrees)
		{
			float angle = -aDegrees * DegToRad;
			float dx = aPoint.x - aPivot.x;
			float dy = aPivot.y - aPoint.y;
			return new Vector2(
				aPivot.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy, 
				aPivot.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy)
			);
		}

		/// <summary>
		/// Rotates the point around pivot.
		/// </summary>
		/// <param name="aPoint">Rotating point.</param>
		/// <param name="aPivot">Pivot point.</param>
		/// <param name="aAngle">Rotation angle in radians.</param>
		/// <returns>Point poisition after rotation.</returns>
		public static Vector2 RotateAroundRad(Vector2 aPoint, Vector2 aPivot, float aRadians)
		{
			float angle = -aRadians;
			float dx = aPoint.x - aPivot.x;
			float dy = aPivot.y - aPoint.y;
			return new Vector2(
				aPivot.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy, 
				aPivot.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy)
			);
		}
	}
}