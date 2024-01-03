using System;
using HarmonyLib;
using UnityEngine;

namespace editenemy
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(RoundManager), "PlotOutEnemiesForNextHour")]
	public static class EditEnemy
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002564 File Offset: 0x00000764
		private static void Postfix(RoundManager __instance)
		{
			if (__instance != null)
			{
				if (__instance.allEnemyVents != null)
				{
					foreach (EnemyVent enemyVent in __instance.allEnemyVents)
					{
						Debug.Log("洞穴生成时间=" + enemyVent.spawnTime.ToString() + "洞穴名字=" + enemyVent.name);
					}
				}
				for (int j = 0; j < __instance.enemySpawnTimes.Count; j++)
				{
					__instance.enemySpawnTimes[j] = 20;
				}
				Debug.Log("修改怪物参数成功!");
			}
		}
	}
}
