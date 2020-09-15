using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000047 RID: 71
	public class OneDollarBuyActivity : Activity, IEventListener
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x0000F300 File Offset: 0x0000D500
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000F314 File Offset: 0x0000D514
		public bool Init()
		{
			try
			{
				Dictionary<int, int> OpenStateDict = new Dictionary<int, int>();
				string strPlatformOpen = GameManager.systemParamsList.GetParamValueByName("OneDollarBuyOpen");
				if (!string.IsNullOrEmpty(strPlatformOpen))
				{
					string[] Fields = strPlatformOpen.Split(new char[]
					{
						'|'
					});
					foreach (string dat in Fields)
					{
						string[] State = dat.Split(new char[]
						{
							','
						});
						if (State.Length == 2)
						{
							OpenStateDict[Global.SafeConvertToInt32(State[0])] = Global.SafeConvertToInt32(State[1]);
						}
					}
				}
				OpenStateDict.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/OneDollarBuy.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/OneDollarBuy.xml"));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("OneDollarBuy");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "BeginTime");
					this.ToDate = Global.GetSafeAttributeStr(args, "FinishTime");
					this.ActivityType = 45;
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.OneDollarBuyConfigData.ID = (int)Global.GetSafeAttributeLong(args, "ID");
					DateTime.TryParse(this.FromDate, out this.OneDollarBuyConfigData.FromDate);
					DateTime.TryParse(this.ToDate, out this.OneDollarBuyConfigData.ToDate);
					this.OneDollarBuyConfigData.ZhiGouID = (int)Global.GetSafeAttributeLong(args, "ZhiGouID");
					this.OneDollarBuyConfigData.SinglePurchase = (int)Global.GetSafeAttributeLong(args, "SinglePurchase");
					string GoodsOne = Global.GetSafeAttributeStr(args, "GoodsID1");
					string GoodsTwo = Global.GetSafeAttributeStr(args, "GoodsID2");
					UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(this.OneDollarBuyConfigData.ZhiGouID, this.OneDollarBuyConfigData.SinglePurchase, GoodsOne, GoodsTwo, string.Format("1元直购 ID={0}", this.OneDollarBuyConfigData.ID));
				}
				base.PredealDateTime();
				if (!this.InActivityTime())
				{
					GameManager.ClientMgr.NotifyAllActivityState(8, 0, "", "", 0);
				}
				else
				{
					GameManager.ClientMgr.NotifyAllActivityState(8, this.PlatformOpenStateVavle, "", "", 0);
				}
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/OneDollarBuy.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000F5FC File Offset: 0x0000D7FC
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				if (this.OneDollarBuyConfigData.ZhiGouID == obj.ChargeItemConfig.ChargeItemID)
				{
					string cmd = this.BuildOneDollarBuyActInfoForClient(obj.Player);
					obj.Player.sendCmd(1621, cmd, false);
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000F664 File Offset: 0x0000D864
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					8,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					8,
					this.PlatformOpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000F724 File Offset: 0x0000D924
		public string BuildOneDollarBuyActInfoForClient(GameClient client)
		{
			int PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, this.OneDollarBuyConfigData.ZhiGouID);
			return string.Format("{0}:{1}", this.OneDollarBuyConfigData.ID, PurNum);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000F770 File Offset: 0x0000D970
		public bool CheckClientCanBuy(GameClient client)
		{
			bool result;
			if (0 == this.PlatformOpenStateVavle)
			{
				result = false;
			}
			else
			{
				int PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, this.OneDollarBuyConfigData.ZhiGouID);
				result = (PurNum < this.OneDollarBuyConfigData.SinglePurchase);
			}
			return result;
		}

		// Token: 0x0400018A RID: 394
		protected const string OneDollarBuyActivityData_fileName = "Config/OneDollarBuy.xml";

		// Token: 0x0400018B RID: 395
		protected OneDollarBuyConfig OneDollarBuyConfigData = new OneDollarBuyConfig();

		// Token: 0x0400018C RID: 396
		protected int PlatformOpenStateVavle = 0;
	}
}
