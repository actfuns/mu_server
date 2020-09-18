using System;
using System.Text;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class ZhanMengShiJianCmdProcessor : ICmdProcessor
	{
		
		private ZhanMengShiJianCmdProcessor()
		{
		}

		
		public static ZhanMengShiJianCmdProcessor getInstance()
		{
			return ZhanMengShiJianCmdProcessor.instance;
		}

		
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

		
		private static ZhanMengShiJianCmdProcessor instance = new ZhanMengShiJianCmdProcessor();
	}
}
