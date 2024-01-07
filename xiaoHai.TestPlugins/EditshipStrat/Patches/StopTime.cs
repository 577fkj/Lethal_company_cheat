using System;
using HarmonyLib;

namespace EditshipStrat.Patches
{
	// Token: 0x0200001C RID: 28
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("MoveGlobalTime")]
	public static class StopTime
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003D04 File Offset: 0x00001F04
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00003D0B File Offset: 0x00001F0B
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x0600006A RID: 106 RVA: 0x00003D14 File Offset: 0x00001F14
		private static bool Prefix()
		{
			bool playerResurrectedManually = StopTime.PlayerResurrectedManually;
			return !playerResurrectedManually;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003D35 File Offset: 0x00001F35
		public static void SetPlayerResurrectedManually(bool a)
		{
			StopTime.PlayerResurrectedManually = a;
		}
	}
}
