using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000796 RID: 1942
	public class ShengXiaoGuessManager
	{
		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06003292 RID: 12946 RVA: 0x002CE390 File Offset: 0x002CC590
		public int GuessMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x002CE3A8 File Offset: 0x002CC5A8
		public void Init()
		{
			this.InitLegalGuessKeys();
			this.ReloadConfig(true);
			this.Reset();
		}

		// Token: 0x06003294 RID: 12948 RVA: 0x002CE3C4 File Offset: 0x002CC5C4
		public void ReloadConfig(bool throwAble = false)
		{
			int[] paramsArr = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengXiaoGuessParams", ',');
			if (paramsArr.Length != 6)
			{
				if (throwAble)
				{
					throw new Exception("SystemParmas.xml中生肖竞猜参数ShengXiaoGuessParams配置个数不对");
				}
			}
			else
			{
				this.MapCode = paramsArr[0];
				this.WaitingEnterSecs = paramsArr[1];
				this.NeedGoodsID = paramsArr[2];
				this.SingleGoodsToYuanBaoNum = paramsArr[3];
				this.MaxMortgageForOnce = paramsArr[4];
				this.GateGoldForBroadcast = paramsArr[5];
				this.LegalServerLines.Clear();
				int[] lineArr = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengXiaoGuessLines", ',');
				foreach (int line in lineArr)
				{
					this.LegalServerLines.Add(line);
				}
			}
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x002CE490 File Offset: 0x002CC690
		protected void InitLegalGuessKeys()
		{
			this.LegalGuessKeyList.Clear();
			for (int i = 0; i < 12; i++)
			{
				this.LegalGuessKeyList.Add(1 << i);
			}
			for (int i = 0; i < 6; i++)
			{
				this.LegalGuessKeyList.Add(3 << 2 * i);
			}
			for (int i = 0; i < 6; i++)
			{
				if (i < 5)
				{
					this.LegalGuessKeyList.Add(15 << 2 * i);
				}
				else
				{
					this.LegalGuessKeyList.Add(3075);
				}
			}
			for (int i = 0; i < 2; i++)
			{
				this.LegalGuessKeyList.Add(63 << 6 * i);
			}
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x002CE568 File Offset: 0x002CC768
		protected void Reset()
		{
			this.GuessStates = ShengXiaoGuessStates.NoMortgage;
			lock (this.GuessItemListDict)
			{
				this.GuessItemListDict.Clear();
			}
			this.StateStartTicks = TimeUtil.NOW();
			this.IsBossKilled = false;
			this.ThisTimeCountDownSecs = 0L;
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x002CE5DC File Offset: 0x002CC7DC
		public void Process()
		{
			if (this.GuessStates > ShengXiaoGuessStates.NoMortgage)
			{
				this.ProcessGuessing();
			}
			else
			{
				this.ProcessNoGuess();
			}
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x002CE610 File Offset: 0x002CC810
		protected void ProcessGuessing()
		{
			if (this.GuessStates == ShengXiaoGuessStates.MortgageCountDown)
			{
				long ticks = TimeUtil.NOW();
				if (ticks >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					this.GuessStates = ShengXiaoGuessStates.BossCountDown;
					GameManager.MonsterZoneMgr.ReloadNormalMapMonsters(this.MapCode, 1);
					this.StateStartTicks = TimeUtil.NOW();
					this.ThisTimeCountDownSecs = (long)this.WaitingKillBossSecs;
					GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, (int)this.ThisTimeCountDownSecs, 0, this.GetPreGuessResult());
				}
			}
			else if (this.GuessStates == ShengXiaoGuessStates.BossCountDown)
			{
				if (this.WaitingKillBossSecs > 0)
				{
				}
				if (this.IsBossKilled)
				{
					this.GuessStates = ShengXiaoGuessStates.EndKillBoss;
				}
			}
			else if (this.GuessStates == ShengXiaoGuessStates.EndKillBoss)
			{
				int resultShengXiaoMask = this.GenerateRandomShengXiao();
				this.AddGuessResultHistory(resultShengXiaoMask);
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, resultShengXiaoMask, 0, this.GetPreGuessResult());
				this.ProcessAwards(resultShengXiaoMask);
				this.GuessStates = ShengXiaoGuessStates.NoMortgage;
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, 0, 0, this.GetPreGuessResult());
				this.Reset();
			}
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x002CE794 File Offset: 0x002CC994
		protected void ProcessNoGuess()
		{
			if (this.GuessItemListDict.Count > 0)
			{
				this.GuessStates = ShengXiaoGuessStates.MortgageCountDown;
				this.StateStartTicks = TimeUtil.NOW();
				this.ThisTimeCountDownSecs = (long)this.WaitingEnterSecs;
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, this.WaitingEnterSecs, 0, this.GetPreGuessResult());
			}
		}

		// Token: 0x0600329A RID: 12954 RVA: 0x002CE80C File Offset: 0x002CCA0C
		public int SubNeedGoods(GameClient client, int totalMortgageNum, bool allowAutoBuy = false)
		{
			int needSubGoodsNum = totalMortgageNum;
			int needBuyGoodsNum = 0;
			if (Global.GetTotalGoodsCountByID(client, this.NeedGoodsID) < totalMortgageNum)
			{
				if (!allowAutoBuy)
				{
					return -3998;
				}
				needBuyGoodsNum = Global.GMax(0, totalMortgageNum - Global.GetTotalGoodsCountByID(client, this.NeedGoodsID));
				needSubGoodsNum = totalMortgageNum - needBuyGoodsNum;
			}
			if (needSubGoodsNum > 0)
			{
				bool usedBinding = false;
				bool usedTimeLimited = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.NeedGoodsID, needSubGoodsNum, false, out usedBinding, out usedTimeLimited, false))
				{
					return -4010;
				}
			}
			if (needBuyGoodsNum > 0)
			{
				int subMoney = Global.SubUserMoneyForGoods(client, this.NeedGoodsID, needBuyGoodsNum, "生肖运程竞猜物品");
				if (subMoney <= 0)
				{
					return subMoney;
				}
			}
			return 1;
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x002CE908 File Offset: 0x002CCB08
		public int IsMortgageLegal(int guessKey, int mortgageNum)
		{
			int result;
			if (this.GuessStates > ShengXiaoGuessStates.MortgageCountDown)
			{
				result = -3700;
			}
			else if (guessKey <= 0 || guessKey > 4095 || mortgageNum <= 0)
			{
				result = -3800;
			}
			else if (!this.LegalGuessKeyList.Contains(guessKey))
			{
				result = -3990;
			}
			else if (mortgageNum > this.MaxMortgageForOnce)
			{
				result = -3996;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x002CE988 File Offset: 0x002CCB88
		public int AddGuess(GameClient client, int guessKey, int mortgageNum, bool allowAutoBuy = false)
		{
			int ret = 1;
			int oldMortgage = 0;
			Dictionary<int, int> dict = null;
			lock (this.GuessItemListDict)
			{
				if (this.GuessItemListDict.TryGetValue(client.ClientData.RoleID, out dict) && null != dict)
				{
					if (dict.TryGetValue(guessKey, out oldMortgage))
					{
						dict[guessKey] = oldMortgage + mortgageNum;
					}
					else
					{
						dict.Add(guessKey, mortgageNum);
					}
				}
				else
				{
					dict = new Dictionary<int, int>();
					dict.Add(guessKey, mortgageNum);
					this.GuessItemListDict.Add(client.ClientData.RoleID, dict);
				}
			}
			GameManager.SystemServerEvents.AddEvent(string.Format("扣除角色竞猜注码金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				client.ClientData.Gold,
				mortgageNum
			}), EventLevels.Record);
			return ret;
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x002CEAC0 File Offset: 0x002CCCC0
		public void OnBossKilled()
		{
			if (this.GuessStates == ShengXiaoGuessStates.BossCountDown)
			{
				this.IsBossKilled = true;
			}
		}

		// Token: 0x0600329E RID: 12958 RVA: 0x002CEAE8 File Offset: 0x002CCCE8
		public bool ClientEnter(GameClient gameClient)
		{
			bool result;
			if (this.LegalServerLines.IndexOf(GameManager.ServerLineID) < 0)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				if (this.GuessStates < ShengXiaoGuessStates.EndKillBoss)
				{
					long ticks = TimeUtil.NOW();
					long theTicks = ticks - this.StateStartTicks;
					if (this.ThisTimeCountDownSecs > 0L)
					{
						theTicks = this.ThisTimeCountDownSecs * 1000L - theTicks;
					}
					if (theTicks >= 1200L || this.ThisTimeCountDownSecs <= 0L)
					{
						GameManager.ClientMgr.NotifyClientShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, (int)this.GuessStates, (int)(theTicks / 1000L), 0, this.GetPreGuessResult());
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600329F RID: 12959 RVA: 0x002CEBC1 File Offset: 0x002CCDC1
		public void ClientLeave()
		{
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x002CEBC4 File Offset: 0x002CCDC4
		public List<int> GetLegalGuessServerLines()
		{
			return this.LegalServerLines;
		}

		// Token: 0x060032A1 RID: 12961 RVA: 0x002CEBDC File Offset: 0x002CCDDC
		public Dictionary<int, int> GetRoleGuessDictionay(int roleID)
		{
			Dictionary<int, int> dict = null;
			lock (this.GuessItemListDict)
			{
				if (this.GuessItemListDict.TryGetValue(roleID, out dict) && null != dict)
				{
					return dict;
				}
			}
			return new Dictionary<int, int>();
		}

		// Token: 0x060032A2 RID: 12962 RVA: 0x002CEC54 File Offset: 0x002CCE54
		public void ProcessAwards(int resultShengXiaoMask)
		{
			string roleGuessResult = "";
			string batchGuessResult = "";
			List<int> lsRoleID = this.GuessItemListDict.Keys.ToList<int>();
			try
			{
				foreach (int roleID in lsRoleID)
				{
					roleGuessResult = "";
					GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
					Dictionary<int, int> dict = null;
					if (this.GuessItemListDict.TryGetValue(roleID, out dict) && null != dict)
					{
						foreach (KeyValuePair<int, int> item in dict)
						{
							if (roleGuessResult.Length > 0)
							{
								roleGuessResult += "|";
							}
							if ((item.Key & resultShengXiaoMask) <= 0)
							{
								roleGuessResult += string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
								{
									item.Key,
									item.Value,
									resultShengXiaoMask,
									0,
									(otherClient != null) ? otherClient.ClientData.Gold : -1
								});
								Global.AddShengXiaoGuessHistoryToStaticsDB(otherClient, roleID, item.Key, item.Value, resultShengXiaoMask, 0, (otherClient != null) ? otherClient.ClientData.Gold : -1);
							}
							else
							{
								int nAwardMulitple = this.GetMultipleByGuessKey(item.Key);
								if (nAwardMulitple > 0)
								{
									int nAwardNumber = nAwardMulitple * item.Value;
									int nAwardGold = nAwardNumber * this.SingleGoodsToYuanBaoNum;
									if (nAwardGold > 0)
									{
										GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, roleID, nAwardGold, "角色竞猜", string.Concat(roleID));
										if (null != otherClient)
										{
											GameManager.SystemServerEvents.AddEvent(string.Format("角色竞猜获取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												otherClient.ClientData.Gold,
												nAwardGold
											}), EventLevels.Record);
											if (nAwardGold >= this.GateGoldForBroadcast)
											{
												Global.BroadcastShengXiaoGuessWinHint(otherClient, nAwardMulitple, Global.GetShengXiaoNameByCode(resultShengXiaoMask), nAwardGold);
											}
										}
										else
										{
											GameManager.SystemServerEvents.AddEvent(string.Format("角色竞猜获取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
											{
												roleID,
												"离线角色",
												"未知",
												nAwardGold
											}), EventLevels.Record);
										}
										roleGuessResult += string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
										{
											item.Key,
											item.Value,
											resultShengXiaoMask,
											nAwardGold,
											(otherClient != null) ? otherClient.ClientData.Gold : -1
										});
										Global.AddShengXiaoGuessHistoryToStaticsDB(otherClient, roleID, item.Key, item.Value, resultShengXiaoMask, nAwardGold, (otherClient != null) ? otherClient.ClientData.Gold : -1);
									}
								}
							}
						}
						if (null != otherClient)
						{
							GameManager.ClientMgr.NotifyShengXiaoGuessResultMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, roleGuessResult);
						}
						if (roleGuessResult.Length > 0)
						{
							if (batchGuessResult.Length > 0)
							{
								batchGuessResult += ";";
							}
							batchGuessResult += string.Format("{0},{1}", roleID, roleGuessResult);
						}
						if (batchGuessResult.Length > 1200)
						{
							GameManager.DBCmdMgr.AddDBCmd(10094, string.Format("{0}", batchGuessResult), null, 0);
							batchGuessResult = "";
						}
					}
				}
				if (batchGuessResult.Length > 0)
				{
					GameManager.DBCmdMgr.AddDBCmd(10094, string.Format("{0}", batchGuessResult), null, 0);
				}
			}
			catch
			{
			}
			lock (this.GuessItemListDict)
			{
				this.GuessItemListDict.Clear();
			}
		}

		// Token: 0x060032A3 RID: 12963 RVA: 0x002CF1A4 File Offset: 0x002CD3A4
		public int GenerateRandomShengXiao()
		{
			int nRandomShengXiaoIndex = Global.GetRandomNumber(0, 12);
			return 1 << nRandomShengXiaoIndex;
		}

		// Token: 0x060032A4 RID: 12964 RVA: 0x002CF1C8 File Offset: 0x002CD3C8
		protected int GetMultipleByGuessKey(int guessKey)
		{
			int value = 0;
			int result;
			if (this.AwardMultipleDict.TryGetValue(guessKey, out value))
			{
				result = value;
			}
			else if (guessKey <= 0)
			{
				result = -3000;
			}
			else
			{
				int nOneCount = 0;
				for (int i = 0; i < 12; i++)
				{
					nOneCount += (guessKey >> i & 1);
				}
				if (nOneCount <= 0 || nOneCount > 12)
				{
					result = -3001;
				}
				else
				{
					int nMultiple = 12 / nOneCount;
					if (this.AwardMultipleDict.Count > 50)
					{
						this.AwardMultipleDict.Clear();
					}
					this.AwardMultipleDict.Add(guessKey, nMultiple);
					result = nMultiple;
				}
			}
			return result;
		}

		// Token: 0x060032A5 RID: 12965 RVA: 0x002CF294 File Offset: 0x002CD494
		protected void AddGuessResultHistory(int result)
		{
			lock (this.ShengXiaoGuessResultHistory)
			{
				if (this.ShengXiaoGuessResultHistory.Count > 10)
				{
					this.ShengXiaoGuessResultHistory.RemoveAt(0);
				}
				this.ShengXiaoGuessResultHistory.Add(result);
			}
		}

		// Token: 0x060032A6 RID: 12966 RVA: 0x002CF30C File Offset: 0x002CD50C
		public string GetGuessResultHistory()
		{
			string results = "";
			lock (this.ShengXiaoGuessResultHistory)
			{
				foreach (int item in this.ShengXiaoGuessResultHistory)
				{
					if (results.Length > 0)
					{
						results += "|";
					}
					results += string.Format("{0}", item);
				}
			}
			return results;
		}

		// Token: 0x060032A7 RID: 12967 RVA: 0x002CF3DC File Offset: 0x002CD5DC
		private int GetPreGuessResult()
		{
			lock (this.ShengXiaoGuessResultHistory)
			{
				if (this.ShengXiaoGuessResultHistory.Count > 0)
				{
					return this.ShengXiaoGuessResultHistory[this.ShengXiaoGuessResultHistory.Count - 1];
				}
			}
			return 0;
		}

		// Token: 0x04003EAA RID: 16042
		private int MapCode = -1;

		// Token: 0x04003EAB RID: 16043
		private int WaitingEnterSecs = 120;

		// Token: 0x04003EAC RID: 16044
		private int WaitingKillBossSecs = 0;

		// Token: 0x04003EAD RID: 16045
		private long ThisTimeCountDownSecs = 0L;

		// Token: 0x04003EAE RID: 16046
		private int MaxMortgageForOnce = 100000;

		// Token: 0x04003EAF RID: 16047
		private int NeedGoodsID = -1;

		// Token: 0x04003EB0 RID: 16048
		private int SingleGoodsToYuanBaoNum = 100;

		// Token: 0x04003EB1 RID: 16049
		private int GateGoldForBroadcast = 10000;

		// Token: 0x04003EB2 RID: 16050
		private List<int> LegalServerLines = new List<int>();

		// Token: 0x04003EB3 RID: 16051
		private ShengXiaoGuessStates GuessStates = ShengXiaoGuessStates.NoMortgage;

		// Token: 0x04003EB4 RID: 16052
		private Dictionary<int, int> AwardMultipleDict = new Dictionary<int, int>();

		// Token: 0x04003EB5 RID: 16053
		private List<int> LegalGuessKeyList = new List<int>();

		// Token: 0x04003EB6 RID: 16054
		private long StateStartTicks = 0L;

		// Token: 0x04003EB7 RID: 16055
		private bool IsBossKilled = false;

		// Token: 0x04003EB8 RID: 16056
		private List<int> ShengXiaoGuessResultHistory = new List<int>();

		// Token: 0x04003EB9 RID: 16057
		private Dictionary<int, Dictionary<int, int>> GuessItemListDict = new Dictionary<int, Dictionary<int, int>>();
	}
}
