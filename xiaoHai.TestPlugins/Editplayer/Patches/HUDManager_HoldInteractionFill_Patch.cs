using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(HUDManager), "HoldInteractionFill")]
	public class HUDManager_HoldInteractionFill_Patch
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00003804 File Offset: 0x00001A04
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
