using System;
using System.Reflection;
using BepInEx;
using Editplayer.Patches;
using EditshipStrat.Patches;
using GameNetcodeStuff;
using Testplugin;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XHGUI
{
	// Token: 0x02000010 RID: 16
	public class Xhgui : MonoBehaviour
	{

		// Token: 0x0400000A RID: 10
		private PlayerControllerB player = new PlayerControllerB();

		// Token: 0x0400000B RID: 11
		public static bool NoInvisible;

		// Token: 0x0400000C RID: 12
		private bool HotKeyPressed = false;

		// Token: 0x0400000D RID: 13
		private float updateInterval = 0.5f;

		// Token: 0x0400000E RID: 14
		private float timeSinceLastUpdate = 0f;

		// Token: 0x0400000F RID: 15
		public StartOfRound playerinfo;

		// Token: 0x04000010 RID: 16
		public bool gamestrat = false;

		// Token: 0x04000011 RID: 17
		private bool isWUDISelected = false;

		// Token: 0x04000012 RID: 18
		private bool allowTIMESTOP = false;

		// Token: 0x04000013 RID: 19
		private bool allowStopSprintMeter = false;

		// Token: 0x04000014 RID: 20
		private bool AlwaysFullyCharge = false;

		// Token: 0x04000015 RID: 21
		private bool once = true;

		// Token: 0x04000016 RID: 22
		public bool first = true;

		// Token: 0x04000018 RID: 24
		public string localnowX;

		// Token: 0x04000019 RID: 25
		public string localnowY;

		// Token: 0x0400001A RID: 26
		public string localnowZ;

		// Token: 0x0400001B RID: 27
		public bool noclip = false;

		// Token: 0x0400001C RID: 28
		public bool NoFog = false;

		// Token: 0x0400001D RID: 29
		public float GrabDistance = 5f;

		// Token: 0x0400001E RID: 30
		public float _movementSpeed = 4.6f;

		// Token: 0x0400001F RID: 31
		public float _climpSpeed = 5f;

		// Token: 0x04000020 RID: 32
		private Rect rect = new Rect(5f, 5f, 400f, 600f);

		// Token: 0x04000021 RID: 33
		private float tpx = 0f;

		// Token: 0x04000022 RID: 34
		private float tpy = 0f;

		// Token: 0x04000023 RID: 35
		private float tpz = 0f;

		// Token: 0x04000024 RID: 36
		private GUILayoutOption[] Toggleoptions = new GUILayoutOption[]
		{
			GUILayout.Width(140f),
			GUILayout.Height(30f)
		};

		// Token: 0x04000026 RID: 38
		public string savetp = "";

		// Token: 0x04000027 RID: 39
		private Vector2 _scrapListScrollPosition = Vector2.zero;

		// Token: 0x04000028 RID: 40
		private Vector2 _enemyListScrollPosition = Vector2.zero;

		// Token: 0x04000029 RID: 41
		private Vector2 _playerListScrollPosition = Vector2.zero;

		// Token: 0x0400002A RID: 42
		private float _noClipSpeed = 5f;

		// Token: 0x0400002B RID: 43
		private int toolbarInt = 0;

		// Token: 0x0400002C RID: 44
		private string money = "60";

		// Token: 0x0400002D RID: 45
		private bool EnableInvite;

		// Token: 0x0400002E RID: 46
		private float jump = 11f;

		// Token: 0x0400002F RID: 47
		private string[] toolbarStrings = new string[]
		{
			"主界面",
			"传送管理界面",
			"自由传送界面"
		};

		// Token: 0x0600002B RID: 43 RVA: 0x00002C0F File Offset: 0x00000E0F
		private void Start()
		{
			Debug.Log("来了老弟");
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002C20 File Offset: 0x00000E20
		private void Update()
		{
			this.timeSinceLastUpdate += Time.deltaTime;
			if (this.timeSinceLastUpdate >= this.updateInterval)
			{
				this.timeSinceLastUpdate = 0f;
				if (editplayer.GetlocalPlayer() != null && editplayer.Instance() != null)
				{
					this.playerinfo = editplayer.Instance();
					this.player = editplayer.GetlocalPlayer();
					if (this.player != null && this.playerinfo != null)
					{
						if (this.first)
						{
							Debug.Log("Playerinfo和 player 均检测成功" + this.playerinfo.allowLocalPlayerDeath.ToString() + "  ,," + this.player.playerUsername);
							HUDManager.Instance.DisplayTip("Plugin1.0.2", "YUAN SHEN QI DONG !YOU CAN PRESS YOUR HOTKEY NOW!", true, false, "LC_EclipseTip");
							this.first = false;
						}
						this.localnowX = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.x);
						this.localnowY = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.y);
						this.localnowZ = string.Format("{0:0.00}", this.player.thisPlayerBody.localPosition.z);
						this.gamestrat = true;
						this.HandleNoClip(this.noclip);
						this.HandleGrabDistance(this.GrabDistance);
						this.HandleMovementSpeed();
						this.SetClimpSpeed();
					}
				}
			}
			bool wasPressedThisFrame = Keyboard.current[testplugin.Instance1.memuhotkey.Value].wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				if (!this.gamestrat)
				{
					Debug.Log("警告!!!先等开飞船再调出菜单!");
				}
				else
				{
					this.HotKeyPressed = !this.HotKeyPressed;
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002E24 File Offset: 0x00001024
		public void OnGUI()
		{
			if (this.HotKeyPressed)
			{
				this.rect = GUILayout.Window(1, this.rect, new GUI.WindowFunction(this.windowfunc), "主菜单", Array.Empty<GUILayoutOption>());
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002E6C File Offset: 0x0000106C
		private void SetClimpSpeed()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.climbSpeed = this._climpSpeed;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002EA0 File Offset: 0x000010A0
		private void SetJump()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.jumpForce = this.jump;
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002ED4 File Offset: 0x000010D4
		private void HandleMovementSpeed()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.movementSpeed = this._movementSpeed;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002F08 File Offset: 0x00001108
		private void HandleNoClip(bool _setNoClip)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				Transform transform = localPlayerController.gameplayCamera.transform;
				if (transform != null)
				{
					Collider component = localPlayerController.GetComponent<CharacterController>();
					if (component != null)
					{
						if (!_setNoClip)
						{
							component.enabled = true;
						}
						else
						{
							component.enabled = false;
							Vector3 a = default(Vector3);
							if (UnityInput.Current.GetKey(KeyCode.W))
							{
								a += transform.forward;
							}
							if (UnityInput.Current.GetKey(KeyCode.S))
							{
								a += transform.forward * -1f;
							}
							if (UnityInput.Current.GetKey(KeyCode.D))
							{
								a += transform.right;
							}
							if (UnityInput.Current.GetKey(KeyCode.A))
							{
								a += transform.right * -1f;
							}
							if (UnityInput.Current.GetKey(KeyCode.Space))
							{
								a.y += transform.up.y;
							}
							if (UnityInput.Current.GetKey(KeyCode.LeftControl))
							{
								a.y += transform.up.y * -1f;
							}
							Vector3 localPosition = localPlayerController.transform.localPosition;
							if (!localPosition.Equals(Vector3.zero))
							{
								localPlayerController.transform.localPosition = localPosition + a * (this._noClipSpeed * Time.deltaTime);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000030D8 File Offset: 0x000012D8
		private void HandleNoFog(bool enable)
		{
			GameObject gameObject = GameObject.Find("Systems");
			if (gameObject != null)
			{
				gameObject.transform.Find("Rendering").Find("VolumeMain").gameObject.SetActive(!enable);
				RenderSettings.fog = enable;
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003130 File Offset: 0x00001330
		private void HandleGrabDistance(float _grabDistance)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.grabDistance = _grabDistance;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003160 File Offset: 0x00001360
		private void editMovementSpeed(float _movementSpeed)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.movementSpeed = _movementSpeed;
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003190 File Offset: 0x00001390
		private void alwaysfullyCharge(bool a)
		{
			GrabbableObject currentlyHeldObjectServer = this.player.currentlyHeldObjectServer;
			if (currentlyHeldObjectServer != null)
			{
				bool requiresBattery = currentlyHeldObjectServer.itemProperties.requiresBattery;
				if (requiresBattery)
				{
					if (a)
					{
						currentlyHeldObjectServer.SyncBatteryServerRpc(100);
					}
				}
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000031D8 File Offset: 0x000013D8
		private void SetMoney()
		{
			Terminal terminal = new Terminal();
			terminal.groupCredits = int.Parse(this.money);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003200 File Offset: 0x00001400
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
			GrabbableObject[] array = FindObjectsOfType<GrabbableObject>();
			if (array != null && array.Length == 0)
			{
				foreach (GrabbableObject grabbableObject in array)
				{
					if (grabbableObject != null && grabbableObject.itemProperties.isScrap && grabbableObject.grabbable && !grabbableObject.isHeld && (grabbableObject == null || !grabbableObject.isInShipRoom || !grabbableObject.isInElevator))
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						if (localPlayerController != null)
						{
							ScanNodeProperties componentInChildren = grabbableObject.GetComponentInChildren<ScanNodeProperties>();
							if (componentInChildren != null)
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
								if (GUILayout.Button("T", Array.Empty<GUILayoutOption>()))
								{
									TeleportPlayer(grabbableObject.transform.position);
								}
								if (GUILayout.Button("+", Array.Empty<GUILayoutOption>()))
								{
									grabbableObject.SetScrapValue(scrapValue + 1);
								}
								if (GUILayout.Button("-", Array.Empty<GUILayoutOption>()))
								{
									grabbableObject.SetScrapValue(scrapValue - 1);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000347C File Offset: 0x0000167C
		private static void TeleportPlayer(Vector3 position)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			if (localPlayerController != null)
			{
				localPlayerController.TeleportPlayer(position, false, 0f, false, true);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000034B4 File Offset: 0x000016B4
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
			EnemyAI[] array = FindObjectsOfType<EnemyAI>();
			if (array != null)
			{
				foreach (EnemyAI enemyAI in array)
				{
					if (enemyAI != null && !enemyAI.isEnemyDead)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						if (localPlayerController != null)
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
							if (GUILayout.Button("T", Array.Empty<GUILayoutOption>()))
							{
								TeleportPlayer(enemyAI.transform.position);
							}
							if (GUILayout.Button("死", Array.Empty<GUILayoutOption>()))
							{
								enemyAI.KillEnemyServerRpc(false);
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003688 File Offset: 0x00001888
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
			if (allPlayerObjects != null)
			{
				GameObject[] array = allPlayerObjects;
				for (int i = 0; i < array.Length; i++)
				{
					PlayerControllerB component = array[i].GetComponent<PlayerControllerB>();
					if (component != null && !component.isPlayerDead && !component.IsOwner)
					{
						PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
						if (localPlayerController != null)
						{
							string playerUsername = component.playerUsername;
							float num = Vector3.Distance(component.transform.position, localPlayerController.transform.position);
							if (!playerUsername.Contains("Player #"))
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
								if (GUILayout.Button("T", Array.Empty<GUILayoutOption>()))
								{
									TeleportPlayer(component.transform.position);
								}
								if (GUILayout.Button("死", Array.Empty<GUILayoutOption>()))
								{
									component.DamagePlayerFromOtherClientServerRpc(component.health, Vector3.zero, (int)component.playerClientId);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
				}
			}
		}

		private void windowfunc(int windowID)
		{
			this.toolbarInt = GUI.Toolbar(new Rect(25f, 25f, 350f, 30f), this.toolbarInt, this.toolbarStrings);
			switch (this.toolbarInt) { 
				case 0:
					this.maniwindowfnc();
					break;
				case 1:
					this.tpwindowfnc();
					break;
				case 2:
					this.freetpwindowfnc();
					break;
			}
			GUI.DragWindow();
		}

		private void maniwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(30f);
			GUILayout.Label("欢迎使用MOD test1.0.2", Array.Empty<GUILayoutOption>());


			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.isWUDISelected = GUILayout.Toggle(this.isWUDISelected, "是否开启无敌", this.Toggleoptions);
			if (this.playerinfo != null && this.player != null)
			{
				if (this.isWUDISelected)
				{
					this.playerinfo.allowLocalPlayerDeath = false;
					StartOfRound.Instance.allowLocalPlayerDeath = false;
					this.player.AllowPlayerDeath();
					if (this.once)
					{
						HUDManager.Instance.DisplayTip("GOD MODE !", "YOU ARE GOD NOW! JUST DO IT!", true, false, "LC_EclipseTip");
						this.once = false;
					}
				}
				else
				{
					this.playerinfo.allowLocalPlayerDeath = true;
					StartOfRound.Instance.allowLocalPlayerDeath = true;
					this.player.AllowPlayerDeath();
				}
			}

			this.allowTIMESTOP = GUILayout.Toggle(this.allowTIMESTOP, "是否开启时停", this.Toggleoptions);
			if (this.playerinfo != null)
			{
				if (this.allowTIMESTOP)
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
			if (this.playerinfo != null)
			{
				if (this.allowStopSprintMeter)
				{
					StopSprintMeter.isStopspringntMeter(true);
				}
				else
				{
					StopSprintMeter.isStopspringntMeter(false);
				}
			}


			this.AlwaysFullyCharge = GUILayout.Toggle(this.AlwaysFullyCharge, "是否永远满电", this.Toggleoptions);
			if (this.AlwaysFullyCharge)
			{
				this.alwaysfullyCharge(true);
			}
			else
			{
				this.alwaysfullyCharge(false);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.EnableInvite = GUILayout.Toggle(this.EnableInvite, "是否开启中途加入", this.Toggleoptions);
			this.NoFog = GUILayout.Toggle(this.NoFog, "是否开启无迷雾", this.Toggleoptions);
			this.HandleNoFog(this.NoFog);
			GUILayout.EndHorizontal();

			GUILayout.Space(30f);

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("拾取距离 " + this.GrabDistance.ToString(), Array.Empty<GUILayoutOption>());
			this.GrabDistance = GUILayout.HorizontalSlider(this.GrabDistance, 5f, 100f, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("设置拾取距离", Array.Empty<GUILayoutOption>()))
			{
				this.HandleGrabDistance(this.GrabDistance);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10f);

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("移速:" + this._movementSpeed.ToString(), Array.Empty<GUILayoutOption>());
			GUILayout.Space(15f);
			this._movementSpeed = GUILayout.HorizontalSlider(this._movementSpeed, 4.6f, 100f, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("设置移动速度", Array.Empty<GUILayoutOption>()))
			{
				this.HandleMovementSpeed();
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10f);

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("攀爬速度:" + this._climpSpeed.ToString(), Array.Empty<GUILayoutOption>());
			this._climpSpeed = GUILayout.HorizontalSlider(this._climpSpeed, 4.6f, 100f, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("设置攀爬速度", Array.Empty<GUILayoutOption>()))
			{
				this.SetClimpSpeed();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("金钱:" + this.money.ToString(), Array.Empty<GUILayoutOption>());
			this.money = GUILayout.TextField(this.money, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("设置金钱", Array.Empty<GUILayoutOption>()))
			{
				this.SetMoney();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("弹跳:" + this.jump.ToString(), Array.Empty<GUILayoutOption>());
			this.jump = GUILayout.HorizontalSlider(this.jump, 11f, 100f, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("设置弹跳", Array.Empty<GUILayoutOption>()))
			{
				this.SetJump();
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);

			if (this.player) { 
				this.player.climbSpeed = 10f;
			}

			if (GUILayout.Button("复活自己为幽灵", Array.Empty<GUILayoutOption>()))
			{
				ResetMiscValuesPatch.SetPlayerResurrectedManually();
				this.playerinfo.ReviveDeadPlayers();
				this.playerinfo.localPlayerController.DamagePlayerServerRpc(1, 99);
				HUDManager.Instance.UpdateHealthUI(100, true);
			}
			GUILayout.Space(10f);

			if (GUILayout.Button("立即保存飞船所有物品", Array.Empty<GUILayoutOption>()))
			{
				MethodInfo method = typeof(GameNetworkManager).GetMethod("SaveItemsInShip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(GameNetworkManager.Instance, null);
					Debug.Log("映射调用");
				}
				//try
				//{
				//	GameNetworkManager.Instance.SaveItemsInShip();
				//	Debug.Log("直接调用");
				//}
				//catch
				//{
				//	MethodInfo method = typeof(GameNetworkManager).GetMethod("SaveItemsInShip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				//	if (method != null)
				//	{
				//		method.Invoke(GameNetworkManager.Instance, null);
				//		Debug.Log("映射调用");
				//	}
				//}
			}
		}

		private void tpwindowfnc()
		{
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Space(10f);
			GUILayout.Label("玩家列表: (T: 点击传送)", Array.Empty<GUILayoutOption>());
			this._playerListScrollPosition = GUILayout.BeginScrollView(this._playerListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawPlayerTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			GUILayout.Label("物品列表: (T: 点击传送), (+/-: 加减价值)", Array.Empty<GUILayoutOption>());
			this._scrapListScrollPosition = GUILayout.BeginScrollView(this._scrapListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawScrapTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Label("怪物列表: (T: 点击传送), (死: 杀死怪物)", Array.Empty<GUILayoutOption>());
			this._enemyListScrollPosition = GUILayout.BeginScrollView(this._enemyListScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(100f)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawEnemyTable();
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			if (GUILayout.Button("传送回飞船", Array.Empty<GUILayoutOption>()))
			{
				Xhgui.TeleportPlayer(StartOfRound.Instance.shipDoorNode.transform.position);
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
			if (GUILayout.Button("传送玩家到指定坐标", Array.Empty<GUILayoutOption>()))
			{
				float x;
				float y;
				float z;
				if (float.TryParse(s, out x) && float.TryParse(s2, out y) && float.TryParse(s3, out z))
				{
					this.tpx = x;
					this.tpy = y;
					this.tpz = z;
				}
				Vector3 pos = new Vector3(this.tpx, this.tpy, this.tpz);
				if (this.playerinfo != null)
				{
					GameNetworkManager.Instance.localPlayerController.TeleportPlayer(pos, false, 0f, false, true);
				}
			}
		}
	}
}
