using System;
using System.Collections.Generic;
using System.Linq;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic;
using KF.Contract.Data;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class TianTiDbCmdProcessor : ICmdProcessor
	{
		
		private TianTiDbCmdProcessor()
		{
		}

		
		public static TianTiDbCmdProcessor getInstance()
		{
			return TianTiDbCmdProcessor.instance;
		}

		
		public void registerProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10200, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(969, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10201, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10202, TianTiDbCmdProcessor.getInstance());
			this.LoadZhanDuiData();
			TCPCmdDispatcher.getInstance().registerProcessor(3670, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3709, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3715, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3688, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3716, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3699, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3717, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3722, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3723, TianTiDbCmdProcessor.getInstance());
		}

		
		public bool LoadZhanDuiData()
		{
			bool result = false;
			lock (this.Mutex)
			{
				string sql = "select`zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid,zorkjifen,zorkwin,zorkwinstreak,zorkbossinjure,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui";
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					using (MySQLDataReader reader = conn.ExecuteReader(sql, new MySQLParameter[0]))
					{
						result = true;
						this.ZhanDuiDataList.Age = 1L;
						this.ZhanDuiDataList.V = new List<TianTi5v5ZhanDuiData>();
						while (reader.Read())
						{
							TianTi5v5ZhanDuiData data = new TianTi5v5ZhanDuiData();
							data.ZhanDuiID = Convert.ToInt32(reader[0].ToString());
							data.LeaderRoleID = Convert.ToInt32(reader["leaderid"].ToString());
							data.XuanYan = reader["xuanyan"].ToString();
							data.ZhanDuiName = reader["zhanduiname"].ToString();
							data.DuanWeiId = Convert.ToInt32(reader["duanweiid"].ToString());
							data.ZhanDouLi = Convert.ToInt64(reader["zhanli"].ToString());
							byte[] bytes = (reader["data1"] as byte[]) ?? new byte[0];
							data.teamerList = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(bytes, 0, bytes.Length);
							data.DuanWeiJiFen = Convert.ToInt32(reader["duanweijifen"].ToString());
							data.DuanWeiRank = Convert.ToInt32(reader["duanweirank"].ToString());
							data.LianSheng = Convert.ToInt32(reader["liansheng"].ToString());
							data.FightCount = Convert.ToInt32(reader["fightcount"].ToString());
							data.SuccessCount = Convert.ToInt32(reader["successcount"].ToString());
							data.LastFightTime = Convert.ToDateTime(reader["lastfighttime"].ToString());
							data.MonthDuanWeiRank = Convert.ToInt32(reader["monthduanweirank"].ToString());
							data.LeaderRoleName = reader["leaderrolename"].ToString();
							data.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
							data.ZorkJiFen = Convert.ToInt32(reader["zorkjifen"].ToString());
							data.ZorkWin = Convert.ToInt32(reader["zorkwin"].ToString());
							data.ZorkWinStreak = Convert.ToInt32(reader["zorkwinstreak"].ToString());
							data.ZorkBossInjure = Convert.ToInt32(reader["zorkbossinjure"].ToString());
							data.ZorkLastFightTime = Convert.ToDateTime(reader["zorklastfighttime"].ToString());
							data.EscapeJiFen = Convert.ToInt32(reader["escapejifen"].ToString());
							data.EscapeLastFightTime = Convert.ToDateTime(reader["escapelastfighttime"].ToString());
							this.ZhanDuiDataDict[data.ZhanDuiID] = new AgeDataT<TianTi5v5ZhanDuiData>(1L, data);
							this.ZhanDuiDataList.V.Add(data);
						}
					}
				}
				this.ZhanDuiDataList.V.Sort(new Comparison<TianTi5v5ZhanDuiData>(this.ZhanDuiDataCompare));
			}
			return result;
		}

		
		public int ZhanDuiDataCompare(TianTi5v5ZhanDuiData x, TianTi5v5ZhanDuiData y)
		{
			int result;
			if (x.LastFightTime < this.MonthStartDateTime)
			{
				if (y.LastFightTime < this.MonthStartDateTime)
				{
					int ret = y.DuanWeiJiFen - x.DuanWeiJiFen;
					result = ((ret != 0) ? ret : (x.ZhanDuiID - y.ZhanDuiID));
				}
				else
				{
					result = 1;
				}
			}
			else if (y.LastFightTime < this.MonthStartDateTime)
			{
				result = -1;
			}
			else
			{
				int ret = y.DuanWeiJiFen - x.DuanWeiJiFen;
				result = ((ret != 0) ? ret : (x.ZhanDuiID - y.ZhanDuiID));
			}
			return result;
		}

		
		public bool QueryZhanDuiRoleInfo(TianTi5v5ZhanDuiData data)
		{
			bool modify = false;
			if (data != null && null != data.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData role in data.teamerList)
				{
					try
					{
						int roleID = role.RoleID;
						DBRoleInfo roleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
						if (null != roleInfo)
						{
							int RebornLevel = Global.GetRoleParamsInt32(roleInfo, "10241");
							if (role.RoleName != roleInfo.RoleName || role.RoleOcc != roleInfo.Occupation || role.ZhanLi != (long)roleInfo.CombatForce || role.RebornLevel != RebornLevel)
							{
								modify = true;
							}
							role.RoleName = roleInfo.RoleName;
							role.RoleOcc = roleInfo.Occupation;
							role.ZhanLi = (long)roleInfo.CombatForce;
							role.ZhuanSheng = roleInfo.ChangeLifeCount;
							role.Level = roleInfo.Level;
							role.RebornLevel = RebornLevel;
							if (role.RoleID == data.LeaderRoleID)
							{
								data.LeaderRoleName = roleInfo.RoleName;
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				data.ZhanDouLi = data.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
			}
			return modify;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			DBManager dbMgr = DBManager.getInstance();
			int result = 0;
			if (nID <= 3688)
			{
				if (nID != 969)
				{
					if (nID != 3670)
					{
						if (nID == 3688)
						{
							AgeDataT<int> requestData = DataHelper.BytesToObject<AgeDataT<int>>(cmdParams, 0, count);
							AgeDataT<List<TianTi5v5ZhanDuiMiniData>> resultData = new AgeDataT<List<TianTi5v5ZhanDuiMiniData>>(requestData.Age, null);
							int maxCount = requestData.V;
							lock (this.Mutex)
							{
								if (this.ZhanDuiDataListNeedUpdate)
								{
									List<TianTi5v5ZhanDuiData> list = new List<TianTi5v5ZhanDuiData>();
									foreach (AgeDataT<TianTi5v5ZhanDuiData> item in this.ZhanDuiDataDict.Values)
									{
										if (null != item.V)
										{
											list.Add(item.V);
										}
									}
									DateTime now = DateTime.Now;
									this.MonthStartDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
									list.Sort(new Comparison<TianTi5v5ZhanDuiData>(this.ZhanDuiDataCompare));
									this.ZhanDuiDataList.V = list;
									this.ZhanDuiDataListNeedUpdate = false;
									TimeUtil.AgeByNow(ref this.ZhanDuiDataList.Age);
								}
								if (requestData.Age < this.ZhanDuiDataList.Age)
								{
									resultData.Age = this.ZhanDuiDataList.Age;
									resultData.V = new List<TianTi5v5ZhanDuiMiniData>();
									int i = 0;
									while (i < this.ZhanDuiDataList.V.Count && i < maxCount)
									{
										TianTi5v5ZhanDuiData pData = this.ZhanDuiDataList.V[i];
										TianTi5v5ZhanDuiMiniData miniData = new TianTi5v5ZhanDuiMiniData();
										miniData.ZhanDuiID = pData.ZhanDuiID;
										miniData.DuanWeiID = pData.DuanWeiId;
										miniData.DuiZhangName = pData.ZhanDuiName;
										miniData.XuanYan = pData.XuanYan;
										miniData.ZhanDouLi = pData.ZhanDouLi;
										miniData.Name = pData.LeaderRoleName;
										miniData.MemberList = new List<RoleNameLevelData>();
										foreach (TianTi5v5ZhanDuiRoleData role2 in pData.teamerList)
										{
											miniData.MemberList.Add(new RoleNameLevelData(role2.ZhuanSheng, role2.Level, role2.RoleName, role2.RoleID == pData.LeaderRoleID, role2.RoleOcc));
										}
										resultData.V.Add(miniData);
										i++;
									}
								}
							}
							client.sendCmd<AgeDataT<List<TianTi5v5ZhanDuiMiniData>>>(nID, resultData);
						}
					}
					else
					{
						TianTiLogItemData kf5V5LogItemData = DataHelper.BytesToObject<TianTiLogItemData>(cmdParams, 0, count);
						if (kf5V5LogItemData != null && kf5V5LogItemData.RoleId > 0)
						{
							lock (this.Mutex)
							{
								KF5V5RoleLogData pKF5VRoleLogData;
								if (this.KF5V5RoleLogDataDict.TryGetValue(kf5V5LogItemData.RoleId, out pKF5VRoleLogData))
								{
									pKF5VRoleLogData.LogItemList.Insert(0, kf5V5LogItemData);
								}
							}
							DBWriter.InsertKF5v5ItemLog(DBManager.getInstance(), kf5V5LogItemData);
						}
						client.sendCmd<int>(nID, result);
					}
				}
				else
				{
					int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
					bool needLoad = false;
					TianTiRoleLogData tianTiRoleLogData;
					lock (this.Mutex)
					{
						if (!this.TianTiRoleLogDataDict.TryGetValue(roleId, out tianTiRoleLogData))
						{
							tianTiRoleLogData = new TianTiRoleLogData();
							this.TianTiRoleLogDataDict.Add(roleId, tianTiRoleLogData);
							needLoad = true;
						}
					}
					if (needLoad)
					{
						tianTiRoleLogData.LogItemList = DBQuery.GetTianTiLogItemDataList(dbMgr, roleId, 100);
					}
					lock (this.Mutex)
					{
						if (null != tianTiRoleLogData.LogItemList)
						{
							if (tianTiRoleLogData.LogItemList.Count > 0)
							{
								if (tianTiRoleLogData.LogItemList.Count > 100)
								{
									int c = tianTiRoleLogData.LogItemList.Count - 100;
									if (c > 0)
									{
										tianTiRoleLogData.LogItemList.RemoveRange(100, c);
									}
								}
							}
						}
					}
					client.sendCmd<List<TianTiLogItemData>>(nID, tianTiRoleLogData.LogItemList);
				}
			}
			else if (nID <= 3709)
			{
				if (nID != 3699)
				{
					if (nID == 3709)
					{
						int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
						bool needLoad = false;
						KF5V5RoleLogData kf5v5RoleLogData;
						lock (this.Mutex)
						{
							if (!this.KF5V5RoleLogDataDict.TryGetValue(roleId, out kf5v5RoleLogData))
							{
								kf5v5RoleLogData = new KF5V5RoleLogData();
								this.KF5V5RoleLogDataDict.Add(roleId, kf5v5RoleLogData);
								needLoad = true;
							}
						}
						if (needLoad)
						{
							kf5v5RoleLogData.LogItemList = DBQuery.GetT5v5ItemDataList(dbMgr, roleId, 100);
						}
						lock (this.Mutex)
						{
							if (null != kf5v5RoleLogData.LogItemList)
							{
								if (kf5v5RoleLogData.LogItemList.Count > 0)
								{
									if (kf5v5RoleLogData.LogItemList.Count > 100)
									{
										int c = kf5v5RoleLogData.LogItemList.Count - 100;
										if (c > 0)
										{
											kf5v5RoleLogData.LogItemList.RemoveRange(100, c);
										}
									}
								}
							}
						}
						client.sendCmd<List<TianTiLogItemData>>(nID, kf5v5RoleLogData.LogItemList);
					}
				}
				else
				{
					int zhanDuiID = DataHelper.BytesToObject<int>(cmdParams, 0, count);
					lock (this.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> zhanDuiData;
						if (this.ZhanDuiDataDict.TryGetValue(zhanDuiID, out zhanDuiData))
						{
							TimeUtil.AgeByNow(ref zhanDuiData.Age);
							zhanDuiData.V = null;
							this.ZhanDuiDataListNeedUpdate = true;
							using (MyDbConnection3 conn = new MyDbConnection3(false))
							{
								string cmdText = string.Format("delete from t_kf_5v5_zhandui where zhanduiid={0}", zhanDuiID);
								result = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<int>(nID, result);
				}
			}
			else
			{
				switch (nID)
				{
				case 3715:
				{
					AgeDataT<int> requestData = DataHelper.BytesToObject<AgeDataT<int>>(cmdParams, 0, count);
					int zhanDuiID = requestData.V;
					AgeDataT<TianTi5v5ZhanDuiData> zhanDuiData;
					lock (this.Mutex)
					{
						if (this.ZhanDuiDataDict.TryGetValue(requestData.V, out zhanDuiData))
						{
							if (this.QueryZhanDuiRoleInfo(zhanDuiData.V))
							{
								TimeUtil.AgeByNow(ref zhanDuiData.Age);
							}
							if (requestData.Age == zhanDuiData.Age || zhanDuiData.V == null)
							{
								zhanDuiData = new AgeDataT<TianTi5v5ZhanDuiData>(zhanDuiData.Age, null);
							}
							goto IL_91C;
						}
					}
					TianTi5v5ZhanDuiData data = new TianTi5v5ZhanDuiData();
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string sql = string.Format("select zhanduiname,xuanyan,`leaderid`,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,lastfighttime,monthduanweirank,zhanli,data1,leaderrolename,zoneid,zorkjifen,zorkwin,zorkwinstreak,zorkbossinjure,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui where zhanduiid={0}", zhanDuiID);
						using (MySQLDataReader reader = conn.ExecuteReader(sql, new MySQLParameter[0]))
						{
							if (reader.Read())
							{
								data.ZhanDuiID = zhanDuiID;
								data.LeaderRoleID = Convert.ToInt32(reader["leaderid"].ToString());
								data.XuanYan = reader["xuanyan"].ToString();
								data.ZhanDuiName = reader["zhanduiname"].ToString();
								data.DuanWeiId = Convert.ToInt32(reader["duanweiid"].ToString());
								data.ZhanDouLi = Convert.ToInt64(reader["zhanli"].ToString());
								byte[] bytes = (reader["data1"] as byte[]) ?? new byte[0];
								data.teamerList = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(bytes, 0, bytes.Length);
								data.DuanWeiJiFen = Convert.ToInt32(reader["duanweijifen"].ToString());
								data.DuanWeiRank = Convert.ToInt32(reader["duanweirank"].ToString());
								data.LianSheng = Convert.ToInt32(reader["liansheng"].ToString());
								data.FightCount = Convert.ToInt32(reader["fightcount"].ToString());
								data.SuccessCount = Convert.ToInt32(reader["successcount"].ToString());
								data.LastFightTime = Convert.ToDateTime(reader["lastfighttime"].ToString());
								data.MonthDuanWeiRank = Convert.ToInt32(reader["monthduanweirank"].ToString());
								data.LeaderRoleName = reader["leaderrolename"].ToString();
								data.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
								data.ZorkJiFen = Convert.ToInt32(reader["zorkjifen"].ToString());
								data.ZorkWin = Convert.ToInt32(reader["zorkwin"].ToString());
								data.ZorkWinStreak = Convert.ToInt32(reader["zorkwinstreak"].ToString());
								data.ZorkBossInjure = Convert.ToInt32(reader["zorkbossinjure"].ToString());
								data.ZorkLastFightTime = Convert.ToDateTime(reader["zorklastfighttime"].ToString());
								data.EscapeJiFen = Convert.ToInt32(reader["escapejifen"].ToString());
								data.EscapeLastFightTime = Convert.ToDateTime(reader["escapelastfighttime"].ToString());
							}
						}
					}
					lock (this.Mutex)
					{
						if (data.ZhanDuiID > 0)
						{
							if (!this.ZhanDuiDataDict.TryGetValue(requestData.V, out zhanDuiData) || zhanDuiData.V == null)
							{
								zhanDuiData = new AgeDataT<TianTi5v5ZhanDuiData>(1L, data);
								this.ZhanDuiDataDict[requestData.V] = zhanDuiData;
							}
						}
						else
						{
							zhanDuiData = new AgeDataT<TianTi5v5ZhanDuiData>(requestData.Age + 1L, null);
							this.ZhanDuiDataDict[requestData.V] = zhanDuiData;
						}
						if (this.QueryZhanDuiRoleInfo(zhanDuiData.V))
						{
							TimeUtil.AgeByNow(ref zhanDuiData.Age);
						}
					}
					IL_91C:
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, zhanDuiData);
					break;
				}
				case 3716:
				{
					AgeDataT<TianTi5v5ZhanDuiData> zhanDuiData = null;
					AgeDataT<TianTi5v5ZhanDuiData> requestData2 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (requestData2 != null && requestData2.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData data = requestData2.V;
							if (!this.ZhanDuiDataDict.TryGetValue(data.ZhanDuiID, out zhanDuiData))
							{
								zhanDuiData = new AgeDataT<TianTi5v5ZhanDuiData>(0L, data);
								this.ZhanDuiDataDict[data.ZhanDuiID] = zhanDuiData;
								this.ZhanDuiDataListNeedUpdate = true;
							}
							else
							{
								if (zhanDuiData.V == null || zhanDuiData.V.LeaderRoleID != requestData2.V.LeaderRoleID || zhanDuiData.V.LeaderRoleName != requestData2.V.LeaderRoleName || zhanDuiData.V.DuanWeiId != requestData2.V.DuanWeiId)
								{
									this.ZhanDuiDataListNeedUpdate = true;
								}
								if (requestData2.V.ZhanDuiDataModeType == 1)
								{
									zhanDuiData.V.ZhanDuiID = data.ZhanDuiID;
									zhanDuiData.V.XuanYan = data.XuanYan;
									zhanDuiData.V.ZhanDuiName = data.ZhanDuiName;
									zhanDuiData.V.LeaderRoleID = data.LeaderRoleID;
									zhanDuiData.V.ZhanDouLi = data.ZhanDouLi;
									zhanDuiData.V.teamerList = data.teamerList;
									zhanDuiData.V.TeamerRidList = data.TeamerRidList;
									zhanDuiData.V.LeaderRoleName = data.LeaderRoleName;
									zhanDuiData.V.ZoneID = data.ZoneID;
								}
								else if (requestData2.V.ZhanDuiDataModeType == 0)
								{
									zhanDuiData.V.DuanWeiId = data.DuanWeiId;
									zhanDuiData.V.DuanWeiJiFen = data.DuanWeiJiFen;
									zhanDuiData.V.DuanWeiRank = data.DuanWeiRank;
									zhanDuiData.V.ZhanDouLi = data.ZhanDouLi;
									zhanDuiData.V.LianSheng = data.LianSheng;
									zhanDuiData.V.SuccessCount = data.SuccessCount;
									zhanDuiData.V.FightCount = data.FightCount;
									zhanDuiData.V.MonthDuanWeiRank = data.MonthDuanWeiRank;
									zhanDuiData.V.LastFightTime = data.LastFightTime;
									using (List<TianTi5v5ZhanDuiRoleData>.Enumerator enumerator2 = zhanDuiData.V.teamerList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											TianTi5v5ZhanDuiRoleData role = enumerator2.Current;
											TianTi5v5ZhanDuiRoleData newRoleInfo = data.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == role.RoleID);
											if (null != newRoleInfo)
											{
												role.MonthFightCounts = newRoleInfo.MonthFightCounts;
												role.TodayFightCount = newRoleInfo.TodayFightCount;
												role.MonthFigntCount = newRoleInfo.MonthFigntCount;
												role.ZhanLi = newRoleInfo.ZhanLi;
												role.RoleOcc = newRoleInfo.RoleOcc;
												role.ZhuanSheng = newRoleInfo.ZhuanSheng;
												role.Level = newRoleInfo.Level;
												role.RebornLevel = newRoleInfo.RebornLevel;
												role.ModelData = newRoleInfo.ModelData;
											}
										}
									}
								}
							}
							requestData2.Age = TimeUtil.AgeByNow(ref zhanDuiData.Age);
							using (MyDbConnection3 conn = new MyDbConnection3(false))
							{
								data = zhanDuiData.V;
								if (null != data)
								{
									MySQLParameter p = new MySQLParameter("@p1", data.ZhanDuiName);
									MySQLParameter p2 = new MySQLParameter("@p2", data.XuanYan);
									string teamerList = DataHelper.ObjectToHexString<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList);
									string cmdText = string.Format("INSERT INTO t_kf_5v5_zhandui (zhanduiid,zhanduiname,xuanyan,`leaderid`,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,lastfighttime,monthduanweirank,zhanli,data1,leaderrolename,zoneid) VALUES({0},@p1,@p2,{3},{4},{5},{6},{7},{8},{9},'{10}',{11},{12},{14},'{13}',{15}) ON DUPLICATE KEY UPDATE zhanduiname=@p1,xuanyan=@p2,leaderid={3},duanweiid={4},duanweijifen={5},duanweirank={6},liansheng={7},fightcount={8},successcount={9},lastfighttime='{10}',monthduanweirank={11},zhanli={12},data1={14},leaderrolename='{13}',zoneid={15}", new object[]
									{
										data.ZhanDuiID,
										data.ZhanDuiName,
										data.XuanYan,
										data.LeaderRoleID,
										data.DuanWeiId,
										data.DuanWeiJiFen,
										data.DuanWeiRank,
										data.LianSheng,
										data.FightCount,
										data.SuccessCount,
										data.LastFightTime,
										data.MonthDuanWeiRank,
										data.ZhanDouLi,
										data.LeaderRoleName,
										teamerList,
										data.ZoneID
									});
									int ret = conn.ExecuteSql(cmdText, new MySQLParameter[]
									{
										p,
										p2
									});
								}
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, zhanDuiData);
					break;
				}
				case 3717:
				{
					int[] args = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
					DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref args[0]);
					if (null != roleInfo)
					{
						roleInfo.ZhanDuiID = args[1];
						roleInfo.ZhanDuiZhiWu = args[2];
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("update t_roles set zhanduiid={1},zhanduizhiwu={2} where rid={0}", roleInfo.RoleID, roleInfo.ZhanDuiID, roleInfo.ZhanDuiZhiWu);
							result = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
						}
					}
					client.sendCmd<int>(nID, result);
					break;
				}
				case 3718:
				case 3719:
				case 3720:
				case 3721:
					break;
				case 3722:
				{
					AgeDataT<TianTi5v5ZhanDuiData> zhanDuiData = null;
					AgeDataT<TianTi5v5ZhanDuiData> requestData2 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (requestData2 != null && requestData2.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData data = requestData2.V;
							if (!this.ZhanDuiDataDict.TryGetValue(data.ZhanDuiID, out zhanDuiData))
							{
								client.sendCmd(30767, "0");
								break;
							}
							zhanDuiData.V.ZorkJiFen = data.ZorkJiFen;
							zhanDuiData.V.ZorkWin = data.ZorkWin;
							zhanDuiData.V.ZorkWinStreak = data.ZorkWinStreak;
							zhanDuiData.V.ZorkBossInjure = data.ZorkBossInjure;
							zhanDuiData.V.ZorkLastFightTime = data.ZorkLastFightTime;
							requestData2.Age = TimeUtil.AgeByNow(ref zhanDuiData.Age);
							using (MyDbConnection3 conn = new MyDbConnection3(false))
							{
								string cmdText = string.Format("UPDATE t_kf_5v5_zhandui SET zorkjifen={1}, zorkwin={2}, zorkwinstreak={3}, zorkbossinjure={4}, zorklastfighttime='{5}' WHERE zhanduiid={0}; ", new object[]
								{
									data.ZhanDuiID,
									data.ZorkJiFen,
									data.ZorkWin,
									data.ZorkWinStreak,
									data.ZorkBossInjure,
									data.ZorkLastFightTime
								});
								int ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, zhanDuiData);
					break;
				}
				case 3723:
				{
					AgeDataT<TianTi5v5ZhanDuiData> zhanDuiData = null;
					AgeDataT<TianTi5v5ZhanDuiData> requestData2 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (requestData2 != null && requestData2.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData data = requestData2.V;
							if (!this.ZhanDuiDataDict.TryGetValue(data.ZhanDuiID, out zhanDuiData))
							{
								client.sendCmd(30767, "0");
								break;
							}
							zhanDuiData.V.EscapeJiFen = data.EscapeJiFen;
							zhanDuiData.V.EscapeLastFightTime = data.EscapeLastFightTime;
							requestData2.Age = TimeUtil.AgeByNow(ref zhanDuiData.Age);
							using (MyDbConnection3 conn = new MyDbConnection3(false))
							{
								string cmdText = string.Format("UPDATE t_kf_5v5_zhandui SET escapejifen={1}, escapelastfighttime='{2}' WHERE zhanduiid={0}; ", data.ZhanDuiID, data.EscapeJiFen, data.EscapeLastFightTime);
								int ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, zhanDuiData);
					break;
				}
				default:
					switch (nID)
					{
					case 10200:
					{
						TianTiLogItemData tianTiLogItemData = DataHelper.BytesToObject<TianTiLogItemData>(cmdParams, 0, count);
						if (tianTiLogItemData != null && tianTiLogItemData.RoleId > 0)
						{
							lock (this.Mutex)
							{
								TianTiRoleLogData tianTiRoleLogData;
								if (this.TianTiRoleLogDataDict.TryGetValue(tianTiLogItemData.RoleId, out tianTiRoleLogData))
								{
									tianTiRoleLogData.LogItemList.Insert(0, tianTiLogItemData);
								}
							}
							DBWriter.InsertTianTiItemLog(DBManager.getInstance(), tianTiLogItemData);
						}
						client.sendCmd<int>(nID, result);
						break;
					}
					case 10201:
					{
						RoleTianTiData roleTianTiData = DataHelper.BytesToObject<RoleTianTiData>(cmdParams, 0, count);
						if (null != roleTianTiData)
						{
							DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleTianTiData.RoleId);
							if (null != dbRoleInfo)
							{
								lock (dbRoleInfo)
								{
									dbRoleInfo.TianTiData = roleTianTiData;
									DBWriter.UpdateTianTiRoleData(dbMgr, roleTianTiData);
								}
							}
						}
						client.sendCmd<int>(nID, result);
						break;
					}
					case 10202:
					{
						int[] dataArray = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
						if (dataArray != null && dataArray.Length == 2)
						{
							DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref dataArray[0]);
							if (null != dbRoleInfo)
							{
								lock (dbRoleInfo)
								{
									dbRoleInfo.TianTiData.RongYao = dataArray[1];
									if (dbRoleInfo.TianTiData.LastFightDayId > 0)
									{
										result = DBWriter.UpdateTianTiRoleRongYao(dbMgr, dataArray[0], dataArray[1]);
									}
									else
									{
										result = DBWriter.UpdateTianTiRoleData(dbMgr, dbRoleInfo.TianTiData);
									}
								}
							}
						}
						client.sendCmd<int>(nID, result);
						break;
					}
					}
					break;
				}
			}
		}

		
		private const int MaxCacheLogItemCount = 100;

		
		private static TianTiDbCmdProcessor instance = new TianTiDbCmdProcessor();

		
		private object Mutex = new object();

		
		private Dictionary<int, TianTiRoleLogData> TianTiRoleLogDataDict = new Dictionary<int, TianTiRoleLogData>();

		
		private Dictionary<int, KF5V5RoleLogData> KF5V5RoleLogDataDict = new Dictionary<int, KF5V5RoleLogData>();

		
		private Queue<TianTiLogItemData> TianTiRoleItemLogCacheQueue = new Queue<TianTiLogItemData>();

		
		private Queue<TianTiLogItemData> TianTiRoleItemLogWriteQueue = new Queue<TianTiLogItemData>();

		
		private bool ZhanDuiDataListNeedUpdate;

		
		private Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>> ZhanDuiDataDict = new Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>>();

		
		private AgeDataT<List<TianTi5v5ZhanDuiData>> ZhanDuiDataList = new AgeDataT<List<TianTi5v5ZhanDuiData>>();

		
		private DateTime MonthStartDateTime;
	}
}
