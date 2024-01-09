using System;
using HarmonyLib;

namespace Pautils
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(SteamLobbyManager), "RefreshServerListButton")]
	public class SteamLobbyManager_RefreshServerListButton_Patch
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002BA0 File Offset: 0x00000DA0
		public static bool Prefix(SteamLobbyManager __instance)
		{
			PAUtils.SetValue(__instance, "refreshServerListTimer", 1f, PAUtils.protectedFlags);
			return true;
		}
	}
}
