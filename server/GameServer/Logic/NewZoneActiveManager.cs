using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200076B RID: 1899
	public class NewZoneActiveManager
	{
		// Token: 0x060030CA RID: 12490 RVA: 0x002B4DB8 File Offset: 0x002B2FB8
		public static TCPProcessCmdResults ProcessQueryLevelUpMadmanCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (NewZoneActiveManager.QueryLevelUpMadman(client, pool, nID, out tcpOutPacket))
				{
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessQueryLevelUpMadmanCmd", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x002B4F14 File Offset: 0x002B3114
		public static bool QueryLevelUpMadman(GameClient client, TCPOutPacketPool pool, int nID, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				KingActivity instActivity = (KingActivity)Global.GetActivity(ActivityTypes.NewZoneUpLevelMadman);
				NewZoneUpLevelData data = new NewZoneUpLevelData();
				int count = instActivity.RoleLimit.Count;
				data.Items = new List<NewZoneUpLevelItemData>();
				for (int i = 1; i < count + 1; i++)
				{
					NewZoneUpLevelItemData item = new NewZoneUpLevelItemData();
					AwardItem awd = instActivity.GetAward(client, i);
					item.LeftNum = awd.MinAwardCondionValue2 - Global.GetChongJiLingQuShenZhuangQuota(client, i);
					item.GetAward = !Global.CanGetChongJiLingQuShenZhuang(client, i);
					data.Items.Add(item);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<NewZoneUpLevelData>(data, pool, nID);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "LevelUpMadman", false, false);
			}
			return false;
		}

		// Token: 0x060030CC RID: 12492 RVA: 0x002B4FEC File Offset: 0x002B31EC
		public static TCPProcessCmdResults ProcessGetActiveInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(fields[0]);
				int activetype = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				return Global.RequestToDBServer2(tcpClientPool, pool, nID, Global.GetActivityRequestCmdString((ActivityTypes)activetype, client, 0), out tcpOutPacket, client.ServerId);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060030CD RID: 12493 RVA: 0x002B5174 File Offset: 0x002B3374
		private static TCPProcessCmdResults GetNewLevelUpMadmanAward(GameClient client, TCPOutPacketPool pool, int nID, int nRoleID, int nActivityType, int nBtnIndex, out TCPOutPacket tcpOutPacket)
		{
			Activity instActivity = Global.GetActivity((ActivityTypes)nActivityType);
			TCPProcessCmdResults result;
			if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
			{
				string strcmd = string.Format("{0}:{1}:0", -20, nActivityType);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				int nChangeLifeLev = client.ClientData.ChangeLifeCount;
				AwardItem tmpItem = instActivity.GetAward(client, nOcc, 1);
				if (tmpItem == null)
				{
					string strcmd = string.Format("{0}:{1}:0", -1, nActivityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else if (!instActivity.CanGiveAward())
				{
					string strcmd = string.Format("{0}:{1}:0", -10, nActivityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					tmpItem = instActivity.GetAward(client, nBtnIndex);
					if (nChangeLifeLev < tmpItem.MinAwardCondionValue)
					{
						string strcmd = string.Format("{0}:{1}:0", -100, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else if (nChangeLifeLev == tmpItem.MinAwardCondionValue && client.ClientData.Level < tmpItem.MinAwardCondionValue3)
					{
						string strcmd = string.Format("{0}:{1}:0", -101, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else if (!Global.CanGetChongJiLingQuShenZhuang(client, nBtnIndex))
					{
						string strcmd = string.Format("{0}:{1}:0", -103, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else
					{
						int nQuota = Global.GetChongJiLingQuShenZhuangQuota(client, nBtnIndex);
						if (nQuota >= tmpItem.MinAwardCondionValue2)
						{
							string strcmd = string.Format("{0}:{1}:0", -102, nActivityType);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
						else
						{
							instActivity.GiveAward(client, nBtnIndex, nOcc);
							Global.CompleteChongJiLingQuShenZhuang(client, nBtnIndex, nQuota + 1);
							AwardItem tmpItem2 = instActivity.GetAward(client, nBtnIndex, 2);
							if (tmpItem2 != null && tmpItem2.GoodsDataList.Count > 0)
							{
								Global.BroadcastChongJiLingQuShengZhuangHint(client, nBtnIndex, tmpItem2.GoodsDataList[nOcc].GoodsID);
							}
							string strcmd = string.Format("{0}:{1}:{2}", 1, nActivityType, nBtnIndex);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060030CE RID: 12494 RVA: 0x002B5414 File Offset: 0x002B3614
		public static TCPProcessCmdResults ProcessGetActiveAwards(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(fields[0]);
				int activityType = Global.SafeConvertToInt32(fields[1]);
				int extTag = Global.SafeConvertToInt32(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				switch (activityType)
				{
				case 33:
					return NewZoneActiveManager.GetNewLevelUpMadmanAward(client, pool, nID, roleID, activityType, extTag, out tcpOutPacket);
				case 34:
				case 35:
				case 36:
				case 37:
					return NewZoneActiveManager.GetActiveAwards(client, tcpClientPool, pool, nID, roleID, activityType, out tcpOutPacket);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060030CF RID: 12495 RVA: 0x002B55FC File Offset: 0x002B37FC
		private static TCPProcessCmdResults GetActiveAwards(GameClient client, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, int roleID, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				Activity instActivity = Global.GetActivity((ActivityTypes)activityType);
				string strcmd;
				if (null == instActivity)
				{
					strcmd = string.Format("{0}:{1}:0", -1, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!instActivity.CanGiveAward())
				{
					strcmd = string.Format("{0}:{1}:0", -10, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!instActivity.HasEnoughBagSpaceForAwardGoods(client))
				{
					strcmd = string.Format("{0}:{1}:0", -20, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] dbFields = null;
				int nDBExecuteID = 629;
				string dbCmds = Global.GetActivityRequestCmdString((ActivityTypes)activityType, client, activityType);
				if (nDBExecuteID <= 0 || string.IsNullOrEmpty(dbCmds))
				{
					strcmd = string.Format("{0}:{1}:0", -4, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Global.RequestToDBServer(tcpClientPool, pool, nDBExecuteID, dbCmds, out dbFields, client.ServerId);
				if (dbFields == null || dbFields.Length != 3)
				{
					strcmd = string.Format("{0}:{1}:0", -5, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int result = Global.SafeConvertToInt32(dbFields[0]);
				if (result <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", result, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!instActivity.GiveAward(client, Global.SafeConvertToInt32(dbFields[1])))
				{
					strcmd = string.Format("{0}:{1}:0", -7, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, activityType, dbFields[1]);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "GetActiveAwards", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
