using System;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using Pautils;
using UnityEngine;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000014 RID: 20
	[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
	public static class StopSprintMeter
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000316F File Offset: 0x0000136F
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00003176 File Offset: 0x00001376
		public static bool stopspringtMeter { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004F RID: 79 RVA: 0x0000317E File Offset: 0x0000137E
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00003185 File Offset: 0x00001385
		public static bool notwohand { get; private set; }

		// Token: 0x06000051 RID: 81 RVA: 0x00003190 File Offset: 0x00001390
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

		// Token: 0x06000052 RID: 82 RVA: 0x00003268 File Offset: 0x00001468
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

		// Token: 0x06000053 RID: 83 RVA: 0x00003290 File Offset: 0x00001490
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

		// Token: 0x02000025 RID: 37
		[HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
		public class StartOfRound_UpdatePlayerVoiceEffects_Patch
		{
			// Token: 0x060000A2 RID: 162 RVA: 0x00007C08 File Offset: 0x00005E08
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
