using System;
using UnityEngine;

namespace RENDER
{
	// Token: 0x0200000E RID: 14
	public static class Render
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003340 File Offset: 0x00001540
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00003357 File Offset: 0x00001557
		public static Color Color
		{
			get
			{
				return GUI.color;
			}
			set
			{
				GUI.color = value;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003361 File Offset: 0x00001561
		public static void Line(Vector2 from, Vector2 to, float thickness, Color color)
		{
			Render.Color = color;
			Render.Line(from, to, thickness);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003374 File Offset: 0x00001574
		public static void Line(Vector2 from, Vector2 to, float thickness)
		{
			Vector2 normalized = (to - from).normalized;
			float num = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			GUIUtility.RotateAroundPivot(num, from);
			Render.Box(from, Vector2.right * (from - to).magnitude, thickness, false);
			GUIUtility.RotateAroundPivot(-num, from);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000033DD File Offset: 0x000015DD
		public static void Box(Vector2 position, Vector2 size, float thickness, Color color, bool centered = true)
		{
			Render.Color = color;
			Render.Box(position, size, thickness, centered);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000033F4 File Offset: 0x000015F4
		public static void Box(Vector2 position, Vector2 size, float thickness, bool centered = true)
		{
			if (centered)
			{
				position -= size / 2f;
			}
			GUI.DrawTexture(new Rect(position.x, position.y, size.x, thickness), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(position.x, position.y, thickness, size.y), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(position.x + size.x, position.y, thickness, size.y), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(position.x, position.y + size.y, size.x + thickness, thickness), Texture2D.whiteTexture);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000034B8 File Offset: 0x000016B8
		public static void Cross(Vector2 position, Vector2 size, float thickness, Color color)
		{
			Render.Color = color;
			Render.Cross(position, size, thickness);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000034CC File Offset: 0x000016CC
		public static void Cross(Vector2 position, Vector2 size, float thickness)
		{
			GUI.DrawTexture(new Rect(position.x - size.x / 2f, position.y, size.x, thickness), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(position.x, position.y - size.y / 2f, thickness, size.y), Texture2D.whiteTexture);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000353A File Offset: 0x0000173A
		public static void Dot(Vector2 position, Color color)
		{
			Render.Color = color;
			Render.Dot(position);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000354B File Offset: 0x0000174B
		public static void Dot(Vector2 position)
		{
			Render.Box(position - Vector2.one, Vector2.one * 2f, 1f, true);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003574 File Offset: 0x00001774
		public static void String(GUIStyle Style, float X, float Y, float W, float H, string str, Color col, bool centerx = false, bool centery = false)
		{
			GUIContent content = new GUIContent(str);
			Vector2 vector = Style.CalcSize(content);
			object obj = centerx ? (X - vector.x / 2f) : X;
			float num = centery ? (Y - vector.y / 2f) : Y;
			Style.normal.textColor = Color.black;
			object obj2 = obj;
			GUI.Label(new Rect((float)obj2, num, vector.x, H), str, Style);
			Style.normal.textColor = col;
			GUI.Label(new Rect((float)obj2 + 1f, num + 1f, vector.x, H), str, Style);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003630 File Offset: 0x00001830
		public static void Circle(Vector2 center, float radius, float thickness, Color color)
		{
			Render.Color = color;
			Vector2 from = center + new Vector2(radius, 0f);
			for (int i = 1; i <= 360; i++)
			{
				float f = (float)i * 0.017453292f;
				Vector2 vector = center + new Vector2(radius * Mathf.Cos(f), radius * Mathf.Sin(f));
				Render.Line(from, vector, thickness);
				from = vector;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000036A4 File Offset: 0x000018A4
		public static void FilledCircle(Vector2 center, float radius, Color color)
		{
			Render.Color = color;
			float num = radius * radius;
			for (float num2 = -radius; num2 <= radius; num2 += 1f)
			{
				for (float num3 = -radius; num3 <= radius; num3 += 1f)
				{
					bool flag = num3 * num3 + num2 * num2 <= num;
					if (flag)
					{
						Render.Line(center + new Vector2(num3, num2), center + new Vector2(num3 + 1f, num2), 1f);
					}
				}
			}
		}

		// Token: 0x02000028 RID: 40
		private class RingArray
		{
			// Token: 0x17000010 RID: 16
			// (get) Token: 0x060000B8 RID: 184 RVA: 0x000091C4 File Offset: 0x000073C4
			// (set) Token: 0x060000B9 RID: 185 RVA: 0x000091CC File Offset: 0x000073CC
			public Vector2[] Positions { get; private set; }

			// Token: 0x060000BA RID: 186 RVA: 0x000091D8 File Offset: 0x000073D8
			public RingArray(int numSegments)
			{
				this.Positions = new Vector2[numSegments];
				float num = 360f / (float)numSegments;
				for (int i = 0; i < numSegments; i++)
				{
					float f = 0.017453292f * num * (float)i;
					this.Positions[i] = new Vector2(Mathf.Sin(f), Mathf.Cos(f));
				}
			}
		}
	}
}
