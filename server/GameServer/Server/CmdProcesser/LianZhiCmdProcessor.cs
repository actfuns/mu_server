using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	
	public class LianZhiCmdProcessor : ICmdProcessor
	{
		
		public LianZhiCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		
		public static LianZhiCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new LianZhiCmdProcessor(cmdID);
		}

		
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

		
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_EXEC_LIANZHI;
	}
}
