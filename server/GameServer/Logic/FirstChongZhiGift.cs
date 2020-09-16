using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class FirstChongZhiGift : Activity
	{
		
		public override bool GiveAward(GameClient client)
		{
			bool result;
			if (null == this.AwardDict)
			{
				result = false;
			}
			else
			{
				client.ClientData.ClearAwardRecord((RoleAwardMsg)this.ActivityType);
				base.GiveAward(client, this.AwardDict);
				if (null == this.AwardDict2)
				{
					result = false;
				}
				else
				{
					this.GiveAwardByOccupation(client, this.AwardDict2, client.ClientData.Occupation);
					GameManager.ClientMgr.NotifyGetAwardMsg(client, (RoleAwardMsg)this.ActivityType, "");
					result = true;
				}
			}
			return result;
		}

		
		protected bool GiveAwardByOccupation(GameClient client, AwardItem myAwardItem, int occupation)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null && myAwardItem.GoodsDataList.Count > 0)
				{
					int count = myAwardItem.GoodsDataList.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData data = myAwardItem.GoodsDataList[i];
						if (Global.IsCanGiveRewardByOccupation(client, data.GoodsID))
						{
							client.ClientData.AddAwardRecord((RoleAwardMsg)this.ActivityType, myAwardItem.GoodsDataList[i], false);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data.GoodsID, data.GCount, data.Quality, "", data.Forge_level, data.Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", data.AddPropIndex, data.BornIndex, data.Lucky, data.Strong, data.ExcellenceInfo, data.AppendPropLev, data.ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType), ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType)), null, client.ServerId);
					client.ClientData.AddAwardRecord((RoleAwardMsg)this.ActivityType, MoneyTypes.YuanBao, myAwardItem.AwardYuanBao);
				}
				result = true;
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int nOcc)
		{
			bool result;
			if (this.AwardDict.GoodsDataList.Count <= 0 && this.AwardDict2.GoodsDataList.Count <= 0)
			{
				result = true;
			}
			else if (Global.CanAddGoodsDataList(client, this.AwardDict.GoodsDataList))
			{
				int nOccu = Global.CalcOriginalOccupationID(client);
				List<GoodsData> lData = new List<GoodsData>();
				foreach (GoodsData item in this.AwardDict.GoodsDataList)
				{
					lData.Add(item);
				}
				int count = this.AwardDict2.GoodsDataList.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData data = this.AwardDict2.GoodsDataList[i];
					if (Global.IsRoleOccupationMatchGoods(nOccu, data.GoodsID))
					{
						lData.Add(this.AwardDict2.GoodsDataList[i]);
					}
				}
				result = Global.CanAddGoodsDataList(client, lData);
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public AwardItem AwardDict = new AwardItem();

		
		public AwardItem AwardDict2 = new AwardItem();
	}
}
