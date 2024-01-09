using System;
using GameNetcodeStuff;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000014 RID: 20
	[HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
	public class PlayerControllerB_CheckConditionsForEmote_Patch
	{
		// Token: 0x06000053 RID: 83 RVA: 0x0000389C File Offset: 0x00001A9C
		public static bool Prefix(PlayerControllerB __instance, ref bool __result)
		{
			bool emoteWalk = Xhgui.Instance.EmoteWalk;
			bool result;
			if (emoteWalk)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
