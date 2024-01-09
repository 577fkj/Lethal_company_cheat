using System;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000016 RID: 22
	[HarmonyPatch(typeof(StartOfRound), "ResetMiscValues")]
	public static class ResetMiscValuesPatch
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003953 File Offset: 0x00001B53
		// (set) Token: 0x06000058 RID: 88 RVA: 0x0000395A File Offset: 0x00001B5A
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x06000059 RID: 89 RVA: 0x00003964 File Offset: 0x00001B64
		private static void Prefix()
		{
			bool playerResurrectedManually = ResetMiscValuesPatch.PlayerResurrectedManually;
			if (playerResurrectedManually)
			{
				ResetMiscValuesPatch.PlayerResurrectedManually = false;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003985 File Offset: 0x00001B85
		public static void SetPlayerResurrectedManually()
		{
			ResetMiscValuesPatch.PlayerResurrectedManually = true;
		}
	}
}
