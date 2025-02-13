using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Anthill.Core
{
	public class AntSystemMonitor
	{
		public float xBorder = 48;
		public float yBorder = 20;
		public int rightLinePadding = -15;
		public string labelFormat = "{0:0.0}";
		public string axisFormat = "{0:0.0}";
		public int gridLines = 1;
		public float axisRounding = 10.0f;
		public float anchorRadius = 1.0f;
		public Color lineColor = Color.magenta;

		private readonly GUIStyle _labelTextStyle;
		private readonly GUIStyle _centeredStyle;
		private readonly Vector3[] _cachedLinePointVerticies;
		private readonly Vector3[] _linePoints;

		public AntSystemMonitor(int dataLength)
		{
			_labelTextStyle = new(GUI.skin.label)
			{
				alignment = TextAnchor.UpperRight
			};
			
			_centeredStyle = new()
			{
				alignment = TextAnchor.UpperCenter
			};

			_centeredStyle.normal.textColor = Color.white;
			_linePoints = new Vector3[dataLength];
			_cachedLinePointVerticies = new Vector3[]
			{
				new Vector3(-1, 1, 0) * anchorRadius,
				new Vector3(1, 1, 0) * anchorRadius,
				new Vector3(1, -1, 0) * anchorRadius,
				new Vector3(-1, -1, 0) * anchorRadius,
			};
		}

		public void Draw(float[] data, float height)
		{
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, height);
			float top = rect.y + yBorder;
			float floor = rect.y + rect.height - yBorder;
			float availableHeight = floor - top;
			float max = (data.Length != 0) ? data.Max() : 0.0f;
			if (max % axisRounding != 0)
			{
				max = max + axisRounding - (max % axisRounding);
			}

			DrawGridLines(top, rect.width, availableHeight, max);
			DrawAvg(data, floor, rect.width, availableHeight, max);
			DrawLine(data, floor, rect.width, availableHeight, max);
		}

		private void DrawGridLines(float top, float width, float availableHeight, float max)
		{
			Color c = Handles.color;
			Handles.color = Color.grey;
			int n = gridLines + 1;
			float lineSpacing = availableHeight / n;
			for (int i = 0; i <= n; i++)
			{
				float lineY = top + (lineSpacing * i);
				Handles.DrawLine(
					new Vector2(xBorder, lineY),
					new Vector2(width - rightLinePadding, lineY)
				);

				GUI.Label(
					new Rect(0.0f, lineY - 8.0f, xBorder - 2.0f, 50.0f),
					string.Format(axisFormat, max * (1.0f - (i / n))),
					_labelTextStyle
				);
			}
			Handles.color = c;
		}

		private void DrawAvg(float[] data, float floor, float width, float availableHeight, float max)
		{
			Color c = Handles.color;
			Handles.color = Color.yellow;

			float avg = data.Average();
			float lineY = floor - (availableHeight * (avg / max));
			Handles.DrawLine(
				new Vector2(xBorder, lineY),
				new Vector2(width - rightLinePadding, lineY)
			);
			Handles.color = c;
		}

		private void DrawLine(float[] data, float floor, float width, float availableHeight, float max)
		{
			float lineWidth = (float) (width - xBorder - rightLinePadding) / data.Length;
			Color c = Handles.color;
			var labelRect = new Rect();
			Vector2 newLine;
			bool mousePositionDiscovered = false;
			float mouseHoverDataValue = 0.0f;
			float linePointScale;
			Handles.color = lineColor;
			Handles.matrix = Matrix4x4.identity;
			HandleUtility.handleMaterial.SetPass(0);
			
			for (int i = 0, n = data.Length; i < n; i++)
			{
				float value = data[i];
				float lineTop = floor - (availableHeight * (value / max));
				newLine = new Vector2(xBorder + (lineWidth * i), lineTop);
				_linePoints[i] = new Vector3(newLine.x, newLine.y, 0);
				linePointScale = 1.0f;

				if (!mousePositionDiscovered)
				{
					float anchorPosRadius3 = anchorRadius * 3.0f;
					float anchorPosRadius6 = anchorRadius * 6.0f;
					Vector2 anchorPos = newLine - (Vector2.up * 0.5f);
					labelRect = new Rect(
						anchorPos.x - anchorPosRadius3, 
						anchorPos.y - anchorPosRadius3,
						anchorPosRadius6,
						anchorPosRadius6
					);

					if (labelRect.Contains(Event.current.mousePosition))
					{
						mousePositionDiscovered = true;
						mouseHoverDataValue = value;
						linePointScale = 3.0f;
					}
				}

				Handles.matrix = Matrix4x4.TRS(_linePoints[i], Quaternion.identity, Vector3.one * linePointScale);
				Handles.DrawAAConvexPolygon(_cachedLinePointVerticies);
			}

			Handles.matrix = Matrix4x4.identity;
			Handles.DrawAAPolyLine(2.0f, data.Length, _linePoints);

			if (mousePositionDiscovered)
			{
				labelRect.y -= 16.0f;
				labelRect.width += 50.0f;
				labelRect.x -= 25.0f;
				GUI.Label(labelRect, string.Format(labelFormat, mouseHoverDataValue), _centeredStyle);
			}
			Handles.color = c;
		}
	}
}