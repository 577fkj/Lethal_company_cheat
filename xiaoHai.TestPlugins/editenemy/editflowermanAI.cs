using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x0200001A RID: 26
	[HarmonyPatch(typeof(FlowermanAI))]
	public class editflowermanAI
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00003C24 File Offset: 0x00001E24
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
