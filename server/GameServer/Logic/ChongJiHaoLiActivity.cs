using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class ChongJiHaoLiActivity : KingActivity
	{
		
		public override AwardItem GetAward(GameClient client, int _params)
		{
			AwardItem result;
			if (this.AwardDict.ContainsKey(_params))
			{
				result = this.AwardDict[_params];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public override AwardItem GetAward(GameClient client, int _params1, int _params2)
		{
			if (_params2 == 1)
			{
				if (this.AwardDict.ContainsKey(1))
				{
					return this.AwardDict[1];
				}
			}
			else if (_params2 == 2)
			{
				if (this.AwardDict2.ContainsKey(_params1))
				{
					return this.AwardDict2[_params1];
				}
			}
			return null;
		}

		
		public override bool GiveAward(GameClient client, int _params1, int _params2)
		{
			AwardItem myAwardItem = null;
			if (this.AwardDict.ContainsKey(_params1))
			{
				myAwardItem = this.AwardDict[_params1];
			}
			bool result;
			if (null == myAwardItem)
			{
				result = false;
			}
			else
			{
				base.GiveAward(client, myAwardItem);
				if (this.AwardDict2.ContainsKey(_params1))
				{
					myAwardItem = this.AwardDict2[_params1];
				}
				if (null == myAwardItem)
				{
					result = false;
				}
				else
				{
					this.GiveAwardByOccupation(client, myAwardItem, _params2);
					result = true;
				}
			}
			return result;
		}

		
		protected new bool GiveAwardByOccupation(GameClient client, AwardItem myAwardItem, int occupation)
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
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data.GoodsID, data.GCount, data.Quality, "", data.Forge_level, data.Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", data.AddPropIndex, data.BornIndex, data.Lucky, data.Strong, data.ExcellenceInfo, data.AppendPropLev, data.ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType), ActivityTypes.None, "");
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType)), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int nBtnIndex)
		{
			bool result;
			if (Global.CanAddGoodsDataList(client, this.AwardDict[nBtnIndex].GoodsDataList))
			{
				int nOccu = Global.CalcOriginalOccupationID(client);
				List<GoodsData> lData = new List<GoodsData>();
				foreach (GoodsData item in this.AwardDict[nBtnIndex].GoodsDataList)
				{
					lData.Add(item);
				}
				if (this.AwardDict2.ContainsKey(nBtnIndex))
				{
					int count = this.AwardDict2[nBtnIndex].GoodsDataList.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData data = this.AwardDict2[nBtnIndex].GoodsDataList[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, data.GoodsID))
						{
							lData.Add(this.AwardDict2[nBtnIndex].GoodsDataList[i]);
						}
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
	}
}
