using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	
	public class WashPropsCmdProcessor : ICmdProcessor
	{
		
		public WashPropsCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		
		public static WashPropsCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new WashPropsCmdProcessor(cmdID);
		}

		
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

		
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS;
	}
}
