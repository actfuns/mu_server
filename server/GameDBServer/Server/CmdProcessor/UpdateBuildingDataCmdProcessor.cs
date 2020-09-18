using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class UpdateBuildingDataCmdProcessor : ICmdProcessor
	{
		
		private UpdateBuildingDataCmdProcessor()
		{
		}

		
		public static UpdateBuildingDataCmdProcessor getInstance()
		{
			return UpdateBuildingDataCmdProcessor.instance;
		}

		
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

		
		private static UpdateBuildingDataCmdProcessor instance = new UpdateBuildingDataCmdProcessor();
	}
}
