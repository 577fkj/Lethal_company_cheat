using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
	public class ShipBuildModeManager_PlayerMeetsConditionsToBuild_Patch
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000037A4 File Offset: 0x000019A4
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
