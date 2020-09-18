using System;
using GameDBServer.Logic.WanMoTa;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class GetWanMoTaoDetailCmdProcessor : ICmdProcessor
	{
		
		private GetWanMoTaoDetailCmdProcessor()
		{
		}

		
		public static GetWanMoTaoDetailCmdProcessor getInstance()
		{
			return GetWanMoTaoDetailCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			WanMotaInfo data = WanMoTaManager.getInstance().getWanMoTaData(roleId);
			client.sendCmd<WanMotaInfo>(10159, data);
		}

		
		private static GetWanMoTaoDetailCmdProcessor instance = new GetWanMoTaoDetailCmdProcessor();
	}
}
