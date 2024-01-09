using System;
using System.Linq;
using HarmonyLib;
using Testplugin;
using UnityEngine;

namespace EditnewLevel.Patches
{
	// Token: 0x0200001D RID: 29
	[HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
	public static class EditNewLevel
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00003EC4 File Offset: 0x000020C4
		[HarmonyPrefix]
		private static bool EditNewLevelSpawn(ref SelectableLevel newLevel)
		{
			bool flag = testplugin.Instance1.Monsterrarity.Value != -1;
			if (flag)
			{
				newLevel.maxEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
				newLevel.maxOutsideEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
				newLevel.maxDaytimeEnemyPowerCount = testplugin.Instance1.MaxMonster.Value * 10;
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
			bool flag2 = testplugin.Instance1.Itemrarity.Value != -1;
			if (flag2)
			{
				bool flag3 = testplugin.Instance.ItemmaxValue.Value != -1;
				if (flag3)
				{
					newLevel.maxScrap = testplugin.Instance1.ItemmaxValue_value.Value;
					newLevel.minScrap = newLevel.maxScrap / 2;
				}
				bool flag4 = testplugin.Instance.ItemmaxValue_value.Value != -1;
				if (flag4)
				{
					newLevel.maxTotalScrapValue = testplugin.Instance1.ItemmaxValue.Value * 100;
					newLevel.minTotalScrapValue = newLevel.maxTotalScrapValue / 2;
				}
				Debug.Log(string.Concat(new string[]
				{
					"小海提醒你:最大价值:",
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
					bool value4 = testplugin.Instance1.ItemNotNeedTwoHands.Value;
					if (value4)
					{
						spawnableItemWithRarity.spawnableItem.twoHanded = false;
						spawnableItemWithRarity.spawnableItem.twoHandedAnimation = false;
					}
					bool flag5 = testplugin.Instance1.ItemmaxValue.Value != -1;
					if (flag5)
					{
						spawnableItemWithRarity.spawnableItem.maxValue = testplugin.Instance1.ItemmaxValue.Value;
						spawnableItemWithRarity.spawnableItem.minValue = spawnableItemWithRarity.spawnableItem.maxValue / 5;
					}
					bool flag6 = testplugin.Instance1.Itemweight.Value != -1f;
					if (flag6)
					{
						spawnableItemWithRarity.spawnableItem.weight = testplugin.Instance1.Itemweight.Value;
					}
					spawnableItemWithRarity.rarity = testplugin.Instance1.Itemrarity.Value;
				}
			}
			return true;
		}

		// Token: 0x0200002A RID: 42
		[HarmonyPatch(typeof(RoundManager))]
		[HarmonyPatch("Awake")]
		public static class moreenemy
		{
			// Token: 0x060000BD RID: 189 RVA: 0x0000933C File Offset: 0x0000753C
			public static void Postfix(RoundManager __instance)
			{
				bool flag = testplugin.Instance1.Monsterrarity.Value != -1;
				if (flag)
				{
					int[] array = (from i in Enumerable.Range(120, 36)
					select i * 30).ToArray<int>();
					Debug.Log("小海提醒你:每半小时都能生成敌人");
				}
			}
		}
	}
}
