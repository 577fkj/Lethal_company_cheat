using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using Editplayer.Patches;
using EditshipStrat.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using Pautils;
using ProjectApparatus;
using RENDER;
using render;
using Steamworks;
using Testplugin;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XHGUI
{
	public class Xhgui : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log("小海提醒你:来了老弟");
			Instance = this;
		}

		private void Update()
		{
			timeSinceLastUpdate += Time.deltaTime;
			if (timeSinceLastUpdate >= updateInterval)
			{
				timeSinceLastUpdate = 0f;
				if (editplayer.GetlocalPlayer() != null && editplayer.Instance() != null)
				{
					playerinfo = editplayer.Instance();
					player = editplayer.GetlocalPlayer();
					if (player != null && playerinfo != null)
					{
						localnowX = string.Format("{0:0.00}", player.thisPlayerBody.localPosition.x);
						localnowY = string.Format("{0:0.00}", player.thisPlayerBody.localPosition.z);
						localnowZ = string.Format("{0:0.00}", player.thisPlayerBody.localPosition.y);
						gamestrat = true;
						Features.Possession.UpdatePossession();
						if (LightShow)
						{
							if (GameObjectManager.Instance.shipLights)
							{
								GameObjectManager.Instance.shipLights.SetShipLightsServerRpc(!GameObjectManager.Instance.shipLights.areLightsOn);
							}
						}
						if (first)
						{
							Debug.Log("小海提醒你:Playerinfo和 player 均检测成功" + playerinfo.allowLocalPlayerDeath.ToString() + "  ,," + player.playerUsername);
							HUDManager.Instance.DisplayTip("XiaoHaiPlugin1.0.4小海mod开启成功", "YUAN SHEN QI DONG !YOU CAN PRESS YOUR HOTKEY NOW!", true, false, "LC_EclipseTip");
							str_Quota = TimeOfDay.Instance.profitQuota.ToString();
							str_QuotaFulfilled = TimeOfDay.Instance.quotaFulfilled.ToString();
							base.StartCoroutine(GameObjectManager.Instance.CollectObjects());
							money = GameObjectManager.Instance.shipTerminal.groupCredits.ToString();
							harmony.PatchAll();
							// PAUtils.SendChatMessage("欢迎使用小海mod,请勿在路人房滥用.QQ二群726351737", (int)player.playerClientId);
							customDeadline = (int)TimeOfDay.Instance.timeUntilDeadline / 1080;
							first = false;
						}
					}
				}
			}
			bool wasPressedThisFrame = Keyboard.current[testplugin.Instance1.memuhotkey.Value].wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				if (!testplugin.Instance.JustGO.Value)
				{
					if (!gamestrat)
					{
						Debug.Log("小海提醒你:警告!!!先等开飞船再调出菜单!");
						return;
					}
				}
				HotKeyPressed = !HotKeyPressed;
			}
		}

		public void OnGUI()
		{
			if (DoorESP | ShipESP)
			{
				HandleESP();
			}
			bool hotKeyPressed = HotKeyPressed;
			if (hotKeyPressed)
			{
				rect = GUILayout.Window(1, rect, new GUI.WindowFunction(windowfunc), "主菜单", Array.Empty<GUILayoutOption>());
				Style = new GUIStyle(GUI.skin.label);
				Style.normal.textColor = Color.white;
				Style.fontStyle = FontStyle.Bold;
			}
		}

		private void DisplayObjects<T>(IEnumerable<T> objects, bool shouldDisplay, Func<T, string> labelSelector, Func<T, Color> colorSelector) where T : Component
		{
			if (shouldDisplay)
			{
				foreach (T t in objects)
				{
					if (t != null && t.gameObject.activeSelf)
					{
						float distance = PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position, t.transform.position);
						Vector3 vector;
						if (PAUtils.WorldToScreen(GameObjectManager.Instance.localPlayer.gameplayCamera, t.transform.position, out vector))
						{
							string text = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(t));
							text = text + " [" + distance.ToString().ToUpper() + "M]";
							Render.String(Style, vector.x, vector.y, 150f, 50f, text, colorSelector(t), true, true);
						}
					}
				}
			}
		}

		private void DisplayDoors()
		{
			DisplayObjects<EntranceTeleport>(GameObjectManager.Instance.entranceTeleports, DoorESP, delegate(EntranceTeleport entranceTeleport)
			{
				string result;
				if (!entranceTeleport.isEntranceToBuilding)
				{
					result = "出口";
				}
				else
				{
					result = "入口";
				}
				return result;
			}, (EntranceTeleport _) => c_Door);
		}

		private void DisplayShip()
		{
			DisplayObjects<HangarShipDoor>(new HangarShipDoor[]
			{
				GameObjectManager.Instance.shipDoor
			}, ShipESP, (HangarShipDoor _) => "飞船", (HangarShipDoor _) => c_ship);
		}

		private void NightVision()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.nightVision.enabled = Nightvision;
				localPlayerController.nightVision.intensity = (Nightvision ? 3000f : 360f);
				localPlayerController.nightVision.range = (Nightvision ? 10000f : 12f);
			}
		}

		private void SetJump()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController != null;
			if (!flag)
			{
				localPlayerController.jumpForce = jump;
				jump = localPlayerController.jumpForce;
			}
		}

		private void HandleMovementSpeed()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.movementSpeed = _movementSpeed;
			}
		}

		private void HandleNoClip(bool _setNoClip)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				Transform transform = localPlayerController.gameplayCamera.transform;
				bool flag2 = transform == null;
				if (!flag2)
				{
					Collider component = localPlayerController.GetComponent<CharacterController>();
					bool flag3 = component == null;
					if (!flag3)
					{
						bool flag4 = !_setNoClip;
						if (flag4)
						{
							component.enabled = true;
						}
						else
						{
							component.enabled = false;
							Vector3 a = default(Vector3);
							localPlayerController.twoHanded = false;
							bool key = UnityInput.Current.GetKey(KeyCode.W);
							if (key)
							{
								a += transform.forward;
							}
							bool key2 = UnityInput.Current.GetKey(KeyCode.S);
							if (key2)
							{
								a += transform.forward * -1f;
							}
							bool key3 = UnityInput.Current.GetKey(KeyCode.D);
							if (key3)
							{
								a += transform.right;
							}
							bool key4 = UnityInput.Current.GetKey(KeyCode.A);
							if (key4)
							{
								a += transform.right * -1f;
							}
							bool key5 = UnityInput.Current.GetKey(KeyCode.Space);
							if (key5)
							{
								a.y += transform.up.y;
							}
							bool key6 = UnityInput.Current.GetKey(KeyCode.LeftControl);
							if (key6)
							{
								a.y += transform.up.y * -1f;
							}
							Vector3 localPosition = localPlayerController.transform.localPosition;
							bool flag5 = localPosition.Equals(Vector3.zero);
							if (!flag5)
							{
								Vector3 localPosition2 = localPosition + a * (_noClipSpeed * Time.deltaTime);
								localPlayerController.transform.localPosition = localPosition2;
							}
						}
					}
				}
			}
		}

		private void HandleNoFog(bool enable)
		{
			GameObject gameObject = GameObject.Find("Systems");
			bool flag = gameObject == null;
			if (!flag)
			{
				gameObject.transform.Find("Rendering").Find("VolumeMain").gameObject.SetActive(!enable);
				RenderSettings.fog = enable;
			}
		}

		private void alwaysfullyCharge()
		{
			bool flag = !AlwaysFullyCharge;
			if (!flag)
			{
				GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
				bool flag2 = array == null || array.Length == 0;
				if (!flag2)
				{
					foreach (GrabbableObject grabbableObject in array)
					{
						bool flag3 = !(grabbableObject == null);
						if (flag3)
						{
							PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
							bool flag4 = !(localPlayerController == null) && grabbableObject.itemProperties.requiresBattery && !(grabbableObject.playerHeldBy != localPlayerController);
							if (flag4)
							{
								grabbableObject.insertedBattery.empty = false;
								grabbableObject.insertedBattery.charge = 1f;
							}
						}
					}
				}
			}
		}

		private void DrawScrapTable()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("名字", new GUILayoutOption[]
			{
				GUILayout.MinWidth(60f)
			});
			GUILayout.Label("距离(m)", new GUILayoutOption[]
			{
				GUILayout.MinWidth(30f)
			});
			GUILayout.Label("价值", new GUILayoutOption[]
			{
				GUILayout.MinWidth(30f)
			});
			GUILayout.Label("操作", new GUILayoutOption[]
			{
				GUILayout.MinWidth(30f)
			});
			GUILayout.EndHorizontal();
			GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
			bool flag = array == null || array.Length == 0;
			if (!flag)
			{
				foreach (GrabbableObject grabbableObject in array)
				{
					bool flag2 = !(grabbableObject == null) && grabbableObject.itemProperties.isScrap && grabbableObject.grabbable && !grabbableObject.isHeld && (grabbableObject == null || !grabbableObject.isInShipRoom || !grabbableObject.isInElevator);
					if (flag2)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						bool flag3 = !(localPlayerController == null);
						if (flag3)
						{
							ScanNodeProperties componentInChildren = grabbableObject.GetComponentInChildren<ScanNodeProperties>();
							bool flag4 = !(componentInChildren == null);
							if (flag4)
							{
								string headerText = componentInChildren.headerText;
								float num = Vector3.Distance(grabbableObject.transform.position, localPlayerController.transform.position);
								int scrapValue = grabbableObject.scrapValue;
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								GUILayout.Label(headerText, new GUILayoutOption[]
								{
									GUILayout.MinWidth(120f)
								});
								GUILayout.Label(num.ToString("F2"), new GUILayoutOption[]
								{
									GUILayout.MinWidth(50f)
								});
								GUILayout.Label(scrapValue.ToString(), new GUILayoutOption[]
								{
									GUILayout.MinWidth(30f)
								});
								bool flag5 = GUILayout.Button("T", Array.Empty<GUILayoutOption>());
								if (flag5)
								{
									TeleportPlayer(grabbableObject.transform.position);
								}
								bool flag6 = GUILayout.Button("+", Array.Empty<GUILayoutOption>());
								if (flag6)
								{
									grabbableObject.SetScrapValue(scrapValue + 1);
								}
								bool flag7 = GUILayout.Button("-", Array.Empty<GUILayoutOption>());
								if (flag7)
								{
									grabbableObject.SetScrapValue(scrapValue - 1);
								}
								bool flag8 = GUILayout.Button("√", Array.Empty<GUILayoutOption>());
								if (flag8)
								{
									grabbableObject.transform.SetPositionAndRotation(localPlayerController.transform.position, grabbableObject.transform.rotation);
									grabbableObject.startFallingPosition = localPlayerController.transform.position;
									grabbableObject.targetFloorPosition = localPlayerController.transform.position;
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		private static void TeleportPlayer(Vector3 position)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.TeleportPlayer(position, false, 0f, false, true);
			}
		}

		private void SetMoney(string Money)
		{
			int groupCredits;
			int.TryParse(Money, out groupCredits);
			GameObjectManager.Instance.shipTerminal.groupCredits = groupCredits;
		}

		private void DrawEnemyTable()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("名字", new GUILayoutOption[]
			{
				GUILayout.MinWidth(60f)
			});
			GUILayout.Label("距离(m)", new GUILayoutOption[]
			{
				GUILayout.MinWidth(20f)
			});
			GUILayout.Label("操作", new GUILayoutOption[]
			{
				GUILayout.MinWidth(50f)
			});
			GUILayout.EndHorizontal();
			EnemyAI[] array = UnityEngine.Object.FindObjectsOfType<EnemyAI>();
			bool flag = array == null;
			if (!flag)
			{
				foreach (EnemyAI enemyAI in array)
				{
					bool flag2 = !(enemyAI == null) && !enemyAI.isEnemyDead;
					if (flag2)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						bool flag3 = !(localPlayerController == null);
						if (flag3)
						{
							ScanNodeProperties componentInChildren = enemyAI.GetComponentInChildren<ScanNodeProperties>();
							string text = componentInChildren ? componentInChildren.headerText : enemyAI.enemyType.enemyName;
							float num = Vector3.Distance(enemyAI.transform.position, localPlayerController.transform.position);
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUILayout.Label(text, new GUILayoutOption[]
							{
								GUILayout.MinWidth(120f)
							});
							GUILayout.Label(num.ToString("F2"), new GUILayoutOption[]
							{
								GUILayout.MinWidth(50f)
							});
							bool flag4 = GUILayout.Button("T", Array.Empty<GUILayoutOption>());
							if (flag4)
							{
								TeleportPlayer(enemyAI.transform.position);
							}
							bool flag5 = GUILayout.Button("死", Array.Empty<GUILayoutOption>());
							if (flag5)
							{
								enemyAI.KillEnemyServerRpc(false);
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}
		}

		private void DrawPlayerTable()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("名字", new GUILayoutOption[]
			{
				GUILayout.MinWidth(60f)
			});
			GUILayout.Label("距离(m)", new GUILayoutOption[]
			{
				GUILayout.MinWidth(20f)
			});
			GUILayout.Label("操作", new GUILayoutOption[]
			{
				GUILayout.MinWidth(30f)
			});
			GUILayout.EndHorizontal();
			GameObject[] allPlayerObjects = StartOfRound.Instance.allPlayerObjects;
			bool flag = allPlayerObjects == null;
			if (!flag)
			{
				GameObject[] array = allPlayerObjects;
				for (int i = 0; i < array.Length; i++)
				{
					PlayerControllerB component = array[i].GetComponent<PlayerControllerB>();
					bool flag2 = !(component == null) && !component.isPlayerDead && !component.IsOwner;
					if (flag2)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						bool flag3 = !(localPlayerController == null);
						if (flag3)
						{
							string playerUsername = component.playerUsername;
							float num = Vector3.Distance(component.transform.position, localPlayerController.transform.position);
							bool flag4 = !playerUsername.Contains("Player #");
							if (flag4)
							{
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								GUILayout.Label(playerUsername, new GUILayoutOption[]
								{
									GUILayout.MinWidth(120f)
								});
								GUILayout.Label(num.ToString("F2"), new GUILayoutOption[]
								{
									GUILayout.MinWidth(50f)
								});
								bool flag5 = GUILayout.Button("T", Array.Empty<GUILayoutOption>());
								if (flag5)
								{
									TeleportPlayer(component.transform.position);
								}
								bool flag6 = GUILayout.Button("死", Array.Empty<GUILayoutOption>());
								if (flag6)
								{
									component.DamagePlayerFromOtherClientServerRpc(component.health, Vector3.zero, (int)component.playerClientId);
								}
								bool flag7 = GUILayout.Button("+", Array.Empty<GUILayoutOption>());
								bool flag8 = flag7;
								if (flag8)
								{
									SteamFriends.OpenUserOverlay(component.playerSteamId, "steamid");
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		private void ReviveLocalPlayer()
		{
			PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
			StartOfRound.Instance.allPlayersDead = false;
			localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
			bool flag = localPlayer.isPlayerDead || localPlayer.isPlayerControlled;
			bool flag2 = flag;
			if (flag2)
			{
				localPlayer.isClimbingLadder = false;
				localPlayer.ResetZAndXRotation();
				localPlayer.thisController.enabled = true;
				localPlayer.health = 100;
				localPlayer.disableLookInput = false;
				bool isPlayerDead = localPlayer.isPlayerDead;
				bool flag3 = isPlayerDead;
				checked
				{
					if (flag3)
					{
						localPlayer.isPlayerDead = false;
						localPlayer.isPlayerControlled = true;
						localPlayer.isInElevator = true;
						localPlayer.isInHangarShipRoom = true;
						localPlayer.isInsideFactory = false;
						localPlayer.wasInElevatorLastFrame = false;
						StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
						localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position, false, 0f, false, true);
						localPlayer.setPositionOfDeadPlayer = false;
						localPlayer.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[(int)((IntPtr)((long)localPlayer.playerClientId))], true, true);
						localPlayer.helmetLight.enabled = false;
						localPlayer.Crouch(false);
						localPlayer.criticallyInjured = false;
						bool flag4 = localPlayer.playerBodyAnimator != null;
						bool flag5 = flag4;
						if (flag5)
						{
							localPlayer.playerBodyAnimator.SetBool("Limp", false);
						}
						localPlayer.bleedingHeavily = false;
						localPlayer.activatingItem = false;
						localPlayer.twoHanded = false;
						localPlayer.inSpecialInteractAnimation = false;
						localPlayer.disableSyncInAnimation = false;
						localPlayer.inAnimationWithEnemy = null;
						localPlayer.holdingWalkieTalkie = false;
						localPlayer.speakingToWalkieTalkie = false;
						localPlayer.isSinking = false;
						localPlayer.isUnderwater = false;
						localPlayer.sinkingValue = 0f;
						localPlayer.statusEffectAudio.Stop();
						localPlayer.DisableJetpackControlsLocally();
						localPlayer.health = 100;
						localPlayer.mapRadarDotAnimator.SetBool("dead", false);
						bool isOwner = localPlayer.IsOwner;
						bool flag6 = isOwner;
						if (flag6)
						{
							HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
							localPlayer.hasBegunSpectating = false;
							HUDManager.Instance.RemoveSpectateUI();
							HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
							localPlayer.hinderedMultiplier = 1f;
							localPlayer.isMovementHindered = 0;
							localPlayer.sourcesCausingSinking = 0;
							localPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
						}
					}
					SoundManager.Instance.earsRingingTimer = 0f;
					localPlayer.voiceMuffledByEnemy = false;
					SoundManager.Instance.playerVoicePitchTargets[(int)((IntPtr)((long)localPlayer.playerClientId))] = 1f;
				}
				SoundManager.Instance.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
				bool flag7 = localPlayer.currentVoiceChatIngameSettings == null;
				bool flag8 = flag7;
				if (flag8)
				{
					StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
				}
				bool flag9 = localPlayer.currentVoiceChatIngameSettings != null;
				bool flag10 = flag9;
				if (flag10)
				{
					bool flag11 = localPlayer.currentVoiceChatIngameSettings.voiceAudio == null;
					bool flag12 = flag11;
					if (flag12)
					{
						localPlayer.currentVoiceChatIngameSettings.InitializeComponents();
					}
					bool flag13 = localPlayer.currentVoiceChatIngameSettings.voiceAudio == null;
					bool flag14 = flag13;
					if (flag14)
					{
						return;
					}
					localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
				}
			}
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			localPlayerController.bleedingHeavily = false;
			localPlayerController.criticallyInjured = false;
			localPlayerController.playerBodyAnimator.SetBool("Limp", false);
			localPlayerController.health = 100;
			HUDManager.Instance.UpdateHealthUI(100, false);
			localPlayerController.spectatedPlayerScript = null;
			HUDManager.Instance.audioListenerLowPass.enabled = false;
			StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, localPlayerController);
			RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag15 = !array[i].isHeld;
				bool flag16 = flag15;
				if (flag16)
				{
					bool isServer = StartOfRound.Instance.IsServer;
					bool flag17 = isServer;
					if (flag17)
					{
						bool isSpawned = array[i].NetworkObject.IsSpawned;
						bool flag18 = isSpawned;
						if (flag18)
						{
							array[i].NetworkObject.Despawn(true);
						}
						else
						{
							UnityEngine.Object.Destroy(array[i].gameObject);
						}
					}
				}
				else
				{
					bool flag19 = array[i].isHeld && array[i].playerHeldBy != null;
					bool flag20 = flag19;
					if (flag20)
					{
						array[i].playerHeldBy.DropAllHeldItems(true, false);
					}
				}
			}
			DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
			for (int j = 0; j < array2.Length; j++)
			{
				UnityEngine.Object.Destroy(array2[j].gameObject);
			}
			StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
			StartOfRound.Instance.allPlayersDead = false;
			StartOfRound.Instance.UpdatePlayerVoiceEffects();
			StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
		}

		private void ESPEnemy(PlayerControllerB player, Camera camera)
		{
			EnemyAI[] array = UnityEngine.Object.FindObjectsOfType<EnemyAI>();
			bool flag = array == null;
			if (!flag)
			{
				foreach (EnemyAI enemyAI in array)
				{
					bool flag2 = !(enemyAI == null) && !enemyAI.isEnemyDead;
					if (flag2)
					{
						Vector3 position = enemyAI.transform.position;
						Vector3 vector = camera.WorldToScreenPoint(position);
						bool flag3 = IsOnScreen(vector, camera);
						if (flag3)
						{
							ScanNodeProperties componentInChildren = enemyAI.GetComponentInChildren<ScanNodeProperties>();
							string str = componentInChildren ? componentInChildren.headerText : enemyAI.enemyType.enemyName;
							float num = Vector3.Distance(player.transform.position, enemyAI.transform.position);
							Vector2 vector2 = new Vector2(vector.x * ScreenScale.x, (float)Screen.height - vector.y * ScreenScale.y);
							Color color = _setESPColor ? Color.red : Color.white;
							string text = "";
							bool drawName = _drawName;
							if (drawName)
							{
								text += str;
							}
							bool drawDistance = _drawDistance;
							if (drawDistance)
							{
								text = text + " | " + num.ToString("F1") + "m";
							}
							bool flag4 = text != "";
							if (flag4)
							{
								RenDer.DrawString(new Vector2(vector2.x, vector2.y - 20f), text, true);
							}
							bool drawLine = _drawLine;
							if (drawLine)
							{
								RenDer.DrawLine(ScreenCenter, vector2, color, 2f);
							}
						}
					}
				}
			}
		}

		private bool IsOnScreen(Vector3 position, Camera camera)
		{
			return position.x >= 0f && position.x <= (float)camera.pixelWidth && position.y >= 0f && position.y <= (float)camera.pixelHeight && position.z >= 0f;
		}

		private void HandleESP()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				Camera camera = localPlayerController.isPlayerDead ? localPlayerController.playersManager.spectateCamera : localPlayerController.gameplayCamera;
				bool flag2 = camera == null;
				if (!flag2)
				{
					ScreenScale = new Vector2((float)Screen.width / (float)camera.pixelWidth, (float)Screen.height / (float)camera.pixelHeight);
					ScreenCenter = new Vector2((float)(Screen.width / 2), (float)(Screen.height - 1));
					bool setESPEnemy = _setESPEnemy;
					if (setESPEnemy)
					{
						ESPEnemy(localPlayerController, camera);
					}
					bool doorESP = DoorESP;
					if (doorESP)
					{
						DisplayDoors();
					}
					bool shipESP = ShipESP;
					if (shipESP)
					{
						DisplayShip();
					}
				}
			}
		}

		internal static void SpawnEnemyfunc(string enemyName, bool random, int num)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				Vector3 position = localPlayerController.transform.position;
				foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in currentLevel.Enemies)
				{
					bool flag2 = spawnableEnemyWithRarity.enemyType.enemyName.ToLower().Contains(enemyName.ToLower());
					bool flag3 = flag2;
					if (flag3)
					{
						try
						{
							string enemyName2 = spawnableEnemyWithRarity.enemyType.enemyName;
							if (random)
							{
								SpawnEnemy(spawnableEnemyWithRarity, num, true, new Vector3(0f, 0f, 0f));
							}
							else
							{
								SpawnEnemy(spawnableEnemyWithRarity, num, true, position);
							}
							Debug.Log("小海提醒你:Spawned " + spawnableEnemyWithRarity.enemyType.enemyName);
						}
						catch
						{
							Debug.Log("小海提醒你:Could not spawn enemy");
						}
						Debug.Log("Spawned: " + spawnableEnemyWithRarity.enemyType.enemyName);
						break;
					}
				}
			}
		}

		internal static void SpawnEnemy(SpawnableEnemyWithRarity enemy, int amount, bool inside, Vector3 location)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				bool flag2 = !localPlayerController.isHostPlayerObject;
				bool flag3 = !flag2;
				if (flag3)
				{
					bool flag4 = location.x != 0f && location.y != 0f && location.z != 0f && inside;
					bool flag5 = flag4;
					if (flag5)
					{
						try
						{
							for (int i = 0; i < amount; i++)
							{
								currentRound.SpawnEnemyOnServer(location, 0f, currentLevel.Enemies.IndexOf(enemy));
							}
							return;
						}
						catch
						{
							return;
						}
					}
					bool flag6 = location.x != 0f && location.y != 0f && location.z != 0f && !inside;
					bool flag7 = flag6;
					if (flag7)
					{
						try
						{
							int j;
							for (j = 0; j < amount; j++)
							{
								UnityEngine.Object.Instantiate<GameObject>(currentLevel.OutsideEnemies[currentLevel.OutsideEnemies.IndexOf(enemy)].enemyType.enemyPrefab, location, Quaternion.Euler(Vector3.zero)).gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
							}
							string str = "Spawned an enemy. Total Spawned: ";
							string str2 = j.ToString();
							string str3 = "at position:";
							Debug.Log(str + str2 + str3);
							return;
						}
						catch
						{
							return;
						}
					}
					if (inside)
					{
						try
						{
							int k;
							for (k = 0; k < amount; k++)
							{
								currentRound.SpawnEnemyOnServer(currentRound.allEnemyVents[UnityEngine.Random.Range(0, currentRound.allEnemyVents.Length)].floorNode.position, currentRound.allEnemyVents[k].floorNode.eulerAngles.y, currentLevel.Enemies.IndexOf(enemy));
							}
							Debug.Log(string.Format("You wanted to spawn: {0} enemies", amount));
							Debug.Log("小海提醒你:Total Spawned: " + k.ToString());
							return;
						}
						catch
						{
							Debug.Log("小海提醒你:Failed to spawn enemies, check your command.");
							return;
						}
					}
					int l;
					for (l = 0; l < amount; l++)
					{
						UnityEngine.Object.Instantiate<GameObject>(currentLevel.OutsideEnemies[currentLevel.OutsideEnemies.IndexOf(enemy)].enemyType.enemyPrefab, GameObject.FindGameObjectsWithTag("OutsideAINode")[UnityEngine.Random.Range(0, GameObject.FindGameObjectsWithTag("OutsideAINode").Length - 1)].transform.position, Quaternion.Euler(Vector3.zero)).gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
					}
					Debug.Log(string.Format("You wanted to spawn: {0} enemies", amount));
					Debug.Log("小海提醒你:Total Spawned: " + l.ToString());
				}
			}
		}

		[HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
		[HarmonyPostfix]
		private static void UpdateNewInfo(ref EnemyVent[] ___allEnemyVents, ref SelectableLevel ___currentLevel)
		{
			currentLevel = ___currentLevel;
			currentLevelVents = ___allEnemyVents;
		}

		[HarmonyPatch(typeof(RoundManager), "AdvanceHourAndSpawnNewBatchOfEnemies")]
		[HarmonyPrefix]
		private static void UpdateCurrentLevelInfo(ref EnemyVent[] ___allEnemyVents, ref SelectableLevel ___currentLevel)
		{
			currentLevel = ___currentLevel;
			currentLevelVents = ___allEnemyVents;
		}

		[HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
		[HarmonyPostfix]
		private static void PatchDeadline(TimeOfDay __instance)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				bool flag2 = localPlayerController.IsHost && Instance.customDeadline != int.MinValue;
				bool flag3 = flag2;
				if (flag3)
				{
					__instance.quotaVariables.deadlineDaysAmount = Instance.customDeadline;
					Debug.Log("小海提醒你:Instance.customDeadline" + __instance.quotaVariables.deadlineDaysAmount.ToString());
					__instance.timeUntilDeadline = (float)(__instance.quotaVariables.deadlineDaysAmount + Instance.customDeadline) * __instance.totalTime;
					TimeOfDay.Instance.timeUntilDeadline = (float)((int)(TimeOfDay.Instance.totalTime * (float)TimeOfDay.Instance.quotaVariables.deadlineDaysAmount));
					TimeOfDay.Instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);
					StartOfRound.Instance.deadlineMonitorText.text = "剩余天数:\n " + TimeOfDay.Instance.daysUntilDeadline.ToString();
					Debug.Log(StartOfRound.Instance.deadlineMonitorText.text);
				}
			}
		}

		private void windowfunc(int windowID)
		{
			toolbarInt = GUI.Toolbar(new Rect(25f, 25f, 350f, 30f), toolbarInt, toolbarStrings);
			bool flag = toolbarInt == 0;
			if (flag)
			{
				UI.Reset();
				maniwindowfnc();
				Vector2 vector = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
			}
			bool flag2 = toolbarInt == 1;
			if (flag2)
			{
				tpwindowfnc();
			}
			bool flag3 = toolbarInt == 2;
			if (flag3)
			{
				freetpwindowfnc();
			}
			GUI.DragWindow();
		}

		private void maniwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(30f);
			GUILayout.Label("欢迎使用小海MOD test1.0.4", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			isWUDISelected = GUILayout.Toggle(isWUDISelected, "是否开启无敌", Toggleoptions);
			bool flag = isWUDISelected;
			if (flag)
			{
				bool flag2 = playerinfo != null && player != null;
				if (flag2)
				{
					playerinfo.allowLocalPlayerDeath = false;
					StartOfRound.Instance.allowLocalPlayerDeath = false;
					player.AllowPlayerDeath();
					bool flag3 = once;
					if (flag3)
					{
						HUDManager.Instance.DisplayTip("GOD MODE !", "YOU ARE GOD NOW! JUST DO IT!----XiaoHai!", true, false, "LC_EclipseTip");
						once = false;
					}
				}
			}
			else
			{
				playerinfo.allowLocalPlayerDeath = true;
				StartOfRound.Instance.allowLocalPlayerDeath = true;
				player.AllowPlayerDeath();
			}
			allowTIMESTOP = GUILayout.Toggle(allowTIMESTOP, "是否开启时停", Toggleoptions);
			bool flag4 = playerinfo != null;
			if (flag4)
			{
				bool flag5 = allowTIMESTOP;
				if (flag5)
				{
					StopTime.SetPlayerResurrectedManually(true);
				}
				else
				{
					StopTime.SetPlayerResurrectedManually(false);
				}
			}
			Nightvision = GUILayout.Toggle(Nightvision, "是否开启夜视仪", Toggleoptions);
			bool flag6 = playerinfo != null;
			if (flag6)
			{
				NightVision();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			allowStopSprintMeter = GUILayout.Toggle(allowStopSprintMeter, "是否无限耐力", Toggleoptions);
			bool flag7 = playerinfo != null;
			if (flag7)
			{
				bool flag8 = allowStopSprintMeter;
				if (flag8)
				{
					StopSprintMeter.isStopspringntMeter(true);
				}
				else
				{
					StopSprintMeter.isStopspringntMeter(false);
				}
			}
			AlwaysFullyCharge = GUILayout.Toggle(AlwaysFullyCharge, "是否永远满电", Toggleoptions);
			alwaysfullyCharge();
			noclip = GUILayout.Toggle(noclip, "飞升模式", Toggleoptions);
			HandleNoClip(noclip);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			NoInvisible = GUILayout.Toggle(NoInvisible, "是否对怪物隐身(无效)", Toggleoptions);
			NoFog = GUILayout.Toggle(NoFog, "是否开启无迷雾", Toggleoptions);
			HandleNoFog(NoFog);
			jishihudong = GUILayout.Toggle(jishihudong, "是否开启瞬间开门", Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			PlaceAnywhere = GUILayout.Toggle(PlaceAnywhere, "是否随意摆放家具", Toggleoptions);
			noneedtwohands = GUILayout.Toggle(noneedtwohands, "是否取消双手持有", Toggleoptions);
			bool flag9 = playerinfo != null;
			if (flag9)
			{
				StopSprintMeter.isNoNeedTwoHands(noneedtwohands);
			}
			InfiniteZapGun = GUILayout.Toggle(InfiniteZapGun, "电击枪无限控制", Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			EmoteWalk = GUILayout.Toggle(EmoteWalk, "表情时可移动", Toggleoptions);
			shotgunammo = GUILayout.Toggle(shotgunammo, "是否无限弹药", Toggleoptions);
			LightShow = GUILayout.Toggle(LightShow, "灯光秀", Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			HearEveryone = GUILayout.Toggle(HearEveryone, "顺风耳", Toggleoptions);
			DoorESP = GUILayout.Toggle(DoorESP, "透视大门", Toggleoptions);
			ShipESP = GUILayout.Toggle(ShipESP, "透视飞船", Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			ctrlenemy = GUILayout.Toggle(ctrlenemy, "控制最近的怪物(制作中)", Array.Empty<GUILayoutOption>());
			bool flag10 = ctrlenemy;
			if (flag10)
			{
				Features.Possession.StartPossession();
			}
			else
			{
				Features.Possession.StopPossession();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(30f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("移速:" + _movementSpeed.ToString(), Array.Empty<GUILayoutOption>());
			movementSpeedInput = GUILayout.TextField(movementSpeedInput, Array.Empty<GUILayoutOption>());
			float movementSpeed;
			bool flag11 = float.TryParse(movementSpeedInput, out movementSpeed);
			if (flag11)
			{
				GUI.FocusControl(null);
				_movementSpeed = movementSpeed;
			}
			bool flag12 = GUILayout.Button("修改移动速度", Array.Empty<GUILayoutOption>());
			if (flag12)
			{
				GUI.FocusControl(null);
				HandleMovementSpeed();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("金钱:" + money, Array.Empty<GUILayoutOption>());
			money = GUILayout.TextField(money, Array.Empty<GUILayoutOption>());
			bool flag13 = GUILayout.Button("修改金钱", Array.Empty<GUILayoutOption>());
			if (flag13)
			{
				SetMoney(money);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("剩余天数:" + customDeadline.ToString(), Array.Empty<GUILayoutOption>());
			customDeadlineinp = GUILayout.TextField(customDeadlineinp, Array.Empty<GUILayoutOption>());
			bool flag14 = GUILayout.Button("修改天数(次日生效)", Array.Empty<GUILayoutOption>());
			if (flag14)
			{
				GUI.FocusControl(null);
				int num;
				bool flag15 = int.TryParse(customDeadlineinp, out num);
				if (flag15)
				{
					customDeadline = num;
					TimeOfDay.Instance.timeUntilDeadline = (float)(customDeadline * 1080);
					Instance.customDeadline = (int)TimeOfDay.Instance.timeUntilDeadline / 1080;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("利润配额:" + str_QuotaFulfilled + "/" + str_Quota, Array.Empty<GUILayoutOption>());
			str_QuotaFulfilled = GUILayout.TextField(str_QuotaFulfilled, Array.Empty<GUILayoutOption>());
			GUILayout.Label("/", Array.Empty<GUILayoutOption>());
			str_Quota = GUILayout.TextField(str_Quota, Array.Empty<GUILayoutOption>());
			bool flag16 = GUILayout.Button("修改", Array.Empty<GUILayoutOption>());
			if (flag16)
			{
				GUI.FocusControl(null);
				bool flag17 = TimeOfDay.Instance;
				if (flag17)
				{
					TimeOfDay.Instance.profitQuota = int.Parse(str_Quota);
					TimeOfDay.Instance.quotaFulfilled = int.Parse(str_QuotaFulfilled);
					TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("弹跳:" + jump.ToString(), Array.Empty<GUILayoutOption>());
			jumpinput = GUILayout.TextField(jumpinput, Array.Empty<GUILayoutOption>());
			float num2;
			bool flag18 = float.TryParse(jumpinput, out num2);
			if (flag18)
			{
				jump = num2;
			}
			bool flag19 = GUILayout.Button("修改弹跳力", Array.Empty<GUILayoutOption>());
			if (flag19)
			{
				GUI.FocusControl(null);
				SetJump();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			TerminalSignal = GUILayout.TextField(TerminalSignal, new GUILayoutOption[]
			{
				GUILayout.Width(250f),
				GUILayout.Height(20f)
			});
			bool flag20 = GUILayout.Button("发送信号", Array.Empty<GUILayoutOption>());
			if (flag20)
			{
				GUI.FocusControl(null);
				bool flag21 = !StartOfRound.Instance.unlockablesList.unlockables[17].hasBeenUnlockedByPlayer;
				if (flag21)
				{
					StartOfRound.Instance.BuyShipUnlockableServerRpc(17, GameObjectManager.instance.shipTerminal.groupCredits);
					StartOfRound.Instance.SyncShipUnlockablesServerRpc();
				}
				HUDManager.Instance.UseSignalTranslatorServerRpc(TerminalSignal);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			SendChat = GUILayout.TextField(SendChat, new GUILayoutOption[]
			{
				GUILayout.Width(250f),
				GUILayout.Height(20f)
			});
			bool flag22 = GUILayout.Button("发送信息", Array.Empty<GUILayoutOption>());
			if (flag22)
			{
				GUI.FocusControl(null);
				PAUtils.SendChatMessage(SendChat, (int)player.playerClientId);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			bool flag23 = GUILayout.Button("复活自己为幽灵", Array.Empty<GUILayoutOption>());
			bool flag24 = flag23;
			if (flag24)
			{
				ResetMiscValuesPatch.SetPlayerResurrectedManually();
				playerinfo.ReviveDeadPlayers();
				playerinfo.localPlayerController.DamagePlayerServerRpc(1, 99);
				HUDManager.Instance.UpdateHealthUI(100, true);
			}
			bool flag25 = GUILayout.Button("真实复活自己", Array.Empty<GUILayoutOption>());
			if (flag25)
			{
				ReviveLocalPlayer();
			}
			GUILayout.Space(10f);
			bool flag26 = GUILayout.Button("立即保存飞船所有物品", Array.Empty<GUILayoutOption>());
			bool flag27 = flag26;
			if (flag27)
			{
				MethodInfo method = typeof(GameNetworkManager).GetMethod("SaveItemsInShip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag28 = method != null;
				if (flag28)
				{
					method.Invoke(GameNetworkManager.Instance, null);
					Debug.Log("小海提醒你:映射调用");
				}
			}
		}

		private void tpwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("玩家列表: (T: 传送过去),(+:打开STEAM主页)", Array.Empty<GUILayoutOption>());
			_playerListScrollPosition = GUILayout.BeginScrollView(_playerListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			DrawPlayerTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			GUILayout.Label("物品列表: (T: 传送过去), (+/-: 加减价值),(√:传送过来)", Array.Empty<GUILayoutOption>());
			_scrapListScrollPosition = GUILayout.BeginScrollView(_scrapListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			DrawScrapTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Label("怪物列表: (T: 传送过去), (死: 杀死怪物)", Array.Empty<GUILayoutOption>());
			_enemyListScrollPosition = GUILayout.BeginScrollView(_enemyListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			DrawEnemyTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			bool flag = GUILayout.Button("传送回飞船", Array.Empty<GUILayoutOption>());
			if (flag)
			{
				TeleportPlayer(StartOfRound.Instance.shipDoorNode.transform.position);
			}
		}

		private void freetpwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("传送管理器", Array.Empty<GUILayoutOption>());
			GUILayout.Label("当前玩家坐标为:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("X:" + localnowX, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Y:" + localnowY, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Z:" + localnowZ, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.Label("飞船为0,0,0附近", Array.Empty<GUILayoutOption>());
			GUILayout.Label("X轴:" + tpx.ToString("F2"), Array.Empty<GUILayoutOption>());
			tpx = GUILayout.HorizontalSlider(tpx, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s = GUILayout.TextField(tpx.ToString("F2"), Array.Empty<GUILayoutOption>());
			GUILayout.Label("Y轴:" + tpz.ToString("F2"), Array.Empty<GUILayoutOption>());
			tpz = GUILayout.HorizontalSlider(tpz, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s2 = GUILayout.TextField(tpz.ToString("F2"), Array.Empty<GUILayoutOption>());
			GUILayout.Label("高度Z轴:" + tpy.ToString("F2"), Array.Empty<GUILayoutOption>());
			tpy = GUILayout.HorizontalSlider(tpy, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s3 = GUILayout.TextField(tpy.ToString("F2"), Array.Empty<GUILayoutOption>());
			bool flag = GUILayout.Button("传送玩家到指定坐标", Array.Empty<GUILayoutOption>());
			bool flag2 = flag;
			if (flag2)
			{
				float num;
				float num2;
				float num3;
				if (float.TryParse(s, out num) && float.TryParse(s2, out num2) && float.TryParse(s3, out num3))
				{
					tpx = num;
					tpy = num2;
					tpz = num3;
				}
				Vector3 pos = new Vector3(tpx, tpy, tpz);
				if (playerinfo != null)
				{
					GameNetworkManager.Instance.localPlayerController.TeleportPlayer(pos, false, 0f, false, true);
				}
			}
		}

		public bool PlaceAnywhere = false;

		public static Xhgui Instance = new Xhgui();

		private PlayerControllerB player = new PlayerControllerB();

		public static bool NoInvisible;

		private bool HotKeyPressed = false;

		private float updateInterval = 0.5f;

		private float timeSinceLastUpdate = 0f;

		public bool renyizhuaqu = false;

		public bool renyijianzao = false;

		public StartOfRound playerinfo;

		public bool gamestrat = false;

		private bool isWUDISelected = false;

		private bool allowTIMESTOP = false;

		private bool allowStopSprintMeter = false;

		private bool AlwaysFullyCharge = false;

		private bool once = true;

		public bool first = true;

		public string localnowX;

		public string localnowY;

		public string localnowZ;

		public bool noclip = false;

		public bool NoFog = false;

		public float GrabDistance = 5f;

		public float _movementSpeed = 4.6f;

		public float _climpSpeed = 5f;

		public bool Nightvision = false;

		public bool InfiniteZapGun = false;

		public bool EmoteWalk;

		public bool shotgunammo = false;

		public bool HearEveryone = false;

		public bool DoorESP = false;

		public bool ShipESP = false;

		public bool LightShow = false;

		private Rect rect = new Rect(5f, 5f, 400f, 600f);

		private float tpx = 0f;

		private float tpy = 0f;

		private float tpz = 0f;

		private GUILayoutOption[] Toggleoptions = new GUILayoutOption[]
		{
			GUILayout.Width(140f),
			GUILayout.Height(30f)
		};

		public string savetp = "";

		private Vector2 _scrapListScrollPosition = Vector2.zero;

		private Vector2 _enemyListScrollPosition = Vector2.zero;

		private string movementSpeedInput = "";

		private string jumpinput = "";

		private string customDeadlineinp = "";

		private string str_QuotaFulfilled;

		private string str_Quota;

		private Vector2 _playerListScrollPosition = Vector2.zero;

		private float _noClipSpeed = 5f;

		private int toolbarInt = 0;

		private string money;

		private float jump = 11f;

		private bool noneedtwohands;

		private readonly Harmony harmony = new Harmony("55");

		private static GUIStyle Style;

		public bool ctrlenemy;

		public bool jishihudong;

		private string TerminalSignal = "";

		private string SendChat = "";

		private string[] toolbarStrings = new string[]
		{
			"主界面",
			"传送管理界面",
			"自由传送界面"
		};

		public Color c_Door = new Color(0.74f, 0.74f, 1f, 1f);

		public Color c_ship = new Color(255f, 0.74f, 1f, 1f);

		public static bool SetGodMode;

		private bool _setESPColor = true;

		private bool _drawLine = true;

		private bool _drawDistance;

		private bool _drawName = true;

		private bool _setESPEnemy;

		private Vector2 ScreenScale;

		private Vector2 ScreenCenter;

		internal static RoundManager currentRound;

		internal static SelectableLevel currentLevel;

		internal static EnemyVent[] currentLevelVents;

		public int customDeadline;
	}
}
