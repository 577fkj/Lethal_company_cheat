using System;
using HarmonyLib;

namespace Pautils.ProjectApparatus
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
	public class SteamLobbyManager_loadLobbyListAndFilter_Patch
	{
		// Token: 0x06000039 RID: 57 RVA: 0x0000331C File Offset: 0x0000151C
		public static bool Prefix(SteamLobbyManager __instance)
		{
			__instance.censorOffensiveLobbyNames = false;
			return true;
		}
	}
}
