using System;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.UI;
using XHGUI;

namespace LethalMenu.EnemyControl
{
	// Token: 0x02000006 RID: 6
	internal class GameObjectUtil
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002920 File Offset: 0x00000B20
		public static Camera CreateCamera(string name, Transform pos, bool copyPlayerTexture = true)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
			Camera camera = new GameObject(name).AddComponent<Camera>();
			camera.transform.position = pos.position;
			camera.transform.rotation = pos.rotation;
			camera.targetTexture = (copyPlayerTexture ? localPlayerController.gameplayCamera.targetTexture : new RenderTexture(1920, 1080, 24));
			camera.cullingMask = localPlayerController.gameplayCamera.cullingMask;
			camera.farClipPlane = localPlayerController.gameplayCamera.farClipPlane;
			camera.nearClipPlane = localPlayerController.gameplayCamera.nearClipPlane;
			return camera;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000029CC File Offset: 0x00000BCC
		public static RawImage CreateMiniCamDisplay(Texture targetTexture)
		{
			RawImage rawImage = new GameObject("SpectateMiniCamDisplay").AddComponent<RawImage>();
			rawImage.rectTransform.anchorMin = new Vector2(1f, 1f);
			rawImage.rectTransform.anchorMax = new Vector2(1f, 1f);
			rawImage.rectTransform.pivot = new Vector2(1f, 1f);
			rawImage.rectTransform.sizeDelta = new Vector2(192f, 108f);
			rawImage.rectTransform.anchoredPosition = new Vector2(1f, 1f);
			rawImage.texture = targetTexture;
			rawImage.transform.SetParent(HUDManager.Instance.playerScreenTexture.transform, false);
			return rawImage;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002A9C File Offset: 0x00000C9C
		public static Light CreateLight()
		{
			Light light = UnityEngine.Object.Instantiate<Light>(Xhgui.Instance.playerinfo.localPlayerController.nightVision);
			light.enabled = true;
			light.intensity = 3600f;
			light.range = 1000f;
			return light;
		}
	}
}
