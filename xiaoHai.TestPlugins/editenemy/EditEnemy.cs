using System;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace editenemy
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch(typeof(RoundManager), "PlotOutEnemiesForNextHour")]
	public static class EditEnemy
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00003B50 File Offset: 0x00001D50
		private static void Prefix(RoundManager __instance)
		{
			bool flag = __instance != null;
			if (flag)
			{
				bool flag2 = testplugin.Instance1.Monsterrarity.Value != -1;
				if (flag2)
				{
					bool flag3 = __instance.allEnemyVents != null;
					if (flag3)
					{
						foreach (EnemyVent enemyVent in __instance.allEnemyVents)
						{
							Debug.Log("小海提醒你:洞穴生成时间=" + enemyVent.spawnTime.ToString() + "洞穴名字=" + enemyVent.name);
						}
					}
					for (int j = 0; j < __instance.enemySpawnTimes.Count; j++)
					{
						__instance.enemySpawnTimes[j] = 20;
						Debug.Log("小海提醒你:修改怪物参数成功!");
					}
				}
			}
		}
	}
}
