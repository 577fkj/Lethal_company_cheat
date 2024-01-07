using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
	public class ShipBuildModeManager_PlayerMeetsConditionsToBuild_Patch
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00002F84 File Offset: 0x00001184
		public static bool Prefix(ShipBuildModeManager __instance, ref bool __result)
		{
			bool placeAnywhere = Xhgui.Instance.PlaceAnywhere;
			bool result;
			if (placeAnywhere)
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
