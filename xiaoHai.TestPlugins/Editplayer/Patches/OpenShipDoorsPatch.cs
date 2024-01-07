using System;
using GameNetcodeStuff;
using HarmonyLib;
using Testplugin;

namespace Editplayer.Patches
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(StartOfRound), "OpenShipDoors")]
	public static class OpenShipDoorsPatch
	{
		// Token: 0x06000047 RID: 71 RVA: 0x000030B0 File Offset: 0x000012B0
		public static void Postfix(StartOfRound __instance)
		{
			OpenShipDoorsPatch.opendoor_instance = __instance;
			__instance.maxShipItemCapacity = 999;
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = testplugin.Instance1.Itemweight.Value != -1f;
			if (flag)
			{
				bool flag2 = localPlayerController != null;
				if (flag2)
				{
					localPlayerController.carryWeight = testplugin.Instance1.Itemweight.Value;
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000311C File Offset: 0x0000131C
		public static StartOfRound Getopenthedoor()
		{
			return OpenShipDoorsPatch.opendoor_instance;
		}

		// Token: 0x04000023 RID: 35
		private static StartOfRound opendoor_instance;
	}
}
