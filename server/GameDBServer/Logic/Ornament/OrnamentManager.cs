using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Ornament
{
	
	public class OrnamentManager
	{
		
		public static TCPProcessCmdResults ProcessUpdateOrnamentDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				OrnamentUpdateDbData dbData = DataHelper.BytesToObject<OrnamentUpdateDbData>(data, 0, count);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref dbData.RoleId);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, dbData.RoleId), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.OrnamentDataDict)
					{
						dbRoleInfo.OrnamentDataDict = new Dictionary<int, OrnamentData>();
					}
					dbRoleInfo.OrnamentDataDict[dbData.Data.ID] = dbData.Data;
				}
				DBWriter.UpdateOramentData(dbMgr, dbData);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<bool>(true, pool, nID);
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
