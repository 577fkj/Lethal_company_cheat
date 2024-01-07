using System;
using HarmonyLib;

namespace Pautils
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(SteamLobbyManager), "RefreshServerListButton")]
	public class SteamLobbyManager_RefreshServerListButton_Patch
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002380 File Offset: 0x00000580
		public static bool Prefix(SteamLobbyManager __instance)
		{
			PAUtils.SetValue(__instance, "refreshServerListTimer", 1f, PAUtils.protectedFlags);
			return true;
		}
	}
}
