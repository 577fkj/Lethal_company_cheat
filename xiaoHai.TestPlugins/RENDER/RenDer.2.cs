using System;
using UnityEngine;

namespace render
{
	// Token: 0x0200001C RID: 28
	public class RenDer : MonoBehaviour
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00003CC2 File Offset: 0x00001EC2
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00003CC9 File Offset: 0x00001EC9
		public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003CD4 File Offset: 0x00001ED4
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00003CEB File Offset: 0x00001EEB
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

		// Token: 0x0600006F RID: 111 RVA: 0x00003CF8 File Offset: 0x00001EF8
		public static void DrawString(Vector2 position, string label, bool centered = true)
		{
			GUIContent content = new GUIContent(label);
			Vector2 vector = RenDer.StringStyle.CalcSize(content);
			GUI.Label(new Rect(centered ? (position - vector / 2f) : position, vector), content);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003D40 File Offset: 0x00001F40
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

		// Token: 0x06000071 RID: 113 RVA: 0x00003E24 File Offset: 0x00002024
		public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
		{
			RenDer.DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
			RenDer.DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
			RenDer.DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
			RenDer.DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
		}

		// Token: 0x0400002D RID: 45
		public static Texture2D lineTex;
	}
}
