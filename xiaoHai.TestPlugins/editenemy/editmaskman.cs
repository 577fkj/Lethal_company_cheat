using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(MaskedPlayerEnemy))]
	public class editmaskman
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002664 File Offset: 0x00000864
		[HarmonyPostfix]
		[HarmonyPatch("killAnimation")]
		private static void KILLmaskman(MaskedPlayerEnemy __instance)
		{
			bool flag = !testplugin.Instance.CanbeCarryingbyFlowerman.Value;
			if (flag)
			{
				__instance.enemyType.canDie = true;
				__instance.KillEnemyOnOwnerClient(false);
				Debug.Log("杀死了面具人");
			}
		}
	}
}
