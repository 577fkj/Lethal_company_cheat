using System;
using GameNetcodeStuff;
using HarmonyLib;
using Testplugin;

namespace Editplayer.Patches
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(StartOfRound), "OpenShipDoors")]
	public static class OpenShipDoorsPatch
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000038D0 File Offset: 0x00001AD0
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

		// Token: 0x06000056 RID: 86 RVA: 0x0000393C File Offset: 0x00001B3C
		public static StartOfRound Getopenthedoor()
		{
			return OpenShipDoorsPatch.opendoor_instance;
		}

		// Token: 0x04000027 RID: 39
		private static StartOfRound opendoor_instance;
	}
}
