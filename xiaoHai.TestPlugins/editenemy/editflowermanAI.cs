using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x02000017 RID: 23
	[HarmonyPatch(typeof(FlowermanAI))]
	public class editflowermanAI
	{
		// Token: 0x06000059 RID: 89 RVA: 0x00003404 File Offset: 0x00001604
		[HarmonyPostfix]
		[HarmonyPatch("FinishKillAnimation")]
		private static void KILLflowerman(FlowermanAI __instance)
		{
			bool flag = !testplugin.Instance.CanbeCarryingbyFlowerman.Value;
			if (flag)
			{
				__instance.enemyType.canDie = true;
				__instance.KillEnemyOnOwnerClient(false);
				Debug.Log("小海提醒你:杀死了小黑");
			}
		}
	}
}
