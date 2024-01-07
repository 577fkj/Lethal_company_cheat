using System;
using HarmonyLib;
using Pautils;
using XHGUI;

namespace EditItem
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(PatcherTool), "LateUpdate")]
	public class PatcherTool_LateUpdate_Patch
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000022D4 File Offset: 0x000004D4
		public static void Postfix(PatcherTool __instance)
		{
			bool infiniteZapGun = Xhgui.Instance.InfiniteZapGun;
			bool flag = infiniteZapGun;
			if (flag)
			{
				__instance.gunOverheat = 0f;
				__instance.bendMultiplier = 999f;
				__instance.pullStrength = 999f;
				PAUtils.SetValue(__instance, "timeSpentShocking", 0.01f, PAUtils.protectedFlags);
			}
		}
	}
}
