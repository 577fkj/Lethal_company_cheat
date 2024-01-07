using System;
using HarmonyLib;
using XHGUI;

namespace EditItem
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
	public class ShotgunItem_ItemActivate_Patch
	{
		// Token: 0x0600000D RID: 13 RVA: 0x0000233C File Offset: 0x0000053C
		public static bool Prefix(ShotgunItem __instance)
		{
			bool shotgunammo = Xhgui.Instance.shotgunammo;
			bool flag = shotgunammo;
			if (flag)
			{
				__instance.isReloading = false;
				__instance.shellsLoaded++;
			}
			return true;
		}
	}
}
