using System;
using System.Linq;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace EditnewLevel.Patches
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
	public static class EditNewLevel
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000026B4 File Offset: 0x000008B4
		[HarmonyPrefix]
		private static bool EditNewLevelSpawn(ref SelectableLevel newLevel)
		{
			newLevel.maxEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
			newLevel.maxOutsideEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
			newLevel.maxDaytimeEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
			bool flag = testplugin.Instance1.Monsterrarity.Value != -1;
			if (flag)
			{
				foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in newLevel.Enemies)
				{
					spawnableEnemyWithRarity.rarity = testplugin.Instance1.Monsterrarity.Value;
					spawnableEnemyWithRarity.enemyType.numberSpawned = testplugin.Instance1.MaxMonster.Value;
					spawnableEnemyWithRarity.enemyType.PowerLevel = 1;
					spawnableEnemyWithRarity.enemyType.useNumberSpawnedFalloff = false;
					bool value = testplugin.Instance1.AddMonstEreverytime.Value;
					if (value)
					{
						spawnableEnemyWithRarity.enemyType.probabilityCurve.Evaluate(1f);
					}
					Debug.Log(spawnableEnemyWithRarity.enemyType.enemyName);
				}
				foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity2 in newLevel.OutsideEnemies)
				{
					spawnableEnemyWithRarity2.rarity = testplugin.Instance1.Monsterrarity.Value;
					spawnableEnemyWithRarity2.enemyType.numberSpawned = testplugin.Instance1.MaxMonster.Value;
					spawnableEnemyWithRarity2.enemyType.PowerLevel = 1;
					spawnableEnemyWithRarity2.enemyType.useNumberSpawnedFalloff = false;
					bool value2 = testplugin.Instance1.AddMonstEreverytime.Value;
					if (value2)
					{
						spawnableEnemyWithRarity2.enemyType.probabilityCurve.Evaluate(1f);
					}
					Debug.Log(spawnableEnemyWithRarity2.enemyType.enemyName);
				}
				foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity3 in newLevel.DaytimeEnemies)
				{
					spawnableEnemyWithRarity3.rarity = testplugin.Instance1.Monsterrarity.Value;
					spawnableEnemyWithRarity3.enemyType.numberSpawned = testplugin.Instance1.MaxMonster.Value;
					spawnableEnemyWithRarity3.enemyType.PowerLevel = 1;
					spawnableEnemyWithRarity3.enemyType.useNumberSpawnedFalloff = false;
					bool value3 = testplugin.Instance1.AddMonstEreverytime.Value;
					if (value3)
					{
						spawnableEnemyWithRarity3.enemyType.probabilityCurve.Evaluate(1f);
					}
					Debug.Log(spawnableEnemyWithRarity3.enemyType.enemyName);
				}
			}
			if (testplugin.Instance1.Itemrarity.Value != -1)
			{
				if (testplugin.Instance.ItemmaxValue.Value != -1)
				{
					newLevel.maxScrap = testplugin.Instance1.ItemmaxValue_value.Value;
					newLevel.minScrap = newLevel.maxScrap / 2;
				}
				if (testplugin.Instance.ItemmaxValue_value.Value != -1)
				{
					newLevel.maxTotalScrapValue = testplugin.Instance1.ItemmaxValue.Value * 100;
					newLevel.minTotalScrapValue = newLevel.maxTotalScrapValue / 2;
				}
				Debug.Log(string.Concat(new string[]
				{
					"最大价值:",
					newLevel.maxTotalScrapValue.ToString(),
					"  ,最小总价值",
					newLevel.minTotalScrapValue.ToString(),
					" _最大废品",
					newLevel.maxScrap.ToString(),
					"  最小费品 ",
					newLevel.minScrap.ToString()
				}));
				foreach (SpawnableItemWithRarity spawnableItemWithRarity in newLevel.spawnableScrap)
				{
					if (testplugin.Instance1.ItemmaxValue.Value != -1)
					{
						spawnableItemWithRarity.spawnableItem.maxValue = testplugin.Instance1.ItemmaxValue.Value;
						spawnableItemWithRarity.spawnableItem.minValue = spawnableItemWithRarity.spawnableItem.maxValue / 5;
					}
					spawnableItemWithRarity.rarity = testplugin.Instance1.Itemrarity.Value;
				}
			}
			return true;
		}

		// Token: 0x02000012 RID: 18
		[HarmonyPatch(typeof(RoundManager))]
		[HarmonyPatch("Awake")]
		public static class moreenemy
		{
			// Token: 0x06000048 RID: 72 RVA: 0x000052E8 File Offset: 0x000034E8
			public static void Postfix(RoundManager __instance)
			{
				int[] array = (from i in Enumerable.Range(120, 36)
				select i * 30).ToArray<int>();
				Debug.Log("每半小时都能生成敌人");
			}
		}
	}
}
