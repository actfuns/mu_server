using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000049 RID: 73
	public class OneDollarChongZhi : Activity
	{
		// Token: 0x060000DD RID: 221 RVA: 0x0000F830 File Offset: 0x0000DA30
		public bool Init()
		{
			try
			{
				Dictionary<int, int> OpenStateDict = new Dictionary<int, int>();
				string strPlatformOpen = GameManager.systemParamsList.GetParamValueByName("YiYuanChongZhiOpen");
				if (!string.IsNullOrEmpty(strPlatformOpen))
				{
					string[] Fields = strPlatformOpen.Split(new char[]
					{
						'|'
					});
					foreach (string dat in Fields)
					{
						string[] State = dat.Split(new char[]
						{
							','
						});
						if (State.Length == 2)
						{
							OpenStateDict[Global.SafeConvertToInt32(State[0])] = Global.SafeConvertToInt32(State[1]);
						}
					}
				}
				OpenStateDict.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/YiYuanChongZhi.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/YiYuanChongZhi.xml"));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("YiYuanChongZhi");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "BeginTime");
					this.ToDate = Global.GetSafeAttributeStr(args, "FinishTime");
					this.ActivityType = 46;
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.OneDollarChongZhiData.ID = (int)Global.GetSafeAttributeLong(args, "ID");
					DateTime.TryParse(this.FromDate, out this.OneDollarChongZhiData.FromDate);
					DateTime.TryParse(this.ToDate, out this.OneDollarChongZhiData.ToDate);
					this.OneDollarChongZhiData.MinZuanShi = (int)Global.GetSafeAttributeLong(args, "MinZhuanShi");
					this.OneDollarChongZhiData.UserListFile = Global.GetSafeAttributeStr(args, "UserList");
					if (!string.IsNullOrEmpty(this.OneDollarChongZhiData.UserListFile))
					{
						string filedir = string.Format("Config/{0}", this.OneDollarChongZhiData.UserListFile);
						filedir = Global.GameResPath(filedir);
						if (File.Exists(filedir))
						{
							string[] allUserIds = File.ReadAllLines(filedir);
							foreach (string userid in allUserIds)
							{
								if (!string.IsNullOrEmpty(userid))
								{
									this.OneDollarChongZhiData.UserListSet.Add(userid.ToLower());
								}
							}
						}
					}
					string GoodsOne = Global.GetSafeAttributeStr(args, "GoodsID1");
					string[] fields = GoodsOne.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("解析1元充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
					}
					else
					{
						this.OneDollarChongZhiData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "1元充值活动配置1");
					}
					string GoodsTwo = Global.GetSafeAttributeStr(args, "GoodsID2");
					if (!string.IsNullOrEmpty(GoodsTwo))
					{
						fields = GoodsTwo.Split(new char[]
						{
							'|'
						});
						this.OneDollarChongZhiData.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "1元充值活动配置2");
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/YiYuanChongZhi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000FBC0 File Offset: 0x0000DDC0
		public void OnRoleLogin(GameClient client)
		{
			if (null != this.OneDollarChongZhiData)
			{
				string userId = client.strUserID.ToLower();
				if (!this.InActivityTime() || !this.OneDollarChongZhiData.UserListSet.Contains(userId))
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						0,
						"",
						0,
						0
					});
					client.sendCmd(770, strcmd, false);
				}
				else
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						this.PlatformOpenStateVavle,
						"",
						0,
						0
					});
					client.sendCmd(770, strcmd, false);
				}
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000FCB8 File Offset: 0x0000DEB8
		public override bool CanGiveAward(GameClient client, int index, int totalMoney)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else if (0 == this.PlatformOpenStateVavle)
			{
				result = false;
			}
			else
			{
				string userId = client.strUserID.ToLower();
				result = this.OneDollarChongZhiData.UserListSet.Contains(userId);
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000FD14 File Offset: 0x0000DF14
		public override bool GiveAward(GameClient client)
		{
			AwardItem myAwardItem = new AwardItem();
			myAwardItem.GoodsDataList = this.OneDollarChongZhiData.GoodsDataListOne;
			base.GiveAward(client, myAwardItem);
			myAwardItem.GoodsDataList = this.OneDollarChongZhiData.GoodsDataListTwo;
			base.GiveAward(client, myAwardItem);
			return true;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000FD64 File Offset: 0x0000DF64
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == this.OneDollarChongZhiData)
			{
				result = false;
			}
			else
			{
				int nOccu = Global.CalcOriginalOccupationID(client);
				List<GoodsData> lData = new List<GoodsData>();
				foreach (GoodsData item in this.OneDollarChongZhiData.GoodsDataListOne)
				{
					lData.Add(item);
				}
				int count = this.OneDollarChongZhiData.GoodsDataListTwo.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData data = this.OneDollarChongZhiData.GoodsDataListTwo[i];
					if (Global.IsCanGiveRewardByOccupation(client, data.GoodsID))
					{
						lData.Add(data);
					}
				}
				result = Global.CanAddGoodsDataList(client, lData);
			}
			return result;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000FE58 File Offset: 0x0000E058
		public override List<int> GetAwardMinConditionlist()
		{
			List<int> cons = new List<int>();
			List<int> result;
			if (null == this.OneDollarChongZhiData)
			{
				result = cons;
			}
			else
			{
				cons.Add(this.OneDollarChongZhiData.MinZuanShi);
				result = cons;
			}
			return result;
		}

		// Token: 0x04000195 RID: 405
		protected const string OneDollarChongZhiData_fileName = "Config/YiYuanChongZhi.xml";

		// Token: 0x04000196 RID: 406
		protected OneDollarChongZhiConfig OneDollarChongZhiData = new OneDollarChongZhiConfig();

		// Token: 0x04000197 RID: 407
		protected int PlatformOpenStateVavle = 0;
	}
}
