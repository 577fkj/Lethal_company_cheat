using System;
using HarmonyLib;

namespace Pautils.ProjectApparatus
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
	public class SteamLobbyManager_loadLobbyListAndFilter_Patch
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002AFC File Offset: 0x00000CFC
		public static bool Prefix(SteamLobbyManager __instance)
		{
			__instance.censorOffensiveLobbyNames = false;
			return true;
		}
	}
}
