using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001EB RID: 491
	public class UpdateBuildingDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A3C RID: 2620 RVA: 0x00061AB0 File Offset: 0x0005FCB0
		private UpdateBuildingDataCmdProcessor()
		{
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00061ABC File Offset: 0x0005FCBC
		public static UpdateBuildingDataCmdProcessor getInstance()
		{
			return UpdateBuildingDataCmdProcessor.instance;
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00061AD4 File Offset: 0x0005FCD4
		public void processCmdUpdateBuildLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				string TimeLog = fields[0];
				BuildingLogType LogType = (BuildingLogType)Convert.ToInt32(fields[1]);
				DBManager dbMgr = DBManager.getInstance();
				DBWriter.UpdateBuildingLog(dbMgr, TimeLog, LogType);
				string strcmd = string.Format("{0}", 0);
				client.sendCmd(nID, strcmd);
			}
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00061BD0 File Offset: 0x0005FDD0
		public void processCmdUpdateBuildData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 9)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				BuildingData myBuildData = new BuildingData();
				int roleID = Convert.ToInt32(fields[0]);
				myBuildData.BuildId = Convert.ToInt32(fields[1]);
				myBuildData.BuildLev = Convert.ToInt32(fields[2]);
				myBuildData.BuildExp = Convert.ToInt32(fields[3]);
				myBuildData.TaskID_1 = Convert.ToInt32(fields[5]);
				myBuildData.TaskID_2 = Convert.ToInt32(fields[6]);
				myBuildData.TaskID_3 = Convert.ToInt32(fields[7]);
				myBuildData.TaskID_4 = Convert.ToInt32(fields[8]);
				myBuildData.BuildTime = fields[4].Replace('$', ':');
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.BuildingDataList)
						{
							dbRoleInfo.BuildingDataList = new List<BuildingData>();
						}
						this.UpdateBuildData(dbRoleInfo.BuildingDataList, myBuildData);
					}
					DBWriter.UpdateBuildingData(dbMgr, roleID, myBuildData);
					string strcmd = string.Format("{0}", 0);
					client.sendCmd(nID, strcmd);
				}
			}
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00061E00 File Offset: 0x00060000
		public bool UpdateBuildData(List<BuildingData> BuildingDataList, BuildingData myBuildData)
		{
			bool bRet = false;
			for (int i = 0; i < BuildingDataList.Count; i++)
			{
				if (BuildingDataList[i].BuildId == myBuildData.BuildId)
				{
					bRet = true;
					BuildingDataList[i] = myBuildData;
					break;
				}
			}
			if (!bRet)
			{
				BuildingDataList.Add(myBuildData);
			}
			return true;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00061E64 File Offset: 0x00060064
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13300:
				this.processCmdUpdateBuildData(client, nID, cmdParams, count);
				break;
			case 13301:
				this.processCmdUpdateBuildLog(client, nID, cmdParams, count);
				break;
			}
		}

		// Token: 0x04000C4C RID: 3148
		private static UpdateBuildingDataCmdProcessor instance = new UpdateBuildingDataCmdProcessor();
	}
}
