using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(HUDManager), "HoldInteractionFill")]
	public class HUDManager_HoldInteractionFill_Patch
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00002FE4 File Offset: 0x000011E4
		public static bool Prefix(HUDManager __instance, ref bool __result)
		{
			bool jishihudong = Xhgui.Instance.jishihudong;
			bool result;
			if (jishihudong)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
