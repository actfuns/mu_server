using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB;
using GameDBServer.DB.DBController;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class JingJiChangManager : JingJiChangConstants, IManager
	{
		
		private JingJiChangManager()
		{
		}

		
		public static JingJiChangManager getInstance()
		{
			return JingJiChangManager.instance;
		}

		
		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		
		private void initCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10140, JingJiGetDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10141, JingJiGetChallengeDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10142, JingJiCreateDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10143, JingJiRequestChallengeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10144, JingJiChallengeEndCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10145, JingJiSaveDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10146, JingJiGetChallengeInfoDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10147, JingJiRemoveCDCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10148, JingJiGetRankingAndRewardTimeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10149, JingJiUpdateNextRewardTimeCmdProcessor.getInstance());
		}

		
		private void initData()
		{
			List<PlayerJingJiData> playerJingJiDataList = JingJiChangDBController.getInstance().getPlayerJingJiDataList();
			if (null != playerJingJiDataList)
			{
				foreach (PlayerJingJiData data in playerJingJiDataList)
				{
					data.convertObject();
					this.playerJingJiDatas.Add(data.roleId, data);
					this.rankingDatas.Add(data.getPlayerJingJiRankingData());
				}
				for (int rankLoop = 0; rankLoop < this.rankingDatas.Count; rankLoop++)
				{
					if (this.rankingDatas[rankLoop].ranking != rankLoop + 1)
					{
						this.rankingDatas[rankLoop].ranking = rankLoop + 1;
						JingJiChangDBController.getInstance().updateJingJiRanking(this.rankingDatas[rankLoop].roleId, this.rankingDatas[rankLoop].ranking);
					}
				}
			}
		}

		
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, JingJiChangPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, JingJiChangPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, JingJiChangPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, JingJiChangPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeData()
		{
			if (null != this.playerJingJiDatas)
			{
				this.playerJingJiDatas.Clear();
			}
			this.playerJingJiDatas = null;
			if (null != this.rankingDatas)
			{
				this.rankingDatas.Clear();
			}
			this.rankingDatas = null;
			if (null != this.lockPlayerJingJiDatas)
			{
				this.lockPlayerJingJiDatas.Clear();
			}
			this.lockPlayerJingJiDatas = null;
			if (null != this.challengeInfos)
			{
				this.challengeInfos.Clear();
			}
			this.challengeInfos = null;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			this.removeListener();
			this.removeData();
			return true;
		}

		
		public bool createRobotData(PlayerJingJiData data)
		{
			lock (this.changeRankingLock)
			{
				if (this.playerJingJiDatas.ContainsKey(data.roleId))
				{
					return false;
				}
				data.isOnline = true;
				this.playerJingJiDatas.Add(data.roleId, data);
				this.challengeInfos.Add(data.roleId, new List<JingJiChallengeInfoData>());
				if (this.rankingDatas.Count >= JingJiChangConstants.RankingList_Max_Num)
				{
					data.ranking = -1;
				}
				else
				{
					data.ranking = this.rankingDatas.Count + 1;
					this.rankingDatas.Add(data.getPlayerJingJiRankingData());
				}
			}
			return JingJiChangDBController.getInstance().insertJingJiData(data);
		}

		
		public void onPlayerLogin(int roleId)
		{
			PlayerJingJiData data = null;
			lock (this.changeRankingLock)
			{
				if (this.playerJingJiDatas.TryGetValue(roleId, out data))
				{
					data.isOnline = true;
				}
				else
				{
					data = JingJiChangDBController.getInstance().getPlayerJingJiDataById(roleId);
					if (null != data)
					{
						data.convertObject();
						data.isOnline = true;
						this.playerJingJiDatas.Add(data.roleId, data);
					}
				}
				if (null != data)
				{
					List<JingJiChallengeInfoData> zhanBaoList = null;
					if (!this.challengeInfos.TryGetValue(roleId, out zhanBaoList))
					{
						zhanBaoList = JingJiChangZhaoBaoDBController.getInstnace().getChallengeInfoListByRoleId(roleId);
						if (null == zhanBaoList)
						{
							zhanBaoList = new List<JingJiChallengeInfoData>();
						}
						this.challengeInfos.Add(roleId, zhanBaoList);
					}
				}
			}
		}

		
		public void onPlayerLogout(int roleId)
		{
			PlayerJingJiData data = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out data);
				if (null != data)
				{
					data.isOnline = false;
					if (data.ranking == -1 && !this.lockPlayerJingJiDatas.ContainsKey(data.roleId))
					{
						this.playerJingJiDatas.Remove(data.roleId);
					}
				}
				this.challengeInfos.Remove(roleId);
			}
		}

		
		public void getRankingAndNextRewardTimeById(int roleId, out int ranking, out long nextRewardTime)
		{
			ranking = -2;
			nextRewardTime = 0L;
			PlayerJingJiData data = this.getPlayerJingJiDataById(roleId);
			if (null != data)
			{
				ranking = data.ranking;
				nextRewardTime = data.nextRewardTime;
			}
		}

		
		public bool updateNextRewardTime(int roleId, long nextRewardTime)
		{
			PlayerJingJiData data = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out data);
			}
			bool result;
			if (null != data)
			{
				data.nextRewardTime = nextRewardTime;
				result = JingJiChangDBController.getInstance().updateNextRewardTime(roleId, nextRewardTime);
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public PlayerJingJiData getPlayerJingJiDataById(int roleId)
		{
			PlayerJingJiData robotData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out robotData);
			}
			if (robotData != null)
			{
				DBRoleInfo dbRoleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleId);
				if (dbRoleInfo != null)
				{
					robotData.AdmiredCount = dbRoleInfo.AdmiredCount;
				}
			}
			return robotData;
		}

		
		public bool removeCD(int roleId)
		{
			PlayerJingJiData data = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out data);
			}
			if (null != data)
			{
				data.nextChallengeTime = 0L;
			}
			return JingJiChangDBController.getInstance().updateNextChallengeTime(roleId, 0L);
		}

		
		public JingJiBeChallengeData requestChallenge(int challengerId, int beChallengerId, int beChallengerRanking)
		{
			JingJiBeChallengeData data = new JingJiBeChallengeData();
			PlayerJingJiData challengerData = null;
			JingJiBeChallengeData result;
			lock (this.changeRankingLock)
			{
				if (!this.playerJingJiDatas.TryGetValue(challengerId, out challengerData))
				{
					data.state = 0;
					result = data;
				}
				else if (TimeUtil.NOW() < challengerData.nextChallengeTime)
				{
					data.state = -1;
					result = data;
				}
				else if ((challengerData.ranking > 100 || challengerData.ranking < 0) && beChallengerRanking <= 3)
				{
					data.state = -5;
					result = data;
				}
				else if (beChallengerRanking > this.rankingDatas.Count || beChallengerRanking < 1)
				{
					data.state = -2;
					result = data;
				}
				else
				{
					PlayerJingJiRankingData rankingData = this.rankingDatas[beChallengerRanking - 1];
					PlayerJingJiData beChallengerData = null;
					if (!this.playerJingJiDatas.TryGetValue(rankingData.roleId, out beChallengerData))
					{
						data.state = 0;
						result = data;
					}
					else if (challengerId == rankingData.roleId)
					{
						data.state = -3;
						result = data;
					}
					else
					{
						BeChallengerCount beChallengerCount = null;
						this.lockPlayerJingJiDatas.TryGetValue(beChallengerData.roleId, out beChallengerCount);
						if (null == beChallengerCount)
						{
							beChallengerCount = new BeChallengerCount();
							beChallengerCount.nBeChallengerCount = 1;
							this.lockPlayerJingJiDatas.Add(beChallengerData.roleId, beChallengerCount);
						}
						else
						{
							beChallengerCount.nBeChallengerCount++;
						}
						data.state = 1;
						data.beChallengerData = beChallengerData;
						result = data;
					}
				}
			}
			return result;
		}

		
		public int onChallengeEnd(JingJiChallengeResultData result)
		{
			PlayerJingJiData challenger = null;
			PlayerJingJiData beChallenger = null;
			int ranking;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(result.playerId, out challenger);
				this.playerJingJiDatas.TryGetValue(result.robotId, out beChallenger);
				BeChallengerCount beChallengerCount = null;
				this.lockPlayerJingJiDatas.TryGetValue(result.robotId, out beChallengerCount);
				if (null != beChallengerCount)
				{
					beChallengerCount.nBeChallengerCount--;
				}
				if (result.isWin)
				{
					int playerRanking = challenger.ranking;
					int robotRanking = beChallenger.ranking;
					if (robotRanking < 1 || playerRanking == robotRanking)
					{
						return challenger.ranking;
					}
					if (playerRanking == -1)
					{
						challenger.ranking = robotRanking;
						beChallenger.ranking = playerRanking;
						this.rankingDatas.Remove(beChallenger.getPlayerJingJiRankingData());
						this.rankingDatas.Add(challenger.getPlayerJingJiRankingData());
						this.rankingDatas.Sort();
						JingJiChangDBController.getInstance().updateJingJiRanking(challenger.roleId, challenger.ranking);
						JingJiChangDBController.getInstance().updateJingJiRanking(beChallenger.roleId, beChallenger.ranking);
					}
					else if (playerRanking > robotRanking)
					{
						challenger.ranking = robotRanking;
						beChallenger.ranking = playerRanking;
						beChallenger.getPlayerJingJiRankingData();
						challenger.getPlayerJingJiRankingData();
						this.rankingDatas.Sort();
						JingJiChangDBController.getInstance().updateJingJiRanking(challenger.roleId, challenger.ranking);
						JingJiChangDBController.getInstance().updateJingJiRanking(beChallenger.roleId, beChallenger.ranking);
					}
				}
				ranking = challenger.ranking;
			}
			return ranking;
		}

		
		private void createChallengeWinChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = beChallengePlayer.roleName;
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Challenge_Win;
			playerZhanBaoData.value = challengePlayer.ranking;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			robotZhanBaoData = new JingJiChallengeInfoData();
			robotZhanBaoData.roleId = beChallengePlayer.roleId;
			robotZhanBaoData.challengeName = challengePlayer.roleName;
			robotZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Be_Challenge_Failed;
			robotZhanBaoData.value = beChallengePlayer.ranking;
			robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		
		private void createChallengeFailedChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = beChallengePlayer.roleName;
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Challenge_Failed;
			playerZhanBaoData.value = challengePlayer.ranking;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			robotZhanBaoData = new JingJiChallengeInfoData();
			robotZhanBaoData.roleId = beChallengePlayer.roleId;
			robotZhanBaoData.challengeName = challengePlayer.roleName;
			robotZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Be_Challenge_Win;
			robotZhanBaoData.value = beChallengePlayer.ranking;
			robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		
		private void createLianShengChallengeInfo(PlayerJingJiData challengePlayer, out JingJiChallengeInfoData playerZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = "";
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_LianSheng;
			playerZhanBaoData.value = challengePlayer.winCount;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		
		public void saveData(JingJiSaveData data, out int winCount)
		{
			winCount = 0;
			PlayerJingJiData playerData = null;
			PlayerJingJiData robotData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(data.roleId, out playerData);
				this.playerJingJiDatas.TryGetValue(data.robotId, out robotData);
				if (data.isWin)
				{
					playerData.level = data.level;
					playerData.changeLiveCount = data.changeLiveCount;
					playerData.nextChallengeTime = data.nextChallengeTime;
					playerData.baseProps = data.baseProps;
					playerData.extProps = data.extProps;
					playerData.equipDatas = data.equipDatas;
					playerData.skillDatas = data.skillDatas;
					playerData.combatForce = data.combatForce;
					playerData.wingData = data.wingData;
					playerData.settingFlags = data.settingFlags;
					playerData.occupationId = data.Occupation;
					playerData.SubOccupation = data.SubOccupation;
					playerData.winCount++;
					playerData.shenShiEquipData = data.ShenShiEuipSkill;
					if (playerData.winCount > playerData.MaxWinCnt)
					{
						playerData.MaxWinCnt = playerData.winCount;
						JingJiChangDBController.getInstance().updateJingJiMaxWinCount(playerData.roleId, playerData.MaxWinCnt);
					}
					playerData.PassiveEffectList = data.PassiveEffectList;
					playerData.convertString();
					JingJiChangDBController.getInstance().updateJingJiDataForWin(playerData);
					JingJiChallengeInfoData playerZhanBaoData;
					JingJiChallengeInfoData robotZhanBaoData;
					this.createChallengeWinChallengeInfoData(playerData, robotData, out playerZhanBaoData, out robotZhanBaoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(playerZhanBaoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(robotZhanBaoData);
					JingJiChallengeInfoData lianShengZhanBaoData = null;
					if (playerData.winCount >= 10 && playerData.winCount % 10 == 0)
					{
						winCount = playerData.winCount;
						this.createLianShengChallengeInfo(playerData, out lianShengZhanBaoData);
						JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(lianShengZhanBaoData);
					}
					List<JingJiChallengeInfoData> playerZhanbaoList = null;
					this.challengeInfos.TryGetValue(playerData.roleId, out playerZhanbaoList);
					if (null != lianShengZhanBaoData)
					{
						playerZhanbaoList.Insert(0, lianShengZhanBaoData);
						if (playerZhanbaoList.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
						}
					}
					playerZhanbaoList.Insert(0, playerZhanBaoData);
					if (playerZhanbaoList.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
					{
						playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
					}
					if (robotData.isOnline)
					{
						List<JingJiChallengeInfoData> robotZhanbaoList = null;
						this.challengeInfos.TryGetValue(robotData.roleId, out robotZhanbaoList);
						robotZhanbaoList.Insert(0, robotZhanBaoData);
						if (robotZhanbaoList.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							robotZhanbaoList.RemoveAt(robotZhanbaoList.Count - 1);
						}
					}
					if (robotData.winCount > 0)
					{
						robotData.winCount = 0;
						JingJiChangDBController.getInstance().updateJingJiWinCount(robotData.roleId, robotData.winCount);
					}
				}
				else
				{
					if (playerData.winCount >= 10)
					{
						winCount = playerData.winCount;
					}
					playerData.winCount = 0;
					playerData.nextChallengeTime = data.nextChallengeTime;
					JingJiChangDBController.getInstance().updateJingJiDataForFailed(playerData.roleId, playerData.nextChallengeTime);
					robotData.winCount++;
					if (robotData.winCount > robotData.MaxWinCnt)
					{
						robotData.MaxWinCnt = robotData.winCount;
						JingJiChangDBController.getInstance().updateJingJiMaxWinCount(robotData.roleId, robotData.MaxWinCnt);
					}
					JingJiChangDBController.getInstance().updateJingJiWinCount(robotData.roleId, robotData.winCount);
					JingJiChallengeInfoData playerZhanBaoData;
					JingJiChallengeInfoData robotZhanBaoData;
					this.createChallengeFailedChallengeInfoData(playerData, robotData, out playerZhanBaoData, out robotZhanBaoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(playerZhanBaoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(robotZhanBaoData);
					List<JingJiChallengeInfoData> playerZhanbaoList = null;
					this.challengeInfos.TryGetValue(playerData.roleId, out playerZhanbaoList);
					playerZhanbaoList.Insert(0, playerZhanBaoData);
					if (playerZhanbaoList.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
					{
						playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
					}
					if (robotData.isOnline)
					{
						List<JingJiChallengeInfoData> robotZhanbaoList = null;
						this.challengeInfos.TryGetValue(robotData.roleId, out robotZhanbaoList);
						robotZhanbaoList.Insert(0, robotZhanBaoData);
						if (robotZhanbaoList.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							robotZhanbaoList.RemoveAt(robotZhanbaoList.Count - 1);
						}
					}
				}
				BeChallengerCount beChallengerCount = null;
				int nBeChallengerCount = 0;
				this.lockPlayerJingJiDatas.TryGetValue(robotData.roleId, out beChallengerCount);
				if (null != beChallengerCount)
				{
					nBeChallengerCount = beChallengerCount.nBeChallengerCount;
					if (nBeChallengerCount <= 0)
					{
						this.lockPlayerJingJiDatas.Remove(robotData.roleId);
					}
				}
				if (robotData.ranking == -1 && nBeChallengerCount <= 0 && !robotData.isOnline)
				{
					this.playerJingJiDatas.Remove(robotData.roleId);
				}
			}
		}

		
		public List<PlayerJingJiMiniData> getChallengeData(int[] challengeRankings)
		{
			List<PlayerJingJiMiniData> miniDataList = new List<PlayerJingJiMiniData>();
			lock (this.changeRankingLock)
			{
				if (challengeRankings.Length > 1 && challengeRankings[0] < 0)
				{
					int delta = Math.Min(this.rankingDatas.Count / 6, -challengeRankings[0]);
					if (delta <= 2)
					{
						return miniDataList;
					}
					challengeRankings[0] = this.rankingDatas.Count - 1 - delta;
					challengeRankings[1] = this.rankingDatas.Count - 1 - delta * 2;
					challengeRankings[2] = this.rankingDatas.Count - 1 - delta * 3;
				}
				int nCheckCount = 0;
				while (nCheckCount++ < 6)
				{
					bool bErrorRank = false;
					foreach (int challengeRanking in challengeRankings)
					{
						PlayerJingJiData robotData = null;
						if (challengeRanking <= this.rankingDatas.Count)
						{
							PlayerJingJiRankingData rankingData = this.rankingDatas[challengeRanking - 1];
							if (rankingData.ranking < 0)
							{
								bErrorRank = true;
								this.rankingDatas.Remove(rankingData);
								break;
							}
						}
					}
					if (!bErrorRank)
					{
						break;
					}
					this.rankingDatas.Sort();
				}
				foreach (int challengeRanking in challengeRankings)
				{
					PlayerJingJiData robotData = null;
					if (challengeRanking <= this.rankingDatas.Count)
					{
						PlayerJingJiRankingData rankingData = this.rankingDatas[challengeRanking - 1];
						if (this.playerJingJiDatas.TryGetValue(rankingData.roleId, out robotData))
						{
							miniDataList.Add(robotData.getPlayerJingJiMiniData());
						}
					}
				}
			}
			return miniDataList;
		}

		
		public List<JingJiChallengeInfoData> getChallengeInfoDataList(int roleId, int pageIndex)
		{
			List<JingJiChallengeInfoData> result;
			if (pageIndex >= JingJiChangConstants.ChallengeInfo_Max_Num)
			{
				result = null;
			}
			else
			{
				List<JingJiChallengeInfoData> zhanbaoDataList = null;
				if (!this.challengeInfos.TryGetValue(roleId, out zhanbaoDataList))
				{
					result = null;
				}
				else
				{
					int minIndex = pageIndex * JingJiChangConstants.ChallengeInfo_PageShowNum;
					int getNum = JingJiChangConstants.ChallengeInfo_PageShowNum;
					if (minIndex >= zhanbaoDataList.Count)
					{
						result = null;
					}
					else
					{
						if (minIndex + getNum >= zhanbaoDataList.Count)
						{
							getNum = zhanbaoDataList.Count - minIndex;
						}
						if (getNum == 0)
						{
							result = null;
						}
						else
						{
							result = zhanbaoDataList.GetRange(minIndex, getNum);
						}
					}
				}
			}
			return result;
		}

		
		public List<PaiHangItemData> getRankingList(int pageIndex)
		{
			int maxIndex = JingJiChangConstants.RankingList_PageShowNum;
			if (maxIndex > this.rankingDatas.Count)
			{
				maxIndex = this.rankingDatas.Count;
			}
			List<PaiHangItemData> _rankingDatas = new List<PaiHangItemData>();
			lock (this.changeRankingLock)
			{
				for (int i = 0; i < maxIndex; i++)
				{
					_rankingDatas.Add(this.rankingDatas[i].getPaiHangItemData());
				}
			}
			return _rankingDatas;
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				try
				{
					lock (this.changeRankingLock)
					{
						PlayerJingJiData data = null;
						if (!this.playerJingJiDatas.TryGetValue(roleId, out data) || data == null)
						{
							return;
						}
						data.roleName = newName;
						data.name = newName;
					}
				}
				catch (Exception)
				{
				}
				JingJiChangDBController.getInstance().OnChangeName(roleId, oldName, newName);
			}
		}

		
		private static JingJiChangManager instance = new JingJiChangManager();

		
		private List<PlayerJingJiRankingData> rankingDatas = new List<PlayerJingJiRankingData>();

		
		private Dictionary<int, PlayerJingJiData> playerJingJiDatas = new Dictionary<int, PlayerJingJiData>();

		
		private Dictionary<int, BeChallengerCount> lockPlayerJingJiDatas = new Dictionary<int, BeChallengerCount>();

		
		private object changeRankingLock = new object();

		
		private Dictionary<int, List<JingJiChallengeInfoData>> challengeInfos = new Dictionary<int, List<JingJiChallengeInfoData>>();
	}
}
