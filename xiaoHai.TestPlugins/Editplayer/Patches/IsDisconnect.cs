using System;
using HarmonyLib;
using UnityEngine;

namespace Editplayer.Patches
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
	public static class IsDisconnect
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000024EB File Offset: 0x000006EB
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000024F2 File Offset: 0x000006F2
		public static bool isDisconnec { get; private set; }

		// Token: 0x0600001E RID: 30 RVA: 0x000024FC File Offset: 0x000006FC
		public static void Prefix()
		{
			isDisconnec = true;
			Debug.Log("正在退出房间" + isDisconnec.ToString());
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002530 File Offset: 0x00000730
		public static void Postfix()
		{
			isDisconnec = false;
			Debug.Log("已经退出房间" + isDisconnec.ToString());
		}
	}
}
