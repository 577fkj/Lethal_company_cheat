using System;
using HarmonyLib;

namespace EditshipStrat.Patches
{
	// Token: 0x0200001F RID: 31
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("MoveGlobalTime")]
	public static class StopTime
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00004524 File Offset: 0x00002724
		// (set) Token: 0x06000077 RID: 119 RVA: 0x0000452B File Offset: 0x0000272B
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x06000078 RID: 120 RVA: 0x00004534 File Offset: 0x00002734
		private static bool Prefix()
		{
			bool playerResurrectedManually = StopTime.PlayerResurrectedManually;
			return !playerResurrectedManually;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004555 File Offset: 0x00002755
		public static void SetPlayerResurrectedManually(bool a)
		{
			StopTime.PlayerResurrectedManually = a;
		}
	}
}
