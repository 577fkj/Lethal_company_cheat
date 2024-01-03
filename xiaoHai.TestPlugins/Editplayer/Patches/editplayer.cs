using System;
using GameNetcodeStuff;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(StartOfRound), "Start")]
	public static class editplayer
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000022B4 File Offset: 0x000004B4
		private static void Postfix(StartOfRound __instance)
		{
			if (editplayer.instance == null)
			{
				editplayer.instance = __instance;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022DC File Offset: 0x000004DC
		public static StartOfRound Instance()
		{
			return editplayer.instance;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022F4 File Offset: 0x000004F4
		public static PlayerControllerB GetlocalPlayer()
		{
			StartOfRound startOfRound = editplayer.instance;
			return (startOfRound != null) ? startOfRound.localPlayerController : null;
		}

		// Token: 0x04000004 RID: 4
		private static StartOfRound instance;
	}
}
