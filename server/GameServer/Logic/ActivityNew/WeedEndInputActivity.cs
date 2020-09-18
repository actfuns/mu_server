using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class WeedEndInputActivity : Activity
	{
		
		public override bool InActivityTime()
		{
			bool result;
			if (string.IsNullOrEmpty(this.FromDate) || string.IsNullOrEmpty(this.ToDate))
			{
				result = false;
			}
			else
			{
				int NowDayOfWeek = (int)TimeUtil.NowDateTime().DayOfWeek;
				string[] DataBeginSplit = this.FromDate.Split(new char[]
				{
					','
				});
				string[] DataEndSplit = this.ToDate.Split(new char[]
				{
					','
				});
				if (0 == NowDayOfWeek)
				{
					NowDayOfWeek = 7;
				}
				int BeginDayOfWeek = Convert.ToInt32(DataBeginSplit[0]);
				int EndDayOfWeek = Convert.ToInt32(DataEndSplit[0]);
				if (NowDayOfWeek < BeginDayOfWeek)
				{
					result = false;
				}
				else if (NowDayOfWeek > EndDayOfWeek)
				{
					result = false;
				}
				else
				{
					string nowTime = TimeUtil.NowDateTime().ToString("HH:mm:ss");
					if (BeginDayOfWeek == EndDayOfWeek)
					{
						if (nowTime.CompareTo(DataBeginSplit[1]) > 0 && nowTime.CompareTo(DataEndSplit[1]) < 0)
						{
							return true;
						}
					}
					else if (NowDayOfWeek == BeginDayOfWeek)
					{
						if (nowTime.CompareTo(DataBeginSplit[1]) > 0)
						{
							return true;
						}
					}
					else if (NowDayOfWeek == EndDayOfWeek)
					{
						if (nowTime.CompareTo(DataEndSplit[1]) < 0)
						{
							return true;
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public override bool InAwardTime()
		{
			return !string.IsNullOrEmpty(this.FromDate) && !string.IsNullOrEmpty(this.ToDate) && this.InActivityTime();
		}

		
		public override int GetParamsValidateCode()
		{
			return 1;
		}

		
		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			if (this.InActivityTime())
			{
				int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
				string strFlag = "WeekEndInputFlag";
				string WeekEndInputRandData = Global.GetRoleParamByName(client, strFlag);
				if (null != WeekEndInputRandData)
				{
					string[] fields = WeekEndInputRandData.Split(new char[]
					{
						'#'
					});
					if (fields.Length == 2)
					{
						int lastday = Convert.ToInt32(fields[0]);
						if (currday == lastday)
						{
							return;
						}
					}
				}
				string result = string.Format("{0}", currday);
				result += '#';
				result += this.BuildRandAwardData(client);
				Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
				if (!isLogin)
				{
				}
			}
		}

		
		public void SyncWeekEndInputData(GameClient client)
		{
			string strcmd = "";
			string strFlag = "WeekEndInputFlag";
			string WeekEndInputRandData = Global.GetRoleParamByName(client, strFlag);
			if (string.IsNullOrEmpty(WeekEndInputRandData))
			{
				strcmd = string.Format("{0}:{1}", -1, 0);
			}
			else
			{
				string[] InputRandData = WeekEndInputRandData.Split(new char[]
				{
					'#'
				});
				if (InputRandData.Length == 2)
				{
					strcmd = string.Format("{0}:{1}", 0, InputRandData[1]);
				}
			}
			client.sendCmd(1501, strcmd, false);
		}

		
		public int GetWeekEndInputOpenDay(GameClient client)
		{
			int OpenDay = 0;
			string strFlag = "WeekEndInputOD";
			string WeekEndInputOpenDay = Global.GetRoleParamByName(client, strFlag);
			if (!string.IsNullOrEmpty(WeekEndInputOpenDay))
			{
				OpenDay = Convert.ToInt32(WeekEndInputOpenDay);
			}
			return OpenDay;
		}

		
		public void UpdateWeekEndInputOpenDay(GameClient client)
		{
			if (this.InAwardTime())
			{
				int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
				string strFlag = "WeekEndInputOD";
				Global.SaveRoleParamsStringToDB(client, strFlag, Convert.ToString(currday), true);
			}
		}

		
		public override bool GiveAward(GameClient client, int NeedYuanBao)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				string strFlag = "WeekEndInputFlag";
				string WeekEndInputRandData = Global.GetRoleParamByName(client, strFlag);
				string[] InputRandData = WeekEndInputRandData.Split(new char[]
				{
					'#'
				});
				if (InputRandData.Length < 2)
				{
					result = false;
				}
				else
				{
					string[] AwardArray = InputRandData[1].Split(new char[]
					{
						'|'
					});
					if (AwardArray.Length <= 0)
					{
						result = false;
					}
					else
					{
						foreach (string awarditem in AwardArray)
						{
							string[] award = awarditem.Split(new char[]
							{
								'$'
							});
							int AwardID = Convert.ToInt32(award[1]);
							WeekEndInputTypeData InputType = null;
							this.InputTypeDict.TryGetValue(AwardID, out InputType);
							if (InputType != null && InputType.MinZuanShi == NeedYuanBao)
							{
								List<WeekEndInputAwardData> AwardList = null;
								this.AwardItemDict.TryGetValue(AwardID, out AwardList);
								if (null != AwardList)
								{
									string[] arrayid = award[2].Split(new char[]
									{
										','
									});
									for (int j = 0; j < arrayid.Length; j++)
									{
										int id = Convert.ToInt32(arrayid[j]);
										if (id > 0 && id <= AwardList.Count)
										{
											base.GiveAward(client, AwardList[id - 1]);
										}
									}
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		
		public int GetNeedGoodsSpace(GameClient client, int NeedYuanBao)
		{
			int result;
			if (!this.InAwardTime())
			{
				result = 0;
			}
			else
			{
				string strFlag = "WeekEndInputFlag";
				string WeekEndInputRandData = Global.GetRoleParamByName(client, strFlag);
				string[] InputRandData = WeekEndInputRandData.Split(new char[]
				{
					'#'
				});
				if (InputRandData.Length < 2)
				{
					result = 0;
				}
				else
				{
					string[] AwardArray = InputRandData[1].Split(new char[]
					{
						'|'
					});
					if (AwardArray.Length <= 0)
					{
						result = 0;
					}
					else
					{
						foreach (string awarditem in AwardArray)
						{
							string[] award = awarditem.Split(new char[]
							{
								'$'
							});
							int AwardID = Convert.ToInt32(award[1]);
							WeekEndInputTypeData InputType = null;
							this.InputTypeDict.TryGetValue(AwardID, out InputType);
							if (InputType != null && InputType.MinZuanShi == NeedYuanBao)
							{
								return InputType.Num;
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		
		public string BuildRandAwardData(GameClient client)
		{
			string strResult = "";
			string result;
			if (!this.InActivityTime())
			{
				result = strResult;
			}
			else
			{
				MeiRiChongZhiActivity actMeiRi = HuodongCachingMgr.GetMeiRiChongZhiActivity();
				if (null == actMeiRi)
				{
					result = strResult;
				}
				else
				{
					List<KeyValuePair<int, WeekEndInputTypeData>> RewardTypeList = new List<KeyValuePair<int, WeekEndInputTypeData>>();
					int nChangeLifeCount = client.ClientData.ChangeLifeCount;
					int nLev = client.ClientData.Level;
					foreach (KeyValuePair<int, WeekEndInputTypeData> kvp in this.InputTypeDict)
					{
						if (kvp.Value.MaxLevel >= nLev && kvp.Value.MinLevel <= nLev && kvp.Value.MaxZhuanSheng >= nChangeLifeCount && kvp.Value.MinZhuanSheng <= nChangeLifeCount)
						{
							RewardTypeList.Add(new KeyValuePair<int, WeekEndInputTypeData>(kvp.Key, kvp.Value));
						}
					}
					foreach (KeyValuePair<int, WeekEndInputTypeData> kvp in RewardTypeList)
					{
						List<WeekEndInputAwardData> AwardList = null;
						this.AwardItemDict.TryGetValue(kvp.Key, out AwardList);
						if (null != AwardList)
						{
							int WhitchOne = actMeiRi.GetIDByYuanBao(kvp.Value.MinZuanShi);
							strResult += WhitchOne;
							strResult += "$";
							strResult += kvp.Key;
							strResult += "$";
							int PercentZero = AwardList[0].RandBeginNum;
							int PercentOne = AwardList[AwardList.Count - 1].RandEndNum;
							lock (this.AwardItemDict)
							{
								for (int Num = 0; Num < kvp.Value.Num; Num++)
								{
									int rate = Global.GetRandomNumber(PercentZero, PercentOne);
									for (int i = 0; i < AwardList.Count; i++)
									{
										if (AwardList[i].RandSkip)
										{
											rate += AwardList[i].RandNumMinus;
										}
										if (!AwardList[i].RandSkip && rate >= AwardList[i].RandBeginNum && rate <= AwardList[i].RandEndNum)
										{
											AwardList[i].RandSkip = true;
											PercentOne -= AwardList[i].RandNumMinus;
											strResult += AwardList[i].id;
											if (Num != kvp.Value.Num - 1)
											{
												strResult += ",";
											}
											break;
										}
									}
								}
								strResult += "|";
								for (int i = 0; i < AwardList.Count; i++)
								{
									if (AwardList[i].RandSkip)
									{
									}
									AwardList[i].RandSkip = false;
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(strResult) && strResult.Substring(strResult.Length - 1) == "|")
					{
						strResult = strResult.Substring(0, strResult.Length - 1);
					}
					result = strResult;
				}
			}
			return result;
		}

		
		public bool ParseActivityTime(string ZhouMoChongZhiTime)
		{
			string[] TimeActivity = ZhouMoChongZhiTime.Split(new char[]
			{
				'|'
			});
			bool result;
			if (TimeActivity == null || TimeActivity.Length != 2)
			{
				result = false;
			}
			else
			{
				string[] DataBeginSplit = TimeActivity[0].Split(new char[]
				{
					','
				});
				string[] DataEndSplit = TimeActivity[1].Split(new char[]
				{
					','
				});
				if (DataBeginSplit == null || DataEndSplit == null || DataBeginSplit.Length != 2 || DataEndSplit.Length != 2)
				{
					result = false;
				}
				else
				{
					this.FromDate = DataBeginSplit[0] + ',' + DataBeginSplit[1];
					this.ToDate = DataEndSplit[0] + ',' + DataEndSplit[1];
					this.FromDate.Trim();
					this.ToDate.Trim();
					result = true;
				}
			}
			return result;
		}

		
		public bool Init()
		{
			try
			{
				string ZhouMoChongZhiTime = GameManager.systemParamsList.GetParamValueByName("ZhouMoChongZhiTime");
				if (!string.IsNullOrEmpty(ZhouMoChongZhiTime))
				{
					if (!this.ParseActivityTime(ZhouMoChongZhiTime))
					{
						return false;
					}
				}
				string fileName = Global.IsolateResPath("Config/Gifts/ZhouMoChongZhiType.xml");
				XElement xmlType = GeneralCachingXmlMgr.GetXElement(fileName);
				if (null == xmlType)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xmlType.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						WeekEndInputTypeData myInputType = new WeekEndInputTypeData();
						int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myInputType.MinZhuanSheng = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng"));
						myInputType.MinLevel = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"));
						myInputType.MaxZhuanSheng = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MaxZhuanSheng"));
						myInputType.MaxLevel = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel"));
						myInputType.MinZuanShi = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinZuanShi"));
						myInputType.Num = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Num"));
						this.InputTypeDict[id] = myInputType;
					}
				}
				fileName = Global.IsolateResPath("Config/Gifts/ZhouMoChongZhi.xml");
				XElement xmlAward = GeneralCachingXmlMgr.GetXElement(fileName);
				if (null == xmlAward)
				{
					return false;
				}
				IEnumerable<XElement> xmlItemsAward = xmlAward.Elements();
				foreach (XElement xmlRandAwardList in xmlItemsAward)
				{
					if (null != xmlRandAwardList)
					{
						List<WeekEndInputAwardData> myRandAwardList = new List<WeekEndInputAwardData>();
						int id = (int)Global.GetSafeAttributeLong(xmlRandAwardList, "ID");
						IEnumerable<XElement> xmlRandAwards = xmlRandAwardList.Elements();
						foreach (XElement xmlRandAward in xmlRandAwards)
						{
							WeekEndInputAwardData myRandAward = new WeekEndInputAwardData();
							myRandAward.id = (int)Global.GetSafeAttributeLong(xmlRandAward, "ID");
							myRandAward.RandBeginNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlRandAward, "BeginNum"));
							myRandAward.RandEndNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlRandAward, "EndNum"));
							myRandAward.RandNumMinus = myRandAward.RandEndNum - myRandAward.RandBeginNum + 1;
							string goodsIDs = Global.GetSafeAttributeStr(xmlRandAward, "Goods");
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型周末充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								myRandAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日周末充值配置1");
							}
							myRandAwardList.Add(myRandAward);
						}
						this.AwardItemDict[id] = myRandAwardList;
					}
				}
				this.ActivityType = 27;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "ZhouMoChongZhiType.xml|ZhouMoChongZhi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		protected Dictionary<int, WeekEndInputTypeData> InputTypeDict = new Dictionary<int, WeekEndInputTypeData>();

		
		protected Dictionary<int, List<WeekEndInputAwardData>> AwardItemDict = new Dictionary<int, List<WeekEndInputAwardData>>();
	}
}
