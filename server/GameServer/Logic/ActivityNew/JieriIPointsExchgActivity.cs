using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001BB RID: 443
	public class JieriIPointsExchgActivity : Activity
	{
		// Token: 0x06000579 RID: 1401 RVA: 0x0004CF5C File Offset: 0x0004B15C
		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			if (this.InActivityTime())
			{
				string strYuanbaoToIPoints = GameManager.systemParamsList.GetParamValueByName("JieRiChongZhiDuiHuan");
				if (!string.IsNullOrEmpty(strYuanbaoToIPoints))
				{
					string[] strFieldsMtoIPoint = strYuanbaoToIPoints.Split(new char[]
					{
						':'
					});
					if (strFieldsMtoIPoint.Length == 2)
					{
						int DivIPoints = Convert.ToInt32(strFieldsMtoIPoint[0]);
						if (DivIPoints != 0)
						{
							double YuanbaoToIPointsDiv = Convert.ToDouble(strFieldsMtoIPoint[1]) / (double)DivIPoints;
							int IPointsAdd = (int)(YuanbaoToIPointsDiv * (double)Global.TransMoneyToYuanBao(addMoney));
							string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								roleid,
								IPointsAdd,
								this.FromDate.Replace(':', '$'),
								this.ToDate.Replace(':', '$')
							});
							Global.ExecuteDBCmd(13151, strcmd, 0);
						}
					}
				}
			}
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0004D058 File Offset: 0x0004B258
		public override bool CheckCondition(GameClient client, int extTag)
		{
			IPointsExchgData ipointsExchgData;
			bool result;
			if (!this.AwardItemDict.TryGetValue(extTag, out ipointsExchgData))
			{
				result = false;
			}
			else if (this.GetIPointsLeftMergeNum(client, extTag) <= 0)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				string[] fields = Global.ExecuteDBCmd(1500, strcmd, client.ServerId);
				result = (fields != null && fields.Length >= 2 && Convert.ToInt32(fields[1]) >= ipointsExchgData.MinAwardCondionValue);
			}
			return result;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0004D124 File Offset: 0x0004B324
		public void NotifyInputPointsInfo(GameClient client, bool bPointsOnly = false)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
			string[] fields = Global.ExecuteDBCmd(1500, strcmd, client.ServerId);
			if (fields != null && fields.Length >= 2)
			{
				string cmdDataDB = fields[0] + ':' + fields[1];
				if (bPointsOnly)
				{
					client.sendCmd(1502, cmdDataDB, false);
				}
				else
				{
					string cmdDataClient = "";
					this.BuildInputPointsDataCmdForClient(client, cmdDataDB, out cmdDataClient);
					client.sendCmd(1500, cmdDataClient, false);
				}
			}
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0004D1E8 File Offset: 0x0004B3E8
		public override bool GiveAward(GameClient client, int _params)
		{
			IPointsExchgData ipointsExchgData;
			bool result;
			if (!this.AwardItemDict.TryGetValue(_params, out ipointsExchgData))
			{
				result = false;
			}
			else
			{
				int retInputPoints = 0;
				if (ipointsExchgData.MinAwardCondionValue > 0)
				{
					int InputPointsCost = -ipointsExchgData.MinAwardCondionValue;
					string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.RoleID,
						InputPointsCost,
						this.FromDate.Replace(':', '$'),
						this.ToDate.Replace(':', '$')
					});
					string[] fields = Global.ExecuteDBCmd(13151, strcmd, client.ServerId);
					if (fields == null || fields.Length < 2)
					{
						return false;
					}
					retInputPoints = Convert.ToInt32(fields[1]);
					if (retInputPoints < 0)
					{
						return false;
					}
				}
				this.ModifyIPointsLeftMergeNum(client, _params, 1);
				base.GiveAward(client, ipointsExchgData);
				this.NotifyInputPointsInfo(client, true);
				client._IconStateMgr.CheckJieRiActivity(client, false);
				client._IconStateMgr.SendIconStateToClient(client);
				string castResList = EventLogManager.NewResPropString(ResLogType.InputPoints, new object[]
				{
					-ipointsExchgData.MinAwardCondionValue,
					retInputPoints + ipointsExchgData.MinAwardCondionValue,
					retInputPoints
				});
				string strResList = EventLogManager.MakeGoodsDataPropString(ipointsExchgData.GoodsDataList);
				EventLogManager.AddPurchaseEvent(client, 6, _params, castResList, strResList);
				result = true;
			}
			return result;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0004D378 File Offset: 0x0004B578
		public bool Init()
		{
			try
			{
				string fileName = "Config/JieRiGifts/ChongZhiDuiHuan.xml";
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
							IPointsExchgData myAwardItem = new IPointsExchgData();
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "NeedChongZhiDianShu"));
							myAwardItem.AwardYuanBao = 0;
							myAwardItem.DayMaxTimes = (int)Global.GetSafeAttributeLong(xmlItem, "MaxNum");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "NewGoodsID");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值点兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值点兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日节日充值点兑换配置1");
								}
							}
							this.AwardItemDict[id] = myAwardItem;
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

		// Token: 0x0600057E RID: 1406 RVA: 0x0004D5FC File Offset: 0x0004B7FC
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			IPointsExchgData allItem = null;
			this.AwardItemDict.TryGetValue(id, out allItem);
			int awardCnt = 0;
			if (allItem != null && allItem.GoodsDataList != null)
			{
				awardCnt += allItem.GoodsDataList.Count;
			}
			return Global.CanAddGoodsNum(client, awardCnt);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0004D64C File Offset: 0x0004B84C
		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				string[] dbFields = null;
				string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 1500, strDbCmd, out dbFields, client.ServerId);
				if (null == dbFields)
				{
					result = false;
				}
				else if (dbFields == null || 2 != dbFields.Length)
				{
					result = false;
				}
				else
				{
					int InputPoints = Convert.ToInt32(dbFields[1]);
					if (InputPoints <= 0)
					{
						result = false;
					}
					else
					{
						foreach (KeyValuePair<int, IPointsExchgData> kvp in this.AwardItemDict)
						{
							int awardid = kvp.Key;
							IPointsExchgData item = kvp.Value;
							if (item.MinAwardCondionValue <= InputPoints && this.GetIPointsLeftMergeNum(client, kvp.Key) > 0)
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0004D7C0 File Offset: 0x0004B9C0
		public void BuildInputPointsDataCmdForClient(GameClient client, string strCmdDB, out string strCmdClient)
		{
			strCmdClient = strCmdDB;
			string[] dbFields = strCmdDB.Split(new char[]
			{
				':'
			});
			if (null != dbFields)
			{
				if (dbFields != null && 2 == dbFields.Length)
				{
					if (!this.InActivityTime())
					{
						strCmdClient = null;
						strCmdClient += dbFields[0];
						strCmdClient += ':';
						strCmdClient += '0';
						strCmdClient += ':';
						foreach (KeyValuePair<int, IPointsExchgData> kvp in this.AwardItemDict)
						{
							strCmdClient += Convert.ToString(kvp.Value.DayMaxTimes);
							strCmdClient += "|";
						}
					}
					else
					{
						strCmdClient += ":";
						foreach (KeyValuePair<int, IPointsExchgData> kvp in this.AwardItemDict)
						{
							strCmdClient += this.GetIPointsLeftMergeNum(client, kvp.Key);
							strCmdClient += '|';
						}
					}
				}
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0004D94C File Offset: 0x0004BB4C
		public int GetIPointsLeftMergeNum(GameClient client, int index)
		{
			JieriIPointsExchgActivity instance = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			int result;
			if (null == instance)
			{
				result = 0;
			}
			else
			{
				IPointsExchgData ExchgData = null;
				this.AwardItemDict.TryGetValue(index, out ExchgData);
				if (ExchgData == null)
				{
					result = 0;
				}
				else
				{
					DateTime startTime = DateTime.Parse(this.FromDate);
					int currday = Global.GetOffsetDay(startTime);
					int lastday = 0;
					int count = 0;
					string strFlag = "InputPointExchg" + index;
					string JieRiIPointExchgFlag = Global.GetRoleParamByName(client, strFlag);
					if (null != JieRiIPointExchgFlag)
					{
						string[] fields = JieRiIPointExchgFlag.Split(new char[]
						{
							','
						});
						if (2 == fields.Length)
						{
							lastday = Convert.ToInt32(fields[0]);
							count = Convert.ToInt32(fields[1]);
						}
					}
					if (currday == lastday)
					{
						result = ExchgData.DayMaxTimes - count;
					}
					else
					{
						result = ExchgData.DayMaxTimes;
					}
				}
			}
			return result;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0004DA48 File Offset: 0x0004BC48
		public int ModifyIPointsLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			DateTime startTime = DateTime.Parse(this.FromDate);
			int currday = Global.GetOffsetDay(startTime);
			string strFlag = "InputPointExchg" + index;
			string JieRiIPointExchgFlag = Global.GetRoleParamByName(client, strFlag);
			int lastday = 0;
			int count = 0;
			if (null != JieRiIPointExchgFlag)
			{
				string[] fields = JieRiIPointExchgFlag.Split(new char[]
				{
					','
				});
				if (2 != fields.Length)
				{
					return 0;
				}
				lastday = Convert.ToInt32(fields[0]);
				count = Convert.ToInt32(fields[1]);
			}
			if (currday == lastday)
			{
				count += addNum;
			}
			else
			{
				lastday = currday;
				count = addNum;
			}
			string result = string.Format("{0},{1}", lastday, count);
			Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
			return count;
		}

		// Token: 0x040009E1 RID: 2529
		protected Dictionary<int, IPointsExchgData> AwardItemDict = new Dictionary<int, IPointsExchgData>();
	}
}
