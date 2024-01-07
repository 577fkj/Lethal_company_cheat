using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(EnemyAI))]
	public class EnemyAIPatch
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00002FB8 File Offset: 0x000011B8
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
