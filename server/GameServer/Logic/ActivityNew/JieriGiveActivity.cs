using System;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieriGiveActivity : JieriGiveRecv_Base
	{
		
		public override string GetConfigFile()
		{
			return "Config/JieRiGifts/JieRiZengSong.xml";
		}

		
		public override string QueryActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = "0:0:0";
			}
			else
			{
				RoleGiveRecvInfo info = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				lock (info)
				{
					result = string.Format("{0}:{1}:{2}", info.TotalGive, info.TotalRecv, info.AwardFlag);
				}
			}
			return result;
		}

		
		public override void FlushIcon(GameClient client)
		{
			if (client != null && client._IconStateMgr.CheckJieriGive(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		
		public override bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			return info != null && info.TotalGive >= condValue;
		}

		
		public string ProcRoleGiveToOther(GameClient client, string receiverRolename, int goodsID, int goodsCnt)
		{
			int receiverRoleid = -1;
			JieriGiveErrorCode ec = JieriGiveErrorCode.Success;
			if (!this.InActivityTime())
			{
				ec = JieriGiveErrorCode.ActivityNotOpen;
			}
			else if (string.IsNullOrEmpty(receiverRolename) || receiverRolename == client.ClientData.RoleName)
			{
				ec = JieriGiveErrorCode.ReceiverCannotSelf;
			}
			else if (!base.IsGiveGoodsID(goodsID))
			{
				ec = JieriGiveErrorCode.GoodsIDError;
			}
			else if (goodsCnt <= 0 || Global.GetTotalGoodsCountByID(client, goodsID) < goodsCnt)
			{
				ec = JieriGiveErrorCode.GoodsNotEnough;
			}
			else
			{
				string dbReq = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					receiverRolename,
					goodsID,
					goodsCnt
				});
				string[] dbRsp = Global.ExecuteDBCmd(13200, dbReq, client.ServerId);
				if (dbRsp == null || dbRsp.Length != 1)
				{
					ec = JieriGiveErrorCode.DBFailed;
				}
				else
				{
					receiverRoleid = Convert.ToInt32(dbRsp[0]);
					if (receiverRoleid == -1)
					{
						ec = JieriGiveErrorCode.ReceiverNotExist;
					}
					else if (receiverRoleid <= 0)
					{
						ec = JieriGiveErrorCode.DBFailed;
					}
					else
					{
						bool bUsedBinding_just_placeholder = false;
						bool bUsedTimeLimited_just_placeholder = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsCnt, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
						{
							ec = JieriGiveErrorCode.GoodsNotEnough;
						}
						else
						{
							GameManager.logDBCmdMgr.AddMessageLog(0, Global.GetGoodsNameByID(goodsID), "节日赠送", client.ClientData.RoleName, receiverRolename, "日志", goodsCnt, client.ClientData.ZoneID, client.strUserID, receiverRoleid, client.ServerId, "");
							ec = JieriGiveErrorCode.Success;
						}
					}
				}
			}
			RoleGiveRecvInfo info = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
			if (ec == JieriGiveErrorCode.Success)
			{
				lock (info)
				{
					info.TotalGive += goodsCnt;
				}
				if (client._IconStateMgr.CheckJieriGive(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
				JieRiGiveKingActivity gkActivity = HuodongCachingMgr.GetJieriGiveKingActivity();
				if (gkActivity != null)
				{
					gkActivity.OnGive(client, goodsID, goodsCnt);
				}
				JieriRecvActivity recvAct = HuodongCachingMgr.GetJieriRecvActivity();
				if (recvAct != null)
				{
					recvAct.OnRecv(receiverRoleid, goodsCnt);
				}
				JieRiRecvKingActivity rkActivity = HuodongCachingMgr.GetJieriRecvKingActivity();
				if (rkActivity != null)
				{
					rkActivity.OnRecv(receiverRoleid, goodsID, goodsCnt, client.ServerId);
				}
			}
			string result;
			lock (info)
			{
				result = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)ec,
					info.TotalGive,
					info.TotalRecv,
					info.AwardFlag
				});
			}
			return result;
		}
	}
}
