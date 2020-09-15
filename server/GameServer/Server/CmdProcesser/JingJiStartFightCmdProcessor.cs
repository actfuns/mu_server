using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A6 RID: 2214
	public class JingJiStartFightCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D7A RID: 15738 RVA: 0x003455D5 File Offset: 0x003437D5
		private JingJiStartFightCmdProcessor()
		{
		}

		// Token: 0x06003D7B RID: 15739 RVA: 0x003455E0 File Offset: 0x003437E0
		public static JingJiStartFightCmdProcessor getInstance()
		{
			return JingJiStartFightCmdProcessor.instance;
		}

		// Token: 0x06003D7C RID: 15740 RVA: 0x003455F8 File Offset: 0x003437F8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 634;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (-1 == JingJiChangManager.getInstance().JingJiChangStartFight(client))
			{
				string strCmd = string.Format("{0}:{1}", -1, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				string strCmd = string.Format("{0}:{1}", 0, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			return result;
		}

		// Token: 0x040047B6 RID: 18358
		private static JingJiStartFightCmdProcessor instance = new JingJiStartFightCmdProcessor();
	}
}
