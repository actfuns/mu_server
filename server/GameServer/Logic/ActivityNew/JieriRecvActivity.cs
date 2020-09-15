using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001C3 RID: 451
	public class JieriRecvActivity : JieriGiveRecv_Base
	{
		// Token: 0x0600059A RID: 1434 RVA: 0x0004EFE4 File Offset: 0x0004D1E4
		public override string GetConfigFile()
		{
			return "Config/JieRiGifts/JieRiShouQu.xml";
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0004EFFC File Offset: 0x0004D1FC
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

		// Token: 0x0600059C RID: 1436 RVA: 0x0004F09C File Offset: 0x0004D29C
		public override void FlushIcon(GameClient client)
		{
			if (client != null && client._IconStateMgr.CheckJieriRecv(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0004F0F0 File Offset: 0x0004D2F0
		public override bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			return info != null && info.TotalRecv >= condValue;
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0004F120 File Offset: 0x0004D320
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
