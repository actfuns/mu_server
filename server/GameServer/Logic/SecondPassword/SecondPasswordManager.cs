using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.SecondPassword
{
	// Token: 0x02000794 RID: 1940
	public class SecondPasswordManager
	{
		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x0600327A RID: 12922 RVA: 0x002CD1F0 File Offset: 0x002CB3F0
		// (set) Token: 0x0600327B RID: 12923 RVA: 0x002CD208 File Offset: 0x002CB408
		public static long ValidSecWhenLogout
		{
			get
			{
				return SecondPasswordManager._ValidSecWhenLogout;
			}
			set
			{
				SecondPasswordManager._ValidSecWhenLogout = value;
				if (SecondPasswordManager._ValidSecWhenLogout < 0L)
				{
					SecondPasswordManager._ValidSecWhenLogout = 300L;
				}
			}
		}

		// Token: 0x0600327C RID: 12924 RVA: 0x002CD23C File Offset: 0x002CB43C
		public static SecPwdState GetSecPwdState(string userid)
		{
			SecPwdState result2;
			if (string.IsNullOrEmpty(userid))
			{
				result2 = null;
			}
			else
			{
				SecPwdState result = null;
				lock (SecondPasswordManager._UsrSecPwdDict)
				{
					SecondPasswordManager._UsrSecPwdDict.TryGetValue(userid, out result);
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600327D RID: 12925 RVA: 0x002CD2AC File Offset: 0x002CB4AC
		public static void SetSecPwdState(string usrid, SecPwdState state)
		{
			if (!string.IsNullOrEmpty(usrid))
			{
				lock (SecondPasswordManager._UsrSecPwdDict)
				{
					if (state != null)
					{
						SecondPasswordManager._UsrSecPwdDict[usrid] = state;
					}
					else
					{
						SecondPasswordManager._UsrSecPwdDict.Remove(usrid);
					}
				}
			}
		}

		// Token: 0x0600327E RID: 12926 RVA: 0x002CD328 File Offset: 0x002CB528
		public static TCPProcessCmdResults ProcessSetSecPwd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				SetSecondPassword setReq = DataHelper.BytesToObject<SetSecondPassword>(data, 0, count);
				if (setReq == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != setReq.RoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), setReq.RoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessSetSecPwd功能尚未开放, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), setReq.RoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(client.strUserID);
				string oldSecPwd_Encrypted = (pwdState != null) ? pwdState.SecPwd : null;
				SecondPasswordError error;
				if (!string.IsNullOrEmpty(oldSecPwd_Encrypted))
				{
					if (setReq.OldSecPwd == null || oldSecPwd_Encrypted != setReq.OldSecPwd)
					{
						error = SecondPasswordError.SecPwdVerifyFailed;
						goto IL_1F8;
					}
				}
				string newSecPwd_Decrypted = SecondPasswordRC4.Decrypt(setReq.NewSecPwd);
				if (string.IsNullOrEmpty(newSecPwd_Decrypted))
				{
					error = SecondPasswordError.SecPwdIsNull;
				}
				else if (!Regex.IsMatch(newSecPwd_Decrypted, "^[a-zA-Z0-9_]+$"))
				{
					error = SecondPasswordError.SecPwdCharInvalid;
				}
				else if (newSecPwd_Decrypted.Length < 6)
				{
					error = SecondPasswordError.SecPwdIsTooShort;
				}
				else if (newSecPwd_Decrypted.Length > 8)
				{
					error = SecondPasswordError.SecPwdIsTooLong;
				}
				else
				{
					string cmd2db = string.Format("{0}:{1}", client.strUserID, setReq.NewSecPwd);
					string[] dbFields = Global.ExecuteDBCmd(10183, cmd2db, client.ServerId);
					if (dbFields == null || dbFields.Length != 2)
					{
						error = SecondPasswordError.SecPwdDBFailed;
					}
					else
					{
						error = SecondPasswordError.SecPwdSetSuccess;
					}
				}
				IL_1F8:
				if (error == SecondPasswordError.SecPwdSetSuccess)
				{
					if (pwdState == null)
					{
						pwdState = new SecPwdState();
					}
					pwdState.SecPwd = setReq.NewSecPwd;
					pwdState.NeedVerify = false;
					SecondPasswordManager.SetSecPwdState(client.strUserID, pwdState);
				}
				int has = 0;
				int need = 0;
				if (pwdState != null)
				{
					has = 1;
					need = (pwdState.NeedVerify ? 1 : 0);
				}
				string rsp = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					setReq.RoleID,
					(int)error,
					has,
					need
				});
				GameManager.ClientMgr.SendToClient(client, rsp, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x002CD62C File Offset: 0x002CB82C
		public static TCPProcessCmdResults ProcClrSecPwd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				VerifySecondPassword verifyReq = DataHelper.BytesToObject<VerifySecondPassword>(data, 0, count);
				if (verifyReq == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string uid = GameManager.OnlineUserSession.FindUserID(socket);
				if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(verifyReq.UserID) || uid != verifyReq.UserID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("玩家请求清除二级密码，但是玩家发送的uid错误, {0}", Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(verifyReq.UserID);
				int errcode;
				if (pwdState == null)
				{
					errcode = 2;
				}
				else if (string.IsNullOrEmpty(verifyReq.SecPwd) || verifyReq.SecPwd != pwdState.SecPwd)
				{
					errcode = 1;
				}
				else if (!SecondPasswordManager.ClearUserSecPwd(verifyReq.UserID))
				{
					errcode = 8;
				}
				else
				{
					errcode = 9;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, errcode.ToString(), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06003280 RID: 12928 RVA: 0x002CD798 File Offset: 0x002CB998
		public static void OnUsrLogout(string userID)
		{
			SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(userID);
			if (pwdState != null)
			{
				pwdState.AuthDeadTime = TimeUtil.NowDateTime().AddSeconds((double)((int)SecondPasswordManager.ValidSecWhenLogout));
				SecondPasswordManager.SetSecPwdState(userID, pwdState);
			}
		}

		// Token: 0x06003281 RID: 12929 RVA: 0x002CD7DC File Offset: 0x002CB9DC
		public static SecPwdState InitUserState(string userID, bool alreadyOnline)
		{
			SecPwdState result2;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				result2 = null;
			}
			else if (string.IsNullOrEmpty(userID))
			{
				result2 = null;
			}
			else
			{
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(userID);
				if (pwdState == null)
				{
					string[] result = Global.ExecuteDBCmd(10184, userID, 0);
					if (result != null && result.Length == 2 && !string.IsNullOrEmpty(result[1]))
					{
						pwdState = new SecPwdState();
						pwdState.SecPwd = result[1];
						pwdState.NeedVerify = true;
					}
				}
				else
				{
					if (alreadyOnline)
					{
						pwdState.NeedVerify = true;
					}
					if (!pwdState.NeedVerify)
					{
						if (TimeUtil.NowDateTime() > pwdState.AuthDeadTime)
						{
							pwdState.NeedVerify = true;
						}
					}
				}
				if (pwdState != null)
				{
					SecondPasswordManager.SetSecPwdState(userID, pwdState);
				}
				result2 = pwdState;
			}
			return result2;
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x002CD8C0 File Offset: 0x002CBAC0
		public static TCPProcessCmdResults ProcessUsrCheckState(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (2 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string userid = fields[0];
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(userid);
				string cmdRsp;
				if (pwdState != null)
				{
					cmdRsp = string.Format("{0}:{1}", 1, pwdState.NeedVerify ? 1 : 0);
				}
				else
				{
					cmdRsp = string.Format("{0}:{1}", 0, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdRsp, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06003283 RID: 12931 RVA: 0x002CDA14 File Offset: 0x002CBC14
		public static TCPProcessCmdResults ProcessUsrVerify(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				VerifySecondPassword verifyReq = DataHelper.BytesToObject<VerifySecondPassword>(data, 0, count);
				if (verifyReq == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState pwdState = SecondPasswordManager.GetSecPwdState(verifyReq.UserID);
				int errcode;
				int has;
				int need;
				if (pwdState == null)
				{
					errcode = 0;
					has = 0;
					need = 0;
				}
				else if (string.IsNullOrEmpty(verifyReq.SecPwd) || verifyReq.SecPwd != pwdState.SecPwd)
				{
					errcode = 1;
					has = 1;
					need = 1;
				}
				else
				{
					errcode = 0;
					has = 1;
					need = 0;
					pwdState.NeedVerify = false;
				}
				string rsp = string.Format("{0}:{1}:{2}", errcode, has, need);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, rsp, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06003284 RID: 12932 RVA: 0x002CDB30 File Offset: 0x002CBD30
		private static bool Update2DB(string useid, string secpwd)
		{
			string cmd2db = string.Format("{0}:{1}", useid, secpwd);
			string[] dbFields = Global.ExecuteDBCmd(10183, cmd2db, 0);
			return dbFields != null && dbFields.Length == 2;
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x002CDB74 File Offset: 0x002CBD74
		private static bool Clear2DB(string userid)
		{
			return SecondPasswordManager.Update2DB(userid, "");
		}

		// Token: 0x06003286 RID: 12934 RVA: 0x002CDB94 File Offset: 0x002CBD94
		public static bool ClearUserSecPwd(string usrid)
		{
			bool result;
			if (string.IsNullOrEmpty(usrid))
			{
				result = false;
			}
			else
			{
				TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(usrid);
				GameClient otherClient = null;
				if (null != clientSocket)
				{
					otherClient = GameManager.ClientMgr.FindClient(clientSocket);
				}
				if (otherClient != null)
				{
					SecPwdState state = SecondPasswordManager.GetSecPwdState(usrid);
					if (state == null)
					{
						result = true;
					}
					else if (SecondPasswordManager.Clear2DB(usrid))
					{
						SecondPasswordManager.SetSecPwdState(usrid, null);
						int has = 0;
						int need = 0;
						string ntf = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							otherClient.ClientData.RoleID,
							7,
							has,
							need
						});
						GameManager.ClientMgr.SendToClient(otherClient, ntf, 861);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else if (SecondPasswordManager.Clear2DB(usrid))
				{
					SecondPasswordManager.SetSecPwdState(usrid, null);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04003EA4 RID: 16036
		private const int _PwdMinLen = 6;

		// Token: 0x04003EA5 RID: 16037
		private const int _PwdMaxLen = 8;

		// Token: 0x04003EA6 RID: 16038
		private static long _ValidSecWhenLogout = 300L;

		// Token: 0x04003EA7 RID: 16039
		private static Dictionary<string, SecPwdState> _UsrSecPwdDict = new Dictionary<string, SecPwdState>();
	}
}
