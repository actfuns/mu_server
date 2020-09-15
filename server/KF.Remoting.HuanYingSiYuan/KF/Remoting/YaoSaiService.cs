using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000029 RID: 41
	public class YaoSaiService
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x0001966C File Offset: 0x0001786C
		public static YaoSaiService Instance()
		{
			return YaoSaiService._instance;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00019684 File Offset: 0x00017884
		public void InitConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string StringManorCommandAward = KuaFuServerManager.systemParamsList.GetParamValueByName("ManorCommandAward");
					string[] ManorCommandFields = StringManorCommandAward.Split(new char[]
					{
						','
					});
					if (ManorCommandFields.Length == 3)
					{
						this.RuntimeData.FuLuHuDongTimes = Global.SafeConvertToInt32(ManorCommandFields[0]);
						this.RuntimeData.FuLuAwardTimes = Global.SafeConvertToInt32(ManorCommandFields[1]);
						this.RuntimeData.FuLuHuDongMinutes = Global.SafeConvertToInt32(ManorCommandFields[2]);
					}
					string fileName = "Config/ManorLevel.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						YaoSaiJianYuManorLevelConfig myData = new YaoSaiJianYuManorLevelConfig();
						myData.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						string tempValue = ConfigHelper.GetElementAttributeValue(xmlItem, "MinLevel", "");
						string[] ValueFields = tempValue.Split(new char[]
						{
							'|'
						});
						if (ValueFields.Length == 2)
						{
							myData.MinZhuanSheng = Global.SafeConvertToInt32(ValueFields[0]);
							myData.MinLevel = Global.SafeConvertToInt32(ValueFields[1]);
						}
						tempValue = ConfigHelper.GetElementAttributeValue(xmlItem, "MaxLevel", "");
						ValueFields = tempValue.Split(new char[]
						{
							'|'
						});
						if (ValueFields.Length == 2)
						{
							myData.MaxZhuanSheng = Global.SafeConvertToInt32(ValueFields[0]);
							myData.MaxLevel = Global.SafeConvertToInt32(ValueFields[1]);
						}
						this.RuntimeData.YaoSaiJianYuLevelDict[myData.ID] = myData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00019900 File Offset: 0x00017B00
		public KuaFuCmdData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				long nowTicks = TimeUtil.NOW();
				long maxSearchTicks = long.MinValue;
				KFPrisonRoleAllData roleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == roleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonRoleAllData myResultData = null;
					KuaFuData<KFPrisonRoleData> srcData = roleAllData.kfRoleData;
					int oldLevelID = this.GetYaoSaiLevelID(srcData.V.UnionLevel);
					int newLevelID = this.GetYaoSaiLevelID(unionlev);
					srcData.V.Faction = faction;
					if (oldLevelID != newLevelID)
					{
						srcData.V.UnionLevel = unionlev;
						this.UpdateYaoSaiPrisonSearchData(roleAllData, oldLevelID);
						this.SaveYaoSaiPrisonRoleDataDB(srcData.V, null);
					}
					List<KFPrisonRoleAllData> searchList;
					if (!this.YaoSaiSearchDataDict.TryGetValue(newLevelID, out searchList))
					{
						result = null;
					}
					else
					{
						for (int randomLoop = 0; randomLoop < 5; randomLoop++)
						{
							KFPrisonRoleAllData searchItem = searchList[Global.GetRandomNumber(0, searchList.Count)];
							if (this.YaoSaiSearchItemCheck(srcData.V, searchItem.kfRoleData.V, frindSet))
							{
								myResultData = searchItem;
							}
						}
						if (null == myResultData)
						{
							foreach (KFPrisonRoleAllData searchItem in searchList)
							{
								if (this.YaoSaiSearchItemCheck(srcData.V, searchItem.kfRoleData.V, frindSet))
								{
									long curSearchTicks = nowTicks - searchItem.SearchTimeStamp;
									if (curSearchTicks >= 60000L)
									{
										myResultData = searchItem;
										break;
									}
									if (curSearchTicks > maxSearchTicks)
									{
										maxSearchTicks = curSearchTicks;
										myResultData = searchItem;
									}
								}
							}
						}
						if (null != myResultData)
						{
							myResultData.SearchTimeStamp = nowTicks;
							result = new KuaFuCmdData
							{
								Age = myResultData.kfRoleData.Age,
								Bytes0 = DataHelper2.ObjectToBytes<KFPrisonRoleData>(myResultData.kfRoleData.V)
							};
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00019B6C File Offset: 0x00017D6C
		public KuaFuCmdData GetYaoSaiPrisonRoleData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData roleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == roleAllData)
				{
					result = null;
				}
				else if (dataAge != roleAllData.kfRoleData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = roleAllData.kfRoleData.Age,
						Bytes0 = DataHelper2.ObjectToBytes<KFPrisonRoleData>(roleAllData.kfRoleData.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = roleAllData.kfRoleData.Age
					};
				}
			}
			return result;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00019C34 File Offset: 0x00017E34
		public KuaFuCmdData GetYaoSaiFuLuListData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (0 == rid)
				{
					result = null;
				}
				else
				{
					KFPrisonFuLuAllData data = this.GetYaoSaiPrisonFuLuAllData(rid, true);
					if (null == data)
					{
						result = null;
					}
					else if (dataAge != data.fuluData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = data.fuluData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<List<KFPrisonRoleData>>(data.fuluData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = data.fuluData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00019D14 File Offset: 0x00017F14
		public KuaFuCmdData GetYaoSaiPrisonLogData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData roleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == roleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonLogAllData logAllData = null;
					if (!this.YaoSaiPrisonLogDataDict.TryGetValue(rid, out logAllData))
					{
						logAllData = new KFPrisonLogAllData();
						if (!this.LoadYaoSaiPrisonLogList(rid, logAllData.LogListData.V))
						{
							return null;
						}
						this.YaoSaiPrisonLogDataDict[rid] = logAllData;
						TimeUtil.AgeByNow(ref logAllData.LogListData.Age);
					}
					logAllData.LogDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
					if (dataAge != logAllData.LogListData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = logAllData.LogListData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<List<KFPrisonLogData>>(logAllData.LogListData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = logAllData.LogListData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00019E68 File Offset: 0x00018068
		public KuaFuCmdData GetYaoSaiPrisonJingJiData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData roleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == roleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonJingJiAllData prisonJingJi = null;
					if (!this.YaoSaiPrisonJingJiDataDict.TryGetValue(rid, out prisonJingJi))
					{
						prisonJingJi = new KFPrisonJingJiAllData();
						if (!this.LoadYaoSaiPrisonJingJiData(rid, prisonJingJi.JingJiData.V))
						{
							return null;
						}
						this.YaoSaiPrisonJingJiDataDict[rid] = prisonJingJi;
						prisonJingJi.JingJiDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						TimeUtil.AgeByNow(ref prisonJingJi.JingJiData.Age);
					}
					if (dataAge != prisonJingJi.JingJiData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = prisonJingJi.JingJiData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFPrisonJingJiData>(prisonJingJi.JingJiData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = prisonJingJi.JingJiData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0001A044 File Offset: 0x00018244
		public int YaoSaiPrisonOpt(int srcrid, int targetid, int type, bool success)
		{
			int result;
			lock (this.Mutex)
			{
				if (type <= -3 || type >= 3)
				{
					result = -1;
				}
				else
				{
					KFPrisonRoleAllData srcAllData = this.GetYaoSaiPrisonRoleAllData(srcrid, true);
					if (null == srcAllData)
					{
						result = -1;
					}
					else
					{
						KFPrisonRoleAllData tarAllData = this.GetYaoSaiPrisonRoleAllData(targetid, true);
						if (null == tarAllData)
						{
							result = -1;
						}
						else
						{
							KuaFuData<KFPrisonRoleData> srcData = srcAllData.kfRoleData;
							KuaFuData<KFPrisonRoleData> tarData = tarAllData.kfRoleData;
							if (-2 == type)
							{
								if (srcrid != tarData.V.OwnerID)
								{
									return -1;
								}
								KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
								if (null != fuluAllData)
								{
									KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
									KFPrisonRoleData fuluData = fuluListData.V.Find((KFPrisonRoleData x) => x.RoleID == targetid);
									if (null != fuluData)
									{
										fuluListData.V.Remove(fuluData);
										TimeUtil.AgeByNow(ref fuluListData.Age);
									}
								}
								tarData.V.OwnerID = 0;
								TimeUtil.AgeByNow(ref tarData.Age);
								this.SaveYaoSaiPrisonRoleDataDB(tarData.V, null);
							}
							else if (-1 == type)
							{
								if (srcData.V.OwnerID == 0)
								{
									return -1;
								}
								if (srcData.V.OwnerID != targetid)
								{
									tarAllData = this.GetYaoSaiPrisonRoleAllData(srcData.V.OwnerID, true);
									if (null == tarAllData)
									{
										return -1;
									}
									tarData = tarAllData.kfRoleData;
								}
								if (success)
								{
									KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(srcData.V.OwnerID, false);
									if (null != fuluAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuLuListData = fuluAllData.fuluData;
										fuLuListData.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == srcrid);
										TimeUtil.AgeByNow(ref fuLuListData.Age);
									}
									srcData.V.OwnerID = 0;
									TimeUtil.AgeByNow(ref srcData.Age);
									this.SaveYaoSaiPrisonRoleDataDB(srcData.V, null);
								}
								KFPrisonLogData srcLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(srcLog, false);
								KFPrisonLogData targetLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = tarData.V.RoleID,
									Name1 = srcData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(targetLog, false);
							}
							else if (0 == type)
							{
								if (srcData.V.OwnerID == targetid)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, -1, success);
								}
								if (tarData.V.OwnerID != 0)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, 1, success);
								}
								if (success)
								{
									KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
									if (null != fuluAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
										fuluListData.V.Add(tarData.V);
										TimeUtil.AgeByNow(ref fuluListData.Age);
									}
									tarData.V.OwnerID = srcrid;
									tarData.V.FuLuTime = TimeUtil.NowDateTime().Ticks;
									TimeUtil.AgeByNow(ref tarData.Age);
									this.SaveYaoSaiPrisonRoleDataDB(tarData.V, null);
								}
								KFPrisonLogData srcLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = tarData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(srcLog, false);
								KFPrisonLogData targetLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = srcData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(targetLog, false);
							}
							else if (1 == type)
							{
								if (srcrid == tarData.V.OwnerID)
								{
									return -1;
								}
								if (srcData.V.OwnerID == targetid)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, -1, success);
								}
								if (tarData.V.OwnerID == 0)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, 0, success);
								}
								KFPrisonLogData thrLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 2, success),
									RoleID = tarData.V.OwnerID,
									Name1 = srcData.V.RoleName,
									Name2 = tarData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(thrLog, false);
								if (success)
								{
									KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(tarData.V.OwnerID, false);
									if (null != fuluAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
										fuluListData.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == targetid);
										TimeUtil.AgeByNow(ref fuluListData.Age);
									}
									fuluAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
									if (null != fuluAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
										fuluListData.V.Add(tarData.V);
										TimeUtil.AgeByNow(ref fuluListData.Age);
									}
									tarData.V.OwnerID = srcrid;
									tarData.V.FuLuTime = TimeUtil.NowDateTime().Ticks;
									TimeUtil.AgeByNow(ref tarData.Age);
									this.SaveYaoSaiPrisonRoleDataDB(tarData.V, null);
								}
								KFPrisonLogData srcLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = tarData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(srcLog, false);
								KFPrisonLogData targetLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = srcData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(targetLog, false);
							}
							else if (2 == type)
							{
								if (srcrid == tarData.V.OwnerID)
								{
									return -1;
								}
								if (0 == tarData.V.OwnerID)
								{
									return -1;
								}
								KFPrisonLogData thrLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 2, success),
									RoleID = tarData.V.OwnerID,
									Name1 = srcData.V.RoleName,
									Name2 = tarData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(thrLog, false);
								if (success)
								{
									KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(tarData.V.OwnerID, false);
									if (null != fuluAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
										fuluListData.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == targetid);
										TimeUtil.AgeByNow(ref fuluListData.Age);
									}
									tarData.V.OwnerID = 0;
									TimeUtil.AgeByNow(ref tarData.Age);
									this.SaveYaoSaiPrisonRoleDataDB(tarData.V, null);
								}
								KFPrisonLogData srcLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = tarData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(srcLog, false);
								KFPrisonLogData targetLog = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = srcData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(targetLog, false);
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0001AA00 File Offset: 0x00018C00
		private void UpdateYaoSaiPrisonSearchData(KFPrisonRoleAllData allData, int oldLevID)
		{
			int newLevID = this.GetYaoSaiLevelID(allData.kfRoleData.V.UnionLevel);
			if (allData.SearchDataIndex == -1)
			{
				List<KFPrisonRoleAllData> searchList = null;
				if (!this.YaoSaiSearchDataDict.TryGetValue(newLevID, out searchList))
				{
					searchList = new List<KFPrisonRoleAllData>();
					this.YaoSaiSearchDataDict[newLevID] = searchList;
				}
				allData.SearchDataIndex = searchList.Count;
				searchList.Add(allData);
			}
			else if (oldLevID != newLevID)
			{
				List<KFPrisonRoleAllData> oldList = null;
				if (!this.YaoSaiSearchDataDict.TryGetValue(oldLevID, out oldList))
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞搜索数据异常 RoleID={0} OldLevID={1} NewUnionLev={2}", allData.kfRoleData.V.RoleID, oldLevID, allData.kfRoleData.V.UnionLevel), null, true);
				}
				else
				{
					List<KFPrisonRoleAllData> newList = null;
					if (!this.YaoSaiSearchDataDict.TryGetValue(newLevID, out newList))
					{
						newList = new List<KFPrisonRoleAllData>();
						this.YaoSaiSearchDataDict[newLevID] = newList;
					}
					oldList[oldList.Count - 1].SearchDataIndex = allData.SearchDataIndex;
					KFPrisonRoleAllData tmpItem = oldList[allData.SearchDataIndex];
					oldList[allData.SearchDataIndex] = oldList[oldList.Count - 1];
					oldList[oldList.Count - 1] = tmpItem;
					oldList.RemoveAt(oldList.Count - 1);
					allData.SearchDataIndex = newList.Count;
					newList.Add(allData);
				}
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0001AB90 File Offset: 0x00018D90
		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole updateData)
		{
			lock (this.Mutex)
			{
				if (this.GetYaoSaiLevelID(updateData.UnionLevel) == -1)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞异常数据更新 RoleID={0} RoleName={1} ZoneID={2} UnionLev={3}", new object[]
					{
						updateData.RoleID,
						updateData.RoleName,
						updateData.ZoneID,
						updateData.UnionLevel
					}), null, true);
					return -1;
				}
				KFPrisonRoleAllData roleAllData = this.GetYaoSaiPrisonRoleAllData(updateData.RoleID, true);
				KuaFuData<KFPrisonRoleData> oldData;
				if (null == roleAllData)
				{
					roleAllData = new KFPrisonRoleAllData();
					this.YaoSaiPrisonRoleDataDict[updateData.RoleID] = roleAllData;
					oldData = roleAllData.kfRoleData;
				}
				else
				{
					oldData = roleAllData.kfRoleData;
				}
				int oldLevID = this.GetYaoSaiLevelID(oldData.V.UnionLevel);
				oldData.V.RoleID = updateData.RoleID;
				oldData.V.RoleName = updateData.RoleName;
				oldData.V.UnionLevel = updateData.UnionLevel;
				oldData.V.Faction = updateData.Faction;
				oldData.V.RoleSex = updateData.RoleSex;
				oldData.V.Occupation = updateData.Occupation;
				oldData.V.CombatForce = updateData.CombatForce;
				oldData.V.ZoneID = updateData.ZoneID;
				oldData.V.OptTime = TimeUtil.NowDateTime().Ticks;
				TimeUtil.AgeByNow(ref oldData.Age);
				roleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
				KFPrisonJingJiAllData prisonJingJi = null;
				if (this.YaoSaiPrisonJingJiDataDict.TryGetValue(updateData.RoleID, out prisonJingJi))
				{
					prisonJingJi.JingJiData.V.PlayerJingJiMirrorData = updateData.PlayerJingJiMirrorData;
					TimeUtil.AgeByNow(ref prisonJingJi.JingJiData.Age);
				}
				this.UpdateYaoSaiPrisonSearchData(roleAllData, oldLevID);
				if (oldData.V.OwnerID != 0)
				{
					KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(oldData.V.OwnerID, false);
					if (null != fuluAllData)
					{
						KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
						TimeUtil.AgeByNow(ref fuluListData.Age);
					}
				}
				this.SaveYaoSaiPrisonRoleDataDB(oldData.V, updateData.PlayerJingJiMirrorData);
			}
			return 0;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0001AE3C File Offset: 0x0001903C
		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			int result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData ownerAllData = this.GetYaoSaiPrisonRoleAllData(ownerid, true);
				if (null == ownerAllData)
				{
					result = -1;
				}
				else
				{
					KFPrisonRoleAllData fuluAllData = this.GetYaoSaiPrisonRoleAllData(fuluid, true);
					if (null == fuluAllData)
					{
						result = -1;
					}
					else
					{
						KuaFuData<KFPrisonRoleData> ownerRoleData = ownerAllData.kfRoleData;
						KuaFuData<KFPrisonRoleData> fuluRoleData = fuluAllData.kfRoleData;
						if (fuluRoleData.V.OwnerID != ownerid)
						{
							result = -1;
						}
						else
						{
							int dayId = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
							if (dayId != fuluRoleData.V.CountDayID)
							{
								fuluRoleData.V.CountDayID = dayId;
								fuluRoleData.V.AwardCount = 0;
							}
							if (fuluRoleData.V.AwardCount >= this.RuntimeData.FuLuAwardTimes)
							{
								KFPrisonLogData beHuDongLog = new KFPrisonLogData
								{
									IntroID = this.TransHuDongTypeToLogType(type, false, false),
									RoleID = fuluRoleData.V.RoleID,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(beHuDongLog, false);
							}
							else
							{
								KFPrisonLogData beHuDongLog = new KFPrisonLogData
								{
									IntroID = this.TransHuDongTypeToLogType(type, false, true),
									RoleID = fuluRoleData.V.RoleID,
									Param1 = param0,
									Param2 = param2,
									State = 0
								};
								this.AddYaoSaiPrisonLogData(beHuDongLog, true);
								fuluRoleData.V.AwardCount++;
							}
							KFPrisonLogData HuDongLog = new KFPrisonLogData
							{
								IntroID = this.TransHuDongTypeToLogType(type, true, true),
								RoleID = ownerid,
								Name1 = fuluRoleData.V.RoleName,
								Param1 = param0,
								Param2 = param1,
								State = 1
							};
							this.AddYaoSaiPrisonLogData(HuDongLog, false);
							TimeUtil.AgeByNow(ref fuluRoleData.Age);
							this.SaveYaoSaiPrisonRoleDataDB(fuluRoleData.V, null);
							result = fuluRoleData.V.AwardCount;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0001B0A8 File Offset: 0x000192A8
		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			lock (this.Mutex)
			{
				string sql = string.Format("UPDATE `t_yaosai_prison_log` SET `state`={0} WHERE `id`={1}", state, id);
				this.ExecuteSqlNoQuery(sql);
				KFPrisonLogAllData logAllData = null;
				if (!this.YaoSaiPrisonLogDataDict.TryGetValue(rid, out logAllData))
				{
					return 0;
				}
				KFPrisonLogData logData = logAllData.LogListData.V.Find((KFPrisonLogData x) => x.ID == id);
				if (null != logData)
				{
					logData.State = state;
					TimeUtil.AgeByNow(ref logAllData.LogListData.Age);
				}
			}
			return 0;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0001B1C0 File Offset: 0x000193C0
		private KFPrisonRoleAllData GetYaoSaiPrisonRoleAllData(int roleID, bool loadFromDB = true)
		{
			KFPrisonRoleAllData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData allData = null;
				if (this.YaoSaiPrisonRoleDataDict.TryGetValue(roleID, out allData))
				{
					result = allData;
				}
				else if (!loadFromDB)
				{
					result = allData;
				}
				else
				{
					YaoSaiSearchParam param = new YaoSaiSearchParam
					{
						roleID = roleID,
						loadFuLu = false
					};
					List<KFPrisonRoleData> roleDataList = this.LoadYaoSaiPrisonRoleList(param);
					if (roleDataList == null || roleDataList.Count == 0)
					{
						result = null;
					}
					else
					{
						allData = new KFPrisonRoleAllData();
						if (roleDataList[0].OwnerID != 0)
						{
							KFPrisonFuLuAllData fuluAllData = this.GetYaoSaiPrisonFuLuAllData(roleDataList[0].OwnerID, false);
							if (null != fuluAllData)
							{
								KuaFuData<List<KFPrisonRoleData>> fuluListData = fuluAllData.fuluData;
								KFPrisonRoleData fuluData = fuluListData.V.Find((KFPrisonRoleData x) => x.RoleID == roleID);
								if (null != fuluData)
								{
									roleDataList[0] = fuluData;
								}
							}
						}
						allData.kfRoleData.V = roleDataList[0];
						TimeUtil.AgeByNow(ref allData.kfRoleData.Age);
						allData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						this.YaoSaiPrisonRoleDataDict[roleID] = allData;
						this.UpdateYaoSaiPrisonSearchData(allData, this.GetYaoSaiLevelID(allData.kfRoleData.V.UnionLevel));
						result = allData;
					}
				}
			}
			return result;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0001B3A0 File Offset: 0x000195A0
		private KFPrisonFuLuAllData GetYaoSaiPrisonFuLuAllData(int roleID, bool loadFromDB = true)
		{
			KFPrisonFuLuAllData result;
			lock (this.Mutex)
			{
				KFPrisonFuLuAllData allData = null;
				if (this.YaoSaiOwnerIDVsFuLuDict.TryGetValue(roleID, out allData))
				{
					result = allData;
				}
				else if (!loadFromDB)
				{
					result = allData;
				}
				else
				{
					YaoSaiSearchParam param = new YaoSaiSearchParam
					{
						roleID = roleID,
						loadFuLu = true
					};
					List<KFPrisonRoleData> fuluDataList = this.LoadYaoSaiPrisonRoleList(param);
					if (fuluDataList == null || fuluDataList.Count == 0)
					{
						result = null;
					}
					else
					{
						allData = new KFPrisonFuLuAllData();
						foreach (KFPrisonRoleData fuluItem in fuluDataList)
						{
							KFPrisonRoleAllData roldAllData = null;
							if (!this.YaoSaiPrisonRoleDataDict.TryGetValue(fuluItem.RoleID, out roldAllData))
							{
								roldAllData = new KFPrisonRoleAllData();
								roldAllData.kfRoleData.V = fuluItem;
								roldAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
								TimeUtil.AgeByNow(ref roldAllData.kfRoleData.Age);
								this.YaoSaiPrisonRoleDataDict[fuluItem.RoleID] = roldAllData;
								this.UpdateYaoSaiPrisonSearchData(roldAllData, this.GetYaoSaiLevelID(roldAllData.kfRoleData.V.UnionLevel));
							}
							allData.fuluData.V.Add(roldAllData.kfRoleData.V);
						}
						allData.DataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						TimeUtil.AgeByNow(ref allData.fuluData.Age);
						this.YaoSaiOwnerIDVsFuLuDict[roleID] = allData;
						result = allData;
					}
				}
			}
			return result;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0001B604 File Offset: 0x00019804
		public void CheckYaoSaiPrisonTimerProc(DateTime now)
		{
			if (this.YaoSaiPrisonJingJiDataDict.Count != 0 || this.YaoSaiPrisonLogDataDict.Count != 0 || this.YaoSaiSearchDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<int> removeKeyList = new List<int>();
					foreach (KeyValuePair<int, List<KFPrisonRoleAllData>> KVP in this.YaoSaiSearchDataDict)
					{
						List<KFPrisonRoleAllData> searchList = KVP.Value;
						if (searchList.Count <= 20000)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("要塞搜索数据清理 LevelID={0}, TotalNum={1}, RemoveNum={2}, LeftNum={3}", new object[]
							{
								KVP.Key,
								searchList.Count,
								0,
								searchList.Count
							}), null, true);
						}
						else
						{
							searchList.Sort(delegate(KFPrisonRoleAllData left, KFPrisonRoleAllData right)
							{
								int result;
								if (left.RoleDataEndTime > right.RoleDataEndTime)
								{
									result = -1;
								}
								else if (left.RoleDataEndTime < right.RoleDataEndTime)
								{
									result = 1;
								}
								else
								{
									result = 0;
								}
								return result;
							});
							for (int i = 0; i < searchList.Count; i++)
							{
								searchList[i].SearchDataIndex = i;
								if (i >= 20000)
								{
									removeKeyList.Add(searchList[i].kfRoleData.V.RoleID);
								}
							}
							LogManager.WriteLog(LogTypes.Info, string.Format("要塞搜索数据清理 LevelID={0}, TotalNum={1}, RemoveNum={2}, LeftNum={3}", new object[]
							{
								KVP.Key,
								searchList.Count,
								searchList.Count - 20000,
								20000
							}), null, true);
							searchList.RemoveRange(20000, searchList.Count - 20000);
						}
					}
					foreach (int key in removeKeyList)
					{
						this.YaoSaiPrisonRoleDataDict.Remove(key);
					}
					removeKeyList.Clear();
					foreach (KeyValuePair<int, KFPrisonFuLuAllData> fuluItem in this.YaoSaiOwnerIDVsFuLuDict)
					{
						if (fuluItem.Value.DataEndTime < now)
						{
							removeKeyList.Add(fuluItem.Key);
						}
					}
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞俘虏数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiOwnerIDVsFuLuDict.Count, removeKeyList.Count, this.YaoSaiOwnerIDVsFuLuDict.Count - removeKeyList.Count), null, true);
					foreach (int key in removeKeyList)
					{
						this.YaoSaiOwnerIDVsFuLuDict.Remove(key);
					}
					removeKeyList.Clear();
					foreach (KeyValuePair<int, KFPrisonJingJiAllData> jingjiItem in this.YaoSaiPrisonJingJiDataDict)
					{
						if (jingjiItem.Value.JingJiDataEndTime < now)
						{
							removeKeyList.Add(jingjiItem.Key);
						}
					}
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞竞技数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiPrisonJingJiDataDict.Count, removeKeyList.Count, this.YaoSaiPrisonJingJiDataDict.Count - removeKeyList.Count), null, true);
					foreach (int key in removeKeyList)
					{
						this.YaoSaiPrisonJingJiDataDict.Remove(key);
					}
					removeKeyList.Clear();
					foreach (KeyValuePair<int, KFPrisonLogAllData> logItem in this.YaoSaiPrisonLogDataDict)
					{
						if (logItem.Value.LogDataEndTime < now)
						{
							removeKeyList.Add(logItem.Key);
						}
					}
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞日志数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiPrisonLogDataDict.Count, removeKeyList.Count, this.YaoSaiPrisonLogDataDict.Count - removeKeyList.Count), null, true);
					foreach (int key in removeKeyList)
					{
						this.YaoSaiPrisonLogDataDict.Remove(key);
					}
				}
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0001BC20 File Offset: 0x00019E20
		private bool YaoSaiSearchItemCheck(KFPrisonRoleData srcData, KFPrisonRoleData searchItem, HashSet<int> frindSet)
		{
			return srcData.RoleID != searchItem.RoleID && srcData.RoleID != searchItem.OwnerID && srcData.OwnerID != searchItem.RoleID && (srcData.Faction == 0 || srcData.Faction != searchItem.Faction) && (frindSet == null || !frindSet.Contains(searchItem.RoleID));
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001BCB0 File Offset: 0x00019EB0
		private void AddYaoSaiPrisonLogData(KFPrisonLogData data, bool broadCast = false)
		{
			lock (this.Mutex)
			{
				KFPrisonLogAllData logAllData = null;
				if (this.YaoSaiPrisonLogDataDict.TryGetValue(data.RoleID, out logAllData))
				{
					logAllData.LogListData.V.Add(data);
					TimeUtil.AgeByNow(ref logAllData.LogListData.Age);
					if (logAllData.LogListData.V.Count > 30)
					{
						int removeNum = logAllData.LogListData.V.Count - 30;
						for (int loop = 0; loop < removeNum; loop++)
						{
							this.DeleteYaoSaiPrisonLogData(logAllData.LogListData.V[loop]);
						}
						logAllData.LogListData.V.RemoveRange(0, removeNum);
					}
				}
				this.InsertYaoSaiPrisonLogData(data);
				if (broadCast)
				{
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.SyncYaoSaiLogData, new object[]
					{
						data.RoleID
					});
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, evItem, 0);
				}
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0001BE04 File Offset: 0x0001A004
		public int GetYaoSaiLevelID(int unionlev)
		{
			int levelID = -1;
			lock (this.RuntimeData.Mutex)
			{
				foreach (YaoSaiJianYuManorLevelConfig item in this.RuntimeData.YaoSaiJianYuLevelDict.Values)
				{
					int minUnionLev = item.MinZhuanSheng * 100 + item.MinLevel;
					int maxUnionLev = item.MaxZhuanSheng * 100 + item.MaxLevel;
					if (unionlev >= minUnionLev && unionlev <= maxUnionLev)
					{
						levelID = item.ID;
						break;
					}
				}
			}
			return levelID;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0001BEEC File Offset: 0x0001A0EC
		public int TransOptTypeToLogType(int type, int src_tar_thr, bool success)
		{
			int LogType = 0;
			switch (type)
			{
			case -2:
				if (1 == src_tar_thr)
				{
				}
				break;
			case -1:
				if (1 == src_tar_thr)
				{
					if (success)
					{
						LogType = 19;
					}
					else
					{
						LogType = 18;
					}
				}
				else if (success)
				{
					LogType = 11;
				}
				else
				{
					LogType = 12;
				}
				break;
			case 0:
				if (1 == src_tar_thr)
				{
					if (success)
					{
						LogType = 1;
					}
					else
					{
						LogType = 2;
					}
				}
				else if (success)
				{
					LogType = 14;
				}
				else
				{
					LogType = 13;
				}
				break;
			case 1:
				if (0 == src_tar_thr)
				{
					if (success)
					{
						LogType = 14;
					}
					else
					{
						LogType = 13;
					}
				}
				else if (1 == src_tar_thr)
				{
					if (success)
					{
						LogType = 1;
					}
					else
					{
						LogType = 2;
					}
				}
				else if (2 == src_tar_thr)
				{
					if (success)
					{
						LogType = 21;
					}
					else
					{
						LogType = 20;
					}
				}
				break;
			case 2:
				if (0 == src_tar_thr)
				{
					if (success)
					{
						LogType = 23;
					}
					else
					{
						LogType = 22;
					}
				}
				else if (1 == src_tar_thr)
				{
					if (success)
					{
						LogType = 10;
					}
					else
					{
						LogType = 9;
					}
				}
				else if (2 == src_tar_thr)
				{
					if (success)
					{
						LogType = 24;
					}
					else
					{
						LogType = 25;
					}
				}
				break;
			default:
				LogType = 0;
				break;
			}
			return LogType;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0001C06C File Offset: 0x0001A26C
		public int TransHuDongTypeToLogType(int type, bool owner, bool success)
		{
			int LogType;
			switch (type)
			{
			case 1:
				if (!owner)
				{
					if (success)
					{
						LogType = 3;
					}
					else
					{
						LogType = 6;
					}
				}
				else
				{
					LogType = 15;
				}
				break;
			case 2:
				if (!owner)
				{
					if (success)
					{
						LogType = 4;
					}
					else
					{
						LogType = 7;
					}
				}
				else
				{
					LogType = 16;
				}
				break;
			case 3:
				if (!owner)
				{
					if (success)
					{
						LogType = 5;
					}
					else
					{
						LogType = 8;
					}
				}
				else
				{
					LogType = 17;
				}
				break;
			default:
				LogType = 0;
				break;
			}
			return LogType;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0001C100 File Offset: 0x0001A300
		public void LoadYaoSaiData()
		{
			long lastTicks = TimeUtil.NOW();
			foreach (YaoSaiJianYuManorLevelConfig levConfig in this.RuntimeData.YaoSaiJianYuLevelDict.Values)
			{
				YaoSaiSearchParam param = new YaoSaiSearchParam
				{
					MinUnionLev = levConfig.MinZhuanSheng * 100 + levConfig.MinLevel,
					MaxUnionLev = levConfig.MaxZhuanSheng * 100 + levConfig.MaxLevel,
					loadFuLu = false
				};
				List<KFPrisonRoleData> roleList = this.LoadYaoSaiPrisonRoleList(param);
				if (roleList == null || roleList.Count == 0)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞监狱数据加载 LevelID={0} CountNum={1}", levConfig.ID, 0), null, true);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("要塞监狱数据加载 LevelID={0} CountNum={1}", levConfig.ID, roleList.Count), null, true);
					foreach (KFPrisonRoleData prisonRole in roleList)
					{
						KFPrisonRoleAllData allData = new KFPrisonRoleAllData();
						allData.kfRoleData.V = prisonRole;
						TimeUtil.AgeByNow(ref allData.kfRoleData.Age);
						allData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						this.YaoSaiPrisonRoleDataDict[prisonRole.RoleID] = allData;
						this.UpdateYaoSaiPrisonSearchData(allData, this.GetYaoSaiLevelID(prisonRole.UnionLevel));
					}
				}
			}
			LogManager.WriteLog(LogTypes.Info, string.Format("要塞监狱数据加载 TakeTime={0}ms", TimeUtil.NOW() - lastTicks), null, true);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0001C30C File Offset: 0x0001A50C
		private bool LoadYaoSaiPrisonJingJiData(int roleID, KFPrisonJingJiData data)
		{
			try
			{
				string strSql = string.Format("SELECT roledata FROM t_yaosai_prison WHERE rid={0}", roleID);
				object jingjiObj = DbHelperMySQL.GetSingle(strSql);
				if (null != jingjiObj)
				{
					data.PlayerJingJiMirrorData = (jingjiObj as byte[]);
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return false;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0001C37C File Offset: 0x0001A57C
		private List<KFPrisonRoleData> LoadYaoSaiPrisonRoleList(YaoSaiSearchParam loadParam)
		{
			List<KFPrisonRoleData> list = new List<KFPrisonRoleData>();
			MySqlDataReader sdr = null;
			List<KFPrisonRoleData> result;
			try
			{
				string strSql;
				if (loadParam.MinUnionLev != 0 || loadParam.MaxUnionLev != 0)
				{
					int loadMaxNum = 12000;
					strSql = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE unionLevel>={0} AND unionLevel<={1} ORDER BY opttime DESC LIMIT 0,{2}", loadParam.MinUnionLev, loadParam.MaxUnionLev, loadMaxNum);
				}
				else if (!loadParam.loadFuLu)
				{
					strSql = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE rid={0}", loadParam.roleID);
				}
				else
				{
					strSql = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE ownerid={0}", loadParam.roleID);
				}
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					KFPrisonRoleData data = new KFPrisonRoleData();
					data.RoleID = Convert.ToInt32(sdr[0].ToString());
					data.RoleName = sdr[1].ToString();
					data.UnionLevel = Convert.ToInt32(sdr[2].ToString());
					data.Faction = Convert.ToInt32(sdr[3].ToString());
					data.RoleSex = Convert.ToByte(sdr[4].ToString());
					data.Occupation = Convert.ToByte(sdr[5].ToString());
					data.CombatForce = Convert.ToInt32(sdr[6].ToString());
					data.ZoneID = Convert.ToInt32(sdr[7].ToString());
					data.AwardCount = Convert.ToInt32(sdr[8].ToString());
					data.CountDayID = Convert.ToInt32(sdr[9].ToString());
					data.OwnerID = Convert.ToInt32(sdr[10].ToString());
					DateTime tempDataTime;
					DateTime.TryParse(sdr[11].ToString(), out tempDataTime);
					data.FuLuTime = tempDataTime.Ticks;
					DateTime.TryParse(sdr[12].ToString(), out tempDataTime);
					data.OptTime = tempDataTime.Ticks;
					list.Add(data);
					index++;
				}
				result = list;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return result;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0001C608 File Offset: 0x0001A808
		private bool LoadYaoSaiPrisonLogList(int RoleID, List<KFPrisonLogData> LogListData)
		{
			MySqlDataReader sdr = null;
			bool result;
			try
			{
				string strSql = string.Format("SELECT id,roleid,introid,param1,param2,name1,name2,state \r\n                            FROM t_yaosai_prison_log WHERE roleid={0} ORDER BY id DESC LIMIT {1};", RoleID, 30);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					LogListData.Add(new KFPrisonLogData
					{
						ID = Convert.ToInt64(sdr[0].ToString()),
						RoleID = Convert.ToInt32(sdr[1].ToString()),
						IntroID = Convert.ToInt32(sdr[2].ToString()),
						Param1 = Convert.ToInt32(sdr[3].ToString()),
						Param2 = Convert.ToInt32(sdr[4].ToString()),
						Name1 = sdr[5].ToString(),
						Name2 = sdr[6].ToString(),
						State = Convert.ToInt32(sdr[7].ToString())
					});
					index++;
				}
				LogListData.Reverse();
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return result;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0001C780 File Offset: 0x0001A980
		private void SaveYaoSaiPrisonRoleDataDB(KFPrisonRoleData data, byte[] PlayerJingJiMirrorData = null)
		{
			DateTime fuluDataTime = new DateTime(data.FuLuTime);
			DateTime optDateTime = new DateTime(data.OptTime);
			if (null != PlayerJingJiMirrorData)
			{
				string sql = string.Format("INSERT INTO `t_yaosai_prison` (`rid`,`rname`,`unionLevel`,`faction`,`sex`,`occupation`,`combatforce`,`zoneid`,`awardCount`,`countDayId`,`ownerid`,`fulutime`,`opttime`,`roledata`) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},'{11}','{12}',@content) on duplicate key update `rname`='{1}',`unionLevel`={2},`faction`={3},`sex`={4},`occupation`={5},`combatforce`={6},`zoneid`={7},`awardCount`={8}, `countDayId`={9},`ownerid`={10},`fulutime`='{11}',`opttime`='{12}',`roledata`=@content;", new object[]
				{
					data.RoleID,
					data.RoleName,
					data.UnionLevel,
					data.Faction,
					data.RoleSex,
					data.Occupation,
					data.CombatForce,
					data.ZoneID,
					data.AwardCount,
					data.CountDayID,
					data.OwnerID,
					fuluDataTime.ToString("yyyy-MM-dd HH:mm:ss"),
					optDateTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", PlayerJingJiMirrorData)
				});
			}
			else
			{
				string sql = string.Format("UPDATE `t_yaosai_prison` SET `rname`='{1}',`unionLevel`={2},`faction`={3},`sex`={4},`occupation`={5},`combatforce`={6},`zoneid`={7},`awardCount`={8},`countDayId`={9},`ownerid`={10},`fulutime`='{11}',`opttime`='{12}' WHERE rid={0}", new object[]
				{
					data.RoleID,
					data.RoleName,
					data.UnionLevel,
					data.Faction,
					data.RoleSex,
					data.Occupation,
					data.CombatForce,
					data.ZoneID,
					data.AwardCount,
					data.CountDayID,
					data.OwnerID,
					fuluDataTime.ToString("yyyy-MM-dd HH:mm:ss"),
					optDateTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				this.ExecuteSqlNoQuery(sql);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0001C99C File Offset: 0x0001AB9C
		private void InsertYaoSaiPrisonLogData(KFPrisonLogData data)
		{
			string sql = string.Format("INSERT INTO `t_yaosai_prison_log` (`roleid`,`introid`,`param1`,`param2`,`name1`,`name2`,`state`) VALUES ({0},{1},{2},{3},'{4}','{5}',{6});", new object[]
			{
				data.RoleID,
				data.IntroID,
				data.Param1,
				data.Param2,
				data.Name1,
				data.Name2,
				data.State
			});
			data.ID = this.ExecuteSqlGetIncrement(sql);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0001CA24 File Offset: 0x0001AC24
		private void DeleteYaoSaiPrisonLogData(KFPrisonLogData data)
		{
			string sql = string.Format("DELETE FROM `t_yaosai_prison_log` WHERE `id`={0};", data.ID);
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0001CA50 File Offset: 0x0001AC50
		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0001CA9C File Offset: 0x0001AC9C
		private long ExecuteSqlGetIncrement(string sqlCmd)
		{
			long result;
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSqlGetIncrement(sqlCmd, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1L;
			}
			return result;
		}

		// Token: 0x04000114 RID: 276
		private static YaoSaiService _instance = new YaoSaiService();

		// Token: 0x04000115 RID: 277
		private object Mutex = new object();

		// Token: 0x04000116 RID: 278
		private YaoSaiRuntimeData RuntimeData = new YaoSaiRuntimeData();

		// Token: 0x04000117 RID: 279
		private Dictionary<int, List<KFPrisonRoleAllData>> YaoSaiSearchDataDict = new Dictionary<int, List<KFPrisonRoleAllData>>();

		// Token: 0x04000118 RID: 280
		private Dictionary<int, KFPrisonRoleAllData> YaoSaiPrisonRoleDataDict = new Dictionary<int, KFPrisonRoleAllData>();

		// Token: 0x04000119 RID: 281
		private Dictionary<int, KFPrisonFuLuAllData> YaoSaiOwnerIDVsFuLuDict = new Dictionary<int, KFPrisonFuLuAllData>();

		// Token: 0x0400011A RID: 282
		private Dictionary<int, KFPrisonJingJiAllData> YaoSaiPrisonJingJiDataDict = new Dictionary<int, KFPrisonJingJiAllData>();

		// Token: 0x0400011B RID: 283
		private Dictionary<int, KFPrisonLogAllData> YaoSaiPrisonLogDataDict = new Dictionary<int, KFPrisonLogAllData>();
	}
}
