using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Ornament;
using GameServer.Logic.Talent;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000730 RID: 1840
	public class JingJiChangManager : JingJiChangConstants, IManager
	{
		// Token: 0x06002CAD RID: 11437 RVA: 0x0027DB7C File Offset: 0x0027BD7C
		private JingJiChangManager()
		{
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x0027DBE4 File Offset: 0x0027BDE4
		public static JingJiChangManager getInstance()
		{
			return JingJiChangManager.instance;
		}

		// Token: 0x06002CAF RID: 11439 RVA: 0x0027DBFC File Offset: 0x0027BDFC
		public bool initialize()
		{
			this.loadStaticData();
			this.initCmdProcessor();
			this.initListener();
			return true;
		}

		// Token: 0x06002CB0 RID: 11440 RVA: 0x0027DC24 File Offset: 0x0027BE24
		private void loadStaticData()
		{
			this.jingjiMainConfig.LoadFromXMlFile("Config/JingJi.xml", "", "ID", 0);
			this.junxianConfig.LoadFromXMlFile("Config/JunXian.xml", "", "Level", 0);
			this.jingjiFuBenId = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiFuBenID", -1);
			this.jingjiBuffId = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiBuff", -1);
			GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this.jingjiFuBenId, out this.jingjiFuBenItem);
			this.nJingJiChangMapCode = this.jingjiFuBenItem.GetIntValue("MapCode", -1);
			this.jingjiFuBenMinZhuanSheng = this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1);
		}

		// Token: 0x06002CB1 RID: 11441 RVA: 0x0027DCE4 File Offset: 0x0027BEE4
		private void initCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(578, 2, JingJiDetailCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(1340, 2, JingJiGetRoleLooksCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(579, 4, JingJiRequestChallengeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(582, 2, JingJiChallengeInfoCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(583, 1, JingJiRankingRewardCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(584, 1, JingJiRemoveCDCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(585, 2, JingJiGetBuffCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(586, 1, JingJiJunxianLevelupCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(587, 1, JingJiLeaveFuBenCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(634, 1, JingJiStartFightCmdProcessor.getInstance());
		}

		// Token: 0x06002CB2 RID: 11442 RVA: 0x0027DDD0 File Offset: 0x0027BFD0
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(9, JingJiPlayerLevelupEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(10, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(11, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(12, JingJiPlayerLogoutEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(13, JingJiPlayerLeaveFuBenEventListener.getInstance());
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x0027DE38 File Offset: 0x0027C038
		public bool startup()
		{
			return true;
		}

		// Token: 0x06002CB4 RID: 11444 RVA: 0x0027DE4C File Offset: 0x0027C04C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x0027DE60 File Offset: 0x0027C060
		public bool destroy()
		{
			this.removeListener();
			if (null != this.jingjichangInstances)
			{
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.Clear();
				}
			}
			return true;
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x0027DECC File Offset: 0x0027C0CC
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(9, JingJiPlayerLevelupEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(10, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(11, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(12, JingJiPlayerLogoutEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(13, JingJiPlayerLeaveFuBenEventListener.getInstance());
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x0027DF54 File Offset: 0x0027C154
		public JingJiDetailData getDetailData(GameClient player, int requestType = 0)
		{
			JingJiDetailData detailData = new JingJiDetailData();
			JingJiDetailData result;
			if (player.ClientData.Level < this.jingjiFuBenItem.GetIntValue("MinLevel", -1) && player.ClientData.ChangeLifeCount == this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1))
			{
				detailData.state = ResultCode.Illegal;
				result = detailData;
			}
			else if (requestType != 0 && requestType != 1)
			{
				detailData.state = ResultCode.Illegal;
				result = detailData;
			}
			else if (player.ClientData.CurrentLifeV <= 0 || player.ClientData.CurrentAction == 12)
			{
				detailData.state = ResultCode.Dead_Error;
				result = detailData;
			}
			else if (player.ClientData.HideGM > 0)
			{
				detailData.state = ResultCode.Illegal;
				result = detailData;
			}
			else
			{
				PlayerJingJiData jingjiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == jingjiData.baseProps)
				{
					PlayerJingJiData data = this.createJingJiData(player);
					Global.sendToDB<byte, byte[]>(10142, DataHelper.ObjectToBytes<PlayerJingJiData>(data), player.ServerId);
					jingjiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				}
				detailData.ranking = jingjiData.ranking;
				detailData.winCount = jingjiData.winCount;
				detailData.nextRewardTime = jingjiData.nextRewardTime;
				detailData.nextChallengeTime = jingjiData.nextChallengeTime;
				detailData.maxwincount = jingjiData.MaxWinCnt;
				int[] beChallengerRankings = new int[JingJiChangConstants.CanChallengeNum];
				if (requestType == 0)
				{
					if (detailData.ranking >= 1 && detailData.ranking <= 3)
					{
						int index = 0;
						for (int i = 1; i <= 4; i++)
						{
							if (i != detailData.ranking)
							{
								beChallengerRankings[index++] = i;
							}
						}
					}
					else
					{
						int Coefficient = -1;
						if (detailData.ranking == -1)
						{
							Coefficient = this.jingjiMainConfig.SystemXmlItemDict.Values.Max((SystemXmlItem x) => x.GetIntValue("MaxRank", -1));
							beChallengerRankings[0] = -Coefficient;
							beChallengerRankings[1] = -Coefficient * 2;
							beChallengerRankings[2] = --Coefficient * 3;
						}
						else
						{
							foreach (SystemXmlItem xmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
							{
								if (detailData.ranking >= xmlItem.GetIntValue("MinRank", -1) && detailData.ranking <= xmlItem.GetIntValue("MaxRank", -1))
								{
									Coefficient = xmlItem.GetIntValue("Coefficient", -1);
									break;
								}
							}
							for (int i = 0; i < JingJiChangConstants.CanChallengeNum; i++)
							{
								beChallengerRankings[i] = detailData.ranking - (i + 1) * Coefficient;
							}
						}
					}
				}
				else if (detailData.ranking >= 1 && detailData.ranking <= 3)
				{
					int index = 0;
					for (int i = 1; i <= 4; i++)
					{
						if (i != detailData.ranking)
						{
							beChallengerRankings[index++] = i;
						}
					}
				}
				else
				{
					int index = 0;
					for (int i = 1; i <= 3; i++)
					{
						if (i != detailData.ranking)
						{
							beChallengerRankings[index++] = i;
						}
					}
				}
				List<PlayerJingJiMiniData> beChallengerMiniDatas = Global.sendToDB<List<PlayerJingJiMiniData>, byte[]>(10141, DataHelper.ObjectToBytes<int[]>(beChallengerRankings), player.ServerId);
				detailData.beChallengerData = beChallengerMiniDatas;
				detailData.freeChallengeNum = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
				FuBenData jingjifuBenData = Global.GetFuBenData(player, this.jingjiFuBenId);
				int nFinishNum;
				int useTotalNum = Global.GetFuBenEnterNum(jingjifuBenData, out nFinishNum);
				int useFreeNum = (useTotalNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? useTotalNum : this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
				detailData.useFreeChallengeNum = useFreeNum;
				int[] vipJingjiCounts = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
				int playerVipLev = player.ClientData.VipLevel;
				detailData.vipChallengeNum = vipJingjiCounts[playerVipLev];
				int useVipNum = (useTotalNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? 0 : (useTotalNum - this.jingjiFuBenItem.GetIntValue("EnterNumber", -1));
				detailData.useVipChallengeNum = useVipNum;
				detailData.state = ResultCode.Success;
				result = detailData;
			}
			return result;
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x0027E458 File Offset: 0x0027C658
		public int getJingJiMapCode()
		{
			return (this.jingjiFuBenItem != null) ? this.jingjiFuBenItem.GetIntValue("MapCode", -1) : -1;
		}

		// Token: 0x06002CB9 RID: 11449 RVA: 0x0027E488 File Offset: 0x0027C688
		public bool CanGradeJunXian(GameClient player)
		{
			int junxian = this.getJunxian(player);
			bool result;
			if (junxian + 1 > this.junxianConfig.SystemXmlItemDict.Count)
			{
				result = false;
			}
			else
			{
				int needShengWang = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetIntValue("NeedShengWang", -1);
				result = (this.getShengWangValue(player) >= needShengWang && GlobalNew.IsGongNengOpened(player, GongNengIDs.JingJiChang, false));
			}
			return result;
		}

		// Token: 0x06002CBA RID: 11450 RVA: 0x0027E50C File Offset: 0x0027C70C
		public int upGradeJunXian(GameClient player)
		{
			int result = this.check(player);
			int result2;
			if (result != ResultCode.Success)
			{
				result2 = result;
			}
			else
			{
				int junxian = this.getJunxian(player);
				if (junxian + 1 > this.junxianConfig.SystemXmlItemDict.Count)
				{
					result2 = ResultCode.Illegal;
				}
				else
				{
					string needGoods = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetStringValue("NeedGoods");
					List<List<int>> GoodsCost = ConfigHelper.ParserIntArrayList(needGoods, true, '|', ',');
					for (int i = 0; i < GoodsCost.Count; i++)
					{
						int goodsId = GoodsCost[i][0];
						int costCount = GoodsCost[i][1];
						int haveGoodsCnt = Global.GetTotalGoodsCountByID(player, goodsId);
						if (haveGoodsCnt < costCount)
						{
							return ResultCode.GoodsNotEnough;
						}
					}
					int needShengWang = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetIntValue("NeedShengWang", -1);
					if (!this.consumeShengWang(player, needShengWang))
					{
						result2 = ResultCode.ShengWang_Not_Enough_Error;
					}
					else if (!GlobalNew.IsGongNengOpened(player, GongNengIDs.JingJiChang, false))
					{
						result2 = ResultCode.Junxian_Null_Error;
					}
					else
					{
						bool bUsedBinding_just_placeholder = false;
						bool bUsedTimeLimited_just_placeholder = false;
						for (int i = 0; i < GoodsCost.Count; i++)
						{
							int goodsId = GoodsCost[i][0];
							int costCount = GoodsCost[i][1];
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, player, goodsId, costCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("进阶军衔改变军衔时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", goodsId, costCount), null, true);
							}
							GoodsData goodsData = new GoodsData();
							goodsData.GoodsID = goodsId;
							goodsData.GCount = costCount;
						}
						this.modifyJunxian(player);
						if (ResultCode.Success == this.activeJunXianBuff(player, true))
						{
							Global.BroadcastClientMUShengWang(player, this.getJunxian(player));
						}
						if (!GlobalNew.IsGongNengOpened(player, GongNengIDs.JingJiChang, false))
						{
							result2 = ResultCode.Junxian_Null_Error;
						}
						else
						{
							player._IconStateMgr.CheckJingJiChangJunXian(player);
							player._IconStateMgr.SendIconStateToClient(player);
							result2 = ResultCode.Success;
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x06002CBB RID: 11451 RVA: 0x0027E784 File Offset: 0x0027C984
		public int activeJunXianBuff(GameClient player, bool replace)
		{
			int result = this.check(player);
			int result2;
			if (result != ResultCode.Success)
			{
				result2 = result;
			}
			else
			{
				int junxian = this.getJunxian(player);
				if (junxian <= 0)
				{
					result2 = ResultCode.Junxian_Null_Error;
				}
				else if (!GlobalNew.IsGongNengOpened(player, GongNengIDs.JingJiChang, false))
				{
					result2 = ResultCode.Junxian_Null_Error;
				}
				else if (replace)
				{
					if (!this.consumeShengWang(player, this.junxianConfig.SystemXmlItemDict[junxian].GetIntValue("XiaoHaoShengWang", -1)))
					{
						result2 = ResultCode.ShengWang_Not_Enough_Error;
					}
					else
					{
						this.installJunXianBuff(player);
						result2 = ResultCode.Success;
					}
				}
				else if (this.isHasJunXianBuff(player))
				{
					result2 = ResultCode.HasJunxianBuff_Error;
				}
				else if (!this.consumeShengWang(player, this.junxianConfig.SystemXmlItemDict[junxian].GetIntValue("XiaoHaoShengWang", -1)))
				{
					result2 = ResultCode.ShengWang_Not_Enough_Error;
				}
				else
				{
					this.installJunXianBuff(player);
					result2 = ResultCode.Success;
				}
			}
			return result2;
		}

		// Token: 0x06002CBC RID: 11452 RVA: 0x0027E888 File Offset: 0x0027CA88
		private bool consumeShengWang(GameClient player, int consumeValue)
		{
			int shengwangValue = this.getShengWangValue(player);
			bool result;
			if (this.getShengWangValue(player) < consumeValue)
			{
				result = false;
			}
			else
			{
				this.changeShengWangValue(player, -consumeValue);
				result = true;
			}
			return result;
		}

		// Token: 0x06002CBD RID: 11453 RVA: 0x0027E8C4 File Offset: 0x0027CAC4
		private int GetRobotMinAttack(int nOccu, EMagicSwordTowardType eType, PlayerJingJiData data)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? ((int)data.extProps[9]) : ((int)data.extProps[7]));
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = (int)data.extProps[7];
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = (int)data.extProps[9];
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x06002CBE RID: 11454 RVA: 0x0027E928 File Offset: 0x0027CB28
		private int GetRobotMaxAttack(int nOccu, EMagicSwordTowardType eType, PlayerJingJiData data)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? ((int)data.extProps[10]) : ((int)data.extProps[10]));
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = (int)data.extProps[10];
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = (int)data.extProps[10];
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x06002CBF RID: 11455 RVA: 0x0027E98C File Offset: 0x0027CB8C
		private int GetRobotAttackType(int nOccu, EMagicSwordTowardType eType)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? 1 : 0);
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = 0;
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = 1;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x06002CC0 RID: 11456 RVA: 0x0027E9CC File Offset: 0x0027CBCC
		private bool isHasJunXianBuff(GameClient player)
		{
			BufferData bufferData = Global.GetBufferDataByID(player, 87);
			return bufferData != null && !Global.IsBufferDataOver(bufferData, 0L);
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x0027EA00 File Offset: 0x0027CC00
		private int getJunxianBuffId(GameClient player)
		{
			int[] buffIds = GameManager.systemParamsList.GetParamValueIntArrayByName("JunXianBufferGoodsIDs", ',');
			int junxian = this.getJunxian(player);
			return buffIds[junxian];
		}

		// Token: 0x06002CC2 RID: 11458 RVA: 0x0027EA30 File Offset: 0x0027CC30
		private void installJunXianBuff(GameClient player)
		{
			int nNewBufferGoodsIndexID = this.getJunxian(player) - 1;
			int nOldBufferGoodsIndexID = -1;
			BufferData bufferData = Global.GetBufferDataByID(player, 87);
			if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
			{
				nOldBufferGoodsIndexID = (int)bufferData.BufferVal;
			}
			if (nOldBufferGoodsIndexID != nNewBufferGoodsIndexID)
			{
				double[] actionParams = new double[]
				{
					(double)nNewBufferGoodsIndexID
				};
				if (actionParams[0] < 1.0 && player.CodeRevision < 1)
				{
					actionParams[0] = 1.0;
				}
				Global.UpdateBufferData(player, BufferItemTypes.MU_JINGJICHANG_JUNXIAN, actionParams, 0, true);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, player);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, player, true, false, 7);
			}
		}

		// Token: 0x06002CC3 RID: 11459 RVA: 0x0027EB14 File Offset: 0x0027CD14
		public void GetNextRewardTime(GameClient player)
		{
			lock (player)
			{
				long[] resultParams = Global.sendToDB<long[], byte[]>(10148, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == resultParams)
				{
					player.ClientData.JingJiNextRewardTime = -1L;
				}
				else
				{
					long _nextRewardTime = resultParams[1];
					if (_nextRewardTime < 1L)
					{
						player.ClientData.JingJiNextRewardTime = -1L;
					}
					else
					{
						player.ClientData.JingJiNextRewardTime = _nextRewardTime;
					}
				}
			}
		}

		// Token: 0x06002CC4 RID: 11460 RVA: 0x0027EBC8 File Offset: 0x0027CDC8
		public bool CanGetrankingReward(GameClient player)
		{
			if (-1L == player.ClientData.JingJiNextRewardTime)
			{
				this.GetNextRewardTime(player);
			}
			return TimeUtil.NOW() >= player.ClientData.JingJiNextRewardTime;
		}

		// Token: 0x06002CC5 RID: 11461 RVA: 0x0027EC1C File Offset: 0x0027CE1C
		public void rankingReward(GameClient player, out int result, out long nextRewardTime)
		{
			result = this.check(player);
			nextRewardTime = 0L;
			if (result == ResultCode.Success)
			{
				int addShengWangValue;
				int addExpValue;
				string goodsInfos;
				lock (player)
				{
					long[] resultParams = Global.sendToDB<long[], byte[]>(10148, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
					int ranking = (int)resultParams[0];
					long _nextRewardTime = resultParams[1];
					if (ranking == -2)
					{
						result = ResultCode.Illegal;
						return;
					}
					if (TimeUtil.NOW() < _nextRewardTime)
					{
						result = ResultCode.RankingReward_CD_Error;
						return;
					}
					this.getRankingRewardValue(player, ranking, out addShengWangValue, out addExpValue, out goodsInfos);
					_nextRewardTime = TimeUtil.NOW() + JingJiChangConstants.RankingReward_CD_Time;
					nextRewardTime = _nextRewardTime;
					Global.sendToDB<int, byte[]>(10149, DataHelper.ObjectToBytes<long[]>(new long[]
					{
						(long)player.ClientData.RoleID,
						_nextRewardTime
					}), player.ServerId);
					this.GetNextRewardTime(player);
				}
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)addExpValue, true, true, false, "none");
				this.addGoods(player, goodsInfos);
				this.changeShengWangValue(player, addShengWangValue);
				player.ClientData.AddAwardRecord(RoleAwardMsg.JingJiChang, goodsInfos, false);
				GameManager.ClientMgr.NotifyGetAwardMsg(player, RoleAwardMsg.JingJiChang, "");
				player._IconStateMgr.CheckJingJiChangJiangLi(player);
				player._IconStateMgr.SendIconStateToClient(player);
			}
		}

		// Token: 0x06002CC6 RID: 11462 RVA: 0x0027EDB8 File Offset: 0x0027CFB8
		private void addGoods(GameClient player, string goodsInfos)
		{
			string[] _goodsInfos = goodsInfos.Split(new char[]
			{
				'|'
			});
			foreach (string goodsInfo in _goodsInfos)
			{
				string[] _goodsInfo = goodsInfo.Split(new char[]
				{
					','
				});
				int goodsId = Convert.ToInt32(_goodsInfo[0]);
				int goodsNum = Convert.ToInt32(_goodsInfo[1]);
				int binding = Convert.ToInt32(_goodsInfo[2]);
				int forgeLevel = Convert.ToInt32(_goodsInfo[3]);
				int nAppendPropLev = Convert.ToInt32(_goodsInfo[4]);
				int lucky = Convert.ToInt32(_goodsInfo[5]);
				int ExcellenceProperty = Convert.ToInt32(_goodsInfo[6]);
				Global.AddGoodsDBCommand(TCPOutPacketPool.getInstance(), player, goodsId, goodsNum, 0, "", forgeLevel, binding, 0, "", false, 1, "竞技场排行榜奖励", "1900-01-01 12:00:00", 0, 0, lucky, 0, ExcellenceProperty, nAppendPropLev, 0, null, null, 0, true);
			}
		}

		// Token: 0x06002CC7 RID: 11463 RVA: 0x0027EE9D File Offset: 0x0027D09D
		private void changeShengWangValue(GameClient player, int value)
		{
			GameManager.ClientMgr.ModifyShengWangValue(player, value, "竞技场", true, true);
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x0027EEB4 File Offset: 0x0027D0B4
		private int getShengWangValue(GameClient player)
		{
			return GameManager.ClientMgr.GetShengWangValue(player);
		}

		// Token: 0x06002CC9 RID: 11465 RVA: 0x0027EED1 File Offset: 0x0027D0D1
		private void modifyJunxian(GameClient player)
		{
			GameManager.ClientMgr.ModifyShengWangLevelValue(player, 1, "改变军衔", true, true);
		}

		// Token: 0x06002CCA RID: 11466 RVA: 0x0027EEE8 File Offset: 0x0027D0E8
		private int getJunxian(GameClient player)
		{
			int junxian = GameManager.ClientMgr.GetShengWangLevelValue(player);
			return (junxian < 0) ? 0 : junxian;
		}

		// Token: 0x06002CCB RID: 11467 RVA: 0x0027EF10 File Offset: 0x0027D110
		private void getRankingRewardValue(GameClient player, int ranking, out int addShengWangValue, out int addExpValue, out string goodsInfos)
		{
			addShengWangValue = -1;
			addExpValue = -1;
			goodsInfos = null;
			foreach (SystemXmlItem xmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
			{
				if (ranking == -1 && xmlItem.GetStringValue("MaxRank").Equals(""))
				{
					addShengWangValue = xmlItem.GetIntValue("ShengWang2", -1);
					addExpValue = xmlItem.GetIntValue("ExpCoefficient2", -1);
					goodsInfos = xmlItem.GetStringValue("GoodsID");
					break;
				}
				if (ranking >= xmlItem.GetIntValue("MinRank", -1) && ranking <= xmlItem.GetIntValue("MaxRank", -1))
				{
					addShengWangValue = xmlItem.GetIntValue("ShengWang2", -1);
					addExpValue = xmlItem.GetIntValue("ExpCoefficient2", -1);
					goodsInfos = xmlItem.GetStringValue("GoodsID");
					break;
				}
			}
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x0027F028 File Offset: 0x0027D228
		public bool isInJingJiFuBen(GameClient player)
		{
			return player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1);
		}

		// Token: 0x06002CCD RID: 11469 RVA: 0x0027F068 File Offset: 0x0027D268
		private int check(GameClient player)
		{
			int result = ResultCode.Success;
			int result2;
			if ((player.ClientData.Level < this.jingjiFuBenItem.GetIntValue("MinLevel", -1) && player.ClientData.ChangeLifeCount == this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1)) || (player.ClientData.IsFlashPlayer == 1 && player.ClientData.MapCode == 6090))
			{
				result = ResultCode.Illegal;
				result2 = result;
			}
			else if (player.ClientData.CurrentLifeV <= 0 || player.ClientData.CurrentAction == 12)
			{
				result = ResultCode.Dead_Error;
				result2 = result;
			}
			else
			{
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06002CCE RID: 11470 RVA: 0x0027F12C File Offset: 0x0027D32C
		public int removeCD(GameClient player)
		{
			int result = this.check(player);
			int result2;
			if (result != ResultCode.Success)
			{
				result2 = result;
			}
			else
			{
				PlayerJingJiData jingjiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == jingjiData.baseProps)
				{
					result2 = ResultCode.Illegal;
				}
				else
				{
					long cdForSeconds = (jingjiData.nextChallengeTime - TimeUtil.NOW()) / 1000L;
					if (cdForSeconds <= 0L)
					{
						result2 = ResultCode.Success;
					}
					else
					{
						int price = (int)Math.Ceiling((double)cdForSeconds * GameManager.systemParamsList.GetParamValueDoubleByName("CDXiaoHaoZhuanShi", 0.0));
						if (price > 0)
						{
							if (player.ClientData.UserMoney < price)
							{
								return ResultCode.Money_Not_Enough_Error;
							}
							if (!GameManager.ClientMgr.SubUserMoney(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().tcpClientPool, TCPOutPacketPool.getInstance(), player, price, "竞技场消除CD", true, true, false, DaiBiSySType.None))
							{
								return ResultCode.Pay_Error;
							}
						}
						Global.sendToDB<bool, byte[]>(10147, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
						result = ResultCode.Success;
						result2 = result;
					}
				}
			}
			return result2;
		}

		// Token: 0x06002CCF RID: 11471 RVA: 0x0027F288 File Offset: 0x0027D488
		public PlayerJingJiData createJingJiData(GameClient player)
		{
			PlayerJingJiData data = new PlayerJingJiData();
			data.roleId = player.ClientData.RoleID;
			data.roleName = Global.FormatRoleName4(player);
			data.name = player.ClientData.RoleName;
			data.zoneId = player.ClientData.ZoneID;
			data.level = player.ClientData.Level;
			data.changeLiveCount = player.ClientData.ChangeLifeCount;
			data.occupationId = player.ClientData.Occupation;
			data.SubOccupation = player.ClientData.SubOccupation;
			data.OccupationList = player.ClientData.OccupationList;
			data.nextChallengeTime = 0L;
			data.nextRewardTime = TimeUtil.NOW() + JingJiChangConstants.RankingReward_CD_Time;
			data.combatForce = player.ClientData.CombatForce;
			data.equipDatas = this.getSaveEquipData(player);
			data.skillDatas = this.getSaveSkillData(player);
			data.baseProps = this.getBaseProps(player);
			data.extProps = this.getExtProps(player);
			data.sex = player.ClientData.RoleSex;
			data.wingData = null;
			if (player.ClientData.MyWingData != null && player.ClientData.MyWingData.WingID > 0)
			{
				data.wingData = player.ClientData.MyWingData;
			}
			data.settingFlags = Global.GetRoleParamsInt64FromDB(player, "SettingBitFlags");
			data.JunTuanId = player.ClientData.JunTuanId;
			data.JunTuanName = player.ClientData.JunTuanName;
			data.JunTuanZhiWu = player.ClientData.JunTuanZhiWu;
			data.LingDi = player.ClientData.LingDi;
			data.ShenShiEquipData = null;
			FuWenTabData shenShiData = ShenShiManager.getInstance().GetRoleFuWenTabData(player);
			if (null != shenShiData)
			{
				data.ShenShiEquipData = new SkillEquipData
				{
					SkillEquip = shenShiData.SkillEquip,
					ShenShiActiveList = shenShiData.ShenShiActiveList
				};
			}
			data.PassiveEffectList = player.ClientData.PassiveEffectList;
			data.CompType = player.ClientData.CompType;
			data.CompZhiWu = player.ClientData.CompZhiWu;
			return data;
		}

		// Token: 0x06002CD0 RID: 11472 RVA: 0x0027F4B8 File Offset: 0x0027D6B8
		public void onPlayerLevelup(GameClient player)
		{
			int nMinLevel;
			int nChangeLifeCount;
			if (GameManager.MagicSwordMgr.IsMagicSword(player))
			{
				nMinLevel = MagicSwordData.InitLevel;
				nChangeLifeCount = MagicSwordData.InitChangeLifeCount;
			}
			else
			{
				nMinLevel = this.jingjiFuBenItem.GetIntValue("MinLevel", -1);
				nChangeLifeCount = this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1);
			}
			if (player.ClientData.Level == nMinLevel && player.ClientData.ChangeLifeCount == nChangeLifeCount && (player.ClientData.IsFlashPlayer != 1 || player.ClientData.MapCode != 6090))
			{
				PlayerJingJiData data = this.createJingJiData(player);
				Global.sendToDB<byte, byte[]>(10142, DataHelper.ObjectToBytes<PlayerJingJiData>(data), player.ServerId);
				if (GameManager.ClientMgr.GetShengWangValue(player) <= 0)
				{
					GameManager.ClientMgr.SaveShengWangValue(player, 0, true);
					GameManager.ClientMgr.NotifySelfParamsValueChange(player, RoleCommonUseIntParamsIndexs.ShengWang, 0);
				}
				if (GameManager.ClientMgr.GetShengWangLevelValue(player) != -1)
				{
					GameManager.ClientMgr.ModifyShengWangLevelValue(player, 0, "初始化军衔二", true, true);
					Global.BroadcastClientMUShengWang(player, this.getJunxian(player));
				}
			}
		}

		// Token: 0x06002CD1 RID: 11473 RVA: 0x0027F5E8 File Offset: 0x0027D7E8
		private List<PlayerJingJiSkillData> getSaveSkillData(GameClient client)
		{
			List<PlayerJingJiSkillData> skillDataList = new List<PlayerJingJiSkillData>();
			List<SkillData> _skillList = client.ClientData.SkillDataList;
			List<PlayerJingJiSkillData> result;
			if (_skillList == null || _skillList.Count == 0)
			{
				result = skillDataList;
			}
			else
			{
				int nOccupation = Global.CalcOriginalOccupationID(client);
				EMagicSwordTowardType eMagicSwordType = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(client.ClientData.Occupation, client.UsingEquipMgr.GetWeaponEquipList(), client);
				int[] skillIdList = JingJiChangConstants.GetJingJiChangeSkillList(client, Global.CalcOriginalOccupationID(nOccupation), eMagicSwordType);
				foreach (int skillId in skillIdList)
				{
					foreach (SkillData _skillData in _skillList)
					{
						if (skillId == _skillData.SkillID)
						{
							PlayerJingJiSkillData skillData = new PlayerJingJiSkillData();
							skillData.skillID = _skillData.SkillID;
							skillData.skillLevel = _skillData.SkillLevel;
							skillData.skillLevel += TalentManager.GetSkillLevel(client, skillData.skillID);
							skillData.skillLevel = Math.Min(skillData.skillLevel, Global.MaxSkillLevel);
							skillData.skillLevel = Global.GMax(0, skillData.skillLevel);
							skillDataList.Add(skillData);
							break;
						}
					}
				}
				result = skillDataList;
			}
			return result;
		}

		// Token: 0x06002CD2 RID: 11474 RVA: 0x0027F76C File Offset: 0x0027D96C
		private double[] getBaseProps(GameClient player)
		{
			return new double[]
			{
				RoleAlgorithm.GetStrength(player, true),
				RoleAlgorithm.GetIntelligence(player, true),
				RoleAlgorithm.GetDexterity(player, true),
				RoleAlgorithm.GetConstitution(player, true)
			};
		}

		// Token: 0x06002CD3 RID: 11475 RVA: 0x0027F7B0 File Offset: 0x0027D9B0
		private double[] getExtProps(GameClient player)
		{
			double[] extProps = new double[177];
			extProps[0] = RoleAlgorithm.GetStrong(player);
			extProps[1] = RoleAlgorithm.GetAttackSpeed(player);
			extProps[2] = RoleAlgorithm.GetMoveSpeed(player);
			extProps[3] = RoleAlgorithm.GetMinADefenseV(player);
			extProps[4] = RoleAlgorithm.GetMaxADefenseV(player);
			extProps[5] = RoleAlgorithm.GetMinMDefenseV(player);
			extProps[6] = RoleAlgorithm.GetMaxMDefenseV(player);
			extProps[7] = RoleAlgorithm.GetMinAttackV(player);
			extProps[8] = RoleAlgorithm.GetMaxAttackV(player);
			extProps[9] = RoleAlgorithm.GetMinMagicAttackV(player);
			extProps[10] = RoleAlgorithm.GetMaxMagicAttackV(player);
			extProps[11] = player.RoleBuffer.GetExtProp(11);
			extProps[12] = player.RoleBuffer.GetExtProp(12);
			extProps[13] = RoleAlgorithm.GetMaxLifeV(player);
			extProps[14] = RoleAlgorithm.GetMaxLifePercentV(player);
			extProps[15] = RoleAlgorithm.GetMaxMagicV(player);
			extProps[16] = RoleAlgorithm.GetMaxMagicPercent(player);
			extProps[17] = RoleAlgorithm.GetLuckV(player);
			extProps[18] = RoleAlgorithm.GetHitV(player);
			extProps[19] = RoleAlgorithm.GetDodgeV(player);
			extProps[20] = RoleAlgorithm.GetLifeRecoverAddPercentV(player);
			extProps[21] = RoleAlgorithm.GetMagicRecoverAddPercentV(player);
			extProps[22] = RoleAlgorithm.GetLifeRecoverValPercentV(player);
			extProps[23] = RoleAlgorithm.GetMagicRecoverValPercentV(player);
			extProps[24] = RoleAlgorithm.GetSubAttackInjurePercent(player);
			extProps[25] = RoleAlgorithm.GetSubAttackInjureValue(player);
			extProps[26] = RoleAlgorithm.GetAddAttackInjurePercent(player);
			extProps[27] = RoleAlgorithm.GetAddAttackInjureValue(player);
			extProps[28] = RoleAlgorithm.GetIgnoreDefensePercent(player);
			extProps[29] = RoleAlgorithm.GetDamageThornPercent(player);
			extProps[30] = RoleAlgorithm.GetDamageThorn(player);
			extProps[31] = RoleAlgorithm.GetPhySkillIncrease(player);
			extProps[32] = 0.0;
			extProps[33] = RoleAlgorithm.GetMagicSkillIncrease(player);
			extProps[34] = 0.0;
			extProps[35] = RoleAlgorithm.GetFatalAttack(player);
			extProps[36] = RoleAlgorithm.GetDoubleAttack(player);
			extProps[37] = RoleAlgorithm.GetDecreaseInjurePercent(player);
			extProps[38] = RoleAlgorithm.GetDecreaseInjureValue(player);
			extProps[39] = RoleAlgorithm.GetCounteractInjurePercent(player);
			extProps[40] = RoleAlgorithm.GetCounteractInjureValue(player);
			extProps[41] = RoleAlgorithm.GetIgnoreDefenseRate(player);
			extProps[42] = player.RoleBuffer.GetExtProp(42);
			extProps[43] = player.RoleBuffer.GetExtProp(43);
			extProps[44] = RoleAlgorithm.GetLifeStealV(player);
			extProps[51] = RoleAlgorithm.GetDeLuckyAttack(player);
			extProps[52] = RoleAlgorithm.GetDeFatalAttack(player);
			extProps[53] = RoleAlgorithm.GetDeDoubleAttack(player);
			extProps[56] = RoleAlgorithm.GetFrozenPercent(player);
			extProps[57] = RoleAlgorithm.GetPalsyPercent(player);
			extProps[58] = RoleAlgorithm.GetSpeedDownPercent(player);
			extProps[59] = RoleAlgorithm.GetBlowPercent(player);
			extProps[61] = RoleAlgorithm.GetSavagePercent(player);
			extProps[62] = RoleAlgorithm.GetColdPercent(player);
			extProps[63] = RoleAlgorithm.GetRuthlessPercent(player);
			extProps[64] = RoleAlgorithm.GetDeSavagePercent(player);
			extProps[65] = RoleAlgorithm.GetDeColdPercent(player);
			extProps[66] = RoleAlgorithm.GetDeRuthlessPercent(player);
			extProps[97] = RoleAlgorithm.GetExtProp(player, 97);
			extProps[98] = RoleAlgorithm.GetExtProp(player, 98);
			extProps[99] = RoleAlgorithm.GetExtProp(player, 99);
			extProps[100] = RoleAlgorithm.GetExtProp(player, 100);
			extProps[118] = RoleAlgorithm.GetExtProp(player, 118);
			for (int i = 119; i < 177; i++)
			{
				extProps[i] = RoleAlgorithm.GetExtProp(player, i);
			}
			return extProps;
		}

		// Token: 0x06002CD4 RID: 11476 RVA: 0x0027FA88 File Offset: 0x0027DC88
		private List<PlayerJingJiEquipData> getSaveEquipData(GameClient client)
		{
			List<PlayerJingJiEquipData> equipDataList = new List<PlayerJingJiEquipData>();
			List<GoodsData> goodsDataList = client.ClientData.GoodsDataList;
			if (null != goodsDataList)
			{
				foreach (GoodsData goods in goodsDataList)
				{
					if (this.canSaveEquip(goods))
					{
						equipDataList.Add(new PlayerJingJiEquipData
						{
							EquipId = goods.GoodsID,
							ExcellenceInfo = goods.ExcellenceInfo,
							Forge_level = goods.Forge_level,
							BagIndex = goods.BagIndex
						});
					}
				}
			}
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						GoodsData DamonGoodsData = client.ClientData.DamonGoodsDataList[i];
						if (DamonGoodsData.GCount > 0 && 0 != DamonGoodsData.Using)
						{
							equipDataList.Add(new PlayerJingJiEquipData
							{
								EquipId = DamonGoodsData.GoodsID,
								ExcellenceInfo = DamonGoodsData.ExcellenceInfo,
								Forge_level = DamonGoodsData.Forge_level
							});
						}
					}
				}
			}
			if (null != client.ClientData.FashionGoodsDataList)
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].GCount > 0 && client.ClientData.FashionGoodsDataList[i].Using != 0 && client.ClientData.FashionGoodsDataList[i].Site == 6000)
						{
							GoodsData FashionGoodsData = client.ClientData.FashionGoodsDataList[i];
							equipDataList.Add(new PlayerJingJiEquipData
							{
								EquipId = FashionGoodsData.GoodsID,
								ExcellenceInfo = FashionGoodsData.ExcellenceInfo,
								Forge_level = FashionGoodsData.Forge_level
							});
						}
					}
				}
			}
			return equipDataList;
		}

		// Token: 0x06002CD5 RID: 11477 RVA: 0x0027FD6C File Offset: 0x0027DF6C
		private bool canSaveEquip(GoodsData equip)
		{
			bool result;
			if (equip.Site != 0 && equip.Site != 5000)
			{
				result = false;
			}
			else if (equip.Using > 0)
			{
				int category = Global.GetGoodsCatetoriy(equip.GoodsID);
				result = (category >= 0 && category < 49 && category != 5 && category != 6 && category != 22 && category != 23);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x0027FDEC File Offset: 0x0027DFEC
		public bool IsJingJiChangMap(int nMapCode)
		{
			return nMapCode == this.nJingJiChangMapCode;
		}

		// Token: 0x06002CD7 RID: 11479 RVA: 0x0027FE14 File Offset: 0x0027E014
		public int requestChallenge(GameClient player, int beChallengerId, int beChallengerRanking, int enterType)
		{
			int result = this.check(player);
			int result2;
			if (result != ResultCode.Success)
			{
				result2 = result;
			}
			else if (this.IsJingJiChangMap(player.ClientData.MapCode))
			{
				result = ResultCode.Challenge_CD_Error;
				result2 = result;
			}
			else
			{
				JingJiBeChallengeData requestChallengeData = Global.sendToDB<JingJiBeChallengeData, byte[]>(10143, DataHelper.ObjectToBytes<int[]>(new int[]
				{
					player.ClientData.RoleID,
					beChallengerId,
					beChallengerRanking
				}), player.ServerId);
				result = requestChallengeData.state;
				if (result == -1)
				{
					int nNeedVip = (int)GameManager.systemParamsList.GetParamValueDoubleByName("VIPJingJiCD", 0.0);
					if (nNeedVip > 0 && player.ClientData.VipLevel >= nNeedVip)
					{
						result = ResultCode.Success;
					}
				}
				if (result != ResultCode.Success)
				{
					switch (result)
					{
					case -5:
						result = ResultCode.CantChallenger;
						break;
					case -4:
						result = ResultCode.BeChallenger_Lock_Error;
						break;
					case -3:
						result = ResultCode.BeChallenger_Ranking_Change_Error;
						break;
					case -2:
						result = ResultCode.BeChallenger_Null_Error;
						break;
					case -1:
						result = ResultCode.Challenge_CD_Error;
						break;
					default:
						result = ResultCode.Illegal;
						break;
					}
					result2 = result;
				}
				else
				{
					result = this.checkEnterNum(player, enterType);
					if (result != ResultCode.Success)
					{
						result2 = result;
					}
					else
					{
						result = this.enterJingJiChang(player, requestChallengeData.beChallengerData, JingJiFuBenType.NORMAL);
						result2 = result;
					}
				}
			}
			return result2;
		}

		// Token: 0x06002CD8 RID: 11480 RVA: 0x0027FF90 File Offset: 0x0027E190
		public bool checkAction(GameClient player)
		{
			bool result;
			if (player.ClientData.StallDataItem != null)
			{
				result = false;
			}
			else
			{
				int currentAction = player.ClientData.CurrentAction;
				switch (currentAction)
				{
				case 3:
				case 4:
				case 6:
				case 9:
					break;
				case 5:
				case 7:
				case 8:
					goto IL_52;
				default:
					if (currentAction != 24)
					{
						goto IL_52;
					}
					break;
				}
				return false;
				IL_52:
				result = true;
			}
			return result;
		}

		// Token: 0x06002CD9 RID: 11481 RVA: 0x0027FFF4 File Offset: 0x0027E1F4
		public int checkEnterNum(GameClient player, int enterType)
		{
			int result = ResultCode.Success;
			int freeNum = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			FuBenData jingjifuBenData = Global.GetFuBenData(player, this.jingjiFuBenId);
			int nFinishNum;
			int useTotalNum = Global.GetFuBenEnterNum(jingjifuBenData, out nFinishNum);
			if (enterType == JingJiChangConstants.Enter_Type_Free)
			{
				if (useTotalNum >= freeNum)
				{
					return ResultCode.FreeNum_Error;
				}
			}
			else if (enterType == JingJiChangConstants.Enter_Type_Vip)
			{
				if (useTotalNum >= freeNum)
				{
					int vipNum = useTotalNum - freeNum;
					int[] vipJingjiCounts = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
					int playerVipLev = player.ClientData.VipLevel;
					int vipCanUseNum = vipJingjiCounts[playerVipLev];
					if (vipCanUseNum <= vipNum)
					{
						return ResultCode.VipNum_Error;
					}
					int price = (int)GameManager.systemParamsList.GetParamValueIntByName("VIPGouMaiJingJi", -1);
					if (player.ClientData.UserMoney < price)
					{
						return ResultCode.Money_Not_Enough_Error;
					}
					if (!GameManager.ClientMgr.SubUserMoney(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().tcpClientPool, TCPOutPacketPool.getInstance(), player, price, "竞技场额外进入", true, true, false, DaiBiSySType.None))
					{
						result = ResultCode.Pay_Error;
					}
					else
					{
						result = ResultCode.Success;
					}
				}
			}
			return result;
		}

		// Token: 0x06002CDA RID: 11482 RVA: 0x00280160 File Offset: 0x0027E360
		public int JingJiChangStartFight(GameClient client)
		{
			int result;
			if (this.IsHaveFuBen(client.ClientData.FuBenSeqID))
			{
				JingJiChangInstance instance = null;
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.TryGetValue(client.ClientData.FuBenSeqID, out instance);
				}
				if (null == instance)
				{
					result = -1;
				}
				else
				{
					if (instance.getState() == JingJiFuBenState.INITIALIZED)
					{
						instance.ResetJingJiTime();
						instance.switchState(JingJiFuBenState.WAITING_CHANGEMAP_FINISH);
					}
					result = 0;
				}
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x06002CDB RID: 11483 RVA: 0x00280214 File Offset: 0x0027E414
		public int enterJingJiChang(GameClient client, PlayerJingJiData beChallengerData, JingJiFuBenType type = JingJiFuBenType.NORMAL)
		{
			ProcessTask.ProcessAddTaskVal(client, TaskTypes.JingJiChang, -1, 1, new object[0]);
			GameMap gameMap = null;
			int mapId = this.jingjiFuBenItem.GetIntValue("MapCode", -1);
			int result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapId, out gameMap))
			{
				result = ResultCode.Map_Error;
			}
			else
			{
				string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (dbFields == null || dbFields.Length < 2)
				{
					result = ResultCode.FubenSeqId_Error;
				}
				else
				{
					int fuBenSeqID = Global.SafeConvertToInt32(dbFields[1]);
					client.ClientData.FuBenSeqID = fuBenSeqID;
					FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, client.ClientData.FuBenSeqID, 0, this.jingjiFuBenId);
					Robot robot = this.createRobot(client, beChallengerData, this.jingjiFuBenItem.GetIntValue("MapCode", -1));
					JingJiChangInstance jingjichangInstance = new JingJiChangInstance(client, robot, fuBenSeqID, type);
					lock (this.jingjichangInstances)
					{
						this.jingjichangInstances.Add(jingjichangInstance.getFuBenSeqId(), jingjichangInstance);
					}
					ScheduleExecutor2.Instance.scheduleExecute(jingjichangInstance, 0, 100);
					GameManager.ClientMgr.UserFullLife(client, "进入竞技场", false);
					GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, -1, mapId, gameMap.DefaultBirthPosX, gameMap.DefaultBirthPosY, client.ClientData.RoleDirection, 123);
					if (JingJiFuBenType.NORMAL == type)
					{
						Global.UpdateFuBenData(client, this.jingjiFuBenId, 1, 0);
						client._IconStateMgr.CheckJingJiChangLeftTimes(client);
						client._IconStateMgr.SendIconStateToClient(client);
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JoinJingJiChangTimes));
					}
					result = ResultCode.Success;
				}
			}
			return result;
		}

		// Token: 0x06002CDC RID: 11484 RVA: 0x0028041C File Offset: 0x0027E61C
		public void createSkillIDs(List<PlayerJingJiSkillData> skillDatas, Robot robot)
		{
			if (skillDatas != null && skillDatas.Count != 0)
			{
				int[] skillIDs = new int[skillDatas.Count];
				for (int i = 0; i < skillDatas.Count; i++)
				{
					skillIDs[i] = skillDatas[i].skillID;
					int skillLevel;
					if (!robot.skillInfos.TryGetValue(skillDatas[i].skillID, out skillLevel) || skillDatas[i].skillLevel > skillLevel)
					{
						robot.skillInfos[skillDatas[i].skillID] = skillDatas[i].skillLevel;
					}
				}
				robot.MonsterInfo.SkillIDs = robot.skillInfos.Keys.ToArray<int>();
			}
		}

		// Token: 0x06002CDD RID: 11485 RVA: 0x002804F0 File Offset: 0x0027E6F0
		private double GetRobotExtProps(int nIndex, double[] extProps)
		{
			double result;
			if (nIndex > extProps.Length - 1)
			{
				result = 0.0;
			}
			else
			{
				result = extProps[nIndex];
			}
			return result;
		}

		// Token: 0x06002CDE RID: 11486 RVA: 0x002805C4 File Offset: 0x0027E7C4
		public Robot createRobot(GameClient player, PlayerJingJiData beChallengerData, int mapCode)
		{
			int roleId = (int)GameManager.MonsterIDMgr.GetNewID(mapCode);
			RoleDataMini roleDataMini = this.createRoleDataMini(roleId, beChallengerData, mapCode);
			Robot robot = new Robot(player, roleDataMini);
			robot.Lucky = (int)beChallengerData.extProps[17];
			robot.DoubleValue = (int)beChallengerData.extProps[36];
			robot.FatalValue = (int)beChallengerData.extProps[35];
			robot.DeLucky = this.GetRobotExtProps(51, beChallengerData.extProps);
			robot.DeDoubleValue = this.GetRobotExtProps(53, beChallengerData.extProps);
			robot.DeFatalValue = this.GetRobotExtProps(52, beChallengerData.extProps);
			robot.SavageValue = this.GetRobotExtProps(61, beChallengerData.extProps);
			robot.ColdValue = this.GetRobotExtProps(62, beChallengerData.extProps);
			robot.RuthlessValue = this.GetRobotExtProps(63, beChallengerData.extProps);
			robot.DeSavageValue = this.GetRobotExtProps(64, beChallengerData.extProps);
			robot.DeColdValue = this.GetRobotExtProps(65, beChallengerData.extProps);
			robot.DeRuthlessValue = this.GetRobotExtProps(66, beChallengerData.extProps);
			robot.FireAttack = (int)this.GetRobotExtProps(69, beChallengerData.extProps);
			robot.WaterAttack = (int)this.GetRobotExtProps(70, beChallengerData.extProps);
			robot.LightningAttack = (int)this.GetRobotExtProps(71, beChallengerData.extProps);
			robot.SoilAttack = (int)this.GetRobotExtProps(72, beChallengerData.extProps);
			robot.IceAttack = (int)this.GetRobotExtProps(73, beChallengerData.extProps);
			robot.WindAttack = (int)this.GetRobotExtProps(74, beChallengerData.extProps);
			robot.FirePenetration = (double)((int)this.GetRobotExtProps(75, beChallengerData.extProps));
			robot.WaterPenetration = (double)((int)this.GetRobotExtProps(76, beChallengerData.extProps));
			robot.LightningPenetration = (double)((int)this.GetRobotExtProps(77, beChallengerData.extProps));
			robot.SoilPenetration = (double)((int)this.GetRobotExtProps(78, beChallengerData.extProps));
			robot.IcePenetration = (double)((int)this.GetRobotExtProps(79, beChallengerData.extProps));
			robot.WindPenetration = (double)((int)this.GetRobotExtProps(80, beChallengerData.extProps));
			robot.ElementPenetration = (double)((int)this.GetRobotExtProps(118, beChallengerData.extProps));
			robot.DeFirePenetration = (double)((int)this.GetRobotExtProps(81, beChallengerData.extProps));
			robot.DeWaterPenetration = (double)((int)this.GetRobotExtProps(82, beChallengerData.extProps));
			robot.DeLightningPenetration = (double)((int)this.GetRobotExtProps(83, beChallengerData.extProps));
			robot.DeSoilPenetration = (double)((int)this.GetRobotExtProps(84, beChallengerData.extProps));
			robot.DeIcePenetration = (double)((int)this.GetRobotExtProps(85, beChallengerData.extProps));
			robot.DeWindPenetration = (double)((int)this.GetRobotExtProps(86, beChallengerData.extProps));
			this.createSkillIDs(beChallengerData.skillDatas, robot);
			robot.RoleID = roleId;
			robot.UniqueID = Global.GetUniqueID();
			robot.PlayerId = beChallengerData.roleId;
			robot.Name = string.Format("Role_{0}", robot.RoleID);
			robot.MonsterInfo.VSName = beChallengerData.roleName;
			robot.MonsterInfo.SpriteSpeedTickList = new int[]
			{
				148,
				222,
				0,
				222,
				222,
				0,
				185,
				0,
				0,
				0,
				0,
				100,
				148
			};
			robot.MonsterInfo.EachActionFrameRange = new int[]
			{
				3,
				3,
				0,
				3,
				3,
				0,
				3,
				0,
				0,
				0,
				0,
				1,
				3
			};
			robot.MonsterInfo.EffectiveFrame = new int[]
			{
				-1,
				-1,
				-1,
				1,
				1,
				0,
				1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			};
			robot.Sex = beChallengerData.sex;
			robot.VLife = beChallengerData.extProps[13];
			robot.VMana = beChallengerData.extProps[15];
			robot.MonsterInfo.VLifeMax = beChallengerData.extProps[13];
			robot.MonsterInfo.VManaMax = beChallengerData.extProps[15];
			robot.MoveSpeed = beChallengerData.extProps[2];
			int baseOccupation = (beChallengerData.occupationId - beChallengerData.changeLiveCount > 10) ? ((beChallengerData.occupationId - beChallengerData.changeLiveCount) / 10 - 1) : beChallengerData.occupationId;
			EMagicSwordTowardType eMagicSwordType = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(baseOccupation, robot.getRoleDataMini().GoodsDataList, null);
			robot.MonsterInfo.MinAttack = this.GetRobotMinAttack(baseOccupation, eMagicSwordType, beChallengerData);
			robot.MonsterInfo.MaxAttack = this.GetRobotMaxAttack(baseOccupation, eMagicSwordType, beChallengerData);
			robot.MonsterInfo.Defense = (int)beChallengerData.extProps[4];
			robot.MonsterInfo.MDefense = (int)beChallengerData.extProps[6];
			robot.MonsterInfo.HitV = beChallengerData.extProps[18];
			robot.MonsterInfo.Dodge = beChallengerData.extProps[19];
			robot.MonsterInfo.RecoverLifeV = beChallengerData.extProps[20];
			robot.MonsterInfo.RecoverMagicV = beChallengerData.extProps[21];
			robot.MonsterInfo.ExtProps = new double[177];
			for (int i = 0; i < beChallengerData.extProps.Length; i++)
			{
				robot.MonsterInfo.ExtProps[i] = beChallengerData.extProps[i];
				robot.DynamicData.ExtProps[i] = beChallengerData.extProps[i];
			}
			robot.MonsterInfo.VLevel = beChallengerData.level;
			robot.MonsterInfo.ChangeLifeCount = beChallengerData.changeLiveCount;
			robot.MonsterInfo.VExperience = 0;
			robot.MonsterInfo.VMoney = 0;
			robot.MonsterInfo.SeekRange = 100;
			robot.MonsterInfo.EquipmentBody = -1;
			robot.MonsterInfo.EquipmentWeapon = -1;
			robot.MonsterInfo.ToOccupation = baseOccupation;
			robot.MonsterInfo.FallGoodsPackID = -1;
			robot.MonsterType = 1801;
			robot.MonsterInfo.BattlePersonalJiFen = 0;
			robot.MonsterInfo.BattleZhenYingJiFen = 0;
			robot.MonsterInfo.FallBelongTo = 0;
			robot.MonsterInfo.DaimonSquareJiFen = 0;
			robot.MonsterInfo.BloodCastJiFen = 0;
			robot.MonsterInfo.WolfScore = 0;
			robot.MonsterInfo.AttackType = this.GetRobotAttackType(baseOccupation, eMagicSwordType);
			robot.Camp = -1;
			robot.PetAiControlType = -1;
			robot.NextSeekEnemyTicks = 500L;
			robot.OwnerClient = null;
			robot.OwnerMonster = null;
			robot.FrozenPercent = this.GetRobotExtProps(56, beChallengerData.extProps);
			robot.PalsyPercent = this.GetRobotExtProps(57, beChallengerData.extProps);
			robot.SpeedDownPercent = this.GetRobotExtProps(58, beChallengerData.extProps);
			robot.BlowPercent = this.GetRobotExtProps(59, beChallengerData.extProps);
			robot.DeFrozenPercent = this.GetRobotExtProps(97, beChallengerData.extProps);
			robot.DePalsyPercent = this.GetRobotExtProps(98, beChallengerData.extProps);
			robot.DeSpeedDownPercent = this.GetRobotExtProps(99, beChallengerData.extProps);
			robot.DeBlowPercent = this.GetRobotExtProps(100, beChallengerData.extProps);
			return robot;
		}

		// Token: 0x06002CDF RID: 11487 RVA: 0x00280CC8 File Offset: 0x0027EEC8
		private RoleDataMini createRoleDataMini(int roleId, PlayerJingJiData data, int mapCode)
		{
			int armorMax = (int)this.GetRobotExtProps(119, data.extProps);
			RoleDataMini roleData = new RoleDataMini
			{
				RoleID = roleId,
				RoleName = data.name,
				ZoneID = data.zoneId,
				RoleSex = data.sex,
				Occupation = data.occupationId,
				SubOccupation = data.SubOccupation,
				OccupationList = data.OccupationList,
				Level = data.level,
				MapCode = mapCode,
				MaxLifeV = (int)data.extProps[13],
				LifeV = (int)data.extProps[13],
				MaxMagicV = (int)data.extProps[15],
				MagicV = (int)data.extProps[15],
				BodyCode = this.FindEquipCode(data.equipDatas, 1),
				WeaponCode = this.FindEquipCode(data.equipDatas, 0),
				GoodsDataList = JingJiChangManager.GetUsingGoodsList(data.equipDatas),
				ChangeLifeLev = data.changeLiveCount,
				ChangeLifeCount = data.changeLiveCount,
				BufferMiniInfo = new List<BufferDataMini>(),
				MyWingData = data.wingData,
				SettingBitFlags = data.settingFlags,
				JunTuanId = data.JunTuanId,
				JunTuanName = data.JunTuanName,
				JunTuanZhiWu = data.JunTuanZhiWu,
				LingDi = data.LingDi,
				HuiJiData = data.HuiJiData,
				ShenShiEquipData = data.ShenShiEquipData,
				PassiveEffectList = data.PassiveEffectList,
				CurrentArmorV = armorMax,
				MaxArmorV = armorMax
			};
			roleData.BodyCode = Global.GMax(roleData.RoleSex, roleData.BodyCode);
			roleData.WeaponCode = Global.GMax(0, roleData.WeaponCode);
			return roleData;
		}

		// Token: 0x06002CE0 RID: 11488 RVA: 0x00280E94 File Offset: 0x0027F094
		public static List<GoodsData> GetUsingGoodsList(List<PlayerJingJiEquipData> equipDatas)
		{
			int WuQiNum = 0;
			int Hand = -1;
			HashSet<int> WuQiBagIndexSet = new HashSet<int>();
			List<GoodsData> goodsDataList = new List<GoodsData>();
			if (null != equipDatas)
			{
				for (int i = 0; i < equipDatas.Count; i++)
				{
					int category = Global.GetGoodsCatetoriy(equipDatas[i].EquipId);
					GoodsData data = new GoodsData();
					data.GoodsID = equipDatas[i].EquipId;
					data.ExcellenceInfo = equipDatas[i].ExcellenceInfo;
					data.Forge_level = equipDatas[i].Forge_level;
					data.BagIndex = equipDatas[i].BagIndex;
					data.Using = 1;
					if (category >= 11 && category <= 21)
					{
						WuQiBagIndexSet.Add(data.BagIndex);
						SystemXmlItem systemGoods = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipDatas[i].EquipId, out systemGoods))
						{
							WuQiNum++;
							int handType = systemGoods.GetIntValue("HandType", -1);
							if (2 != handType)
							{
								Hand = handType;
							}
							else
							{
								WuQiNum++;
							}
						}
					}
					goodsDataList.Add(data);
				}
				if (WuQiNum >= 3 && WuQiBagIndexSet.Count == 1)
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						int tmpcategory = Global.GetGoodsCatetoriy(goodsDataList[i].GoodsID);
						if (tmpcategory >= 11 && tmpcategory <= 21)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsDataList[i].GoodsID, out systemGoods))
							{
								int handType = systemGoods.GetIntValue("HandType", -1);
								if (2 == handType)
								{
									if (-1 == Hand)
									{
										goodsDataList[i].BagIndex = 1;
										break;
									}
									goodsDataList[i].BagIndex = Hand;
									break;
								}
							}
						}
					}
				}
			}
			return goodsDataList;
		}

		// Token: 0x06002CE1 RID: 11489 RVA: 0x002810CC File Offset: 0x0027F2CC
		private int FindEquipCode(List<PlayerJingJiEquipData> equipDatas, int category)
		{
			int result;
			if (equipDatas == null)
			{
				result = -1;
			}
			else
			{
				lock (equipDatas)
				{
					for (int i = 0; i < equipDatas.Count; i++)
					{
						SystemXmlItem systemGoods = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipDatas[i].EquipId, out systemGoods))
						{
							if ((category >= 0 && category <= 4) || (category >= 7 && category <= 21))
							{
								return systemGoods.GetIntValue("EquipCode", -1);
							}
						}
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x06002CE2 RID: 11490 RVA: 0x0028119C File Offset: 0x0027F39C
		public bool IsHaveFuBen(int nFuBenSeqID)
		{
			bool bContainFuBenID = false;
			lock (this.jingjichangInstances)
			{
				if (this.jingjichangInstances.ContainsKey(nFuBenSeqID))
				{
					bContainFuBenID = true;
				}
			}
			return bContainFuBenID;
		}

		// Token: 0x06002CE3 RID: 11491 RVA: 0x00281204 File Offset: 0x0027F404
		public void onChallengeEndForPlayerDead(GameClient player, Monster monster)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1) && monster.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance instance = null;
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out instance);
				}
				if (null != instance)
				{
					lock (instance)
					{
						if (instance.getState() != JingJiFuBenState.STOP_CD && instance.getState() != JingJiFuBenState.STOPED && instance.getState() != JingJiFuBenState.DESTROYED)
						{
							Robot robot = instance.getRobot();
							robot.stopAttack();
							this.processFailed(player, robot, new JingJiChallengeResultData
							{
								playerId = player.ClientData.RoleID,
								robotId = robot.PlayerId,
								isWin = false
							}, instance.type);
							instance.switchState(JingJiFuBenState.STOP_CD);
						}
					}
				}
			}
		}

		// Token: 0x06002CE4 RID: 11492 RVA: 0x002813A4 File Offset: 0x0027F5A4
		public void onChallengeEndForMonsterDead(GameClient player, Monster monster)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1) && monster.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				if (monster is Robot)
				{
					JingJiChangInstance instance = null;
					lock (this.jingjichangInstances)
					{
						if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out instance))
						{
							return;
						}
					}
					if (monster.VLife <= 0.0 && player.ClientData.CurrentLifeV > 0)
					{
						lock (instance)
						{
							if (instance.getState() != JingJiFuBenState.STOP_CD && instance.getState() != JingJiFuBenState.STOPED && instance.getState() != JingJiFuBenState.DESTROYED)
							{
								Robot robot = instance.getRobot();
								robot.stopAttack();
								this.processWin(player, robot, new JingJiChallengeResultData
								{
									playerId = player.ClientData.RoleID,
									robotId = robot.PlayerId,
									isWin = true
								}, instance.type);
								instance.switchState(JingJiFuBenState.STOP_CD);
							}
						}
					}
					else
					{
						this.onChallengeEndForPlayerDead(player, monster);
					}
				}
			}
		}

		// Token: 0x06002CE5 RID: 11493 RVA: 0x0028158C File Offset: 0x0027F78C
		public void onChallengeEndForPlayerLeaveFuBen(GameClient player)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance instance = null;
				lock (this.jingjichangInstances)
				{
					if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out instance))
					{
						return;
					}
				}
				lock (instance)
				{
					if (instance.getState() != JingJiFuBenState.STOP_CD && instance.getState() != JingJiFuBenState.STOPED && instance.getState() != JingJiFuBenState.DESTROYED)
					{
						Robot robot = instance.getRobot();
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, instance.type);
						if (player.ClientData.CurrentLifeV <= 0)
						{
							this.relive(player);
						}
						instance.switchState(JingJiFuBenState.DESTROYED);
					}
				}
			}
		}

		// Token: 0x06002CE6 RID: 11494 RVA: 0x0028172C File Offset: 0x0027F92C
		public void onChallengeEndForPlayerLogout(GameClient player)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance instance = null;
				lock (this.jingjichangInstances)
				{
					if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out instance))
					{
						return;
					}
				}
				lock (instance)
				{
					if (instance.getState() != JingJiFuBenState.STOP_CD && instance.getState() != JingJiFuBenState.STOPED && instance.getState() != JingJiFuBenState.DESTROYED)
					{
						Robot robot = instance.getRobot();
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, instance.type);
						if (player.ClientData.CurrentLifeV <= 0)
						{
							this.relive(player);
						}
						instance.switchState(JingJiFuBenState.DESTROYED);
					}
				}
			}
		}

		// Token: 0x06002CE7 RID: 11495 RVA: 0x002818CC File Offset: 0x0027FACC
		private void processWin(GameClient player, Robot robot, JingJiChallengeResultData resultData, JingJiFuBenType type)
		{
			if (JingJiFuBenType.NORMAL != type)
			{
				GlobalEventSource.getInstance().fireEvent(new JingJiChangWinEventObject(player, robot, (int)type));
			}
			else
			{
				int ranking = this.getChallengeEndRanking(resultData, player.ServerId);
				int addShengWangValue;
				int addExpValue;
				int challengeCD;
				this.getChallengeReward(player, ranking, true, out addShengWangValue, out addExpValue, out challengeCD);
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)addExpValue, true, true, false, "none");
				this.changeShengWangValue(player, addShengWangValue);
				JingJiSaveData saveData = new JingJiSaveData();
				saveData.isWin = true;
				saveData.nextChallengeTime = ((challengeCD > 0) ? (TimeUtil.NOW() + (long)(challengeCD * 1000)) : 0L);
				saveData.roleId = player.ClientData.RoleID;
				saveData.level = player.ClientData.Level;
				saveData.changeLiveCount = player.ClientData.ChangeLifeCount;
				saveData.combatForce = player.ClientData.CombatForce;
				saveData.equipDatas = this.getSaveEquipData(player);
				saveData.skillDatas = this.getSaveSkillData(player);
				saveData.baseProps = this.getBaseProps(player);
				saveData.extProps = this.getExtProps(player);
				saveData.robotId = robot.PlayerId;
				saveData.wingData = null;
				saveData.Occupation = player.ClientData.Occupation;
				saveData.SubOccupation = player.ClientData.SubOccupation;
				saveData.ShenShiEuipSkill = null;
				if (player.ClientData.MyWingData != null && player.ClientData.MyWingData.WingID > 0)
				{
					saveData.wingData = player.ClientData.MyWingData;
				}
				saveData.settingFlags = Global.GetRoleParamsInt64FromDB(player, "SettingBitFlags");
				saveData.HuiJiData = player.ClientData.HuiJiData;
				FuWenTabData shenShiData = ShenShiManager.getInstance().GetRoleFuWenTabData(player);
				if (null != shenShiData)
				{
					saveData.ShenShiEuipSkill = new SkillEquipData
					{
						SkillEquip = shenShiData.SkillEquip,
						ShenShiActiveList = shenShiData.ShenShiActiveList
					};
				}
				saveData.PassiveEffectList = player.PassiveEffectList;
				int winCount = Global.sendToDB<int, byte[]>(10145, DataHelper.ObjectToBytes<JingJiSaveData>(saveData), player.ServerId);
				if (winCount > 0)
				{
					this.LianShengGongGao(player, robot.MonsterInfo.VSName, false, winCount);
				}
				player.sendCmd(580, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					addShengWangValue,
					addExpValue,
					ranking
				}), false);
				SevenDayGoalEventObject evRank = SevenDayGoalEvPool.Alloc(player, ESevenDayGoalFuncType.JingJiChangRank);
				evRank.Arg1 = ranking;
				GlobalEventSource.getInstance().fireEvent(evRank);
				SevenDayGoalEventObject evWin = SevenDayGoalEvPool.Alloc(player, ESevenDayGoalFuncType.WinJingJiChangTimes);
				GlobalEventSource.getInstance().fireEvent(evWin);
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(player, OrnamentGoalType.OGT_JingJiChallenge, new int[0]));
				ProcessTask.ProcessAddTaskVal(player, TaskTypes.JingJiChang_Win, -1, 1, new object[0]);
			}
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x00281BCC File Offset: 0x0027FDCC
		private void processFailed(GameClient player, Robot robot, JingJiChallengeResultData resultData, JingJiFuBenType type)
		{
			if (JingJiFuBenType.NORMAL != type)
			{
				GlobalEventSource.getInstance().fireEvent(new JingJiChangFailedEventObject(player, robot, (int)type));
			}
			else
			{
				int ranking = this.getChallengeEndRanking(resultData, player.ServerId);
				int addShengWangValue;
				int addExpValue;
				int challengeCD;
				this.getChallengeReward(player, ranking, false, out addShengWangValue, out addExpValue, out challengeCD);
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)addExpValue, true, true, false, "none");
				this.changeShengWangValue(player, addShengWangValue);
				int winCount = Global.sendToDB<int, byte[]>(10145, DataHelper.ObjectToBytes<JingJiSaveData>(new JingJiSaveData
				{
					isWin = false,
					nextChallengeTime = ((challengeCD > 0) ? (TimeUtil.NOW() + (long)(challengeCD * 1000)) : 0L),
					robotId = robot.PlayerId,
					roleId = player.ClientData.RoleID
				}), player.ServerId);
				if (winCount > 0)
				{
					this.LianShengGongGao(player, robot.MonsterInfo.VSName, false, winCount);
				}
				player.sendCmd(580, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					addShengWangValue,
					addExpValue,
					ranking
				}), false);
			}
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x00281D10 File Offset: 0x0027FF10
		private void LianShengGongGao(GameClient player, string robotName, bool isWin, int winCount)
		{
		}

		// Token: 0x06002CEA RID: 11498 RVA: 0x00281D14 File Offset: 0x0027FF14
		private void getChallengeReward(GameClient player, int ranking, bool isWin, out int addShengWangValue, out int addExpValue, out int challengeCD)
		{
			addShengWangValue = -1;
			addExpValue = -1;
			challengeCD = -1;
			foreach (SystemXmlItem xmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
			{
				int nChangeLev = player.ClientData.ChangeLifeCount;
				double nRate;
				if (nChangeLev == 0)
				{
					nRate = 1.0;
				}
				else
				{
					nRate = Data.ChangeLifeEverydayExpRate[nChangeLev];
				}
				addExpValue = (int)((double)xmlItem.GetIntValue("ExpCoefficient1", -1) * nRate);
				if (ranking == -1 && xmlItem.GetStringValue("MaxRank").Equals(""))
				{
					if (isWin)
					{
						addShengWangValue = xmlItem.GetIntValue("ShengWang1", -1);
						challengeCD = xmlItem.GetIntValue("CD", -1);
					}
					else
					{
						addShengWangValue = xmlItem.GetIntValue("ShengWang1", -1) / 2;
						addExpValue /= 2;
						challengeCD = xmlItem.GetIntValue("CD", -1);
					}
					break;
				}
				if (ranking >= xmlItem.GetIntValue("MinRank", -1) && ranking <= xmlItem.GetIntValue("MaxRank", -1))
				{
					if (isWin)
					{
						addShengWangValue = xmlItem.GetIntValue("ShengWang1", -1);
						challengeCD = xmlItem.GetIntValue("CD", -1);
					}
					else
					{
						addShengWangValue = xmlItem.GetIntValue("ShengWang1", -1) / 2;
						addExpValue /= 2;
						challengeCD = xmlItem.GetIntValue("CD", -1);
					}
					break;
				}
			}
			int nNeedVip = (int)GameManager.systemParamsList.GetParamValueDoubleByName("VIPJingJiCD", 0.0);
			if (nNeedVip > 0 && player.ClientData.VipLevel >= nNeedVip)
			{
				challengeCD = 0;
			}
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x00281F24 File Offset: 0x00280124
		private int getChallengeEndRanking(JingJiChallengeResultData resultData, int serverId)
		{
			return Global.sendToDB<int, byte[]>(10144, DataHelper.ObjectToBytes<JingJiChallengeResultData>(resultData), serverId);
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x00281F48 File Offset: 0x00280148
		private void relive(GameClient player)
		{
			player.ClientData.CurrentLifeV = player.ClientData.LifeV;
			player.ClientData.CurrentMagicV = player.ClientData.MagicV;
			Global.ClientRealive(player, (int)player.CurrentPos.X, (int)player.CurrentPos.Y, player.ClientData.RoleDirection);
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x00281FB4 File Offset: 0x002801B4
		public void onJingJiFuBenStartCD(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			player.sendCmd(581, "", false);
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x00281FDC File Offset: 0x002801DC
		public void onJingJiFuBenStarted(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			Robot robot = instance.getRobot();
			GameMap gameMap = GameManager.MapMgr.DictMaps[this.jingjiFuBenItem.GetIntValue("MapCode", -1)];
			int gridX = gameMap.CorrectWidthPointToGridPoint(JingJiChangConstants.RobotBothX) / gameMap.MapGridWidth;
			int gridY = gameMap.CorrectHeightPointToGridPoint(JingJiChangConstants.RobotBothY) / gameMap.MapGridHeight;
			GameManager.MonsterZoneMgr.AddDynamicRobot(player.CurrentMapCode, robot, player.ClientData.CopyMapID, 1, gridX, gridY, 1, 0, SceneUIClasses.NormalCopy, player.ClientData.RoleID);
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x00282074 File Offset: 0x00280274
		public void onRobotBron(Robot robot)
		{
			GameClient client = GameManager.ClientMgr.FindClient((int)robot.Tag);
			if (null != client)
			{
				this.SendMySelfJingJiFakeRoleItem(client, robot);
				robot.startAttack();
				GameManager.ClientMgr.BroadSpecialMapAIEvent(client.ClientData.MapCode, client.ClientData.CopyMapID, 1, 0);
			}
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x002820DC File Offset: 0x002802DC
		public void SendMySelfJingJiFakeRoleItem(GameClient player, Robot robot)
		{
			RoleDataMini roleDataMini = robot.getRoleDataMini();
			roleDataMini.PosX = (int)player.CurrentPos.X;
			roleDataMini.PosY = (int)player.CurrentPos.Y;
			player.sendCmd<RoleDataMini>(110, roleDataMini, false);
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x00282128 File Offset: 0x00280328
		public void onJingJiFuBenStopForTimeOutCD(JingJiChangInstance instance)
		{
			if (null != instance)
			{
				lock (instance)
				{
					GameClient player = instance.getPlayer();
					Robot robot = instance.getRobot();
					if (player != null && null != robot)
					{
						GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(player.ClientData.MapCode, player.ClientData.CopyMapID);
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, instance.type);
					}
				}
			}
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x00282204 File Offset: 0x00280404
		public void onJingJiFuBenStoped(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			if (player != null && !player.LogoutState)
			{
				if (player.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
				{
					if (player.ClientData.CurrentLifeV <= 0)
					{
						this.relive(player);
					}
					if (!Global.CanChangeMap(player, player.ClientData.LastMapCode, player.ClientData.LastPosX, player.ClientData.LastPosY, true))
					{
						player.ClientData.LastMapCode = GameManager.MainMapCode;
						player.ClientData.LastPosX = 0;
						player.ClientData.LastPosY = 0;
					}
					GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), player, -1, player.ClientData.LastMapCode, player.ClientData.LastPosX, player.ClientData.LastPosY, player.ClientData.RoleDirection, 123);
					player.sendCmd(587, string.Format("{0}", (int)instance.type), false);
				}
			}
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x00282338 File Offset: 0x00280538
		public void onJingJiFuBenDestroy(JingJiChangInstance instance)
		{
			ScheduleExecutor2.Instance.scheduleCancle(instance);
			lock (this.jingjichangInstances)
			{
				this.jingjichangInstances.Remove(instance.getFuBenSeqId());
			}
			instance.release();
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x002823A4 File Offset: 0x002805A4
		public void onLeaveFuBenForStopCD(GameClient player)
		{
			JingJiChangInstance instance = null;
			lock (this.jingjichangInstances)
			{
				if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out instance))
				{
					return;
				}
			}
			lock (instance)
			{
				if (instance.getState() == JingJiFuBenState.STOP_CD)
				{
					Robot robot = instance.getRobot();
					robot.stopAttack();
					instance.switchState(JingJiFuBenState.STOPED);
				}
			}
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x0028246C File Offset: 0x0028066C
		public int GetLeftEnterCount(GameClient client)
		{
			int freeChallengeNum = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			FuBenData jingjifuBenData = Global.GetFuBenData(client, this.jingjiFuBenId);
			int nFinishNum;
			int useTotalNum = Global.GetFuBenEnterNum(jingjifuBenData, out nFinishNum);
			int useFreeNum = (useTotalNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? useTotalNum : this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			int[] vipJingjiCounts = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
			int playerVipLev = client.ClientData.VipLevel;
			int vipChallengeNum = vipJingjiCounts[playerVipLev];
			int useVipNum = (useTotalNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? 0 : (useTotalNum - this.jingjiFuBenItem.GetIntValue("EnterNumber", -1));
			return freeChallengeNum + vipChallengeNum - useFreeNum - useVipNum;
		}

		// Token: 0x04003B71 RID: 15217
		private static JingJiChangManager instance = new JingJiChangManager();

		// Token: 0x04003B72 RID: 15218
		private SystemXmlItems rewardConfig = new SystemXmlItems();

		// Token: 0x04003B73 RID: 15219
		private SystemXmlItems jingjiMainConfig = new SystemXmlItems();

		// Token: 0x04003B74 RID: 15220
		private SystemXmlItems junxianConfig = new SystemXmlItems();

		// Token: 0x04003B75 RID: 15221
		private SystemXmlItem jingjiFuBenItem = null;

		// Token: 0x04003B76 RID: 15222
		public int nJingJiChangMapCode = 0;

		// Token: 0x04003B77 RID: 15223
		private Dictionary<int, JingJiChangInstance> jingjichangInstances = new Dictionary<int, JingJiChangInstance>();

		// Token: 0x04003B78 RID: 15224
		private int jingjiFuBenId = -1;

		// Token: 0x04003B79 RID: 15225
		private int jingjiBuffId = -1;

		// Token: 0x04003B7A RID: 15226
		private int jingjiFuBenMinZhuanSheng = -1;

		// Token: 0x04003B7B RID: 15227
		private string[] junxianBuffTimeConfig;
	}
}
