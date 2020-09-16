using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.MagicSword
{
	
	public class MagicSwordManager
	{
		
		public void LoadMagicSwordData()
		{
			try
			{
				string MagicSwordInitStr = GameManager.systemParamsList.GetParamValueByName("MJSChuShi");
				string[] MagicSwordInitArr = MagicSwordInitStr.Split(new char[]
				{
					'|'
				});
				if (MagicSwordInitArr.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, "魔剑士静态数据有误，无法读取", null, true);
				}
				else
				{
					string[] tmpArr = MagicSwordInitArr[4].Split(new char[]
					{
						','
					});
					if (tmpArr.Length != 2)
					{
						LogManager.WriteLog(LogTypes.Error, "魔剑士静态数据转生与级数有误，无法读取", null, true);
					}
					else
					{
						MagicSwordData.InitTaskID = int.Parse(MagicSwordInitArr[0]);
						MagicSwordData.InitTaskNpcID = int.Parse(MagicSwordInitArr[1]);
						MagicSwordData.InitPrevTaskID = int.Parse(MagicSwordInitArr[2]);
						MagicSwordData.InitMapID = int.Parse(MagicSwordInitArr[3]);
						MagicSwordData.InitChangeLifeCount = int.Parse(tmpArr[0]);
						MagicSwordData.InitLevel = int.Parse(tmpArr[1]);
						if (null == MagicSwordData.StrengthWeaponList)
						{
							MagicSwordData.StrengthWeaponList = new List<int>();
						}
						MagicSwordData.StrengthWeaponList.Clear();
						MagicSwordInitStr = GameManager.systemParamsList.GetParamValueByName("LiMJSDaTianShi");
						MagicSwordInitArr = MagicSwordInitStr.Split(new char[]
						{
							','
						});
						if (MagicSwordInitArr.Length > 0)
						{
							for (int i = 0; i < MagicSwordInitArr.Length; i++)
							{
								MagicSwordData.StrengthWeaponList.Add(int.Parse(MagicSwordInitArr[i]));
							}
						}
						if (null == MagicSwordData.IntelligenceWeaponList)
						{
							MagicSwordData.IntelligenceWeaponList = new List<int>();
						}
						MagicSwordData.IntelligenceWeaponList.Clear();
						MagicSwordInitStr = GameManager.systemParamsList.GetParamValueByName("ZhiMJSDaTianShi");
						MagicSwordInitArr = MagicSwordInitStr.Split(new char[]
						{
							','
						});
						if (MagicSwordInitArr.Length > 0)
						{
							for (int i = 0; i < MagicSwordInitArr.Length; i++)
							{
								MagicSwordData.IntelligenceWeaponList.Add(int.Parse(MagicSwordInitArr[i]));
							}
						}
						MagicSwordData.StrAttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("LiMJSAttackSkill", -1);
						MagicSwordData.IntAttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("ZhiMJSAttackSkill", -1);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMagicSwordData", new object[0])));
			}
		}

		
		public bool InitMagicSwordInfo(GameClient client, EMagicSwordTowardType eType)
		{
			bool result;
			if (!this.IsVersionSystemOpenOfMagicSword())
			{
				result = false;
			}
			else if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，初始化魔剑士信息", new object[0]), null, true);
				result = false;
			}
			else if (Global.AutoUpChangeLifeAndLevel(client, MagicSwordData.InitChangeLifeCount, MagicSwordData.InitLevel))
			{
				this.AutoMaigcSwordFirstAddPoint(client, eType);
				this.AutoGiveMagicSwordGoods(client);
				this.AutoGiveMagicSwordDefaultSkillHotKey(client, eType);
				GlobalEventSource.getInstance().fireEvent(new PlayerLevelupEventObject(client));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool IsMagicSword(GameClient client)
		{
			return client != null && this.IsMagicSword(client.ClientData.Occupation);
		}

		
		public bool IsMagicSword(int nOccu)
		{
			return Global.CalcOriginalOccupationID(nOccu) == 3;
		}

		
		public bool IsFirstLoginMagicSword(GameClient client, int nDestChangeLifeCount)
		{
			return this.IsVersionSystemOpenOfMagicSword() && client != null && this.IsMagicSword(client) && client.ClientData.ChangeLifeCount < nDestChangeLifeCount;
		}

		
		public bool IsMagicSwordAngelWeapon(GameClient client, int nGoodsID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.ClientData.Occupation != 3)
			{
				result = false;
			}
			else
			{
				List<int> tmpList;
				switch (this.GetMagicSwordTowardType(client))
				{
				case EMagicSwordTowardType.EMST_Strength:
					tmpList = MagicSwordData.StrengthWeaponList;
					break;
				case EMagicSwordTowardType.EMST_Intelligence:
					tmpList = MagicSwordData.IntelligenceWeaponList;
					break;
				default:
					return false;
				}
				for (int i = 0; i < tmpList.Count; i++)
				{
					if (nGoodsID == tmpList[i])
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		public bool IsMagicSwordWeapon(int nGoodsID)
		{
			SystemXmlItem systemGoods = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemGoods))
			{
				result = false;
			}
			else
			{
				ItemCategories eCategoriy = (ItemCategories)systemGoods.GetIntValue("Categoriy", -1);
				bool bRes = false;
				switch (eCategoriy)
				{
				case ItemCategories.WuQi_Jian:
				case ItemCategories.WuQi_Fu:
				case ItemCategories.WuQi_Chui:
				case ItemCategories.WuQi_Mao:
				case ItemCategories.WuQi_Zhang:
				case ItemCategories.WuQi_Dun:
				case ItemCategories.WuQi_Dao:
					bRes = true;
					break;
				}
				result = bRes;
			}
			return result;
		}

		
		public bool IsVersionSystemOpenOfMagicSword()
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MagicSword") && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot5);
		}

		
		public bool CanUseMagicOfMagicSword(GameClient client, int nMagicID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.buffManager.IsBuffEnabled(121))
			{
				result = true;
			}
			else if (!this.IsMagicSword(client))
			{
				result = true;
			}
			else if (!this.IsVersionSystemOpenOfMagicSword())
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemMagics = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(nMagicID, out systemMagics))
				{
					result = false;
				}
				else
				{
					int nMagicDamageType = systemMagics.GetIntValue("DamageType", -1);
					if (nMagicDamageType < 0)
					{
						result = true;
					}
					else
					{
						switch (nMagicDamageType)
						{
						case 1:
							if (nMagicID != 10000)
							{
								switch (nMagicID)
								{
								case 10088:
								case 10089:
								case 10090:
								case 10091:
									break;
								default:
									goto IL_122;
								}
							}
							return true;
						case 2:
							if (nMagicID != 10100)
							{
								switch (nMagicID)
								{
								case 10188:
								case 10189:
								case 10190:
								case 10191:
									break;
								default:
									goto IL_122;
								}
							}
							return true;
						}
						IL_122:
						List<GoodsData> WeaponList = client.UsingEquipMgr.GetWeaponEquipList();
						lock (WeaponList)
						{
							if (WeaponList == null || WeaponList.Count <= 0)
							{
								result = false;
							}
							else
							{
								List<GoodsData> BaseWeap = new List<GoodsData>();
								List<GoodsData> RebornWeap = new List<GoodsData>();
								foreach (GoodsData it in WeaponList)
								{
									if (RebornEquip.IsRebornEquipShengQi(it.GoodsID) || RebornEquip.IsRebornEquipShengWu(it.GoodsID))
									{
										RebornWeap.Add(it);
									}
									else
									{
										BaseWeap.Add(it);
									}
								}
								SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
								GoodsData goods;
								if (sceneType != SceneUIClasses.ChongShengMap)
								{
									if (client.ClientData.RebornShowEquip == 0)
									{
										goods = RebornEquip.GetMagicWeaponGoods(BaseWeap, true);
									}
									else
									{
										goods = RebornEquip.GetMagicWeaponGoods(RebornWeap, false);
									}
								}
								else
								{
									goods = RebornEquip.GetMagicWeaponGoods(RebornWeap, false);
								}
								if (goods == null)
								{
									result = false;
								}
								else
								{
									SystemXmlItem systemGoods = null;
									if (RebornEquip.IsRebornEquipShengQi(goods.GoodsID) || RebornEquip.IsRebornEquipShengWu(goods.GoodsID))
									{
										if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
										{
											RebornEquipXmlStruct REMap;
											if (!RebornEquip.EquipSQSW.TryGetValue(goods.GoodsID, out REMap))
											{
												return false;
											}
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(REMap.LMJSModGoodID, out systemGoods))
											{
												return false;
											}
										}
										else
										{
											RebornEquipXmlStruct REMap;
											if (!RebornEquip.EquipSQSW.TryGetValue(goods.GoodsID, out REMap))
											{
												return false;
											}
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(REMap.FMJSModGoodID, out systemGoods))
											{
												return false;
											}
										}
									}
									else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goods.GoodsID, out systemGoods))
									{
										return false;
									}
									int nStrength = systemGoods.GetIntValue("Strength", -1);
									int nIntelligence = systemGoods.GetIntValue("Intelligence", -1);
									int nWeaponDamageType = (nStrength >= nIntelligence) ? 1 : 2;
									if (nWeaponDamageType == nMagicDamageType)
									{
										result = true;
									}
									else
									{
										LogManager.WriteLog(LogTypes.Warning, string.Format("武器与技能类型不符，无法释放技能: RoleID={0}, 武器id{1}, 武器类型{2}, 技能id{3}, 技能类型{4}", new object[]
										{
											client.ClientData.RoleID,
											goods.GoodsID,
											nWeaponDamageType,
											nMagicID,
											nMagicDamageType
										}), null, true);
										result = false;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public bool IsCanAward2MagicSword(GameClient client, int nGoodsID)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			bool result;
			if (!this.IsMagicSword(nOcc))
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemGoods))
				{
					result = false;
				}
				else if (Global.GetMainOccupationByGoodsID(nGoodsID) == -1)
				{
					result = true;
				}
				else
				{
					EMagicSwordTowardType eEMSType = this.GetMagicSwordTowardType(client);
					int nStrength = systemGoods.GetIntValue("Strength", -1);
					int nIntelligence = systemGoods.GetIntValue("Intelligence", -1);
					EMagicSwordTowardType eEMSGoodType = EMagicSwordTowardType.EMST_Intelligence;
					if (nStrength >= nIntelligence)
					{
						eEMSGoodType = EMagicSwordTowardType.EMST_Strength;
					}
					result = (eEMSType == eEMSGoodType);
				}
			}
			return result;
		}

		
		public EMagicSwordTowardType GetMagicSwordTowardType(GameClient client)
		{
			double meStrength = RoleAlgorithm.GetStrength(client, true);
			double meIntelligence = RoleAlgorithm.GetIntelligence(client, true);
			EMagicSwordTowardType result;
			if (meStrength >= meIntelligence)
			{
				result = EMagicSwordTowardType.EMST_Strength;
			}
			else
			{
				result = EMagicSwordTowardType.EMST_Intelligence;
			}
			return result;
		}

		
		public void AutoGiveMagicSwordGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法给与魔剑士新手装备", new object[0]), null, true);
			}
			else if (this.IsMagicSword(client))
			{
				int nRoleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> giveEquip;
					if (EMagicSwordTowardType.EMST_Strength == this.GetMagicSwordTowardType(client))
					{
						giveEquip = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("LiMJSZhuangBei"), true, '|', ',');
						if (null == giveEquip)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备默认数据报错.RoleID{0}", nRoleID), null, true);
							return;
						}
						if (giveEquip.Count <= 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量为空.RoleID{0}", nRoleID), null, true);
							return;
						}
					}
					else
					{
						giveEquip = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("ZhiMJSZhuangBei"), true, '|', ',');
						if (null == giveEquip)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备默认数据报错.RoleID{0}", nRoleID), null, true);
							return;
						}
						if (giveEquip.Count <= 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量为空.RoleID{0}", nRoleID), null, true);
							return;
						}
					}
					bool bRingFalg = false;
					for (int i = 0; i < giveEquip.Count; i++)
					{
						int nGoodID = giveEquip[i][0];
						int nNum = giveEquip[i][1];
						int nBind = giveEquip[i][2];
						int nIntensify = giveEquip[i][3];
						int nAppendPropLev = giveEquip[i][4];
						int nLuck = giveEquip[i][5];
						int nExcellence = giveEquip[i][6];
						SystemXmlItem sytemGoodsItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodID, out sytemGoodsItem))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量ID不存在:RoleID{0},GoodsID={1}", nRoleID, nGoodID), null, true);
						}
						else if (!Global.IsRoleOccupationMatchGoods(client, nGoodID))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备与职业不符RoleID{0}, 物品id{1}.", nRoleID, nGoodID), null, true);
						}
						else if (1 != nNum)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量必须为1件RoleID{0}, 数量{1}.", nRoleID, nNum), null, true);
						}
						else
						{
							int nSeriralID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodID, nNum, 0, "", nIntensify, nBind, 0, "", false, 1, "自动给于魔剑士装备", "1900-01-01 12:00:00", 0, 0, nLuck, 0, nExcellence, nAppendPropLev, 0, null, null, 0, true);
							if (nSeriralID <= 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", nRoleID), null, true);
							}
							else
							{
								GoodsData newEquip = Global.GetGoodsByDbID(client, nSeriralID);
								if (null == newEquip)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量[GetGoodsByID]失败.RoleID{0}", nRoleID), null, true);
								}
								else
								{
									int nBagIndex = 0;
									int nCatetoriy = Global.GetGoodsCatetoriy(newEquip.GoodsID);
									if (nCatetoriy == 6 && bRingFalg)
									{
										nBagIndex++;
									}
									string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										1,
										newEquip.Id,
										newEquip.GoodsID,
										1,
										newEquip.Site,
										newEquip.GCount,
										nBagIndex,
										""
									});
									TCPProcessCmdResults eErrorCode = Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
									if (TCPProcessCmdResults.RESULT_FAILED == eErrorCode)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", nRoleID), null, true);
									}
									else
									{
										Global.RefreshEquipProp(client, newEquip);
										if (nCatetoriy == 6)
										{
											bRingFalg = true;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		
		public void AutoGiveMagicSwordDefaultSkillHotKey(GameClient client, EMagicSwordTowardType eType)
		{
			if (null != client)
			{
				string skillStr;
				switch (eType)
				{
				case EMagicSwordTowardType.EMST_Strength:
					skillStr = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
					{
						10004,
						10000,
						10006,
						10001
					});
					break;
				case EMagicSwordTowardType.EMST_Intelligence:
					skillStr = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
					{
						10104,
						10100,
						10106,
						10101
					});
					break;
				default:
					return;
				}
				string cmdStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, skillStr);
				client.ClientData.MainQuickBarKeys = skillStr;
				GameManager.DBCmdMgr.AddDBCmd(10010, cmdStr, null, client.ServerId);
			}
		}

		
		public EMagicSwordTowardType GetMagicSwordTypeByWeapon(int nOccu, List<GoodsData> list, GameClient client = null)
		{
			EMagicSwordTowardType result;
			lock (list)
			{
				EMagicSwordTowardType eType = EMagicSwordTowardType.EMST_Strength;
				if (!this.IsMagicSword(nOccu))
				{
					result = EMagicSwordTowardType.EMST_Not;
				}
				else if (list == null || list.Count <= 0)
				{
					result = eType;
				}
				else
				{
					SystemXmlItem systemGoods = null;
					List<GoodsData> WeaponList = new List<GoodsData>();
					List<GoodsData> BaseWeap = new List<GoodsData>();
					List<GoodsData> RebornWeap = new List<GoodsData>();
					GoodsData goodsData;
					for (int i = 0; i < list.Count; i++)
					{
						goodsData = list[i];
						if (null != goodsData)
						{
							int GoodId = goodsData.GoodsID;
							if (RebornEquip.IsRebornEquipShengQi(GoodId) || RebornEquip.IsRebornEquipShengWu(GoodId))
							{
								RebornWeap.Add(goodsData);
							}
							else
							{
								BaseWeap.Add(goodsData);
							}
						}
					}
					if (client == null)
					{
						if (BaseWeap == null || BaseWeap.Count <= 0)
						{
							return eType;
						}
						goodsData = RebornEquip.GetMagicWeaponGoods(BaseWeap, true);
					}
					else
					{
						SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
						if (sceneType != SceneUIClasses.ChongShengMap)
						{
							if (client.ClientData.RebornShowEquip == 0)
							{
								goodsData = RebornEquip.GetMagicWeaponGoods(BaseWeap, true);
							}
							else
							{
								if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
								{
									return EMagicSwordTowardType.EMST_Strength;
								}
								return EMagicSwordTowardType.EMST_Intelligence;
							}
						}
						else
						{
							if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
							{
								return EMagicSwordTowardType.EMST_Strength;
							}
							return EMagicSwordTowardType.EMST_Intelligence;
						}
					}
					if (null == goodsData)
					{
						result = eType;
					}
					else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
					{
						result = eType;
					}
					else
					{
						int nStrength = systemGoods.GetIntValue("Strength", -1);
						int nIntelligence = systemGoods.GetIntValue("Intelligence", -1);
						eType = ((nStrength >= nIntelligence) ? EMagicSwordTowardType.EMST_Strength : EMagicSwordTowardType.EMST_Intelligence);
						result = eType;
					}
				}
			}
			return result;
		}

		
		public void AutoMaigcSwordFirstAddPoint(GameClient client, EMagicSwordTowardType eType)
		{
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法根据参数表配置第一次给魔剑士加点", new object[0]), null, true);
			}
			else if (this.IsMagicSword(client))
			{
				if (eType == EMagicSwordTowardType.EMST_Strength || eType == EMagicSwordTowardType.EMST_Intelligence)
				{
					int nRoleID = client.ClientData.RoleID;
					try
					{
						string MagicSwordInitAttrStr;
						if (eType == EMagicSwordTowardType.EMST_Strength)
						{
							MagicSwordInitAttrStr = GameManager.systemParamsList.GetParamValueByName("LiMJS");
						}
						else
						{
							MagicSwordInitAttrStr = GameManager.systemParamsList.GetParamValueByName("ZhiMJS");
						}
						string[] MagicSwordInitAttrArr = MagicSwordInitAttrStr.Split(new char[]
						{
							','
						});
						if (MagicSwordInitAttrArr.Length != 4)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士读取初始加点失败，无法创建魔剑士, RoleID={0}", nRoleID), null, true);
						}
						else
						{
							int nPoint = 0;
							for (int i = 0; i < MagicSwordInitAttrArr.Length; i++)
							{
								nPoint += int.Parse(MagicSwordInitAttrArr[i]);
							}
							int nTotal = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
							int nStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrength");
							int nIntelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligence");
							int nDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterity");
							int nConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitution");
							int nRemainPoint = nTotal - nStrength - nIntelligence - nDexterity - nConstitution;
							if (nRemainPoint < nPoint)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士初始加点不足，无法创建魔剑士, RoleID={0}", nRoleID), null, true);
							}
							else
							{
								client.ClientData.PropStrength += int.Parse(MagicSwordInitAttrArr[0]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
								client.ClientData.PropIntelligence += int.Parse(MagicSwordInitAttrArr[1]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
								client.ClientData.PropDexterity += int.Parse(MagicSwordInitAttrArr[2]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
								client.ClientData.PropConstitution += int.Parse(MagicSwordInitAttrArr[3]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitution", client.ClientData.PropConstitution, true);
								client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
								client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
								if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
								{
									client.ClientData.CurrentLifeV = client.ClientData.LifeV;
								}
								if (client.ClientData.CurrentMagicV > client.ClientData.MagicV)
								{
									client.ClientData.CurrentMagicV = client.ClientData.MagicV;
								}
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							}
						}
					}
					catch (Exception ex)
					{
						DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
		}
	}
}
