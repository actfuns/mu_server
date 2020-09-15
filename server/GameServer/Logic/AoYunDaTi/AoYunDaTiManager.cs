using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000207 RID: 519
	public class AoYunDaTiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x06000681 RID: 1665 RVA: 0x0005A190 File Offset: 0x00058390
		public static AoYunDaTiManager getInstance()
		{
			return AoYunDaTiManager.instance;
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0005A1A8 File Offset: 0x000583A8
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0005A1C4 File Offset: 0x000583C4
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(20200, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20201, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20202, 2, 2, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20203, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20204, 2, 2, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20205, 3, 3, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0005A268 File Offset: 0x00058468
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0005A27C File Offset: 0x0005847C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0005A290 File Offset: 0x00058490
		public bool LoadConfig()
		{
			this.Destory_Work();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				string strGoods = GameManager.systemParamsList.GetParamValueByName(AoyunDaTiConsts.GoodsParams);
				if (strGoods == "" || strGoods == null)
				{
					return true;
				}
				string[] goodsParams = strGoods.Split(new char[]
				{
					'|'
				});
				string[] tinaShi = goodsParams[0].Split(new char[]
				{
					','
				});
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice = new int[2];
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit = new int[2];
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[0] = Convert.ToInt32(tinaShi[0]);
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0] = Convert.ToInt32(tinaShi[1]);
				string[] eMo = goodsParams[1].Split(new char[]
				{
					','
				});
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[1] = Convert.ToInt32(eMo[0]);
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1] = Convert.ToInt32(eMo[1]);
				AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuanShengExpXiShu", ',');
			}
			AoYunDaTiManager.LoadQuestionTime();
			AoYunDaTiManager.LoadQuestionBank();
			AoYunDaTiManager.LoadPaiHangAwad();
			AoYunDaTiManager.LoadAoYunDaTi();
			DateTime now = TimeUtil.NowDateTime();
			this.SetCurrentQuestionTimeItem(now);
			AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = (currentQuestionTime != null && now > currentQuestionTime.BeginTime);
			}
			AoYunDaTiManager.InitAoyunRank();
			AoYunDaTiManager.InitLastAoyunRank();
			AoYunDaTiManager.CheckActivityIcon(AoYunDaTiManager.AoyunRunTimeData.DaTiOpen);
			return true;
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0005A4B8 File Offset: 0x000586B8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0005A4CC File Offset: 0x000586CC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 20200:
					result = this.ProcessAoYunInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 20201:
					result = this.ProcessGetQuestionCmd(client, nID, bytes, cmdParams);
					break;
				case 20202:
					result = this.ProcessAnswerQuestionCmd(client, nID, bytes, cmdParams);
					break;
				case 20203:
					result = this.ProcessRecPaiHangAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 20204:
					result = this.ProcessUseGoodsCmd(client, nID, bytes, cmdParams);
					break;
				case 20205:
					result = this.ProcessBuyGoodsCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0005A5A0 File Offset: 0x000587A0
		public bool ProcessAoYunInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				bool isHaveAward = false;
				int key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey - 1;
				if (key >= 0 && key < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					AoyunQuestionTimeItem lastQuestionTime = AoYunDaTiManager.XmlQuestionTimeList[key];
					DateTime lastAnswerTime = Global.GetRoleParamsDateTimeFromDB(client, "20006");
					DateTime lastBeginTime = lastQuestionTime.BeginTime;
					isHaveAward = (lastAnswerTime > lastBeginTime);
					DateTime recTime = Global.GetRoleParamsDateTimeFromDB(client, "20005");
					DateTime lastEndTime = lastQuestionTime.EndTime;
					isHaveAward = (isHaveAward && lastEndTime > recTime);
				}
				AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTime == null)
				{
					currentQuestionTime = new AoyunQuestionTimeItem
					{
						BeginTime = DateTime.MaxValue,
						EndTime = DateTime.MinValue
					};
				}
				List<AoyunPaiHangRoleData> rankList = new List<AoyunPaiHangRoleData>();
				foreach (AoyunPaiHangRoleData item in AoYunDaTiManager.AoyunRunTimeData.AoyunRankList)
				{
					if (item.RoleRank < 0 || item.RoleRank > 20)
					{
						break;
					}
					rankList.Add(item);
				}
				AoyunDatiMainData data = new AoyunDatiMainData
				{
					AoyunPaiHangRoleDataArray = rankList,
					StartTime = currentQuestionTime.BeginTime,
					EndTime = currentQuestionTime.EndTime,
					SelfRank = -1,
					TianShiNum = Global.GetRoleParamsInt32FromDB(client, "20002"),
					EMoNum = Global.GetRoleParamsInt32FromDB(client, "20004"),
					NextStartTime = DateTime.MaxValue
				};
				int oldRank = 0;
				if (AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
				{
					AoyunPaiHangRoleData roleData = AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Find((AoyunPaiHangRoleData _x) => _x.RoleId == roleID);
					if (roleData != null)
					{
						data.SelfRank = roleData.RoleRank;
					}
				}
				if (AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(roleID, out oldRank) && !AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
				{
					data.SelfRank = oldRank;
				}
				data.IsHaveAward = (isHaveAward && oldRank > 0);
				key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1;
				if (key > 0 && key < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					AoyunQuestionTimeItem nextQuestionTime = AoYunDaTiManager.XmlQuestionTimeList[key];
					data.NextStartTime = nextQuestionTime.BeginTime;
				}
				else
				{
					data.NextStartTime = DateTime.MaxValue;
				}
				client.sendCmd<AoyunDatiMainData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0005A8E0 File Offset: 0x00058AE0
		public bool ProcessGetQuestionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int roleID = Convert.ToInt32(cmdParams[0]);
				AoyunQuestionItemData data = AoYunDaTiManager.GetRoleQuestionFromDB(client, now);
				client.sendCmd<AoyunQuestionItemData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0005A964 File Offset: 0x00058B64
		public bool ProcessAnswerQuestionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int roleID = Convert.ToInt32(cmdParams[0]);
				int answer = Convert.ToInt32(cmdParams[1]);
				int result = 1;
				DateTime lastAnswer = Global.GetRoleParamsDateTimeFromDB(client, "20006");
				AoyunQuestionBankItem currentQuestion = AoYunDaTiManager.GetCurrentQuestionBank();
				if (currentQuestion == null)
				{
					result = -1007;
				}
				else if (answer < 0 || answer > 3)
				{
					result = -1006;
				}
				else if (lastAnswer > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
				{
					result = -1005;
				}
				else if (now > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestion.QuestionAwardXml.ExamTime))
				{
					result = -1;
				}
				else
				{
					if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime < Global.GetRoleParamsDateTimeFromDB(client, "20001"))
					{
						int[] answerIndex = currentQuestion.QuestionAwardXml.AnswerIndex;
						if (answer == answerIndex[1] || answer == answerIndex[2])
						{
							result = -1007;
							client.sendCmd(nID, result.ToString(), false);
							return true;
						}
					}
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic[roleID] = answer;
					}
					Global.SaveRoleParamsDateTimeToDB(client, "20006", now, true);
				}
				client.sendCmd(nID, result.ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0005AB88 File Offset: 0x00058D88
		public bool ProcessGetPaiHangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string fileName = Global.GameResPath(AoyunDaTiConsts.QuestionAward);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0005ABF0 File Offset: 0x00058DF0
		public bool ProcessRecPaiHangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int roleID = Convert.ToInt32(cmdParams[0]);
				string goodsOne = "";
				int rank = -1;
				if (!AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(roleID, out rank))
				{
					rank = -1;
				}
				string data;
				if (rank < 0)
				{
					data = "-1:0:";
					client.sendCmd(nID, data, false);
					return true;
				}
				int key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey - 1;
				if (key < 0)
				{
					data = "-1:0:";
					client.sendCmd(nID, data, false);
					return true;
				}
				AoyunQuestionTimeItem lastQuestionTime = null;
				if (key < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					lastQuestionTime = AoYunDaTiManager.XmlQuestionTimeList[key];
					DateTime recTime = Global.GetRoleParamsDateTimeFromDB(client, "20005");
					DateTime endTime = lastQuestionTime.EndTime;
					if (endTime <= recTime)
					{
						data = "-1002:" + rank + ":";
						client.sendCmd(nID, data, false);
						return true;
					}
					DateTime lastAnswerTime = Global.GetRoleParamsDateTimeFromDB(client, "20006");
					DateTime lastBeginTime = lastQuestionTime.BeginTime;
					if (lastAnswerTime < lastBeginTime)
					{
						data = "-1:0:";
						client.sendCmd(nID, data, false);
						return true;
					}
				}
				int i;
				for (i = 0; i < AoYunDaTiManager.XmlPaiHangAward.Count - 1; i++)
				{
					if (rank >= AoYunDaTiManager.XmlPaiHangAward[i].BeginNum && rank <= AoYunDaTiManager.XmlPaiHangAward[i].EndNum)
					{
						goodsOne = AoYunDaTiManager.XmlPaiHangAward[i].GoodsOne;
						AoYunDaTiManager.GiveGoodsAward(client, goodsOne);
						Global.SaveRoleParamsDateTimeToDB(client, "20005", lastQuestionTime.EndTime, true);
						break;
					}
				}
				if (i == AoYunDaTiManager.XmlPaiHangAward.Count - 1)
				{
					goodsOne = AoYunDaTiManager.XmlPaiHangAward[i].GoodsOne;
					AoYunDaTiManager.GiveGoodsAward(client, goodsOne);
					Global.SaveRoleParamsDateTimeToDB(client, "20005", lastQuestionTime.EndTime, true);
				}
				data = string.Concat(new object[]
				{
					"1:",
					rank,
					":",
					goodsOne
				});
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0005AED0 File Offset: 0x000590D0
		public bool ProcessUseGoodsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int roleID = Convert.ToInt32(cmdParams[0]);
				int goodsType = Convert.ToInt32(cmdParams[1]);
				string goodsUseTime = (goodsType == 0) ? "20001" : "20003";
				DateTime useTime = Global.GetRoleParamsDateTimeFromDB(client, goodsUseTime);
				string goodsCountName = (goodsType == 0) ? "20002" : "20004";
				int goodsCount = Global.GetRoleParamsInt32FromDB(client, goodsCountName);
				AoyunQuestionBankItem currentQuestion = AoYunDaTiManager.GetCurrentQuestionBank();
				string data;
				if (now > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestion.QuestionAwardXml.ExamTime))
				{
					data = string.Concat(new object[]
					{
						"-1005:-1:-1:",
						goodsType,
						":",
						goodsCount
					});
					client.sendCmd(nID, data, false);
					return true;
				}
				if (AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.ContainsKey(roleID))
				{
					data = string.Concat(new object[]
					{
						"-1006:-1:-1:",
						goodsType,
						":",
						goodsCount
					});
					client.sendCmd(nID, data, false);
					return true;
				}
				if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime < useTime)
				{
					data = "-1001:";
					if (goodsType == 0)
					{
						int[] answerIndex = currentQuestion.QuestionAwardXml.AnswerIndex;
						object obj = data;
						data = string.Concat(new object[]
						{
							obj,
							answerIndex[1],
							":",
							answerIndex[2],
							":0:",
							goodsCount
						});
					}
					else
					{
						data = data + "-1:-1:1:" + goodsCount;
					}
					client.sendCmd(nID, data, false);
					return true;
				}
				if (goodsCount <= 0)
				{
					data = string.Concat(new object[]
					{
						"-1:-1:-1:",
						goodsType,
						":",
						goodsCount
					});
					client.sendCmd(nID, data, false);
					return true;
				}
				goodsCount--;
				if (goodsType == 0)
				{
					int[] answerIndex = currentQuestion.QuestionAwardXml.AnswerIndex;
					data = string.Concat(new object[]
					{
						"1:",
						answerIndex[1],
						":",
						answerIndex[2],
						":0:",
						goodsCount
					});
				}
				else
				{
					data = "1:-1:-1:1:" + goodsCount;
				}
				Global.SaveRoleParamsInt32ValueToDB(client, goodsCountName, goodsCount, true);
				Global.SaveRoleParamsDateTimeToDB(client, goodsUseTime, now, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具使用", goodsCountName, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, goodsCount, client.ServerId, null);
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0005B284 File Offset: 0x00059484
		public bool ProcessBuyGoodsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int roleID = Convert.ToInt32(cmdParams[0]);
				int goodsType = Convert.ToInt32(cmdParams[1]);
				int num = Convert.ToInt32(cmdParams[2]);
				string data;
				if (goodsType < 0 || goodsType > 1 || num < 1 || num > AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[goodsType])
				{
					data = "-1004::";
					client.sendCmd(nID, data, false);
					return true;
				}
				string goodsBuyCount = (goodsType == 0) ? "20002" : "20004";
				int goodsCount = Global.GetRoleParamsInt32FromDB(client, goodsBuyCount);
				if (num + goodsCount > AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[goodsType])
				{
					data = string.Concat(new object[]
					{
						"-1:",
						goodsType,
						":",
						goodsCount
					});
					client.sendCmd(nID, data, false);
					return true;
				}
				int zhuanShi = num * AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[goodsType];
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zhuanShi, "奥运答题道具购买", true, true, false, DaiBiSySType.None))
				{
					data = string.Concat(new object[]
					{
						"-1003:",
						goodsType,
						":",
						goodsCount
					});
					client.sendCmd(nID, data, false);
					return true;
				}
				num += goodsCount;
				Global.SaveRoleParamsInt32ValueToDB(client, goodsBuyCount, num, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具购买", goodsBuyCount, client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
				data = string.Concat(new object[]
				{
					"1:",
					goodsType,
					":",
					num
				});
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0005B520 File Offset: 0x00059720
		private void SetCurrentQuestionTimeItem(DateTime time)
		{
			for (int i = 0; i < AoYunDaTiManager.XmlQuestionTimeList.Count; i++)
			{
				DateTime endTime = AoYunDaTiManager.XmlQuestionTimeList[i].EndTime;
				if (!(time >= endTime))
				{
					lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
					{
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = i;
					}
					this.SetCurrentQuestionList(time);
					this.SetCurrentQuestionNum(time);
					return;
				}
			}
			lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
			{
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = AoYunDaTiManager.XmlQuestionTimeList.Count;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
			}
			LogManager.WriteLog(LogTypes.Error, string.Format("获取奥运答题时间配置出错，没有存在比当前时间还晚的活动", new object[0]), null, true);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0005B668 File Offset: 0x00059868
		private void SetCurrentQuestionList(DateTime time)
		{
			int questionBegin = 0;
			int questionEnd = 0;
			try
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				}
				AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTime != null)
				{
					int questionNum = currentQuestionTime.QuestionNum;
					questionBegin = currentQuestionTime.QuestionBegin;
					questionEnd = currentQuestionTime.QuestionEnd;
					if (AoYunDaTiManager.XmlQuestionBankDic.Count == 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("获取奥运答题题目列表出错，问题词典不存在", new object[0]), null, true);
					}
					else
					{
						List<int> questionBankKeyList = new List<int>();
						if (questionEnd - questionBegin + 1 < questionNum || AoYunDaTiManager.XmlQuestionBankDic.Count < questionNum)
						{
							for (int i = 0; i < questionNum; i++)
							{
								int qId = Global.GetRandomNumber(questionBegin, questionEnd);
								AoyunQuestionBankItem question;
								if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(qId, out question))
								{
									questionBankKeyList.Add(qId);
								}
								else
								{
									i--;
								}
							}
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList = questionBankKeyList;
							}
						}
						else
						{
							int qID = questionBegin;
							if (currentQuestionTime.RandomType == 1)
							{
								for (int i = 0; i < questionNum; i++)
								{
									if (AoYunDaTiManager.XmlQuestionBankDic.ContainsKey(qID))
									{
										questionBankKeyList.Add(qID);
									}
									else
									{
										i--;
									}
									qID++;
									if (qID > questionBegin + questionNum * 5)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("获取奥运答题题目列表出错", new object[0]), null, true);
										return;
									}
								}
							}
							else
							{
								List<int> qIDset = new List<int>();
								for (int i = qID; i <= questionEnd; i++)
								{
									if (AoYunDaTiManager.XmlQuestionBankDic.ContainsKey(i))
									{
										qIDset.Add(i);
									}
								}
								int len = qIDset.Count;
								for (int i = 0; i < questionNum; i++)
								{
									int sit = Global.GetRandomNumber(i, len);
									int tmp = qIDset[i];
									qIDset[i] = qIDset[sit];
									qIDset[sit] = tmp;
									questionBankKeyList.Add(qIDset[i]);
								}
							}
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList = questionBankKeyList;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("出题出错，开始题：{0} 结束题：{1}", questionBegin, questionEnd), null, true);
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0005B9E4 File Offset: 0x00059BE4
		private void SetCurrentQuestionNum(DateTime now)
		{
			AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
			if (currentQuestionTime == null)
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
				}
			}
			else
			{
				if (now > currentQuestionTime.EndTime)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("获取奥运答题题目信息出错，结束时间：{0} 当前时间：{1}", currentQuestionTime.EndTime, now), null, true);
				}
				if (now < currentQuestionTime.BeginTime)
				{
					lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
					{
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
					}
				}
				else
				{
					double dTime = (now - currentQuestionTime.BeginTime).TotalSeconds;
					for (int i = 0; i < AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count; i++)
					{
						int qId = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList[i];
						AoyunQuestionBankItem question;
						if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(qId, out question))
						{
							if (dTime <= (double)(question.QuestionAwardXml.ExamTime + question.QuestionAwardXml.WaitTime))
							{
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = i;
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = now;
								}
								return;
							}
							dTime -= (double)(question.QuestionAwardXml.ExamTime + question.QuestionAwardXml.WaitTime);
						}
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("获取奥运答题题目信息出错，问题答完了，开始时间：{0} 当前时间：{1}", currentQuestionTime.BeginTime, now), null, true);
				}
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0005BC44 File Offset: 0x00059E44
		private static void LoadQuestionTime()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(AoyunDaTiConsts.QuestionTime);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					AoYunDaTiManager.XmlQuestionTimeList.Clear();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							AoyunQuestionTimeItem questionTimeItem = new AoyunQuestionTimeItem();
							string beginTime = Global.GetDefAttributeStr(xmlItem, "Date", "") + " " + Global.GetDefAttributeStr(xmlItem, "BeginTime", "");
							questionTimeItem.BeginTime = DateTime.Parse(beginTime);
							string endTime = Global.GetDefAttributeStr(xmlItem, "Date", "") + " " + Global.GetDefAttributeStr(xmlItem, "EndTime", "");
							questionTimeItem.EndTime = DateTime.Parse(endTime);
							questionTimeItem.RandomType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RandomType", "0"));
							questionTimeItem.QuestionNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0"));
							questionTimeItem.QuestionBegin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "QuestionBegin", "0"));
							questionTimeItem.QuestionEnd = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "QuestionEnd", "0"));
							AoYunDaTiManager.XmlQuestionTimeList.Add(questionTimeItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0005BE78 File Offset: 0x0005A078
		private static void LoadQuestionBank()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(AoyunDaTiConsts.QuestionBank);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					AoYunDaTiManager.XmlQuestionBankDic.Clear();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							AoyunQuestionAwardXml questionAwardItem = new AoyunQuestionAwardXml();
							questionAwardItem.TrueAnswer = 0;
							questionAwardItem.JinBi = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "JinBi", "0"));
							questionAwardItem.Exp = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Exp", "0"));
							questionAwardItem.TianShiItem = Array.ConvertAll<string, double>(Global.GetDefAttributeStr(xmlItem, "TianShiItem", "").Split(new char[]
							{
								'|'
							}), (string s) => double.Parse(s));
							questionAwardItem.EMoItem = Array.ConvertAll<string, double>(Global.GetDefAttributeStr(xmlItem, "TianShiItem", "").Split(new char[]
							{
								'|'
							}), (string s) => double.Parse(s));
							questionAwardItem.ExamTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ExamTime", "0"));
							questionAwardItem.WaitTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WaitTime", "0"));
							questionAwardItem.WinScore = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WinScore", "0"));
							questionAwardItem.LoseScore = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LoseScore", "0"));
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							AoyunQuestionItemData questionItem = new AoyunQuestionItemData();
							questionItem.Question = Global.GetDefAttributeStr(xmlItem, "Questiong", "");
							string[] answerContent = new string[]
							{
								Global.GetDefAttributeStr(xmlItem, "Ture", ""),
								Global.GetDefAttributeStr(xmlItem, "False1", ""),
								Global.GetDefAttributeStr(xmlItem, "False2", ""),
								Global.GetDefAttributeStr(xmlItem, "False3", "")
							};
							int[] answerIndex = new int[]
							{
								0,
								1,
								2,
								3
							};
							questionItem.AnswerContentArray = new string[4];
							for (int i = 0; i < 4; i++)
							{
								int random = Global.GetRandomNumber(i, 4);
								int iTmp = answerIndex[i];
								answerIndex[i] = answerIndex[random];
								answerIndex[random] = iTmp;
							}
							questionAwardItem.AnswerIndex = answerIndex;
							questionAwardItem.TrueAnswer = questionAwardItem.AnswerIndex[0];
							questionItem.AnswerContentArray[questionAwardItem.AnswerIndex[0]] = answerContent[0];
							questionItem.AnswerContentArray[questionAwardItem.AnswerIndex[1]] = answerContent[1];
							questionItem.AnswerContentArray[questionAwardItem.AnswerIndex[2]] = answerContent[2];
							questionItem.AnswerContentArray[questionAwardItem.AnswerIndex[3]] = answerContent[3];
							questionItem.UseTianShi = false;
							questionItem.UseEMo = false;
							questionItem.EndTime = DateTime.MinValue;
							AoyunQuestionBankItem questionBankItem = new AoyunQuestionBankItem
							{
								QuestionItem = questionItem,
								QuestionAwardXml = questionAwardItem
							};
							AoYunDaTiManager.XmlQuestionBankDic.Add(key, questionBankItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0005C260 File Offset: 0x0005A460
		private static void LoadPaiHangAwad()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(AoyunDaTiConsts.QuestionAward);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					AoYunDaTiManager.XmlPaiHangAward.Clear();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							AoyunDaTiPaiHangAwardXml paiHangAward = new AoyunDaTiPaiHangAwardXml
							{
								Name = Global.GetDefAttributeStr(xmlItem, "Name", ""),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EngNum", "0")),
								GoodsOne = Global.GetDefAttributeStr(xmlItem, "GoodsOne", ""),
								MinScore = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinScore", "0"))
							};
							AoYunDaTiManager.XmlPaiHangAward.Add(paiHangAward);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0005C3F0 File Offset: 0x0005A5F0
		private static void LoadAoYunDaTi()
		{
			try
			{
				if (AoYunDaTiManager.XmlQuestionTimeList == null || AoYunDaTiManager.XmlQuestionTimeList.Count <= 0)
				{
					AoYunDaTiManager.XmlAoyunDaTiOpen = new AoyunDaTiOpenXml
					{
						HuoDongID = 1,
						BeginTime = DateTime.MaxValue,
						EndTime = DateTime.MinValue
					};
				}
				else
				{
					AoYunDaTiManager.XmlAoyunDaTiOpen = new AoyunDaTiOpenXml
					{
						HuoDongID = 1,
						BeginTime = AoYunDaTiManager.XmlQuestionTimeList[0].BeginTime.Date,
						EndTime = AoYunDaTiManager.XmlQuestionTimeList[AoYunDaTiManager.XmlQuestionTimeList.Count - 1].EndTime.Date.AddDays(1.0)
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("活动开启配置信息, 失败。", new object[0]), ex, true);
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0005C4E8 File Offset: 0x0005A6E8
		private static void UpdateQuestion(DateTime now)
		{
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
			}
			int num = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum;
			num++;
			if (num <= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count)
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = num;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = now;
				}
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0005C5C0 File Offset: 0x0005A7C0
		private static void SendQuestionToAll(DateTime now)
		{
			AoyunQuestionBankItem currentQuestion = AoYunDaTiManager.GetCurrentQuestionBank();
			if (currentQuestion != null)
			{
				int index = 0;
				GameClient client;
				while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
				{
					AoyunQuestionItemData data = AoYunDaTiManager.GetRoleQuestionFromDB(client, now);
					if (data != null)
					{
						client.sendCmd<AoyunQuestionItemData>(20201, data, false);
					}
				}
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0005C628 File Offset: 0x0005A828
		private static void SendAnswerOverToAll()
		{
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
			{
				string result = "0";
				int rank = 0;
				if (AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(client.ClientData.RoleID, out rank))
				{
					if (rank > 0)
					{
						result = "1";
					}
				}
				client.sendCmd(20207, result, false);
			}
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0005C6AC File Offset: 0x0005A8AC
		private static void SendAnswerToAll(DateTime now)
		{
			try
			{
				AoyunQuestionBankItem currentQuestion = AoYunDaTiManager.GetCurrentQuestionBank();
				if (currentQuestion != null)
				{
					AoyunQuestionAwardXml questionAwardXml = currentQuestion.QuestionAwardXml;
					AoyunQuestionTimeItem questionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
					if (questionTime != null)
					{
						int index = 0;
						GameClient client;
						while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
						{
							int rid = client.ClientData.RoleID;
							int answer = -1;
							int tianShiCount = Global.GetRoleParamsInt32FromDB(client, "20002");
							int eMoCount = Global.GetRoleParamsInt32FromDB(client, "20004");
							int rolePoint = 0;
							DateTime answerTime = Global.GetRoleParamsDateTimeFromDB(client, "20006");
							if (answerTime > questionTime.BeginTime)
							{
								rolePoint = Global.GetRoleParamsInt32FromDB(client, "20008");
							}
							if (AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.TryGetValue(rid, out answer))
							{
								AoyunQuestionAward data = new AoyunQuestionAward();
								int jinBi = questionAwardXml.JinBi;
								int exp = questionAwardXml.Exp;
								data.RightAnswer = questionAwardXml.TrueAnswer;
								int changeLife = client.ClientData.ChangeLifeCount;
								if (AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef.Length > changeLife)
								{
									exp *= AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef[changeLife];
								}
								int point;
								if (questionAwardXml.TrueAnswer != answer)
								{
									data.Result = -1;
									point = questionAwardXml.LoseScore;
									jinBi /= 2;
									exp /= 2;
									data.RightAnswer = questionAwardXml.TrueAnswer;
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.TianShiItem[1])
									{
										if (tianShiCount + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0])
										{
											tianShiCount++;
											Global.SaveRoleParamsInt32ValueToDB(client, "20002", tianShiCount, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(7, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20002", client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, tianShiCount, client.ServerId, null);
										}
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.EMoItem[1])
									{
										if (eMoCount + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1])
										{
											eMoCount++;
											Global.SaveRoleParamsInt32ValueToDB(client, "20004", eMoCount, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(8, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20004", client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, eMoCount, client.ServerId, null);
										}
									}
								}
								else
								{
									data.Result = 1;
									point = questionAwardXml.WinScore;
									if (answerTime > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
									{
										point -= (answerTime - AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime).Seconds;
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.TianShiItem[0])
									{
										if (tianShiCount + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0])
										{
											tianShiCount++;
											Global.SaveRoleParamsInt32ValueToDB(client, "20002", tianShiCount, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(7, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20002", client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, tianShiCount, client.ServerId, null);
										}
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.EMoItem[0])
									{
										if (eMoCount + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1])
										{
											eMoCount++;
											Global.SaveRoleParamsInt32ValueToDB(client, "20004", eMoCount, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(8, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20004", client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, eMoCount, client.ServerId, null);
										}
									}
									List<int> questionStateList = Global.GetRoleParamsIntListFromDB(client, "20007");
									questionStateList[AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum] = 1;
									Global.SaveRoleParamsIntListToDB(client, questionStateList, "20007", true);
								}
								DateTime eMoUserTime = Global.GetRoleParamsDateTimeFromDB(client, "20003");
								if (eMoUserTime > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
								{
									point *= 2;
								}
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, jinBi, "奥运答题添加", false);
								GameManager.ClientMgr.ProcessRoleExperience(client, (long)exp, false, true, false, "none");
								rolePoint += point;
								AoyunPaiHangRoleData roleData = null;
								if (!AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.TryGetValue(rid, out roleData))
								{
									roleData = new AoyunPaiHangRoleData
									{
										ZoneId = client.ClientData.ZoneID,
										RoleId = rid,
										RoleName = client.ClientData.RoleName,
										RoleCurrentPoint = 0,
										RoleRank = 0
									};
								}
								roleData.RolePoint = rolePoint;
								Global.SaveRoleParamsInt32ValueToDB(client, "20008", rolePoint, true);
								if (point > 0)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format(GLang.GetLang(666, new object[0]), point), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								data.TianShiCount = tianShiCount;
								data.EMoCount = eMoCount;
								data.RolePoint = rolePoint;
								client.sendCmd<AoyunQuestionAward>(20206, data, false);
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic[rid] = roleData;
								}
							}
							else
							{
								AoyunQuestionAward data = new AoyunQuestionAward
								{
									Result = -1,
									RightAnswer = questionAwardXml.TrueAnswer,
									TianShiCount = tianShiCount,
									EMoCount = eMoCount,
									RolePoint = rolePoint
								};
								Global.SaveRoleParamsInt32ValueToDB(client, "20008", rolePoint, true);
								client.sendCmd<AoyunQuestionAward>(20206, data, false);
							}
						}
						lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
						{
							AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Values.ToList<AoyunPaiHangRoleData>();
						}
						AoYunDaTiManager.UpdateRankList();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载发送问题答案给所有在线用户 失败:", new object[0]), ex, true);
			}
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0005CF14 File Offset: 0x0005B114
		private static void InitAoyunRank()
		{
			List<AoyunPaiHangRoleData> lastRank = AoYunDaTiManager.GetAoyunRoleListFromDB();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = lastRank;
			}
			AoYunDaTiManager.UpdateRankList();
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0005CF78 File Offset: 0x0005B178
		private static void InitLastAoyunRank()
		{
			Dictionary<int, int> aoyunRoleDic = AoYunDaTiManager.GetLastAoyunRoleDicFromDB();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.LastRankDic = aoyunRoleDic;
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0005CFD4 File Offset: 0x0005B1D4
		private void UpdateCurrentQuestionTimeItem(DateTime time)
		{
			try
			{
				for (int i = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1; i < AoYunDaTiManager.XmlQuestionTimeList.Count; i++)
				{
					DateTime endTime = AoYunDaTiManager.XmlQuestionTimeList[i].EndTime;
					if (!(time >= endTime))
					{
						lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
						{
							AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = i;
						}
						this.SetCurrentQuestionList(time);
						this.SetCurrentQuestionNum(time);
						return;
					}
				}
				LogManager.WriteLog(LogTypes.Info, string.Format("活动结束了", new object[0]), null, true);
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = AoYunDaTiManager.XmlQuestionTimeList.Count;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("奥运答题更新当前答题时间设置失败：", new object[0]), ex, true);
			}
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0005D178 File Offset: 0x0005B378
		private static AoyunQuestionTimeItem GetCurrentQuestionTimeItem()
		{
			int key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey;
			AoyunQuestionTimeItem result;
			if (key < 0 || key >= AoYunDaTiManager.XmlQuestionTimeList.Count)
			{
				result = null;
			}
			else
			{
				result = AoYunDaTiManager.XmlQuestionTimeList[key];
			}
			return result;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0005D1C0 File Offset: 0x0005B3C0
		private static AoyunQuestionBankItem GetCurrentQuestionBank()
		{
			int num = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum;
			AoyunQuestionBankItem result;
			if (num < 0 || num >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count)
			{
				result = null;
			}
			else
			{
				int qId = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList[num];
				AoyunQuestionBankItem question;
				if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(qId, out question))
				{
					result = question;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0005D230 File Offset: 0x0005B430
		private static AoyunQuestionItemData GetRoleQuestionFromDB(GameClient client, DateTime now)
		{
			AoyunQuestionItemData result;
			try
			{
				AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTime == null)
				{
					result = null;
				}
				else
				{
					AoyunQuestionBankItem currentQuestion = AoYunDaTiManager.GetCurrentQuestionBank();
					if (currentQuestion == null)
					{
						result = null;
					}
					else
					{
						AoyunQuestionItemData data = currentQuestion.QuestionItem;
						DateTime useTianShiTime = Global.GetRoleParamsDateTimeFromDB(client, "20001");
						data.UseTianShi = (useTianShiTime >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime);
						DateTime useEMoTime = Global.GetRoleParamsDateTimeFromDB(client, "20003");
						data.UseEMo = (useEMoTime >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime);
						data.QuestionId = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum + 1;
						data.EndTime = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestion.QuestionAwardXml.ExamTime);
						DateTime answerTime = Global.GetRoleParamsDateTimeFromDB(client, "20006");
						if (answerTime < currentQuestionTime.BeginTime)
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "20008", 0, true);
						}
						data.RolePoint = Global.GetRoleParamsInt32FromDB(client, "20008");
						if (!AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.TryGetValue(client.ClientData.RoleID, out data.RoleAnswer))
						{
							data.RoleAnswer = -1;
						}
						List<int> questionStateList = Global.GetRoleParamsIntListFromDB(client, "20007");
						if (questionStateList.Count != currentQuestionTime.QuestionNum)
						{
							questionStateList.Clear();
							for (int i = 0; i < currentQuestionTime.QuestionNum; i++)
							{
								questionStateList.Add(0);
							}
							Global.SaveRoleParamsIntListToDB(client, questionStateList, "20007", true);
						}
						else if (currentQuestionTime.BeginTime > Global.GetRoleParamsDateTimeFromDB(client, "20006"))
						{
							questionStateList.Clear();
							for (int i = 0; i < currentQuestionTime.QuestionNum; i++)
							{
								questionStateList.Add(0);
							}
							Global.SaveRoleParamsIntListToDB(client, questionStateList, "20007", true);
						}
						data.QuestionState = new List<bool>();
						foreach (int i in questionStateList)
						{
							int i;
							data.QuestionState.Add(i == 1);
						}
						result = data;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("奥运答题获取角色答题信息失败：", new object[0]), ex, true);
				result = null;
			}
			return result;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0005D4F8 File Offset: 0x0005B6F8
		private static void UpdateRankList()
		{
			try
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
				}
				List<AoyunPaiHangRoleData> rankList = AoYunDaTiManager.AoyunRunTimeData.AoyunRankList;
				if (rankList != null && rankList.Count >= 1)
				{
					rankList.Sort((AoyunPaiHangRoleData x, AoyunPaiHangRoleData y) => y.RolePoint - x.RolePoint);
					Dictionary<int, AoyunPaiHangRoleData> rankDic = new Dictionary<int, AoyunPaiHangRoleData>();
					int rank = 1;
					int awardIndex = 0;
					int len = rankList.Count<AoyunPaiHangRoleData>();
					for (int i = 0; i < len; i++)
					{
						AoyunPaiHangRoleData item = rankList[i];
						if (item.RolePoint > 0)
						{
							AoyunDaTiPaiHangAwardXml awardItem = AoYunDaTiManager.XmlPaiHangAward[awardIndex];
							while (awardItem.MinScore > item.RolePoint && awardIndex < AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								awardItem = AoYunDaTiManager.XmlPaiHangAward[++awardIndex];
								rank = awardItem.BeginNum;
							}
							if (awardItem.MinScore > item.RolePoint && awardIndex == AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								rankList[i].RoleRank = -1;
							}
							else
							{
								rankList[i].RoleRank = rank++;
							}
							rankDic.Add(rankList[i].RoleId, rankList[i]);
							if (rank == awardItem.EndNum + 1 && awardIndex < AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								awardItem = AoYunDaTiManager.XmlPaiHangAward[++awardIndex];
							}
						}
					}
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic = rankDic;
						AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = rankList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("奥运答题获取排行榜失败：", new object[0]), ex, true);
			}
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0005D798 File Offset: 0x0005B998
		private static List<AoyunPaiHangRoleData> GetAoyunRoleListFromDB()
		{
			List<AoyunPaiHangRoleData> list = Global.sendToDB<List<AoyunPaiHangRoleData>, int>(20300, 0, 0);
			if (null == list)
			{
				list = new List<AoyunPaiHangRoleData>();
			}
			return list;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0005D7CC File Offset: 0x0005B9CC
		private static Dictionary<int, int> GetLastAoyunRoleDicFromDB()
		{
			Dictionary<int, int> dic = Global.sendToDB<Dictionary<int, int>, int>(20301, 0, 0);
			if (null == dic)
			{
				dic = new Dictionary<int, int>();
			}
			return dic;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0005D800 File Offset: 0x0005BA00
		private static void GiveGoodsAward(GameClient client, string goods)
		{
			string[] goodList = goods.Split(new char[]
			{
				'|'
			});
			List<GoodsData> awardList = new List<GoodsData>();
			for (int i = 0; i < goodList.Length; i++)
			{
				if (!(goodList[i] == ""))
				{
					string[] goodItem = goodList[i].Split(new char[]
					{
						','
					});
					if (goodItem.Length == 7)
					{
						GoodsData goodsData = new GoodsData
						{
							Id = -1,
							GoodsID = Convert.ToInt32(goodItem[0]),
							Using = 0,
							Forge_level = Convert.ToInt32(goodItem[3]),
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							GCount = Convert.ToInt32(goodItem[1]),
							Binding = Convert.ToInt32(goodItem[2]),
							BagIndex = 0,
							Lucky = Convert.ToInt32(goodItem[5]),
							ExcellenceInfo = Convert.ToInt32(goodItem[6]),
							AppendPropLev = Convert.ToInt32(goodItem[4])
						};
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
						{
							string strinfo = string.Format("系统中不存在{0}", goodsData.GoodsID);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							awardList.Add(goodsData);
						}
					}
				}
			}
			if (!Global.CanAddGoodsNum(client, goodList.Length))
			{
				Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(10, new object[0]), GLang.GetLang(10, new object[0]), 0, 0, 0);
			}
			else
			{
				foreach (GoodsData item in awardList)
				{
					SystemXmlItem systemXmlGood = null;
					GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemXmlGood);
					string goodsName = systemXmlGood.GetStringValue("Title");
					LogManager.WriteLog(LogTypes.SQL, string.Format("奥运答题奖励{0} {1}", client.ClientData.RoleID, goodsName), null, true);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GCount, item.Quality, "", item.Forge_level, item.Binding, item.Site, "", true, 1, "奥运答题奖励", "1900-01-01 12:00:00", 0, 0, item.Lucky, 0, item.ExcellenceInfo, item.AppendPropLev, 0, null, null, 0, true);
				}
			}
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0005DAE4 File Offset: 0x0005BCE4
		private static bool CheckAoYunDaTiOpen(DateTime now)
		{
			bool result;
			if (AoYunDaTiManager.XmlAoyunDaTiOpen == null)
			{
				result = false;
			}
			else if (!AoYunDaTiManager.AoyunRunTimeData.GongNengOpen && now >= AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime && now <= AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime)
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.GongNengOpen = true;
				}
				GameManager.ClientMgr.NotifyAllActivityState(7, 1, AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.HuoDongID);
				result = true;
			}
			else if (AoYunDaTiManager.AoyunRunTimeData.GongNengOpen && now >= AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime)
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.GongNengOpen = false;
				}
				GameManager.ClientMgr.NotifyAllActivityState(7, 0, AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.HuoDongID);
				result = false;
			}
			else
			{
				result = AoYunDaTiManager.AoyunRunTimeData.GongNengOpen;
			}
			return result;
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0005DC90 File Offset: 0x0005BE90
		public void NotifyActivityState(GameClient client)
		{
			if (AoYunDaTiManager.AoyunRunTimeData.GongNengOpen)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					7,
					1,
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
					7,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			client._IconStateMgr.AddFlushIconState(18000, AoYunDaTiManager.AoyunRunTimeData.DaTiOpen);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0005DD78 File Offset: 0x0005BF78
		private static void CheckActivityIcon(bool show)
		{
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
			{
				client._IconStateMgr.AddFlushIconState(18000, show);
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0005DDC6 File Offset: 0x0005BFC6
		public void AddGrade(GameClient client, int grade)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "20008", grade, true);
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0005DDD8 File Offset: 0x0005BFD8
		public void AoyunDaTiTimer_Work()
		{
			DateTime now = TimeUtil.NowDateTime();
			if (AoYunDaTiManager.CheckAoYunDaTiOpen(now))
			{
				AoyunQuestionTimeItem currentQuestionTime = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTime != null)
				{
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						if (!AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
						{
							if (now > currentQuestionTime.BeginTime)
							{
								AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Clear();
								AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
								AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
								Global.sendToDB<int, int>(20303, 0, 0);
								AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = true;
								AoYunDaTiManager.CheckActivityIcon(true);
								AoYunDaTiManager.UpdateQuestion(now);
								AoYunDaTiManager.SendQuestionToAll(now);
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.SendAnswer = true;
								}
							}
							return;
						}
					}
					AoyunQuestionBankItem question = AoYunDaTiManager.GetCurrentQuestionBank();
					if (question != null)
					{
						double dTime = (now - AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime).TotalSeconds;
						if (dTime > (double)(question.QuestionAwardXml.ExamTime + question.QuestionAwardXml.WaitTime))
						{
							if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum == AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count - 1)
							{
								EventLogManager.AddGameEvent(LogRecordType.AoYunDaTi, new object[]
								{
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1,
									currentQuestionTime.BeginTime,
									currentQuestionTime.EndTime,
									AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Count
								});
								Dictionary<int, int> lastRankDic = new Dictionary<int, int>();
								foreach (AoyunPaiHangRoleData item in AoYunDaTiManager.AoyunRunTimeData.AoyunRankList)
								{
									lastRankDic[item.RoleId] = item.RoleRank;
								}
								Global.sendToDB<int, Dictionary<int, int>>(20302, lastRankDic, 0);
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.LastRankDic = lastRankDic;
								}
								AoYunDaTiManager.SendAnswerOverToAll();
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = false;
								}
								AoYunDaTiManager.CheckActivityIcon(false);
								this.UpdateCurrentQuestionTimeItem(now);
							}
							else
							{
								AoYunDaTiManager.UpdateQuestion(now);
								AoYunDaTiManager.SendQuestionToAll(now);
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.SendAnswer = true;
								}
							}
						}
						else if (AoYunDaTiManager.QuestionDataTimer.SendAnswer && dTime > (double)question.QuestionAwardXml.ExamTime)
						{
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.SendAnswer = false;
							}
							AoYunDaTiManager.SendAnswerToAll(now);
						}
					}
				}
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0005E208 File Offset: 0x0005C408
		private void Destory_Work()
		{
			lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
			{
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
			}
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Clear();
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
				AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
			}
		}

		// Token: 0x04000B88 RID: 2952
		public static AoyunData AoyunRunTimeData = new AoyunData();

		// Token: 0x04000B89 RID: 2953
		private static QuestionData QuestionDataTimer = new QuestionData();

		// Token: 0x04000B8A RID: 2954
		private static AoYunDaTiManager instance = new AoYunDaTiManager();

		// Token: 0x04000B8B RID: 2955
		private static List<AoyunQuestionTimeItem> XmlQuestionTimeList = new List<AoyunQuestionTimeItem>();

		// Token: 0x04000B8C RID: 2956
		private static Dictionary<int, AoyunQuestionBankItem> XmlQuestionBankDic = new Dictionary<int, AoyunQuestionBankItem>();

		// Token: 0x04000B8D RID: 2957
		private static List<AoyunDaTiPaiHangAwardXml> XmlPaiHangAward = new List<AoyunDaTiPaiHangAwardXml>();

		// Token: 0x04000B8E RID: 2958
		private static AoyunDaTiOpenXml XmlAoyunDaTiOpen = null;
	}
}
