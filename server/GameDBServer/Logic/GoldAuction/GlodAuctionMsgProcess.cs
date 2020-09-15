using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x0200013C RID: 316
	internal class GlodAuctionMsgProcess : IManager, ICmdProcessor
	{
		// Token: 0x0600053A RID: 1338 RVA: 0x0002BA04 File Offset: 0x00029C04
		public static GlodAuctionMsgProcess getInstance()
		{
			return GlodAuctionMsgProcess.instance;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0002BA1C File Offset: 0x00029C1C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0002BA30 File Offset: 0x00029C30
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0002BA44 File Offset: 0x00029C44
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0002BA58 File Offset: 0x00029C58
		public bool startup()
		{
			lock (this.AuctionMsgMutex)
			{
				TCPCmdDispatcher.getInstance().registerProcessor(2080, GlodAuctionMsgProcess.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2081, GlodAuctionMsgProcess.getInstance());
			}
			return true;
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0002BACC File Offset: 0x00029CCC
		public void LoadDataFromDB(DBManager DBMgr)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					GoldAuctionDB.DelData(string.Format("UpDBWay='Del' AND UpdateTime < '{0}'", DateTime.Now.AddDays(-365.0).ToString("yyyy-MM-dd HH:mm:ss")));
					this.AuctionDict.Clear();
					for (int i = 1; i < 3; i++)
					{
						List<GoldAuctionDBItem> dList;
						if (GoldAuctionDB.Select(out dList, i))
						{
							this.AuctionDict.Add(i, dList);
							LjlLog.WriteLog(LogTypes.Info, string.Format("初始化金团数据type={0},len={1}", ((AuctionOrderEnum)i).ToString(), dList.Count), "");
						}
						else
						{
							LjlLog.WriteLog(LogTypes.Info, string.Format("初始化金团数据type={0},false", ((AuctionOrderEnum)i).ToString()), "");
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0002BC10 File Offset: 0x00029E10
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					if (nID == 2080)
					{
						this.GetGlodAuction(client, nID, cmdParams, count);
					}
					else if (nID == 2081)
					{
						this.SetGlodAuction(client, nID, cmdParams, count);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]id err id={0}", nID), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0002BCE0 File Offset: 0x00029EE0
		private void GetGlodAuction(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			GetAuctionDBData msgData = new GetAuctionDBData();
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				int type = Convert.ToInt32(cmdData);
				List<GoldAuctionDBItem> tempList;
				if (3 <= type || 0 >= type)
				{
					msgData.Flag = false;
					LjlLog.WriteLog(LogTypes.Error, string.Format("GetGlodAuction 金团类型错误{0} type=", type), "");
				}
				else if (this.AuctionDict.TryGetValue(type, out tempList))
				{
					msgData.Flag = CopyData.CopyAuctionDBItem(tempList, ref msgData.ItemList);
				}
				else
				{
					msgData.Flag = GoldAuctionDB.Select(out msgData.ItemList, type);
					if (msgData.Flag)
					{
						tempList = new List<GoldAuctionDBItem>();
						if (CopyData.CopyAuctionDBItem(msgData.ItemList, ref tempList))
						{
							this.AuctionDict.Add(type, tempList);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			client.sendCmd<GetAuctionDBData>(nID, msgData);
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0002BE48 File Offset: 0x0002A048
		private void SetGlodAuction(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string msgStr = "false";
			try
			{
				GoldAuctionDBItem tempData = DataHelper.BytesToObject<GoldAuctionDBItem>(cmdParams, 0, count);
				if (3 <= tempData.AuctionType || 0 >= tempData.AuctionType)
				{
					LjlLog.WriteLog(LogTypes.Error, string.Format("SetGlodAuction 金团类型错误 type={0}", tempData.AuctionType), "");
				}
				else if (tempData.UpDBWay == 2)
				{
					msgStr = GoldAuctionDB.Insert(tempData).ToString();
					LjlLog.WriteLog(LogTypes.Info, string.Format("Add 金团新物品 type={0}, time={1}, AuctionSource={2}, upway={3}, msgDbStr={4}", new object[]
					{
						tempData.AuctionType,
						tempData.ProductionTime,
						tempData.AuctionSource,
						((DBAuctionUpEnum)tempData.UpDBWay).ToString(),
						msgStr
					}), "");
				}
				else
				{
					msgStr = GoldAuctionDB.Update(tempData).ToString();
				}
				client.sendCmd(nID, msgStr);
				if (msgStr.Equals("True"))
				{
					List<GoldAuctionDBItem> tempList;
					if (this.AuctionDict.TryGetValue(tempData.OldAuctionType, out tempList))
					{
						tempList.RemoveAll((GoldAuctionDBItem x) => x.ProductionTime == tempData.ProductionTime && x.AuctionSource == tempData.AuctionSource);
					}
					if (this.AuctionDict.TryGetValue(tempData.AuctionType, out tempList) && tempData.UpDBWay != 1)
					{
						tempList.Add(tempData);
					}
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
		}

		// Token: 0x04000805 RID: 2053
		private static GlodAuctionMsgProcess instance = new GlodAuctionMsgProcess();

		// Token: 0x04000806 RID: 2054
		private object AuctionMsgMutex = new object();

		// Token: 0x04000807 RID: 2055
		private Dictionary<int, List<GoldAuctionDBItem>> AuctionDict = new Dictionary<int, List<GoldAuctionDBItem>>();
	}
}
