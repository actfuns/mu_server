using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006F9 RID: 1785
	public class KingActivity : Activity
	{
		// Token: 0x06002AF2 RID: 10994 RVA: 0x002645B0 File Offset: 0x002627B0
		public override string GetAwardMinConditionValues()
		{
			StringBuilder strBuilder = new StringBuilder();
			int maxPaiHang = this.AwardDict.Count;
			for (int paiHang = 1; paiHang <= maxPaiHang; paiHang++)
			{
				if (this.AwardDict.ContainsKey(paiHang))
				{
					if (strBuilder.Length > 0)
					{
						strBuilder.Append("_");
					}
					strBuilder.Append(this.AwardDict[paiHang].MinAwardCondionValue);
				}
			}
			return strBuilder.ToString();
		}

		// Token: 0x06002AF3 RID: 10995 RVA: 0x00264644 File Offset: 0x00262844
		public override List<int> GetAwardMinConditionlist()
		{
			List<int> cons = new List<int>();
			int maxPaiHang = this.AwardDict.Count;
			for (int paiHang = 1; paiHang <= maxPaiHang; paiHang++)
			{
				if (this.AwardDict.ContainsKey(paiHang))
				{
					cons.Add(this.AwardDict[paiHang].MinAwardCondionValue);
				}
			}
			return cons;
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x002646B0 File Offset: 0x002628B0
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardDict[_params];
			}
			bool result;
			if (null == myAwardItem)
			{
				result = false;
			}
			else
			{
				this.GiveAward(client, _params, client.ClientData.Occupation);
				result = true;
			}
			return result;
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x0026470C File Offset: 0x0026290C
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
				client.ClientData.ClearAwardRecord((RoleAwardMsg)this.ActivityType);
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
					GameManager.ClientMgr.NotifyGetAwardMsg(client, (RoleAwardMsg)this.ActivityType, "");
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002AF6 RID: 10998 RVA: 0x002647C0 File Offset: 0x002629C0
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
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data.GoodsID, data.GCount, data.Quality, "", data.Forge_level, data.Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", data.AddPropIndex, data.BornIndex, data.Lucky, data.Strong, data.ExcellenceInfo, data.AppendPropLev, data.ChangeLifeLevForEquip, null, null, 0, true);
							break;
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
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002AF7 RID: 10999 RVA: 0x002649E0 File Offset: 0x00262BE0
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

		// Token: 0x06002AF8 RID: 11000 RVA: 0x00264B28 File Offset: 0x00262D28
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			int needSpace = 0;
			int maxKey = -1;
			foreach (int key in this.AwardDict.Keys)
			{
				if (needSpace < this.AwardDict[key].GoodsDataList.Count)
				{
					needSpace = this.AwardDict[key].GoodsDataList.Count;
					maxKey = key;
				}
			}
			bool result;
			if (-1 == maxKey)
			{
				result = true;
			}
			else
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				if (!Global.CanAddGoodsDataList(client, this.AwardDict[maxKey].GoodsDataList))
				{
					result = false;
				}
				else if (!this.AwardDict2.ContainsKey(maxKey) || this.AwardDict2[maxKey].GoodsDataList == null || this.AwardDict2[maxKey].GoodsDataList.Count == 0)
				{
					result = true;
				}
				else if (Global.CanAddGoodsDataList(client, this.AwardDict2[maxKey].GoodsDataList))
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> lData = new List<GoodsData>();
					foreach (GoodsData item in this.AwardDict[maxKey].GoodsDataList)
					{
						lData.Add(item);
					}
					lData.Add(this.AwardDict2[maxKey].GoodsDataList[0]);
					result = Global.CanAddGoodsDataList(client, lData);
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x04003A0D RID: 14861
		public Dictionary<int, int> RoleLimit = new Dictionary<int, int>();

		// Token: 0x04003A0E RID: 14862
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();

		// Token: 0x04003A0F RID: 14863
		public Dictionary<int, AwardItem> AwardDict2 = new Dictionary<int, AwardItem>();
	}
}
