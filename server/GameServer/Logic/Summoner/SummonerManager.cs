using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Summoner
{
	
	public class SummonerManager
	{
		
		public void LoadSummonerData()
		{
			try
			{
				string SummonerInitStr = GameManager.systemParamsList.GetParamValueByName("ZHSChuShi");
				string[] SummonerInitArr = SummonerInitStr.Split(new char[]
				{
					'|'
				});
				if (SummonerInitArr.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, "召唤师静态数据有误，无法读取", null, true);
				}
				else
				{
					string[] tmpArr = SummonerInitArr[4].Split(new char[]
					{
						','
					});
					if (tmpArr.Length != 2)
					{
						LogManager.WriteLog(LogTypes.Error, "召唤师静态数据转生与级数有误，无法读取", null, true);
					}
					else
					{
						SummonerData.InitTaskID = int.Parse(SummonerInitArr[0]);
						SummonerData.InitTaskNpcID = int.Parse(SummonerInitArr[1]);
						SummonerData.InitPrevTaskID = int.Parse(SummonerInitArr[2]);
						SummonerData.InitMapID = int.Parse(SummonerInitArr[3]);
						SummonerData.InitChangeLifeCount = int.Parse(tmpArr[0]);
						SummonerData.InitLevel = int.Parse(tmpArr[1]);
						if (null == SummonerData.WeaponList)
						{
							SummonerData.WeaponList = new List<int>();
						}
						SummonerData.WeaponList.Clear();
						SummonerInitStr = GameManager.systemParamsList.GetParamValueByName("ZHSDaTianShi");
						SummonerInitArr = SummonerInitStr.Split(new char[]
						{
							','
						});
						if (SummonerInitArr.Length > 0)
						{
							for (int i = 0; i < SummonerInitArr.Length; i++)
							{
								SummonerData.WeaponList.Add(int.Parse(SummonerInitArr[i]));
							}
						}
						if (null == SummonerData.CreateMapSet)
						{
							SummonerData.CreateMapSet = new HashSet<int>();
						}
						SummonerData.CreateMapSet.Clear();
						SummonerInitStr = GameManager.systemParamsList.GetParamValueByName("ZHSCreateMap");
						SummonerInitArr = SummonerInitStr.Split(new char[]
						{
							','
						});
						if (SummonerInitArr.Length > 0 && SummonerInitStr != "")
						{
							for (int i = 0; i < SummonerInitArr.Length; i++)
							{
								SummonerData.CreateMapSet.Add(int.Parse(SummonerInitArr[i]));
							}
						}
						SummonerData.AttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("ZHSAttackSkill", -1);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadSummonerData", new object[0])));
			}
		}

		
		public bool InitSummonerInfo(GameClient client)
		{
			bool result;
			if (!this.IsVersionSystemOpenOfSummoner())
			{
				result = false;
			}
			else if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，初始化召唤师信息", new object[0]), null, true);
				result = false;
			}
			else if (Global.AutoUpChangeLifeAndLevel(client, SummonerData.InitChangeLifeCount, SummonerData.InitLevel))
			{
				this.AutoSummonerFirstAddPoint(client);
				this.AutoGiveSummonerGoods(client);
				this.AutoGiveSummonerDefaultSkillHotKey(client);
				GlobalEventSource.getInstance().fireEvent(new PlayerLevelupEventObject(client));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool IsSummoner(int nOccu)
		{
			return Global.CalcOriginalOccupationID(nOccu) == 5;
		}

		
		public bool IsFirstLoginSummoner(GameClient client, int nDestChangeLifeCount)
		{
			return this.IsVersionSystemOpenOfSummoner() && client != null && this.IsSummoner(client.ClientData.Occupation) && client.ClientData.ChangeLifeCount < nDestChangeLifeCount;
		}

		
		public bool IsSummonerWeapon(GameClient client, int nGoodsID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.ClientData.Occupation != 5)
			{
				result = false;
			}
			else
			{
				List<int> tmpList = SummonerData.WeaponList;
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

		
		public bool IsVersionSystemOpenOfSummoner()
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Summoner") && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot3);
		}

		
		public void AutoGiveSummonerGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法给与召唤师新手装备", new object[0]), null, true);
			}
			else if (this.IsSummoner(client.ClientData.Occupation))
			{
				int nRoleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> giveEquip = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("ZHSZhuangBei"), true, '|', ',');
					if (null == giveEquip)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备默认数据报错.RoleID{0}", nRoleID), null, true);
					}
					else if (giveEquip.Count <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量为空.RoleID{0}", nRoleID), null, true);
					}
					else
					{
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
								LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量ID不存在:RoleID{0},GoodsID={1}", nRoleID, nGoodID), null, true);
							}
							else if (!Global.IsRoleOccupationMatchGoods(client, nGoodID))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备与职业不符RoleID{0}, 物品id{1}.", nRoleID, nGoodID), null, true);
							}
							else if (1 != nNum)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量必须为1件RoleID{0}, 数量{1}.", nRoleID, nNum), null, true);
							}
							else
							{
								int nSeriralID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodID, nNum, 0, "", nIntensify, nBind, 0, "", false, 1, "自动给于召唤师装备", "1900-01-01 12:00:00", 0, 0, nLuck, 0, nExcellence, nAppendPropLev, 0, null, null, 0, true);
								if (nSeriralID <= 0)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", nRoleID), null, true);
								}
								else
								{
									GoodsData newEquip = Global.GetGoodsByDbID(client, nSeriralID);
									if (null == newEquip)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量[GetGoodsByID]失败.RoleID{0}", nRoleID), null, true);
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
											LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", nRoleID), null, true);
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
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		
		public void AutoGiveSummonerDefaultSkillHotKey(GameClient client)
		{
			if (null != client)
			{
				string skillStr = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
				{
					11006,
					11000,
					11004,
					11001
				});
				string cmdStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, skillStr);
				client.ClientData.MainQuickBarKeys = skillStr;
				GameManager.DBCmdMgr.AddDBCmd(10010, cmdStr, null, client.ServerId);
			}
		}

		
		public void AutoSummonerFirstAddPoint(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法根据参数表配置第一次给召唤师加点", new object[0]), null, true);
			}
			else if (this.IsSummoner(client.ClientData.Occupation))
			{
				int nRoleID = client.ClientData.RoleID;
				try
				{
					string SummonerInitAttrStr = GameManager.systemParamsList.GetParamValueByName("ZHSShuXing");
					string[] SummonerInitAttrArr = SummonerInitAttrStr.Split(new char[]
					{
						','
					});
					if (SummonerInitAttrArr.Length != 4)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("召唤师读取初始加点失败，无法创建召唤师, RoleID={0}", nRoleID), null, true);
					}
					else
					{
						int nPoint = 0;
						for (int i = 0; i < SummonerInitAttrArr.Length; i++)
						{
							nPoint += int.Parse(SummonerInitAttrArr[i]);
						}
						int nTotal = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
						int nStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrength");
						int nIntelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligence");
						int nDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterity");
						int nConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitution");
						int nRemainPoint = nTotal - nStrength - nIntelligence - nDexterity - nConstitution;
						if (nRemainPoint < nPoint)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("召唤师初始加点不足，无法创建召唤师, RoleID={0}", nRoleID), null, true);
						}
						else
						{
							client.ClientData.PropStrength += int.Parse(SummonerInitAttrArr[0]);
							Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
							client.ClientData.PropIntelligence += int.Parse(SummonerInitAttrArr[1]);
							Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
							client.ClientData.PropDexterity += int.Parse(SummonerInitAttrArr[2]);
							Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
							client.ClientData.PropConstitution += int.Parse(SummonerInitAttrArr[3]);
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
