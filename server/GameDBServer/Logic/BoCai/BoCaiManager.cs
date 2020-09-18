using System;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic.BoCai
{
	
	internal class BoCaiManager : IManager, ICmdProcessor
	{
		
		public static BoCaiManager getInstance()
		{
			return BoCaiManager.instance;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			bool result;
			lock (this.mutex)
			{
				TCPCmdDispatcher.getInstance().registerProcessor(2082, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2083, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2084, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2085, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2086, BoCaiManager.getInstance());
				result = true;
			}
			return result;
		}

		
		public void LoadDataFromDB(DBManager DBMgr)
		{
			try
			{
				DateTime time = TimeUtil.NowDateTime();
				string Periods = TimeUtil.DataTimeToString(time, "yyMMdd");
				BoCaiDBOperator.DelData("t_bocai_shop", string.Format("Periods!='{0}'", Periods));
				for (int i = 1; i < 3; i++)
				{
					long DataPeriods = this.GetNowPeriods(time, i);
					BoCaiDBOperator.DelData("t_bocai_open_lottery", string.Format("IsAward=1 AND BocaiType={1} AND DataPeriods < {0}", DataPeriods, i));
					BoCaiDBOperator.DelData("t_bocai_buy_history", string.Format("IsSend=1 AND BocaiType={1} AND DataPeriods < {0}", DataPeriods, i));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private long GetNowPeriods(DateTime time, int type)
		{
			long result;
			if (type == 2)
			{
				result = Convert.ToInt64(string.Format("{0}1", TimeUtil.DataTimeToString(time.AddYears(-1), "yyMMdd")));
			}
			else if (type == 1)
			{
				result = Convert.ToInt64(string.Format("{0}001", TimeUtil.DataTimeToString(time.AddMonths(-6), "yyMMdd")));
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				lock (this.mutex)
				{
					if (nID == 2082)
					{
						this.updateBuy(client, nID, cmdParams, count);
					}
					else if (nID == 2083)
					{
						string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
						int cmdType = Convert.ToInt32(cmdData.Split(new char[]
						{
							','
						})[0]);
						if (cmdType == 1)
						{
							this.getBoCaiBuyList(client, nID, cmdData, false);
						}
						else if (cmdType == 3)
						{
							this.getBoCaiOpenList(client, nID, cmdData);
						}
						else if (cmdType == 2)
						{
							this.getBoCaiBuyList(client, nID, cmdData, true);
						}
					}
					else if (nID == 2084)
					{
						this.updateOpen(client, nID, cmdParams, count);
					}
					else if (nID == 2085)
					{
						this.getShopList(client, nID, cmdParams, count);
					}
					else if (nID == 2086)
					{
						this.setShopData(client, nID, cmdParams, count);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]id err id={0}", nID), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private void updateBuy(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string msgStr = "false";
			try
			{
				BuyBoCai2SDB Data = DataHelper.BytesToObject<BuyBoCai2SDB>(cmdParams, 0, count);
				msgStr = BoCaiDBOperator.ReplaceBuyBoCai(Data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd(nID, msgStr);
		}

		
		private void updateOpen(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string msgStr = "false";
			try
			{
				OpenLottery Data = DataHelper.BytesToObject<OpenLottery>(cmdParams, 0, count);
				msgStr = BoCaiDBOperator.ReplaceOpenLottery(Data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd(nID, msgStr);
		}

		
		private void getBoCaiBuyList(GameServerClient client, int nID, string cmdData, bool isNoSend)
		{
			GetBuyBoCaiList msgData = new GetBuyBoCaiList();
			msgData.Flag = false;
			try
			{
				string[] files = cmdData.Split(new char[]
				{
					','
				});
				int type = Convert.ToInt32(files[2]);
				long DataPeriods = Convert.ToInt64(files[1]);
				BoCaiDBOperator.SelectBuyBoCai(type, DataPeriods, out msgData.ItemList, isNoSend);
				if (null != msgData.ItemList)
				{
					msgData.Flag = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<GetBuyBoCaiList>(nID, msgData);
		}

		
		private void getBoCaiOpenList(GameServerClient client, int nID, string cmdData)
		{
			GetOpenList msgData = new GetOpenList();
			msgData.Flag = false;
			try
			{
				string[] files = cmdData.Split(new char[]
				{
					','
				});
				int type = Convert.ToInt32(files[1]);
				long DataPeriods = Convert.ToInt64(files[1]);
				msgData.MaxDataPeriods = BoCaiDBOperator.GetMaxData(type);
				BoCaiDBOperator.SelectOpenLottery(type, out msgData.ItemList);
				if (msgData.MaxDataPeriods >= 0L && null != msgData.ItemList)
				{
					msgData.Flag = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<GetOpenList>(nID, msgData);
		}

		
		private void getShopList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			BoCaiShopDBData msgData = new BoCaiShopDBData();
			try
			{
				DateTime time = TimeUtil.NowDateTime();
				string Periods = TimeUtil.DataTimeToString(time, "yyMMdd");
				BoCaiDBOperator.SelectBoCaiShop(Periods, out msgData.ItemList);
				msgData.Flag = (null != msgData.ItemList);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd<BoCaiShopDBData>(nID, msgData);
		}

		
		private void setShopData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string msgStr = "false";
			try
			{
				BoCaiShopDB Data = DataHelper.BytesToObject<BoCaiShopDB>(cmdParams, 0, count);
				msgStr = BoCaiDBOperator.ReplaceBoCaiShop(Data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd(nID, msgStr);
		}

		
		private static BoCaiManager instance = new BoCaiManager();

		
		private object mutex = new object();
	}
}
