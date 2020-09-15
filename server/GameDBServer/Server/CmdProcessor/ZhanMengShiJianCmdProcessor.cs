using System;
using System.Text;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001FF RID: 511
	public class ZhanMengShiJianCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A8F RID: 2703 RVA: 0x000653A4 File Offset: 0x000635A4
		private ZhanMengShiJianCmdProcessor()
		{
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x000653B0 File Offset: 0x000635B0
		public static ZhanMengShiJianCmdProcessor getInstance()
		{
			return ZhanMengShiJianCmdProcessor.instance;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x000653C8 File Offset: 0x000635C8
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmd = new UTF8Encoding().GetString(cmdParams, 0, count);
			string[] param = cmd.Split(new char[]
			{
				':'
			});
			ZhanMengShiJianData data = new ZhanMengShiJianData();
			data.BHID = Convert.ToInt32(param[0]);
			data.RoleName = Convert.ToString(param[1]);
			data.ShiJianType = Convert.ToInt32(param[2]);
			data.SubValue1 = Convert.ToInt32(param[3]);
			data.SubValue2 = Convert.ToInt32(param[4]);
			data.SubValue3 = Convert.ToInt32(param[5]);
			data.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (data.ShiJianType == ZhanMengShiJianConstants.ChangeZhiWu)
			{
				string otherRoleName;
				string otherUserID;
				Global.GetRoleNameAndUserID(DBManager.getInstance(), data.SubValue3, out otherRoleName, out otherUserID);
				data.RoleName = otherRoleName;
			}
			ZhanMengShiJianManager.getInstance().onAddZhanMengShiJian(data);
			byte[] arrSendData = DataHelper.ObjectToBytes<string>(string.Format("{0}", 1));
			client.sendCmd<byte[]>(10138, arrSendData);
		}

		// Token: 0x04000C6E RID: 3182
		private static ZhanMengShiJianCmdProcessor instance = new ZhanMengShiJianCmdProcessor();
	}
}
