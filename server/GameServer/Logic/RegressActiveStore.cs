using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004AF RID: 1199
	public class RegressActiveStore : Activity
	{
		// Token: 0x06001647 RID: 5703 RVA: 0x0015CAE4 File Offset: 0x0015ACE4
		public bool Init()
		{
			this.ActivityType = 114;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string fileName = Global.GameResPath("Config\\HuiGuiStore.xml");
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
					RegressActiveStoreXML Regress = new RegressActiveStoreXML();
					Regress.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					Regress.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HuoDongLevel"));
					Regress.Day = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Day"));
					Regress.OrigPrice = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "OrigPrice"));
					Regress.Price = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Price"));
					Regress.SinglePurchase = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "SinglePurchase"));
					string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID");
					if (!string.IsNullOrEmpty(goods))
					{
						string[] fields = goods.Split(new char[]
						{
							'|'
						});
						if (fields.Length > 0)
						{
							Regress.GoodsID = GoodsHelper.ParseGoodsDataList(fields, fileName);
						}
					}
					this.regressActiveStoreXML.Add(Regress.ID, Regress);
				}
				if (this.regressActiveStoreXML == null)
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

		// Token: 0x06001648 RID: 5704 RVA: 0x0015CD14 File Offset: 0x0015AF14
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

		// Token: 0x06001649 RID: 5705 RVA: 0x0015CDD4 File Offset: 0x0015AFD4
		public bool RegressStoreGoodsBuyCheck(GameClient client, int ConfID, int Level, int Day, int GoodsID, int Count, string stage, out int needYuanBao, out int Sum, out GoodsData goodData)
		{
			needYuanBao = 0;
			Sum = 0;
			goodData = null;
			RegressActiveStoreXML Conf;
			bool result;
			if (!this.regressActiveStoreXML.TryGetValue(ConfID, out Conf))
			{
				result = false;
			}
			else if (Conf.HuoDongLevel != Level || Conf.Day != Day)
			{
				result = false;
			}
			else if (Count <= 0)
			{
				result = false;
			}
			else
			{
				GoodsData BuyGood = null;
				foreach (GoodsData it in Conf.GoodsID)
				{
					if (it.GoodsID == GoodsID)
					{
						BuyGood = it;
					}
				}
				if (BuyGood == null)
				{
					result = false;
				}
				else
				{
					string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ConfID, stage);
					string[] dbResult;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14134, GetInfoStr, out dbResult, 0))
					{
						result = false;
					}
					else if (dbResult == null || dbResult.Length != 4 || Convert.ToInt32(dbResult[1]) != 0)
					{
						result = false;
					}
					else
					{
						int DBSum = Convert.ToInt32(dbResult[3]);
						int SumPast = Conf.SinglePurchase - DBSum;
						if (SumPast < Count)
						{
							result = false;
						}
						else
						{
							needYuanBao = Count * Conf.Price;
							Sum = DBSum + Count;
							goodData = BuyGood;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04001FD9 RID: 8153
		protected const string RegressActiveStoreXml = "Config\\HuiGuiStore.xml";

		// Token: 0x04001FDA RID: 8154
		private Dictionary<int, RegressActiveStoreXML> regressActiveStoreXML = new Dictionary<int, RegressActiveStoreXML>();
	}
}
