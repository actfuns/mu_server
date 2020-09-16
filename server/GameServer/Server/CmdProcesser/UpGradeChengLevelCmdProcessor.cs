using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	
	public class UpGradeChengLevelCmdProcessor : ICmdProcessor
	{
		
		public UpGradeChengLevelCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		
		public static UpGradeChengLevelCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new UpGradeChengLevelCmdProcessor(cmdID);
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = (int)this.CmdID;
			bool result;
			if (this.CmdID == TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU)
			{
				int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
				int nChengJiuLevel = Global.SafeConvertToInt32(cmdParams[1]);
				int nRet = ChengJiuManager.TryToActiveNewChengJiuBuffer(client, true, nChengJiuLevel);
				string strCmd = string.Format("{0}:{1}", nRoleID, nRet);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU;
	}
}
