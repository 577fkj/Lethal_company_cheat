using System;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(StartOfRound), "ResetMiscValues")]
	public static class ResetMiscValuesPatch
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00003133 File Offset: 0x00001333
		// (set) Token: 0x0600004A RID: 74 RVA: 0x0000313A File Offset: 0x0000133A
		public static bool PlayerResurrectedManually { get; private set; }

		// Token: 0x0600004B RID: 75 RVA: 0x00003144 File Offset: 0x00001344
		private static void Prefix()
		{
			bool playerResurrectedManually = ResetMiscValuesPatch.PlayerResurrectedManually;
			if (playerResurrectedManually)
			{
				ResetMiscValuesPatch.PlayerResurrectedManually = false;
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003165 File Offset: 0x00001365
		public static void SetPlayerResurrectedManually()
		{
			ResetMiscValuesPatch.PlayerResurrectedManually = true;
		}
	}
}
