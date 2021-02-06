namespace Anthill.Extensions
{
	using UnityEngine;

	public static class TextureExtension
	{
		#region Public Methods

		/// <summary>
		/// Returns the full Rect of this texture, with options for position and scale.
		/// </summary>
		public static Rect GetRect(this Texture2D aTexture, float aX = 0.0f, float aY = 0.0f, float aScale = 1.0f)
		{
			return new Rect(aX, aY, aTexture.width * aScale, aTexture.height * aScale);
		}

		#endregion
	}
}