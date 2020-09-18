using System;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieriRecvActivity : JieriGiveRecv_Base
	{
		
		public override string GetConfigFile()
		{
			return "Config/JieRiGifts/JieRiShouQu.xml";
		}

		
		public override string QueryActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = "0:0";
			}
			else
			{
				RoleGiveRecvInfo info = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				lock (info)
				{
					result = string.Format("{0}:{1}", info.TotalRecv, info.AwardFlag);
				}
			}
			return result;
		}

		
		public override void FlushIcon(GameClient client)
		{
			if (client != null && client._IconStateMgr.CheckJieriRecv(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		
		public override bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			return info != null && info.TotalRecv >= condValue;
		}

		
		public void OnRecv(int roleid, int goodsCnt)
		{
			if (this.InActivityTime())
			{
				bool bLoadFromDb;
				RoleGiveRecvInfo info = base.GetRoleGiveRecvInfo(roleid, out bLoadFromDb);
				if (info != null)
				{
					if (!bLoadFromDb)
					{
						lock (info)
						{
							info.TotalRecv += goodsCnt;
						}
					}
					GameClient client = GameManager.ClientMgr.FindClient(roleid);
					if (client != null)
					{
						if (client._IconStateMgr.CheckJieriRecv(client))
						{
							client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
							client._IconStateMgr.SendIconStateToClient(client);
						}
					}
				}
			}
		}
	}
}
