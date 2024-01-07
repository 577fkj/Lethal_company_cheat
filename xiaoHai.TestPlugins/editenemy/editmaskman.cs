using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x02000018 RID: 24
	[HarmonyPatch(typeof(MaskedPlayerEnemy))]
	public class editmaskman
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00003454 File Offset: 0x00001654
		[HarmonyPostfix]
		[HarmonyPatch("killAnimation")]
		private static void KILLmaskman(MaskedPlayerEnemy __instance)
		{
			bool flag = !testplugin.Instance.CanbeCarryingbyFlowerman.Value;
			if (flag)
			{
				__instance.enemyType.canDie = true;
				__instance.KillEnemyOnOwnerClient(false);
				Debug.Log("小海提醒你:杀死了面具人");
			}
		}
	}
}
