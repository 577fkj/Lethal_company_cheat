using System;
using HarmonyLib;
using Pautils;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
	public class ShipBuildModeManager_Update_Patch
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00002F14 File Offset: 0x00001114
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
