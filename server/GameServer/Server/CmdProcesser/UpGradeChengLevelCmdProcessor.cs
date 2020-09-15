using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008AA RID: 2218
	public class UpGradeChengLevelCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D99 RID: 15769 RVA: 0x00349C5C File Offset: 0x00347E5C
		public UpGradeChengLevelCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x00349C7C File Offset: 0x00347E7C
		public static UpGradeChengLevelCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new UpGradeChengLevelCmdProcessor(cmdID);
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x00349C94 File Offset: 0x00347E94
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

		// Token: 0x040047BB RID: 18363
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU;
	}
}
