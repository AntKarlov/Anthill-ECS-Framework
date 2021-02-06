namespace Anthill.Extensions
{
	using UnityEngine;

	public static class ColorExtension
	{
		/// <summary>
		/// Returns a HEX version of the given Unity Color, without the initial #.
		/// </summary>
		/// <param name="aColor">Value of the color.</param>
		/// <returns>Returns Hex of the color as String.</returns>
		public static string ToHex(this Color32 aColor, bool aIncludeAlpha = false)
		{
			var result = $"{aColor.r.ToString("X2")}{aColor.g.ToString("X2")}{aColor.b.ToString("X2")}";
			if (aIncludeAlpha)
			{
				result = string.Concat(result, aColor.a.ToString("X2"));
			}
			return result;
		}

		/// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #.
        /// </summary>
        /// <param name="aIncludeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(this Color aColor, bool aIncludeAlpha = false)
        {
            return ToHex((Color32) aColor, aIncludeAlpha);
        }

		/// <summary>
		/// Converts value of the Hex into Color.
		/// </summary>
		/// <param name="aHex">Hex code in the string.</param>
		/// <returns>Returns Color value.</returns>
		public static Color32 ToColor(this string aHex)
		{
			if (aHex.IndexOf('#') > -1)
			{
				aHex = aHex.Replace("0x", "");
				aHex = aHex.Replace("#", "");

				byte r = byte.Parse(aHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				byte g = byte.Parse(aHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				byte b = byte.Parse(aHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				byte a = 255;

				if (aHex.Length == 8)
				{
					a = byte.Parse(aHex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
				}

				return new Color32(r, g, b, a);
			}
			else
			{
				var v = new Color();
				string[] arr = aHex.Split(' ');
				if (arr.Length >= 4)
				{
					float f;
					v.r = (float.TryParse(arr[0], out f)) ? f : 1.0f;
					v.g = (float.TryParse(arr[1], out f)) ? f : 1.0f;
					v.b = (float.TryParse(arr[2], out f)) ? f : 1.0f;
					v.a = (float.TryParse(arr[3], out f)) ? f : 1.0f;
				}
				return v;
			}
		}

		/// <summary>
		/// Sets color from 0-255 range values.
		/// </summary>
		/// <param name="aColor">Original color.</param>
		/// <param name="aRed">Red value in range 0-255.</param>
		/// <param name="aGreen">Green value in range 0-255.</param>
		/// <param name="aBlue">Blue value in range 0-255.</param>
		/// <param name="aAlpha">Alpha value in range 0-255.</param>
		public static void Set(this Color aColor, float aRed, float aGreen, float aBlue, float aAlpha = 255.0f)
		{
			aColor = new Color(aRed / 255.0f, aGreen / 255.0f, aBlue / 255.0f, aAlpha / 255.0f);
		}

		/// <summary>
		/// Returns a new color equal to the given one with changed brightness.
		/// </summary>
		/// <param name="aColor">Original color.</param>
		/// <param name="aBrightnessFactor">Brightness factor (multiplied by current brightness).</param>
		/// <param name="aAlpha">If set applies this alpha value.</param>
		public static Color ChangeBrightness(this Color aColor, float aBrightnessFactor, float? aAlpha = null)
		{
			float h, s, v;
			Color.RGBToHSV(aColor, out h, out s, out v);
			v *= aBrightnessFactor;
			// v = (v < 0.0f) ? 0.0f : (v > 1.0f) ? 1.0f : v;
			v = Mathf.Clamp01(v);

			Color result = Color.HSVToRGB(h, s, v);
			if (aAlpha != null)
			{
				result.a = (float) aAlpha;
			}
			return result;
		}

		/// <summary>
		/// Returns a new color equal to the given one with changed saturation.
		/// </summary>
		/// <param name="aColor">Original color.</param>
		/// <param name="aSaturationFactor">Saturation factor (multiplied by current brightness).</param>
		/// <param name="aAlpha">If set applies this alpha value.</param>
		public static Color ChangeSaturation(this Color aColor, float aSaturationFactor, float? aAlpha = null)
		{
			float h, s, v;
			Color.RGBToHSV(aColor, out h, out s, out v);
			s *= aSaturationFactor;
			// s = (s < 0.0f) ? 0.0f : (s > 1.0f) ? 1.0f : s;
			s = Mathf.Clamp01(s);

			Color result = Color.HSVToRGB(h, s, v);
			if (aAlpha != null)
			{
				result.a = (float) aAlpha;
			}
			return result;
		}

		/// <summary>
        /// Changes the alpha of this color and returns it.
        /// </summary>
        public static Color SetAlpha(this Color aColor, float aAlpha)
        {
            aColor.a = aAlpha;
            return aColor;
        }
	}
}