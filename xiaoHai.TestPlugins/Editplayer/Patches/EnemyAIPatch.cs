using System;
using HarmonyLib;
using XHGUI;

namespace Editplayer.Patches
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(EnemyAI))]
	public class EnemyAIPatch
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002288 File Offset: 0x00000488
		[HarmonyPatch("EnableEnemyMesh")]
		[HarmonyPrefix]
		private static void EnableEnemyMeshPrefix(ref bool enable)
		{
			if (Xhgui.NoInvisible)
			{
				enable = true;
			}
		}
	}
}
