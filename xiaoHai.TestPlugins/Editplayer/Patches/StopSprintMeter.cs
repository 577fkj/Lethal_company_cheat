using System;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using Pautils;
using UnityEngine;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000017 RID: 23
	[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
	public static class StopSprintMeter
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005B RID: 91 RVA: 0x0000398F File Offset: 0x00001B8F
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00003996 File Offset: 0x00001B96
		public static bool stopspringtMeter { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005D RID: 93 RVA: 0x0000399E File Offset: 0x00001B9E
		// (set) Token: 0x0600005E RID: 94 RVA: 0x000039A5 File Offset: 0x00001BA5
		public static bool notwohand { get; private set; }

		// Token: 0x0600005F RID: 95 RVA: 0x000039B0 File Offset: 0x00001BB0
		public static void Postfix(PlayerControllerB __instance)
		{
			bool stopspringtMeter = StopSprintMeter.stopspringtMeter;
			if (stopspringtMeter)
			{
				__instance.sprintMeter = 1f;
			}
			bool flag = __instance.currentlyHeldObjectServer != null;
			if (flag)
			{
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				PAUtils.SetValue(__instance, "interactableObjectsMask", Xhgui.Instance.renyizhuaqu ? LayerMask.GetMask(new string[]
				{
					"Props",
					"InteractableObject"
				}) : 832, bindingFlags);
				__instance.grabDistance = (Xhgui.Instance.renyizhuaqu ? 9999f : 5f);
				bool notwohand = StopSprintMeter.notwohand;
				if (notwohand)
				{
					__instance.twoHanded = false;
					__instance.twoHandedAnimation = false;
					__instance.currentlyHeldObjectServer.itemProperties.twoHanded = false;
					__instance.currentlyHeldObjectServer.itemProperties.twoHandedAnimation = false;
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003A88 File Offset: 0x00001C88
		public static void isStopspringntMeter(bool a)
		{
			if (a)
			{
				StopSprintMeter.stopspringtMeter = true;
			}
			else
			{
				StopSprintMeter.stopspringtMeter = false;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003AB0 File Offset: 0x00001CB0
		public static void isNoNeedTwoHands(bool a)
		{
			if (a)
			{
				StopSprintMeter.notwohand = true;
			}
			else
			{
				StopSprintMeter.notwohand = false;
			}
		}

		// Token: 0x02000029 RID: 41
		[HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
		public class StartOfRound_UpdatePlayerVoiceEffects_Patch
		{
			// Token: 0x060000BB RID: 187 RVA: 0x00009240 File Offset: 0x00007440
			public static void Postfix(StartOfRound __instance)
			{
				bool flag = Xhgui.Instance.HearEveryone && !StartOfRound.Instance.shipIsLeaving;
				bool flag2 = flag;
				if (flag2)
				{
					for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
					{
						PlayerControllerB playerControllerB = __instance.allPlayerScripts[i];
						AudioSource currentVoiceChatAudioSource = playerControllerB.currentVoiceChatAudioSource;
						currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>().enabled = false;
						currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>().enabled = false;
						currentVoiceChatAudioSource.panStereo = 0f;
						SoundManager.Instance.playerVoicePitchTargets[(int)((IntPtr)((long)playerControllerB.playerClientId))] = 1f;
						SoundManager.Instance.SetPlayerPitch(1f, (int)playerControllerB.playerClientId);
						currentVoiceChatAudioSource.spatialBlend = 0f;
						playerControllerB.currentVoiceChatIngameSettings.set2D = true;
						playerControllerB.voicePlayerState.Volume = 1f;
					}
				}
			}
		}
	}
}
