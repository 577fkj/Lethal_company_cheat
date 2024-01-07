using System;
using UnityEngine;

namespace render
{
	// Token: 0x02000019 RID: 25
	public class RenDer : MonoBehaviour
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000034A2 File Offset: 0x000016A2
		// (set) Token: 0x0600005E RID: 94 RVA: 0x000034A9 File Offset: 0x000016A9
		public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000034B4 File Offset: 0x000016B4
		// (set) Token: 0x06000060 RID: 96 RVA: 0x000034CB File Offset: 0x000016CB
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

		// Token: 0x06000061 RID: 97 RVA: 0x000034D8 File Offset: 0x000016D8
		public static void DrawString(Vector2 position, string label, bool centered = true)
		{
			GUIContent content = new GUIContent(label);
			Vector2 vector = RenDer.StringStyle.CalcSize(content);
			GUI.Label(new Rect(centered ? (position - vector / 2f) : position, vector), content);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003520 File Offset: 0x00001720
		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
		{
			Matrix4x4 matrix = GUI.matrix;
			bool flag = !RenDer.lineTex;
			if (flag)
			{
				RenDer.lineTex = new Texture2D(1, 1);
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
			GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), RenDer.lineTex);
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003604 File Offset: 0x00001804
		public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
		{
			RenDer.DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
			RenDer.DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
			RenDer.DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
			RenDer.DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
		}

		// Token: 0x04000029 RID: 41
		public static Texture2D lineTex;
	}
}
