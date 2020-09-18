using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiCreateDataCmdProcessor : ICmdProcessor
	{
		
		private JingJiCreateDataCmdProcessor()
		{
		}

		
		public static JingJiCreateDataCmdProcessor getInstance()
		{
			return JingJiCreateDataCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			PlayerJingJiData data = DataHelper.BytesToObject<PlayerJingJiData>(cmdParams, 0, count);
			if (null != data)
			{
				if (!JingJiChangManager.getInstance().createRobotData(data))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("JingJiChangCreateDataCmdProcessor.processCmd， 创建竞技场数据失败, roleId={0}", data.roleId), null, true);
				}
			}
			else
			{
				LogManager.WriteLog(LogTypes.Error, "JingJiChangCreateDataCmdProcessor.processCmd， 竞技场数据解析失败", null, true);
			}
			client.sendCmd<byte>(10142, 0);
		}

		
		private static JingJiCreateDataCmdProcessor instance = new JingJiCreateDataCmdProcessor();
	}
}
