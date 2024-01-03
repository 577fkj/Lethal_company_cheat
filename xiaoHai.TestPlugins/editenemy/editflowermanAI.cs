using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x0200000B RID: 11
	[HarmonyPatch(typeof(FlowermanAI))]
	public class editflowermanAI
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00002614 File Offset: 0x00000814
		[HarmonyPostfix]
		[HarmonyPatch("FinishKillAnimation")]
		private static void KILLflowerman(FlowermanAI __instance)
		{
			bool flag = !testplugin.Instance.CanbeCarryingbyFlowerman.Value;
			if (flag)
			{
				__instance.enemyType.canDie = true;
				__instance.KillEnemyOnOwnerClient(false);
				Debug.Log("杀死了小黑");
			}
		}
	}
}
