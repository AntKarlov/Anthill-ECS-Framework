namespace Anthill.Extensions
{
	using UnityEngine;

	public static class RectExtension
	{
		#region Public Methods

		/// <summary>
		/// Adds one rect into another, and returns the resulting RectA.
		/// </summary>
		public static Rect Add(this Rect aRectA, Rect aRectB)
		{
			if (aRectB.xMin < aRectA.xMin) aRectA.xMin = aRectB.xMin;
			if (aRectB.xMax > aRectA.xMax) aRectA.xMax = aRectB.xMax;
			if (aRectB.yMin < aRectA.yMin) aRectA.yMin = aRectB.yMin;
			if (aRectB.yMax > aRectA.yMax) aRectA.yMax = aRectB.yMax;
			return aRectA;
		}

		/// <summary>
		/// Returns a copy or the Rect expanded around its center by the given amount
		/// </summary>
		/// <param name="aAmount">Indicates how much to expand the rect on each size.</param>
		public static Rect Expand(this Rect aRect, float aAmount)
		{
			float exSize = aAmount * 2;
			return aRect.Shift(-aAmount, -aAmount, exSize, exSize);
		}

		/// <summary>
		/// Returns a copy or the Rect expanded around its center by the given amount.
		/// </summary>
		/// <param name="aAmountX">Indicates how much to expand the rect on each horizontal side</param>
		/// <param name="aAmountY">Indicates how much to expand the rect on each vertical side</param>
		public static Rect Expand(this Rect aRect, float aAmountX, float aAmountY)
		{
			return aRect.Shift(-aAmountX, -aAmountY, aAmountX * 2, aAmountY * 2);
		}

		/// <summary>
		/// Returns a copy or the Rect contracted around its center by the given amount.
		/// </summary>
		/// <param name="amount">Indicates how much to contract the rect on each size.</param>
		public static Rect Contract(this Rect aRect, float aAmount)
		{
			float exSize = aAmount * 2;
			return aRect.Shift(aAmount, aAmount, -exSize, -exSize);
		}

		/// <summary>
		/// Returns a copy or the Rect contracted around its center by the given amount.
		/// </summary>
		/// <param name="aAmountX">Indicates how much to contract the rect on each horizontal side.</param>
		/// <param name="aAmountY">Indicates how much to contract the rect on each vertical side.</param>
		public static Rect Contract(this Rect aRect, float aAmountX, float aAmountY)
		{
			return aRect.Shift(aAmountX, aAmountY, -aAmountX * 2, -aAmountY * 2);
		}

		/// <summary>
		/// Returns a copy of the Rect resized so it fits proportionally within the given size limits.
		/// </summary>
		/// <param name="aWidth">Width to fit.</param>
		/// <param name="aHeight">Height to fit.</param>
		/// <param name="aShrinkOnly">If TRUE (default) only shrinks the rect if needed, if FALSE also enlarges it to fit</param>
		/// <returns></returns>
		public static Rect Fit(this Rect aRect, float aWidth, float aHeight, bool aShrinkOnly = true)
		{
			if (aShrinkOnly && aRect.width <= aWidth && aRect.height <= aHeight)
			{
				return aRect;
			}

			float wPerc = aWidth / aRect.width;
			float hPerc = aHeight / aRect.height;
			float perc = (wPerc < hPerc) ? wPerc : hPerc;
			aRect.width *= perc;
			aRect.height *= perc;
			return aRect;
		}

		/// <summary>
		/// Returns TRUE if the first rect includes the second one.
		/// </summary>
		/// <param name="aFull">If TRUE, returns TRUE only if the second rect is fully included,
		/// otherwise just if some part of it is included</param>
		public static bool Includes(this Rect aRectA, Rect aRectB, bool aFull = true)
		{
			if (aFull)
			{
				return (aRectA.xMin <= aRectB.xMin && 
					aRectA.xMax > aRectB.xMax && 
					aRectB.yMin <= aRectB.yMin && 
					aRectA.yMax >= aRectB.yMax);
			}

			return (aRectB.xMax > aRectA.xMin && 
				aRectB.xMin < aRectA.xMax && 
				aRectB.yMax > aRectA.yMin && 
				aRectB.yMin < aRectA.yMax);
		}

		/// <summary>
		/// Returns TRUE if this rect intersects the given one, and also outputs the intersection area.
		/// </summary>
		/// <param name="aIntersection">Intersection area.</param>
		public static bool Intersects(this Rect aRectA, Rect aRectB, out Rect aIntersection)
		{
			aIntersection = new Rect();
			if (!aRectA.Overlaps(aRectB))
			{
				return false;
			}

			float minX = (aRectA.x < aRectB.x) ? aRectB.x : aRectA.x;
			float maxX = (aRectA.xMax > aRectB.xMax) ? aRectB.xMax : aRectA.xMax;
			float minY = (aRectA.y < aRectB.y) ? aRectB.y : aRectA.y;
			float maxY = (aRectA.yMax > aRectB.yMax) ? aRectB.yMax : aRectA.yMax;

			aIntersection = new Rect(minX, minY, maxX - minX, maxY - minY);

			// TIP: Fix for Unity floating point imprecision where Overlaps returns TRUE 
			//      in some cases when rects are simply adjacent.
			const float epsilon = 0.001f;
			if (aIntersection.width < epsilon || aIntersection.height < epsilon)
			{
				aIntersection = new Rect();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a copy of the Rect with its X/Y coordinates set to 0.
		/// </summary>
		public static Rect ResetXY(this Rect aRect)
		{
			aRect.x = aRect.y = 0;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its values shifted according the the given parameters.
		/// </summary>
		public static Rect Shift(this Rect aRect, float aX, float aY, float aWidth, float aHeight)
		{
			return new Rect(aRect.x + aX, aRect.y + aY, aRect.width + aWidth, aRect.height + aHeight);
		}

		/// <summary>
		/// Returns a copy of the Rect with its X value shifted by the given value.
		/// </summary>
		public static Rect ShiftX(this Rect aRect, float aX)
		{
			return new Rect(aRect.x + aX, aRect.y, aRect.width, aRect.height);
		}

		/// <summary>
		/// Returns a copy of the Rect with its Y value shifted by the given value.
		/// </summary>
		public static Rect ShiftY(this Rect aRect, float aY)
		{
			return new Rect(aRect.x, aRect.y + aY, aRect.width, aRect.height);
		}

		/// <summary>
		/// Returns a copy of the Rect with its x shifted by the given value and 
		/// its width shrinked/expanded accordingly.
		/// (so that the xMax value will stay the same as before)
		/// </summary>
		public static Rect ShiftXAndResize(this Rect aRect, float aX)
		{
			return new Rect(aRect.x + aX, aRect.y, aRect.width - aX, aRect.height);
		}

		/// <summary>
		/// Returns a copy of the Rect with its y shifted by the given value and 
		/// its height shrinked/expanded accordingly.
		/// (so that the yMax value will stay the same as before)
		/// </summary>
		public static Rect ShiftYAndResize(this Rect aRect, float aY)
		{
			return new Rect(aRect.x, aRect.y + aY, aRect.width, aRect.height - aY);
		}

		/// <summary>
		/// Returns a copy of the Rect with its X property set to the given value.
		/// </summary>
		public static Rect SetX(this Rect aRect, float aValue)
		{
			aRect.x = aValue;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its Y property set to the given value.
		/// </summary>
		public static Rect SetY(this Rect aRect, float aValue)
		{
			aRect.y = aValue;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its height property set to the given value.
		/// </summary>
		public static Rect SetHeight(this Rect aRect, float aValue)
		{
			aRect.height = aValue;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its width property set to the given value.
		/// </summary>
		public static Rect SetWidth(this Rect aRect, float aValue)
		{
			aRect.width = aValue;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its X,Y properties set so the rect center corresponds to the given values.
		/// </summary>
		public static Rect SetCenter(this Rect aRect, float aX, float aY)
		{
			aRect.x = aX - aRect.width * 0.5f;
			aRect.y = aY - aRect.height * 0.5f;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its X property set so the rect X center corresponds to the given value.
		/// </summary>
		public static Rect SetCenterX(this Rect aRect, float aValue)
		{
			aRect.x = aValue - aRect.width * 0.5f;
			return aRect;
		}

		/// <summary>
		/// Returns a copy of the Rect with its Y property set so the rect Y center corresponds to the given value.
		/// </summary>
		public static Rect SetCenterY(this Rect aRect, float aValue)
		{
			aRect.y = aValue - aRect.height * 0.5f;
			return aRect;
		}

		#endregion
	}
}