using System;
using GameNetcodeStuff;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace Editplayer.Patches
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(StartOfRound), "OpenShipDoors")]
	public static class OpenShipDoorsPatch
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002318 File Offset: 0x00000518
		public static void Postfix(StartOfRound __instance)
		{
			__instance.maxShipItemCapacity = 999;
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (testplugin.Instance1.Itemweight.Value != -1f)
			{
				if (localPlayerController != null)
				{
					localPlayerController.carryWeight = testplugin.Instance1.Itemweight.Value;
				}
			}
			opendoor_instance = __instance;
			AllItemsList allItemsList = new AllItemsList();
			foreach (Item item in allItemsList.itemsList)
			{
				Debug.Log("物品名:" + item.itemName + "物品ID:" + item.itemId.ToString());
				item.requiresBattery = false;
				if (testplugin.Instance1.ItemNotNeedTwoHands.Value)
				{
					item.twoHanded = false;
				}
				item.twoHandedAnimation = false;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002428 File Offset: 0x00000628
		public static StartOfRound Getopenthedoor()
		{
			return opendoor_instance;
		}

		// Token: 0x04000005 RID: 5
		private static StartOfRound opendoor_instance;
	}
}
