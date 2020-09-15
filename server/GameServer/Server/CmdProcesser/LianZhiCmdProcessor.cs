using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A7 RID: 2215
	public class LianZhiCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D7E RID: 15742 RVA: 0x0034568B File Offset: 0x0034388B
		public LianZhiCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x003456A8 File Offset: 0x003438A8
		public static LianZhiCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new LianZhiCmdProcessor(cmdID);
		}

		// Token: 0x06003D80 RID: 15744 RVA: 0x003456C0 File Offset: 0x003438C0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = (int)this.CmdID;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ZhuanHuan, true))
			{
				result = true;
			}
			else if (this.CmdID == TCPGameServerCmds.CMD_SPR_EXEC_LIANZHI)
			{
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				int count = Global.SafeConvertToInt32(cmdParams[2]);
				result = LianZhiManager.GetInstance().ExecLianZhi(client, type, count);
			}
			else
			{
				result = (this.CmdID == TCPGameServerCmds.CMD_SPR_QUERY_LIANZHICOUNT && LianZhiManager.GetInstance().QueryLianZhiCount(client));
			}
			return result;
		}

		// Token: 0x040047B7 RID: 18359
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_EXEC_LIANZHI;
	}
}
