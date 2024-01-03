using System;
using HarmonyLib;

namespace EditshipStrat.Patches
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(TimeOfDay))]
	[HarmonyPatch("MoveGlobalTime")]
	public static class StopTime
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002BD5 File Offset: 0x00000DD5
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002BDC File Offset: 0x00000DDC
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x06000029 RID: 41 RVA: 0x00002BE4 File Offset: 0x00000DE4
		private static bool Prefix()
		{
			bool playerResurrectedManually = PlayerResurrectedManually;
			return !playerResurrectedManually;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002C05 File Offset: 0x00000E05
		public static void SetPlayerResurrectedManually(bool a)
		{
            PlayerResurrectedManually = a;
		}
	}
}
