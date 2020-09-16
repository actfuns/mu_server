using System;
using System.Text;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.UnionAlly;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Name
{
	
	public class NameManager : SingletonTemplate<NameManager>
	{
		
		private NameManager()
		{
		}

		
		public void LoadConfig()
		{
			try
			{
				int[] arr = GameManager.systemParamsList.GetParamValueIntArrayByName("NameLengthRange", ',');
				if (arr != null && arr.Length >= 2)
				{
					this.NameMinLen = arr[0];
					this.NameMaxLen = arr[1];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "NameManager.LoadConfig", ex, true);
				this.NameMinLen = 2;
				this.NameMaxLen = 7;
			}
		}

		
		public TCPProcessCmdResults ProcessChangeName(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleId = Convert.ToInt32(fields[0]);
				int zoneId = Convert.ToInt32(fields[1]);
				string newName = fields[2];
				string uid = GameManager.OnlineUserSession.FindUserID(socket);
				if (string.IsNullOrEmpty(uid))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名时，找不到socket对应的uid，其中roleid={0}，zoneid={1}，newname={2}", roleId, zoneId, newName), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ChangeNameResult result = new ChangeNameResult();
				if (socket.IsKuaFuLogin || GameManager.ClientMgr.FindClient(socket) != null)
				{
					result.ErrCode = 11;
				}
				else
				{
					result.ErrCode = (int)this.HandleChangeName(uid, zoneId, roleId, newName);
				}
				result.ZoneId = zoneId;
				result.NewName = newName;
				result.NameInfo = this.GetChangeNameInfo(uid, zoneId, socket.ServerId);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ChangeNameResult>(result, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private ChangeNameError HandleChangeName(string uid, int zoneId, int roleId, string newName)
		{
			ChangeNameError result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = ChangeNameError.ServerDenied;
			}
			else
			{
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(uid);
				if (pwdState != null && pwdState.NeedVerify)
				{
					result = ChangeNameError.NeedVerifySecPwd;
				}
				else if (string.IsNullOrEmpty(newName) || NameServerNamager.CheckInvalidCharacters(newName, false) <= 0)
				{
					result = ChangeNameError.InvalidName;
				}
				else if (!this.IsNameLengthOK(newName))
				{
					result = ChangeNameError.NameLengthError;
				}
				else if (NameServerNamager.RegisterNameToNameServer(zoneId, uid, new string[]
				{
					newName
				}, 0, roleId) <= 0)
				{
					result = ChangeNameError.NameAlreayUsed;
				}
				else
				{
					int canFreeMod = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("FreeModName") ? 1 : 0;
					int canZuanShiMod = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("ZuanShiModName") ? 1 : 0;
					string[] dbRet = Global.ExecuteDBCmd(14001, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
					{
						uid,
						zoneId,
						roleId,
						newName,
						this.CostZuanShiBase,
						this.CostZuanShiMax,
						canFreeMod,
						canZuanShiMod
					}), 0);
					if (dbRet == null || dbRet.Length != 4)
					{
						result = ChangeNameError.DBFailed;
					}
					else
					{
						int ec = Convert.ToInt32(dbRet[0]);
						string oldName = dbRet[1];
						int costDiamond = Convert.ToInt32(dbRet[2]);
						int leftDiamond = Convert.ToInt32(dbRet[3]);
						if (ec == 0)
						{
							if (costDiamond > 0)
							{
								string msg = "改名 " + oldName + " -> " + newName;
								EventLogManager.AddResourceEvent(uid, zoneId, roleId, MoneyTypes.YuanBao, (long)(-(long)costDiamond), (long)leftDiamond, msg);
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "改名", oldName, newName, "减少", costDiamond, zoneId, uid, leftDiamond, 0, null);
							}
							this._OnChangeNameSuccess(roleId, oldName, newName);
						}
						result = (ChangeNameError)ec;
					}
				}
			}
			return result;
		}

		
		private void _OnChangeNameSuccess(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				RoleName2IDs.OnChangeName(roleId, oldName, newName);
				MarryLogic.OnChangeName(roleId, oldName, newName);
				GameManager.ArenaBattleMgr.OnChangeName(roleId, oldName, newName);
				if (LuoLanChengZhanManager.getInstance().GetLuoLanChengZhuRoleID() == roleId)
				{
					LuoLanChengZhanManager.getInstance().OnChangeName(roleId, oldName, newName);
				}
				GameManager.BloodCastleCopySceneMgr.OnChangeName(roleId, oldName, newName);
				GameManager.DaimonSquareCopySceneMgr.OnChangeName(roleId, oldName, newName);
				GameManager.BattleMgr.OnChangeName(roleId, oldName, newName);
				GameManager.AngelTempleMgr.OnChangeName(roleId, oldName, newName);
				MonsterBossManager.OnChangeName(roleId, oldName, newName);
				JieRiGiveKingActivity gkAct = HuodongCachingMgr.GetJieriGiveKingActivity();
				if (gkAct != null)
				{
					gkAct.OnChangeName(roleId, oldName, newName);
				}
				JieRiRecvKingActivity rkAct = HuodongCachingMgr.GetJieriRecvKingActivity();
				if (rkAct != null)
				{
					rkAct.OnChangeName(roleId, oldName, newName);
				}
				AllyManager.getInstance().UnionLeaderChangName(roleId, oldName, newName);
				JunTuanManager.getInstance().OnRoleChangName(roleId, oldName, newName);
				CompManager.getInstance().OnChangeName(roleId, oldName, newName);
				RebornManager.getInstance().OnChangeName(roleId, oldName, newName);
			}
		}

		
		public bool IsNameLengthOK(string name)
		{
			return !string.IsNullOrEmpty(name) && name.Length >= this.NameMinLen && name.Length <= this.NameMaxLen;
		}

		
		public ChangeNameInfo GetChangeNameInfo(string uid, int zoneId, int serverId)
		{
			return Global.sendToDB<ChangeNameInfo, string>(14002, string.Format("{0}:{1}", uid, zoneId), serverId);
		}

		
		public void GM_ChangeNameTest(GameClient client, string newName)
		{
		}

		
		public void GM_SetFreeModName(int roleid, int count)
		{
			GameClient client = GameManager.ClientMgr.FindClient(roleid);
			if (client != null)
			{
				int leftCount = Global.GetRoleParamsInt32FromDB(client, "LeftFreeChangeNameTimes");
				int newCount = count + leftCount;
				Global.SaveRoleParamsInt32ValueToDB(client, "LeftFreeChangeNameTimes", newCount, true);
			}
			else
			{
				Global.UpdateRoleParamByNameOffline(roleid, "LeftFreeChangeNameTimes", count.ToString(), 0);
			}
		}

		
		public void GM_ChangeBangHuiName(GameClient client, string newName)
		{
			if (client != null)
			{
				this.HandleChangeBangHuiName(client, newName);
			}
		}

		
		public TCPProcessCmdResults ProcessChangeBangHuiName(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleId = Convert.ToInt32(fields[0]);
				string newName = fields[1];
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleId))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleId), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (client.ClientData.Faction <= 0)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EChangeGuildNameError ne = this.HandleChangeBangHuiName(client, newName);
				string rsp = string.Format("{0}:{1}:{2}", (int)ne, client.ClientData.Faction, newName);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), rsp, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private EChangeGuildNameError HandleChangeBangHuiName(GameClient client, string newName)
		{
			EChangeGuildNameError ne;
			if (string.IsNullOrEmpty(newName) || NameServerNamager.CheckInvalidCharacters(newName, false) <= 0)
			{
				ne = EChangeGuildNameError.InvalidName;
			}
			else if (!this.IsNameLengthOK(newName))
			{
				ne = EChangeGuildNameError.LengthError;
			}
			else
			{
				string[] result = Global.ExecuteDBCmd(14006, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, newName), client.ServerId);
				if (result == null || result.Length < 1)
				{
					ne = EChangeGuildNameError.DBFailed;
				}
				else
				{
					ne = (EChangeGuildNameError)Convert.ToInt32(result[0]);
				}
			}
			if (ne == EChangeGuildNameError.Success)
			{
				client.ClientData.BHName = newName;
				GameManager.ClientMgr.NotifyBangHuiChangeName(client.ClientData.Faction, newName);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				Global.UpdateBangHuiMiniDataName(client.ClientData.Faction, newName);
				LuoLanChengZhanManager.getInstance().ReShowLuolanKing(0);
				if (GameManager.ArenaBattleMgr.GetPKKingRoleID() == client.ClientData.RoleID)
				{
					GameManager.ArenaBattleMgr.ReShowPKKing();
				}
				AllyManager.getInstance().UnionDataChange(client.ClientData.Faction, client.ServerId, false, 0);
				JunTuanManager.getInstance().OnBangHuiChangeName(client.ClientData.Faction, newName);
			}
			return ne;
		}

		
		private int NameMinLen = 2;

		
		private int NameMaxLen = 7;

		
		public int CostZuanShiBase = 300;

		
		public int CostZuanShiMax = 1500;
	}
}
