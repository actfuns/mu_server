using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Name
{
	
	public class NameManager : SingletonTemplate<NameManager>
	{
		
		private NameManager()
		{
		}

		
		public TCPProcessCmdResults ProcChangeName(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string uid = fields[0];
				int zoneId = Convert.ToInt32(fields[1]);
				int roleId = Convert.ToInt32(fields[2]);
				string newName = fields[3];
				int costZuanShiBase = Convert.ToInt32(fields[4]);
				int costZuanShiMax = Convert.ToInt32(fields[5]);
				int canFreeMod = Convert.ToInt32(fields[6]);
				int canZuanShiMod = Convert.ToInt32(fields[7]);
				string oldName = "";
				string failedMsg = "";
				int needZuanShi = 0;
				DBUserInfo userInfo = null;
				DBRoleInfo roleInfo = null;
				ChangeNameType cnt = ChangeNameType.CNT_Unknown;
				ChangeNameError cne = ChangeNameError.Success;
				bool bNewNameTakePlaceHolder = false;
				userInfo = dbMgr.GetDBUserInfo(uid);
				if (userInfo == null)
				{
					failedMsg = "账号找不到 : " + uid;
					cne = ChangeNameError.DBFailed;
				}
				else
				{
					lock (userInfo)
					{
						int i;
						for (i = 0; i < userInfo.ListRoleIDs.Count; i++)
						{
							if (userInfo.ListRoleZoneIDs[i] == zoneId && userInfo.ListRoleIDs[i] == roleId)
							{
								break;
							}
						}
						if (i == userInfo.ListRoleIDs.Count)
						{
							failedMsg = string.Concat(new object[]
							{
								"账号: ",
								uid,
								" 在 ",
								zoneId.ToString(),
								" 区，不包含角色 ",
								roleId
							});
							cne = ChangeNameError.NotContainRole;
							goto IL_641;
						}
					}
					roleInfo = dbMgr.GetDBRoleInfo(ref roleId);
					if (null == roleInfo)
					{
						failedMsg = "查找不到dbroleinfo,roleid=" + roleId.ToString();
						cne = ChangeNameError.DBFailed;
					}
					else if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(roleId))
					{
						failedMsg = "处在预删除状态dbroleinfo,roleid=" + roleId.ToString();
						cne = ChangeNameError.DBFailed;
					}
					else
					{
						oldName = roleInfo.RoleName;
						int leftFreeTimes = Global.GetRoleParamsInt32(roleInfo, "LeftFreeChangeNameTimes");
						if (leftFreeTimes > 0 && 1 == canFreeMod)
						{
							cnt = ChangeNameType.CNT_Free;
						}
						else
						{
							cnt = ChangeNameType.CNT_ZuanShi;
						}
						if ((cnt == ChangeNameType.CNT_Free && 1 != canFreeMod) || (cnt == ChangeNameType.CNT_ZuanShi && 1 != canZuanShiMod))
						{
							failedMsg = "服务器没有开放改名功能, " + cnt.ToString();
							cne = ChangeNameError.ServerDenied;
						}
						else
						{
							SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(oldName);
							if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(newName) || dbMgr.IsRolenameExist(newName) || !SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, newName))
							{
								failedMsg = "新名字： " + newName + " 已被占用";
								cne = ChangeNameError.NameAlreayUsed;
							}
							else
							{
								bNewNameTakePlaceHolder = true;
								if (PreNamesManager.SetUsedPreName(newName))
								{
									DBWriter.UpdatePreNameUsedState(dbMgr, newName, 1);
								}
								string updateKey = "";
								string updateVal = "";
								if (cnt == ChangeNameType.CNT_Free)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("角色请求免费改名，roleid={0}, old name={1}, new name={2}", roleId, oldName, newName), null, true);
									leftFreeTimes = Math.Max(0, leftFreeTimes - 1);
									updateKey = "LeftFreeChangeNameTimes";
									updateVal = leftFreeTimes.ToString();
								}
								else if (cnt == ChangeNameType.CNT_ZuanShi)
								{
									int alreadyZuanShiTimes = Global.GetRoleParamsInt32(roleInfo, "AlreadyZuanShiChangeNameTimes");
									needZuanShi = Math.Min(costZuanShiMax, costZuanShiBase * (alreadyZuanShiTimes + 1));
									LogManager.WriteLog(LogTypes.Error, string.Format("角色请求钻石改名，roleid={0}, old name={1}, new name={2}, costzuanshi={3}", new object[]
									{
										roleId,
										oldName,
										newName,
										needZuanShi
									}), null, true);
									lock (userInfo)
									{
										if (userInfo.Money < needZuanShi)
										{
											failedMsg = "钻石不足";
											cne = ChangeNameError.ZuanShiNotEnough;
											goto IL_641;
										}
										int tmpMoney = userInfo.Money;
										userInfo.Money -= needZuanShi;
										if (!DBWriter.UpdateUserInfo(dbMgr, userInfo))
										{
											userInfo.Money = tmpMoney;
											failedMsg = string.Format("改名时更新用户的元宝失败，UserID={0}", userInfo.UserID);
											cne = ChangeNameError.DBFailed;
											goto IL_641;
										}
										alreadyZuanShiTimes++;
										updateKey = "AlreadyZuanShiChangeNameTimes";
										updateVal = alreadyZuanShiTimes.ToString();
									}
								}
								Global.UpdateRoleParamByName(dbMgr, roleInfo, updateKey, updateVal, null);
								string cmdText = string.Format("UPDATE t_roles SET rname='{0}' WHERE rid={1} AND userid='{2}' AND zoneid={3}", new object[]
								{
									newName,
									roleId,
									uid,
									zoneId
								});
								if (!this._Util_ExecNonQuery(dbMgr, cmdText))
								{
									failedMsg = "更新t_roles的名字失败";
									cne = ChangeNameError.DBFailed;
								}
								else
								{
									lock (userInfo)
									{
										for (int i = 0; i < userInfo.ListRoleIDs.Count; i++)
										{
											if (userInfo.ListRoleZoneIDs[i] == zoneId && userInfo.ListRoleIDs[i] == roleId)
											{
												userInfo.ListRoleNames[i] = newName;
												break;
											}
										}
									}
									lock (roleInfo)
									{
										oldName = roleInfo.RoleName;
										roleInfo.RoleName = newName;
									}
									cne = ChangeNameError.Success;
								}
							}
						}
					}
				}
				IL_641:
				if (cne == ChangeNameError.Success)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名成功，roleid={0}, old name={1}，new name={2}", roleId, oldName, newName), null, true);
					this.AddChangeNameDBRecord(dbMgr, roleId, oldName, newName, cnt, needZuanShi);
					this._OnChangeNameSuccess(dbMgr, roleId, zoneId, oldName, newName);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名失败，roleid={0}, name={1}, reason={2}", roleId, oldName, failedMsg), null, true);
					if (bNewNameTakePlaceHolder)
					{
						SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(newName);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)cne,
					oldName,
					needZuanShi,
					userInfo.Money
				}), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private void _OnChangeNameSuccess(DBManager dbMgr, int roleId, int zoneId, string oldName, string newName)
		{
			DBManager.getInstance().DBRoleMgr.OnChangeName(roleId, zoneId, oldName, newName);
			FuBenHistManager.OnChangeName(roleId, oldName, newName);
			PaiHangManager.OnChangeName(roleId, oldName, newName);
			string sql = string.Format("UPDATE t_mail SET senderrname='{0}' WHERE senderrid={1}", newName, roleId);
			sql = string.Format("UPDATE t_mail SET reveiverrname='{0}' WHERE receiverrid={1}", newName, roleId);
			if (!this._Util_ExecNonQuery(dbMgr, sql))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("更新t_mail [reveiverrname] 失败, roleId={0}, oldName={1}, newName={2}", roleId, oldName, newName), null, true);
			}
		}

		
		private bool _Util_ExecNonQuery(DBManager dbMgr, string sql)
		{
			bool bRet = false;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					bRet = false;
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				}
				cmd.Dispose();
				cmd = null;
				bRet = true;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return bRet;
		}

		
		public TCPProcessCmdResults ProcQueryEachRoleInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string uid = fields[0];
				int zoneId = Convert.ToInt32(fields[1]);
				ChangeNameInfo info = new ChangeNameInfo();
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(uid);
				if (dbUserInfo != null)
				{
					lock (dbUserInfo)
					{
						info.ZuanShi = dbUserInfo.Money;
						for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
						{
							if (dbUserInfo.ListRoleZoneIDs[i] == zoneId)
							{
								int roleID = dbUserInfo.ListRoleIDs[i];
								DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
								if (dbRoleInfo != null)
								{
									int roleId = dbUserInfo.ListRoleIDs[i];
									int leftFreeTimes = Global.GetRoleParamsInt32(dbRoleInfo, "LeftFreeChangeNameTimes");
									int alreadyZuanshiTimes = Global.GetRoleParamsInt32(dbRoleInfo, "AlreadyZuanShiChangeNameTimes");
									info.RoleList.Add(new EachRoleChangeName
									{
										RoleId = roleId,
										LeftFreeTimes = leftFreeTimes,
										AlreadyZuanShiTimes = alreadyZuanshiTimes
									});
								}
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ChangeNameInfo>(info, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private bool AddChangeNameDBRecord(DBManager dbMgr, int roleid, string oldName, string newName, ChangeNameType cnt, int costDiamond)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_change_name(roleid,oldname,newname,type,cost_diamond,time) VALUES({0},'{1}','{2}',{3},{4},'{5}')", new object[]
				{
					roleid,
					oldName,
					newName,
					(int)cnt,
					costDiamond,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				bRet = conn.ExecuteNonQueryBool(sql, 0);
			}
			return bRet;
		}

		
		public bool IsNameCanUseInDb(DBManager dbMgr, string name)
		{
			bool result;
			if (dbMgr == null || string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				MySQLConnection conn = null;
				string prefixName = name + "99999999";
				try
				{
					int key = Thread.CurrentThread.ManagedThreadId;
					string sql = string.Format("REPLACE INTO t_name_check(`id`,`name`) VALUES({0},'{1}');", key, prefixName);
					conn = dbMgr.DBConns.PopDBConnection();
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
					MySQLCommand cmd = new MySQLCommand(sql, conn);
					cmd.ExecuteNonQuery();
					cmd = new MySQLCommand(string.Format("SELECT name FROM t_name_check WHERE Id = {0};", key), conn);
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					if (reader.Read())
					{
						string nameInDb = reader["name"].ToString();
						if (!string.IsNullOrEmpty(nameInDb) && nameInDb == prefixName)
						{
							return true;
						}
					}
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					if (null != conn)
					{
						dbMgr.DBConns.PushDBConnection(conn);
					}
				}
				result = false;
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcChangeBangHuiName(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			string bhOldName = "";
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleId = Convert.ToInt32(fields[0]);
				int factionId = Convert.ToInt32(fields[1]);
				string newName = fields[2];
				EChangeGuildNameError ne = EChangeGuildNameError.DBFailed;
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleId);
				if (null == roleInfo)
				{
					ne = EChangeGuildNameError.DBFailed;
				}
				else
				{
					lock (roleInfo)
					{
						if (roleInfo.Faction != factionId || roleInfo.BHZhiWu != 1)
						{
							ne = EChangeGuildNameError.OperatorDenied;
							goto IL_381;
						}
					}
					if (!this.IsNameCanUseInDb(dbMgr, newName))
					{
						ne = EChangeGuildNameError.InvalidName;
					}
					else
					{
						BangHuiDetailData detail = DBQuery.QueryBangHuiInfoByID(dbMgr, factionId);
						if (detail == null || detail.CanModNameTimes <= 0 || detail.BZRoleID != roleId)
						{
							ne = EChangeGuildNameError.OperatorDenied;
						}
						else
						{
							bhOldName = detail.BHName;
							SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(detail.BHName);
							if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(newName) || dbMgr.IsBangHuiNameExist(newName))
							{
								ne = EChangeGuildNameError.NameAlreadyUsed;
							}
							else
							{
								string sql = string.Format("UPDATE t_banghui SET bhname='{0}', can_mod_name_times={1} WHERE bhid={2}", newName, detail.CanModNameTimes - 1, factionId);
								if (!this._Util_ExecNonQuery(dbMgr, sql))
								{
									ne = EChangeGuildNameError.DBFailed;
								}
								else
								{
									lock (roleInfo)
									{
										roleInfo.BHName = newName;
									}
									if (!DBWriter.UpdateAllRoleBangHuiName(dbMgr, factionId, newName))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("更新帮会id={0}的名字 {1} => {2}，更新t_roles未(全部)成功", factionId, detail.BHName, newName), null, true);
									}
									List<DBRoleInfo> dbRoleInfoList = dbMgr.DBRoleMgr.GetCachingDBRoleInfoListByFaction(factionId);
									if (null != dbRoleInfoList)
									{
										for (int i = 0; i < dbRoleInfoList.Count; i++)
										{
											dbRoleInfoList[i].BHName = newName;
										}
									}
									ZhanMengShiJianData eventData = new ZhanMengShiJianData();
									eventData.BHID = factionId;
									eventData.ShiJianType = ZhanMengShiJianConstants.ChangeName;
									eventData.RoleName = roleInfo.RoleName;
									eventData.SubSzValue1 = newName;
									eventData.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
									ZhanMengShiJianManager.getInstance().onAddZhanMengShiJian(eventData);
									string recordSql = string.Format("INSERT INTO t_change_name_banghui(bhid,by_role,old_name,new_name,time) VALUES({0},{1},'{2}','{3}','{4}')", new object[]
									{
										factionId,
										roleId,
										bhOldName,
										newName,
										DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
									});
									this._Util_ExecNonQuery(dbMgr, recordSql);
									ne = EChangeGuildNameError.Success;
								}
							}
						}
					}
				}
				IL_381:
				if (ne == EChangeGuildNameError.Success)
				{
					GameDBManager.BangHuiLingDiMgr.OnChangeBangHuiName(factionId, bhOldName, newName);
					string gmCmdData = string.Format("-synclingdi", new object[0]);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", (int)ne), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public TCPProcessCmdResults ProcAddBangHuiChangeNameTimes(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string cmdData = new UTF8Encoding().GetString(data, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleId = Convert.ToInt32(fields[0]);
				int addValue = Convert.ToInt32(fields[1]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleId);
				int ne;
				if (null == roleInfo)
				{
					ne = -1;
				}
				else
				{
					int factionId = roleInfo.Faction;
					string sql = string.Format("UPDATE t_banghui SET can_mod_name_times=can_mod_name_times+{0} WHERE bhid={1}", addValue, factionId);
					if (!this._Util_ExecNonQuery(dbMgr, sql))
					{
						ne = -3;
					}
					else
					{
						BangHuiDetailData detail = DBQuery.QueryBangHuiInfoByID(dbMgr, factionId);
						if (detail == null)
						{
							ne = -2;
						}
						else
						{
							ne = detail.CanModNameTimes;
						}
					}
				}
				byte[] bytesData = DataHelper.ObjectToBytes<int>(ne);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
