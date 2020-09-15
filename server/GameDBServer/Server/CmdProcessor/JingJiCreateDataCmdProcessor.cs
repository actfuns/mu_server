using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001EF RID: 495
	public class JingJiCreateDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A53 RID: 2643 RVA: 0x00062035 File Offset: 0x00060235
		private JingJiCreateDataCmdProcessor()
		{
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00062040 File Offset: 0x00060240
		public static JingJiCreateDataCmdProcessor getInstance()
		{
			return JingJiCreateDataCmdProcessor.instance;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00062058 File Offset: 0x00060258
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

		// Token: 0x04000C51 RID: 3153
		private static JingJiCreateDataCmdProcessor instance = new JingJiCreateDataCmdProcessor();
	}
}
