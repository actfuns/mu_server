using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.JingJiChang
{
	
	public class PrestigeMedalManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		
		public static PrestigeMedalManager getInstance()
		{
			return PrestigeMedalManager.instance;
		}

		
		public bool initialize()
		{
			return PrestigeMedalManager.initPrestigeMedal();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(782, 1, 1, PrestigeMedalManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(783, 2, 2, PrestigeMedalManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public void processEvent(EventObject eventObject)
		{
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 782:
				result = this.ProcessCmdPrestigeMedalInfo(client, nID, bytes, cmdParams);
				break;
			case 783:
				result = this.ProcessCmdPrestigeMedalUp(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		public bool ProcessCmdPrestigeMedalInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (cmdParams.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), cmdParams.Length), null, true);
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), roleID), null, true);
					return false;
				}
				PrestigeMedalData runeData = PrestigeMedalManager.GetPrestigeMedalData(client);
				client.sendCmd<PrestigeMedalData>(782, runeData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCmdPrestigeMedalUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (cmdParams.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), cmdParams.Length), null, true);
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int runeID = Convert.ToInt32(cmdParams[1]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), roleID), null, true);
					return false;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessCmdPrestigeMedalUp功能尚未开放, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), roleID), null, true);
					return false;
				}
				PrestigeMedalData runeData = PrestigeMedalManager.UpPrestigeMedal(client, runeID);
				client.sendCmd<PrestigeMedalData>(783, runeData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public static bool initPrestigeMedal()
		{
			bool result = PrestigeMedalManager.LoadPrestigeMedalBasicData();
			bool result2 = PrestigeMedalManager.LoadPrestigeMedalSpecialData();
			return result && result2;
		}

		
		public static bool LoadPrestigeMedalBasicData()
		{
			string fileName = "Config/ShengWangXunZhang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			bool result;
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/ShengWangXunZhang.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					PrestigeMedalManager._prestigeMedalBasicList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							PrestigeMedalBasicData config = new PrestigeMedalBasicData();
							config.MedalID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.MedalName = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "Name", ""));
							config.LifeMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LifeV", "0"));
							config.AttackMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddAttack", "0"));
							config.DefenseMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddDefense", "0"));
							config.HitMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HitV", "0"));
							config.PrestigeCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostShengWang", "0"));
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
							PrestigeMedalManager._prestigeMedalBasicList.Add(config.MedalID, config);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/ShengWangXunZhang.xml时文件出现异常!!!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public static bool LoadPrestigeMedalSpecialData()
		{
			string fileName = "Config/ShengWangSpecialAttribute.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			bool result;
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/ShengWangSpecialAttribute.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					PrestigeMedalManager._prestigeMedalSpecialList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							PrestigeMedalSpecialData config = new PrestigeMedalSpecialData();
							config.SpecialID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							config.MedalID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedFuWen", "0"));
							config.DoubleAttack = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "ZhiMingYiJi", "0"));
							config.DiDouble = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DiKangZhiMingYiJi", "0"));
							PrestigeMedalManager._prestigeMedalSpecialList.Add(config.MedalID, config);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/ShengWangSpecialAttribute.xml时出现异常!!!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public static PrestigeMedalBasicData GetPrestigeMedalBasicDataByID(int id)
		{
			PrestigeMedalBasicData result;
			if (PrestigeMedalManager._prestigeMedalBasicList.ContainsKey(id))
			{
				result = PrestigeMedalManager._prestigeMedalBasicList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public static PrestigeMedalSpecialData GetPrestigeMedalSpecialDataByID(int id)
		{
			PrestigeMedalSpecialData result;
			if (PrestigeMedalManager._prestigeMedalSpecialList.ContainsKey(id))
			{
				result = PrestigeMedalManager._prestigeMedalSpecialList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public static int GetPrestigeMedalUpCount(GameClient client)
		{
			int count = 0;
			int dayOld = 0;
			List<int> data = Global.GetRoleParamsIntListFromDB(client, "PrestigeMedalUpCount");
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
				PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, count, true);
			}
			return count;
		}

		
		public static void ModifyPrestigeMedalUpCount(GameClient client, int count, bool writeToDB = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				List<int> dataList = new List<int>();
				dataList.AddRange(new int[]
				{
					TimeUtil.NowDateTime().DayOfYear,
					count
				});
				Global.SaveRoleParamsIntListToDB(client, dataList, "PrestigeMedalUpCount", writeToDB);
			}
		}

		
		public static int GetPrestigeMedalDiamond(GameClient client, int upCount)
		{
			int[] diamondList = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengWangXunZhangZuanShi", ',');
			if (upCount >= diamondList.Length)
			{
				upCount = diamondList.Length - 1;
			}
			return diamondList[upCount];
		}

		
		public static PrestigeMedalData GetPrestigeMedalData(GameClient client)
		{
			PrestigeMedalData result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				result = null;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.PrestigeMedal, false))
			{
				result = null;
			}
			else
			{
				PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
				if (prestigeMedalData == null)
				{
					prestigeMedalData = new PrestigeMedalData();
					List<int> data = Global.GetRoleParamsIntListFromDB(client, "PrestigeMedal");
					PrestigeMedalBasicData basic;
					if (data == null || data.Count <= 0)
					{
						basic = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(PrestigeMedalManager._defaultMedalID);
						prestigeMedalData.RoleID = client.ClientData.RoleID;
						prestigeMedalData.MedalID = basic.MedalID;
						PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
					}
					else
					{
						prestigeMedalData.RoleID = client.ClientData.RoleID;
						prestigeMedalData.MedalID = data[0];
						prestigeMedalData.LifeAdd = data[1];
						prestigeMedalData.AttackAdd = data[2];
						prestigeMedalData.DefenseAdd = data[3];
						prestigeMedalData.HitAdd = data[4];
						if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
						{
							prestigeMedalData.UpResultType = 3;
							basic = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(PrestigeMedalManager._prestigeMedalBasicList.Count);
						}
						else
						{
							basic = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(prestigeMedalData.MedalID);
						}
					}
					prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, PrestigeMedalManager.GetPrestigeMedalUpCount(client));
					prestigeMedalData.Prestige = basic.PrestigeCost;
					client.ClientData.prestigeMedalData = prestigeMedalData;
				}
				prestigeMedalData.PrestigeLeft = GameManager.ClientMgr.GetShengWangValue(client);
				result = prestigeMedalData;
			}
			return result;
		}

		
		public static void ModifyPrestigeMedalData(GameClient client, PrestigeMedalData data, bool writeToDB = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				List<int> dataList = new List<int>();
				dataList.AddRange(new int[]
				{
					data.MedalID,
					data.LifeAdd,
					data.AttackAdd,
					data.DefenseAdd,
					data.HitAdd
				});
				Global.SaveRoleParamsIntListToDB(client, dataList, "PrestigeMedal", writeToDB);
			}
		}

		
		public static PrestigeMedalData UpPrestigeMedal(GameClient client, int MedalID)
		{
			PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
			PrestigeMedalData result;
			if (prestigeMedalData != null && prestigeMedalData.UpResultType == 3)
			{
				prestigeMedalData.UpResultType = -4;
				result = prestigeMedalData;
			}
			else if (prestigeMedalData == null || prestigeMedalData.MedalID != MedalID)
			{
				prestigeMedalData.UpResultType = 0;
				result = prestigeMedalData;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.PrestigeMedal, false))
			{
				prestigeMedalData.UpResultType = -1;
				result = prestigeMedalData;
			}
			else
			{
				PrestigeMedalBasicData basicMedal = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(MedalID);
				int prestigeNow = GameManager.ClientMgr.GetShengWangValue(client);
				if (basicMedal.PrestigeCost > prestigeNow)
				{
					prestigeMedalData.UpResultType = -2;
					result = prestigeMedalData;
				}
				else
				{
					int upCount = PrestigeMedalManager.GetPrestigeMedalUpCount(client);
					int diamondNeed = PrestigeMedalManager.GetPrestigeMedalDiamond(client, upCount);
					if (diamondNeed > 0 && !GameManager.ClientMgr.SubUserMoney(client, diamondNeed, "声望勋章提升", true, true, true, true, DaiBiSySType.ShengWangYinJi))
					{
						prestigeMedalData.UpResultType = -3;
						result = prestigeMedalData;
					}
					else
					{
						try
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, -basicMedal.PrestigeCost, "声望勋章提升", false, true);
						}
						catch (Exception)
						{
							prestigeMedalData.UpResultType = -2;
							return prestigeMedalData;
						}
						int[] addNums = null;
						int rate = 0;
						int r = Global.GetRandomNumber(0, 100);
						for (int i = 0; i < basicMedal.RateList.Count; i++)
						{
							rate += basicMedal.RateList[i];
							if (r <= rate)
							{
								addNums = basicMedal.AddNumList[i];
								prestigeMedalData.BurstType = i;
								break;
							}
						}
						prestigeMedalData.LifeAdd += addNums[0] * PrestigeMedalManager._medalRate;
						prestigeMedalData.LifeAdd = ((prestigeMedalData.LifeAdd > basicMedal.LifeMax) ? basicMedal.LifeMax : prestigeMedalData.LifeAdd);
						prestigeMedalData.AttackAdd += addNums[1] * PrestigeMedalManager._medalRate;
						prestigeMedalData.AttackAdd = ((prestigeMedalData.AttackAdd > basicMedal.AttackMax) ? basicMedal.AttackMax : prestigeMedalData.AttackAdd);
						prestigeMedalData.DefenseAdd += addNums[2] * PrestigeMedalManager._medalRate;
						prestigeMedalData.DefenseAdd = ((prestigeMedalData.DefenseAdd > basicMedal.DefenseMax) ? basicMedal.DefenseMax : prestigeMedalData.DefenseAdd);
						prestigeMedalData.HitAdd += addNums[3] * PrestigeMedalManager._medalRate;
						prestigeMedalData.HitAdd = ((prestigeMedalData.HitAdd > basicMedal.HitMax) ? basicMedal.HitMax : prestigeMedalData.HitAdd);
						if (prestigeMedalData.LifeAdd < basicMedal.LifeMax || prestigeMedalData.DefenseAdd < basicMedal.DefenseMax || prestigeMedalData.AttackAdd < basicMedal.AttackMax || prestigeMedalData.HitAdd < basicMedal.HitMax)
						{
							prestigeMedalData.UpResultType = 1;
							prestigeMedalData.Prestige = basicMedal.PrestigeCost;
							prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, upCount + 1);
						}
						else
						{
							prestigeMedalData.MedalID++;
							prestigeMedalData.LifeAdd = 0;
							prestigeMedalData.AttackAdd = 0;
							prestigeMedalData.DefenseAdd = 0;
							prestigeMedalData.HitAdd = 0;
							prestigeMedalData.UpResultType = 2;
							if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
							{
								prestigeMedalData.UpResultType = 3;
								prestigeMedalData.Prestige = 0;
								prestigeMedalData.Diamond = 0;
							}
							else
							{
								basicMedal = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(prestigeMedalData.MedalID);
								prestigeMedalData.Prestige = basicMedal.PrestigeCost;
								prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, upCount + 1);
							}
						}
						PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, upCount + 1, true);
						PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
						client.ClientData.prestigeMedalData = prestigeMedalData;
						PrestigeMedalManager.SetPrestigeMedalProps(client, prestigeMedalData);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						prestigeMedalData.PrestigeLeft = GameManager.ClientMgr.GetShengWangValue(client);
						result = prestigeMedalData;
					}
				}
			}
			return result;
		}

		
		public static void initSetPrestigeMedalProps(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				if (GlobalNew.IsGongNengOpened(client, GongNengIDs.PrestigeMedal, false))
				{
					PrestigeMedalData PrestigeMedalData = PrestigeMedalManager.GetPrestigeMedalData(client);
					PrestigeMedalManager.SetPrestigeMedalProps(client, PrestigeMedalData);
				}
			}
		}

		
		public static void SetPrestigeMedalProps(GameClient client, PrestigeMedalData PrestigeMedalData)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				int life = PrestigeMedalData.LifeAdd;
				int attack = PrestigeMedalData.AttackAdd;
				int defense = PrestigeMedalData.DefenseAdd;
				int hit = PrestigeMedalData.HitAdd;
				foreach (PrestigeMedalBasicData d in PrestigeMedalManager._prestigeMedalBasicList.Values)
				{
					if (d.MedalID < PrestigeMedalData.MedalID)
					{
						life += d.LifeMax;
						attack += d.AttackMax;
						defense += d.DefenseMax;
						hit += d.HitMax;
					}
				}
				double zhuoYue = 0.0;
				double diKang = 0.0;
				if (PrestigeMedalData.MedalID > 1)
				{
					PrestigeMedalSpecialData d2 = PrestigeMedalManager.GetPrestigeMedalSpecialDataByID(PrestigeMedalData.MedalID - 1);
					zhuoYue += d2.DoubleAttack;
					diKang += d2.DiDouble;
				}
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					13,
					life
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					45,
					attack
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					46,
					defense
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					18,
					hit
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					36,
					zhuoYue
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					53,
					diKang
				});
			}
		}

		
		public static void SetPrestigeLevel(GameClient client, int level)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWangLevel", level, true, "2020-12-12 12:12:12");
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望等级", "GM", "系统", client.ClientData.RoleName, "修改", level, client.ClientData.ZoneID, client.strUserID, level, client.ServerId, null);
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.GM, LogRecordType.IntValueWithType, new object[]
			{
				level,
				RoleAttributeType.ShengWangLevel
			});
			if (level > 0)
			{
				JingJiChangManager.getInstance().activeJunXianBuff(client, true);
			}
			Global.UpdateBufferData(client, BufferItemTypes.MU_JINGJICHANG_JUNXIAN, new double[]
			{
				(double)level - 1.0
			}, 0, true);
			ChengJiuManager.OnRoleJunXianChengJiu(client);
			Global.BroadcastClientMUShengWang(client, level);
			GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWangLevel, level);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			client._IconStateMgr.CheckJingJiChangJunXian(client);
			client._IconStateMgr.CheckSpecialActivity(client);
			client._IconStateMgr.CheckEverydayActivity(client);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		
		public static void SetPrestigeMedalLevel(GameClient client, int level)
		{
			level = ((level <= 0) ? 1 : level);
			PrestigeMedalData prestigeMedalData = new PrestigeMedalData();
			PrestigeMedalBasicData basic = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(level);
			prestigeMedalData.RoleID = client.ClientData.RoleID;
			prestigeMedalData.MedalID = basic.MedalID;
			if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
			{
				prestigeMedalData.UpResultType = 3;
			}
			PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
			client.ClientData.prestigeMedalData = prestigeMedalData;
			PrestigeMedalManager.SetPrestigeMedalProps(client, prestigeMedalData);
		}

		
		public static void SetPrestigeMedalCount(GameClient client, int count)
		{
			count = ((count < 0) ? 0 : count);
			PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, count, true);
			PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
			prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, PrestigeMedalManager.GetPrestigeMedalUpCount(client));
			client.ClientData.prestigeMedalData = prestigeMedalData;
		}

		
		public static void SetPrestigeMedalRate(GameClient client, int rate)
		{
			PrestigeMedalManager._medalRate = rate;
		}

		
		private static int _medalRate = 1;

		
		private static int _defaultMedalID = 1;

		
		private static Dictionary<int, PrestigeMedalBasicData> _prestigeMedalBasicList = new Dictionary<int, PrestigeMedalBasicData>();

		
		private static Dictionary<int, PrestigeMedalSpecialData> _prestigeMedalSpecialList = new Dictionary<int, PrestigeMedalSpecialData>();

		
		private int _State = 0;

		
		private static PrestigeMedalManager instance = new PrestigeMedalManager();

		
		private enum PrestigeMedalResultType
		{
			
			End = 3,
			
			Next = 2,
			
			Success = 1,
			
			Fail = 0,
			
			EnoOpen = -1,
			
			EnoPrestige = -2,
			
			EnoDiamond = -3,
			
			EOver = -4
		}
	}
}
