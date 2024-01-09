using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x0200001B RID: 27
	[HarmonyPatch(typeof(MaskedPlayerEnemy))]
	public class editmaskman
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00003C74 File Offset: 0x00001E74
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
