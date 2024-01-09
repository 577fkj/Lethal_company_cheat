using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameNetcodeStuff;
using UnityEngine;

namespace Pautils
{
	// Token: 0x0200000A RID: 10
	public class Settings
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002BD8 File Offset: 0x00000DD8
		public static Settings Instance
		{
			get
			{
				bool flag = Settings.instance == null;
				if (flag)
				{
					Settings.instance = new Settings();
				}
				return Settings.instance;
			}
		}

		// Token: 0x0400001A RID: 26
		public static float TEXT_HEIGHT = 30f;

		// Token: 0x0400001B RID: 27
		public Rect windowRect = new Rect(50f, 50f, 545f, 400f);

		// Token: 0x0400001C RID: 28
		public bool b_isMenuOpen;

		// Token: 0x0400001D RID: 29
		public Dictionary<PlayerControllerB, bool> b_DemiGod = new Dictionary<PlayerControllerB, bool>();

		// Token: 0x0400001E RID: 30
		public Dictionary<PlayerControllerB, bool> b_SpamChat = new Dictionary<PlayerControllerB, bool>();

		// Token: 0x0400001F RID: 31
		public string str_DamageToGive = "1";

		// Token: 0x04000020 RID: 32
		public string str_HealthToHeal = "1";

		// Token: 0x04000021 RID: 33
		public string str_ChatAsPlayer = "Hello World!";

		// Token: 0x04000022 RID: 34
		private static Settings instance;

		// Token: 0x02000026 RID: 38
		public static class ResourceReader
		{
			// Token: 0x060000B7 RID: 183 RVA: 0x00009154 File Offset: 0x00007354
			public static string ReadTextFile(string resourceName)
			{
				string result = string.Empty;
				using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					using (StreamReader streamReader = new StreamReader(manifestResourceStream))
					{
						result = streamReader.ReadToEnd();
					}
				}
				return result;
			}
		}
	}
}
