using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004A9 RID: 1193
	public class RegressActiveSignGift : Activity
	{
		// Token: 0x06001632 RID: 5682 RVA: 0x0015B410 File Offset: 0x00159610
		public bool Init()
		{
			this.ActivityType = 111;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string fileName = Global.GameResPath("Config\\HuiGuiLoginNumGift.xml");
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RegressActiveSignGiftXML Regress = new RegressActiveSignGiftXML();
					Regress.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					Regress.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HuoDongLevel"));
					Regress.TimeOl = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TimeOl"));
					string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
					if (!string.IsNullOrEmpty(goods))
					{
						string[] fields = goods.Split(new char[]
						{
							'|'
						});
						if (fields.Length > 0)
						{
							Regress.GoodsID1 = GoodsHelper.ParseGoodsDataList(fields, fileName);
						}
					}
					goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
					if (!string.IsNullOrEmpty(goods))
					{
						string[] fields = goods.Split(new char[]
						{
							'|'
						});
						if (fields.Length > 0)
						{
							Regress.GoodsID2 = GoodsHelper.ParseGoodsDataList(fields, fileName);
						}
					}
					this.regressActiveSignGiftXML.Add(Regress.ID, Regress);
				}
				if (this.regressActiveSignGiftXML == null)
				{
					return false;
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return true;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0015B650 File Offset: 0x00159850
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
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
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0015B710 File Offset: 0x00159910
		public bool GetAwardGoodsList(GameClient client, int Level, int Day, out List<GoodsData> OutGoodsData, out int DBDay)
		{
			DBDay = 0;
			OutGoodsData = new List<GoodsData>();
			foreach (RegressActiveSignGiftXML iter in this.regressActiveSignGiftXML.Values)
			{
				if (iter.HuoDongLevel == Level)
				{
					if (iter.TimeOl == Day && iter.GoodsID1 != null)
					{
						OutGoodsData.AddRange(iter.GoodsID1);
						DBDay = iter.TimeOl;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0015B7C4 File Offset: 0x001599C4
		public bool GiveAward(GameClient client, List<GoodsData> goodsData)
		{
			bool result;
			if (goodsData == null)
			{
				result = false;
			}
			else
			{
				foreach (GoodsData it in goodsData)
				{
					if (Global.GetGoodsRebornEquip(it.GoodsID) == 1)
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.GoodsID, it.GCount, it.Quality, it.Props, it.Forge_level, it.Binding, 15000, it.Jewellist, true, 1, "三周年每日签到奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.GoodsID, it.GCount, it.Quality, it.Props, it.Forge_level, it.Binding, 0, it.Jewellist, true, 1, "三周年每日签到奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x04001FC0 RID: 8128
		protected const string RegressActiveSignGiftXml = "Config\\HuiGuiLoginNumGift.xml";

		// Token: 0x04001FC1 RID: 8129
		private Dictionary<int, RegressActiveSignGiftXML> regressActiveSignGiftXML = new Dictionary<int, RegressActiveSignGiftXML>();
	}
}
