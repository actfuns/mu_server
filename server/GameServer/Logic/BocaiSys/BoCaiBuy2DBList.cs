using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x02000074 RID: 116
	public class BoCaiBuy2DBList
	{
		// Token: 0x06000182 RID: 386 RVA: 0x00018B45 File Offset: 0x00016D45
		private BoCaiBuy2DBList()
		{
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00018B74 File Offset: 0x00016D74
		public static BoCaiBuy2DBList getInstance()
		{
			return BoCaiBuy2DBList.instance;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00018C64 File Offset: 0x00016E64
		public void AddData(BuyBoCai2SDB DbData, int num, bool isAdd = true)
		{
			lock (this.mutex)
			{
				this.dataList.RemoveAll((BoCaiBuy2DBData x) => x.data.m_RoleID == DbData.m_RoleID && x.data.strBuyValue.Equals(DbData.strBuyValue) && x.data.DataPeriods == DbData.DataPeriods);
				this.FailDataList.RemoveAll((BoCaiBuy2DBData x) => x.data.m_RoleID == DbData.m_RoleID && x.data.strBuyValue.Equals(DbData.strBuyValue) && x.data.DataPeriods == DbData.DataPeriods);
				if (isAdd)
				{
					BoCaiBuy2DBData data = default(BoCaiBuy2DBData);
					data.data = DbData;
					data.itemNum = num;
					this.dataList.Add(data);
				}
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00018D3C File Offset: 0x00016F3C
		public void SoptServer()
		{
			try
			{
				lock (this.mutex)
				{
					this.dataList.AddRange(this.FailDataList);
					foreach (BoCaiBuy2DBData item in this.dataList)
					{
						BuyBoCai2SDB DbData = item.data;
						if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, DbData, 0)))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩购买队列]  服务器关闭... 发送失败 {0}，{1},BocaiType={2},DataPeriods={3},BuyNum={4},BuyValue={5}，“<0是发送状态”={6},win={7},send={8}", new object[]
							{
								DbData.m_RoleName,
								DbData.m_RoleID,
								DbData.BocaiType,
								DbData.DataPeriods,
								DbData.BuyNum,
								DbData.strBuyValue,
								item.itemNum,
								DbData.IsWin,
								DbData.IsSend
							}), null, true);
							GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买博彩DB通信失败扣物品成功", DbData.m_RoleName, DbData.m_RoleName, "减少", item.itemNum, DbData.ZoneID, DbData.strUserID, -1, DbData.ServerId, "");
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩购买失败队列]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00018F44 File Offset: 0x00017144
		public void BigTimeUpData()
		{
			try
			{
				List<BoCaiBuy2DBData> tempList = new List<BoCaiBuy2DBData>();
				lock (this.mutex)
				{
					if ((TimeUtil.NowDateTime() - this.upFailDataTime).TotalMinutes > 1.0)
					{
						this.upFailDataTime = TimeUtil.NowDateTime();
						tempList.AddRange(this.FailDataList);
						this.FailDataList.Clear();
					}
					tempList.AddRange(this.dataList);
					this.dataList.Clear();
				}
				foreach (BoCaiBuy2DBData item in tempList)
				{
					BuyBoCai2SDB DbData = item.data;
					if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, DbData, 0)))
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩购买队列] 发送失败 稍后会继续执行 {0}，{1},BocaiType={2},DataPeriods={3},BuyNum={4},BuyValue={5}，“<0是发送状态”={6},win={7},send={8}", new object[]
						{
							DbData.m_RoleName,
							DbData.m_RoleID,
							DbData.BocaiType,
							DbData.DataPeriods,
							DbData.BuyNum,
							DbData.strBuyValue,
							item.itemNum,
							DbData.IsWin,
							DbData.IsSend
						}), null, true);
						lock (this.mutex)
						{
							this.FailDataList.Add(item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩购买失败队列]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x040002A2 RID: 674
		private static BoCaiBuy2DBList instance = new BoCaiBuy2DBList();

		// Token: 0x040002A3 RID: 675
		private DateTime upFailDataTime;

		// Token: 0x040002A4 RID: 676
		private object mutex = new object();

		// Token: 0x040002A5 RID: 677
		private List<BoCaiBuy2DBData> dataList = new List<BoCaiBuy2DBData>();

		// Token: 0x040002A6 RID: 678
		private List<BoCaiBuy2DBData> FailDataList = new List<BoCaiBuy2DBData>();
	}
}
