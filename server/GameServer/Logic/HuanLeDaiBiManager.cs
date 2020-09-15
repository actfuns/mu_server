using System;
using GameServer.Logic.BocaiSys;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000083 RID: 131
	public class HuanLeDaiBiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x000210A8 File Offset: 0x0001F2A8
		private HuanLeDaiBiManager()
		{
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x000210B4 File Offset: 0x0001F2B4
		public static HuanLeDaiBiManager GetInstance()
		{
			return HuanLeDaiBiManager.instance;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x000210CC File Offset: 0x0001F2CC
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000210E0 File Offset: 0x0001F2E0
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x000210F4 File Offset: 0x0001F2F4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00021108 File Offset: 0x0001F308
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0002111C File Offset: 0x0001F31C
		public bool startup()
		{
			this.GetHuanLeDaiBi(0);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2087, 3, 3, HuanLeDaiBiManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00021150 File Offset: 0x0001F350
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (nID == 2087)
				{
					int info = 0;
					int ExchangeNum = Convert.ToInt32(cmdParams[1]);
					int type = Convert.ToInt32(cmdParams[2]);
					if (ExchangeNum < 1)
					{
						info = 11;
					}
					else if (type == 1)
					{
						this.ExchangeHuanlebi(client, ExchangeNum, ref info);
					}
					else if (type == 2)
					{
						this.ExchangeLuckStar(client, ExchangeNum, ref info);
					}
					else
					{
						info = 19;
					}
					client.sendCmd(nID, info.ToString(), false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00021224 File Offset: 0x0001F424
		private void ExchangeLuckStar(GameClient client, int ExchangeNum, ref int mgsData)
		{
			try
			{
				if (!GameManager.ClientMgr.SubUserMoney(client, ExchangeNum, "兑换幸运之星", true, true, true, true, DaiBiSySType.None))
				{
					mgsData = 12;
				}
				else if (!GameManager.ClientMgr.ModifyLuckStarValue(client, ExchangeNum, "兑换幸运之星", false, DaiBiSySType.None))
				{
					mgsData = 20;
				}
				else if (!GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, ExchangeNum, "兑换幸运之星", true, true, false))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_幸运之星] 兑换幸运之星 添加魔晶失败id={0},name={1}，num={2}", client.ClientData.RoleID, client.ClientData.RoleName, ExchangeNum), null, true);
				}
			}
			catch (Exception ex)
			{
				mgsData = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_幸运之星]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x000212FC File Offset: 0x0001F4FC
		private void ExchangeHuanlebi(GameClient client, int ExchangeNum, ref int mgsData)
		{
			try
			{
				GoodsData Goods = this.GetHuanLeDaiBi(ExchangeNum);
				mgsData = 0;
				if (ExchangeNum < 1)
				{
					mgsData = 11;
				}
				else if (client.ClientData.UserMoney < ExchangeNum)
				{
					mgsData = 12;
				}
				else if (!Global.CanAddGoods3(client, Goods.GoodsID, ExchangeNum, Goods.Binding, "1900-01-01 12:00:00", true))
				{
					mgsData = 13;
				}
				if (mgsData == 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(client, ExchangeNum, "兑换欢乐币", true, true, true, true, DaiBiSySType.None))
					{
						mgsData = 12;
					}
					else
					{
						int ret = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, Goods.GoodsID, Goods.GCount, Goods.Quality, Goods.Props, Goods.Forge_level, Goods.Binding, Goods.Site, Goods.Jewellist, true, 1, "兑换欢乐币", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_欢乐代币]兑换欢乐币ret={1}，num={0},,id={2},name={3}", new object[]
						{
							ExchangeNum,
							ret,
							client.ClientData.RoleID,
							client.ClientData.RoleName
						}), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				mgsData = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00021494 File Offset: 0x0001F694
		public GoodsData GetHuanLeDaiBi(int GCount = 0)
		{
			try
			{
				string str = GameManager.systemParamsList.GetParamValueByName("HuanLeDaiBi");
				GoodsData data = GlobalNew.ParseGoodsData(str);
				data.GCount = GCount;
				return data;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "欢乐代币格式不对", null, true);
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00021510 File Offset: 0x0001F710
		public bool UseHuanledaibi(GameClient client, int UseNum)
		{
			try
			{
				int goodsid = this.GetHuanLeDaiBi(0).GoodsID;
				if (this.HuanledaibiEnough(client, UseNum))
				{
					bool flag;
					return GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsid, UseNum, false, out flag, out flag, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000215AC File Offset: 0x0001F7AC
		public bool HuanledaibiEnough(GameClient client, int UseNum)
		{
			try
			{
				int goodsid = this.GetHuanLeDaiBi(0).GoodsID;
				return Global.GetTotalGoodsCountByID(client, goodsid) >= UseNum;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00021610 File Offset: 0x0001F810
		public bool HuanledaibiReplaceEnough(GameClient client, int UseNum, DaiBiSySType type)
		{
			try
			{
				if (!BoCaiConfigMgr.CanReplaceMoney(type))
				{
					return false;
				}
				return this.HuanledaibiEnough(client, UseNum);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00021670 File Offset: 0x0001F870
		public bool UseReplaceMoney(GameClient client, int UseNum, DaiBiSySType type, string info, bool isLuckStar = false)
		{
			try
			{
				if (UseNum == 0)
				{
					return true;
				}
				if (!BoCaiConfigMgr.CanReplaceMoney(type))
				{
					return false;
				}
				if (this.UseHuanledaibi(client, UseNum))
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "欢乐代币代替钻石", info, client.ClientData.RoleName, "系统", "减少", UseNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_欢乐代币]{0}", string.Format("msg={0},subNum={1},type={2},isLuckStar={3},id={4},name={5}", new object[]
					{
						info,
						UseNum,
						type,
						isLuckStar,
						client.ClientData.RoleID,
						client.ClientData.RoleName
					})), null, true);
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x04000305 RID: 773
		private static HuanLeDaiBiManager instance = new HuanLeDaiBiManager();
	}
}
