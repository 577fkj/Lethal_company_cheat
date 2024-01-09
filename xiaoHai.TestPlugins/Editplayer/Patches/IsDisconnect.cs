using System;
using HarmonyLib;
using UnityEngine;

namespace Editplayer.Patches
{
	// Token: 0x02000018 RID: 24
	[HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
	public static class IsDisconnect
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003AD7 File Offset: 0x00001CD7
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003ADE File Offset: 0x00001CDE
		public static bool isDisconnec { get; private set; }

		// Token: 0x06000064 RID: 100 RVA: 0x00003AE8 File Offset: 0x00001CE8
		public static void Prefix()
		{
			IsDisconnect.isDisconnec = true;
			Debug.Log("小海提醒你:正在退出房间" + IsDisconnect.isDisconnec.ToString());
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003B1C File Offset: 0x00001D1C
		public static void Postfix()
		{
			IsDisconnect.isDisconnec = false;
			Debug.Log("小海提醒你:已经退出房间" + IsDisconnect.isDisconnec.ToString());
		}
	}
}
