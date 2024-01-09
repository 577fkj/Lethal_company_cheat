using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using Editplayer.Patches;
using EditshipStrat.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.EnemyControl;
using Pautils;
using RENDER;
using render;
using Steamworks;
using Testplugin;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XHGUI
{
	// Token: 0x02000021 RID: 33
	public class Xhgui : MonoBehaviour
	{
		// Token: 0x0600007A RID: 122 RVA: 0x0000455F File Offset: 0x0000275F
		private void Start()
		{
			Debug.Log("小海提醒你:来了老弟");
			Xhgui.Instance = this;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004574 File Offset: 0x00002774
		public virtual void Update()
		{
			bool flag = editplayer.GetlocalPlayer() != null && editplayer.Instance() != null;
			if (flag)
			{
				this.playerinfo = editplayer.Instance();
				this.player = editplayer.GetlocalPlayer();
				this.HandleNoClip(this.noclip);
				bool flag2 = this.player != null && this.playerinfo != null;
				if (flag2)
				{
					this.localnowX = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.x);
					this.localnowY = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.z);
					this.localnowZ = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.y);
					EnemyControl.StopControl();
					EnemyControl.ControlEnemy();
					bool flag3 = Keyboard.current[Key.F5].wasPressedThisFrame && this.opendoor;
					if (flag3)
					{
						Xhgui.UnlockAllDoors();
					}
					this.gamestrat = true;
					bool flag4 = this.opendoor;
					if (flag4)
					{
					}
					bool lightShow = this.LightShow;
					if (lightShow)
					{
						bool flag5 = GameObjectManager.Instance.shipLights;
						if (flag5)
						{
							GameObjectManager.Instance.shipLights.SetShipLightsServerRpc(!GameObjectManager.Instance.shipLights.areLightsOn);
						}
					}
					bool flag6 = this.first;
					if (flag6)
					{
						Debug.Log("小海提醒你:Playerinfo和 player 均检测成功" + this.playerinfo.allowLocalPlayerDeath.ToString() + "  ,," + this.player.playerUsername);
						HUDManager.Instance.DisplayTip("XiaoHaiPlugin1.0.5小海mod开启成功", "YUAN SHEN QI DONG !YOU CAN PRESS YOUR HOTKEY NOW!", true, false, "LC_EclipseTip");
						this.str_Quota = TimeOfDay.Instance.profitQuota.ToString();
						this.defaultGrabDistance = this.playerinfo.localPlayerController.grabDistance;
						this.str_QuotaFulfilled = TimeOfDay.Instance.quotaFulfilled.ToString();
						base.StartCoroutine(this.CollectObjects());
						base.StartCoroutine(GameObjectManager.Instance.CollectObjects());
						this.money = GameObjectManager.Instance.shipTerminal.groupCredits.ToString();
						DateTime d = DateTime.ParseExact(testplugin.Instance.ExpirationTime, "yyyy-MM-dd HH:mm:ss", null);
						DateTime now = DateTime.Now;
						bool flag7 = (d - now).TotalDays < 3.0;
						if (flag7)
						{
							PAUtils.SendChatMessage("欢迎使用小海mod,请勿在路人房滥用.QQ二群726351737", (int)this.player.playerClientId);
						}
						this.customDeadline = (int)TimeOfDay.Instance.timeUntilDeadline / 1080;
						Xhgui.enemyRaritys = new Dictionary<SpawnableEnemyWithRarity, int>();
						Xhgui.levelEnemySpawns = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();
						Xhgui.enemyPropCurves = new Dictionary<SpawnableEnemyWithRarity, AnimationCurve>();
						this.first = false;
					}
				}
			}
			bool wasPressedThisFrame = Keyboard.current[Key.Space].wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				GUI.FocusControl(null);
			}
			bool wasPressedThisFrame2 = Keyboard.current[testplugin.Instance1.memuhotkey.Value].wasPressedThisFrame;
			if (wasPressedThisFrame2)
			{
				bool flag8 = !testplugin.Instance.JustGO.Value;
				if (flag8)
				{
					bool flag9 = !this.gamestrat;
					if (flag9)
					{
						Debug.Log("小海提醒你:警告!!!先等开飞船再调出菜单!");
						return;
					}
				}
				this.HotKeyPressed = !this.HotKeyPressed;
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000048F1 File Offset: 0x00002AF1
		public IEnumerator CollectObjects()
		{
			for (;;)
			{
				this.CollectObjects<TerminalAccessibleObject>(Xhgui.bigDoors, (TerminalAccessibleObject obj) => obj.isBigDoor);
				this.CollectObjects<DoorLock>(Xhgui.doorLocks, null);
				yield return new WaitForSeconds(0.2f);
			}
			yield break;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004900 File Offset: 0x00002B00
		private void CollectObjects<T>(List<T> list, Func<T, bool> filter = null) where T : MonoBehaviour
		{
			list.Clear();
			bool flag = filter != null;
			IEnumerable<T> collection;
			if (flag)
			{
				collection = UnityEngine.Object.FindObjectsOfType<T>().Where(filter);
			}
			else
			{
				IEnumerable<T> enumerable = UnityEngine.Object.FindObjectsOfType<T>();
				collection = enumerable;
			}
			list.AddRange(collection);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004940 File Offset: 0x00002B40
		public void OnGUI()
		{
			bool flag = this.DoorESP | this.ShipESP;
			if (flag)
			{
				this.HandleESP();
			}
			bool hotKeyPressed = this.HotKeyPressed;
			if (hotKeyPressed)
			{
				this.rect = GUILayout.Window(1, this.rect, new GUI.WindowFunction(this.windowfunc), "主菜单", Array.Empty<GUILayoutOption>());
				Xhgui.Style = new GUIStyle(GUI.skin.label);
				Xhgui.Style.normal.textColor = Color.white;
				Xhgui.Style.fontStyle = FontStyle.Bold;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000049D8 File Offset: 0x00002BD8
		private void DisplayObjects<T>(IEnumerable<T> objects, bool shouldDisplay, Func<T, string> labelSelector, Func<T, Color> colorSelector) where T : Component
		{
			bool flag = !shouldDisplay;
			if (!flag)
			{
				foreach (T t in objects)
				{
					bool flag2 = t != null && t.gameObject.activeSelf;
					if (flag2)
					{
						float distance = PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position, t.transform.position);
						Vector3 vector;
						bool flag3 = PAUtils.WorldToScreen(GameObjectManager.Instance.localPlayer.gameplayCamera, t.transform.position, out vector);
						if (flag3)
						{
							string text = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(t));
							text = text + " [" + distance.ToString().ToUpper() + "M]";
							Render.String(Xhgui.Style, vector.x, vector.y, 150f, 50f, text, colorSelector(t), true, true);
						}
					}
				}
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004B18 File Offset: 0x00002D18
		private void DisplayDoors()
		{
			this.DisplayObjects<EntranceTeleport>(GameObjectManager.Instance.entranceTeleports, this.DoorESP, delegate(EntranceTeleport entranceTeleport)
			{
				bool flag = !entranceTeleport.isEntranceToBuilding;
				string result;
				if (flag)
				{
					result = "出口";
				}
				else
				{
					result = "入口";
				}
				return result;
			}, (EntranceTeleport _) => this.c_Door);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004B68 File Offset: 0x00002D68
		private void DisplayShip()
		{
			this.DisplayObjects<HangarShipDoor>(new HangarShipDoor[]
			{
				GameObjectManager.Instance.shipDoor
			}, this.ShipESP, (HangarShipDoor _) => "飞船", (HangarShipDoor _) => this.c_ship);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004BC4 File Offset: 0x00002DC4
		private void NightVision()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.nightVision.enabled = this.Nightvision;
				localPlayerController.nightVision.intensity = (this.Nightvision ? 3000f : 360f);
				localPlayerController.nightVision.range = (this.Nightvision ? 10000f : 12f);
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004C40 File Offset: 0x00002E40
		private void SetJump()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.jumpForce = this.jump;
				this.jump = localPlayerController.jumpForce;
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004C80 File Offset: 0x00002E80
		private void HandleMovementSpeed()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.movementSpeed = this._movementSpeed;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004CB4 File Offset: 0x00002EB4
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
								Vector3 localPosition2 = localPosition + a * (this._noClipSpeed * Time.deltaTime);
								localPlayerController.transform.localPosition = localPosition2;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004E88 File Offset: 0x00003088
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

		// Token: 0x06000087 RID: 135 RVA: 0x00004EE0 File Offset: 0x000030E0
		private void alwaysfullyCharge()
		{
			bool flag = !this.AlwaysFullyCharge;
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

		// Token: 0x06000088 RID: 136 RVA: 0x00004FB0 File Offset: 0x000031B0
		private void Windowspwanfunc()
		{
			PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
			int num = 1;
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(30f);
			GUILayout.Label("玩家列表", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("玩家ID", Array.Empty<GUILayoutOption>());
			GUILayout.Label("玩家名字", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
			Dictionary<int, PlayerControllerB> dictionary2 = new Dictionary<int, PlayerControllerB>();
			this.playerPos = GUILayout.BeginScrollView(this.playerPos, new GUILayoutOption[]
			{
				GUILayout.Height(150f)
			});
			foreach (PlayerControllerB playerControllerB in allPlayerScripts)
			{
				string playerUsername = playerControllerB.playerUsername;
				bool flag = !playerUsername.Contains("Player #");
				if (flag)
				{
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(num.ToString(), Array.Empty<GUILayoutOption>());
					dictionary.Add(num, playerControllerB.transform.position);
					dictionary2.Add(num, playerControllerB);
					num++;
					GUILayout.Label(playerUsername, Array.Empty<GUILayoutOption>());
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("请输入生成数量", Array.Empty<GUILayoutOption>());
			Xhgui.num = GUILayout.TextField(Xhgui.num, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("请输入玩家ID选择生成位置,-1为随机位置", Array.Empty<GUILayoutOption>());
			this.positionstr = GUILayout.TextField(this.positionstr, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			GUILayout.EndHorizontal();
			bool flag2 = false;
			int key;
			Vector3 vector;
			PlayerControllerB playerControllerB2;
			if (int.TryParse(this.positionstr, out key) && dictionary.TryGetValue(key, out vector) && dictionary2.TryGetValue(key, out playerControllerB2))
			{
				Xhgui.position = new Vector3(vector.x - 5f, vector.y - 5f, vector.z);
				PlayerControllerB playerControllerB3 = playerControllerB2;
				Xhgui.inside = playerControllerB3.isInsideFactory;
				flag2 = false;
			}
			else
			{
				flag2 = true;
				Xhgui.inside = false;
			}
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("怪物列表:", Array.Empty<GUILayoutOption>());
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(250f)
			});
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			EnemyAI[] array2 = Resources.FindObjectsOfTypeAll<EnemyAI>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (EnemyAI enemyAI in array2)
			{
				string text = enemyAI.enemyType.enemyName;
				bool flag4 = hashSet.Contains(text);
				if (!flag4)
				{
					hashSet.Add(text);
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					string text2;
					text = (this.enemyname_cn_english.TryGetValue(text, out text2) ? text2 : text);
					GUILayout.Label(text, new GUILayoutOption[]
					{
						GUILayout.MinWidth(120f)
					});
					bool flag5 = GUILayout.Button("生成", Array.Empty<GUILayoutOption>());
					if (flag5)
					{
						int num2;
						try
						{
							num2 = int.Parse(Xhgui.num);
						}
						catch
						{
							num2 = 1;
						}
						GameObject[] array4 = (!Xhgui.inside) ? RoundManager.Instance.outsideAINodes : RoundManager.Instance.insideAINodes;
						for (int k = 0; k < num2; k++)
						{
							bool flag6 = flag2;
							if (flag6)
							{
								GameObject gameObject = array4[UnityEngine.Random.Range(0, array4.Length)];
								Xhgui.position = gameObject.transform.position;
							}
							RoundManager.Instance.SpawnEnemyGameObject(Xhgui.position, 0f, -1, enemyAI.enemyType);
						}
						GUI.FocusControl(null);
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000053F4 File Offset: 0x000035F4
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
									Xhgui.TeleportPlayer(grabbableObject.transform.position);
									GUI.FocusControl(null);
								}
								bool flag6 = GUILayout.Button("+", Array.Empty<GUILayoutOption>());
								if (flag6)
								{
									grabbableObject.SetScrapValue(scrapValue + 1);
									GUI.FocusControl(null);
								}
								bool flag7 = GUILayout.Button("-", Array.Empty<GUILayoutOption>());
								if (flag7)
								{
									grabbableObject.SetScrapValue(scrapValue - 1);
									GUI.FocusControl(null);
								}
								bool flag8 = GUILayout.Button("√", Array.Empty<GUILayoutOption>());
								if (flag8)
								{
									grabbableObject.transform.SetPositionAndRotation(localPlayerController.transform.position, grabbableObject.transform.rotation);
									grabbableObject.startFallingPosition = localPlayerController.transform.position;
									grabbableObject.targetFloorPosition = localPlayerController.transform.position;
									GUI.FocusControl(null);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000056EC File Offset: 0x000038EC
		private static void TeleportPlayer(Vector3 position)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				localPlayerController.TeleportPlayer(position, false, 0f, false, true);
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005724 File Offset: 0x00003924
		private void SetMoney(string Money)
		{
			int groupCredits;
			int.TryParse(Money, out groupCredits);
			GameObjectManager.Instance.shipTerminal.groupCredits = groupCredits;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000574C File Offset: 0x0000394C
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
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = !this.ctrlenemy;
			if (flag)
			{
				EnemyControl.StopControl();
			}
			GUILayout.EndHorizontal();
			EnemyAI[] array = UnityEngine.Object.FindObjectsOfType<EnemyAI>();
			bool flag2 = array == null;
			if (!flag2)
			{
				foreach (EnemyAI enemyAI in array)
				{
					bool flag3 = !(enemyAI == null) && !enemyAI.isEnemyDead;
					if (flag3)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						bool flag4 = !(localPlayerController == null);
						if (flag4)
						{
							ScanNodeProperties componentInChildren = enemyAI.GetComponentInChildren<ScanNodeProperties>();
							string text = enemyAI.enemyType.enemyName;
							string text2;
							text = (this.enemyname_cn_english.TryGetValue(text, out text2) ? text2 : text);
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
							bool flag5 = GUILayout.Button("T", Array.Empty<GUILayoutOption>());
							if (flag5)
							{
								Xhgui.TeleportPlayer(enemyAI.transform.position);
								GUI.FocusControl(null);
							}
							bool flag6 = GUILayout.Button("死", Array.Empty<GUILayoutOption>());
							if (flag6)
							{
								enemyAI.KillEnemyServerRpc(true);
								GUI.FocusControl(null);
							}
							bool flag7 = GUILayout.Button("附", Array.Empty<GUILayoutOption>());
							if (flag7)
							{
								bool flag8 = this.ctrlenemy;
								if (flag8)
								{
									EnemyControl.Control(enemyAI);
								}
							}
							bool flag9 = !localPlayerController.isInsideFactory;
							if (flag9)
							{
								bool flag10 = GUILayout.Button("电", Array.Empty<GUILayoutOption>());
								bool flag11 = flag10;
								if (flag11)
								{
									this.Strike(enemyAI.transform.position);
									GUI.FocusControl(null);
								}
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000059D4 File Offset: 0x00003BD4
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
									Xhgui.TeleportPlayer(component.transform.position);
									GUI.FocusControl(null);
								}
								bool flag6 = GUILayout.Button("死", Array.Empty<GUILayoutOption>());
								if (flag6)
								{
									component.DamagePlayerFromOtherClientServerRpc(component.health, Vector3.zero, (int)component.playerClientId);
									bool flag7 = !component.isPlayerDead;
									if (flag7)
									{
										component.AllowPlayerDeath();
										component.KillPlayerServerRpc((int)component.playerClientId, true, component.transform.position, 2, 1);
									}
									GUI.FocusControl(null);
								}
								bool flag8 = GUILayout.Button("+", Array.Empty<GUILayoutOption>());
								bool flag9 = flag8;
								if (flag9)
								{
									SteamFriends.OpenUserOverlay(component.playerSteamId, "steamid");
									GUI.FocusControl(null);
								}
								bool flag10 = !component.isInsideFactory;
								if (flag10)
								{
									bool flag11 = GUILayout.Button("电", Array.Empty<GUILayoutOption>());
									bool flag12 = flag11;
									if (flag12)
									{
										this.Strike(component.transform.position);
										GUI.FocusControl(null);
									}
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005C94 File Offset: 0x00003E94
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

		// Token: 0x0600008F RID: 143 RVA: 0x00006174 File Offset: 0x00004374
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
						Vector3 vector = enemyAI.transform.position;
						Vector3 vector2 = camera.WorldToScreenPoint(vector);
						bool flag3 = this.IsOnScreen(vector2, camera);
						if (flag3)
						{
							ScanNodeProperties componentInChildren = enemyAI.GetComponentInChildren<ScanNodeProperties>();
							string str = componentInChildren ? componentInChildren.headerText : enemyAI.enemyType.enemyName;
							float num = Vector3.Distance(player.transform.position, enemyAI.transform.position);
							Vector2 vector3 = new Vector2(vector2.x * this.ScreenScale.x, (float)Screen.height - vector2.y * this.ScreenScale.y);
							Color color = this._setESPColor ? Color.red : Color.white;
							string text = "";
							bool drawName = this._drawName;
							if (drawName)
							{
								text += str;
							}
							bool drawDistance = this._drawDistance;
							if (drawDistance)
							{
								text = text + " | " + num.ToString("F1") + "m";
							}
							bool flag4 = text != "";
							if (flag4)
							{
								RenDer.DrawString(new Vector2(vector3.x, vector3.y - 20f), text, true);
							}
							bool drawLine = this._drawLine;
							if (drawLine)
							{
								RenDer.DrawLine(this.ScreenCenter, vector3, color, 2f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000633C File Offset: 0x0000453C
		private bool IsOnScreen(Vector3 position, Camera camera)
		{
			return position.x >= 0f && position.x <= (float)camera.pixelWidth && position.y >= 0f && position.y <= (float)camera.pixelHeight && position.z >= 0f;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000639C File Offset: 0x0000459C
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
					this.ScreenScale = new Vector2((float)Screen.width / (float)camera.pixelWidth, (float)Screen.height / (float)camera.pixelHeight);
					this.ScreenCenter = new Vector2((float)(Screen.width / 2), (float)(Screen.height - 1));
					bool setESPEnemy = this._setESPEnemy;
					if (setESPEnemy)
					{
						this.ESPEnemy(localPlayerController, camera);
					}
					bool doorESP = this.DoorESP;
					if (doorESP)
					{
						this.DisplayDoors();
					}
					bool shipESP = this.ShipESP;
					if (shipESP)
					{
						this.DisplayShip();
					}
				}
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006478 File Offset: 0x00004678
		internal static void SpawnEnemyfunc(EnemyAI Enemy, bool random, int num, Vector3 position, bool inside)
		{
			PlayerControllerB localPlayerController = Xhgui.Instance.playerinfo.localPlayerController;
			bool flag = localPlayerController == null && Xhgui.currentLevel.Enemies == null;
			if (!flag)
			{
				foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in Xhgui.currentLevel.Enemies)
				{
					try
					{
						Debug.Log(spawnableEnemyWithRarity.enemyType.enemyName);
					}
					catch
					{
						Debug.Log("spawnableEnemyWithRarity=null!!");
						break;
					}
					bool flag2 = spawnableEnemyWithRarity.enemyType.enemyName.ToLower().Contains(Enemy.enemyType.enemyName.ToLower());
					bool flag3 = flag2;
					if (flag3)
					{
						try
						{
							string enemyName = spawnableEnemyWithRarity.enemyType.enemyName;
							if (random)
							{
								Xhgui.SpawnEnemy(spawnableEnemyWithRarity, num, inside, new Vector3(0f, 0f, 0f));
							}
							else
							{
								Xhgui.SpawnEnemy(spawnableEnemyWithRarity, num, inside, position);
							}
							Debug.Log("小海提醒你:Spawned " + spawnableEnemyWithRarity.enemyType.enemyName);
						}
						catch
						{
							Debug.Log("小海提醒你:生成失败");
						}
						Debug.Log("Spawned: " + spawnableEnemyWithRarity.enemyType.enemyName);
						break;
					}
				}
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000662C File Offset: 0x0000482C
		internal static void SpawnEnemy(SpawnableEnemyWithRarity enemy, int amount, bool inside, Vector3 location)
		{
			PlayerControllerB localPlayerController = Xhgui.Instance.playerinfo.localPlayerController;
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
								Xhgui.currentRound.SpawnEnemyOnServer(location, 0f, Xhgui.currentLevel.Enemies.IndexOf(enemy));
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
								UnityEngine.Object.Instantiate<GameObject>(Xhgui.currentLevel.OutsideEnemies[Xhgui.currentLevel.OutsideEnemies.IndexOf(enemy)].enemyType.enemyPrefab, location, Quaternion.Euler(Vector3.zero)).gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
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
								Xhgui.currentRound.SpawnEnemyOnServer(Xhgui.currentRound.allEnemyVents[UnityEngine.Random.Range(0, Xhgui.currentRound.allEnemyVents.Length)].floorNode.position, Xhgui.currentRound.allEnemyVents[k].floorNode.eulerAngles.y, Xhgui.currentLevel.Enemies.IndexOf(enemy));
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
						UnityEngine.Object.Instantiate<GameObject>(Xhgui.currentLevel.OutsideEnemies[Xhgui.currentLevel.OutsideEnemies.IndexOf(enemy)].enemyType.enemyPrefab, GameObject.FindGameObjectsWithTag("OutsideAINode")[UnityEngine.Random.Range(0, GameObject.FindGameObjectsWithTag("OutsideAINode").Length - 1)].transform.position, Quaternion.Euler(Vector3.zero)).gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
					}
					Debug.Log(string.Format("You wanted to spawn: {0} enemies", amount));
					Debug.Log("小海提醒你:Total Spawned: " + l.ToString());
				}
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00006970 File Offset: 0x00004B70
		[HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
		[HarmonyPrefix]
		private static bool ModifyLevel(ref SelectableLevel newLevel)
		{
			Xhgui.currentRound = RoundManager.Instance;
			bool flag = !Xhgui.levelEnemySpawns.ContainsKey(newLevel);
			bool flag2 = flag;
			if (flag2)
			{
				List<SpawnableEnemyWithRarity> list = new List<SpawnableEnemyWithRarity>();
				foreach (SpawnableEnemyWithRarity item in newLevel.Enemies)
				{
					list.Add(item);
				}
				Xhgui.levelEnemySpawns.Add(newLevel, list);
			}
			List<SpawnableEnemyWithRarity> enemies;
			Xhgui.levelEnemySpawns.TryGetValue(newLevel, out enemies);
			newLevel.Enemies = enemies;
			foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in newLevel.Enemies)
			{
				bool flag3 = !Xhgui.enemyRaritys.ContainsKey(spawnableEnemyWithRarity);
				bool flag4 = flag3;
				if (flag4)
				{
					Xhgui.enemyRaritys.Add(spawnableEnemyWithRarity, spawnableEnemyWithRarity.rarity);
				}
				int rarity;
				Xhgui.enemyRaritys.TryGetValue(spawnableEnemyWithRarity, out rarity);
				spawnableEnemyWithRarity.rarity = rarity;
			}
			foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity2 in newLevel.OutsideEnemies)
			{
				bool flag5 = !Xhgui.enemyRaritys.ContainsKey(spawnableEnemyWithRarity2);
				bool flag6 = flag5;
				if (flag6)
				{
					Xhgui.enemyRaritys.Add(spawnableEnemyWithRarity2, spawnableEnemyWithRarity2.rarity);
				}
				int rarity2;
				Xhgui.enemyRaritys.TryGetValue(spawnableEnemyWithRarity2, out rarity2);
				spawnableEnemyWithRarity2.rarity = rarity2;
			}
			foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity3 in newLevel.Enemies)
			{
				bool flag7 = !Xhgui.enemyPropCurves.ContainsKey(spawnableEnemyWithRarity3);
				bool flag8 = flag7;
				if (flag8)
				{
					Xhgui.enemyPropCurves.Add(spawnableEnemyWithRarity3, spawnableEnemyWithRarity3.enemyType.probabilityCurve);
				}
				AnimationCurve probabilityCurve = new AnimationCurve();
				Xhgui.enemyPropCurves.TryGetValue(spawnableEnemyWithRarity3, out probabilityCurve);
				spawnableEnemyWithRarity3.enemyType.probabilityCurve = probabilityCurve;
			}
			return true;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006BCC File Offset: 0x00004DCC
		[HarmonyPatch(typeof(RoundManager), "EnemyCannotBeSpawned")]
		[HarmonyPrefix]
		private static bool OverrideCannotSpawn()
		{
			return false;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006BDF File Offset: 0x00004DDF
		[HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
		[HarmonyPostfix]
		private static void UpdateNewInfo(ref EnemyVent[] ___allEnemyVents, ref SelectableLevel ___currentLevel, RoundManager __instance)
		{
			Xhgui.currentLevel = ___currentLevel;
			Xhgui.currentLevelVents = ___allEnemyVents;
			Xhgui.currentRound = __instance;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006BF6 File Offset: 0x00004DF6
		[HarmonyPatch(typeof(RoundManager), "AdvanceHourAndSpawnNewBatchOfEnemies")]
		[HarmonyPrefix]
		private static void UpdateCurrentLevelInfo(ref EnemyVent[] ___allEnemyVents, ref SelectableLevel ___currentLevel, RoundManager __instance)
		{
			Xhgui.currentLevel = ___currentLevel;
			Xhgui.currentLevelVents = ___allEnemyVents;
			Xhgui.currentRound = __instance;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006C10 File Offset: 0x00004E10
		[HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
		[HarmonyPostfix]
		private static void PatchDeadline(TimeOfDay __instance)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = localPlayerController == null;
			if (!flag)
			{
				bool flag2 = localPlayerController.IsHost && Xhgui.Instance.customDeadline != int.MinValue;
				bool flag3 = flag2;
				if (flag3)
				{
					__instance.quotaVariables.deadlineDaysAmount = Xhgui.Instance.customDeadline;
					Debug.Log("小海提醒你:Xhgui.Instance.customDeadline" + __instance.quotaVariables.deadlineDaysAmount.ToString());
					__instance.timeUntilDeadline = (float)(__instance.quotaVariables.deadlineDaysAmount + Xhgui.Instance.customDeadline) * __instance.totalTime;
					TimeOfDay.Instance.timeUntilDeadline = (float)((int)(TimeOfDay.Instance.totalTime * (float)TimeOfDay.Instance.quotaVariables.deadlineDaysAmount));
					TimeOfDay.Instance.SyncTimeClientRpc(__instance.globalTime, (int)__instance.timeUntilDeadline);
					StartOfRound.Instance.deadlineMonitorText.text = "剩余天数:\n " + TimeOfDay.Instance.daysUntilDeadline.ToString();
					Debug.Log(StartOfRound.Instance.deadlineMonitorText.text);
				}
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006D3C File Offset: 0x00004F3C
		public static void UnlockAllDoors()
		{
			Xhgui.doorLocks.FindAll((DoorLock door) => door.isLocked).ForEach(delegate(DoorLock door)
			{
				door.UnlockDoorServerRpc();
			});
			Xhgui.bigDoors.ForEach(delegate(TerminalAccessibleObject door)
			{
				door.SetDoorOpenServerRpc(true);
			});
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006DC4 File Offset: 0x00004FC4
		[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
		[HarmonyPostfix]
		public static void PlayerLateUpdate(PlayerControllerB __instance)
		{
			bool throughWalls = Xhgui.Instance.ThroughWalls;
			if (throughWalls)
			{
				__instance.grabDistance = 10000f;
				LayerMask layerMask = LayerMask.GetMask(new string[]
				{
					"Props"
				});
				layerMask = LayerMask.GetMask(new string[]
				{
					"Props"
				});
				layerMask = LayerMask.GetMask(new string[]
				{
					"InteractableObject"
				});
				layerMask = LayerMask.GetMask(new string[]
				{
					"Props",
					"InteractableObject"
				});
				typeof(PlayerControllerB).GetField("interactableObjectsMask", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(__instance, layerMask.value);
			}
			else
			{
				typeof(PlayerControllerB).GetField("interactableObjectsMask", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(__instance, 832);
				bool flag = !Xhgui.Instance.ThroughWalls;
				if (flag)
				{
					__instance.grabDistance = 5f;
				}
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006ECC File Offset: 0x000050CC
		public void Strike(Vector3 position)
		{
			bool flag = RoundManager.Instance;
			if (flag)
			{
				RoundManager.Instance.LightningStrikeServerRpc(position);
			}
		}

		private void windowfunc(int windowID)
		{
			bool idcardTrue = testplugin.Instance.IDcardTrue;
			if (idcardTrue)
			{
				this.toolbarInt = GUI.Toolbar(new Rect(25f, 25f, 400f, 30f), this.toolbarInt, this.toolbarStrings);
				bool flag = this.toolbarInt == 0;
				if (flag)
				{
					UI.Reset();
					Vector2 vector = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
					this.maniwindowfnc();
				}
				bool flag2 = this.toolbarInt == 1;
				if (flag2)
				{
					this.tpwindowfnc();
				}
				bool flag3 = this.toolbarInt == 2;
				if (flag3)
				{
					this.freetpwindowfnc();
				}
				bool flag4 = this.toolbarInt == 3;
				if (flag4)
				{
					this.Windowspwanfunc();
				}
			}
			else
			{
				GUILayout.Label("登录状态为" + testplugin.Instance.loginret, Array.Empty<GUILayoutOption>());
			}
			GUILayout.Label("您的卡密到期时间为:" + testplugin.Instance.ExpirationTime, Array.Empty<GUILayoutOption>());
			GUI.DragWindow();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007450 File Offset: 0x00005650
		[CompilerGenerated]
		private void maniwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(30f);
			GUILayout.Label("欢迎使用小海MOD test1.0.5", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.isWUDISelected = GUILayout.Toggle(this.isWUDISelected, "是否开启无敌", this.Toggleoptions);
			bool flag = this.isWUDISelected;
			if (flag)
			{
				bool flag2 = this.playerinfo != null && this.player != null;
				if (flag2)
				{
					this.playerinfo.allowLocalPlayerDeath = false;
					StartOfRound.Instance.allowLocalPlayerDeath = false;
					this.player.AllowPlayerDeath();
					bool flag3 = this.once;
					if (flag3)
					{
						HUDManager.Instance.DisplayTip("GOD MODE !", "YOU ARE GOD NOW! JUST DO IT!----XiaoHai!", true, false, "LC_EclipseTip");
						this.once = false;
					}
				}
			}
			else
			{
				this.playerinfo.allowLocalPlayerDeath = true;
				StartOfRound.Instance.allowLocalPlayerDeath = true;
				this.player.AllowPlayerDeath();
			}
			this.Nightvision = GUILayout.Toggle(this.Nightvision, "夜视仪(传送进设备无效)", this.Toggleoptions);
			bool flag4 = this.playerinfo != null;
			if (flag4)
			{
				this.NightVision();
			}
			this.allowTIMESTOP = GUILayout.Toggle(this.allowTIMESTOP, "是否开启时停", this.Toggleoptions);
			bool flag5 = this.playerinfo != null;
			if (flag5)
			{
				bool flag6 = this.allowTIMESTOP;
				if (flag6)
				{
					StopTime.SetPlayerResurrectedManually(true);
				}
				else
				{
					StopTime.SetPlayerResurrectedManually(false);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.allowStopSprintMeter = GUILayout.Toggle(this.allowStopSprintMeter, "是否无限耐力", this.Toggleoptions);
			bool flag7 = this.playerinfo != null;
			if (flag7)
			{
				bool flag8 = this.allowStopSprintMeter;
				if (flag8)
				{
					StopSprintMeter.isStopspringntMeter(true);
				}
				else
				{
					StopSprintMeter.isStopspringntMeter(false);
				}
			}
			this.AlwaysFullyCharge = GUILayout.Toggle(this.AlwaysFullyCharge, "是否永远满电", this.Toggleoptions);
			this.alwaysfullyCharge();
			this.noclip = GUILayout.Toggle(this.noclip, "飞升模式", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			Xhgui.NoInvisible = GUILayout.Toggle(Xhgui.NoInvisible, "是否对怪物隐身(无效)", this.Toggleoptions);
			this.NoFog = GUILayout.Toggle(this.NoFog, "是否开启无迷雾", this.Toggleoptions);
			this.HandleNoFog(this.NoFog);
			this.jishihudong = GUILayout.Toggle(this.jishihudong, "是否开启瞬间开门", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.PlaceAnywhere = GUILayout.Toggle(this.PlaceAnywhere, "是否随意摆放家具", this.Toggleoptions);
			this.noneedtwohands = GUILayout.Toggle(this.noneedtwohands, "是否取消双手持有", this.Toggleoptions);
			bool flag9 = this.playerinfo != null;
			if (flag9)
			{
				StopSprintMeter.isNoNeedTwoHands(this.noneedtwohands);
			}
			this.InfiniteZapGun = GUILayout.Toggle(this.InfiniteZapGun, "电击枪无限控制", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.EmoteWalk = GUILayout.Toggle(this.EmoteWalk, "表情时可移动", this.Toggleoptions);
			this.shotgunammo = GUILayout.Toggle(this.shotgunammo, "是否无限弹药", this.Toggleoptions);
			this.LightShow = GUILayout.Toggle(this.LightShow, "灯光秀", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.HearEveryone = GUILayout.Toggle(this.HearEveryone, "顺风耳", this.Toggleoptions);
			this.DoorESP = GUILayout.Toggle(this.DoorESP, "透视大门", this.Toggleoptions);
			this.ShipESP = GUILayout.Toggle(this.ShipESP, "透视飞船", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.ThroughWalls = GUILayout.Toggle(this.ThroughWalls, "穿墙互动", this.Toggleoptions);
			GUILayout.EndHorizontal();
			GUILayout.Space(30f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("移速:" + this._movementSpeed.ToString(), Array.Empty<GUILayoutOption>());
			this.movementSpeedInput = GUILayout.TextField(this.movementSpeedInput, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			float movementSpeed;
			bool flag10 = float.TryParse(this.movementSpeedInput, out movementSpeed);
			if (flag10)
			{
				this._movementSpeed = movementSpeed;
			}
			bool flag11 = GUILayout.Button("修改移动速度", Array.Empty<GUILayoutOption>());
			if (flag11)
			{
				this.HandleMovementSpeed();
				GUI.FocusControl(null);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("弹跳:" + this.jump.ToString(), Array.Empty<GUILayoutOption>());
			this.jumpinput = GUILayout.TextField(this.jumpinput, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			float num;
			bool flag12 = float.TryParse(this.jumpinput, out num);
			if (flag12)
			{
				this.jump = num;
			}
			bool flag13 = GUILayout.Button("修改弹跳力", Array.Empty<GUILayoutOption>());
			if (flag13)
			{
				GUI.FocusControl(null);
				this.SetJump();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("金钱:" + this.money, Array.Empty<GUILayoutOption>());
			this.money = GUILayout.TextField(this.money, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			bool flag14 = GUILayout.Button("修改金钱", Array.Empty<GUILayoutOption>());
			if (flag14)
			{
				GUI.FocusControl(null);
				this.SetMoney(this.money);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("剩余天数:" + this.customDeadline.ToString(), Array.Empty<GUILayoutOption>());
			this.customDeadlineinp = GUILayout.TextField(this.customDeadlineinp, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			bool flag15 = GUILayout.Button("修改天数(次日生效)", Array.Empty<GUILayoutOption>());
			if (flag15)
			{
				GUI.FocusControl(null);
				int num2;
				bool flag16 = int.TryParse(this.customDeadlineinp, out num2);
				if (flag16)
				{
					this.customDeadline = num2;
					TimeOfDay.Instance.timeUntilDeadline = (float)(this.customDeadline * 1080);
					Xhgui.Instance.customDeadline = (int)TimeOfDay.Instance.timeUntilDeadline / 1080;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("利润配额:" + this.str_QuotaFulfilled + "/" + this.str_Quota, Array.Empty<GUILayoutOption>());
			this.str_QuotaFulfilled = GUILayout.TextField(this.str_QuotaFulfilled, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			GUILayout.Label("/", Array.Empty<GUILayoutOption>());
			this.str_Quota = GUILayout.TextField(this.str_Quota, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(50f),
				GUILayout.MinWidth(40f)
			});
			bool flag17 = GUILayout.Button("修改", Array.Empty<GUILayoutOption>());
			if (flag17)
			{
				GUI.FocusControl(null);
				bool flag18 = TimeOfDay.Instance;
				if (flag18)
				{
					TimeOfDay.Instance.profitQuota = int.Parse(this.str_Quota);
					TimeOfDay.Instance.quotaFulfilled = int.Parse(this.str_QuotaFulfilled);
					TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.TerminalSignal = GUILayout.TextField(this.TerminalSignal, new GUILayoutOption[]
			{
				GUILayout.Width(250f),
				GUILayout.Height(20f)
			});
			bool flag19 = GUILayout.Button("发送信号", Array.Empty<GUILayoutOption>());
			if (flag19)
			{
				GUI.FocusControl(null);
				bool flag20 = !StartOfRound.Instance.unlockablesList.unlockables[17].hasBeenUnlockedByPlayer;
				if (flag20)
				{
					StartOfRound.Instance.BuyShipUnlockableServerRpc(17, GameObjectManager.instance.shipTerminal.groupCredits);
					StartOfRound.Instance.SyncShipUnlockablesServerRpc();
				}
				HUDManager.Instance.UseSignalTranslatorServerRpc(this.TerminalSignal);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.SendChat = GUILayout.TextField(this.SendChat, new GUILayoutOption[]
			{
				GUILayout.Width(250f),
				GUILayout.Height(20f)
			});
			bool flag21 = GUILayout.Button("发送信息", Array.Empty<GUILayoutOption>());
			if (flag21)
			{
				GUI.FocusControl(null);
				PAUtils.SendChatMessage(this.SendChat, (int)this.player.playerClientId);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			bool flag22 = GUILayout.Button("复活自己为幽灵", Array.Empty<GUILayoutOption>());
			bool flag23 = flag22;
			if (flag23)
			{
				ResetMiscValuesPatch.SetPlayerResurrectedManually();
				this.playerinfo.ReviveDeadPlayers();
				this.playerinfo.localPlayerController.DamagePlayerServerRpc(1, 99);
				HUDManager.Instance.UpdateHealthUI(100, true);
			}
			bool flag24 = GUILayout.Button("真实复活自己", Array.Empty<GUILayoutOption>());
			if (flag24)
			{
				this.ReviveLocalPlayer();
			}
			GUILayout.Space(10f);
			bool flag25 = GUILayout.Button("立即保存飞船所有物品", Array.Empty<GUILayoutOption>());
			bool flag26 = flag25;
			if (flag26)
			{
				try
				{
					this.gameNetworkManager.SaveItemsInShip();
					Debug.Log("小海提醒你:直接调用");
				}
				catch
				{
					MethodInfo method = typeof(GameNetworkManager).GetMethod("SaveItemsInShip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag27 = method != null;
					if (flag27)
					{
						method.Invoke(this.gameNetworkManager, null);
						Debug.Log("小海提醒你:映射调用");
					}
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00007E98 File Offset: 0x00006098
		[CompilerGenerated]
		private void tpwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("玩家列表: (T: 传送过去),(+:打开STEAM主页)", Array.Empty<GUILayoutOption>());
			this._playerListScrollPosition = GUILayout.BeginScrollView(this._playerListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawPlayerTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			GUILayout.Label("物品列表: (T: 传送过去), (+/-: 加减价值),(√:传送过来)", Array.Empty<GUILayoutOption>());
			this._scrapListScrollPosition = GUILayout.BeginScrollView(this._scrapListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawScrapTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.ctrlenemy = GUILayout.Toggle(this.ctrlenemy, "附身怪物(开启后点击\"附\"", Array.Empty<GUILayoutOption>());
			this.opendoor = GUILayout.Toggle(this.opendoor, "按F5强制开附近门", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.Label("怪物列表: (T: 传送过去), (死: 杀死怪物)", Array.Empty<GUILayoutOption>());
			this._enemyListScrollPosition = GUILayout.BeginScrollView(this._enemyListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawEnemyTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			bool flag = GUILayout.Button("传送回飞船", Array.Empty<GUILayoutOption>());
			if (flag)
			{
				Xhgui.TeleportPlayer(StartOfRound.Instance.shipDoorNode.transform.position);
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00008060 File Offset: 0x00006260
		[CompilerGenerated]
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
			GUILayout.Label("X:" + this.localnowX, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Y:" + this.localnowY, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Z:" + this.localnowZ, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.Label("飞船为0,0,0附近", Array.Empty<GUILayoutOption>());
			GUILayout.Label("X轴:" + this.tpx.ToString("F2"), Array.Empty<GUILayoutOption>());
			this.tpx = GUILayout.HorizontalSlider(this.tpx, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s = GUILayout.TextField(this.tpx.ToString("F2"), Array.Empty<GUILayoutOption>());
			GUILayout.Label("Y轴:" + this.tpz.ToString("F2"), Array.Empty<GUILayoutOption>());
			this.tpz = GUILayout.HorizontalSlider(this.tpz, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s2 = GUILayout.TextField(this.tpz.ToString("F2"), Array.Empty<GUILayoutOption>());
			GUILayout.Label("高度Z轴:" + this.tpy.ToString("F2"), Array.Empty<GUILayoutOption>());
			this.tpy = GUILayout.HorizontalSlider(this.tpy, -999f, 999f, Array.Empty<GUILayoutOption>());
			string s3 = GUILayout.TextField(this.tpy.ToString("F2"), Array.Empty<GUILayoutOption>());
			bool flag = GUILayout.Button("传送玩家到指定坐标", Array.Empty<GUILayoutOption>());
			bool flag2 = flag;
			if (flag2)
			{
				float num;
				float num2;
				float num3;
				if (float.TryParse(s, out num) && float.TryParse(s2, out num2) && float.TryParse(s3, out num3))
				{
					this.tpx = num;
					this.tpy = num2;
					this.tpz = num3;
				}
				Vector3 pos = new Vector3(this.tpx, this.tpy, this.tpz);
				bool flag4 = this.playerinfo != null;
				if (flag4)
				{
					GameNetworkManager.Instance.localPlayerController.TeleportPlayer(pos, false, 0f, false, true);
				}
			}
		}

		// Token: 0x0400002F RID: 47
		public bool ThroughWalls = false;

		// Token: 0x04000030 RID: 48
		private EnemyControl enemyControlInstance;

		// Token: 0x04000031 RID: 49
		public bool PlaceAnywhere = false;

		// Token: 0x04000032 RID: 50
		public static Xhgui Instance = new Xhgui();

		// Token: 0x04000033 RID: 51
		private PlayerControllerB player = new PlayerControllerB();

		// Token: 0x04000034 RID: 52
		public static bool NoInvisible;

		// Token: 0x04000035 RID: 53
		private bool HotKeyPressed = false;

		// Token: 0x04000036 RID: 54
		private float updateInterval = 0.5f;

		// Token: 0x04000037 RID: 55
		private float timeSinceLastUpdate = 0f;

		// Token: 0x04000038 RID: 56
		public bool renyizhuaqu = false;

		// Token: 0x04000039 RID: 57
		public bool renyijianzao = false;

		// Token: 0x0400003A RID: 58
		public StartOfRound playerinfo;

		// Token: 0x0400003B RID: 59
		public bool gamestrat = false;

		// Token: 0x0400003C RID: 60
		private bool isWUDISelected = false;

		// Token: 0x0400003D RID: 61
		private bool allowTIMESTOP = false;

		// Token: 0x0400003E RID: 62
		private bool allowStopSprintMeter = false;

		// Token: 0x0400003F RID: 63
		private bool AlwaysFullyCharge = false;

		// Token: 0x04000040 RID: 64
		private bool once = true;

		// Token: 0x04000041 RID: 65
		public bool first = true;

		// Token: 0x04000042 RID: 66
		private GameNetworkManager gameNetworkManager;

		// Token: 0x04000043 RID: 67
		public string localnowX;

		// Token: 0x04000044 RID: 68
		public string localnowY;

		// Token: 0x04000045 RID: 69
		public string localnowZ;

		// Token: 0x04000046 RID: 70
		public bool noclip = false;

		// Token: 0x04000047 RID: 71
		public bool NoFog = false;

		// Token: 0x04000048 RID: 72
		public float GrabDistance = 5f;

		// Token: 0x04000049 RID: 73
		public float _movementSpeed = 4.6f;

		// Token: 0x0400004A RID: 74
		public float _climpSpeed = 5f;

		// Token: 0x0400004B RID: 75
		public bool Nightvision = false;

		// Token: 0x0400004C RID: 76
		public bool InfiniteZapGun = false;

		// Token: 0x0400004D RID: 77
		public bool EmoteWalk;

		// Token: 0x0400004E RID: 78
		public bool shotgunammo = false;

		// Token: 0x0400004F RID: 79
		public bool HearEveryone = false;

		// Token: 0x04000050 RID: 80
		public bool DoorESP = false;

		// Token: 0x04000051 RID: 81
		public bool ShipESP = false;

		// Token: 0x04000052 RID: 82
		public bool LightShow = false;

		// Token: 0x04000053 RID: 83
		private Dictionary<string, string> enemyname_cn_english = new Dictionary<string, string>
		{
			{
				"Jester",
				"八音盒"
			},
			{
				"Girl",
				"小女孩"
			},
			{
				"Red pill",
				"红色药丸(极其抽象 有点恐怖)"
			},
			{
				"Masked",
				"面具假人"
			},
			{
				"Hoarding bug",
				"囤积虫"
			},
			{
				"Spring",
				"弹簧头"
			},
			{
				"Baboon hawk",
				"狒狒鹰"
			},
			{
				"Forest Giant",
				"巨人"
			},
			{
				"Red Locust Bees",
				"电击蜜蜂"
			},
			{
				"Bunker Spider",
				"蜘蛛"
			},
			{
				"Blob",
				"史莱姆"
			},
			{
				"Puffer",
				"大嘴"
			},
			{
				"Flowerman",
				"小黑"
			},
			{
				"Docile Locust Bees",
				"蝗虫"
			},
			{
				"Nutcracker",
				"胡桃夹子"
			},
			{
				"MouthDog",
				"无眼狗"
			},
			{
				"Earth Leviathan",
				"大地沙虫"
			},
			{
				"Centipede",
				"抱脸虫"
			},
			{
				"Lasso",
				"套索人"
			},
			{
				"Crawler",
				"爬行者"
			},
			{
				"Manticoil",
				"飞鸟"
			},
			{
				"ForestGiant",
				"巨人"
			}
		};

		// Token: 0x04000054 RID: 84
		private Rect rect = new Rect(5f, 5f, 450f, 600f);

		// Token: 0x04000055 RID: 85
		private float tpx = 0f;

		// Token: 0x04000056 RID: 86
		private float tpy = 0f;

		// Token: 0x04000057 RID: 87
		private float tpz = 0f;

		// Token: 0x04000058 RID: 88
		private GUILayoutOption[] Toggleoptions = new GUILayoutOption[]
		{
			GUILayout.Width(140f),
			GUILayout.Height(30f)
		};

		// Token: 0x04000059 RID: 89
		private bool close = true;

		// Token: 0x0400005A RID: 90
		public string savetp = "";

		// Token: 0x0400005B RID: 91
		private Vector2 _scrapListScrollPosition = Vector2.zero;

		// Token: 0x0400005C RID: 92
		private Vector2 _enemyListScrollPosition = Vector2.zero;

		// Token: 0x0400005D RID: 93
		private string movementSpeedInput = "";

		// Token: 0x0400005E RID: 94
		private string jumpinput = "";

		// Token: 0x0400005F RID: 95
		private string customDeadlineinp = "";

		// Token: 0x04000060 RID: 96
		private string str_QuotaFulfilled;

		// Token: 0x04000061 RID: 97
		private string str_Quota;

		// Token: 0x04000062 RID: 98
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x04000063 RID: 99
		private Vector2 playerPos = Vector2.zero;

		// Token: 0x04000064 RID: 100
		private Vector2 _playerListScrollPosition = Vector2.zero;

		// Token: 0x04000065 RID: 101
		private float _noClipSpeed = 5f;

		// Token: 0x04000066 RID: 102
		private int toolbarInt = 0;

		// Token: 0x04000067 RID: 103
		private string money;

		// Token: 0x04000068 RID: 104
		private bool EnableInvite;

		// Token: 0x04000069 RID: 105
		private float jump = 11f;

		// Token: 0x0400006A RID: 106
		private bool noneedtwohands;

		// Token: 0x0400006B RID: 107
		private static GUIStyle Style;

		// Token: 0x0400006C RID: 108
		public bool ctrlenemy;

		// Token: 0x0400006D RID: 109
		public bool jishihudong;

		// Token: 0x0400006E RID: 110
		public bool opendoor = false;

		// Token: 0x0400006F RID: 111
		private string TerminalSignal = "小海NB";

		// Token: 0x04000070 RID: 112
		private string SendChat = "";

		// Token: 0x04000071 RID: 113
		private string[] toolbarStrings = new string[]
		{
			"主界面",
			"传送管理界面",
			"自由传送界面",
			"生成单位界面"
		};

		// Token: 0x04000072 RID: 114
		public Color c_Door = new Color(0.74f, 0.74f, 1f, 1f);

		// Token: 0x04000073 RID: 115
		public Color c_ship = new Color(255f, 0.74f, 1f, 1f);

		// Token: 0x04000074 RID: 116
		internal static string num = "1";

		// Token: 0x04000075 RID: 117
		internal static Vector3 position;

		// Token: 0x04000076 RID: 118
		internal static bool inside = false;

		// Token: 0x04000077 RID: 119
		private string positionstr = "1";

		// Token: 0x04000078 RID: 120
		public static bool SetGodMode;

		// Token: 0x04000079 RID: 121
		private bool _setESPColor = true;

		// Token: 0x0400007A RID: 122
		private bool _drawLine = true;

		// Token: 0x0400007B RID: 123
		private bool _drawDistance;

		// Token: 0x0400007C RID: 124
		private bool _drawName = true;

		// Token: 0x0400007D RID: 125
		private bool _setESPEnemy;

		// Token: 0x0400007E RID: 126
		private bool _setESPScrap;

		// Token: 0x0400007F RID: 127
		private Vector2 ScreenScale;

		// Token: 0x04000080 RID: 128
		private Vector2 ScreenCenter;

		// Token: 0x04000081 RID: 129
		internal static RoundManager currentRound;

		// Token: 0x04000082 RID: 130
		internal static SelectableLevel currentLevel;

		// Token: 0x04000083 RID: 131
		internal static EnemyVent[] currentLevelVents;

		// Token: 0x04000084 RID: 132
		public int customDeadline;

		// Token: 0x04000085 RID: 133
		public static Dictionary<SpawnableEnemyWithRarity, int> enemyRaritys;

		// Token: 0x04000086 RID: 134
		public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> levelEnemySpawns;

		// Token: 0x04000087 RID: 135
		public static Dictionary<SpawnableEnemyWithRarity, AnimationCurve> enemyPropCurves;

		// Token: 0x04000088 RID: 136
		public static List<TerminalAccessibleObject> bigDoors = new List<TerminalAccessibleObject>();

		// Token: 0x04000089 RID: 137
		public static List<DoorLock> doorLocks = new List<DoorLock>();

		// Token: 0x0400008A RID: 138
		private float defaultGrabDistance = -1f;
	}
}
