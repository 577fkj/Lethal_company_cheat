using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using XHGUI;

namespace LethalMenu.EnemyControl
{
	// Token: 0x02000005 RID: 5
	internal class EnemyControl : Xhgui
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000023DC File Offset: 0x000005DC
		public static void Control(EnemyAI enemy)
		{
			bool isEnemyDead = enemy.isEnemyDead;
			if (isEnemyDead)
			{
				Debug.Log("怪物死了");
			}
			else
			{
				EnemyControl.enemy = enemy;
				Debug.Log("控制怪物");
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002414 File Offset: 0x00000614
		public static void StopControl()
		{
			bool flag = !Xhgui.Instance.ctrlenemy && EnemyControl.enemy != null;
			if (flag)
			{
				bool flag2 = EnemyControl.camera != null;
				if (flag2)
				{
					UnityEngine.Object.Destroy(EnemyControl.camera.gameObject);
				}
				bool flag3 = EnemyControl.light != null;
				if (flag3)
				{
					UnityEngine.Object.Destroy(EnemyControl.light.gameObject);
				}
				EnemyControl.camera = null;
				EnemyControl.light = null;
				EnemyControl.enemy.moveTowardsDestination = true;
				EnemyControl.enemy.updatePositionThreshold = 1f;
				EnemyControl.enemy.GetComponentsInChildren<Collider>().ToList<Collider>().ForEach(delegate(Collider c)
				{
					c.enabled = true;
				});
				Xhgui.Instance.playerinfo.localPlayerController.GetComponent<CharacterController>().enabled = true;
				EnemyControl.enemy = null;
				CameraManager.GetBaseCamera().enabled = true;
				CameraManager.ActiveCamera = CameraManager.GetBaseCamera();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002519 File Offset: 0x00000719
		public override void Update()
		{
			EnemyControl.StopControl();
			EnemyControl.ControlEnemy();
			Debug.Log("控制怪物中/..///");
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002534 File Offset: 0x00000734
		public static void ControlEnemy()
		{
			bool flag = EnemyControl.enemy == null || !Xhgui.Instance.ctrlenemy;
			if (!flag)
			{
				bool flag2 = !EnemyControl.enemy.IsOwner;
				if (flag2)
				{
					EnemyControl.enemy.ChangeEnemyOwnerServerRpc(Xhgui.Instance.playerinfo.localPlayerController.actualClientId);
				}
				EnemyControl.enemy.updatePositionThreshold = 0f;
				bool flag3 = EnemyControl.camera == null;
				if (flag3)
				{
					EnemyControl.camera = GameObjectUtil.CreateCamera("EnemyControlCam", CameraManager.GetBaseCamera().transform, true);
					EnemyControl.camera.enabled = true;
					EnemyControl.light = GameObjectUtil.CreateLight();
					EnemyControl.light.transform.SetParent(EnemyControl.camera.transform, false);
					CameraManager.GetBaseCamera().enabled = false;
					CameraManager.ActiveCamera = EnemyControl.camera;
					EnemyControl.camera.transform.position = EnemyControl.enemy.transform.position - EnemyControl.enemy.transform.forward * 3f + EnemyControl.enemy.transform.up * 1.5f;
				}
				EnemyControl.light.intensity = Xhgui.Instance.playerinfo.localPlayerController.nightVision.intensity;
				EnemyControl.light.range = Xhgui.Instance.playerinfo.localPlayerController.nightVision.range;
				Xhgui.Instance.playerinfo.localPlayerController.GetComponent<CharacterController>().enabled = false;
				Vector3 a = default(Vector3);
				bool isPressed = Keyboard.current.wKey.isPressed;
				if (isPressed)
				{
					a += EnemyControl.enemy.transform.forward;
				}
				bool isPressed2 = Keyboard.current.sKey.isPressed;
				if (isPressed2)
				{
					a -= EnemyControl.enemy.transform.forward;
				}
				bool isPressed3 = Keyboard.current.aKey.isPressed;
				if (isPressed3)
				{
					a -= EnemyControl.enemy.transform.right;
				}
				bool isPressed4 = Keyboard.current.dKey.isPressed;
				if (isPressed4)
				{
					a += EnemyControl.enemy.transform.right;
				}
				bool isPressed5 = Keyboard.current.spaceKey.isPressed;
				if (isPressed5)
				{
					a += EnemyControl.enemy.transform.up;
				}
				bool isPressed6 = Keyboard.current.ctrlKey.isPressed;
				if (isPressed6)
				{
					a -= EnemyControl.enemy.transform.up;
				}
				Vector3 vector = EnemyControl.enemy.transform.position + a * (Xhgui.Instance.playerinfo.localPlayerController.movementSpeed * Time.deltaTime);
				Vector3 position = vector - EnemyControl.enemy.transform.forward * 2f + EnemyControl.enemy.transform.up * 3f;
				EnemyControl.camera.transform.SetPositionAndRotation(position, Xhgui.Instance.playerinfo.localPlayerController.gameplayCamera.transform.rotation);
				bool flag4 = a.Equals(Vector3.zero);
				if (!flag4)
				{
					EnemyControl.enemy.transform.SetPositionAndRotation(vector, Xhgui.Instance.playerinfo.localPlayerController.gameplayCamera.transform.rotation);
					EnemyControl.enemy.moveTowardsDestination = false;
					EnemyControl.enemy.TargetClosestPlayer(1.5f, false, 70f);
					EnemyControl.enemy.SetDestinationToPosition(vector, false);
					EnemyControl.enemy.SyncPositionToClients();
				}
			}
		}

		// Token: 0x04000017 RID: 23
		private static Camera camera;

		// Token: 0x04000018 RID: 24
		private static Light light;

		// Token: 0x04000019 RID: 25
		private static EnemyAI enemy;
	}
}
