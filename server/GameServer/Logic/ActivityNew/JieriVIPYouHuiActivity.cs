using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000044 RID: 68
	public class JieriVIPYouHuiActivity : Activity
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x0000E710 File Offset: 0x0000C910
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/VIPYouHuiLiBao.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/VIPYouHuiLiBao.xml"));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							JieriVIPYouHuiActivityConfig myConfigItem = new JieriVIPYouHuiActivityConfig();
							myConfigItem.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myConfigItem.MinVIPLev = (int)Global.GetSafeAttributeLong(xmlItem, "VIPLevel");
							myConfigItem.Price = (int)Global.GetSafeAttributeLong(xmlItem, "Price");
							myConfigItem.SinglePurchase = (int)Global.GetSafeAttributeLong(xmlItem, "SinglePurchase");
							myConfigItem.FullPurchase = (int)Global.GetSafeAttributeLong(xmlItem, "FullPurchase");
							string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							string[] fields = goodsIDsOne.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日活动VIP优惠配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								myConfigItem.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "节日活动VIP优惠配置1");
							}
							string goodsIDsTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDsTwo))
							{
								fields = goodsIDsTwo.Split(new char[]
								{
									'|'
								});
								myConfigItem.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "节日活动VIP优惠配置2");
							}
							string goodsIDsThr = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
							myConfigItem.GoodsDataListThr.Init(goodsIDsThr, Global.GetSafeAttributeStr(xmlItem, "EffectiveTime"), "节日活动VIP优惠配置3");
							this.VIPYouHuiCofigDict[myConfigItem.ID] = myConfigItem;
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/ChongZhiDuiHuan.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000EA08 File Offset: 0x0000CC08
		public int GetSinglePurchase(GameClient client, int extTag)
		{
			string[] strlist = null;
			string strs = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(strs))
			{
				strlist = strs.Split(new char[]
				{
					','
				});
			}
			int offsetDay = Global.GetOffsetDay(DateTime.Parse(this.FromDate));
			int result;
			if (strlist == null || offsetDay != Global.SafeConvertToInt32(strlist[0]))
			{
				Global.SaveRoleParamsStringToDB(client, "35", string.Format("{0}", offsetDay), true);
				result = 0;
			}
			else
			{
				for (int index = 1; index < strlist.Length - 1; index += 2)
				{
					if (extTag == Global.SafeConvertToInt32(strlist[index]))
					{
						return Global.SafeConvertToInt32(strlist[index + 1]);
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000EAD4 File Offset: 0x0000CCD4
		public int GetFullPurchase(GameClient client, int extTag)
		{
			string[] strlist = null;
			string strs = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			if (!string.IsNullOrEmpty(strs))
			{
				strlist = strs.Split(new char[]
				{
					','
				});
			}
			int offsetDay = Global.GetOffsetDay(DateTime.Parse(this.FromDate));
			int result;
			if (strlist == null || offsetDay != Global.SafeConvertToInt32(strlist[0]))
			{
				GameManager.GameConfigMgr.SetGameConfigItem("vip_fullpurchase", string.Format("{0}", offsetDay));
				Global.UpdateDBGameConfigg("vip_fullpurchase", string.Format("{0}", offsetDay));
				result = 0;
			}
			else
			{
				for (int index = 1; index < strlist.Length - 1; index += 2)
				{
					if (extTag == Global.SafeConvertToInt32(strlist[index]))
					{
						return Global.SafeConvertToInt32(strlist[index + 1]);
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000EBC8 File Offset: 0x0000CDC8
		protected void UpdateSinglePurchase(GameClient client, int extTag)
		{
			string strs = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(strs))
			{
				string[] strlist = strs.Split(new char[]
				{
					','
				});
				if (strlist.Length != 0)
				{
					string strsForSave = strlist[0];
					bool IfAddNewData = true;
					for (int index = 1; index < strlist.Length - 1; index += 2)
					{
						object obj = strsForSave;
						strsForSave = string.Concat(new object[]
						{
							obj,
							',',
							strlist[index],
							','
						});
						if (extTag == Global.SafeConvertToInt32(strlist[index]))
						{
							strsForSave += string.Format("{0}", Global.SafeConvertToInt32(strlist[index + 1]) + 1);
							IfAddNewData = false;
						}
						else
						{
							strsForSave += strlist[index + 1];
						}
					}
					if (IfAddNewData)
					{
						object obj = strsForSave;
						strsForSave = string.Concat(new object[]
						{
							obj,
							",",
							extTag,
							",1"
						});
					}
					Global.SaveRoleParamsStringToDB(client, "35", strsForSave, true);
				}
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000ED1C File Offset: 0x0000CF1C
		protected void UpdateFullPurchase(GameClient client, int extTag)
		{
			string strs = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			if (!string.IsNullOrEmpty(strs))
			{
				string[] strlist = strs.Split(new char[]
				{
					','
				});
				if (strlist.Length != 0)
				{
					string strsForSave = strlist[0];
					bool IfAddNewData = true;
					for (int index = 1; index < strlist.Length - 1; index += 2)
					{
						object obj = strsForSave;
						strsForSave = string.Concat(new object[]
						{
							obj,
							',',
							strlist[index],
							','
						});
						if (extTag == Global.SafeConvertToInt32(strlist[index]))
						{
							strsForSave += string.Format("{0}", Global.SafeConvertToInt32(strlist[index + 1]) + 1);
							IfAddNewData = false;
						}
						else
						{
							strsForSave += strlist[index + 1];
						}
					}
					if (IfAddNewData)
					{
						object obj = strsForSave;
						strsForSave = string.Concat(new object[]
						{
							obj,
							",",
							extTag,
							",1"
						});
					}
					GameManager.GameConfigMgr.SetGameConfigItem("vip_fullpurchase", strsForSave);
					Global.UpdateDBGameConfigg("vip_fullpurchase", strsForSave);
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000EE88 File Offset: 0x0000D088
		public string BuildQueryVIPYouHuiActivityCmd(GameClient client)
		{
			this.GetFullPurchase(client, 0);
			this.GetSinglePurchase(client, 0);
			string retstring = "";
			string strsFullPur = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			string strsSinglePur = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(strsFullPur) && strsFullPur.Split(new char[]
			{
				','
			}).Length > 1)
			{
				retstring += strsFullPur.Substring(strsFullPur.IndexOf(',') + 1);
			}
			retstring += '|';
			if (!string.IsNullOrEmpty(strsSinglePur) && strsSinglePur.Split(new char[]
			{
				','
			}).Length > 1)
			{
				retstring += strsSinglePur.Substring(strsSinglePur.IndexOf(',') + 1);
			}
			return retstring;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000EF70 File Offset: 0x0000D170
		public override bool CheckCondition(GameClient client, int extTag)
		{
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieriVIPYouHuiActivityConfig configData = null;
				result = (this.VIPYouHuiCofigDict.TryGetValue(extTag, out configData) && (configData.MinVIPLev < 0 || client.ClientData.VipLevel >= configData.MinVIPLev) && client.ClientData.UserMoney >= configData.Price && this.GetSinglePurchase(client, extTag) < configData.SinglePurchase && this.GetFullPurchase(client, extTag) < configData.FullPurchase);
			}
			return result;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000F028 File Offset: 0x0000D228
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				JieriVIPYouHuiActivityConfig configData = null;
				if (!this.VIPYouHuiCofigDict.TryGetValue(_params, out configData))
				{
					result = false;
				}
				else
				{
					if (configData.Price > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, configData.Price, "节日活动VIP优惠", true, true, false, DaiBiSySType.None))
						{
							return false;
						}
					}
					AwardItem myAwardItem = new AwardItem();
					myAwardItem.GoodsDataList = configData.GoodsDataListOne;
					base.GiveAward(client, myAwardItem);
					myAwardItem.GoodsDataList = configData.GoodsDataListTwo;
					base.GiveAward(client, myAwardItem);
					myAwardItem = configData.GoodsDataListThr.ToAwardItem();
					base.GiveEffectiveTimeAward(client, myAwardItem);
					this.UpdateSinglePurchase(client, _params);
					this.UpdateFullPurchase(client, _params);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000F114 File Offset: 0x0000D314
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			JieriVIPYouHuiActivityConfig configData = null;
			bool result;
			if (!this.VIPYouHuiCofigDict.TryGetValue(id, out configData))
			{
				result = false;
			}
			else
			{
				int nOccu = Global.CalcOriginalOccupationID(client);
				List<GoodsData> lData = new List<GoodsData>();
				foreach (GoodsData item in configData.GoodsDataListOne)
				{
					lData.Add(item);
				}
				int count = configData.GoodsDataListTwo.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData data = configData.GoodsDataListTwo[i];
					if (Global.IsCanGiveRewardByOccupation(client, data.GoodsID))
					{
						lData.Add(data);
					}
				}
				AwardItem tmpAwardItem = configData.GoodsDataListThr.ToAwardItem();
				foreach (GoodsData item in tmpAwardItem.GoodsDataList)
				{
					lData.Add(item);
				}
				result = Global.CanAddGoodsDataList(client, lData);
			}
			return result;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000F25C File Offset: 0x0000D45C
		public bool CanGetAnyAward(GameClient client)
		{
			foreach (KeyValuePair<int, JieriVIPYouHuiActivityConfig> item in this.VIPYouHuiCofigDict)
			{
				if (this.CheckCondition(client, item.Value.ID))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400017A RID: 378
		public const string VIPYouHuiActivityData_fileName = "Config/JieRiGifts/VIPYouHuiLiBao.xml";

		// Token: 0x0400017B RID: 379
		protected Dictionary<int, JieriVIPYouHuiActivityConfig> VIPYouHuiCofigDict = new Dictionary<int, JieriVIPYouHuiActivityConfig>();
	}
}
