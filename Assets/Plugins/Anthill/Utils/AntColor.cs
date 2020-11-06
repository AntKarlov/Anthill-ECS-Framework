namespace Anthill.Utils
{
	using UnityEngine;

	public static class AntColor
	{
		/// <summary>
		/// Преобразует значение Color в Hex формат.
		/// </summary>
		/// <param name="aColor">Color значение.</param>
		/// <returns>Возвращает Hex код цвета в формате string.</returns>
		public static string ColorToHex(Color32 aColor)
		{
			return string.Format("#{0}{1}{2}{3}", 
				aColor.r.ToString("X2"),
				aColor.g.ToString("X2"),
				aColor.b.ToString("X2"),
				aColor.a.ToString("X2"));
		}

		/// <summary>
		/// Преобразует значение Hex в Color формат.
		/// </summary>
		/// <param name="aHex">Hex код цвета в формате string.</param>
		/// <returns>Возвращает Color значение.</returns>
		public static Color32 HexToColor(string aHex)
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

		public static Color NewColor(float aRed, float aGreen, float aBlue, float aAlpha = 255.0f)
		{
			return new Color(aRed / 255.0f, aGreen / 255.0f, aBlue / 255.0f, aAlpha / 255.0f);
		}
	}
}