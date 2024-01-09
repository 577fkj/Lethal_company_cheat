using System;
using GameNetcodeStuff;
using HarmonyLib;
using Pautils;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectApparatus
{
	// Token: 0x02000020 RID: 32
	public static class Features
	{
		// Token: 0x0200002B RID: 43
		public class Thirdperson : MonoBehaviour
		{
			// Token: 0x060000BE RID: 190 RVA: 0x000093A4 File Offset: 0x000075A4
			public void Start()
			{
				Features.Thirdperson.ThirdpersonCamera thirdpersonCamera = base.gameObject.AddComponent<Features.Thirdperson.ThirdpersonCamera>();
				thirdpersonCamera.hideFlags = HideFlags.HideAndDontSave;
				UnityEngine.Object.DontDestroyOnLoad(thirdpersonCamera);
			}

			// Token: 0x040000C0 RID: 192
			private static bool _previousState;

			// Token: 0x02000031 RID: 49
			[HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
			public class QuickMenuManager_OpenQuickMenu_Patch
			{
				// Token: 0x060000D8 RID: 216 RVA: 0x00009A5C File Offset: 0x00007C5C
				public static void Prefix()
				{
					Features.Thirdperson._previousState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					bool viewState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					if (viewState)
					{
						GameObjectManager.instance.localPlayer.quickMenuManager.isMenuOpen = false;
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000032 RID: 50
			[HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
			public class QuickMenuManager_CloseQuickMenu_Patch
			{
				// Token: 0x060000DA RID: 218 RVA: 0x00009AA4 File Offset: 0x00007CA4
				public static void Prefix()
				{
					bool previousState = Features.Thirdperson._previousState;
					if (previousState)
					{
						GameObjectManager.instance.localPlayer.inTerminalMenu = false;
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000033 RID: 51
			[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
			public class Terminal_BeginUsingTerminal_Patch
			{
				// Token: 0x060000DC RID: 220 RVA: 0x00009ADC File Offset: 0x00007CDC
				public static void Prefix()
				{
					Features.Thirdperson._previousState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					bool viewState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					if (viewState)
					{
						GameObjectManager.instance.localPlayer.quickMenuManager.isMenuOpen = false;
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000034 RID: 52
			[HarmonyPatch(typeof(Terminal), "QuitTerminal")]
			public class Terminal_QuitTerminal_Patch
			{
				// Token: 0x060000DE RID: 222 RVA: 0x00009B24 File Offset: 0x00007D24
				public static void Prefix()
				{
					bool previousState = Features.Thirdperson._previousState;
					if (previousState)
					{
						GameObjectManager.instance.localPlayer.inTerminalMenu = false;
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000035 RID: 53
			[HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
			public class PlayerControllerB_KillPlayer_Patch
			{
				// Token: 0x060000E0 RID: 224 RVA: 0x00009B5C File Offset: 0x00007D5C
				public static void Prefix()
				{
					bool viewState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					if (viewState)
					{
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000036 RID: 54
			public class ThirdpersonCamera : MonoBehaviour
			{
				// Token: 0x060000E2 RID: 226 RVA: 0x00009B84 File Offset: 0x00007D84
				private void Awake()
				{
					Features.Thirdperson.ThirdpersonCamera._camera = base.gameObject.AddComponent<Camera>();
					Features.Thirdperson.ThirdpersonCamera._camera.hideFlags = HideFlags.HideAndDontSave;
					Features.Thirdperson.ThirdpersonCamera._camera.fieldOfView = 66f;
					Features.Thirdperson.ThirdpersonCamera._camera.nearClipPlane = 0.1f;
					Features.Thirdperson.ThirdpersonCamera._camera.cullingMask = 557520895;
					Features.Thirdperson.ThirdpersonCamera._camera.enabled = false;
					UnityEngine.Object.DontDestroyOnLoad(Features.Thirdperson.ThirdpersonCamera._camera);
				}

				// Token: 0x060000E3 RID: 227 RVA: 0x00009BF8 File Offset: 0x00007DF8
				private void Update()
				{
					bool flag = GameObjectManager.instance.localPlayer == null || GameObjectManager.instance.localPlayer.quickMenuManager.isMenuOpen || GameObjectManager.instance.localPlayer.inTerminalMenu || GameObjectManager.instance.localPlayer.isPlayerDead;
					if (!flag)
					{
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}

				// Token: 0x060000E4 RID: 228 RVA: 0x00009C60 File Offset: 0x00007E60
				public static void Toggle()
				{
					bool flag = GameObjectManager.instance.localPlayer == null || GameObjectManager.instance.localPlayer.isTypingChat || GameObjectManager.instance.localPlayer.quickMenuManager.isMenuOpen || GameObjectManager.instance.localPlayer.inTerminalMenu || GameObjectManager.instance.localPlayer.isPlayerDead;
					if (!flag)
					{
						Features.Thirdperson.ThirdpersonCamera.ViewState = !Features.Thirdperson.ThirdpersonCamera.ViewState;
						GameObject gameObject = GameObject.Find("Systems/UI/Canvas/Panel/");
						Canvas component = GameObject.Find("Systems/UI/Canvas/").GetComponent<Canvas>();
						bool viewState = Features.Thirdperson.ThirdpersonCamera.ViewState;
						if (viewState)
						{
							GameObjectManager.instance.localPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.On;
							gameObject.SetActive(false);
							component.worldCamera = Features.Thirdperson.ThirdpersonCamera._camera;
							component.renderMode = RenderMode.ScreenSpaceOverlay;
							GameObjectManager.instance.localVisor.SetActive(false);
							GameObjectManager.instance.localPlayer.thisPlayerModelArms.enabled = false;
							GameObjectManager.instance.localPlayer.gameplayCamera.enabled = false;
							Features.Thirdperson.ThirdpersonCamera._camera.enabled = true;
						}
						else
						{
							GameObjectManager.instance.localPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
							gameObject.SetActive(true);
							component.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
							component.renderMode = RenderMode.ScreenSpaceCamera;
							GameObjectManager.instance.localPlayer.thisPlayerModelArms.enabled = (Features.Possession.possessedEnemy == null);
							GameObjectManager.instance.localPlayer.gameplayCamera.enabled = true;
							Features.Thirdperson.ThirdpersonCamera._camera.enabled = false;
						}
					}
				}

				// Token: 0x040000D1 RID: 209
				private static Camera _camera;

				// Token: 0x040000D2 RID: 210
				public static bool ViewState;
			}
		}

		// Token: 0x0200002C RID: 44
		public static class Possession
		{
			// Token: 0x060000C0 RID: 192 RVA: 0x000093D8 File Offset: 0x000075D8
			public static void StartPossession()
			{
				PlayerControllerB localPlayer = GameObjectManager.instance.localPlayer;
				bool flag = !localPlayer || localPlayer.isPlayerDead;
				if (!flag)
				{
					float num = float.MaxValue;
					EnemyAI x = null;
					foreach (EnemyAI enemyAI in GameObjectManager.Instance.enemies)
					{
						bool flag2 = !(enemyAI == Features.Possession.lastpossessedEnemy) && !enemyAI.isEnemyDead;
						if (flag2)
						{
							float distance = PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.transform.position, enemyAI.transform.position);
							bool flag3 = distance < num;
							if (flag3)
							{
								num = distance;
								x = enemyAI;
							}
						}
					}
					bool flag4 = x != null;
					if (flag4)
					{
						Features.Possession.StopPossession();
						Features.Possession.possessedEnemy = x;
						Features.Possession.beginPossession = true;
					}
				}
			}

			// Token: 0x060000C1 RID: 193 RVA: 0x000094DC File Offset: 0x000076DC
			public static void StopPossession()
			{
				PlayerControllerB localPlayer = GameObjectManager.instance.localPlayer;
				bool flag = localPlayer && !localPlayer.isPlayerDead;
				if (flag)
				{
					localPlayer.DisablePlayerModel(localPlayer.playersManager.allPlayerObjects[(int)((IntPtr)(checked((long)localPlayer.playerClientId)))], true, false);
					localPlayer.thisPlayerModelArms.enabled = true;
				}
				bool flag2 = Features.Possession.lastpossessedEnemy != null;
				if (flag2)
				{
					Collider[] componentsInChildren = Features.Possession.lastpossessedEnemy.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = true;
					}
					Features.Possession.lastpossessedEnemy.ChangeEnemyOwnerServerRpc(0UL);
					Features.Possession.lastpossessedEnemy.updatePositionThreshold = 1f;
					Features.Possession.lastpossessedEnemy.moveTowardsDestination = true;
				}
				Features.Possession.possessedEnemy = null;
				Features.Possession.lastpossessedEnemy = null;
			}

			// Token: 0x060000C2 RID: 194 RVA: 0x000095B8 File Offset: 0x000077B8
			public static void UpdatePossession()
			{
				bool flag = Features.Possession.possessedEnemy;
				if (flag)
				{
					PlayerControllerB localPlayer = GameObjectManager.instance.localPlayer;
					bool flag2 = !localPlayer || localPlayer.isPlayerDead;
					if (flag2)
					{
						Features.Possession.StopPossession();
					}
					else
					{
						bool isEnemyDead = Features.Possession.possessedEnemy.isEnemyDead;
						if (isEnemyDead)
						{
							Features.Possession.StopPossession();
						}
						else
						{
							bool flag3 = Features.Possession.beginPossession;
							if (flag3)
							{
								localPlayer.DisablePlayerModel(localPlayer.playersManager.allPlayerObjects[(int)((IntPtr)(checked((long)localPlayer.playerClientId)))], false, true);
								GameObjectManager.instance.localPlayer.TeleportPlayer(Features.Possession.possessedEnemy.transform.position, false, 0f, false, true);
								Features.Possession.beginPossession = false;
							}
							Collider[] componentsInChildren = Features.Possession.possessedEnemy.GetComponentsInChildren<Collider>();
							for (int i = 0; i < componentsInChildren.Length; i++)
							{
								componentsInChildren[i].enabled = false;
							}
							Features.Possession.possessedEnemy.ChangeEnemyOwnerServerRpc(GameObjectManager.Instance.localPlayer.actualClientId);
							Features.Possession.possessedEnemy.updatePositionThreshold = 0f;
							Features.Possession.possessedEnemy.moveTowardsDestination = false;
							Features.Possession.possessedEnemy.transform.eulerAngles = GameObjectManager.instance.localPlayer.transform.eulerAngles;
							Features.Possession.possessedEnemy.transform.position = GameObjectManager.instance.localPlayer.transform.position;
							Features.Possession.lastpossessedEnemy = Features.Possession.possessedEnemy;
						}
					}
				}
			}

			// Token: 0x040000C1 RID: 193
			public static bool beginPossession;

			// Token: 0x040000C2 RID: 194
			public static EnemyAI lastpossessedEnemy;

			// Token: 0x040000C3 RID: 195
			public static EnemyAI possessedEnemy;
		}
	}
}
