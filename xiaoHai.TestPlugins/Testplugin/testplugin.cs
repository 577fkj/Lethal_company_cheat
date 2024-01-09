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
	// Token: 0x02000022 RID: 34
	[BepInPlugin("XiaoHai.plugin.CompanyPlus", "xiaoHai_CompanyPlus", "1.0.5")]
	public class testplugin : BaseUnityPlugin
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00008309 File Offset: 0x00006509
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00008310 File Offset: 0x00006510
		public static bool k { get; set; }

		// Token: 0x060000A6 RID: 166 RVA: 0x00008318 File Offset: 0x00006518
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
			this.IDcardTrue = this.Login();
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
				this.editplayer.PatchAll();
			}
		}

		// Token: 0x060000A7 RID: 167
		private bool Login()
		{
			this.code = "Crack By FKJ";
			Debug.Log("小海提醒你:已登录");
			Debug.Log(this.code);
			this.ExpirationTime = this.getUserExpirationTime();
			Debug.Log("小海提醒你:已登录,你到期时间为 " + this.ExpirationTime);
			this.isNewVerion = true;
			return true;
		}

		// Token: 0x060000A8 RID: 168
		private string getUserExpirationTime()
		{
			return "2099-12-31 00:00:00";
		}

		// Token: 0x0400008B RID: 139
		private Dictionary<int, string> errorMessages = new Dictionary<int, string>
		{
			{
				-135,
				"卡密不存在或者试用到期!"
			},
			{
				-5,
				"错误的参数,请检查参数是否正确."
			},
			{
				-6,
				"还未登录"
			},
			{
				-7,
				"私人服务器,没有权限进行登录."
			},
			{
				-8,
				"账户余额不足."
			},
			{
				-9,
				"非VIP用户的注册用户达到上限（VIP无此限制）."
			},
			{
				-10,
				"非Vip无法使用此接口"
			},
			{
				-11,
				"开启自动状态检测失败,还未登陆!"
			},
			{
				-12,
				"开启自动状态检测失败!"
			},
			{
				-13,
				"动态算法只支持独立服务器调用."
			},
			{
				-14,
				"错误的调用."
			},
			{
				-15,
				"频繁调用,请等待10分钟后再做尝试."
			},
			{
				-16,
				"接口未开启."
			},
			{
				-17,
				"错误的调用方式,请确认后台接口的调用方式."
			},
			{
				-18,
				"单IP频繁访问限制."
			},
			{
				-19,
				"接口调用失败,调用次数不足."
			},
			{
				-20,
				"变量数据不存在."
			},
			{
				-21,
				"机器码一样,无需转绑."
			},
			{
				-23,
				"此接口开启了强制算法,但是没使用."
			},
			{
				-24,
				"操作频繁,请重试!"
			},
			{
				-101,
				"用户名填写错误,必须以字母开头6-16位字母或数字!"
			},
			{
				-102,
				"用户不存在."
			},
			{
				-103,
				"请先登陆再调用此方法."
			},
			{
				-104,
				"密码填写错误,请输入6-16位密码！."
			},
			{
				-105,
				"邮箱填写错误,请正确输入邮箱,最大长度 32！."
			},
			{
				-106,
				"用户名重复."
			},
			{
				-107,
				"邮箱重复."
			},
			{
				-108,
				"新密码输入错误."
			},
			{
				-109,
				"用户名或密码错误"
			},
			{
				-110,
				"用户使用时间已到期"
			},
			{
				-111,
				"用户未在绑定的电脑上登陆."
			},
			{
				-112,
				"用户在别的地方登陆."
			},
			{
				-113,
				"过期时间有误."
			},
			{
				-114,
				"登录数据不存在"
			},
			{
				-115,
				"用户已被禁用."
			},
			{
				-116,
				"密码修改申请过于频繁."
			},
			{
				-117,
				"未输入机器码."
			},
			{
				-118,
				"重绑次数超过限制."
			},
			{
				-119,
				"使用天数不足,重绑失败."
			},
			{
				-120,
				"注册失败,注册次数超过限制."
			},
			{
				-121,
				"用户机器码不能超过32位."
			},
			{
				-122,
				"用户已经被删除"
			},
			{
				-123,
				"用户密码输入错误"
			},
			{
				-124,
				"用户登录数达到最大"
			},
			{
				-125,
				"错误的用户操作类别"
			},
			{
				-126,
				"过期时间变更记录创建失败"
			},
			{
				-127,
				"用户充值失败"
			},
			{
				-128,
				"用户数据超过最大限制"
			},
			{
				-129,
				"用户被开发者禁止使用,请咨询开发者是否被拉到黑名单"
			},
			{
				-131,
				"用户使用次数不足"
			},
			{
				-132,
				"用户使用点数不足"
			},
			{
				-133,
				"用户状态码错误"
			},
			{
				-134,
				"用户状态码不存在"
			},
			{
				-201,
				"程序不存在"
			},
			{
				-202,
				"程序密钥输入错误"
			},
			{
				-203,
				"程序版本号错误"
			},
			{
				-204,
				"程序版本不存在"
			},
			{
				-205,
				"用户未申请使用程序"
			},
			{
				-206,
				"程序版本需要更新"
			},
			{
				-207,
				"程序版本已停用"
			},
			{
				-208,
				"程序未开启后台接口功能."
			},
			{
				-209,
				"程序接口密码错误"
			},
			{
				-210,
				"程序停止新用户注册"
			},
			{
				-211,
				"程序不允许用户机器码转绑"
			},
			{
				-213,
				"非Vip无法使用此接口"
			},
			{
				-301,
				"卡密输入错误"
			},
			{
				-302,
				"卡密不存在"
			},
			{
				-303,
				"卡密已经使用"
			},
			{
				-304,
				"卡密已经过期"
			},
			{
				-305,
				"卡密已经冻结"
			},
			{
				-306,
				"卡密已经退换"
			},
			{
				-308,
				"卡密已经换卡"
			},
			{
				-401,
				"单码卡密错误"
			},
			{
				-402,
				"单码卡密机器码错误"
			},
			{
				-403,
				"单码卡密IP错误"
			},
			{
				-404,
				"单码卡密类型错误"
			},
			{
				-405,
				"单码卡密被禁用"
			},
			{
				-406,
				"单码卡密不存在"
			},
			{
				-407,
				"单码卡密未激活"
			},
			{
				-408,
				"单码卡密已经使用"
			},
			{
				-409,
				"单码充值卡密错误"
			},
			{
				-410,
				"单码卡密过期"
			},
			{
				-420,
				"单码卡密在别的电脑上登录"
			},
			{
				-421,
				"单码卡密超过最大登录数,如果确定已经下线,请等60分钟后重试"
			},
			{
				-422,
				"单码IP一样,无需转绑"
			},
			{
				-501,
				"单码管理信息错误"
			},
			{
				-502,
				"单码机器码转绑次数超过限制"
			},
			{
				-503,
				"单码机器码转绑后将过期"
			},
			{
				-504,
				"单码IP转绑次数超过限制"
			},
			{
				-505,
				"单码IP转绑后将过期"
			},
			{
				-506,
				"单码未开启机器码验证,无需转绑."
			},
			{
				-507,
				"单码未开启IP地址验证,无需转绑"
			}
		};

		// Token: 0x0400008C RID: 140
		public const string GUID = "XiaoHai.plugin.CompanyPlus";

		// Token: 0x0400008D RID: 141
		public const string NAME = "xiaoHai_CompanyPlus";

		// Token: 0x0400008E RID: 142
		public const string VERSION = "1.0.5";

		// Token: 0x04000090 RID: 144
		private readonly Harmony harmony = new Harmony("XiaoHai.plugin.CompanyPlus.0");

		// Token: 0x04000091 RID: 145
		private readonly Harmony harmony1 = new Harmony("XiaoHai.plugin.CompanyPlus.1");

		// Token: 0x04000092 RID: 146
		private readonly Harmony editplayer = new Harmony("XiaoHai.plugin.CompanyPlus.4");

		// Token: 0x04000093 RID: 147
		public ConfigEntry<Key> memuhotkey;

		// Token: 0x04000094 RID: 148
		public ConfigEntry<int> deadlineDaysAmount;

		// Token: 0x04000095 RID: 149
		public ConfigEntry<int> startingCredits;

		// Token: 0x04000096 RID: 150
		public ConfigEntry<int> startingQuota;

		// Token: 0x04000097 RID: 151
		public ConfigEntry<int> Monsterrarity;

		// Token: 0x04000098 RID: 152
		public ConfigEntry<float> baseIncrease;

		// Token: 0x04000099 RID: 153
		public ConfigEntry<int> Itemrarity;

		// Token: 0x0400009A RID: 154
		public ConfigEntry<int> MaxMonster;

		// Token: 0x0400009B RID: 155
		public ConfigEntry<bool> AddMonstEreverytime;

		// Token: 0x0400009C RID: 156
		public ConfigEntry<float> Itemweight;

		// Token: 0x0400009D RID: 157
		public ConfigEntry<int> ItemmaxValue;

		// Token: 0x0400009E RID: 158
		public ConfigEntry<int> ItemmaxValue_value;

		// Token: 0x0400009F RID: 159
		public ConfigEntry<bool> ItemNotNeedBattery;

		// Token: 0x040000A0 RID: 160
		public ConfigEntry<bool> JustGO;

		// Token: 0x040000A1 RID: 161
		public ConfigEntry<bool> CanbeCarryingbyFlowerman;

		// Token: 0x040000A2 RID: 162
		public ConfigEntry<bool> ItemNotNeedTwoHands;

		// Token: 0x040000A3 RID: 163
		public ConfigEntry<string> SingleCode;

		// Token: 0x040000A4 RID: 164
		public bool IDcardTrue = false;

		// Token: 0x040000A5 RID: 165
		public string ExpirationTime;

		// Token: 0x040000A6 RID: 166
		public static testplugin Instance;

		// Token: 0x040000A7 RID: 167
		public static testplugin Instance1;

		// Token: 0x040000A8 RID: 168
		public bool shouldRenderGUI = false;

		// Token: 0x040000A9 RID: 169
		public bool isNewVerion = true;

		// Token: 0x040000AA RID: 170
		public static ManualLogSource mls;

		// Token: 0x040000AB RID: 171
		public static Xhgui xhgui;

		// Token: 0x040000AC RID: 172
		private string code;

		// Token: 0x040000AD RID: 173
		public string loginret;

		// Token: 0x0200002F RID: 47
		public class WebPost
		{
			// Token: 0x060000D1 RID: 209 RVA: 0x00009864 File Offset: 0x00007A64
			private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
			{
				return true;
			}

			// Token: 0x060000D2 RID: 210 RVA: 0x00009878 File Offset: 0x00007A78
			public static string ApiPost(string url, IDictionary<string, string> parameters)
			{
				string result;
				try
				{
					Encoding utf = Encoding.UTF8;
					HttpWebRequest httpWebRequest = null;
					ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(testplugin.WebPost.CheckValidationResult);
					httpWebRequest = (WebRequest.Create(url.Trim()) as HttpWebRequest);
					httpWebRequest.ProtocolVersion = HttpVersion.Version10;
					httpWebRequest.Method = "POST";
					httpWebRequest.ContentType = "application/x-www-form-urlencoded";
					httpWebRequest.UserAgent = testplugin.WebPost.DefaultUserAgent;
					bool flag = parameters != null && parameters.Count != 0;
					if (flag)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (string text in parameters.Keys)
						{
							stringBuilder.AppendFormat((stringBuilder.Length > 0) ? "&{0}={1}" : "{0}={1}", text, parameters[text]);
						}
						byte[] bytes = utf.GetBytes(stringBuilder.ToString());
						using (Stream requestStream = httpWebRequest.GetRequestStream())
						{
							requestStream.Write(bytes, 0, bytes.Length);
						}
					}
					HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
					Stream responseStream = httpWebResponse.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream);
					string text2 = streamReader.ReadToEnd();
					result = text2;
				}
				catch (Exception ex)
				{
					Debug.Log("小海提醒你:An exception occurred: " + ex.ToString());
					throw;
				}
				return result;
			}

			// Token: 0x040000CE RID: 206
			private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
		}
	}
}
