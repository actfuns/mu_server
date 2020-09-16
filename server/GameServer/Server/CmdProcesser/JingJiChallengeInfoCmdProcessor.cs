using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiChallengeInfoCmdProcessor : ICmdProcessor
	{
		
		private JingJiChallengeInfoCmdProcessor()
		{
		}

		
		public static JingJiChallengeInfoCmdProcessor getInstance()
		{
			return JingJiChallengeInfoCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			bool result;
			if (client.ClientData.CurrentLifeV <= 0 || client.ClientData.CurrentAction == 12)
			{
				result = true;
			}
			else
			{
				int pageIndex = Convert.ToInt32(cmdParams[1]);
				int roleId = client.ClientData.RoleID;
				List<JingJiChallengeInfoData> dataList = Global.sendToDB<List<JingJiChallengeInfoData>, byte[]>(10146, DataHelper.ObjectToBytes<int[]>(new int[]
				{
					roleId,
					pageIndex
				}), client.ServerId);
				client.sendCmd<List<JingJiChallengeInfoData>>(582, dataList, false);
				result = true;
			}
			return result;
		}

		
		private static JingJiChallengeInfoCmdProcessor instance = new JingJiChallengeInfoCmdProcessor();
	}
}
