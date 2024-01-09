using System;
using GameNetcodeStuff;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(StartOfRound), "Start")]
	public static class editplayer
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00003838 File Offset: 0x00001A38
		private static void Prefix(StartOfRound __instance)
		{
			bool flag = editplayer.instance == null;
			if (flag)
			{
				editplayer.instance = __instance;
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003860 File Offset: 0x00001A60
		public static StartOfRound Instance()
		{
			return editplayer.instance;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003878 File Offset: 0x00001A78
		public static PlayerControllerB GetlocalPlayer()
		{
			StartOfRound startOfRound = editplayer.instance;
			return (startOfRound != null) ? startOfRound.localPlayerController : null;
		}

		// Token: 0x04000026 RID: 38
		private static StartOfRound instance;
	}
}
