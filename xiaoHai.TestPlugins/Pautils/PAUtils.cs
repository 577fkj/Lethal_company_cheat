using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pautils
{
	// Token: 0x02000009 RID: 9
	public static class PAUtils
	{
		// Token: 0x0600001F RID: 31
		[DllImport("User32.dll")]
		public static extern short GetAsyncKeyState(int key);

		// Token: 0x06000020 RID: 32
		[DllImport("User32.dll")]
		public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

		// Token: 0x06000021 RID: 33 RVA: 0x0000290B File Offset: 0x00000B0B
		public static void ShowMessageBox(string message)
		{
			PAUtils.MessageBox(IntPtr.Zero, message, "Project Apparatus", 0U);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002920 File Offset: 0x00000B20
		public static void SetValue(object instance, string variableName, object value, BindingFlags bindingFlags)
		{
			FieldInfo field = instance.GetType().GetField(variableName, bindingFlags);
			bool flag = field == null;
			if (!flag)
			{
				field.SetValue(instance, value);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002954 File Offset: 0x00000B54
		public static object GetValue(object instance, string variableName, BindingFlags bindingFlags)
		{
			FieldInfo field = instance.GetType().GetField(variableName, bindingFlags);
			bool flag = field != null;
			object result;
			if (flag)
			{
				result = field.GetValue(instance);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000298C File Offset: 0x00000B8C
		public static object CallMethod(object instance, string methodName, BindingFlags bindingFlags, params object[] parameters)
		{
			MethodInfo method = instance.GetType().GetMethod(methodName, bindingFlags);
			bool flag = method != null;
			object result;
			if (flag)
			{
				result = method.Invoke(instance, parameters);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000029C4 File Offset: 0x00000BC4
		public static string ConvertFirstLetterToUpperCase(string input)
		{
			bool flag = string.IsNullOrEmpty(input);
			string result;
			if (flag)
			{
				result = input;
			}
			else
			{
				result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000029F8 File Offset: 0x00000BF8
		public static string TruncateString(string inputStr, int charLimit)
		{
			bool flag = inputStr.Length <= charLimit;
			string result;
			if (flag)
			{
				result = inputStr;
			}
			else
			{
				result = inputStr.Substring(0, charLimit - 3) + "...";
			}
			return result;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002A34 File Offset: 0x00000C34
		public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
		{
			screen = camera.WorldToViewportPoint(world);
			screen.x *= (float)Screen.width;
			screen.y *= (float)Screen.height;
			screen.y = (float)Screen.height - screen.y;
			return screen.z > 0f;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A94 File Offset: 0x00000C94
		public static float GetDistance(Vector3 pos1, Vector3 pos2)
		{
			return (float)Math.Round((double)Vector3.Distance(pos1, pos2));
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002AB4 File Offset: 0x00000CB4
		public static void SendChatMessage(string str, int playerid = -1)
		{
			string text = str;
			bool flag = HUDManager.Instance.lastChatMessage == text;
			if (flag)
			{
				text += "\r";
			}
			HUDManager.Instance.AddTextToChatOnServer(text, playerid);
		}

		// Token: 0x04000021 RID: 33
		public static BindingFlags protectedFlags = BindingFlags.Instance | BindingFlags.NonPublic;
	}
}
