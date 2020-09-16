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
	
	public class ChengJiuManager : IManager
	{
		
		public static ChengJiuManager GetInstance()
		{
			return ChengJiuManager.Instance;
		}

		
		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(670, 2, UpGradeChengLevelCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU));
			return true;
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
			return true;
		}

		
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

		
		public static void SetAchievementRuneCount(GameClient client, int count)
		{
			ChengJiuManager.ModifyAchievementRuneUpCount(client, count, true);
		}

		
		public static void SetAchievementRuneRate(GameClient client, int rate)
		{
			ChengJiuManager._runeRate = rate;
		}

		
		public static void initAchievementRune()
		{
			ChengJiuManager.LoadAchievementRuneBasicData();
			ChengJiuManager.LoadAchievementRuneSpecialData();
		}

		
		public static void initSetAchievementRuneProps(GameClient client)
		{
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.AchievementRune, false))
			{
				AchievementRuneData achievementRuneData = ChengJiuManager.GetAchievementRuneData(client);
				ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
			}
		}

		
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

		
		public static int GetAchievementRuneDiamond(GameClient client, int upCount)
		{
			int[] diamondList = GameManager.systemParamsList.GetParamValueIntArrayByName("ChengJiuFuWenZuanShi", ',');
			if (upCount >= diamondList.Length)
			{
				upCount = diamondList.Length - 1;
			}
			return diamondList[upCount];
		}

		
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

		
		public static void SaveRoleChengJiuData(GameClient client)
		{
		}

		
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

		
		public static void AddChengJiuPoints(GameClient client, string strFrom, int modifyValue = 1, bool forceUpdateBuffer = true, bool writeToDB = false)
		{
			GameManager.ClientMgr.ModifyChengJiuPointsValue(client, modifyValue, strFrom, writeToDB, true);
		}

		
		public static void SaveKilledMonsterNumToDB(GameClient client, bool bWriteDB = false)
		{
			ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.TotalKilledMonsterNum, ChengJiuExtraDataField.TotalKilledMonsterNum, bWriteDB);
		}

		
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

		
		public static int GetChengJiuLevel(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "ChengJiuLevel");
		}

		
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

		
		public int upGradeChengJiuBuffer(GameClient player)
		{
			return 1;
		}

		
		public static bool CanActiveNextChengHao(GameClient client)
		{
			return GameManager.ClientMgr.GetChengJiuPointsValue(client) >= ChengJiuManager.GetUpLevelNeedChengJiuPoint(client);
		}

		
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

		
		public static bool IsChengJiuCompleted(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, false);
		}

		
		public static bool IsChengJiuAwardFetched(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, true);
		}

		
		public static void OnChengJiuCompleted(GameClient client, int chengJiuID)
		{
			ChengJiuManager.UpdateChengJiuFlag(client, chengJiuID, false);
			ChengJiuManager.GiveChengJiuAward(client, chengJiuID, "完成成就ID：" + chengJiuID);
			ChengJiuManager.NotifyClientChengJiuData(client, chengJiuID);
			GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteChengJiu));
			ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, -1, -1, TaskTypes.ChengJiuUpdate, null, chengJiuID, -1L, null);
		}

		
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

		
		public static void OnFirstKillMonster(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 100))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 100);
			}
		}

		
		public static void OnFirstAddFriend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 101))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 101);
			}
		}

		
		public static void OnFirstInFaction(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 103))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 103);
			}
		}

		
		public static void OnFirstInTeam(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 102))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 102);
			}
		}

		
		public static void OnFirstHeCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 104))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 104);
			}
		}

		
		public static void OnFirstQiangHua(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 105))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 105);
			}
		}

		
		public static void OnFirstAppend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 106))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 106);
			}
		}

		
		public static void OnFirstJiCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 107))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 107);
			}
		}

		
		public static void OnFirstBaiTan(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 108))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 108);
			}
		}

		
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

		
		public static void CheckBossChengJiu(GameClient client, int nNum)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 800, 803, (long)nNum, "KillBoss");
		}

		
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

		
		public static void OnRoleLevelUp(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 200, 204, (long)client.ClientData.Level, "LevelLimit");
		}

		
		public static void OnRoleChangeLife(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 300, 304, (long)client.ClientData.ChangeLifeCount, "ZhuanShengLimit");
		}

		
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

		
		public static void OnRoleEquipmentQiangHua(GameClient client, int equipStarsNum)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1200, 1210, (long)equipStarsNum, "QiangHuaLimit");
		}

		
		public static void OnRoleGoodsAppend(GameClient client, int AppendLev)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1300, 1308, (long)AppendLev, "ZhuiJiaLimit");
		}

		
		public static void OnRoleGoodsHeCheng(GameClient client, int goodsIDCreated)
		{
			int nCompletedID = ChengJiuManager.CheckEquipmentChengJiu(client, 1400, 1411, (long)goodsIDCreated, "HeChengLimit");
		}

		
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

		
		public static void OnRoleGuildChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 2004))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2000, 2004, (long)client.ClientData.BangGong, "ZhanGong");
			}
		}

		
		public static void OnRoleJunXianChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, ChengJiuTypes.JunXianChengJiuEnd))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2050, ChengJiuTypes.JunXianChengJiuEnd, (long)GameManager.ClientMgr.GetShengWangLevelValue(client), "JunXian");
			}
		}

		
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

		
		public const string EncodingLatin1 = "latin1";

		
		private static Dictionary<int, int> _DictFlagIndex = new Dictionary<int, int>();

		
		private static Dictionary<int, AchievementRuneBasicData> _achievementRuneBasicList = new Dictionary<int, AchievementRuneBasicData>();

		
		private static Dictionary<int, AchievementRuneSpecialData> _achievementRuneSpecialList = new Dictionary<int, AchievementRuneSpecialData>();

		
		private static int _runeRate = 1;

		
		private static ChengJiuManager Instance = new ChengJiuManager();

		
		private enum AchievementRuneResultType
		{
			
			End = 3,
			
			Next = 2,
			
			Success = 1,
			
			Efail = 0,
			
			EnoOpen = -1,
			
			EnoAchievement = -2,
			
			EnoDiamond = -3,
			
			EOver = -4
		}
	}
}
