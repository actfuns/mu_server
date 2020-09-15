using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008AB RID: 2219
	public class WashPropsCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D9C RID: 15772 RVA: 0x00349D0A File Offset: 0x00347F0A
		public WashPropsCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x00349D28 File Offset: 0x00347F28
		public static WashPropsCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new WashPropsCmdProcessor(cmdID);
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x00349D40 File Offset: 0x00347F40
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = (int)this.CmdID;
			bool result;
			if (this.CmdID == TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS)
			{
				int dbid = Global.SafeConvertToInt32(cmdParams[1]);
				int washIndex = Global.SafeConvertToInt32(cmdParams[2]);
				bool bUseBinding = Global.SafeConvertToInt32(cmdParams[3]) > 0;
				int moneyType = Global.SafeConvertToInt32(cmdParams[4]);
				result = WashPropsManager.WashProps(client, dbid, washIndex, bUseBinding, moneyType);
			}
			else if (this.CmdID == TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT)
			{
				int leftGoodsDBID = Global.SafeConvertToInt32(cmdParams[1]);
				int rightGoodsDBID = Global.SafeConvertToInt32(cmdParams[2]);
				int moneyType = Global.SafeConvertToInt32(cmdParams[3]);
				result = WashPropsManager.WashPropsInherit(client, leftGoodsDBID, rightGoodsDBID, moneyType);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x040047BC RID: 18364
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS;
	}
}
