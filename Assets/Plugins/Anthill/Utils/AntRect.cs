namespace Anthill.Utils
{
	using UnityEngine;
	
	public class AntRect
	{
		public float x;
		public float y;
		public float width;
		public float height;
		public float left;
		public float top;
		public float right;
		public float bottom;

		public AntRect(float aX = 0.0f, float aY = 0.0f, float aWidth = 0.0f, float aHeight = 0.0f)
		{
			x = aX;
			y = aY;
			width = aWidth;
			height = aHeight;
			UpdateBounds();
		}

		public void UpdateBounds()
		{
			left = x;
			right = x + width;
			top = y + height;
			bottom = y;
		}

		public bool IsInside(float aX, float aY)
		{
			return (aX >= left && aX <= right && aY >= bottom && aY <= top);
		}

		public bool IsInside(Vector2 aPoint)
		{
			return (aPoint.x >= left && aPoint.x <= right && aPoint.y >= bottom && aPoint.y <= top);
		}

		public float X
		{
			get => x;
			set
			{
				x = value;
				UpdateBounds();
			}
		}

		public float Y
		{
			get => y;
			set
			{
				y = value;
				UpdateBounds();
			}
		}

		public float Width
		{
			get => width;
			set
			{
				width = value;
				UpdateBounds();
			}
		}

		public float Height
		{
			get => height;
			set
			{
				height = value;
				UpdateBounds();
			}
		}

		public float Left { get => left; }
		public float Top { get => top; }
		public float Right { get => right; }
		public float Bottom { get => bottom; }
	}
}