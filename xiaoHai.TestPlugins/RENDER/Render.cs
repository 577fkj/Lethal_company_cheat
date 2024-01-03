using System;
using UnityEngine;

namespace RENDER
{
	// Token: 0x02000003 RID: 3
	public class Render : MonoBehaviour
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002086 File Offset: 0x00000286
		// (set) Token: 0x06000005 RID: 5 RVA: 0x0000208D File Offset: 0x0000028D
		public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002098 File Offset: 0x00000298
		// (set) Token: 0x06000007 RID: 7 RVA: 0x000020AF File Offset: 0x000002AF
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

		// Token: 0x06000008 RID: 8 RVA: 0x000020BC File Offset: 0x000002BC
		public static void DrawString(Vector2 position, string label, bool centered = true)
		{
			GUIContent content = new GUIContent(label);
			Vector2 vector = Render.StringStyle.CalcSize(content);
			GUI.Label(new Rect(centered ? (position - vector / 2f) : position, vector), content);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002104 File Offset: 0x00000304
		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
		{
			Matrix4x4 matrix = GUI.matrix;
			bool flag = !Render.lineTex;
			if (flag)
			{
				Render.lineTex = new Texture2D(1, 1);
			}
			Color color2 = GUI.color;
			GUI.color = color;
			float num = Vector3.Angle(pointB - pointA, Vector2.right);
			bool flag2 = pointA.y > pointB.y;
			if (flag2)
			{
				num = -num;
			}
			GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
			GUIUtility.RotateAroundPivot(num, pointA);
			GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), Render.lineTex);
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021E8 File Offset: 0x000003E8
		public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
		{
			Render.DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
			Render.DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
			Render.DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
			Render.DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
		}

		// Token: 0x04000003 RID: 3
		public static Texture2D lineTex;
	}
}
