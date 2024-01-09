using System;
using HarmonyLib;
using XHGUI;

namespace EditItem
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
	public class ShotgunItem_ItemActivate_Patch
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002B5C File Offset: 0x00000D5C
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
