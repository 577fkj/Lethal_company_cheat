using System;
using HarmonyLib;
using Testplugin;

namespace EditshipStrat.Patches
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("Awake")]
	public static class TimeOfDayAwakePatch
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002B68 File Offset: 0x00000D68
		private static void Postfix(TimeOfDay __instance)
		{
			bool flag = __instance.quotaVariables != null;
			bool flag2 = flag;
			if (flag2)
			{
				__instance.quotaVariables.deadlineDaysAmount = testplugin.Instance.deadlineDaysAmount.Value;
				__instance.quotaVariables.startingQuota = testplugin.Instance.startingQuota.Value;
				__instance.quotaVariables.startingCredits = testplugin.Instance.startingCredits.Value;
			}
		}
	}
}
