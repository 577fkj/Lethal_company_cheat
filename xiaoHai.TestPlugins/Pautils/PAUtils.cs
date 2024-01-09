using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pautils
{
	// Token: 0x0200000C RID: 12
	public static class PAUtils
	{
		// Token: 0x0600002D RID: 45
		[DllImport("User32.dll")]
		public static extern short GetAsyncKeyState(int key);

		// Token: 0x0600002E RID: 46
		[DllImport("User32.dll")]
		public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

		// Token: 0x0600002F RID: 47 RVA: 0x0000312B File Offset: 0x0000132B
		public static void ShowMessageBox(string message)
		{
			PAUtils.MessageBox(IntPtr.Zero, message, "Project Apparatus", 0U);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003140 File Offset: 0x00001340
		public static void SetValue(object instance, string variableName, object value, BindingFlags bindingFlags)
		{
			FieldInfo field = instance.GetType().GetField(variableName, bindingFlags);
			bool flag = field == null;
			if (!flag)
			{
				field.SetValue(instance, value);
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003174 File Offset: 0x00001374
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

		// Token: 0x06000032 RID: 50 RVA: 0x000031AC File Offset: 0x000013AC
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

		// Token: 0x06000033 RID: 51 RVA: 0x000031E4 File Offset: 0x000013E4
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

		// Token: 0x06000034 RID: 52 RVA: 0x00003218 File Offset: 0x00001418
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

		// Token: 0x06000035 RID: 53 RVA: 0x00003254 File Offset: 0x00001454
		public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
		{
			screen = camera.WorldToViewportPoint(world);
			screen.x *= (float)Screen.width;
			screen.y *= (float)Screen.height;
			screen.y = (float)Screen.height - screen.y;
			return screen.z > 0f;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000032B4 File Offset: 0x000014B4
		public static float GetDistance(Vector3 pos1, Vector3 pos2)
		{
			return (float)Math.Round((double)Vector3.Distance(pos1, pos2));
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000032D4 File Offset: 0x000014D4
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

		// Token: 0x04000025 RID: 37
		public static BindingFlags protectedFlags = BindingFlags.Instance | BindingFlags.NonPublic;
	}
}
