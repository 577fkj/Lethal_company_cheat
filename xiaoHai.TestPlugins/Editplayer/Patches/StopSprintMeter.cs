using System;
using GameNetcodeStuff;
using HarmonyLib;

namespace Editplayer.Patches
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
	public static class StopSprintMeter
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000247B File Offset: 0x0000067B
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002482 File Offset: 0x00000682
		public static bool stopspringtMeter { get; private set; }

		// Token: 0x0600001A RID: 26 RVA: 0x0000248C File Offset: 0x0000068C
		public static void Postfix(PlayerControllerB __instance)
		{
			bool stopspringtMeter = StopSprintMeter.stopspringtMeter;
			if (stopspringtMeter)
			{
				__instance.sprintMeter = 1f;
				GameNetworkManager.Instance.localPlayerController.sprintMeter = 1f;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000024C4 File Offset: 0x000006C4
		public static void isStopspringntMeter(bool a)
		{
			if (a)
			{
				stopspringtMeter = true;
			}
			else
			{
				stopspringtMeter = false;
			}
		}
	}
}
