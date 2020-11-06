namespace Anthill.Utils
{
	using UnityEngine;
	
	public static class AntMath
	{
		public const float DEGREES = 180.0f / Mathf.PI;
		public const float RADIANS = Mathf.PI / 180.0f;

		// private static Vector3 _mouseHelper = new Vector3(0, 0, 10.0f);

		// public static float ScaleByFOV(float aFOV, float aDistance, float aViewHeight)
		// {
		// 	// focal length is the position where objects are seen at their 
		// 	// natural size on the screen
		// 	// fLength = viewHeight / 2 arctan(radianFOV)
		// 	float fl = aViewHeight / (2.0f * Mathf.Tan(aFOV * DEGREES));
		// 	// scale = fLength / (fLength + distanceFromViewer);
		// 	return 1.0f / (fl / (fl + aDistance));
		// }

		/// <summary>
		/// Возвращает координаты мыши на экране.
		/// </summary>
		// public static Vector3 GetMouseWorldPosition()
		// {
		// 	_mouseHelper.x = Input.mousePosition.x;
		// 	_mouseHelper.y = Input.mousePosition.y;
		// 	return Camera.main.ScreenToWorldPoint(_mouseHelper);
		// }

		/// <summary>
		/// Определяет попадает ли указанный объект в область видимости камеры.
		/// </summary>
		// public static bool IsVisibleFrom(Renderer aRenderer, Camera aCamera)
		// {
		// 	Plane[] planes = GeometryUtility.CalculateFrustumPlanes(aCamera);
		// 	return GeometryUtility.TestPlanesAABB(planes, aRenderer.bounds);
		// }
		 
		/// <summary>
		/// Normalize any angle as -180 to 180 in degress.
		/// </summary>
		/// <param name="aValue">Angle in the degrees.</param>
		/// <returns>Normalized angle from -180 to 180 deg.</returns>
		public static float Angle(float aValue)
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
		public static float AngleDifference(float aA, float aB)
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
		public static bool AngleArea(float aAngleDeg, float aTarget, float aArea)
		{
			return Mathf.Abs(AngleDifference(aAngleDeg, aTarget) * DEGREES) <= aArea;
		}

		/// <summary>
		/// Inverse angle.
		/// </summary>
		/// <param name="aValue">Angle in degrees.</param>
		/// <returns>Returns inversed angle.</returns>
		public static float InvertAngleDeg(float aValue)
		{
			return Angle(aValue) + 180.0f;
		}
		
		/// <summary>
		/// Converts a negative angle value to a positive one.
		/// </summary>
		/// <param name="aValue">Any angle in degrees.</param>
		/// <returns>Positive angle in degrees.</returns>
		public static float AbsAngleDeg(float aValue)
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
		public static float LerpAngleDeg(float aLower, float aUpper, float aCoef)
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
			return Angle(aLower);
		}

		/// <summary>
		/// Limits the value of angle.
		/// </summary>
		/// <param name="aAngle">Angle in degrees.</param>
		/// <param name="aLimit">Limit angle.</param>
		/// <returns>New value of the angle if angle outside the limit.</returns>
		public static float LimitAngle(float aAngle, float aLimit)
		{
			if (aAngle > aLimit)
			{
				aAngle = aLimit;
			}
			else if (aAngle < -aLimit)
			{
				aAngle = -aLimit;
			}
			return aAngle;
		}

		/// <summary>
		/// Calcs angle between two points.
		/// </summary>
		/// <param name="aA">First point.</param>
		/// <param name="aB">Second point.</param>
		/// <returns>Angle between points in radians.</returns>
		public static float AngleRad(Vector2 aA, Vector2 aB)
		{
			return Mathf.Atan2(aB.y - aA.y, aB.x - aA.x);
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aX">X pos.</param>
		/// <param name="aY">Y pos.</param>
		/// <returns>Angle in radians.</returns>
		public static float AngleRad(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX);
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aPoint">Target point.</param>
		/// <returns>Angle in radians.</returns>
		public static float AngleRad(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.y, aPoint.x);
		}

		/// <summary>
		/// Calcs angle between two points.
		/// </summary>
		/// <param name="aA">First point.</param>
		/// <param name="aB">Second point.</param>
		/// <returns>Angle between points in degrees.</returns>
		public static float AngleDeg(Vector2 aA, Vector2 aB)
		{
			return Mathf.Atan2(aB.y - aA.y, aB.x - aA.x) * DEGREES;
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aX">X pos.</param>
		/// <param name="aY">Y pos.</param>
		/// <returns>Angle in degrees.</returns>
		public static float AngleDeg(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX) * DEGREES;
		}

		/// <summary>
		/// Calcs angle to point from zero point.
		/// </summary>
		/// <param name="aPoint">Target point.</param>
		/// <returns>Angle in degrees.</returns>
		public static float AngleDeg(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.y, aPoint.x) * DEGREES;
		}

		/// <summary>
		/// Normalizes angle in the degrees.
		/// </summary>
		/// <param name="aAngle">Angle in degrees.</param>
		/// <returns>Normalized angle in degrees.</returns>
		public static float NormalizeAngleDeg(float aAngle)
		{
			if (aAngle < 0.0f)
			{
				aAngle = 360.0f + aAngle;
			}
			else if (aAngle >= 360.0f)
			{
				aAngle = aAngle - 360.0f;
			}
			return aAngle;
		}

		/// <summary>
		/// Normalizes angle in the radians.
		/// </summary>
		/// <param name="aAngle">Angle in radians.</param>
		/// <returns>Normalized angle in radians.</returns>
		public static float NormalizeAngleRad(float aAngle)
		{
			if (aAngle < 0.0f)
			{
				aAngle = Mathf.PI * 2.0f + aAngle;
			}
			else if (aAngle >= Mathf.PI * 2.0f)
			{
				aAngle = aAngle - Mathf.PI * 2.0f;
			}
			return aAngle;
		}

		/// <summary>
		/// Rotates the point around pivot.
		/// </summary>
		/// <param name="aPoint">Rotating point.</param>
		/// <param name="aPivot">Pivot point.</param>
		/// <param name="aAngle">Rotation angle in degrees.</param>
		/// <returns>Point poisition after rotation.</returns>
		public static Vector2 RotatePointDeg(Vector2 aPoint, Vector2 aPivot, float aAngle)
		{
			aAngle = -aAngle * RADIANS;
			float dx = aPoint.x - aPivot.x;
			float dy = aPivot.y - aPoint.y;
			return new Vector2(
				aPivot.x + Mathf.Cos(aAngle) * dx - Mathf.Sin(aAngle) * dy, 
				aPivot.y - (Mathf.Sin(aAngle) * dx + Mathf.Cos(aAngle) * dy)
			);
		}

		/// <summary>
		/// Rotates the point around pivot.
		/// </summary>
		/// <param name="aPoint">Rotating point.</param>
		/// <param name="aPivot">Pivot point.</param>
		/// <param name="aAngle">Rotation angle in radians.</param>
		/// <returns>Point poisition after rotation.</returns>
		public static Vector2 RotatePointRad(Vector2 aPoint, Vector2 aPivot, float aAngle)
		{
			aAngle = -aAngle;
			float dx = aPoint.x - aPivot.x;
			float dy = aPivot.y - aPoint.y;
			return new Vector2(
				aPivot.x + Mathf.Cos(aAngle) * dx - Mathf.Sin(aAngle) * dy, 
				aPivot.y - (Mathf.Sin(aAngle) * dx + Mathf.Cos(aAngle) * dy)
			);
		}

		/// <summary>
		/// Checks if the value in the specified range.
		/// </summary>
		/// <param name="aValue">Value.</param>
		/// <param name="aLower">Lower range.</param>
		/// <param name="aUpper">Upper range.</param>
		/// <returns>True if value between lower and upper range.</returns>
		public static bool InRange(float aValue, float aLower, float aUpper)
		{
			return (aValue > aLower && aValue < aUpper);
		}

		/// <summary>
		/// Returns closest value from specified range.
		/// </summary>
		/// <param name="aValue">Value.</param>
		/// <param name="aOut1">First checking value.</param>
		/// <param name="aOut2">Second checking value.</param>
		/// <returns>Returns closest first or second value.</returns>
		public static float Closest(float aValue, float aOut1, float aOut2)
		{
			return (Mathf.Abs(aValue - aOut1) < Mathf.Abs(aValue - aOut2)) ? aOut1 : aOut2;
		}

		/// <summary>
		/// Limits value by range.
		/// </summary>
		/// <param name="aValue">Value.</param>
		/// <param name="aLimit">Limit.</param>
		/// <returns>If value outside the limit then will limited to negative or positive limit value.</returns>
		public static float Limit(float aValue, float aLimit)
		{
			if (aValue > aLimit)
			{
				aValue = aLimit;
			}
			else if (aValue < -aLimit)
			{
				aValue = -aLimit;
			}
			return aValue;
		}

		/// <summary>
		/// Returns random rounded value from the range.
		/// </summary>
		/// <param name="aLower">Lower value.</param>
		/// <param name="aUpper">Upper value.</param>
		/// <returns>Random value from range.</returns>
		public static int RandomRangeInt(int aLower, int aUpper)
		{
			return Mathf.RoundToInt(Random.value * (aUpper - aLower)) + aLower;
		}

		/// <summary>
		/// Returns random value from the range.
		/// </summary>
		/// <param name="aLower">Lower value.</param>
		/// <param name="aUpper">Upper value.</param>
		/// <returns>Random value from range.</returns>
		public static float RandomRangeFloat(float aLower, float aUpper)
		{
			return Random.value * (aUpper - aLower) + aLower;
		}

		/// <summary>
		/// Returns random value from the range.
		/// </summary>
		/// <param name="aValue">Vector where is X is lower and Y is upper value.</param>
		/// <returns>Random value from range.</returns>
		public static float RandomRangeFloat(Vector2 aValue)
		{
			return RandomRangeFloat(aValue.x, aValue.y);
		}

		/// <summary>
		/// Compares the specified values with a given difference error.
		/// </summary>
		/// <param name="aA">First value.</param>
		/// <param name="aB">Second value.</param>
		/// <param name="aDiff">Difference error.</param>
		/// <returns>Returns true if values is equals.</returns>
		public static bool Equal(float aA, float aB, float aDiff = 0.00001f)
		{
			return (Mathf.Abs(aA - aB) <= aDiff);
		}

		/// <summary>
		/// Compares the specified Vectors with a given difference error.
		/// </summary>
		/// <param name="aA">First vector.</param>
		/// <param name="aB">Second vector.</param>
		/// <param name="aDiff">Difference error.</param>
		/// <returns>Returns true if vectors is equals.</returns>
		public static bool Equal(Vector2 aA, Vector2 aB, float aDiff = 0.00001f)
		{
			return (Equal(aA.x, aB.x, aDiff) && Equal(aA.y, aB.y, aDiff));
		}

		public static bool Equal(Vector3 aA, Vector3 aB, float aDiff = 0.00001f)
		{
			return (Equal(aA.x, aB.x, aDiff) && Equal(aA.y, aB.y, aDiff) && Equal(aA.z, aB.z, aDiff));
		}

		/// <summary>
		/// Converts a value from one range to another.
		/// </summary>
		/// <param name="aValue">Value.</param>
		/// <param name="aLower1">Lower value of the first range.</param>
		/// <param name="aUpper1">Upper value of the first range.</param>
		/// <param name="aLower2">Lower value of the second range.</param>
		/// <param name="aUpper2">Upper value of the second range.</param>
		/// <returns>Value in the second range.</returns>
		// public static float Remap(float aValue, float aLower1, float aUpper1, float aLower2, float aUpper2)
		// {
		// 	return aLower2 + (aUpper2 - aLower2) * (aValue - aLower1) / (aUpper1 - aLower1);
		// }

		public static float Remap(float aValue, float aLower1, float aUpper1, float aLower2, float aUpper2)
		{
			float t = InvLerp(aLower1, aUpper1, aValue);
			return Lerp(aLower2, aUpper2, t);
		}

		/// <summary>
		///	Ограничивает значение заданным диапазоном.
		/// </summary>
		/// <param name="aValue">Значение.</param>
		/// <param name="aLower">Наименьшее значение диапазона.</param>
		/// <param name="aUpper">Наибольшее значение диапазона.</param>
		/// <returns>Если значение выходит за рамки диапазона, то вернется граница диапазона.</returns>
		
		/// <summary>
		/// Limits value by specified range.
		/// </summary>
		/// <param name="aValue">Value.</param>
		/// <param name="aLower">Lower value of range.</param>
		/// <param name="aUpper">Upper value of range.</param>
		/// <returns>If value out of range, then value will be changed.</returns>
		public static float TrimToRange(float aValue, float aLower, float aUpper)
		{
			return (aValue > aUpper) ? aUpper : (aValue < aLower) ? aLower : aValue;
		}

		/// <summary>
		/// Change the value with a given coefficient.
		/// </summary>
		/// <param name="aLower">Current value.</param>
		/// <param name="aUpper">Target value.</param>
		/// <param name="aCoef">Change coefficient.</param>
		/// <returns>The new value taking into account the coefficient.</returns>
		// public static float Lerp(float aLower, float aUpper, float aCoef)
		// {
		// 	return aLower + ((aUpper - aLower) * aCoef);
		// }

		public static float Lerp(float aLower, float aUpper, float aValue)
		{
			return (1.0f - aValue) * aLower + aUpper * aValue;
		}

		public static float ClampedLerp(float aLower, float aUpper, float aValue)
		{
			return Lerp(aLower, aUpper, Mathf.Clamp01(aValue));
		}

		public static float InvLerp(float aLower, float aUpper, float aValue)
		{
			return (aValue - aLower) / (aUpper - aLower);
		}

		/// <summary>
		/// Change the Vector with a given coefficient.
		/// </summary>
		/// <param name="aLower">Current vector.</param>
		/// <param name="aUpper">Target vector.</param>
		/// <param name="aCoef">Change coefficient.</param>
		/// <returns>The new vector taking into account the coefficient.</returns>
		public static Vector2 Lerp(Vector2 aLower, Vector2 aUpper, float aCoef)
		{
			aLower.x = Lerp(aLower.x, aUpper.x, aCoef);
			aLower.y = Lerp(aLower.y, aUpper.y, aCoef);
			return aLower;
		}

		/// <summary>
		/// Calcs distance between zero point and target point.
		/// </summary>
		/// <param name="aX">X of target point.</param>
		/// <param name="aY">Y of target point.</param>
		/// <returns>Distance between zero point and target point.</returns>
		public static float Distance(float aX, float aY)
		{
			return Mathf.Sqrt(aX * aX + aY * aY);
		}

		/// <summary>
		/// Calcs distance between zero point and target point.
		/// </summary>
		/// <param name="aA">Target point.</param>
		/// <returns>Distance between zero point and target point.</returns>
		public static float Distance(Vector2 aA)
		{
			return Mathf.Sqrt(aA.x * aA.x + aA.y * aA.y);
		}

		/// <summary>
		/// Calcs distance between two points.
		/// </summary>
		/// <param name="aA">First point.</param>
		/// <param name="aB">Second point.</param>
		/// <returns>Distance between points.</returns>
		public static float Distance(Vector2 aA, Vector2 aB)
		{
			float dx = aB.x - aA.x;
			float dy = aB.y - aA.y;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}

		/// <summary>
		/// Defines the largest number of two given.
		/// </summary>
		/// <param name="aA">First value.</param>
		/// <param name="aB">Second value.</param>
		/// <returns>Largest value.</returns>
		public static float Max(float aA, float aB)
		{
			return (aA > aB) ? aA : aB;
		}
		
		/// <summary>
		/// Defines the largest number of two given.
		/// </summary>
		/// <param name="aA">First value.</param>
		/// <param name="aB">Second value.</param>
		/// <returns>Largest value.</returns>
		public static int Max(int aA, int aB)
		{
			return (aA > aB) ? aA : aB;
		}

		/// <summary>
		/// Defines the smallest number of two given.
		/// </summary>
		/// <param name="aA">First value.</param>
		/// <param name="aB">Second value.</param>
		/// <returns>Smallest value.</returns>
		public static float Min(float aA, float aB)
		{
			return (aA < aB) ? aA : aB;
		}

		/// <summary>
		/// Defines the smallest number of two given.
		/// </summary>
		/// <param name="aA">First value.</param>
		/// <param name="aB">Second value.</param>
		/// <returns>Smallest value.</returns>
		public static int Min(int aA, int aB)
		{
			return (aA < aB) ? aA : aB;
		}
		
		/// <summary>
		/// Returns a unique ID generated by standard Unity tools.
		/// </summary>
		/// <returns>Unique random string.</returns>
		public static string GetUniqueID()
		{
			return System.Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Returns a unique ID string.
		/// </summary>
		/// <returns>Unique random string.</returns>
		public static string UniqueID()
		{
			var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			int currentEpochTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
			int z1 = Random.Range(0, 1000000);
			int z2 = Random.Range(0, 1000000);
			return string.Concat(currentEpochTime.ToString(), "-", z1.ToString(), "-", z2.ToString());
		}

		/// <summary>
		/// Proportional resizing.
		/// </summary>
		/// <param name="aWidth">Current width.</param>
		/// <param name="aHeight">Current height.</param>
		/// <param name="aMaxWidth">Max allowed width.</param>
		/// <param name="aMaxHeight">Max allowed height.</param>
		/// <returns>Vector with resized sizes where is X = width, Y = height.</returns>
		public static Vector2 Resize(float aWidth, float aHeight, float aMaxWidth, float aMaxHeight)
		{
			if (aWidth <= aMaxWidth && aHeight <= aMaxHeight)
			{
				return new Vector2(aWidth, aHeight);
			}

			float p;
			if (aWidth >= aMaxWidth)
			{
				p = aMaxWidth / aWidth;
				return new Vector2(aMaxWidth, aHeight * p);
			}

			p = aMaxHeight / aHeight;
			return new Vector2(aWidth * p, aMaxHeight);
		}
	}
}