using System;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalMenu.EnemyControl
{
	// Token: 0x02000004 RID: 4
	public class CameraManager
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000022D4 File Offset: 0x000004D4
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002334 File Offset: 0x00000534
		public static Camera ActiveCamera
		{
			get
			{
				bool flag = !StartOfRound.Instance;
				if (flag)
				{
					CameraManager._camera = null;
				}
				PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
				bool flag2 = CameraManager._camera == null || CameraManager.UsingBaseCamera();
				if (flag2)
				{
					CameraManager._camera = CameraManager.GetBaseCamera();
				}
				return CameraManager._camera;
			}
			set
			{
				CameraManager._camera = value;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002340 File Offset: 0x00000540
		public static Camera GetBaseCamera()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			bool flag = !localPlayerController.isPlayerDead;
			Camera result;
			if (flag)
			{
				result = localPlayerController.gameplayCamera;
			}
			else
			{
				result = StartOfRound.Instance.spectateCamera;
			}
			return result;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002380 File Offset: 0x00000580
		public static bool UsingBaseCamera()
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			return CameraManager._camera.GetInstanceID() == localPlayerController.gameplayCamera.GetInstanceID() || CameraManager._camera.GetInstanceID() == StartOfRound.Instance.spectateCamera.GetInstanceID();
		}

		// Token: 0x04000016 RID: 22
		private static Camera _camera;
	}
}
