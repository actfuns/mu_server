using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.Talent
{
	
	public class TalentManager : ICmdProcessorEx, ICmdProcessor
	{
		
		public static TalentManager getInstance()
		{
			return TalentManager.instance;
		}

		
		public bool initialize()
		{
			TalentManager.LoadTalentExpInfo();
			TalentManager.LoadTalentSpecialData();
			TalentManager.LoadTalentInfoData();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(999, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1000, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1001, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1002, 2, 2, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1003, 3, 3, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 999:
				result = this.ProcessCmdTalentOther(client, nID, bytes, cmdParams);
				break;
			case 1000:
				result = this.ProcessCmdTalentGetData(client, nID, bytes, cmdParams);
				break;
			case 1001:
				result = this.ProcessCmdTalentAddExp(client, nID, bytes, cmdParams);
				break;
			case 1002:
				result = this.ProcessCmdTalentWash(client, nID, bytes, cmdParams);
				break;
			case 1003:
				result = this.ProcessCmdTalentAddEffect(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		public bool ProcessCmdTalentOther(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				TalentData talentData = null;
				GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
				if (otherClient != null)
				{
					talentData = TalentManager.GetTalentData(otherClient);
				}
				client.sendCmd<TalentData>(999, talentData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCmdTalentGetData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				TalentData talentData = TalentManager.GetTalentData(client);
				client.sendCmd<TalentData>(1000, talentData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCmdTalentAddExp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int state = TalentManager.TalentAddExp(client);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1001, talentData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCmdTalentWash(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int washType = int.Parse(cmdParams[1]);
				int state = TalentManager.TalentWash(client, washType);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1002, talentData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCmdTalentAddEffect(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int effectId = int.Parse(cmdParams[1]);
				int count = int.Parse(cmdParams[2]);
				int state = TalentManager.TalentAddEffect(client, effectId, count);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1003, talentData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private static TalentData GetTalentData(GameClient client)
		{
			TalentData result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Talent, false))
			{
				result = null;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = null;
			}
			else
			{
				client.ClientData.MyTalentData.IsOpen = true;
				client.ClientData.MyTalentData.SkillOneValue = client.ClientData.MyTalentPropData.SkillOneValue;
				client.ClientData.MyTalentData.SkillAllValue = client.ClientData.MyTalentPropData.SkillAllValue;
				client.ClientData.MyTalentData.Occupation = client.ClientData.Occupation;
				result = client.ClientData.MyTalentData;
			}
			return result;
		}

		
		private static int TalentAddExp(GameClient client)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Talent, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (client.ClientData.Experience <= 0L)
			{
				result = TalentResultType.EnoExp;
			}
			else
			{
				TalentData talentData = client.ClientData.MyTalentData;
				int talentCount = (talentData.TotalCount <= 0) ? 1 : (talentData.TotalCount + 1);
				if (!TalentManager._TalentExpList.ContainsKey(talentCount))
				{
					result = TalentResultType.EnoOpenPoint;
				}
				else
				{
					TalentExpInfo expInfo = TalentManager._TalentExpList[talentCount];
					int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
					if (level < expInfo.RoleLevel)
					{
						result = TalentResultType.EnoOpenPoint;
					}
					else
					{
						long needExp = expInfo.Exp - talentData.Exp;
						long exp = 0L;
						long expRole = client.ClientData.Experience;
						bool isUp = false;
						long expAdd;
						if (needExp <= expRole)
						{
							isUp = true;
							expAdd = needExp;
						}
						else
						{
							exp = talentData.Exp + expRole;
							talentCount--;
							expAdd = expRole;
						}
						if (!TalentManager.DBTalentModify(client.ClientData.RoleID, talentCount, exp, expAdd, isUp, client.ClientData.ZoneID, client.ServerId))
						{
							result = TalentResultType.EFail;
						}
						else
						{
							if (isUp)
							{
								talentData.Exp = exp;
								talentData.TotalCount++;
								client.ClientData.Experience -= needExp;
							}
							else
							{
								talentData.Exp = exp;
								client.ClientData.Experience -= expRole;
							}
							GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -exp);
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_Talent, new int[0]));
							if (isUp)
							{
								result = TalentResultType.Success;
							}
							else
							{
								result = TalentResultType.SuccessHalf;
							}
						}
					}
				}
			}
			return result;
		}

		
		private static int TalentWash(GameClient client, int washType)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Talent, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = TalentResultType.EnoOpen;
			}
			else
			{
				TalentData talentData = client.ClientData.MyTalentData;
				int washCount = TalentManager.GetTalentUseCount(talentData);
				if (washCount <= 0)
				{
					result = TalentResultType.EnoTalentCount;
				}
				else
				{
					if (washType == 0)
					{
						int needDiamond = TalentManager.GetWashDiamond(washCount);
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needDiamond, "天赋洗点", true, true, false, DaiBiSySType.TianFuXiDian))
						{
							return TalentResultType.EnoDiamond;
						}
					}
					else
					{
						int goodsId = 0;
						int goodsCount = 0;
						TalentManager.GetWashGoods(out goodsId, out goodsCount);
						GoodsData goodsData = Global.GetGoodsByID(client, goodsId);
						if (goodsData == null)
						{
							return TalentResultType.EnoWash;
						}
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, goodsCount, false, false))
						{
							return TalentResultType.EnoWash;
						}
					}
					if (!TalentManager.DBTalentEffectClear(client.ClientData.RoleID, client.ClientData.ZoneID, client.ServerId))
					{
						result = TalentResultType.EFail;
					}
					else
					{
						talentData.CountList[1] = 0;
						talentData.CountList[2] = 0;
						talentData.CountList[3] = 0;
						talentData.EffectList = new List<TalentEffectItem>();
						TalentPropData propData = client.ClientData.MyTalentPropData;
						propData.ResetProps();
						TalentManager.SetTalentProp(client, TalentEffectType.PropBasic, propData.PropItem);
						TalentManager.SetTalentProp(client, TalentEffectType.PropExt, propData.PropItem);
						TalentManager.RefreshProp(client);
						result = TalentResultType.Success;
					}
				}
			}
			return result;
		}

		
		private static int TalentAddEffect(GameClient client, int effectID, int addCount)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Talent, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = TalentResultType.EnoOpen;
			}
			else
			{
				TalentInfo talentInfo = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, effectID);
				if (talentInfo == null)
				{
					result = TalentResultType.EnoEffect;
				}
				else
				{
					TalentData talentData = client.ClientData.MyTalentData;
					int talentCountLeft = talentData.TotalCount - TalentManager.GetTalentUseCount(talentData);
					if (talentCountLeft < addCount)
					{
						result = TalentResultType.EnoTalentCount;
					}
					else if (!TalentManager.IsEffectOpen(talentData, talentInfo.NeedTalentID, talentInfo.NeedTalentLevel))
					{
						result = TalentResultType.EnoOpenPreEffect;
					}
					else if (talentInfo.NeedTalentCount > 0 && talentInfo.NeedTalentCount > talentData.CountList[talentInfo.Type])
					{
						result = TalentResultType.EnoOpenPreCount;
					}
					else
					{
						int newLevel = 0;
						TalentEffectItem effectItemOld = TalentManager.GetOpenEffectItem(talentData, effectID);
						if (effectItemOld != null)
						{
							if (effectItemOld.Level >= talentInfo.LevelMax)
							{
								return TalentResultType.EisMaxLevel;
							}
							newLevel = effectItemOld.Level;
						}
						newLevel += addCount;
						List<TalentEffectInfo> newItemEffectList = talentInfo.EffectList[newLevel];
						if (newLevel > talentInfo.LevelMax)
						{
							result = TalentResultType.EisMaxLevel;
						}
						else if (!TalentManager.DBTalentEffectModify(client.ClientData.RoleID, talentInfo.Type, effectID, newLevel, client.ClientData.ZoneID, client.ServerId))
						{
							result = TalentResultType.EFail;
						}
						else
						{
							Dictionary<int, int> countList;
							int type;
							(countList = talentData.CountList)[type = talentInfo.Type] = countList[type] + addCount;
							if (effectItemOld == null)
							{
								effectItemOld = new TalentEffectItem();
								effectItemOld.ID = effectID;
								effectItemOld.TalentType = talentInfo.Type;
								talentData.EffectList.Add(effectItemOld);
							}
							effectItemOld.Level = newLevel;
							effectItemOld.ItemEffectList = newItemEffectList;
							TalentManager.initTalentEffectProp(client);
							TalentManager.RefreshProp(client);
							result = TalentResultType.Success;
						}
					}
				}
			}
			return result;
		}

		
		public static void ModifyEffect(GameClient client, int effectID, int talentType, int newLevel)
		{
			try
			{
				TalentData talentData = client.ClientData.MyTalentData;
				TalentInfo talentInfo = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, effectID);
				if (talentInfo != null)
				{
					List<TalentEffectInfo> newItemEffectList = talentInfo.EffectList[newLevel];
					bool result = TalentManager.DBTalentEffectModify(client.ClientData.RoleID, talentType, effectID, newLevel, client.ClientData.ZoneID, client.ServerId);
					Dictionary<int, int> countList;
					(countList = talentData.CountList)[talentType] = countList[talentType] + newLevel;
					TalentEffectItem effectItemOld = TalentManager.GetOpenEffectItem(talentData, effectID);
					if (effectItemOld == null)
					{
						effectItemOld = new TalentEffectItem();
						effectItemOld.ID = effectID;
						effectItemOld.TalentType = talentType;
						talentData.EffectList.Add(effectItemOld);
					}
					effectItemOld.Level = newLevel;
					effectItemOld.ItemEffectList = newItemEffectList;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		private static bool IsEffectOpen(TalentData talentData, int effectID, int level)
		{
			bool result;
			if (effectID <= 0)
			{
				result = true;
			}
			else
			{
				TalentEffectItem item = TalentManager.GetOpenEffectItem(talentData, effectID);
				if (item != null)
				{
					if (item.Level >= level)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		private static TalentEffectItem GetOpenEffectItem(TalentData talentData, int effectID)
		{
			TalentEffectItem result;
			if (effectID <= 0)
			{
				result = null;
			}
			else
			{
				foreach (TalentEffectItem item in talentData.EffectList)
				{
					if (item.ID == effectID)
					{
						return item;
					}
				}
				result = null;
			}
			return result;
		}

		
		private static int GetTalentUseCount(TalentData talentData)
		{
			int result;
			if (talentData.CountList == null || talentData.CountList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				result = talentData.CountList[2] + talentData.CountList[1] + talentData.CountList[3];
			}
			return result;
		}

		
		public static void initTalentEffectProp(GameClient client)
		{
			TalentData myTalentData = TalentManager.GetTalentData(client);
			if (myTalentData != null && myTalentData.IsOpen)
			{
				TalentPropData myPropData = client.ClientData.MyTalentPropData;
				myPropData.ResetProps();
				foreach (TalentEffectItem item in myTalentData.EffectList)
				{
					TalentInfo talentInfo = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, item.ID);
					if (talentInfo.LevelMax >= item.Level)
					{
						item.ItemEffectList = talentInfo.EffectList[item.Level];
						foreach (TalentEffectInfo info in item.ItemEffectList)
						{
							switch (info.EffectType)
							{
							case 1:
								myPropData.PropItem.BaseProps[info.EffectID] += (double)((int)info.EffectValue);
								break;
							case 2:
								myPropData.PropItem.ExtProps[info.EffectID] += info.EffectValue;
								break;
							case 3:
								if (myPropData.SkillOneValue.ContainsKey(info.EffectID))
								{
									Dictionary<int, int> skillOneValue;
									int effectID;
									(skillOneValue = myPropData.SkillOneValue)[effectID = info.EffectID] = skillOneValue[effectID] + (int)info.EffectValue;
								}
								else
								{
									myPropData.SkillOneValue.Add(info.EffectID, (int)info.EffectValue);
								}
								break;
							case 4:
								myPropData.SkillAllValue += (int)info.EffectValue;
								break;
							}
						}
					}
				}
				TalentManager.InitSpecialProp(client);
				client.ClientData.MyTalentData.SkillOneValue = client.ClientData.MyTalentPropData.SkillOneValue;
				client.ClientData.MyTalentData.SkillAllValue = client.ClientData.MyTalentPropData.SkillAllValue;
				TalentManager.SetTalentProp(client, TalentEffectType.PropBasic, myPropData.PropItem);
				TalentManager.SetTalentProp(client, TalentEffectType.PropExt, myPropData.PropItem);
			}
		}

		
		private static void InitSpecialProp(GameClient client)
		{
			TalentData talentData = client.ClientData.MyTalentData;
			if (talentData.CountList != null && talentData.CountList.Count > 0)
			{
				foreach (KeyValuePair<int, int> c in talentData.CountList)
				{
					int type = c.Key;
					int value = c.Value;
					TalentSpecialInfo specialInfo = TalentManager._TalentSpecialList[type];
					int count = value / specialInfo.SingleCount;
					foreach (KeyValuePair<int, double> item in specialInfo.EffectList)
					{
						int effectType = item.Key;
						double effectValue = item.Value * (double)count;
						client.ClientData.MyTalentPropData.PropItem.ExtProps[effectType] += effectValue;
					}
				}
			}
		}

		
		private static void SetTalentProp(GameClient client, TalentEffectType type, EquipPropItem item)
		{
			switch (type)
			{
			case TalentEffectType.PropBasic:
				client.ClientData.PropsCacheManager.SetBaseProps(new object[]
				{
					14,
					(int)type,
					item.BaseProps
				});
				break;
			case TalentEffectType.PropExt:
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					14,
					(int)type,
					item.ExtProps
				});
				break;
			}
		}

		
		public static void RefreshProp(GameClient client)
		{
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				DelayExecProcIds.RecalcProps,
				DelayExecProcIds.NotifyRefreshProps
			});
		}

		
		public static int GetSkillLevel(GameClient client, int skillID)
		{
			int level = 0;
			if (client.ClientData.MyTalentData.IsOpen)
			{
				TalentPropData talentData = client.ClientData.MyTalentPropData;
				if (talentData.SkillOneValue.ContainsKey(skillID))
				{
					level += talentData.SkillOneValue[skillID];
				}
				else
				{
					SystemXmlItem systemMagic = null;
					if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagic))
					{
						return level;
					}
					int nParentMagicID = systemMagic.GetIntValue("ParentMagicID", -1);
					if (nParentMagicID > 0)
					{
						SkillData ParentSkillData = Global.GetSkillDataByID(client, nParentMagicID);
						if (null != ParentSkillData)
						{
							if (talentData.SkillOneValue.ContainsKey(ParentSkillData.SkillID))
							{
								level += talentData.SkillOneValue[ParentSkillData.SkillID];
							}
						}
					}
				}
				level += talentData.SkillAllValue;
			}
			return level;
		}

		
		private static void LoadTalentExpInfo()
		{
			string fileName = Global.GameResPath("Config/TianFuDian.xml");
			XElement xml = CheckHelper.LoadXml(fileName, true);
			if (null != xml)
			{
				try
				{
					TalentManager._TalentExpList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							TalentExpInfo info = new TalentExpInfo();
							info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TianFuDian", "0"));
							info.Exp = Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "NeedExp", "0"));
							string[] level = Global.GetDefAttributeStr(xmlItem, "NeedLevel", "0,0").Split(new char[]
							{
								','
							});
							info.RoleLevel = int.Parse(level[0]) * 100 + int.Parse(level[1]);
							TalentManager._TalentExpList.Add(info.ID, info);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
				}
			}
		}

		
		private static int GetWashDiamond(int count)
		{
			int[] diamondList = GameManager.systemParamsList.GetParamValueIntArrayByName("ResettingTianFuCostZuanShi", ',');
			return Math.Min(count * diamondList[0], diamondList[1]);
		}

		
		private static void GetWashGoods(out int goodsID, out int goodsCount)
		{
			int[] arr = GameManager.systemParamsList.GetParamValueIntArrayByName("ResettingTianFuCostGoods", ',');
			goodsID = arr[0];
			goodsCount = arr[1];
		}

		
		public static void LoadTalentSpecialData()
		{
			string fileName = Global.GameResPath("Config/TianFuGroupProperty.xml");
			XElement xml = CheckHelper.LoadXml(fileName, true);
			if (null != xml)
			{
				try
				{
					Dictionary<int, TalentSpecialInfo> dict = new Dictionary<int, TalentSpecialInfo>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							TalentSpecialInfo config = new TalentSpecialInfo();
							config.SpecialType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TianFuType", "0"));
							config.SingleCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedExp", "0"));
							config.EffectList = new Dictionary<int, double>();
							double value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "TripleAttack", "0"));
							config.EffectList.Add(61, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "SlowAttack", "0"));
							config.EffectList.Add(62, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "VampiricAttack", "0"));
							config.EffectList.Add(63, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "TripleDefense", "0"));
							config.EffectList.Add(64, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "SlowDefensee", "0"));
							config.EffectList.Add(65, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "VampiricDefense", "0"));
							config.EffectList.Add(66, value);
							dict.Add(config.SpecialType, config);
						}
					}
					TalentManager._TalentSpecialList = dict;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
				}
			}
		}

		
		private static void LoadTalentInfoData()
		{
			TalentManager._TalentInfoList.Clear();
			for (int i = 0; i < 6; i++)
			{
				Dictionary<int, TalentInfo> list = new Dictionary<int, TalentInfo>();
				string fileName = Global.GameResPath(string.Format("Config/TianFuProperty_{0}.xml", i));
				XElement xml = CheckHelper.LoadXml(fileName, false);
				if (null == xml)
				{
					TalentManager._TalentInfoList.Add(i, list);
				}
				else
				{
					try
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (xmlItem != null)
							{
								TalentInfo config = new TalentInfo();
								config.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								config.Type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TianFuType", "0"));
								config.Name = Global.GetDefAttributeStr(xmlItem, "Name", "");
								config.NeedTalentCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedInputPoint", "0"));
								config.NeedTalentID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedTianFu", "0"));
								config.NeedTalentLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedTianFuLevel", "0"));
								config.LevelMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LevelMax", "0"));
								config.EffectType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EffectType", "0"));
								config.EffectList = new Dictionary<int, List<TalentEffectInfo>>();
								string effect = Global.GetDefAttributeStr(xmlItem, "Effect1", "");
								TalentManager.XmlGetTalentEffect(config, 1, effect);
								effect = Global.GetDefAttributeStr(xmlItem, "Effect2", "");
								TalentManager.XmlGetTalentEffect(config, 2, effect);
								effect = Global.GetDefAttributeStr(xmlItem, "Effect3", "");
								TalentManager.XmlGetTalentEffect(config, 3, effect);
								effect = Global.GetDefAttributeStr(xmlItem, "Effect4", "");
								TalentManager.XmlGetTalentEffect(config, 4, effect);
								effect = Global.GetDefAttributeStr(xmlItem, "Effect5", "");
								TalentManager.XmlGetTalentEffect(config, 5, effect);
								list.Add(config.ID, config);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!{1}", fileName, ex.Message), null, true);
					}
					TalentManager._TalentInfoList.Add(i, list);
				}
			}
		}

		
		private static void XmlGetTalentEffect(TalentInfo talentInfo, int level, string strEffect)
		{
			if (!string.IsNullOrEmpty(strEffect))
			{
				string[] arrEffect = strEffect.Split(new char[]
				{
					'|'
				});
				List<TalentEffectInfo> list = new List<TalentEffectInfo>();
				foreach (string effect in arrEffect)
				{
					string[] arr = effect.Split(new char[]
					{
						','
					});
					TalentEffectInfo info = new TalentEffectInfo();
					info.EffectType = talentInfo.EffectType;
					switch (info.EffectType)
					{
					case 1:
						info.EffectID = (int)Enum.Parse(typeof(UnitPropIndexes), arr[0]);
						info.EffectValue = double.Parse(arr[1]);
						break;
					case 2:
						info.EffectID = (int)Enum.Parse(typeof(ExtPropIndexes), arr[0]);
						info.EffectValue = double.Parse(arr[1]);
						break;
					case 3:
					case 4:
						info.EffectID = int.Parse(arr[1]);
						info.EffectValue = double.Parse(arr[2]);
						break;
					}
					list.Add(info);
				}
				talentInfo.EffectList.Add(level, list);
			}
		}

		
		private static TalentInfo GetTalentInfoByID(int type, int id)
		{
			TalentInfo result;
			if (type >= 6 || type < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, TalentInfo> list = TalentManager._TalentInfoList[type];
				if (list.ContainsKey(id))
				{
					result = list[id];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		private static bool DBTalentModify(int roleID, int totalCount, long exp, long expAdd, bool isUp, int zoneID, int serverID)
		{
			bool result = false;
			int up = isUp ? 1 : 0;
			string cmd2db = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				roleID,
				totalCount,
				exp,
				expAdd,
				up,
				zoneID
			});
			string[] dbFields = Global.ExecuteDBCmd(13108, cmd2db, serverID);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = (dbFields[0] == 0.ToString());
			}
			return result;
		}

		
		public static bool DBTalentEffectModify(int roleID, int talentType, int effectID, int effectLevel, int zoneID, int serverID)
		{
			bool result = false;
			string cmd2db = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				roleID,
				talentType,
				effectID,
				effectLevel,
				zoneID
			});
			string[] dbFields = Global.ExecuteDBCmd(13109, cmd2db, serverID);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = (dbFields[0] == 0.ToString());
			}
			return result;
		}

		
		public static bool DBTalentEffectClear(int roleID, int zoneID, int serverID)
		{
			bool result = false;
			string cmd2db = string.Format("{0}:{1}", roleID, zoneID);
			string[] dbFields = Global.ExecuteDBCmd(13110, cmd2db, serverID);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = (dbFields[0] == 0.ToString());
			}
			return result;
		}

		
		public static bool TalentAddCount(GameClient client, int count)
		{
			TalentData talentData = client.ClientData.MyTalentData;
			bool result;
			if (!talentData.IsOpen)
			{
				result = false;
			}
			else if (!TalentManager.DBTalentModify(client.ClientData.RoleID, count, 0L, 0L, false, client.ClientData.ZoneID, client.ServerId))
			{
				result = false;
			}
			else
			{
				talentData.TotalCount = count;
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_Talent, new int[0]));
				result = true;
			}
			return result;
		}

		
		private static TalentManager instance = new TalentManager();

		
		private static Dictionary<int, TalentExpInfo> _TalentExpList = new Dictionary<int, TalentExpInfo>();

		
		private static Dictionary<int, TalentSpecialInfo> _TalentSpecialList = new Dictionary<int, TalentSpecialInfo>();

		
		private static Dictionary<int, Dictionary<int, TalentInfo>> _TalentInfoList = new Dictionary<int, Dictionary<int, TalentInfo>>();
	}
}
