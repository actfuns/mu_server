using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001BD RID: 445
	public class JieriLianXuChargeActivity : Activity
	{
		// Token: 0x06000584 RID: 1412 RVA: 0x0004DB38 File Offset: 0x0004BD38
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.CfgFile));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.CfgFile));
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
				Dictionary<int, JieriLianXuChargeActivity._ChargeLvl> awardId2ChargeLvl = new Dictionary<int, JieriLianXuChargeActivity._ChargeLvl>();
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							int awardId = (int)Global.GetSafeAttributeLong(xmlItem, "Group");
							int needCharge = (int)Global.GetSafeAttributeLong(xmlItem, "NeedZuanShi");
							JieriLianXuChargeActivity._ChargeLvl chargeLvl = null;
							if (!awardId2ChargeLvl.TryGetValue(awardId, out chargeLvl))
							{
								chargeLvl = new JieriLianXuChargeActivity._ChargeLvl();
								chargeLvl.Id = awardId;
								chargeLvl.NeedCharge = needCharge;
								awardId2ChargeLvl[awardId] = chargeLvl;
							}
							JieriLianXuChargeActivity._DayAward dayAward = new JieriLianXuChargeActivity._DayAward();
							dayAward.LianXuDay = (int)Global.GetSafeAttributeLong(xmlItem, "Day");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项为空", this.CfgFile), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									dayAward.AwardGoods.GoodsDataList.AddRange(HuodongCachingMgr.ParseGoodsDataList(fields, "连续充值活动goods1配置"));
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									dayAward.AwardGoods.GoodsDataList.AddRange(HuodongCachingMgr.ParseGoodsDataList(fields, "连续充值活动goods2配置"));
								}
							}
							chargeLvl.AwardList.Add(dayAward);
						}
					}
					this.chargeLvlList.AddRange(awardId2ChargeLvl.Values.ToList<JieriLianXuChargeActivity._ChargeLvl>());
					base.PredealDateTime();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0004DEA4 File Offset: 0x0004C0A4
		public string QueryMyActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = string.Format("{0}", 2);
			}
			else
			{
				List<JieriLianXuChargeActivity._AwardInfo> myDataLst = this._GetMyActInfoFromDB(client);
				if (myDataLst == null)
				{
					result = string.Format("{0}", 4);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(0);
					foreach (JieriLianXuChargeActivity._AwardInfo info in myDataLst)
					{
						sb.Append(":").Append(info.AwardId);
						sb.Append(",").Append(info.LianXuDay);
						sb.Append(",").Append(info.AwardFlag);
					}
					result = sb.ToString();
				}
			}
			return result;
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0004E044 File Offset: 0x0004C244
		public JieriLianXuChargeErrorCode HandleGetAward(GameClient client, int awardId, int day)
		{
			JieriLianXuChargeErrorCode result;
			if (!this.InAwardTime() || client == null)
			{
				result = JieriLianXuChargeErrorCode.NotAwardTime;
			}
			else
			{
				JieriLianXuChargeActivity._ChargeLvl cl = this.chargeLvlList.Find((JieriLianXuChargeActivity._ChargeLvl _cl) => _cl.Id == awardId);
				if (cl == null)
				{
					result = JieriLianXuChargeErrorCode.ConfigError;
				}
				else
				{
					JieriLianXuChargeActivity._DayAward da = cl.AwardList.Find((JieriLianXuChargeActivity._DayAward _da) => _da.LianXuDay == day);
					if (da == null)
					{
						result = JieriLianXuChargeErrorCode.ConfigError;
					}
					else
					{
						List<JieriLianXuChargeActivity._AwardInfo> myDataLst = this._GetMyActInfoFromDB(client);
						if (myDataLst == null)
						{
							result = JieriLianXuChargeErrorCode.DBFailed;
						}
						else
						{
							JieriLianXuChargeActivity._AwardInfo info = myDataLst.Find((JieriLianXuChargeActivity._AwardInfo _info) => _info.AwardId == awardId);
							if (info == null)
							{
								result = JieriLianXuChargeErrorCode.ConfigError;
							}
							else if (info.LianXuDay < day || Global.GetIntSomeBit(info.AwardFlag, day) == 1)
							{
								result = JieriLianXuChargeErrorCode.NotMeetAwardCond;
							}
							else
							{
								if (da.AwardGoods != null && da.AwardGoods.GoodsDataList != null && da.AwardGoods.GoodsDataList.Count > 0)
								{
									int AwardGoodsCnt = da.AwardGoods.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
									if (!Global.CanAddGoodsNum(client, AwardGoodsCnt))
									{
										return JieriLianXuChargeErrorCode.NoBagSpace;
									}
								}
								int newAwardFlag = Global.SetIntSomeBit(day, info.AwardFlag, true);
								if (!this._UpdateAwardFlag2DB(client, awardId, newAwardFlag))
								{
									result = JieriLianXuChargeErrorCode.DBFailed;
								}
								else
								{
									info.AwardFlag = newAwardFlag;
									base.GiveAward(client, da.AwardGoods);
									if (client._IconStateMgr.CheckJieriLianXuCharge(client))
									{
										client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
										client._IconStateMgr.SendIconStateToClient(client);
									}
									result = JieriLianXuChargeErrorCode.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0004E2C0 File Offset: 0x0004C4C0
		private bool _UpdateAwardFlag2DB(GameClient client, int awardId, int awardFlag)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				string cmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					awardId,
					this.FromDate.Replace(':', '$'),
					this.ToDate.Replace(':', '$'),
					awardFlag
				});
				string[] dbRet = Global.ExecuteDBCmd(13215, cmd, client.ServerId);
				result = (dbRet != null && dbRet.Length == 1 && Convert.ToInt32(dbRet[0]) > 0);
			}
			return result;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0004E37C File Offset: 0x0004C57C
		private List<JieriLianXuChargeActivity._AwardInfo> _GetMyActInfoFromDB(GameClient client)
		{
			List<JieriLianXuChargeActivity._AwardInfo> result2;
			if (client == null)
			{
				result2 = null;
			}
			else if (!this.InActivityTime() && !this.InAwardTime())
			{
				result2 = null;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(client.ClientData.RoleID);
				sb.Append(':').Append(client.ClientData.ZoneID);
				sb.Append(':').Append(this.FromDate.Replace(':', '$'));
				sb.Append(':').Append(this.ToDate.Replace(':', '$'));
				sb.Append(':');
				foreach (JieriLianXuChargeActivity._ChargeLvl cl in this.chargeLvlList)
				{
					sb.Append(cl.Id).Append('_');
				}
				string[] dbRet = Global.ExecuteDBCmd(13214, sb.ToString(), client.ServerId);
				if (dbRet == null || dbRet.Length != 2)
				{
					result2 = null;
				}
				else
				{
					int[] eachDayChargeArr = this._ParseEachDayCharge(dbRet[0]);
					Dictionary<int, int> awardFlagDic = this._ParseAwardFlagOfEachLvl(dbRet[1]);
					List<JieriLianXuChargeActivity._AwardInfo> result = new List<JieriLianXuChargeActivity._AwardInfo>();
					foreach (JieriLianXuChargeActivity._ChargeLvl cl in this.chargeLvlList)
					{
						JieriLianXuChargeActivity._AwardInfo ai = new JieriLianXuChargeActivity._AwardInfo();
						ai.LianXuDay = this._CalcLianXuChargeDay(eachDayChargeArr, cl.NeedCharge);
						ai.AwardId = cl.Id;
						ai.AwardFlag = 0;
						if (awardFlagDic.ContainsKey(cl.Id))
						{
							ai.AwardFlag = awardFlagDic[cl.Id];
						}
						result.Add(ai);
					}
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0004E594 File Offset: 0x0004C794
		private int _CalcLianXuChargeDay(int[] eachDayChargeArray, int atLeastCharge)
		{
			int result;
			if (eachDayChargeArray == null || atLeastCharge <= 0)
			{
				result = 0;
			}
			else
			{
				int dayCnt = 0;
				for (int i = 0; i < eachDayChargeArray.Length; i++)
				{
					if (eachDayChargeArray[i] >= atLeastCharge)
					{
						dayCnt++;
					}
				}
				result = dayCnt;
			}
			return result;
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0004E5E4 File Offset: 0x0004C7E4
		private Dictionary<int, int> _ParseAwardFlagOfEachLvl(string strAwardIdAndFlag)
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			if (!string.IsNullOrEmpty(strAwardIdAndFlag))
			{
				string[] szIdFlag = strAwardIdAndFlag.Split(new char[]
				{
					'$'
				});
				foreach (string str in szIdFlag)
				{
					if (!string.IsNullOrEmpty(str))
					{
						string[] fields = str.Split(new char[]
						{
							','
						});
						int awardId = Convert.ToInt32(fields[0]);
						int awardFlag = Convert.ToInt32(fields[1]);
						result[awardId] = awardFlag;
					}
				}
			}
			return result;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0004E694 File Offset: 0x0004C894
		private int[] _ParseEachDayCharge(string strMoneyOfDays)
		{
			int[] result;
			if (string.IsNullOrEmpty(strMoneyOfDays))
			{
				result = null;
			}
			else
			{
				Dictionary<string, int> chargeOfDay = new Dictionary<string, int>();
				string[] szDayCharge = strMoneyOfDays.Split(new char[]
				{
					'$'
				});
				foreach (string str in szDayCharge)
				{
					if (!string.IsNullOrEmpty(str))
					{
						string[] fields = str.Split(new char[]
						{
							','
						});
						string day = fields[0];
						int money = Convert.ToInt32(fields[1]);
						if (chargeOfDay.ContainsKey(day))
						{
							Dictionary<string, int> dictionary;
							string key;
							(dictionary = chargeOfDay)[key = day] = dictionary[key] + money;
						}
						else
						{
							chargeOfDay.Add(day, money);
						}
					}
				}
				DateTime _startReal = DateTime.Parse(this.FromDate);
				DateTime _endReal = DateTime.Parse(this.ToDate);
				DateTime _startMorning = new DateTime(_startReal.Year, _startReal.Month, _startReal.Day);
				DateTime _endMorning = new DateTime(_endReal.Year, _endReal.Month, _endReal.Day);
				int actTotalDay = (int)(_endMorning - _startMorning).TotalDays + 1;
				if (actTotalDay <= 0)
				{
					result = null;
				}
				else
				{
					int[] eachDayChargeArray = new int[actTotalDay];
					for (int i = 0; i < actTotalDay; i++)
					{
						string szDay = _startMorning.AddDays((double)i).ToString("yyyy-MM-dd");
						if (chargeOfDay.ContainsKey(szDay))
						{
							eachDayChargeArray[i] = chargeOfDay[szDay];
						}
						else
						{
							eachDayChargeArray[i] = 0;
						}
					}
					result = eachDayChargeArray;
				}
			}
			return result;
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0004E88C File Offset: 0x0004CA8C
		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null || !this.InAwardTime())
			{
				result = false;
			}
			else
			{
				List<JieriLianXuChargeActivity._AwardInfo> myDataLst = this._GetMyActInfoFromDB(client);
				if (myDataLst == null)
				{
					result = false;
				}
				else
				{
					using (List<JieriLianXuChargeActivity._AwardInfo>.Enumerator enumerator = myDataLst.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JieriLianXuChargeActivity._AwardInfo info = enumerator.Current;
							JieriLianXuChargeActivity._ChargeLvl cl = this.chargeLvlList.Find((JieriLianXuChargeActivity._ChargeLvl _cl) => _cl.Id == info.AwardId);
							if (cl == null)
							{
								return false;
							}
							foreach (JieriLianXuChargeActivity._DayAward award in cl.AwardList)
							{
								if (award.LianXuDay <= info.LianXuDay && Global.GetIntSomeBit(info.AwardFlag, award.LianXuDay) == 0)
								{
									return true;
								}
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x040009EB RID: 2539
		private readonly string CfgFile = "Config/JieRiGifts/JieRiLianXu.xml";

		// Token: 0x040009EC RID: 2540
		private List<JieriLianXuChargeActivity._ChargeLvl> chargeLvlList = new List<JieriLianXuChargeActivity._ChargeLvl>();

		// Token: 0x020001BE RID: 446
		private class _AwardInfo
		{
			// Token: 0x040009ED RID: 2541
			public int AwardId;

			// Token: 0x040009EE RID: 2542
			public int LianXuDay;

			// Token: 0x040009EF RID: 2543
			public int AwardFlag;
		}

		// Token: 0x020001BF RID: 447
		private class _DayAward
		{
			// Token: 0x040009F0 RID: 2544
			public int LianXuDay;

			// Token: 0x040009F1 RID: 2545
			public AwardItem AwardGoods = new AwardItem();
		}

		// Token: 0x020001C0 RID: 448
		private class _ChargeLvl
		{
			// Token: 0x040009F2 RID: 2546
			public int Id;

			// Token: 0x040009F3 RID: 2547
			public int NeedCharge;

			// Token: 0x040009F4 RID: 2548
			public List<JieriLianXuChargeActivity._DayAward> AwardList = new List<JieriLianXuChargeActivity._DayAward>();
		}
	}
}
