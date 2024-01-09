using System;
using HarmonyLib;
using Pautils;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
	public class ShipBuildModeManager_Update_Patch
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00003734 File Offset: 0x00001934
		public static void Postfix(ShipBuildModeManager __instance)
		{
			bool placeAnywhere = Xhgui.Instance.PlaceAnywhere;
			if (placeAnywhere)
			{
				PlaceableShipObject placeableShipObject = (PlaceableShipObject)PAUtils.GetValue(__instance, "placingObject", PAUtils.protectedFlags);
				bool flag = placeableShipObject;
				if (flag)
				{
					placeableShipObject.AllowPlacementOnCounters = true;
					placeableShipObject.AllowPlacementOnWalls = true;
					PAUtils.SetValue(__instance, "CanConfirmPosition", true, PAUtils.protectedFlags);
				}
			}
		}
	}
}
