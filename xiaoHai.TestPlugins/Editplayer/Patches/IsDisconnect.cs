using System;
using HarmonyLib;
using UnityEngine;

namespace Editplayer.Patches
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
	public static class IsDisconnect
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000032B7 File Offset: 0x000014B7
		// (set) Token: 0x06000055 RID: 85 RVA: 0x000032BE File Offset: 0x000014BE
		public static bool isDisconnec { get; private set; }

		// Token: 0x06000056 RID: 86 RVA: 0x000032C8 File Offset: 0x000014C8
		public static void Prefix()
		{
			IsDisconnect.isDisconnec = true;
			Debug.Log("小海提醒你:正在退出房间" + IsDisconnect.isDisconnec.ToString());
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000032FC File Offset: 0x000014FC
		public static void Postfix()
		{
			IsDisconnect.isDisconnec = false;
			Debug.Log("小海提醒你:已经退出房间" + IsDisconnect.isDisconnec.ToString());
		}
	}
}
