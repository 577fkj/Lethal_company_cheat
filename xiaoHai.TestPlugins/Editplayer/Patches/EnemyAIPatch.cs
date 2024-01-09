using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(EnemyAI))]
	public class EnemyAIPatch
	{
		// Token: 0x0600004C RID: 76 RVA: 0x000037D8 File Offset: 0x000019D8
		[HarmonyPatch("EnableEnemyMesh")]
		[HarmonyPrefix]
		private static void EnableEnemyMeshPrefix(ref bool enable)
		{
			bool flag = !Xhgui.NoInvisible;
			if (!flag)
			{
				enable = true;
			}
		}
	}
}
