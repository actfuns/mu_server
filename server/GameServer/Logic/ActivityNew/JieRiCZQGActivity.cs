using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200019F RID: 415
	public class JieRiCZQGActivity : Activity, IEventListener
	{
		// Token: 0x060004E2 RID: 1250 RVA: 0x00042A50 File Offset: 0x00040C50
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				if (this.CZQGZhiGouIDSet.Contains(obj.ChargeItemConfig.ChargeItemID))
				{
					List<JieriCZQGData> list = this.BuildChongZhiQiangGouInfoForClient(obj.Player);
					obj.Player.sendCmd<List<JieriCZQGData>>(1620, list, false);
				}
			}
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00042AB4 File Offset: 0x00040CB4
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x00042AC8 File Offset: 0x00040CC8
		public bool Init()
		{
			try
			{
				string fileName = "Config/JieRiGifts/JieRiChongZhiQiangGou.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						JieriCZQGConfigData item = new JieriCZQGConfigData();
						item.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						item.ZhiGouID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhiGouID");
						item.SinglePurchase = (int)Global.GetSafeAttributeLong(xmlItem, "SinglePurchase");
						string GoodsOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						string GoodsTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(item.ZhiGouID, item.SinglePurchase, GoodsOne, GoodsTwo, string.Format("充值抢购 ID={0}", item.ID));
						string DayString = Global.GetSafeAttributeStr(xmlItem, "Day");
						string[] DayFiled = DayString.Split(new char[]
						{
							','
						});
						if (DayFiled.Length == 2)
						{
							int SpanFromDay = Global.SafeConvertToInt32(DayFiled[0]) - 1;
							int SpanToDay = Global.SafeConvertToInt32(DayFiled[1]);
							DateTime startTime = DateTime.Parse(this.FromDate);
							item.FromDate = Global.GetAddDaysDataTime(startTime, SpanFromDay, true);
							item.ToDate = Global.GetAddDaysDataTime(startTime, SpanToDay, true);
						}
						this.CZQGConfigDict[item.ID] = item;
						this.CZQGZhiGouIDSet.Add(item.ZhiGouID);
					}
				}
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiQiangGou.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00042DA0 File Offset: 0x00040FA0
		public List<JieriCZQGData> BuildChongZhiQiangGouInfoForClient(GameClient client)
		{
			List<JieriCZQGData> list = new List<JieriCZQGData>();
			List<JieriCZQGData> result;
			if (!this.InActivityTime())
			{
				result = list;
			}
			else
			{
				foreach (KeyValuePair<int, JieriCZQGConfigData> kvp in this.CZQGConfigDict)
				{
					if (!(TimeUtil.NowDateTime() < kvp.Value.FromDate) && !(TimeUtil.NowDateTime() > kvp.Value.ToDate))
					{
						JieriCZQGData item = new JieriCZQGData();
						item.ID = kvp.Value.ID;
						lock (client.ClientData.ChargeItemPurchaseDict)
						{
							Dictionary<int, int> PurchaseDict = client.ClientData.ChargeItemPurchaseDict;
							PurchaseDict.TryGetValue(kvp.Value.ZhiGouID, out item.PurchaseNum);
						}
						list.Add(item);
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x0400091F RID: 2335
		protected Dictionary<int, JieriCZQGConfigData> CZQGConfigDict = new Dictionary<int, JieriCZQGConfigData>();

		// Token: 0x04000920 RID: 2336
		protected HashSet<int> CZQGZhiGouIDSet = new HashSet<int>();
	}
}
