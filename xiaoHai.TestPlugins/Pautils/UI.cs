using System;
using UnityEngine;
using XHGUI;

namespace Pautils
{
	// Token: 0x0200000B RID: 11
	public static class UI
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00002C7E File Offset: 0x00000E7E
		public static void Reset()
		{
			UI.strTooltip = null;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002C88 File Offset: 0x00000E88
		public static void TabContents(string strTabName, UI.Tabs tabToDisplay, Action tabContent)
		{
			bool flag = UI.nTab == tabToDisplay;
			if (flag)
			{
				bool flag2 = strTabName != null;
				if (flag2)
				{
					UI.Header(strTabName);
				}
				tabContent();
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002CBC File Offset: 0x00000EBC
		public static bool CenteredButton(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			bool result = GUILayout.Button(strName, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002D04 File Offset: 0x00000F04
		public static void Tab<T>(string strTabName, ref T iTab, T iTabEle, bool bCenter = false)
		{
			bool flag = bCenter ? UI.CenteredButton(strTabName) : GUILayout.Button(strTabName, Array.Empty<GUILayoutOption>());
			if (flag)
			{
				iTab = iTabEle;
				Settings.Instance.windowRect.height = 400f;
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002D4A File Offset: 0x00000F4A
		public static void Header(string str)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.Label(str, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002D80 File Offset: 0x00000F80
		public static void ColorPicker(string str, ref Color col)
		{
			GUILayout.Label(string.Concat(new string[]
			{
				str,
				" (R: ",
				Mathf.RoundToInt(col.r * 255f).ToString(),
				", G: ",
				Mathf.RoundToInt(col.g * 255f).ToString(),
				", B: ",
				Mathf.RoundToInt(col.b * 255f).ToString(),
				")"
			}), Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			col.r = GUILayout.HorizontalSlider(col.r, 0f, 1f, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			col.g = GUILayout.HorizontalSlider(col.g, 0f, 1f, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			col.b = GUILayout.HorizontalSlider(col.b, 0f, 1f, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002EB8 File Offset: 0x000010B8
		public static bool Checkbox(ref bool var, string option, string tooltip = "")
		{
			bool flag = var;
			var = GUILayout.Toggle(var, option, Array.Empty<GUILayoutOption>());
			bool flag2 = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
			if (flag2)
			{
				UI.strTooltip = tooltip;
			}
			return flag != var;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002F08 File Offset: 0x00001108
		public static void Button(string option, string tooltip, Action action)
		{
			bool flag = GUILayout.Button(option, Array.Empty<GUILayoutOption>());
			if (flag)
			{
				action();
			}
			bool flag2 = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
			if (flag2)
			{
				UI.strTooltip = tooltip;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F54 File Offset: 0x00001154
		public static void RenderTooltip()
		{
			bool flag = UI.strTooltip == null || UI.strTooltip == "";
			if (!flag)
			{
				float num = GUI.skin.label.CalcSize(new GUIContent(UI.strTooltip)).x + 10f;
				Vector2 mousePosition = Event.current.mousePosition;
				Color c_Door = Xhgui.Instance.c_Door;
				GUI.color = new Color(c_Door.r, c_Door.g, c_Door.b, 0.8f);
				GUI.Box(new Rect(mousePosition.x + 20f, mousePosition.y + 20f, num, Settings.TEXT_HEIGHT), GUIContent.none);
				GUI.color = Color.white;
				GUI.Label(new Rect(mousePosition.x + 25f, mousePosition.y + 25f, num - 10f, Settings.TEXT_HEIGHT), UI.strTooltip);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003050 File Offset: 0x00001250
		public static Texture2D MakeTexture(int width, int height, Color color)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000030A4 File Offset: 0x000012A4
		public static Texture2D MakeGradientTexture(int width, int height, Color startColor, Color endColor, bool isHorizontal = true)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float t = isHorizontal ? ((float)j / (float)(width - 1)) : ((float)i / (float)(height - 1));
					array[i * width + j] = Color.Lerp(startColor, endColor, t);
				}
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x04000023 RID: 35
		public static UI.Tabs nTab;

		// Token: 0x04000024 RID: 36
		public static string strTooltip;

		// Token: 0x02000027 RID: 39
		public enum Tabs
		{
			// Token: 0x040000B7 RID: 183
			Start,
			// Token: 0x040000B8 RID: 184
			Self,
			// Token: 0x040000B9 RID: 185
			Misc,
			// Token: 0x040000BA RID: 186
			ESP,
			// Token: 0x040000BB RID: 187
			Players,
			// Token: 0x040000BC RID: 188
			Graphics,
			// Token: 0x040000BD RID: 189
			Upgrades,
			// Token: 0x040000BE RID: 190
			Settings
		}
	}
}
