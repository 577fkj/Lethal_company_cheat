using System;
using UnityEngine;

namespace RENDER
{
	// Token: 0x0200000B RID: 11
	public static class Render
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002B20 File Offset: 0x00000D20
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002B37 File Offset: 0x00000D37
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

		// Token: 0x0600002F RID: 47 RVA: 0x00002B41 File Offset: 0x00000D41
		public static void Line(Vector2 from, Vector2 to, float thickness, Color color)
		{
			Render.Color = color;
			Render.Line(from, to, thickness);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002B54 File Offset: 0x00000D54
		public static void Line(Vector2 from, Vector2 to, float thickness)
		{
			Vector2 normalized = (to - from).normalized;
			float num = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			GUIUtility.RotateAroundPivot(num, from);
			Render.Box(from, Vector2.right * (from - to).magnitude, thickness, false);
			GUIUtility.RotateAroundPivot(-num, from);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002BBD File Offset: 0x00000DBD
		public static void Box(Vector2 position, Vector2 size, float thickness, Color color, bool centered = true)
		{
			Render.Color = color;
			Render.Box(position, size, thickness, centered);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002BD4 File Offset: 0x00000DD4
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

		// Token: 0x06000033 RID: 51 RVA: 0x00002C98 File Offset: 0x00000E98
		public static void Cross(Vector2 position, Vector2 size, float thickness, Color color)
		{
			Render.Color = color;
			Render.Cross(position, size, thickness);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002CAC File Offset: 0x00000EAC
		public static void Cross(Vector2 position, Vector2 size, float thickness)
		{
			GUI.DrawTexture(new Rect(position.x - size.x / 2f, position.y, size.x, thickness), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(position.x, position.y - size.y / 2f, thickness, size.y), Texture2D.whiteTexture);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002D1A File Offset: 0x00000F1A
		public static void Dot(Vector2 position, Color color)
		{
			Render.Color = color;
			Render.Dot(position);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002D2B File Offset: 0x00000F2B
		public static void Dot(Vector2 position)
		{
			Render.Box(position - Vector2.one, Vector2.one * 2f, 1f, true);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002D54 File Offset: 0x00000F54
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

		// Token: 0x06000038 RID: 56 RVA: 0x00002E10 File Offset: 0x00001010
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

		// Token: 0x06000039 RID: 57 RVA: 0x00002E84 File Offset: 0x00001084
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

		// Token: 0x02000024 RID: 36
		private class RingArray
		{
			// Token: 0x1700000F RID: 15
			// (get) Token: 0x0600009F RID: 159 RVA: 0x00007B8C File Offset: 0x00005D8C
			// (set) Token: 0x060000A0 RID: 160 RVA: 0x00007B94 File Offset: 0x00005D94
			public Vector2[] Positions { get; private set; }

			// Token: 0x060000A1 RID: 161 RVA: 0x00007BA0 File Offset: 0x00005DA0
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
