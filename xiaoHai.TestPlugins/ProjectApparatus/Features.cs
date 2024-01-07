using System;
using GameNetcodeStuff;
using HarmonyLib;
using Pautils;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectApparatus
{
	// Token: 0x0200001D RID: 29
	public static class Features
	{
		// Token: 0x02000027 RID: 39
		public class Thirdperson : MonoBehaviour
		{
			// Token: 0x060000A5 RID: 165 RVA: 0x00007D6C File Offset: 0x00005F6C
			public void Start()
			{
				Features.Thirdperson.ThirdpersonCamera thirdpersonCamera = base.gameObject.AddComponent<Features.Thirdperson.ThirdpersonCamera>();
				thirdpersonCamera.hideFlags = HideFlags.HideAndDontSave;
				UnityEngine.Object.DontDestroyOnLoad(thirdpersonCamera);
			}

			// Token: 0x040000AB RID: 171
			private static bool _previousState;

			// Token: 0x0200002C RID: 44
			[HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
			public class QuickMenuManager_OpenQuickMenu_Patch
			{
				// Token: 0x060000B5 RID: 181 RVA: 0x00008340 File Offset: 0x00006540
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

			// Token: 0x0200002D RID: 45
			[HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
			public class QuickMenuManager_CloseQuickMenu_Patch
			{
				// Token: 0x060000B7 RID: 183 RVA: 0x00008388 File Offset: 0x00006588
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

			// Token: 0x0200002E RID: 46
			[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
			public class Terminal_BeginUsingTerminal_Patch
			{
				// Token: 0x060000B9 RID: 185 RVA: 0x000083C0 File Offset: 0x000065C0
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

			// Token: 0x0200002F RID: 47
			[HarmonyPatch(typeof(Terminal), "QuitTerminal")]
			public class Terminal_QuitTerminal_Patch
			{
				// Token: 0x060000BB RID: 187 RVA: 0x00008408 File Offset: 0x00006608
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

			// Token: 0x02000030 RID: 48
			[HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
			public class PlayerControllerB_KillPlayer_Patch
			{
				// Token: 0x060000BD RID: 189 RVA: 0x00008440 File Offset: 0x00006640
				public static void Prefix()
				{
					bool viewState = Features.Thirdperson.ThirdpersonCamera.ViewState;
					if (viewState)
					{
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}
			}

			// Token: 0x02000031 RID: 49
			public class ThirdpersonCamera : MonoBehaviour
			{
				// Token: 0x060000BF RID: 191 RVA: 0x00008468 File Offset: 0x00006668
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

				// Token: 0x060000C0 RID: 192 RVA: 0x000084DC File Offset: 0x000066DC
				private void Update()
				{
					bool flag = GameObjectManager.instance.localPlayer == null || GameObjectManager.instance.localPlayer.quickMenuManager.isMenuOpen || GameObjectManager.instance.localPlayer.inTerminalMenu || GameObjectManager.instance.localPlayer.isPlayerDead;
					if (!flag)
					{
						Features.Thirdperson.ThirdpersonCamera.Toggle();
					}
				}

				// Token: 0x060000C1 RID: 193 RVA: 0x00008544 File Offset: 0x00006744
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

				// Token: 0x040000B5 RID: 181
				private static Camera _camera;

				// Token: 0x040000B6 RID: 182
				public static bool ViewState;
			}
		}

		// Token: 0x02000028 RID: 40
		public static class Possession
		{
			// Token: 0x060000A7 RID: 167 RVA: 0x00007DA0 File Offset: 0x00005FA0
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

			// Token: 0x060000A8 RID: 168 RVA: 0x00007EA4 File Offset: 0x000060A4
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

			// Token: 0x060000A9 RID: 169 RVA: 0x00007F80 File Offset: 0x00006180
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

			// Token: 0x040000AC RID: 172
			public static bool beginPossession;

			// Token: 0x040000AD RID: 173
			public static EnemyAI lastpossessedEnemy;

			// Token: 0x040000AE RID: 174
			public static EnemyAI possessedEnemy;
		}
	}
}
