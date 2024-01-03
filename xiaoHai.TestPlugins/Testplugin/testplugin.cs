using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using editenemy;
using EditnewLevel.Patches;
using Editplayer.Patches;
using EditshipStrat.Patches;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using XHGUI;

namespace Testplugin
{
	// Token: 0x02000011 RID: 17
	[BepInPlugin("XiaoHai.plugin.CompanyPlus", "xiaoHai_CompanyPlus", "1.0.2")]
	public class testplugin : BaseUnityPlugin
	{

		// Token: 0x04000031 RID: 49
		public const string GUID = "XiaoHai.plugin.CompanyPlus";

		// Token: 0x04000032 RID: 50
		public const string NAME = "xiaoHai_CompanyPlus";

		// Token: 0x04000033 RID: 51
		public const string VERSION = "1.0.2";

		// Token: 0x04000035 RID: 53
		private readonly Harmony harmony = new Harmony("XiaoHai.plugin.CompanyPlus.0");

		// Token: 0x04000036 RID: 54
		private readonly Harmony harmony1 = new Harmony("XiaoHai.plugin.CompanyPlus.1");

		// Token: 0x04000037 RID: 55
		private readonly Harmony editplayer = new Harmony("XiaoHai.plugin.CompanyPlus.4");

		// Token: 0x04000038 RID: 56
		public ConfigEntry<Key> memuhotkey;

		// Token: 0x04000039 RID: 57
		public ConfigEntry<int> deadlineDaysAmount;

		// Token: 0x0400003A RID: 58
		public ConfigEntry<int> startingCredits;

		// Token: 0x0400003B RID: 59
		public ConfigEntry<int> startingQuota;

		// Token: 0x0400003C RID: 60
		public ConfigEntry<int> Monsterrarity;

		// Token: 0x0400003D RID: 61
		public ConfigEntry<int> Itemrarity;

		// Token: 0x0400003E RID: 62
		public ConfigEntry<int> MaxMonster;

		// Token: 0x0400003F RID: 63
		public ConfigEntry<bool> AddMonstEreverytime;

		// Token: 0x04000040 RID: 64
		public ConfigEntry<float> Itemweight;

		// Token: 0x04000041 RID: 65
		public ConfigEntry<int> ItemmaxValue;

		// Token: 0x04000042 RID: 66
		public ConfigEntry<int> ItemmaxValue_value;

		// Token: 0x04000043 RID: 67
		public ConfigEntry<bool> ItemNotNeedBattery;

		// Token: 0x04000044 RID: 68
		public ConfigEntry<bool> CanbeCarryingbyFlowerman;

		// Token: 0x04000045 RID: 69
		public ConfigEntry<bool> ItemNotNeedTwoHands;

		// Token: 0x04000046 RID: 70
		public ConfigEntry<string> SingleCode;

		// Token: 0x04000047 RID: 71
		public bool IDcardTrue = false;

		// Token: 0x04000048 RID: 72
		public string ExpirationTime;

		// Token: 0x04000049 RID: 73
		public static testplugin Instance;

		// Token: 0x0400004A RID: 74
		public static testplugin Instance1;

		// Token: 0x0400004B RID: 75
		public bool shouldRenderGUI = false;

		// Token: 0x0400004C RID: 76
		public bool isNewVerion = true;

		// Token: 0x0400004D RID: 77
		public static ManualLogSource mls;

		// Token: 0x0400004E RID: 78
		public static Xhgui xhgui;

		// Token: 0x04000050 RID: 80
		public string loginret;

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000044ED File Offset: 0x000026ED
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000044F4 File Offset: 0x000026F4
		public static bool k { get; set; }

		// Token: 0x06000042 RID: 66 RVA: 0x000044FC File Offset: 0x000026FC
		public void Awake()
		{
			this.shouldRenderGUI = true;
			GameObject gameObject = new GameObject("XHGUI");
            DontDestroyOnLoad(gameObject);
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.AddComponent<Xhgui>();
			xhgui = (Xhgui)gameObject.GetComponent("XHGUI");
			if (!Instance || !Instance1)
			{
				Instance = this;
				Instance1 = this;
			}
			this.memuhotkey = base.Config.Bind<Key>("Cnfig", "memuhotkey", Key.Home, "菜单快捷键");
			base.Logger.LogInfo("BepInEx:小海提醒你加载成功,您的快捷键为" + this.memuhotkey.Value.ToString());
			this.deadlineDaysAmount = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "deadlineDaysAmount", 5, "这里填初始配额时间(整数)");
			this.startingCredits = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "startingCredits", 60, "这里填初始金钱(整数)");
			this.startingQuota = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "startingQuota", 130, "这里填初始指标(整数)");
			this.Monsterrarity = base.Config.Bind<int>("MonsterConfig", "Monsterrarity", -1, "这里填怪物稀有度(整数,0-100,0为没怪物,-1为不修改)");
			this.MaxMonster = base.Config.Bind<int>("MonsterConfig", "MaxMonster", 8, "这里填怪物最大数0-30,限制怪物的最大数(如果稀有度为0该项失效),8为默认值");
			this.AddMonstEreverytime = base.Config.Bind<bool>("MonsterConfig", "AddMonstEreverytime", false, "是否随时出怪?(暂时无效)");
			this.Itemrarity = base.Config.Bind<int>("ItemConfig", "Itemrarity", -1, "这里填物品稀有度(整数,0-100,0为没物品,-1为不修改)此项为-1 物品项全部失效");
			this.Itemweight = base.Config.Bind<float>("ItemConfig", "Itemweight", -1f, "这里填物品重量0-1000,设置后全部为一个重量,-1为默认 单位为磅//这里好像是指废品 买的无效\"");
			this.ItemmaxValue = base.Config.Bind<int>("ItemConfig", "maxItem", -1, "这里填物品最大数量 没测试过最大值 理论上限为21亿除以单个最大物品价值,-1为默认\n若不为-1,最小值为最大值的50%");
			this.ItemmaxValue_value = base.Config.Bind<int>("ItemConfig", "maxItemValue", -1, "这里填单个物品最大价值 没测试过最大值理论上限为21亿除以最大物品数量,-1为默认\n若不为-1,最小值为最大值的50%");
			this.ItemNotNeedBattery = base.Config.Bind<bool>("ItemConfig", "ItemNotNeedBattery", false, "是否开启所有物品不需要电池?如果开启填true //这里好像是指废品 买的无效");
			this.ItemNotNeedTwoHands = base.Config.Bind<bool>("ItemConfig", "ItemNotNeedTwoHands", false, "是否开启所有物品不需要双手?如果开启填true //这里好像是指废品 买的无效");
			this.CanbeCarryingbyFlowerman = base.Config.Bind<bool>("PlayerConfig", "CanbeCarryingbyFlowerman", true, "能否被小黑抓住");
			this.MaxMonster.Value = ((this.MaxMonster.Value < 0) ? 0 : ((this.MaxMonster.Value > 100) ? 100 : this.MaxMonster.Value));
			if (this.Monsterrarity.Value < 0 || this.Monsterrarity.Value > 100)
			{
				this.Monsterrarity.Value = -1;
			}
			if (this.Itemrarity.Value < 0 || this.Itemrarity.Value > 100)
			{
				this.Itemrarity.Value = -1;
			}
			if (this.Itemweight.Value < 0f || this.Itemweight.Value > 1000f)
			{
				this.Itemweight.Value = -1f;
			}
			this.harmony.PatchAll(typeof(TimeOfDayAwakePatch));
			this.harmony1.PatchAll(typeof(EditNewLevel));
			this.harmony.PatchAll(typeof(editflowermanAI));
			this.harmony.PatchAll(typeof(editmaskman));
			this.harmony.PatchAll(typeof(OpenShipDoorsPatch));
			this.harmony.PatchAll(typeof(ResetMiscValuesPatch));
			this.harmony.PatchAll(typeof(StopTime));
			this.harmony.PatchAll(typeof(StopSprintMeter));
			this.harmony.PatchAll(typeof(IsDisconnect));
			this.editplayer.PatchAll(typeof(editplayer));
		}
	}
}
