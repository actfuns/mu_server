using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	
	public class MerlinMagicBookManager
	{
		
		public static MerlinMagicBookManager getInstance()
		{
			return MerlinMagicBookManager.instance;
		}

		
		public TCPProcessCmdResults ProcessInsertMerlinDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MerlinGrowthSaveDBData updateMerlinData = null;
			int nRoleID = -1;
			bool bRet = false;
			try
			{
				nRoleID = BitConverter.ToInt32(data, 0);
				updateMerlinData = DataHelper.BytesToObject<MerlinGrowthSaveDBData>(data, 4, count - 4);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
					client.sendCmd<bool>(nID, bRet);
					return TCPProcessCmdResults.RESULT_OK;
				}
				DateTime now = DateTime.Now;
				string addTime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long ticks = now.Ticks / 10000L;
				bRet = MerlinDBOperate.InsertMerlinData(dbMgr, updateMerlinData, addTime);
				if (!bRet)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的梅林魔法书失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.MerlinData)
						{
							dbRoleInfo.MerlinData = new MerlinGrowthSaveDBData();
						}
						dbRoleInfo.MerlinData._RoleID = updateMerlinData._RoleID;
						dbRoleInfo.MerlinData._Occupation = updateMerlinData._Occupation;
						dbRoleInfo.MerlinData._Level = updateMerlinData._Level;
						dbRoleInfo.MerlinData._StarNum = updateMerlinData._StarNum;
						dbRoleInfo.MerlinData._StarExp = updateMerlinData._StarExp;
						dbRoleInfo.MerlinData._LuckyPoint = updateMerlinData._LuckyPoint;
						dbRoleInfo.MerlinData._ToTicks = updateMerlinData._ToTicks;
						dbRoleInfo.MerlinData._AddTime = ticks;
						dbRoleInfo.MerlinData._ActiveAttr = updateMerlinData._ActiveAttr;
						dbRoleInfo.MerlinData._UnActiveAttr = updateMerlinData._UnActiveAttr;
						MerlinRankManager.getInstance().createMerlinData(nRoleID);
					}
				}
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, bRet);
			return TCPProcessCmdResults.RESULT_OK;
		}

		
		public TCPProcessCmdResults ProcessUpdateMerlinDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			bool bRet = false;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 15)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd<bool>(nID, bRet);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
					client.sendCmd<bool>(nID, bRet);
					return TCPProcessCmdResults.RESULT_OK;
				}
				long lTmpToTicks = 0L;
				if (fields[6] != "*")
				{
					lTmpToTicks = Convert.ToInt64(fields[6]);
				}
				bRet = MerlinDBOperate.UpdateMerlinData(dbMgr, nRoleID, fields, 1);
				if (!bRet)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新梅林魔法书失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
				}
				else
				{
					dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
					if (null != dbRoleInfo)
					{
						lock (dbRoleInfo)
						{
							dbRoleInfo.MerlinData._Level = DataHelper.ConvertToInt32(fields[1], dbRoleInfo.MerlinData._Level);
							dbRoleInfo.MerlinData._LevelUpFailNum = DataHelper.ConvertToInt32(fields[2], dbRoleInfo.MerlinData._LevelUpFailNum);
							dbRoleInfo.MerlinData._StarNum = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.MerlinData._StarNum);
							dbRoleInfo.MerlinData._StarExp = DataHelper.ConvertToInt32(fields[4], dbRoleInfo.MerlinData._StarExp);
							dbRoleInfo.MerlinData._LuckyPoint = DataHelper.ConvertToInt32(fields[5], dbRoleInfo.MerlinData._LuckyPoint);
							if (fields[6] != "*")
							{
								dbRoleInfo.MerlinData._ToTicks = lTmpToTicks;
							}
							if (fields[7] != "*")
							{
								dbRoleInfo.MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(fields[7], 10) / 100);
							}
							if (fields[8] != "*")
							{
								dbRoleInfo.MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(fields[8], 10) / 100);
							}
							if (fields[9] != "*")
							{
								dbRoleInfo.MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(fields[9], 10) / 100);
							}
							if (fields[10] != "*")
							{
								dbRoleInfo.MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(fields[10], 10) / 100);
							}
							if (fields[11] != "*")
							{
								dbRoleInfo.MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(fields[11], 10) / 100);
							}
							if (fields[12] != "*")
							{
								dbRoleInfo.MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(fields[12], 10) / 100);
							}
							if (fields[13] != "*")
							{
								dbRoleInfo.MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(fields[13], 10) / 100);
							}
							if (fields[14] != "*")
							{
								dbRoleInfo.MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(fields[14], 10) / 100);
							}
						}
						MerlinRankingInfo MerlinInfo = MerlinRankManager.getInstance().getMerlinData(nRoleID);
						if (null != MerlinInfo)
						{
							if (MerlinInfo.nLevel != dbRoleInfo.MerlinData._Level || MerlinInfo.nStarNum != dbRoleInfo.MerlinData._StarNum)
							{
								MerlinInfo.nLevel = dbRoleInfo.MerlinData._Level;
								MerlinInfo.nStarNum = dbRoleInfo.MerlinData._StarNum;
								MerlinRankManager.getInstance().ModifyMerlinRankData(MerlinInfo, false);
							}
						}
					}
				}
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, bRet);
			return TCPProcessCmdResults.RESULT_OK;
		}

		
		public TCPProcessCmdResults ProcessQueryMerlinDataCmd(DBManager dbMgr, TCPOutPacketPool pool, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MerlinGrowthSaveDBData updateMerlinData = null;
			int nRoleID = -1;
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
				nRoleID = Convert.ToInt32(cmdData);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbRoleInfo)
				{
					lock (dbRoleInfo)
					{
						updateMerlinData = dbRoleInfo.MerlinData;
					}
				}
				else
				{
					updateMerlinData = MerlinDBOperate.QueryMerlinData(dbMgr, nRoleID);
				}
				if (null != updateMerlinData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(updateMerlinData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				updateMerlinData = new MerlinGrowthSaveDBData();
				for (int i = 0; i < 4; i++)
				{
					updateMerlinData._ActiveAttr[i] = 0.0;
					updateMerlinData._UnActiveAttr[i] = 0.0;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(updateMerlinData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static MerlinMagicBookManager instance = new MerlinMagicBookManager();
	}
}
