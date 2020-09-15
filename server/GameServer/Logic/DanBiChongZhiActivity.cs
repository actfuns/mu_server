using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006FB RID: 1787
	public class DanBiChongZhiActivity : Activity
	{
		// Token: 0x06002AFC RID: 11004 RVA: 0x00264E64 File Offset: 0x00263064
		public override bool CheckCondition(GameClient client, int danBiID)
		{
			bool result;
			if (danBiID < 1 || danBiID > 9)
			{
				result = false;
			}
			else
			{
				lock (this.DanBiChongZhiAwardDic)
				{
					if (!this.DanBiChongZhiAwardDic.ContainsKey(danBiID))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x00264EDC File Offset: 0x002630DC
		public string DBQueryInfoCmd()
		{
			string dbCmd = "";
			lock (this.DanBiChongZhiAwardDic)
			{
				foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> item in this.DanBiChongZhiAwardDic)
				{
					object obj = dbCmd;
					dbCmd = string.Concat(new object[]
					{
						obj,
						item.Value.MinYuanBao,
						"_",
						item.Value.MaxYuanBao,
						"_"
					});
				}
			}
			if (!string.IsNullOrEmpty(dbCmd))
			{
				dbCmd = dbCmd.Substring(0, dbCmd.Length - 1);
			}
			return dbCmd;
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x00264FE8 File Offset: 0x002631E8
		public bool CheckDanBiChongZhiCountOK(GameClient client, int danBiId)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = this.GetDanBiChongZhiAwardDetail(client, danBiId);
			bool result;
			if (danBiChongZhiAwardDetail == null)
			{
				result = false;
			}
			else
			{
				int maxCount;
				if (danBiChongZhiAwardDetail.SinglePurchase > 127)
				{
					maxCount = 127;
				}
				else
				{
					maxCount = danBiChongZhiAwardDetail.SinglePurchase;
				}
				string sCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					this.FromDate.Replace(':', '$'),
					this.ToDate.Replace(':', '$'),
					this.DBQueryInfoCmd()
				});
				Dictionary<string, string> danBiDicInfo = Global.sendToDB<Dictionary<string, string>, string>(947, sCmd, client.ServerId);
				if (danBiDicInfo != null && danBiDicInfo.Count<KeyValuePair<string, string>>() > 0)
				{
					string key = string.Format("{0}_{1}", danBiChongZhiAwardDetail.MinYuanBao, danBiChongZhiAwardDetail.MaxYuanBao);
					string value = null;
					if (danBiDicInfo.TryGetValue(key, out value))
					{
						string[] spiltArr = value.Split(new char[]
						{
							'_'
						});
						if (spiltArr.Length != 2)
						{
							result = false;
						}
						else
						{
							int inputCount = Convert.ToInt32(spiltArr[0]);
							int awardCount = Convert.ToInt32(spiltArr[1]);
							result = (awardCount < maxCount && inputCount > awardCount);
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06002AFF RID: 11007 RVA: 0x00265178 File Offset: 0x00263378
		public DanBiChongZhiAwardDetail GetDanBiChongZhiAwardDetail(GameClient client, int danBiID)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				this.DanBiChongZhiAwardDic.TryGetValue(danBiID, out danBiChongZhiAwardDetail);
			}
			return danBiChongZhiAwardDetail;
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x002651D8 File Offset: 0x002633D8
		public override int GetParamsValidateCode()
		{
			int result;
			if (this.DanBiChongZhiAwardDic.Count > 9)
			{
				this.CodeForParamsValidate = -50003;
				LogManager.WriteLog(LogTypes.Error, string.Format("活动【{0}】的参数验证失败，错误码{1}", Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), this.CodeForParamsValidate), null, true);
				result = this.CodeForParamsValidate;
			}
			else
			{
				foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> item in this.DanBiChongZhiAwardDic)
				{
					if (item.Value.SinglePurchase > 127 || item.Value.ID < 1 || item.Value.ID > 9)
					{
						this.CodeForParamsValidate = -50003;
						LogManager.WriteLog(LogTypes.Error, string.Format("活动【{0}】的参数验证失败，错误码{1}", Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), this.CodeForParamsValidate), null, true);
						return this.CodeForParamsValidate;
					}
				}
				result = base.GetParamsValidateCode();
			}
			return result;
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x0026530C File Offset: 0x0026350C
		public override bool GiveAward(GameClient client, int danBiID)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				if (!this.DanBiChongZhiAwardDic.TryGetValue(danBiID, out danBiChongZhiAwardDetail))
				{
					return false;
				}
			}
			bool result;
			if (null == danBiChongZhiAwardDetail.AwardDict)
			{
				result = false;
			}
			else
			{
				base.GiveAward(client, danBiChongZhiAwardDetail.AwardDict);
				if (null == danBiChongZhiAwardDetail.AwardDict2)
				{
					result = false;
				}
				else
				{
					this.GiveAwardByOccupation(client, danBiChongZhiAwardDetail.AwardDict2, client.ClientData.Occupation);
					if (null == danBiChongZhiAwardDetail.EffectTimeAwardDict)
					{
						result = false;
					}
					else
					{
						base.GiveEffectiveTimeAward(client, danBiChongZhiAwardDetail.EffectTimeAwardDict.ToAwardItem());
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06002B02 RID: 11010 RVA: 0x002653F4 File Offset: 0x002635F4
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

		// Token: 0x06002B03 RID: 11011 RVA: 0x00265614 File Offset: 0x00263814
		public bool init()
		{
			this.DanBiChongZhiAwardDic.Clear();
			try
			{
				string fileName = "Config/JieRiGifts/JieRiDanBiChongZhi.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = new DanBiChongZhiAwardDetail();
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							AwardEffectTimeItem effectTimeAwardItem = new AwardEffectTimeItem();
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取单笔充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取单笔充值活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "单笔充值活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取单笔充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取单笔充值活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "单笔充值活动配置");
								}
							}
							string timeGoods = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
							string timeList = Global.GetSafeAttributeStr(xmlItem, "EffectiveTime");
							effectTimeAwardItem.Init(timeGoods, timeList, fileName + " 时效性物品");
							int minYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao");
							int maxYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MaxYuanBao");
							int singlePurchase = (int)Global.GetSafeAttributeLong(xmlItem, "SinglePurchase");
							int ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							danBiChongZhiAwardDetail.ID = ID;
							danBiChongZhiAwardDetail.AwardDict = myAwardItem;
							danBiChongZhiAwardDetail.AwardDict2 = myAwardItem2;
							danBiChongZhiAwardDetail.EffectTimeAwardDict = effectTimeAwardItem;
							danBiChongZhiAwardDetail.MinYuanBao = minYuanBao;
							danBiChongZhiAwardDetail.MaxYuanBao = maxYuanBao;
							danBiChongZhiAwardDetail.SinglePurchase = singlePurchase;
							this.DanBiChongZhiAwardDic[ID] = danBiChongZhiAwardDetail;
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiDanBiChongZhi.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		// Token: 0x06002B04 RID: 11012 RVA: 0x002659B4 File Offset: 0x00263BB4
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int danBIID)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				if (!this.DanBiChongZhiAwardDic.TryGetValue(danBIID, out danBiChongZhiAwardDetail))
				{
					return false;
				}
			}
			int totalCnt = danBiChongZhiAwardDetail.TotalAwardCntWithOcc(client);
			return Global.CanAddGoodsNum(client, totalCnt);
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x00265A2C File Offset: 0x00263C2C
		public bool CanGetAnyAward(GameClient client)
		{
			DanBiChongZhiActivity instance = HuodongCachingMgr.GetDanBiChongZhiActivity();
			string sCmd = "";
			if (null != instance)
			{
				sCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					instance.FromDate.Replace(':', '$'),
					instance.ToDate.Replace(':', '$'),
					instance.DBQueryInfoCmd()
				});
			}
			Dictionary<string, string> danBiDicInfo = Global.sendToDB<Dictionary<string, string>, string>(947, sCmd, client.ServerId);
			if (danBiDicInfo != null && danBiDicInfo.Count<KeyValuePair<string, string>>() > 0)
			{
				lock (this.DanBiChongZhiAwardDic)
				{
					foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> item in this.DanBiChongZhiAwardDic)
					{
						string key = string.Format("{0}_{1}", item.Value.MinYuanBao, item.Value.MaxYuanBao);
						string value = null;
						if (danBiDicInfo.TryGetValue(key, out value))
						{
							string[] fileds = value.Split(new char[]
							{
								'_'
							});
							if (fileds.Length == 2)
							{
								int canGetCount = Convert.ToInt32(fileds[0]);
								int getCount = Convert.ToInt32(fileds[1]);
								int maxCount = item.Value.SinglePurchase;
								if (canGetCount > getCount && getCount < maxCount)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04003A17 RID: 14871
		public Dictionary<int, DanBiChongZhiAwardDetail> DanBiChongZhiAwardDic = new Dictionary<int, DanBiChongZhiAwardDetail>();
	}
}
