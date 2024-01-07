﻿using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace EditshipStrat.Patches
{
	// Token: 0x0200001B RID: 27
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("Awake")]
	public static class TimeOfDayAwakePatch
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00003BC8 File Offset: 0x00001DC8
		private static void Prefix(TimeOfDay __instance)
		{
			bool flag = __instance.quotaVariables != null;
			bool flag2 = flag;
			if (flag2)
			{
				__instance.quotaVariables.deadlineDaysAmount = testplugin.Instance.deadlineDaysAmount.Value;
				__instance.quotaVariables.startingQuota = testplugin.Instance.startingQuota.Value;
				__instance.quotaVariables.startingCredits = testplugin.Instance.startingCredits.Value;
				__instance.quotaVariables.baseIncrease = testplugin.Instance.baseIncrease.Value;
			}
			AllItemsList allItemsList = new AllItemsList();
			bool flag3 = allItemsList == null;
			if (!flag3)
			{
				foreach (Item item in allItemsList.itemsList)
				{
					Debug.Log("小海提醒你:物品名:" + item.itemName + "物品ID:" + item.itemId.ToString());
					item.requiresBattery = false;
					bool value = testplugin.Instance1.ItemNotNeedTwoHands.Value;
					if (value)
					{
						item.twoHanded = false;
					}
					item.twoHandedAnimation = false;
				}
			}
		}
	}
}
