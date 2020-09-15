using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Marriage.CoupleWish;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x02000529 RID: 1321
	internal class MarryLogic
	{
		// Token: 0x06001905 RID: 6405 RVA: 0x00186124 File Offset: 0x00184324
		public static void LoadMarryBaseConfig()
		{
			MarryLogic.MarryCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("JieHunCost"));
			MarryLogic.MarryCD = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("QiuHuiCD"));
			MarryLogic.MarryReplyTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MarriageTipsTime"));
			MarryLogic.DivorceCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("DivorceJinBiCost"));
			MarryLogic.DivorceForceCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("DivorceZuanShiCost"));
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x001861B0 File Offset: 0x001843B0
		public static bool IsVersionSystemOpenOfMarriage()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot5) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Marriage");
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x001861E4 File Offset: 0x001843E4
		public static MarryApplyData AddMarryApply(int roleID, MarryApplyType type, int spouseID)
		{
			MarryApplyData data = null;
			lock (MarryLogic.MarryApplyList)
			{
				if (!MarryLogic.MarryApplyList.ContainsKey(roleID))
				{
					data = new MarryApplyData
					{
						ApplyExpireTime = TimeUtil.NOW() + (long)(MarryLogic.MarryReplyTime * 1000),
						ApplyCDEndTime = 0L,
						ApplySpouseRoleID = spouseID,
						ApplyType = type
					};
					if (type == MarryApplyType.ApplyInit)
					{
						data.ApplyCDEndTime = TimeUtil.NOW() + (long)(MarryLogic.MarryCD * 1000);
					}
					else
					{
						data.ApplyCDEndTime = data.ApplyExpireTime;
					}
					MarryLogic.MarryApplyList.Add(roleID, data);
				}
			}
			return data;
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x001862C0 File Offset: 0x001844C0
		public static bool RemoveMarryApply(int roleID, MarryApplyType type = MarryApplyType.ApplyNull)
		{
			bool result;
			lock (MarryLogic.MarryApplyList)
			{
				if (type == MarryApplyType.ApplyNull)
				{
					result = MarryLogic.MarryApplyList.Remove(roleID);
				}
				else
				{
					MarryApplyData applyData;
					bool ret = MarryLogic.MarryApplyList.TryGetValue(roleID, out applyData);
					if (ret)
					{
						if (applyData.ApplyType != type)
						{
							ret = false;
						}
						else if (applyData.ApplyExpireTime == 0L)
						{
							ret = false;
						}
						else if (applyData.ApplyExpireTime <= TimeUtil.NOW())
						{
							ret = false;
						}
						else
						{
							applyData.ApplyExpireTime = 0L;
						}
					}
					result = ret;
				}
			}
			return result;
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x00186398 File Offset: 0x00184598
		public static void ApplyPeriodicClear(long ticks)
		{
			if (ticks >= MarryLogic.NextPeriodicCheckTime)
			{
				MarryLogic.NextPeriodicCheckTime = ticks + 10000L;
				lock (MarryLogic.MarryApplyList)
				{
					foreach (KeyValuePair<int, MarryApplyData> it in MarryLogic.MarryApplyList.ToList<KeyValuePair<int, MarryApplyData>>())
					{
						MarryApplyData applyData = it.Value;
						if (applyData.ApplyExpireTime > 0L && applyData.ApplyExpireTime <= ticks)
						{
							MarryLogic.ApplyReturnMoney(it.Key, applyData, null);
							applyData.ApplyExpireTime = 0L;
						}
						if (applyData.ApplyCDEndTime <= ticks)
						{
							MarryLogic.MarryApplyList.Remove(it.Key);
						}
					}
				}
			}
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x001864A4 File Offset: 0x001846A4
		public static void ApplyShutdownClear()
		{
			lock (MarryLogic.MarryApplyList)
			{
				foreach (KeyValuePair<int, MarryApplyData> it in MarryLogic.MarryApplyList)
				{
					MarryApplyData applyData = it.Value;
					if (applyData.ApplyExpireTime > 0L)
					{
						MarryLogic.ApplyReturnMoney(it.Key, applyData, null);
					}
				}
				MarryLogic.MarryApplyList.Clear();
			}
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x00186568 File Offset: 0x00184768
		public static void ApplyLogoutClear(GameClient client)
		{
			lock (MarryLogic.MarryApplyList)
			{
				MarryApplyData applyData;
				if (MarryLogic.MarryApplyList.TryGetValue(client.ClientData.RoleID, out applyData))
				{
					if (applyData.ApplyExpireTime > 0L)
					{
						MarryLogic.ApplyReturnMoney(0, applyData, client);
						applyData.ApplyExpireTime = 0L;
					}
				}
			}
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x001865F0 File Offset: 0x001847F0
		public static void ApplyReturnMoney(int roleID, MarryApplyData applyData, GameClient client = null)
		{
			if (client == null)
			{
				client = GameManager.ClientMgr.FindClient(roleID);
			}
			if (client != null)
			{
				if (applyData.ApplyType == MarryApplyType.ApplyInit)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.MarryCost, "求婚超时返回钻石", ActivityTypes.None, "");
				}
				else if (applyData.ApplyType == MarryApplyType.ApplyDivorce)
				{
					GameManager.ClientMgr.AddMoney1(client, MarryLogic.DivorceCost, "离婚超时返还绑金", true);
				}
			}
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x0018669C File Offset: 0x0018489C
		public static bool ApplyExist(int roleID)
		{
			lock (MarryLogic.MarryApplyList)
			{
				foreach (KeyValuePair<int, MarryApplyData> kv in MarryLogic.MarryApplyList)
				{
					if (roleID == kv.Value.ApplySpouseRoleID || roleID == kv.Key)
					{
						if (kv.Value.ApplyExpireTime > 0L)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x00186774 File Offset: 0x00184974
		public static TCPProcessCmdResults ProcessMarryInit(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int spouseID = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", -12, roleID, spouseID), false);
					tcpOutPacket = null;
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryResult result = MarryLogic.MarryInit(client, spouseID);
				string strcmd = string.Format("{0}:{1}:{2}", (int)result, roleID, spouseID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x0018699C File Offset: 0x00184B9C
		public static TCPProcessCmdResults ProcessMarryReply(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int sourceID = Convert.ToInt32(fields[1]);
				int accept = Convert.ToInt32(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult result = MarryLogic.MarryReply(client, sourceID, accept);
				string strcmd = string.Format("{0}:{1}:{2}", (int)result, roleID, sourceID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00186B48 File Offset: 0x00184D48
		public static TCPProcessCmdResults ProcessMarryDivorce(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int divorceType = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult result = MarryLogic.MarryDivorce(client, (MarryDivorceType)divorceType);
				string strcmd = string.Format("{0}", (int)result);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x00186CC4 File Offset: 0x00184EC4
		public static TCPProcessCmdResults ProcessMarryAutoReject(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int autoReject = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult result = MarryLogic.MarryAutoReject(client, autoReject);
				string strcmd = string.Format("{0}:{1}", (int)result, client.ClientData.MyMarriageData.byAutoReject);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00186E6C File Offset: 0x0018506C
		public static bool SameSexMarry(bool diff = false)
		{
			return diff || GameManager.PlatConfigMgr.GetGameConfigItemStr("SameSexMarry", "0") == "1";
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x00186EA4 File Offset: 0x001850A4
		public static MarryResult MarryInit(GameClient client, int spouseID)
		{
			MarryResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryResult.Error_Denied_For_Minor_Occupation;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Marriage, true) || !MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (client.ClientData.MyMarriageData.byMarrytype > 0)
			{
				result = MarryResult.SelfMarried;
			}
			else if (client.ClientData.ChangeLifeCount < 3)
			{
				result = MarryResult.SelfLevelNotEnough;
			}
			else if (client.ClientData.ExchangeID > 0 || client.ClientSocket.IsKuaFuLogin || client.ClientData.CopyMapID > 0)
			{
				result = MarryResult.SelfBusy;
			}
			else
			{
				GameClient spouseClient = GameManager.ClientMgr.FindClient(spouseID);
				if (spouseClient == null)
				{
					result = MarryResult.TargetOffline;
				}
				else if (!spouseClient.ClientData.IsMainOccupation)
				{
					result = MarryResult.Error_Denied_For_Minor_Occupation;
				}
				else if (!GlobalNew.IsGongNengOpened(spouseClient, GongNengIDs.Marriage, false))
				{
					result = MarryResult.TargetNotOpen;
				}
				else
				{
					if (!MarryLogic.SameSexMarry(false))
					{
						if (client.ClientData.RoleSex == spouseClient.ClientData.RoleSex)
						{
							return MarryResult.InvalidSex;
						}
					}
					if (spouseClient.ClientData.MyMarriageData.byMarrytype > 0)
					{
						result = MarryResult.TargetMarried;
					}
					else if (spouseClient.ClientData.ChangeLifeCount < 3)
					{
						result = MarryResult.TargetLevelNotEnough;
					}
					else if (spouseClient.ClientData.ExchangeID > 0 || spouseClient.ClientSocket.IsKuaFuLogin || spouseClient.ClientData.CopyMapID > 0)
					{
						result = MarryResult.TargetBusy;
					}
					else if (MarryLogic.ApplyExist(spouseID))
					{
						result = MarryResult.TargetBusy;
					}
					else if (spouseClient.ClientData.MyMarriageData.byAutoReject == 1)
					{
						result = MarryResult.AutoReject;
					}
					else if (MarryLogic.AddMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyInit, spouseID) == null)
					{
						result = MarryResult.ApplyCD;
					}
					else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.MarryCost, "结婚", false, true, false, DaiBiSySType.None))
					{
						MarryLogic.RemoveMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyNull);
						result = MarryResult.MoneyNotEnough;
					}
					else
					{
						string notifyData = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, client.ClientData.RoleName);
						spouseClient.sendCmd(894, notifyData, false);
						result = MarryResult.Success;
					}
				}
			}
			return result;
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x00187150 File Offset: 0x00185350
		public static MarryResult MarryReply(GameClient client, int sourceID, int accept)
		{
			MarryResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (client.ClientData.MyMarriageData.byMarrytype > 0)
			{
				result = MarryResult.SelfMarried;
			}
			else
			{
				GameClient sourceClient = GameManager.ClientMgr.FindClient(sourceID);
				if (sourceClient == null)
				{
					result = MarryResult.ApplyTimeout;
				}
				else if (sourceClient.ClientData.MyMarriageData.byMarrytype > 0)
				{
					result = MarryResult.TargetMarried;
				}
				else if (!MarryLogic.RemoveMarryApply(sourceID, MarryApplyType.ApplyInit))
				{
					result = MarryResult.ApplyTimeout;
				}
				else
				{
					if (!client.ClientData.IsMainOccupation || !sourceClient.ClientData.IsMainOccupation)
					{
						accept = 0;
					}
					if (accept == 0 || client.ClientData.MyMarriageData.byAutoReject == 1)
					{
						string notifyData = string.Format("{0}:{1}:{2}", 1, client.ClientData.RoleID, client.ClientData.RoleName);
						sourceClient.sendCmd(894, notifyData, false);
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, sourceClient, MarryLogic.MarryCost, "求婚被拒绝返还钻石", ActivityTypes.None, "");
					}
					else
					{
						MarryLogic.RemoveMarryApply(sourceID, MarryApplyType.ApplyNull);
						MarryLogic.ApplyLogoutClear(client);
						MarryLogic.RemoveMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyNull);
						int initRingID = 0;
						if (null != MarriageOtherLogic.getInstance().WeddingRingDic.SystemXmlItemDict)
						{
							initRingID = MarriageOtherLogic.getInstance().WeddingRingDic.SystemXmlItemDict.Keys.First<int>();
						}
						if (sourceClient.ClientData.MyMarriageData.nRingID <= 0)
						{
							sourceClient.ClientData.MyMarriageData.nRingID = initRingID;
						}
						if (client.ClientData.MyMarriageData.nRingID <= 0)
						{
							client.ClientData.MyMarriageData.nRingID = initRingID;
						}
						sbyte sourceType = (sourceClient.ClientData.RoleSex != 1 || client.ClientData.RoleSex == sourceClient.ClientData.RoleSex) ? 1 : 2;
						sourceClient.ClientData.MyMarriageData.byMarrytype = sourceType;
						client.ClientData.MyMarriageData.byMarrytype = ((sourceType == 1) ? 2 : 1);
						sourceClient.ClientData.MyMarriageData.nSpouseID = client.ClientData.RoleID;
						client.ClientData.MyMarriageData.nSpouseID = sourceID;
						if (sourceClient.ClientData.MyMarriageData.byGoodwilllevel == 0)
						{
							sourceClient.ClientData.MyMarriageData.ChangTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
							sourceClient.ClientData.MyMarriageData.byGoodwilllevel = 1;
						}
						if (client.ClientData.MyMarriageData.byGoodwilllevel == 0)
						{
							client.ClientData.MyMarriageData.ChangTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
							client.ClientData.MyMarriageData.byGoodwilllevel = 1;
						}
						EventLogManager.AddRingBuyEvent(sourceClient, 0, initRingID, 0, 0, 0, 1, "");
						EventLogManager.AddRingBuyEvent(client, 0, initRingID, 0, 0, 0, 1, "");
						MarryFuBenMgr.UpdateMarriageData2DB(sourceClient);
						MarryFuBenMgr.UpdateMarriageData2DB(client);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(sourceClient, true);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
						MarriageOtherLogic.getInstance().UpdateRingAttr(sourceClient, true, false);
						if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
						{
							client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
							client._IconStateMgr.SendIconStateToClient(client);
						}
						if (sourceClient._IconStateMgr.CheckJieRiFanLi(sourceClient, ActivityTypes.JieriMarriage))
						{
							sourceClient._IconStateMgr.AddFlushIconState(14000, sourceClient._IconStateMgr.IsAnyJieRiTipActived());
							sourceClient._IconStateMgr.SendIconStateToClient(sourceClient);
						}
						FriendData friendData = Global.FindFriendData(client, sourceID);
						if (friendData != null && friendData.FriendType != 0)
						{
							GameManager.ClientMgr.RemoveFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, friendData.DbID);
							friendData = null;
						}
						if (friendData == null)
						{
							GameManager.ClientMgr.AddFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, -1, sourceID, Global.FormatRoleName(sourceClient, sourceClient.ClientData.RoleName), 0);
						}
						friendData = Global.FindFriendData(sourceClient, client.ClientData.RoleID);
						if (friendData != null && friendData.FriendType != 0)
						{
							GameManager.ClientMgr.RemoveFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, sourceClient, friendData.DbID);
							friendData = null;
						}
						if (friendData == null)
						{
							GameManager.ClientMgr.AddFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, sourceClient, -1, client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), 0);
						}
						string broadCastMsg = string.Format(GLang.GetLang(485, new object[0]), sourceClient.ClientData.RoleName, client.ClientData.RoleName);
						Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
						SingletonTemplate<CoupleArenaManager>.Instance().OnMarry(sourceClient, client);
					}
					result = MarryResult.Success;
				}
			}
			return result;
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x00187710 File Offset: 0x00185910
		public static MarryResult MarryDivorce(GameClient client, MarryDivorceType divorceType)
		{
			MarryResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (0 >= client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryResult.NotMarried;
			}
			else if (!SingletonTemplate<CoupleArenaManager>.Instance().IsNowCanDivorce(TimeUtil.NowDateTime()))
			{
				result = MarryResult.DeniedByCoupleAreanTime;
			}
			else
			{
				int spouseID = client.ClientData.MyMarriageData.nSpouseID;
				GameClient spouseClient = GameManager.ClientMgr.FindClient(spouseID);
				if (divorceType == MarryDivorceType.DivorceForce || divorceType == MarryDivorceType.DivorceFree || divorceType == MarryDivorceType.DivorceFreeAccept)
				{
					if (client.ClientData.ExchangeID > 0 || client.ClientSocket.IsKuaFuLogin || client.ClientData.CopyMapID > 0)
					{
						return MarryResult.SelfBusy;
					}
					if (-1 != client.ClientData.FuBenID && MapTypes.MarriageCopy == Global.GetMapType(client.ClientData.MapCode))
					{
						return MarryResult.SelfBusy;
					}
					if (null != spouseClient)
					{
						if (-1 != spouseClient.ClientData.FuBenID && MapTypes.MarriageCopy == Global.GetMapType(spouseClient.ClientData.MapCode))
						{
							return MarryResult.TargetBusy;
						}
					}
					if (divorceType == MarryDivorceType.DivorceForce || divorceType == MarryDivorceType.DivorceFree)
					{
						if (MarryLogic.ApplyExist(client.ClientData.RoleID))
						{
							return MarryResult.SelfBusy;
						}
					}
				}
				int _man = client.ClientData.RoleID;
				int _wife = spouseID;
				if (client.ClientData.MyMarriageData.byMarrytype == 2)
				{
					DataHelper2.Swap<int>(ref _man, ref _wife);
				}
				if (divorceType == MarryDivorceType.DivorceForce)
				{
					if (client.ClientData.UserMoney < MarryLogic.DivorceForceCost)
					{
						return MarryResult.MoneyNotEnough;
					}
					if (!SingletonTemplate<CoupleWishManager>.Instance().PreClearDivorceData(_man, _wife))
					{
						return MarryResult.NotOpen;
					}
					if (!SingletonTemplate<CoupleArenaManager>.Instance().PreClearDivorceData(_man, _wife))
					{
						return MarryResult.NotOpen;
					}
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.DivorceForceCost, "强制离婚", false, true, false, DaiBiSySType.None))
					{
					}
					client.ClientData.MyMarriageData.byMarrytype = -1;
					client.ClientData.MyMarriageData.nSpouseID = -1;
					MarryFuBenMgr.UpdateMarriageData2DB(client);
					MarriageOtherLogic.getInstance().ResetRingAttr(client);
					if (null != spouseClient)
					{
						spouseClient.ClientData.MyMarriageData.nSpouseID = -1;
						spouseClient.ClientData.MyMarriageData.byMarrytype = -1;
						MarryFuBenMgr.UpdateMarriageData2DB(spouseClient);
						MarriageOtherLogic.getInstance().ResetRingAttr(spouseClient);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(spouseClient, true);
						if (spouseClient._IconStateMgr.CheckJieRiFanLi(spouseClient, ActivityTypes.JieriMarriage))
						{
							spouseClient._IconStateMgr.AddFlushIconState(14000, spouseClient._IconStateMgr.IsAnyJieRiTipActived());
							spouseClient._IconStateMgr.SendIconStateToClient(spouseClient);
						}
					}
					else
					{
						string tcpstring = string.Format("{0}", spouseID);
						MarriageData spouseMarriageData = Global.sendToDB<MarriageData, string>(10186, tcpstring, client.ServerId);
						if (spouseMarriageData != null && 0 < spouseMarriageData.byMarrytype)
						{
							spouseMarriageData.byMarrytype = -1;
							spouseMarriageData.nSpouseID = -1;
							MarryFuBenMgr.UpdateMarriageData2DB(spouseID, spouseMarriageData, client);
						}
					}
					MarryPartyLogic.getInstance().MarryPartyRemove(client.ClientData.RoleID, true, client);
					MarryPartyLogic.getInstance().MarryPartyRemove(spouseID, true, client);
					MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string msg = string.Format(GLang.GetLang(486, new object[0]), client.ClientData.RoleName);
					MarryLogic.SendDivorceMail(spouseID, GLang.GetLang(487, new object[0]), msg, spouseClient, client.ServerId);
					SingletonTemplate<CoupleArenaManager>.Instance().OnDivorce(client.ClientData.RoleID, spouseID);
				}
				else if (divorceType == MarryDivorceType.DivorceFree)
				{
					if (null == spouseClient)
					{
						return MarryResult.TargetOffline;
					}
					if (spouseClient.ClientData.ExchangeID > 0 || spouseClient.ClientSocket.IsKuaFuLogin || spouseClient.ClientData.CopyMapID > 0)
					{
						return MarryResult.TargetBusy;
					}
					if (Global.GetTotalBindTongQianAndTongQianVal(client) < MarryLogic.DivorceCost)
					{
						return MarryResult.MoneyNotEnough;
					}
					if (!Global.SubBindTongQianAndTongQian(client, MarryLogic.DivorceCost, "申请离婚"))
					{
						return MarryResult.MoneyNotEnough;
					}
					MarryLogic.AddMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyDivorce, spouseID);
					string notifyData = string.Format("{0}:{1}", client.ClientData.RoleID, 1);
					spouseClient.sendCmd(892, notifyData, false);
					SingletonTemplate<CoupleArenaManager>.Instance().OnSpouseRequestDivorce(spouseClient, client);
				}
				else
				{
					if (null == spouseClient)
					{
						return MarryResult.TargetOffline;
					}
					if (!MarryLogic.RemoveMarryApply(spouseID, MarryApplyType.ApplyDivorce))
					{
						return MarryResult.ApplyTimeout;
					}
					MarryLogic.RemoveMarryApply(spouseID, MarryApplyType.ApplyNull);
					if (divorceType == MarryDivorceType.DivorceFreeAccept)
					{
						if (SingletonTemplate<CoupleWishManager>.Instance().PreClearDivorceData(_man, _wife) && SingletonTemplate<CoupleArenaManager>.Instance().PreClearDivorceData(_man, _wife))
						{
							client.ClientData.MyMarriageData.byMarrytype = -1;
							client.ClientData.MyMarriageData.nSpouseID = -1;
							spouseClient.ClientData.MyMarriageData.byMarrytype = -1;
							spouseClient.ClientData.MyMarriageData.nSpouseID = -1;
							MarryFuBenMgr.UpdateMarriageData2DB(client);
							MarryFuBenMgr.UpdateMarriageData2DB(spouseClient);
							MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
							MarriageOtherLogic.getInstance().SendMarriageDataToClient(spouseClient, true);
							MarriageOtherLogic.getInstance().ResetRingAttr(client);
							MarriageOtherLogic.getInstance().ResetRingAttr(spouseClient);
							MarryPartyLogic.getInstance().MarryPartyRemove(client.ClientData.RoleID, true, client);
							MarryPartyLogic.getInstance().MarryPartyRemove(spouseID, true, client);
							if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
							{
								client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
								client._IconStateMgr.SendIconStateToClient(client);
							}
							if (spouseClient._IconStateMgr.CheckJieRiFanLi(spouseClient, ActivityTypes.JieriMarriage))
							{
								spouseClient._IconStateMgr.AddFlushIconState(14000, spouseClient._IconStateMgr.IsAnyJieRiTipActived());
								spouseClient._IconStateMgr.SendIconStateToClient(spouseClient);
							}
							SingletonTemplate<CoupleArenaManager>.Instance().OnDivorce(client.ClientData.RoleID, spouseID);
						}
						else
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, spouseClient, MarryLogic.DivorceCost, "自由离婚拒绝返还绑金", false);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(488, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, spouseClient, StringUtil.substitute(GLang.GetLang(488, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					else if (divorceType == MarryDivorceType.DivorceFreeReject)
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, spouseClient, MarryLogic.DivorceCost, "自由离婚拒绝返还绑金", false);
						string notifyData = string.Format("{0}:{1}", client.ClientData.RoleID, 3);
						spouseClient.sendCmd(892, notifyData, false);
					}
				}
				result = MarryResult.Success;
			}
			return result;
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x00187F6C File Offset: 0x0018616C
		public static bool SendDivorceMail(int roleID, string subject, string content, GameClient client, int serverId)
		{
			string mailGoodsString = "";
			string strDbCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				-1,
				GLang.GetLang(112, new object[0]),
				roleID,
				"",
				subject,
				content,
				0,
				0,
				0,
				mailGoodsString
			});
			string[] fieldsData = Global.ExecuteDBCmd(10086, strDbCmd, serverId);
			if (client != null)
			{
				client._IconStateMgr.CheckEmailCount(client, true);
			}
			return fieldsData == null;
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x00188028 File Offset: 0x00186228
		public static MarryResult MarryAutoReject(GameClient client, int autoReject)
		{
			if ((int)client.ClientData.MyMarriageData.byAutoReject != autoReject)
			{
				client.ClientData.MyMarriageData.byAutoReject = (sbyte)autoReject;
			}
			MarryFuBenMgr.UpdateMarriageData2DB(client);
			return MarryResult.Success;
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x00188070 File Offset: 0x00186270
		public static bool IsMarried(int roleID)
		{
			RoleDataEx roleDataEx = MarryLogic.GetOfflineRoleData(roleID);
			if (roleDataEx != null && roleDataEx.MyMarriageData != null)
			{
				if (roleDataEx.MyMarriageData.byMarrytype != -1)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x001880B8 File Offset: 0x001862B8
		public static int GetSpouseID(int roleID)
		{
			RoleDataEx roleDataEx = MarryLogic.GetOfflineRoleData(roleID);
			return (roleDataEx != null && roleDataEx.MyMarriageData != null) ? roleDataEx.MyMarriageData.nSpouseID : -1;
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x001880EC File Offset: 0x001862EC
		public static string GetRoleName(int roleID)
		{
			RoleDataEx roleDataEx = MarryLogic.GetOfflineRoleData(roleID);
			return (roleDataEx != null) ? roleDataEx.RoleName : "";
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x00188118 File Offset: 0x00186318
		public static RoleDataEx GetOfflineRoleData(int roleID)
		{
			GameClient client = GameManager.ClientMgr.FindClient(roleID);
			RoleDataEx result;
			if (null != client)
			{
				result = client.ClientData.GetRoleData();
			}
			else
			{
				SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleID);
				result = ((clientData != null) ? clientData.GetRoleData() : null);
			}
			return result;
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00188164 File Offset: 0x00186364
		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (clientData != null && clientData.MyMarriageData != null && clientData.MyMarriageData.nSpouseID != -1)
				{
					GameClient spouseClient = GameManager.ClientMgr.FindClient(clientData.MyMarriageData.nSpouseID);
					if (spouseClient != null)
					{
						MarriageOtherLogic.getInstance().SendSpouseDataToClient(spouseClient);
					}
					MarryPartyLogic.getInstance().OnChangeName(roleId, oldName, newName);
				}
			}
		}

		// Token: 0x04002325 RID: 8997
		public static Dictionary<int, MarryApplyData> MarryApplyList = new Dictionary<int, MarryApplyData>();

		// Token: 0x04002326 RID: 8998
		public static long NextPeriodicCheckTime = 0L;

		// Token: 0x04002327 RID: 8999
		private static int MarryCost;

		// Token: 0x04002328 RID: 9000
		private static int MarryCD;

		// Token: 0x04002329 RID: 9001
		private static int MarryReplyTime;

		// Token: 0x0400232A RID: 9002
		private static int DivorceCost;

		// Token: 0x0400232B RID: 9003
		private static int DivorceForceCost;
	}
}
