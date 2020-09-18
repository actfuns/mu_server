using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class Activity
	{
		
		public bool IsHeFuActivity(int type)
		{
			return type >= 20 && type <= 25;
		}

		
		public bool IsThemeActivity(int type)
		{
			return type >= 150 && type <= 157;
		}

		
		public bool IsJieRiActivity(int type)
		{
			return type == 9 || type == 10 || type == 12 || type == 13 || type == 14 || type == 15 || type == 16 || type == 17 || type == 40 || type == 41 || type == 42 || type == 53 || type == 54 || type == 55 || type == 56 || type == 57 || type == 58 || type == 59 || type == 60 || type == 62 || type == 50 || type == 51 || type == 52 || type == 61 || type == 64 || type == 66 || type == 67 || type == 68 || type == 69 || type == 70 || type == 75 || type == 76 || type == 77;
		}

		
		public virtual bool InActivityTime()
		{
			if (this.IsHeFuActivity(this.ActivityType))
			{
				HeFuActivityConfig config = HuodongCachingMgr.GetHeFuActivityConfing();
				if (null == config)
				{
					return false;
				}
				if (!config.InList(this.ActivityType))
				{
					return false;
				}
			}
			if (this.IsJieRiActivity(this.ActivityType))
			{
				JieriActivityConfig config2 = HuodongCachingMgr.GetJieriActivityConfig();
				if (null == config2)
				{
					return false;
				}
				if (!config2.InList(this.ActivityType))
				{
					return false;
				}
			}
			if (this.IsThemeActivity(this.ActivityType))
			{
				ThemeActivityConfig config3 = HuodongCachingMgr.GetThemeActivityConfig();
				if (config3 == null || config3.ActivityOpenVavle <= 0)
				{
					return false;
				}
				if (!config3.InList(this.ActivityType))
				{
					return false;
				}
				int endData = config3.GetEndData(this.ActivityType);
				if (endData > 0 && TimeUtil.NowDateTime() > Global.GetKaiFuTime().AddDays((double)endData))
				{
					return false;
				}
			}
			bool result;
			if (string.IsNullOrEmpty(this.FromDate) || string.IsNullOrEmpty(this.ToDate))
			{
				result = false;
			}
			else
			{
				DateTime startTime = DateTime.Parse(this.FromDate);
				DateTime endTime = DateTime.Parse(this.ToDate);
				result = (TimeUtil.NowDateTime() >= startTime && TimeUtil.NowDateTime() <= endTime);
			}
			return result;
		}

		
		public virtual bool InAwardTime()
		{
			if (this.IsHeFuActivity(this.ActivityType))
			{
				HeFuActivityConfig config = HuodongCachingMgr.GetHeFuActivityConfing();
				if (null == config)
				{
					return false;
				}
				if (!config.InList(this.ActivityType))
				{
					return false;
				}
			}
			if (this.IsJieRiActivity(this.ActivityType))
			{
				JieriActivityConfig config2 = HuodongCachingMgr.GetJieriActivityConfig();
				if (null == config2)
				{
					return false;
				}
				if (!config2.InList(this.ActivityType))
				{
					return false;
				}
			}
			if (this.IsThemeActivity(this.ActivityType))
			{
				ThemeActivityConfig config3 = HuodongCachingMgr.GetThemeActivityConfig();
				if (config3 == null || config3.ActivityOpenVavle <= 0)
				{
					return false;
				}
				if (!config3.InList(this.ActivityType))
				{
					return false;
				}
				int endData = config3.GetEndData(this.ActivityType);
				if (endData > 0 && TimeUtil.NowDateTime() > Global.GetKaiFuTime().AddDays((double)endData))
				{
					return false;
				}
			}
			DateTime startTime = DateTime.Parse(this.AwardStartDate);
			DateTime endTime = DateTime.Parse(this.AwardEndDate);
			return TimeUtil.NowDateTime() >= startTime && TimeUtil.NowDateTime() <= endTime;
		}

		
		public bool CanGiveAward()
		{
			try
			{
				if (!this.InAwardTime())
				{
					return false;
				}
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		
		public virtual string GetAwardMinConditionValues()
		{
			return null;
		}

		
		public virtual List<int> GetAwardMinConditionlist()
		{
			return null;
		}

		
		public virtual bool CanGiveAward(GameClient client, int index, int totalMoney)
		{
			return true;
		}

		
		public virtual int GetParamsValidateCode()
		{
			int result;
			if (0 != this.CodeForParamsValidate)
			{
				result = this.CodeForParamsValidate;
			}
			else
			{
				int validateCode = 1;
				try
				{
					if (this.FromDate.CompareTo("-1") != 0 || 0 != this.ToDate.CompareTo("-1"))
					{
						DateTime myFromDate = DateTime.Parse(this.FromDate);
						DateTime myToDate = DateTime.Parse(this.ToDate);
						if (myFromDate >= myToDate)
						{
							validateCode = -50001;
						}
					}
					if (validateCode > 0)
					{
						if (this.AwardStartDate.CompareTo("-1") != 0 || 0 != this.AwardEndDate.CompareTo("-1"))
						{
							DateTime myFromDate = DateTime.Parse(this.AwardStartDate);
							DateTime myToDate = DateTime.Parse(this.AwardEndDate);
							if (myFromDate >= myToDate)
							{
								validateCode = -50002;
							}
						}
					}
				}
				catch (Exception)
				{
					validateCode = -50000;
				}
				if (validateCode < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("活动【{0}】的参数验证失败，错误码{1}", Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), validateCode), null, true);
				}
				this.CodeForParamsValidate = validateCode;
				result = validateCode;
			}
			return result;
		}

		
		public virtual bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return true;
		}

		
		public virtual bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params1)
		{
			return true;
		}

		
		public virtual bool GiveAward(GameClient client, int _params)
		{
			return true;
		}

		
		public virtual bool GiveAward(GameClient client, int _params1, int _params2)
		{
			return true;
		}

		
		public virtual bool GiveAward(GameClient client)
		{
			return true;
		}

		
		protected bool GiveAward(GameClient client, AwardItem myAwardItem)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int nGoodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							client.ClientData.AddAwardRecord((RoleAwardMsg)this.ActivityType, myAwardItem.GoodsDataList[i], false);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
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

		
		protected bool GiveEffectiveTimeAward(GameClient client, AwardItem myAwardItem)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int nGoodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							client.ClientData.AddAwardRecord((RoleAwardMsg)this.ActivityType, myAwardItem.GoodsDataList[i], false);
							Global.AddEffectiveTimeGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", false, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), myAwardItem.GoodsDataList[i].Starttime, myAwardItem.GoodsDataList[i].Endtime, myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null);
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

		
		public virtual List<int> GetAwardIDList()
		{
			return null;
		}

		
		public virtual AwardItem GetAward(GameClient client)
		{
			return null;
		}

		
		public virtual AwardItem GetAward(int _params)
		{
			return null;
		}

		
		public virtual AwardItem GetAward(GameClient client, int _params = 0)
		{
			return null;
		}

		
		public virtual AwardItem GetAward(GameClient client, int _params1 = 0, int _params2 = 0)
		{
			return null;
		}

		
		public virtual AwardItem GetAward(GameClient client, int _params1 = 0, int _params2 = 0, int _params3 = 0)
		{
			return null;
		}

		
		public static string GetActivityChineseName(ActivityTypes type)
		{
			string activityName = type.ToString();
			switch (type)
			{
			case ActivityTypes.InputFirst:
				activityName = "首充大礼";
				break;
			case ActivityTypes.InputFanLi:
				activityName = "充值返利";
				break;
			case ActivityTypes.InputJiaSong:
				activityName = "充值加送";
				break;
			case ActivityTypes.InputKing:
				activityName = "充值王";
				break;
			case ActivityTypes.LevelKing:
				activityName = "冲级王";
				break;
			case ActivityTypes.EquipKing:
				activityName = "装备王";
				break;
			case ActivityTypes.HorseKing:
				activityName = "坐骑王";
				break;
			case ActivityTypes.JingMaiKing:
				activityName = "经脉王";
				break;
			case ActivityTypes.JieriDaLiBao:
				activityName = "节日大礼包";
				break;
			case ActivityTypes.JieriDengLuHaoLi:
				activityName = "节日登录豪礼";
				break;
			case ActivityTypes.JieriVIP:
				activityName = "节日VIP大礼";
				break;
			case ActivityTypes.JieriCZSong:
				activityName = "节日充值送礼";
				break;
			case ActivityTypes.JieriLeiJiCZ:
				activityName = "节日累计充值大礼";
				break;
			case ActivityTypes.JieriZiKa:
				activityName = "节日字卡换礼盒";
				break;
			case ActivityTypes.JieriPTXiaoFeiKing:
				activityName = "节日消费王";
				break;
			case ActivityTypes.JieriPTCZKing:
				activityName = "节日充值王";
				break;
			case ActivityTypes.JieriBossAttack:
				activityName = "节日Boss攻城";
				break;
			case (ActivityTypes)18:
			case (ActivityTypes)19:
			case ActivityTypes.HeFuShopLimit:
			case ActivityTypes.HeFuAwardTime:
			case ActivityTypes.XingYunChouJiang:
			case ActivityTypes.YuDuZhuanPanChouJiang:
			case ActivityTypes.NewZoneUpLevelMadman:
			case ActivityTypes.NewZoneRechargeKing:
			case ActivityTypes.NewZoneConsumeKing:
			case ActivityTypes.NewZoneBosskillKing:
			case ActivityTypes.NewZoneFanli:
			case ActivityTypes.TotalCharge:
			case ActivityTypes.TotalConsume:
			case ActivityTypes.JieriTotalConsume:
			case ActivityTypes.JieriDuoBei:
			case ActivityTypes.JieriQiangGou:
			case ActivityTypes.HeFuLuoLan:
			case ActivityTypes.SpecActivity:
			case ActivityTypes.EverydayActivity:
			case ActivityTypes.SpecPriorityActivity:
			case (ActivityTypes)65:
			case ActivityTypes.JieriFuLi:
			case ActivityTypes.JieRiHongBao:
			case ActivityTypes.JieRiChongZhiHongBao:
			case ActivityTypes.JieRiHongBaoKing:
				break;
			case ActivityTypes.HeFuLogin:
				activityName = "合服登陆豪礼";
				break;
			case ActivityTypes.HeFuTotalLogin:
				activityName = "合服累计登陆";
				break;
			case ActivityTypes.HeFuRecharge:
				activityName = "合服充值返利";
				break;
			case ActivityTypes.HeFuPKKing:
				activityName = "合服PK王大礼";
				break;
			case ActivityTypes.HeFuBossAttack:
				activityName = "合服Boss攻城";
				break;
			case ActivityTypes.MeiRiChongZhiHaoLi:
				activityName = "每日充值豪礼";
				break;
			case ActivityTypes.ChongJiLingQuShenZhuang:
				activityName = "充级领取神装";
				break;
			case ActivityTypes.ShenZhuangJiQingHuiKui:
				activityName = "神装激情回赠";
				break;
			case ActivityTypes.XinCZFanLi:
				activityName = "新区充值返利";
				break;
			case ActivityTypes.OneDollarBuy:
				activityName = "1元直购";
				break;
			case ActivityTypes.OneDollarChongZhi:
				activityName = "1元充值";
				break;
			case ActivityTypes.InputFanLiNew:
				activityName = "3周年充值返利";
				break;
			case ActivityTypes.JieriGive:
				activityName = "节日赠送";
				break;
			case ActivityTypes.JieriGiveKing:
				activityName = "节日赠送王";
				break;
			case ActivityTypes.JieriRecvKing:
				activityName = "节日收取王";
				break;
			case ActivityTypes.JieriWing:
				activityName = "节日翅膀返利";
				break;
			case ActivityTypes.JieriAddon:
				activityName = "节日追加返利";
				break;
			case ActivityTypes.JieriStrengthen:
				activityName = "节日强化返利";
				break;
			case ActivityTypes.JieriAchievement:
				activityName = "节日成就返利";
				break;
			case ActivityTypes.JieriMilitaryRank:
				activityName = "节日军衔返利";
				break;
			case ActivityTypes.JieriVIPFanli:
				activityName = "节日VIP返利";
				break;
			case ActivityTypes.JieriAmulet:
				activityName = "节日护身符返利";
				break;
			case ActivityTypes.JieriArchangel:
				activityName = "节日大天使返利";
				break;
			case ActivityTypes.JieriLianXuCharge:
				activityName = "节日连续充值";
				break;
			case ActivityTypes.JieriMarriage:
				activityName = "节日婚姻返利";
				break;
			case ActivityTypes.JieriRecv:
				activityName = "节日收取";
				break;
			case ActivityTypes.JieriInputPointsExchg:
				activityName = "节日充值点数兑换";
				break;
			case ActivityTypes.JieriChongZhiQiangGou:
				activityName = "节日充值抢购";
				break;
			case ActivityTypes.JieriVIPYouHui:
				activityName = "节日VIP优惠";
				break;
			case ActivityTypes.DanBiChongZhi:
				activityName = "单笔充值";
				break;
			case ActivityTypes.JieRiMeiRiLeiJi:
				activityName = "节日每日累充大礼";
				break;
			case ActivityTypes.JieriSuperInputFanLi:
				activityName = "节日超级充值返利";
				break;
			case ActivityTypes.JieRiHuiJi:
				activityName = "节日徽记返利";
				break;
			case ActivityTypes.JieRiFuWen:
				activityName = "节日符文返利";
				break;
			case ActivityTypes.JieriPCKingEveryDay:
				activityName = "节日每日平台充值王";
				break;
			default:
				switch (type)
				{
				case ActivityTypes.TriennialRegressSignAward:
					activityName = "3周年回归签到";
					break;
				case ActivityTypes.TriennialRegressTotalRechargeAward:
					activityName = "3周年回归累计充值";
					break;
				case ActivityTypes.TriennialRegressDayBuy:
					activityName = "3周年回归每日直购";
					break;
				case ActivityTypes.TriennialRegressStore:
					activityName = "3周年回归专属商城";
					break;
				}
				break;
			}
			return activityName;
		}

		
		public void PredealDateTime()
		{
			if (this.FromDate.CompareTo("-1") == 0 && 0 == this.ToDate.CompareTo("-1"))
			{
				this.FromDate = "2008-08-08 08:08:08";
				this.ToDate = "2028-08-08 08:08:08";
			}
			if (this.AwardStartDate.CompareTo("-1") == 0 && 0 == this.AwardEndDate.CompareTo("-1"))
			{
				this.AwardStartDate = "2008-08-08 08:08:08";
				this.AwardEndDate = "2028-08-08 08:08:08";
			}
			if (!DateTime.TryParse(this.FromDate, out this.StartTime))
			{
				this.StartTime = DateTime.Parse("2008-08-08 08:08:08");
			}
			if (!DateTime.TryParse(this.ToDate, out this.EndTime))
			{
				this.EndTime = DateTime.Parse("2028-08-08 08:08:08");
			}
			this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate, this.ToDate).Replace(':', '$');
		}

		
		public virtual bool CheckCondition(GameClient client, int extTag)
		{
			return true;
		}

		
		public string FromDate = "";

		
		public string ToDate = "";

		
		public string AwardStartDate = "";

		
		public string AwardEndDate = "";

		
		public int ActivityType = -1;

		
		protected int CodeForParamsValidate = 0;

		
		public string ActivityKeyStr;

		
		public DateTime StartTime;

		
		public DateTime EndTime;
	}
}
