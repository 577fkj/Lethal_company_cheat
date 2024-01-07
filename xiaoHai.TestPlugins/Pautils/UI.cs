using System;
using UnityEngine;
using XHGUI;

namespace Pautils
{
	// Token: 0x02000008 RID: 8
	public static class UI
	{
		public static void Reset()
		{
			UI.strTooltip = null;
		}

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

		public static void Tab<T>(string strTabName, ref T iTab, T iTabEle, bool bCenter = false)
		{
			bool flag = bCenter ? UI.CenteredButton(strTabName) : GUILayout.Button(strTabName, Array.Empty<GUILayoutOption>());
			if (flag)
			{
				iTab = iTabEle;
				Settings.Instance.windowRect.height = 400f;
			}
		}

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

		// Token: 0x0400001F RID: 31
		public static UI.Tabs nTab;

		// Token: 0x04000020 RID: 32
		public static string strTooltip;

		// Token: 0x02000023 RID: 35
		public enum Tabs
		{
			// Token: 0x040000A2 RID: 162
			Start,
			// Token: 0x040000A3 RID: 163
			Self,
			// Token: 0x040000A4 RID: 164
			Misc,
			// Token: 0x040000A5 RID: 165
			ESP,
			// Token: 0x040000A6 RID: 166
			Players,
			// Token: 0x040000A7 RID: 167
			Graphics,
			// Token: 0x040000A8 RID: 168
			Upgrades,
			// Token: 0x040000A9 RID: 169
			Settings
		}
	}
}
