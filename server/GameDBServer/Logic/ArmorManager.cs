using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class ArmorManager : SingletonTemplate<ArmorManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1447, SingletonTemplate<ArmorManager>.Instance());
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1447)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		
		private void ProcessArmorLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleDataCmdT<RoleArmorData> data = DataHelper.BytesToObject<RoleDataCmdT<RoleArmorData>>(cmdParams, 0, count);
			if (data != null && data.RoleID > 0)
			{
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref data.RoleID);
				if (null != dbRoleInfo)
				{
					if (dbRoleInfo.ArmorData.Armor != data.Value.Armor || dbRoleInfo.ArmorData.Exp != data.Value.Exp)
					{
						dbRoleInfo.ArmorData.Armor = data.Value.Armor;
						dbRoleInfo.ArmorData.Exp = data.Value.Exp;
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("update t_roles set armor={1},armorexp={2} where rid={0}", data.RoleID, data.Value.Armor, data.Value.Exp);
							ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
						}
					}
				}
			}
			client.sendCmd(nID, string.Format("{0}", ret));
		}

		
		private object Mutex = new object();
	}
}
