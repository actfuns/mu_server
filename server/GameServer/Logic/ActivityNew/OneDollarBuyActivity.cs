using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class OneDollarBuyActivity : Activity, IEventListener
	{
		
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		
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

		
		public string BuildOneDollarBuyActInfoForClient(GameClient client)
		{
			int PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, this.OneDollarBuyConfigData.ZhiGouID);
			return string.Format("{0}:{1}", this.OneDollarBuyConfigData.ID, PurNum);
		}

		
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

		
		protected const string OneDollarBuyActivityData_fileName = "Config/OneDollarBuy.xml";

		
		protected OneDollarBuyConfig OneDollarBuyConfigData = new OneDollarBuyConfig();

		
		protected int PlatformOpenStateVavle = 0;
	}
}
