using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000125 RID: 293
	internal class CGetOldResourceManager
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x00027D6C File Offset: 0x00025F6C
		public static TCPProcessCmdResults ProcessQueryGetResourceInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, OldResourceInfo> TmpDict = DBQuery.QueryResourceGetInfo(dbMgr, roleID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, OldResourceInfo>>(TmpDict, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00027ED8 File Offset: 0x000260D8
		public static TCPProcessCmdResults ProcessUpdateGetResourceInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				byte[] retBytes = DataHelper.ObjectToBytes<int>(0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, retBytes, 0, retBytes.Length, nID);
				Dictionary<int, Dictionary<int, OldResourceInfo>> tmpdict = DataHelper.BytesToObject<Dictionary<int, Dictionary<int, OldResourceInfo>>>(data, 0, count);
				Dictionary<int, OldResourceInfo> dict = tmpdict.Values.ToArray<Dictionary<int, OldResourceInfo>>()[0];
				if (tmpdict == null)
				{
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = tmpdict.Keys.ToArray<int>()[0];
				for (int i = 1; i < 20; i++)
				{
					OldResourceInfo info = null;
					if (dict != null)
					{
						dict.TryGetValue(i, out info);
					}
					int ret = DBWriter.UpdateResourceGetInfo(dbMgr, roleid, i, info);
					if (ret <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新资源找回失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleid), null, true);
					}
					retBytes = DataHelper.ObjectToBytes<int>(ret);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, retBytes, 0, retBytes.Length, nID);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
