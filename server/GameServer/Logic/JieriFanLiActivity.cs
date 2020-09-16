using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class JieriFanLiActivity : Activity
	{
		
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
				myAwardItem = null;
				if (this.AwardDict2.ContainsKey(_params1))
				{
					myAwardItem = this.AwardDict2[_params1];
				}
				if (null != myAwardItem)
				{
					this.GiveAwardByOccupation(client, myAwardItem, _params2);
				}
				if (this.AwardDict3.ContainsKey(_params1))
				{
					myAwardItem = this.AwardDict3[_params1].ToAwardItem();
					base.GiveEffectiveTimeAward(client, myAwardItem);
				}
				result = true;
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
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data.GoodsID, data.GCount, data.Quality, "", data.Forge_level, data.Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", data.AddPropIndex, data.BornIndex, data.Lucky, data.Strong, data.ExcellenceInfo, data.AppendPropLev, data.ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType), ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(670, new object[0]) + myAwardItem.AwardYuanBao, new object[0]), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType)), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			int needSpace = 0;
			int maxKey = -1;
			int nOccu = Global.CalcOriginalOccupationID(client);
			foreach (int key in this.AwardDict.Keys)
			{
				int tmpNeedSpace = this.AwardDict[key].GoodsDataList.Count;
				tmpNeedSpace += ((this.AwardDict2[key].GoodsDataList.Count > 0) ? 1 : 0);
				tmpNeedSpace += this.AwardDict3[key].GoodsCnt();
				if (needSpace < tmpNeedSpace)
				{
					needSpace = tmpNeedSpace;
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
				List<GoodsData> lData = new List<GoodsData>();
				foreach (GoodsData item in this.AwardDict[maxKey].GoodsDataList)
				{
					lData.Add(item);
				}
				int count = this.AwardDict2[maxKey].GoodsDataList.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData data = this.AwardDict2[maxKey].GoodsDataList[i];
					if (Global.IsRoleOccupationMatchGoods(nOccu, data.GoodsID))
					{
						lData.Add(data);
					}
				}
				AwardItem tmpAwardItem = this.AwardDict3[maxKey].ToAwardItem();
				foreach (GoodsData item in tmpAwardItem.GoodsDataList)
				{
					lData.Add(item);
				}
				result = Global.CanAddGoodsDataList(client, lData);
			}
			return result;
		}

		
		public override bool CheckCondition(GameClient client, int extTag)
		{
			AwardItem myAwardItem = null;
			if (this.AwardDict.ContainsKey(extTag))
			{
				myAwardItem = this.AwardDict[extTag];
			}
			bool result;
			if (null == myAwardItem)
			{
				result = false;
			}
			else
			{
				ActivityTypes activityType = (ActivityTypes)this.ActivityType;
				switch (activityType)
				{
				case ActivityTypes.JieriWing:
					if (client.ClientData.MyWingData == null || client.ClientData.MyWingData.Using == 0)
					{
						return false;
					}
					if (client.ClientData.MyWingData.WingID < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					if (client.ClientData.MyWingData.WingID == myAwardItem.MinAwardCondionValue && client.ClientData.MyWingData.ForgeLevel < myAwardItem.MinAwardCondionValue2)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAddon:
					if (client.UsingEquipMgr.GetUsingEquipAllAppendPropLeva() < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriStrengthen:
					if (client.UsingEquipMgr.GetUsingEquipAllForge() < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAchievement:
					if (ChengJiuManager.GetChengJiuLevel(client) < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriMilitaryRank:
					if (GameManager.ClientMgr.GetShengWangLevelValue(client) < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriVIPFanli:
					if (client.ClientData.VipLevel < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAmulet:
				{
					GoodsData hufugoods = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 22);
					if (null == hufugoods)
					{
						return false;
					}
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(hufugoods.GoodsID, out systemGoods))
					{
						return false;
					}
					int nSuitID = systemGoods.GetIntValue("SuitID", -1);
					if (nSuitID < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				}
				case ActivityTypes.JieriArchangel:
					if (client.UsingEquipMgr.GetUsingEquipArchangelWeaponSuit() < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriLianXuCharge:
					break;
				case ActivityTypes.JieriMarriage:
					if (client.ClientData.MyMarriageData == null || -1 == client.ClientData.MyMarriageData.byMarrytype || (int)client.ClientData.MyMarriageData.byGoodwilllevel < myAwardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				default:
					switch (activityType)
					{
					case ActivityTypes.JieRiHuiJi:
					{
						EmblemStarInfo starInfo = HuiJiManager.getInstance().GetHuiJiStartInfo(client);
						if (null == starInfo)
						{
							return false;
						}
						if (starInfo.EmblemLevel < myAwardItem.MinAwardCondionValue)
						{
							return false;
						}
						if (starInfo.EmblemLevel == myAwardItem.MinAwardCondionValue && starInfo.EmblemStar < myAwardItem.MinAwardCondionValue2)
						{
							return false;
						}
						goto IL_3A3;
					}
					case ActivityTypes.JieRiFuWen:
						if (ShenShiManager.getInstance().GetCurrentTabTotalLevel(client) < myAwardItem.MinAwardCondionValue)
						{
							return false;
						}
						goto IL_3A3;
					}
					break;
				}
				return false;
				IL_3A3:
				result = true;
			}
			return result;
		}

		
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardItem> AwardDict2 = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardEffectTimeItem> AwardDict3 = new Dictionary<int, AwardEffectTimeItem>();
	}
}
