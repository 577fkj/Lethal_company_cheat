using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameNetcodeStuff;
using UnityEngine;

namespace Pautils
{
	// Token: 0x02000007 RID: 7
	public class Settings
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000023B8 File Offset: 0x000005B8
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

		// Token: 0x04000016 RID: 22
		public static float TEXT_HEIGHT = 30f;

		// Token: 0x04000017 RID: 23
		public Rect windowRect = new Rect(50f, 50f, 545f, 400f);

		// Token: 0x04000018 RID: 24
		public bool b_isMenuOpen;

		// Token: 0x04000019 RID: 25
		public Dictionary<PlayerControllerB, bool> b_DemiGod = new Dictionary<PlayerControllerB, bool>();

		// Token: 0x0400001A RID: 26
		public Dictionary<PlayerControllerB, bool> b_SpamChat = new Dictionary<PlayerControllerB, bool>();

		// Token: 0x0400001B RID: 27
		public string str_DamageToGive = "1";

		// Token: 0x0400001C RID: 28
		public string str_HealthToHeal = "1";

		// Token: 0x0400001D RID: 29
		public string str_ChatAsPlayer = "Hello World!";

		// Token: 0x0400001E RID: 30
		private static Settings instance;

		// Token: 0x02000022 RID: 34
		public static class ResourceReader
		{
			// Token: 0x0600009E RID: 158 RVA: 0x00007B1C File Offset: 0x00005D1C
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
