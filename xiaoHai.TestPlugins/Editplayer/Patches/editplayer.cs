using System;
using GameNetcodeStuff;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(StartOfRound), "Start")]
	public static class editplayer
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003018 File Offset: 0x00001218
		private static void Prefix(StartOfRound __instance)
		{
			bool flag = editplayer.instance == null;
			if (flag)
			{
				editplayer.instance = __instance;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003040 File Offset: 0x00001240
		public static StartOfRound Instance()
		{
			return editplayer.instance;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003058 File Offset: 0x00001258
		public static PlayerControllerB GetlocalPlayer()
		{
			StartOfRound startOfRound = editplayer.instance;
			return (startOfRound != null) ? startOfRound.localPlayerController : null;
		}

		// Token: 0x04000022 RID: 34
		private static StartOfRound instance;
	}
}
