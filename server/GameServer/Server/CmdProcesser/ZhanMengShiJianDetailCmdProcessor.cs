using System;
using System.Collections.Generic;
using GameServer.Logic;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	
	public class ZhanMengShiJianDetailCmdProcessor : ICmdProcessor
	{
		
		private ZhanMengShiJianDetailCmdProcessor()
		{
		}

		
		public static ZhanMengShiJianDetailCmdProcessor getInstance()
		{
			return ZhanMengShiJianDetailCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int bhId = client.ClientData.Faction;
			int pageIndex = Convert.ToInt32(cmdParams[1]);
			byte[] cmd = DataHelper.ObjectToBytes<int[]>(new int[]
			{
				bhId,
				pageIndex
			});
			List<ZhanMengShiJianData> dataList = Global.sendToDB<List<ZhanMengShiJianData>, byte[]>(10139, cmd, client.ServerId);
			client.sendCmd<List<ZhanMengShiJianData>>(566, dataList, false);
			return true;
		}

		
		private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();
	}
}
