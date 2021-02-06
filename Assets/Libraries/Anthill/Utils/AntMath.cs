namespace Anthill.Utils
{
	using UnityEngine;
	
	public static class AntMath
	{
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
		public static float Remap(float aValue, float aLower1, float aUpper1, float aLower2, float aUpper2)
		{
			float t = LerpInv(aLower1, aUpper1, aValue);
			return Lerp(aLower2, aUpper2, t);
		}

		// public static float Remap(float aValue, float aLower1, float aUpper1, float aLower2, float aUpper2)
		// {
		// 	return aLower2 + (aUpper2 - aLower2) * (aValue - aLower1) / (aUpper1 - aLower1);
		// }
		
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
		public static float Lerp(float aLower, float aUpper, float aValue)
		{
			return (1.0f - aValue) * aLower + aUpper * aValue;
		}

		// public static float Lerp(float aLower, float aUpper, float aCoef)
		// {
		// 	return aLower + ((aUpper - aLower) * aCoef);
		// }

		public static float LerpClamped(float aLower, float aUpper, float aValue)
		{
			return Lerp(aLower, aUpper, Mathf.Clamp01(aValue));
		}

		public static float LerpInv(float aLower, float aUpper, float aValue)
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