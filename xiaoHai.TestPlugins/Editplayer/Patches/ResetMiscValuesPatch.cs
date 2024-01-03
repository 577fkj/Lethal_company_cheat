using System;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(StartOfRound), "ResetMiscValues")]
	public static class ResetMiscValuesPatch
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000243F File Offset: 0x0000063F
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002446 File Offset: 0x00000646
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x06000016 RID: 22 RVA: 0x00002450 File Offset: 0x00000650
		private static void Prefix()
		{
			bool playerResurrectedManually = ResetMiscValuesPatch.PlayerResurrectedManually;
			if (playerResurrectedManually)
			{
				ResetMiscValuesPatch.PlayerResurrectedManually = false;
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002471 File Offset: 0x00000671
		public static void SetPlayerResurrectedManually()
		{
			ResetMiscValuesPatch.PlayerResurrectedManually = true;
		}
	}
}
