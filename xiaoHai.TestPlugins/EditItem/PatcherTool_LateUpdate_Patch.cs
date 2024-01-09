using System;
using HarmonyLib;
using Pautils;
using XHGUI;

namespace EditItem
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(PatcherTool), "LateUpdate")]
	public class PatcherTool_LateUpdate_Patch
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002AF4 File Offset: 0x00000CF4
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
