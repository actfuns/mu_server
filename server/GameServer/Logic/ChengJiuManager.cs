using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020005E1 RID: 1505
	public class ChengJiuManager : IManager
	{
		// Token: 0x06001C1D RID: 7197 RVA: 0x001A6C6C File Offset: 0x001A4E6C
		public static ChengJiuManager GetInstance()
		{
			return ChengJiuManager.Instance;
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x001A6C84 File Offset: 0x001A4E84
		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(670, 2, UpGradeChengLevelCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU));
			return true;
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x001A6CB4 File Offset: 0x001A4EB4
		public bool startup()
		{
			return true;
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x001A6CC8 File Offset: 0x001A4EC8
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x001A6CDC File Offset: 0x001A4EDC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x001A6CF0 File Offset: 0x001A4EF0
		public static void InitChengJiuConfig()
		{
			foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.systemChengJiu.SystemXmlItemDict)
			{
				switch (kv.Value.GetIntValue("ID", -1))
				{
				case 7:
				{
					int chengJiuID = kv.Value.GetIntValue("ChengJiuID", -1);
					if (chengJiuID > 2050)
					{
						if (chengJiuID > ChengJiuTypes.JunXianChengJiuEnd)
						{
							ChengJiuTypes.JunXianChengJiuEnd = kv.Key;
						}
					}
					break;
				}
				case 8:
				{
					int chengJiuID = kv.Value.GetIntValue("ChengJiuID", -1);
					if (chengJiuID > ChengJiuTypes.MainLineTaskEnd)
					{
						ChengJiuTypes.MainLineTaskEnd = kv.Key;
					}
					else if (chengJiuID < ChengJiuTypes.MainLineTaskStart)
					{
						ChengJiuTypes.MainLineTaskStart = kv.Key;
					}
					break;
				}
				}
			}
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x001A6E1C File Offset: 0x001A501C
		public static void SetAchievementLevel(GameClient client, int level)
		{
			Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
			{
				(double)level - 1.0
			}, 0, true);
			client.ClientData.ChengJiuLevel = level;
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就等级", "GM", "系统", client.ClientData.RoleName, "修改", level, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
			ChengJiuManager.SetChengJiuLevel(client, client.ClientData.ChengJiuLevel, true);
			Global.BroadcastClientChuanQiChengJiu(client, level);
			GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiuLevel, client.ClientData.ChengJiuLevel);
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x001A6ED4 File Offset: 0x001A50D4
		public static void SetAchievementRuneLevel(GameClient client, int level)
		{
			AchievementRuneData achievementRuneData = new AchievementRuneData();
			AchievementRuneBasicData basic = ChengJiuManager.GetAchievementRuneBasicDataByID(level);
			achievementRuneData.RoleID = client.ClientData.RoleID;
			achievementRuneData.RuneID = basic.RuneID;
			if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
			{
				achievementRuneData.UpResultType = 3;
			}
			ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
			client.ClientData.achievementRuneData = achievementRuneData;
			ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x001A6F49 File Offset: 0x001A5149
		public static void SetAchievementRuneCount(GameClient client, int count)
		{
			ChengJiuManager.ModifyAchievementRuneUpCount(client, count, true);
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x001A6F55 File Offset: 0x001A5155
		public static void SetAchievementRuneRate(GameClient client, int rate)
		{
			ChengJiuManager._runeRate = rate;
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x001A6F5E File Offset: 0x001A515E
		public static void initAchievementRune()
		{
			ChengJiuManager.LoadAchievementRuneBasicData();
			ChengJiuManager.LoadAchievementRuneSpecialData();
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x001A6F70 File Offset: 0x001A5170
		public static void initSetAchievementRuneProps(GameClient client)
		{
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.AchievementRune, false))
			{
				AchievementRuneData achievementRuneData = ChengJiuManager.GetAchievementRuneData(client);
				ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
			}
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x001A6F9C File Offset: 0x001A519C
		public static void LoadAchievementRuneBasicData()
		{
			string fileName = "Config/ChengJiuFuWen.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/ChengJiuFuWen.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ChengJiuManager._achievementRuneBasicList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							AchievementRuneBasicData config = new AchievementRuneBasicData();
							config.RuneID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.RuneName = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "Name", ""));
							config.LifeMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LifeV", "0"));
							config.AttackMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddAttack", "0"));
							config.DefenseMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddDefense", "0"));
							config.DodgeMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Dodge", "0"));
							config.AchievementCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostChengJiu", "0"));
							string addString = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "QiangHua", ""));
							if (addString.Length > 0)
							{
								config.RateList = new List<int>();
								config.AddNumList = new List<int[]>();
								string[] addArr = addString.Split(new char[]
								{
									'|'
								});
								foreach (string str in addArr)
								{
									string[] oneArr = str.Split(new char[]
									{
										','
									});
									float rate = float.Parse(oneArr[0]);
									config.RateList.Add((int)(rate * 100f));
									List<int> numList = new List<int>();
									for (int i = 1; i < oneArr.Length; i++)
									{
										numList.Add(int.Parse(oneArr[i]));
									}
									config.AddNumList.Add(numList.ToArray());
								}
							}
							ChengJiuManager._achievementRuneBasicList.Add(config.RuneID, config);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/ChengJiuFuWen.xml时文件出现异常!!!", ex, true);
				}
			}
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x001A7290 File Offset: 0x001A5490
		public static void LoadAchievementRuneSpecialData()
		{
			string fileName = "Config/ChengJiuSpecialAttribute.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/ChengJiuSpecialAttribute.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ChengJiuManager._achievementRuneSpecialList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							AchievementRuneSpecialData config = new AchievementRuneSpecialData();
							config.SpecialID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.RuneID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedFuWen", "0"));
							config.ZhuoYue = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "ZhuoYueYiJi", "0"));
							config.DiKang = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DiKangZhuoYueYiJi", "0"));
							ChengJiuManager._achievementRuneSpecialList.Add(config.RuneID, config);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/ChengJiuSpecialAttribute.xml时出现异常!!!", ex, true);
				}
			}
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x001A740C File Offset: 0x001A560C
		public static AchievementRuneBasicData GetAchievementRuneBasicDataByID(int id)
		{
			AchievementRuneBasicData result;
			if (ChengJiuManager._achievementRuneBasicList.ContainsKey(id))
			{
				result = ChengJiuManager._achievementRuneBasicList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x001A7440 File Offset: 0x001A5640
		public static AchievementRuneSpecialData GetAchievementRuneSpecialDataByID(int id)
		{
			AchievementRuneSpecialData result;
			if (ChengJiuManager._achievementRuneSpecialList.ContainsKey(id))
			{
				result = ChengJiuManager._achievementRuneSpecialList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x001A7474 File Offset: 0x001A5674
		public static int GetAchievementRuneUpCount(GameClient client)
		{
			int count = 0;
			int dayOld = 0;
			List<int> data = Global.GetRoleParamsIntListFromDB(client, "AchievementRuneUpCount");
			if (data != null && data.Count > 0)
			{
				dayOld = data[0];
			}
			int day = TimeUtil.NowDateTime().DayOfYear;
			if (dayOld == day)
			{
				count = data[1];
			}
			else
			{
				ChengJiuManager.ModifyAchievementRuneUpCount(client, count, true);
			}
			return count;
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x001A74EC File Offset: 0x001A56EC
		public static void ModifyAchievementRuneUpCount(GameClient client, int count, bool writeToDB = false)
		{
			List<int> dataList = new List<int>();
			dataList.AddRange(new int[]
			{
				TimeUtil.NowDateTime().DayOfYear,
				count
			});
			Global.SaveRoleParamsIntListToDB(client, dataList, "AchievementRuneUpCount", writeToDB);
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x001A7534 File Offset: 0x001A5734
		public static int GetAchievementRuneDiamond(GameClient client, int upCount)
		{
			int[] diamondList = GameManager.systemParamsList.GetParamValueIntArrayByName("ChengJiuFuWenZuanShi", ',');
			if (upCount >= diamondList.Length)
			{
				upCount = diamondList.Length - 1;
			}
			return diamondList[upCount];
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x001A756C File Offset: 0x001A576C
		public static AchievementRuneData GetAchievementRuneData(GameClient client)
		{
			AchievementRuneData result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.AchievementRune, false))
			{
				result = null;
			}
			else
			{
				AchievementRuneData achievementRuneData = client.ClientData.achievementRuneData;
				if (achievementRuneData == null)
				{
					achievementRuneData = new AchievementRuneData();
					List<int> data = Global.GetRoleParamsIntListFromDB(client, "AchievementRune");
					AchievementRuneBasicData basic;
					if (data == null || data.Count <= 0)
					{
						basic = ChengJiuManager.GetAchievementRuneBasicDataByID(1);
						achievementRuneData.RoleID = client.ClientData.RoleID;
						achievementRuneData.RuneID = basic.RuneID;
						ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
					}
					else
					{
						achievementRuneData.RoleID = client.ClientData.RoleID;
						achievementRuneData.RuneID = data[0];
						achievementRuneData.LifeAdd = data[1];
						achievementRuneData.AttackAdd = data[2];
						achievementRuneData.DefenseAdd = data[3];
						achievementRuneData.DodgeAdd = data[4];
						if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
						{
							achievementRuneData.UpResultType = 3;
							basic = ChengJiuManager.GetAchievementRuneBasicDataByID(ChengJiuManager._achievementRuneBasicList.Count);
						}
						else
						{
							basic = ChengJiuManager.GetAchievementRuneBasicDataByID(achievementRuneData.RuneID);
						}
					}
					achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, ChengJiuManager.GetAchievementRuneUpCount(client));
					achievementRuneData.Achievement = basic.AchievementCost;
					client.ClientData.achievementRuneData = achievementRuneData;
				}
				achievementRuneData.AchievementLeft = client.ClientData.ChengJiuPoints;
				if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
				{
					achievementRuneData.UpResultType = 3;
				}
				result = achievementRuneData;
			}
			return result;
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x001A7708 File Offset: 0x001A5908
		public static void ModifyAchievementRuneData(GameClient client, AchievementRuneData data, bool writeToDB = false)
		{
			List<int> dataList = new List<int>();
			dataList.AddRange(new int[]
			{
				data.RuneID,
				data.LifeAdd,
				data.AttackAdd,
				data.DefenseAdd,
				data.DodgeAdd
			});
			Global.SaveRoleParamsIntListToDB(client, dataList, "AchievementRune", writeToDB);
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x001A7768 File Offset: 0x001A5968
		public static AchievementRuneData UpAchievementRune(GameClient client, int runeID)
		{
			AchievementRuneData achievementRuneData = client.ClientData.achievementRuneData;
			AchievementRuneData result;
			if (achievementRuneData != null && achievementRuneData.UpResultType == 3)
			{
				achievementRuneData.UpResultType = -4;
				result = achievementRuneData;
			}
			else if (achievementRuneData == null || achievementRuneData.RuneID != runeID)
			{
				achievementRuneData.UpResultType = 0;
				result = achievementRuneData;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.AchievementRune, false))
			{
				achievementRuneData.UpResultType = -1;
				result = achievementRuneData;
			}
			else
			{
				int[] addNums = null;
				AchievementRuneBasicData basicRune = ChengJiuManager.GetAchievementRuneBasicDataByID(runeID);
				int achievementNow = client.ClientData.ChengJiuPoints;
				if (basicRune.AchievementCost > achievementNow)
				{
					achievementRuneData.UpResultType = -2;
					result = achievementRuneData;
				}
				else
				{
					string strCostList = "";
					int oldUserMoney = client.ClientData.UserMoney;
					int oldChengJiu = GameManager.ClientMgr.GetChengJiuPointsValue(client);
					int upCount = ChengJiuManager.GetAchievementRuneUpCount(client);
					int diamondNeed = ChengJiuManager.GetAchievementRuneDiamond(client, upCount);
					if (diamondNeed > 0 && !GameManager.ClientMgr.SubUserMoney(client, diamondNeed, "成就符文提升", true, true, true, true, DaiBiSySType.ChengJieFuWen))
					{
						achievementRuneData.UpResultType = -3;
						result = achievementRuneData;
					}
					else
					{
						strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-diamondNeed,
							oldUserMoney,
							client.ClientData.UserMoney
						});
						try
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -basicRune.AchievementCost, "成就符文提升", false, true);
						}
						catch (Exception)
						{
							achievementRuneData.UpResultType = -2;
							return achievementRuneData;
						}
						strCostList += EventLogManager.AddResPropString(ResLogType.ChengJiu, new object[]
						{
							-basicRune.AchievementCost,
							oldChengJiu,
							GameManager.ClientMgr.GetChengJiuPointsValue(client)
						});
						int rate = 0;
						int r = Global.GetRandomNumber(0, 100);
						for (int i = 0; i < basicRune.RateList.Count; i++)
						{
							rate += basicRune.RateList[i];
							if (r <= rate)
							{
								addNums = basicRune.AddNumList[i];
								achievementRuneData.BurstType = i;
								break;
							}
						}
						achievementRuneData.LifeAdd += addNums[0] * ChengJiuManager._runeRate;
						achievementRuneData.LifeAdd = ((achievementRuneData.LifeAdd > basicRune.LifeMax) ? basicRune.LifeMax : achievementRuneData.LifeAdd);
						achievementRuneData.AttackAdd += addNums[1] * ChengJiuManager._runeRate;
						achievementRuneData.AttackAdd = ((achievementRuneData.AttackAdd > basicRune.AttackMax) ? basicRune.AttackMax : achievementRuneData.AttackAdd);
						achievementRuneData.DefenseAdd += addNums[2] * ChengJiuManager._runeRate;
						achievementRuneData.DefenseAdd = ((achievementRuneData.DefenseAdd > basicRune.DefenseMax) ? basicRune.DefenseMax : achievementRuneData.DefenseAdd);
						achievementRuneData.DodgeAdd += addNums[3] * ChengJiuManager._runeRate;
						achievementRuneData.DodgeAdd = ((achievementRuneData.DodgeAdd > basicRune.DodgeMax) ? basicRune.DodgeMax : achievementRuneData.DodgeAdd);
						if (achievementRuneData.LifeAdd < basicRune.LifeMax || achievementRuneData.DefenseAdd < basicRune.DefenseMax || achievementRuneData.AttackAdd < basicRune.AttackMax || achievementRuneData.DodgeAdd < basicRune.DodgeMax)
						{
							achievementRuneData.UpResultType = 1;
							achievementRuneData.Achievement = basicRune.AchievementCost;
							achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, upCount + 1);
						}
						else
						{
							achievementRuneData.RuneID++;
							achievementRuneData.LifeAdd = 0;
							achievementRuneData.AttackAdd = 0;
							achievementRuneData.DefenseAdd = 0;
							achievementRuneData.DodgeAdd = 0;
							achievementRuneData.UpResultType = 2;
							if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
							{
								achievementRuneData.UpResultType = 3;
								achievementRuneData.Achievement = 0;
								achievementRuneData.Diamond = 0;
							}
							else
							{
								basicRune = ChengJiuManager.GetAchievementRuneBasicDataByID(achievementRuneData.RuneID);
								achievementRuneData.Achievement = basicRune.AchievementCost;
								achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, upCount + 1);
							}
						}
						ChengJiuManager.ModifyAchievementRuneUpCount(client, upCount + 1, true);
						ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
						client.ClientData.achievementRuneData = achievementRuneData;
						ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
						EventLogManager.AddAchievementRuneEvent(client, achievementRuneData.RuneID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							achievementRuneData.LifeAdd,
							achievementRuneData.AttackAdd,
							achievementRuneData.DefenseAdd,
							achievementRuneData.DodgeAdd
						}), strCostList);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						achievementRuneData.AchievementLeft = client.ClientData.ChengJiuPoints;
						result = achievementRuneData;
					}
				}
			}
			return result;
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x001A7C8C File Offset: 0x001A5E8C
		public static void SetAchievementRuneProps(GameClient client, AchievementRuneData achievementRuneData)
		{
			int life = achievementRuneData.LifeAdd;
			int attack = achievementRuneData.AttackAdd;
			int defense = achievementRuneData.DefenseAdd;
			int dodge = achievementRuneData.DodgeAdd;
			foreach (AchievementRuneBasicData d in ChengJiuManager._achievementRuneBasicList.Values)
			{
				if (d.RuneID < achievementRuneData.RuneID)
				{
					life += d.LifeMax;
					attack += d.AttackMax;
					defense += d.DefenseMax;
					dodge += d.DodgeMax;
				}
			}
			double zhuoYue = 0.0;
			double diKang = 0.0;
			if (achievementRuneData.RuneID > 1)
			{
				AchievementRuneSpecialData d2 = ChengJiuManager.GetAchievementRuneSpecialDataByID(achievementRuneData.RuneID - 1);
				zhuoYue += d2.ZhuoYue;
				diKang += d2.DiKang;
			}
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				13,
				life
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				45,
				attack
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				46,
				defense
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				19,
				dodge
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				35,
				zhuoYue
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				52,
				diKang
			});
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x001A7EF8 File Offset: 0x001A60F8
		public static void InitRoleChengJiuData(GameClient client)
		{
			client.ClientData.ContinuousDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin);
			client.ClientData.TotalDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
			client.ClientData.ChengJiuPoints = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ChengJiuPoints);
			client.ClientData.TotalKilledMonsterNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledMonsterNum);
			client.ClientData.ChengJiuLevel = ChengJiuManager.GetChengJiuLevel(client);
			if (client.ClientData.ChengJiuLevel > 0)
			{
				int nNewBufferGoodsIndexID = client.ClientData.ChengJiuLevel;
				Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
				{
					(double)nNewBufferGoodsIndexID - 1.0
				}, 0, true);
			}
			client._IconStateMgr.CheckChengJiuUpLevelState(client);
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x001A7FB6 File Offset: 0x001A61B6
		public static void SaveRoleChengJiuData(GameClient client)
		{
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x001A7FBC File Offset: 0x001A61BC
		public static void InitFlagIndex()
		{
			ChengJiuManager._DictFlagIndex.Clear();
			int index = 0;
			for (int i = 100; i <= 108; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 200; i <= 204; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 300; i <= 304; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 350; i <= 356; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 400; i <= 405; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 500; i <= 508; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 600; i <= 608; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 700; i <= 708; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 800; i <= 803; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 900; i <= 905; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 1000; i <= 1005; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 1100; i <= 1105; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 1200; i <= 1210; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 1300; i <= 1308; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 1400; i <= 1411; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 2000; i <= 2004; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = 2050; i <= ChengJiuTypes.JunXianChengJiuEnd; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
			for (int i = ChengJiuTypes.MainLineTaskStart; i <= ChengJiuTypes.MainLineTaskEnd; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, index);
				index += 2;
			}
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x001A8310 File Offset: 0x001A6510
		protected static ushort GetChengJiuIDByIndex(int index)
		{
			for (int i = 0; i < ChengJiuManager._DictFlagIndex.Count; i++)
			{
				if (ChengJiuManager._DictFlagIndex.ElementAt(i).Value == index)
				{
					return (ushort)ChengJiuManager._DictFlagIndex.ElementAt(i).Key;
				}
			}
			return 0;
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x001A8374 File Offset: 0x001A6574
		protected static int GetCompletedFlagIndex(int chengJiuID)
		{
			int index = -1;
			int result;
			if (ChengJiuManager._DictFlagIndex.TryGetValue(chengJiuID, out index))
			{
				result = index;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x001A83A4 File Offset: 0x001A65A4
		protected static int GetAwardFlagIndex(int chengJiuID)
		{
			int index = -1;
			int result;
			if (ChengJiuManager._DictFlagIndex.TryGetValue(chengJiuID, out index))
			{
				result = index + 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x001A83D4 File Offset: 0x001A65D4
		public static void AddChengJiuPoints(GameClient client, string strFrom, int modifyValue = 1, bool forceUpdateBuffer = true, bool writeToDB = false)
		{
			GameManager.ClientMgr.ModifyChengJiuPointsValue(client, modifyValue, strFrom, writeToDB, true);
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x001A83E8 File Offset: 0x001A65E8
		public static void SaveKilledMonsterNumToDB(GameClient client, bool bWriteDB = false)
		{
			ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.TotalKilledMonsterNum, ChengJiuExtraDataField.TotalKilledMonsterNum, bWriteDB);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x001A8400 File Offset: 0x001A6600
		public static uint GetChengJiuExtraDataByField(GameClient client, ChengJiuExtraDataField field)
		{
			List<uint> lsUint = Global.GetRoleParamsUIntListFromDB(client, "ChengJiuData");
			uint result;
			if (field >= (ChengJiuExtraDataField)lsUint.Count)
			{
				result = 0U;
			}
			else
			{
				result = lsUint[(int)field];
			}
			return result;
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x001A843C File Offset: 0x001A663C
		public static void ModifyChengJiuExtraData(GameClient client, uint value, ChengJiuExtraDataField field, bool writeToDB = false)
		{
			List<uint> lsUint = Global.GetRoleParamsUIntListFromDB(client, "ChengJiuData");
			while (lsUint.Count < (int)(field + 1))
			{
				lsUint.Add(0U);
			}
			lsUint[(int)field] = value;
			Global.SaveRoleParamsUintListToDB(client, lsUint, "ChengJiuData", writeToDB);
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x001A848C File Offset: 0x001A668C
		public static int GetChengJiuLevel(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "ChengJiuLevel");
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x001A84AC File Offset: 0x001A66AC
		public static void SetChengJiuLevel(GameClient client, int value, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "ChengJiuLevel", value, writeToDB);
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriAchievement) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
			GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ChengJiuLevel));
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x001A853C File Offset: 0x001A673C
		public int upGradeChengJiuBuffer(GameClient player)
		{
			return 1;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x001A8550 File Offset: 0x001A6750
		public static bool CanActiveNextChengHao(GameClient client)
		{
			return GameManager.ClientMgr.GetChengJiuPointsValue(client) >= ChengJiuManager.GetUpLevelNeedChengJiuPoint(client);
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x001A8578 File Offset: 0x001A6778
		public static int TryToActiveNewChengJiuBuffer(GameClient client, bool notifyPropsChanged, int nChengJiuLevel = -1)
		{
			double dayXiaoHao = 0.0;
			int nMaxBufferGoodsIndexID = ChengJiuManager.GetNewChengJiuBufferGoodsIndexIDAndDayXiaoHao(client, client.ClientData.ChengJiuPoints, out dayXiaoHao);
			if (-1 != nChengJiuLevel)
			{
				if (client.ClientData.ChengJiuLevel + 1 < nChengJiuLevel)
				{
					return -2;
				}
			}
			int needChengJiuPoint = ChengJiuManager.GetUpLevelNeedChengJiuPoint(client);
			int result;
			if (GameManager.ClientMgr.GetChengJiuPointsValue(client) < needChengJiuPoint)
			{
				result = -5;
			}
			else
			{
				int nNewBufferGoodsIndexID = client.ClientData.ChengJiuLevel + 1;
				if (nNewBufferGoodsIndexID > nMaxBufferGoodsIndexID)
				{
					result = -1;
				}
				else
				{
					string needGoods = GameManager.systemChengJiuBuffer.SystemXmlItemDict[nNewBufferGoodsIndexID].GetStringValue("NeedGoods");
					List<List<int>> GoodsCost = ConfigHelper.ParserIntArrayList(needGoods, true, '|', ',');
					for (int i = 0; i < GoodsCost.Count; i++)
					{
						int goodsId = GoodsCost[i][0];
						int costCount = GoodsCost[i][1];
						int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, goodsId);
						if (haveGoodsCnt < costCount)
						{
							return -6;
						}
					}
					int nOldBufferGoodsIndexID = -1;
					BufferData bufferData = Global.GetBufferDataByID(client, 31);
					if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
					{
						nOldBufferGoodsIndexID = (int)bufferData.BufferVal;
					}
					if (nOldBufferGoodsIndexID == nNewBufferGoodsIndexID && 0 != client.ClientData.ChengJiuLevel)
					{
						result = -3;
					}
					else
					{
						if (nOldBufferGoodsIndexID >= 0)
						{
							if (nNewBufferGoodsIndexID < nOldBufferGoodsIndexID)
							{
								return -4;
							}
						}
						if (nNewBufferGoodsIndexID >= 0)
						{
							Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
							{
								(double)nNewBufferGoodsIndexID - 1.0
							}, 0, true);
							bool bUsedBinding_just_placeholder = false;
							bool bUsedTimeLimited_just_placeholder = false;
							for (int i = 0; i < GoodsCost.Count; i++)
							{
								int goodsId = GoodsCost[i][0];
								int costCount = GoodsCost[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsId, costCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("提升成就等级时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", goodsId, costCount), null, true);
								}
								GoodsData goodsData = new GoodsData();
								goodsData.GoodsID = goodsId;
								goodsData.GCount = costCount;
							}
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -needChengJiuPoint, "提升成就等级", false, true);
							GameManager.ClientMgr.SetChengJiuLevelValue(client, 1, "提升成就等级", true, true);
							if (client.ClientData.ChengJiuLevel >= 4)
							{
								Global.BroadcastClientChuanQiChengJiu(client, nNewBufferGoodsIndexID);
							}
						}
						if (notifyPropsChanged)
						{
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
						if (client._IconStateMgr.CheckChengJiuUpLevelState(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x001A88E4 File Offset: 0x001A6AE4
		public static int GetNewChengJiuBufferGoodsIndexIDAndDayXiaoHao(GameClient client, int chengJiuPoints, out double dayXiaoHao)
		{
			int nNewBufferGoodsIndexID = -1;
			dayXiaoHao = 0.0;
			for (int i = 0; i < GameManager.systemChengJiuBuffer.SystemXmlItemDict.Count; i++)
			{
				SystemXmlItem item = GameManager.systemChengJiuBuffer.SystemXmlItemDict.ElementAt(i).Value;
				int chengJiu = item.GetIntValue("ChengJiu", -1);
				if (chengJiuPoints >= chengJiu)
				{
					nNewBufferGoodsIndexID = item.GetIntValue("ID", -1);
					dayXiaoHao = item.GetDoubleValue("DayXiaoHao");
				}
			}
			if (nNewBufferGoodsIndexID < 0)
			{
				nNewBufferGoodsIndexID = -1;
			}
			return nNewBufferGoodsIndexID;
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x001A8988 File Offset: 0x001A6B88
		public static int GetUpLevelNeedChengJiuPoint(GameClient client)
		{
			SystemXmlItem item;
			int result;
			if (GameManager.systemChengJiuBuffer.SystemXmlItemDict.TryGetValue(client.ClientData.ChengJiuLevel + 1, out item))
			{
				result = item.GetIntValue("ChengJiu", -1);
			}
			else
			{
				result = int.MaxValue;
			}
			return result;
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x001A89D4 File Offset: 0x001A6BD4
		public static bool IsChengJiuCompleted(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, false);
		}

		// Token: 0x06001C46 RID: 7238 RVA: 0x001A89F0 File Offset: 0x001A6BF0
		public static bool IsChengJiuAwardFetched(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, true);
		}

		// Token: 0x06001C47 RID: 7239 RVA: 0x001A8A0C File Offset: 0x001A6C0C
		public static void OnChengJiuCompleted(GameClient client, int chengJiuID)
		{
			ChengJiuManager.UpdateChengJiuFlag(client, chengJiuID, false);
			ChengJiuManager.GiveChengJiuAward(client, chengJiuID, "完成成就ID：" + chengJiuID);
			ChengJiuManager.NotifyClientChengJiuData(client, chengJiuID);
			GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteChengJiu));
			ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, -1, -1, TaskTypes.ChengJiuUpdate, null, chengJiuID, -1L, null);
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x001A8A80 File Offset: 0x001A6C80
		public static void NotifyClientChengJiuData(GameClient client, int justCompletedChengJiu = -1)
		{
			ChengJiuData chengJiuData = new ChengJiuData
			{
				RoleID = client.ClientData.RoleID,
				ChengJiuPoints = (long)client.ClientData.ChengJiuPoints,
				TotalKilledMonsterNum = (long)((ulong)client.ClientData.TotalKilledMonsterNum),
				TotalLoginNum = (long)((ulong)client.ClientData.TotalDayLoginNum),
				ContinueLoginNum = (int)client.ClientData.ContinuousDayLoginNum,
				ChengJiuFlags = ChengJiuManager.GetChengJiuInfoArray(client),
				NowCompletedChengJiu = justCompletedChengJiu,
				TotalKilledBossNum = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledBossNum)),
				CompleteNormalCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum)),
				CompleteHardCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum)),
				CompleteDifficltCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum)),
				GuildChengJiu = (long)client.ClientData.BangGong,
				JunXianChengJiu = (long)GameManager.ClientMgr.GetShengWangLevelValue(client)
			};
			byte[] bytesData = DataHelper.ObjectToBytes<ChengJiuData>(chengJiuData);
			GameManager.ClientMgr.SendToClient(client, bytesData, 420);
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x001A8B78 File Offset: 0x001A6D78
		protected static List<ushort> GetChengJiuInfoArray(GameClient client)
		{
			List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
			int curIndex = 0;
			List<ushort> lsUshort = new List<ushort>();
			for (int i = 0; i < lsLong.Count; i++)
			{
				ulong uValue = lsLong[i];
				for (int subIndex = 0; subIndex < 64; subIndex += 2)
				{
					ulong flag = 3UL << subIndex;
					ushort realFlag = (ushort)((uValue & flag) >> subIndex);
					ushort chengJiuID = ChengJiuManager.GetChengJiuIDByIndex(curIndex);
					ushort preFix = (ushort)(chengJiuID << 2);
					ushort chengJiu = (ushort)(preFix | realFlag);
					lsUshort.Add(chengJiu);
					curIndex += 2;
				}
			}
			return lsUshort;
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x001A8C18 File Offset: 0x001A6E18
		public static int GiveChengJiuAward(GameClient client, int chengJiuID, string strFrom)
		{
			int result;
			if (!ChengJiuManager.IsChengJiuCompleted(client, chengJiuID))
			{
				result = -1;
			}
			else if (ChengJiuManager.IsChengJiuAwardFetched(client, chengJiuID))
			{
				result = -2;
			}
			else
			{
				ChengJiuManager.UpdateChengJiuFlag(client, chengJiuID, true);
				string strResList = "";
				int bindZuanShi = 0;
				int awardBindMoney = 0;
				int awardChengJiuPoints = 0;
				SystemXmlItem itemChengJiu = null;
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(chengJiuID, out itemChengJiu))
				{
					bindZuanShi = Math.Max(0, itemChengJiu.GetIntValue("BindZuanShi", -1));
					awardBindMoney = Math.Max(0, itemChengJiu.GetIntValue("BindMoney", -1));
					awardChengJiuPoints = Math.Max(0, itemChengJiu.GetIntValue("ChengJiu", -1));
				}
				if (bindZuanShi > 0)
				{
					int oldGold = client.ClientData.Gold;
					GameManager.ClientMgr.AddUserGold(client, bindZuanShi, strFrom);
					strResList += EventLogManager.AddResPropString(ResLogType.BindZuanShi, new object[]
					{
						bindZuanShi,
						oldGold,
						client.ClientData.Gold
					});
				}
				if (awardBindMoney > 0)
				{
					awardBindMoney = Global.FilterValue(client, awardBindMoney);
					long oldMoney = (long)client.ClientData.Money1;
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, awardBindMoney, "完成成就：" + chengJiuID, false);
					strResList += EventLogManager.AddResPropString(ResLogType.BindJinbi, new object[]
					{
						awardBindMoney,
						oldMoney,
						client.ClientData.Money1
					});
				}
				if (awardChengJiuPoints > 0)
				{
					int oldChengJiu = GameManager.ClientMgr.GetChengJiuPointsValue(client);
					ChengJiuManager.AddChengJiuPoints(client, strFrom, awardChengJiuPoints, true, true);
					strResList += EventLogManager.AddResPropString(ResLogType.ChengJiu, new object[]
					{
						awardChengJiuPoints,
						oldChengJiu,
						GameManager.ClientMgr.GetChengJiuPointsValue(client)
					});
				}
				if (strResList.Length > 0)
				{
					strResList = strResList.Remove(0, 1);
				}
				EventLogManager.AddChengJiuAwardEvent(client, chengJiuID, strResList);
				result = 0;
			}
			return result;
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x001A8E6C File Offset: 0x001A706C
		public static long getChengJiuValue(GameClient client, AchievementType type)
		{
			if (type > AchievementType.Boss)
			{
				if (type <= AchievementType.EquipForge)
				{
					if (type <= AchievementType.CopyHard)
					{
						if (type == AchievementType.CopyNormal)
						{
							return (long)((ulong)(ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum) + ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum) + ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum)));
						}
						if (type != AchievementType.CopyHard)
						{
							goto IL_187;
						}
						return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum));
					}
					else
					{
						if (type == AchievementType.CopyDifficlt)
						{
							return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum));
						}
						if (type != AchievementType.EquipForge)
						{
							goto IL_187;
						}
					}
				}
				else if (type <= AchievementType.Merge)
				{
					if (type != AchievementType.EquipZhuLing && type != AchievementType.Merge)
					{
						goto IL_187;
					}
				}
				else
				{
					if (type == AchievementType.ShengWangLevel)
					{
						return (long)GameManager.ClientMgr.GetShengWangLevelValue(client);
					}
					if (type != AchievementType.MainLineTask)
					{
						goto IL_187;
					}
					return (long)client.ClientData.MainTaskID;
				}
				return 0L;
			}
			if (type <= AchievementType.LoginContinue)
			{
				if (type == AchievementType.PlayerLevel)
				{
					return (long)Global.GetUnionLevel(client, false);
				}
				if (type == AchievementType.ShenGe)
				{
					return (long)client.ClientData.ChangeLifeCount;
				}
				if (type == AchievementType.LoginContinue)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin));
				}
			}
			else if (type <= AchievementType.BindGoldCoin)
			{
				if (type == AchievementType.LoginTotal)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin));
				}
				if (type == AchievementType.BindGoldCoin)
				{
					return (long)client.ClientData.Money1;
				}
			}
			else
			{
				if (type == AchievementType.Monster)
				{
					return (long)((ulong)client.ClientData.TotalKilledMonsterNum);
				}
				if (type == AchievementType.Boss)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledBossNum));
				}
			}
			IL_187:
			return 0L;
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x001A900C File Offset: 0x001A720C
		public static bool IsFlagIsTrue(GameClient client, int chengJiuID, bool forAward = false)
		{
			int index = ChengJiuManager.GetCompletedFlagIndex(chengJiuID);
			bool result;
			if (index < 0)
			{
				result = false;
			}
			else
			{
				if (forAward)
				{
					index++;
				}
				List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
				if (lsLong.Count <= 0)
				{
					result = false;
				}
				else
				{
					int arrPosIndex = index / 64;
					if (arrPosIndex >= lsLong.Count)
					{
						result = false;
					}
					else
					{
						int subIndex = index % 64;
						ulong destLong = lsLong[arrPosIndex];
						ulong flag = 1UL << subIndex;
						bool bResult = (destLong & flag) > 0UL;
						result = bResult;
					}
				}
			}
			return result;
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x001A90AC File Offset: 0x001A72AC
		public static bool UpdateChengJiuFlag(GameClient client, int chengJiuID, bool forAward = false)
		{
			int index = ChengJiuManager.GetCompletedFlagIndex(chengJiuID);
			bool result;
			if (index < 0)
			{
				result = false;
			}
			else
			{
				if (forAward)
				{
					index++;
				}
				List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
				int arrPosIndex = index / 64;
				while (arrPosIndex > lsLong.Count - 1)
				{
					lsLong.Add(0UL);
				}
				int subIndex = index % 64;
				ulong destLong = lsLong[arrPosIndex];
				ulong flag = 1UL << subIndex;
				lsLong[arrPosIndex] = (destLong | flag);
				Global.SaveRoleParamsUlongListToDB(client, lsLong, "ChengJiuFlags", true);
				result = true;
			}
			return result;
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x001A914C File Offset: 0x001A734C
		public static void OnFirstKillMonster(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 100))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 100);
			}
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x001A9174 File Offset: 0x001A7374
		public static void OnFirstAddFriend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 101))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 101);
			}
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x001A919C File Offset: 0x001A739C
		public static void OnFirstInFaction(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 103))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 103);
			}
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x001A91C4 File Offset: 0x001A73C4
		public static void OnFirstInTeam(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 102))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 102);
			}
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x001A91EC File Offset: 0x001A73EC
		public static void OnFirstHeCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 104))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 104);
			}
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x001A9214 File Offset: 0x001A7414
		public static void OnFirstQiangHua(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 105))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 105);
			}
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x001A923C File Offset: 0x001A743C
		public static void OnFirstAppend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 106))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 106);
			}
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x001A9264 File Offset: 0x001A7464
		public static void OnFirstJiCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 107))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 107);
			}
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x001A928C File Offset: 0x001A748C
		public static void OnFirstBaiTan(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 108))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 108);
			}
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x001A92B4 File Offset: 0x001A74B4
		public static void OnMonsterKilled(GameClient killer, Monster victim)
		{
			if (0U == killer.ClientData.TotalKilledMonsterNum)
			{
				killer.ClientData.TotalKilledMonsterNum = ChengJiuManager.GetChengJiuExtraDataByField(killer, ChengJiuExtraDataField.TotalKilledMonsterNum);
				if (0U == killer.ClientData.TotalKilledMonsterNum)
				{
					ChengJiuManager.OnFirstKillMonster(killer);
				}
			}
			killer.ClientData.TotalKilledMonsterNum += 1U;
			SafeClientData clientData = killer.ClientData;
			clientData.TimerKilledMonsterNum += 1;
			bool bWriteDB = false;
			if (killer.ClientData.ChangeLifeCount == 0)
			{
				if (killer.ClientData.TimerKilledMonsterNum > 200)
				{
					bWriteDB = true;
				}
			}
			else if (killer.ClientData.TimerKilledMonsterNum > 500)
			{
				bWriteDB = true;
			}
			if (bWriteDB)
			{
				killer.ClientData.TimerKilledMonsterNum = 0;
				ChengJiuManager.SaveKilledMonsterNumToDB(killer, bWriteDB);
			}
			ChengJiuManager.CheckMonsterChengJiu(killer);
			if (401 == victim.MonsterType)
			{
				if (!ChengJiuManager.IsChengJiuCompleted(killer, 803))
				{
					for (int i = 0; i < Data.KillBossCountForChengJiu.Length; i++)
					{
						if (victim.MonsterInfo.ExtensionID == Data.KillBossCountForChengJiu[i])
						{
							int nKillBoss = (int)ChengJiuManager.GetChengJiuExtraDataByField(killer, ChengJiuExtraDataField.TotalKilledBossNum);
							ChengJiuManager.ModifyChengJiuExtraData(killer, (uint)(++nKillBoss), ChengJiuExtraDataField.TotalKilledBossNum, true);
							ChengJiuManager.CheckBossChengJiu(killer, nKillBoss);
						}
					}
				}
			}
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x001A942C File Offset: 0x001A762C
		public static void CheckMonsterChengJiu(GameClient client)
		{
			if (client.ClientData.TotalKilledMonsterNum >= client.ClientData.NextKilledMonsterChengJiuNum && 2147483647U != client.ClientData.NextKilledMonsterChengJiuNum)
			{
				uint nextNeedNum = ChengJiuManager.CheckSingleConditionChengJiu(client, 700, 708, (long)((ulong)client.ClientData.TotalKilledMonsterNum), "KillMonster");
				client.ClientData.NextKilledMonsterChengJiuNum = nextNeedNum;
				if (ChengJiuManager.IsChengJiuCompleted(client, 708))
				{
					client.ClientData.NextKilledMonsterChengJiuNum = 2147483647U;
				}
			}
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x001A94C6 File Offset: 0x001A76C6
		public static void CheckBossChengJiu(GameClient client, int nNum)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 800, 803, (long)nNum, "KillBoss");
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x001A94E4 File Offset: 0x001A76E4
		public static void OnTongQianIncrease(GameClient client)
		{
			if (0 > client.ClientData.MaxTongQianNum)
			{
				client.ClientData.MaxTongQianNum = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "MaxTongQianNum"));
			}
			if (client.ClientData.YinLiang >= client.ClientData.MaxTongQianNum)
			{
				client.ClientData.MaxTongQianNum = client.ClientData.YinLiang;
				Global.SaveRoleParamsInt32ValueToDB(client, "MaxTongQianNum", client.ClientData.MaxTongQianNum, false);
				if ((long)client.ClientData.MaxTongQianNum >= (long)((ulong)client.ClientData.NextTongQianChengJiuNum))
				{
					client.ClientData.NextTongQianChengJiuNum = ChengJiuManager.CheckSingleConditionChengJiu(client, 600, 608, (long)client.ClientData.MaxTongQianNum, "TongQianLimit");
				}
			}
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x001A95CB File Offset: 0x001A77CB
		public static void OnRoleLevelUp(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 200, 204, (long)client.ClientData.Level, "LevelLimit");
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x001A95F0 File Offset: 0x001A77F0
		public static void OnRoleChangeLife(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 300, 304, (long)client.ClientData.ChangeLifeCount, "ZhuanShengLimit");
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x001A9618 File Offset: 0x001A7818
		public static void OnRoleLogin(GameClient client, int preLoginDay)
		{
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID < preLoginDay && Math.Abs(dayID - preLoginDay) < 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else if (dayID != preLoginDay)
			{
				client.ClientData.TotalDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
				client.ClientData.ContinuousDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin);
				client.ClientData.TotalDayLoginNum += 1U;
				GameManager.ClientMgr.NotifySelfPropertyValue(client, 137, (long)((ulong)client.ClientData.TotalDayLoginNum));
				int preDay = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
				if (preDay == preLoginDay)
				{
					client.ClientData.ContinuousDayLoginNum += 1U;
					client.ClientData.SeriesLoginNum++;
				}
				else
				{
					client.ClientData.ContinuousDayLoginNum = 1U;
					client.ClientData.SeriesLoginNum = 1;
				}
				if ("" != client.strUserID)
				{
					GameManager.DBCmdMgr.AddDBCmd(10162, string.Format("{0}", client.strUserID), null, client.ServerId);
				}
				Global.UpdateSeriesLoginInfo(client);
				ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.TotalDayLoginNum, ChengJiuExtraDataField.TotalDayLogin, true);
				ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.ContinuousDayLoginNum, ChengJiuExtraDataField.ContinuousDayLogin, true);
				ChengJiuManager.CheckSingleConditionChengJiu(client, 400, 405, (long)((ulong)client.ClientData.ContinuousDayLoginNum), "LoginDayOne");
				ChengJiuManager.CheckSingleConditionChengJiu(client, 500, 508, (long)((ulong)client.ClientData.TotalDayLoginNum), "LoginDayTwo");
				DailyActiveManager.CleanDailyActiveInfo(client);
				if (!client.ClientData.DailyActiveDayLginSetFlag)
				{
					bool bIsCompleted = false;
					DailyActiveManager.ProcessLoginForDailyActive(client, out bIsCompleted);
				}
				client.ClientData.DailyActiveDayLginSetFlag = true;
				GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, -Global.GetRoleParamsInt32FromDB(client, "10255"), "首次登录重生槽免费次数重置", true, true, false);
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x001A986C File Offset: 0x001A7A6C
		public static void OnRoleEquipmentQiangHua(GameClient client, int equipStarsNum)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1200, 1210, (long)equipStarsNum, "QiangHuaLimit");
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x001A9894 File Offset: 0x001A7A94
		public static void OnRoleGoodsAppend(GameClient client, int AppendLev)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1300, 1308, (long)AppendLev, "ZhuiJiaLimit");
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x001A98BC File Offset: 0x001A7ABC
		public static void OnRoleGoodsHeCheng(GameClient client, int goodsIDCreated)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1400, 1411, (long)goodsIDCreated, "HeChengLimit");
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x001A98E4 File Offset: 0x001A7AE4
		public static void ProcessCompleteCopyMapForChengJiu(GameClient client, int nCopyMapLev, int count = 1)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 905) || !ChengJiuManager.IsChengJiuCompleted(client, 1005) || !ChengJiuManager.IsChengJiuCompleted(client, 1105))
			{
				if (nCopyMapLev >= 0)
				{
					switch (nCopyMapLev)
					{
					case 1:
					{
						int nNum = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum);
						nNum++;
						nNum *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)nNum, ChengJiuExtraDataField.CompleteNormalCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 900, 905, (long)nNum, "KillRaid");
						break;
					}
					case 2:
					{
						int nNum = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum);
						nNum++;
						nNum *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)nNum, ChengJiuExtraDataField.CompleteHardCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 1000, 1005, (long)nNum, "KillRaid");
						break;
					}
					case 3:
					{
						int nNum = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum);
						nNum++;
						nNum *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)nNum, ChengJiuExtraDataField.CompleteDifficltCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 1100, 1105, (long)nNum, "KillRaid");
						break;
					}
					}
				}
			}
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x001A99F8 File Offset: 0x001A7BF8
		public static void OnRoleSkillLevelUp(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 356))
			{
				bool bIsAddDefault = false;
				int nCurrentSkillLev = 0;
				for (int i = 0; i < client.ClientData.SkillDataList.Count; i++)
				{
					if (client.ClientData.SkillDataList[i].DbID == -1)
					{
						if (!bIsAddDefault)
						{
							nCurrentSkillLev += client.ClientData.SkillDataList[i].SkillLevel;
							bIsAddDefault = true;
						}
					}
					else
					{
						nCurrentSkillLev += client.ClientData.SkillDataList[i].SkillLevel;
					}
				}
				ChengJiuManager.CheckSingleConditionChengJiu(client, 350, 356, (long)nCurrentSkillLev, "SkillLevel");
			}
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x001A9ABC File Offset: 0x001A7CBC
		public static void OnRoleGuildChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 2004))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2000, 2004, (long)client.ClientData.BangGong, "ZhanGong");
			}
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x001A9B00 File Offset: 0x001A7D00
		public static void OnRoleJunXianChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, ChengJiuTypes.JunXianChengJiuEnd))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2050, ChengJiuTypes.JunXianChengJiuEnd, (long)GameManager.ClientMgr.GetShengWangLevelValue(client), "JunXian");
			}
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x001A9B44 File Offset: 0x001A7D44
		public static void ProcessCompleteMainTaskForChengJiu(GameClient client, int nTaskID)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, ChengJiuTypes.MainLineTaskEnd))
			{
				if (nTaskID >= 0)
				{
					SystemXmlItem itemChengJiu = null;
					int chengJiuID = ChengJiuTypes.MainLineTaskStart;
					while (chengJiuID <= ChengJiuTypes.MainLineTaskEnd)
					{
						if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(chengJiuID, out itemChengJiu))
						{
							if (null != itemChengJiu)
							{
								uint uValue = (uint)itemChengJiu.GetIntValue("RenWu", -1);
								if ((long)nTaskID >= (long)((ulong)uValue))
								{
									if (!ChengJiuManager.IsChengJiuCompleted(client, chengJiuID))
									{
										ChengJiuManager.OnChengJiuCompleted(client, chengJiuID);
									}
								}
							}
						}
						IL_89:
						chengJiuID++;
						continue;
						goto IL_89;
					}
				}
			}
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x001A9BF0 File Offset: 0x001A7DF0
		protected static uint CheckSingleConditionChengJiu(GameClient client, int chengJiuMinID, int chengJiuMaxID, long roleCurrentValue, string strCheckField)
		{
			SystemXmlItem itemChengJiu = null;
			uint needMinValue = 0U;
			int chengJiuID = chengJiuMinID;
			while (chengJiuID <= chengJiuMaxID)
			{
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(chengJiuID, out itemChengJiu))
				{
					if (null != itemChengJiu)
					{
						needMinValue = (uint)itemChengJiu.GetIntValue(strCheckField, -1);
						if (roleCurrentValue < (long)((ulong)needMinValue))
						{
							break;
						}
						if (!ChengJiuManager.IsChengJiuCompleted(client, chengJiuID))
						{
							ChengJiuManager.OnChengJiuCompleted(client, chengJiuID);
						}
					}
				}
				IL_6A:
				chengJiuID++;
				continue;
				goto IL_6A;
			}
			return needMinValue;
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x001A9C80 File Offset: 0x001A7E80
		protected static int CheckEquipmentChengJiu(GameClient client, int chengJiuMinID, int chengJiuMaxID, long roleCurrentValue, string strCheckField)
		{
			SystemXmlItem itemChengJiu = null;
			int maxCompletedID = -1;
			int chengJiuID = chengJiuMinID;
			while (chengJiuID <= chengJiuMaxID)
			{
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(chengJiuID, out itemChengJiu))
				{
					if (null != itemChengJiu)
					{
						string[] needMinValueArray = itemChengJiu.GetStringValue(strCheckField).Split(new char[]
						{
							','
						});
						if (needMinValueArray.Length == 2)
						{
							int needMinValue = Global.SafeConvertToInt32(needMinValueArray[0]);
							if (roleCurrentValue == (long)needMinValue)
							{
								int needMinNum = Global.SafeConvertToInt32(needMinValueArray[1]);
								if (needMinNum <= 1)
								{
									if (!ChengJiuManager.IsChengJiuCompleted(client, chengJiuID))
									{
										ChengJiuManager.OnChengJiuCompleted(client, chengJiuID);
										maxCompletedID = chengJiuID;
									}
								}
							}
						}
					}
				}
				IL_C2:
				chengJiuID++;
				continue;
				goto IL_C2;
			}
			return maxCompletedID;
		}

		// Token: 0x04002A50 RID: 10832
		public const string EncodingLatin1 = "latin1";

		// Token: 0x04002A51 RID: 10833
		private static Dictionary<int, int> _DictFlagIndex = new Dictionary<int, int>();

		// Token: 0x04002A52 RID: 10834
		private static Dictionary<int, AchievementRuneBasicData> _achievementRuneBasicList = new Dictionary<int, AchievementRuneBasicData>();

		// Token: 0x04002A53 RID: 10835
		private static Dictionary<int, AchievementRuneSpecialData> _achievementRuneSpecialList = new Dictionary<int, AchievementRuneSpecialData>();

		// Token: 0x04002A54 RID: 10836
		private static int _runeRate = 1;

		// Token: 0x04002A55 RID: 10837
		private static ChengJiuManager Instance = new ChengJiuManager();

		// Token: 0x020005E2 RID: 1506
		private enum AchievementRuneResultType
		{
			// Token: 0x04002A57 RID: 10839
			End = 3,
			// Token: 0x04002A58 RID: 10840
			Next = 2,
			// Token: 0x04002A59 RID: 10841
			Success = 1,
			// Token: 0x04002A5A RID: 10842
			Efail = 0,
			// Token: 0x04002A5B RID: 10843
			EnoOpen = -1,
			// Token: 0x04002A5C RID: 10844
			EnoAchievement = -2,
			// Token: 0x04002A5D RID: 10845
			EnoDiamond = -3,
			// Token: 0x04002A5E RID: 10846
			EOver = -4
		}
	}
}
