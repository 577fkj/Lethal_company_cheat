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
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using XHGUI;

namespace Testplugin
{
	[BepInPlugin("XiaoHai.plugin.CompanyPlus", "xiaoHai_CompanyPlus", "1.0.4")]
	public class testplugin : BaseUnityPlugin
	{
		public static bool k { get; set; }

		public void Awake()
		{
			this.shouldRenderGUI = true;
			GameObject gameObject = new GameObject("XHGUI");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.AddComponent<Xhgui>();
			testplugin.xhgui = (Xhgui)gameObject.GetComponent("XHGUI");
			bool flag = !testplugin.Instance || !testplugin.Instance1;
			if (flag)
			{
				testplugin.Instance = this;
				testplugin.Instance1 = this;
			}
			this.memuhotkey = base.Config.Bind<Key>("Config//主要设置", "memuhotkey", Key.Home, "菜单快捷键");
			base.Logger.LogInfo("BepInEx:小海提醒你加载成功,您的快捷键为" + this.memuhotkey.Value.ToString());
			this.JustGO = base.Config.Bind<bool>("Config//主要设置", "GOGOGO", false, "是否无视MOD冲突强制运行)");
			this.SingleCode = base.Config.Bind<string>("A卡密", "SingleCode", "试用1小时", "在这里输入你的卡密");
			base.Logger.LogInfo("BepInEx:登录中...");
			this.deadlineDaysAmount = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "deadlineDaysAmount", 5, "这里填初始配额时间(整数)");
			this.startingCredits = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "startingCredits", 60, "这里填初始金钱(整数)");
			this.startingQuota = base.Config.Bind<int>("ShipConfig//初始游戏参数类", "startingQuota", 130, "这里填初始指标(整数)");
			this.baseIncrease = base.Config.Bind<float>("ShipConfig//初始游戏参数类", "baseIncrease", 200f, "这里填基础配额指标增长数");
			this.Monsterrarity = base.Config.Bind<int>("MonsterConfig", "Monsterrarity", -1, "这里填怪物稀有度(整数,0-100,0为没怪物,-1为不修改)");
			this.MaxMonster = base.Config.Bind<int>("MonsterConfig", "MaxMonster", 8, "这里填怪物最大数0-30,限制怪物的最大数(如果稀有度为0该项失效),8为默认值");
			this.AddMonstEreverytime = base.Config.Bind<bool>("MonsterConfig", "AddMonstEreverytime", false, "是否随时出怪?(暂时无效)");
			this.Itemrarity = base.Config.Bind<int>("ItemConfig//物品设置", "Itemrarity", -1, "这里填物品稀有度(整数,0-100,0为没物品,-1为不修改)此项为-1 物品项全部失效");
			this.Itemweight = base.Config.Bind<float>("ItemConfig//物品设置", "Itemweight", -1f, "这里填物品重量0-1000,设置后全部为一个重量,-1为默认 单位为磅//这里好像是指废品 买的无效\"");
			this.ItemmaxValue = base.Config.Bind<int>("ItemConfig//物品设置", "maxItem", -1, "这里填物品最大数量 没测试过最大值 理论上限为21亿除以单个最大物品价值,-1为默认\n若不为-1,最小值为最大值的50%");
			this.ItemmaxValue_value = base.Config.Bind<int>("ItemConfig//物品设置", "maxItemValue", -1, "这里填单个物品最大价值 没测试过最大值理论上限为21亿除以最大物品数量,-1为默认\n若不为-1,最小值为最大值的50%");
			this.ItemNotNeedBattery = base.Config.Bind<bool>("ItemConfig//物品设置", "ItemNotNeedBattery", false, "是否开启所有物品不需要电池?如果开启填true //这里好像是指废品 买的无效");
			this.ItemNotNeedTwoHands = base.Config.Bind<bool>("ItemConfig//物品设置", "ItemNotNeedTwoHands", false, "是否开启所有物品不需要双手?如果开启填true //这里好像是指废品 买的无效");
			this.CanbeCarryingbyFlowerman = base.Config.Bind<bool>("PlayerConfig//玩家属性", "CanbeCarryingbyFlowerman", true, "能否被小黑抓住");
			this.MaxMonster.Value = ((this.MaxMonster.Value < 0) ? 0 : ((this.MaxMonster.Value > 100) ? 100 : this.MaxMonster.Value));
			bool flag2 = this.Monsterrarity.Value < 0 || this.Monsterrarity.Value > 100;
			if (flag2)
			{
				this.Monsterrarity.Value = -1;
			}
			bool flag3 = this.Itemrarity.Value < 0 || this.Itemrarity.Value > 100;
			if (flag3)
			{
				this.Itemrarity.Value = -1;
			}
			bool flag4 = this.Itemweight.Value < 0f || this.Itemweight.Value > 1000f;
			if (flag4)
			{
				this.Itemweight.Value = -1f;
			}
			bool flag5 = this.isNewVerion && this.IDcardTrue;
			if (flag5)
			{
				Debug.Log(this.isNewVerion.ToString() + " and " + this.IDcardTrue.ToString());
			}
			this.editplayer.PatchAll();
		}

		public const string GUID = "XiaoHai.plugin.CompanyPlus";

		public const string NAME = "xiaoHai_CompanyPlus";

		public const string VERSION = "1.0.4";

		private readonly Harmony harmony = new Harmony("XiaoHai.plugin.CompanyPlus.0");

		private readonly Harmony harmony1 = new Harmony("XiaoHai.plugin.CompanyPlus.1");

		private readonly Harmony editplayer = new Harmony("XiaoHai.plugin.CompanyPlus.4");

		public ConfigEntry<Key> memuhotkey;

		public ConfigEntry<int> deadlineDaysAmount;

		public ConfigEntry<int> startingCredits;

		public ConfigEntry<int> startingQuota;

		public ConfigEntry<int> Monsterrarity;

		public ConfigEntry<float> baseIncrease;

		public ConfigEntry<int> Itemrarity;

		public ConfigEntry<int> MaxMonster;

		public ConfigEntry<bool> AddMonstEreverytime;

		public ConfigEntry<float> Itemweight;

		public ConfigEntry<int> ItemmaxValue;

		public ConfigEntry<int> ItemmaxValue_value;

		public ConfigEntry<bool> ItemNotNeedBattery;

		public ConfigEntry<bool> JustGO;

		public ConfigEntry<bool> CanbeCarryingbyFlowerman;

		public ConfigEntry<bool> ItemNotNeedTwoHands;

		public ConfigEntry<string> SingleCode;

		public bool IDcardTrue = false;

		public string ExpirationTime;

		public static testplugin Instance;

		public static testplugin Instance1;

		public bool shouldRenderGUI = false;

		public bool isNewVerion = true;

		public static ManualLogSource mls;

		public static Xhgui xhgui;

		public string loginret;
	}
}
