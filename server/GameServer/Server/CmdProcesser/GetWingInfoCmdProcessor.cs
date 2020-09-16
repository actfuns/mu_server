using System;
using GameServer.Logic;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	
	public class GetWingInfoCmdProcessor : ICmdProcessor
	{
		
		private GetWingInfoCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(678, 1, this);
		}

		
		public static GetWingInfoCmdProcessor getInstance()
		{
			return GetWingInfoCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 678;
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				WingData wingData = new WingData();
				client.sendCmd<WingData>(nID, wingData, false);
				result = true;
			}
			else
			{
				client.sendCmd<WingData>(nID, client.ClientData.MyWingData, false);
				result = true;
			}
			return result;
		}

		
		private static GetWingInfoCmdProcessor instance = new GetWingInfoCmdProcessor();
	}
}
