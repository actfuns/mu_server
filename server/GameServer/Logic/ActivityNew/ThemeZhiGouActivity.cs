using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class ThemeZhiGouActivity : Activity, IEventListener
	{
		
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ThemeActivityZhiGou.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ThemeActivityZhiGou.xml"));
				if (null == xml)
				{
					return false;
				}
				this.ActivityType = 150;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						ThemeZhiGouConfig config = new ThemeZhiGouConfig();
						config.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						config.ZhiGouID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhiGouID");
						config.SinglePurchase = (int)Global.GetSafeAttributeLong(xmlItem, "SinglePurchase");
						int[] dayFromEnd = Global.GetSafeAttributeIntArray(xmlItem, "Day", -1, ',');
						if (dayFromEnd.Length == 2)
						{
							config.FromDate = Global.GetKaiFuTime().AddDays((double)(dayFromEnd[0] - 1));
							config.ToDate = Global.GetKaiFuTime().AddDays((double)dayFromEnd[1]);
						}
						string GoodsOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						string GoodsTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						this.ThemeZhiGouConfigData[config.ID] = config;
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(config.ZhiGouID, config.SinglePurchase, GoodsOne, GoodsTwo, string.Format("主题服直购 ID={0}", config.ID));
					}
				}
				base.PredealDateTime();
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
				if (this.CheckValidChargeItem(obj.ChargeItemConfig.ChargeItemID))
				{
					Dictionary<int, int> ZhiGouInfoDict = this.BuildThemeZhiGouInfoForClient(obj.Player);
					obj.Player.sendCmd<Dictionary<int, int>>(906, ZhiGouInfoDict, false);
					if (obj.Player._IconStateMgr.CheckThemeZhiGou(obj.Player))
					{
						obj.Player._IconStateMgr.SendIconStateToClient(obj.Player);
					}
				}
			}
		}

		
		public Dictionary<int, int> BuildThemeZhiGouInfoForClient(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			Dictionary<int, int> ZhiGouInfoDict = new Dictionary<int, int>();
			foreach (ThemeZhiGouConfig item in this.ThemeZhiGouConfigData.Values)
			{
				if (!(now < item.FromDate) && !(now > item.ToDate))
				{
					ZhiGouInfoDict[item.ID] = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, item.ZhiGouID);
				}
			}
			return ZhiGouInfoDict;
		}

		
		public bool CheckValidChargeItem(int zhigouID)
		{
			DateTime now = TimeUtil.NowDateTime();
			foreach (ThemeZhiGouConfig item in this.ThemeZhiGouConfigData.Values)
			{
				if (!(now < item.FromDate.AddDays(-1.0)) && !(now > item.ToDate))
				{
					if (item.ZhiGouID == zhigouID)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public bool CheckClientCanBuy(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			foreach (ThemeZhiGouConfig item in this.ThemeZhiGouConfigData.Values)
			{
				if (!(now < item.FromDate) && !(now > item.ToDate))
				{
					int PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, item.ZhiGouID);
					if (item.SinglePurchase <= 0 || PurNum < item.SinglePurchase)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		protected const string ThemeActivityZhiGouData_fileName = "Config/ThemeActivityZhiGou.xml";

		
		protected Dictionary<int, ThemeZhiGouConfig> ThemeZhiGouConfigData = new Dictionary<int, ThemeZhiGouConfig>();
	}
}
