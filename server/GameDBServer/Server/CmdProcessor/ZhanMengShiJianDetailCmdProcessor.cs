using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
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

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] param = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int bhId = param[0];
			int pageIndex = param[1];
			client.sendCmd<List<ZhanMengShiJianData>>(10139, ZhanMengShiJianManager.getInstance().getDetailByPageIndex(bhId, pageIndex));
		}

		
		private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();
	}
}
