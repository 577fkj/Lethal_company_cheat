using System;
using GameNetcodeStuff;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
	public class PlayerControllerB_CheckConditionsForEmote_Patch
	{
		// Token: 0x06000045 RID: 69 RVA: 0x0000307C File Offset: 0x0000127C
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
