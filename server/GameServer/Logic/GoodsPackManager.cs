using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006E8 RID: 1768
	public class GoodsPackManager
	{
		// Token: 0x06002A5B RID: 10843 RVA: 0x0025AE2C File Offset: 0x0025902C
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x0025AE54 File Offset: 0x00259054
		public int GetNextGoodsID()
		{
			return (int)(Interlocked.Increment(ref this.BaseGoodsID) & 2147483647L);
		}

		// Token: 0x06002A5D RID: 10845 RVA: 0x0025AE7C File Offset: 0x0025907C
		public int GetNextRoleGoodsPackID()
		{
			return (int)(Interlocked.Increment(ref this.BaseRoleGoodsPackID) & 2147483647L);
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x0025AEA4 File Offset: 0x002590A4
		private void InitGlobalFallGoodsLimitDict()
		{
			lock (this._GlobalFallGoodsLimitDict)
			{
				if (this._GlobalFallGoodsLimitDict.Count <= 0)
				{
					XElement xmlFile = ConfigHelper.Load(Global.GameResPath("Config/EraDropLimit.xml"));
					if (null != xmlFile)
					{
						IEnumerable<XElement> xmlItems = xmlFile.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							int goodsPackID = (int)Global.GetSafeAttributeLong(xmlItem, "DropID");
							int dropLimit = (int)Global.GetSafeAttributeLong(xmlItem, "DropLimit");
							this._GlobalFallGoodsLimitDict[goodsPackID] = dropLimit;
						}
					}
				}
			}
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x0025AFA4 File Offset: 0x002591A4
		private int GetGlobalFallGoodsLimitNum(int goodsPackID)
		{
			int limitNum = 0;
			lock (this._GlobalFallGoodsLimitDict)
			{
				this.InitGlobalFallGoodsLimitDict();
				this._GlobalFallGoodsLimitDict.TryGetValue(goodsPackID, out limitNum);
			}
			return limitNum;
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x0025B008 File Offset: 0x00259208
		private bool JudgeModifyGlobalFallGoodsLimit(int goodsPackID)
		{
			int limitNum = this.GetGlobalFallGoodsLimitNum(goodsPackID);
			bool result;
			if (limitNum <= 0)
			{
				result = true;
			}
			else
			{
				lock (this.GlobalFallGoodsNumDict)
				{
					int curDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
					if (this.GlobalFallGoodsLimitDayID != curDayID)
					{
						this.GlobalFallGoodsNumDict.Clear();
						this.GlobalFallGoodsLimitDayID = curDayID;
					}
					int curNum = 0;
					this.GlobalFallGoodsNumDict.TryGetValue(goodsPackID, out curNum);
					if (curNum >= limitNum)
					{
						return false;
					}
					curNum = (this.GlobalFallGoodsNumDict[goodsPackID] = curNum + 1);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x0025B0D4 File Offset: 0x002592D4
		private List<FallGoodsItem> GetNormalFallGoodsItem(int goodsPackID)
		{
			List<FallGoodsItem> fallGoodsItemList = null;
			lock (this._FallGoodsItemsDict)
			{
				this._FallGoodsItemsDict.TryGetValue(goodsPackID, out fallGoodsItemList);
			}
			List<FallGoodsItem> result;
			if (null != fallGoodsItemList)
			{
				result = fallGoodsItemList;
			}
			else
			{
				SystemXmlItem monsterGoodsItem = null;
				if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
				{
					result = null;
				}
				else
				{
					FallGoodsItem fallGoodsItem = null;
					fallGoodsItemList = new List<FallGoodsItem>();
					string goodsData = monsterGoodsItem.GetStringValue("GoodsID");
					string[] goodsFields = goodsData.Split(new char[]
					{
						'|'
					});
					int basePercent = 0;
					for (int i = 0; i < goodsFields.Length; i++)
					{
						string item = goodsFields[i].Trim();
						if (!(item == ""))
						{
							string[] itemFields = item.Split(new char[]
							{
								','
							});
							if (itemFields.Length == 7)
							{
								fallGoodsItem = null;
								try
								{
									fallGoodsItem = new FallGoodsItem
									{
										GoodsID = Convert.ToInt32(itemFields[0]),
										BasePercent = basePercent,
										SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
										Binding = Convert.ToInt32(itemFields[2]),
										LuckyRate = (int)Convert.ToDouble(itemFields[3]),
										FallLevelID = Convert.ToInt32(itemFields[4]),
										ZhuiJiaID = Convert.ToInt32(itemFields[5]),
										ExcellencePropertyID = Convert.ToInt32(itemFields[6])
									};
									basePercent += fallGoodsItem.SelfPercent;
								}
								catch (Exception)
								{
									fallGoodsItem = null;
								}
								if (null == fallGoodsItem)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, item), null, true);
								}
								else
								{
									fallGoodsItemList.Add(fallGoodsItem);
								}
							}
						}
					}
					if (basePercent > 100000)
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("解析掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
					}
					lock (this._FallGoodsItemsDict)
					{
						this._FallGoodsItemsDict[goodsPackID] = fallGoodsItemList;
					}
					result = fallGoodsItemList;
				}
			}
			return result;
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x0025B374 File Offset: 0x00259574
		private List<FallGoodsItem> ParseGoodsDataList(int goodsPackID, string goodsData)
		{
			List<FallGoodsItem> fallGoodsItemList = new List<FallGoodsItem>();
			string[] goodsFields = goodsData.Split(new char[]
			{
				'|'
			});
			FallGoodsItem fallGoodsItem = null;
			int basePercent = 0;
			for (int i = 0; i < goodsFields.Length; i++)
			{
				string item = goodsFields[i].Trim();
				if (!(item == ""))
				{
					string[] itemFields = item.Split(new char[]
					{
						','
					});
					if (itemFields.Length == 7)
					{
						fallGoodsItem = null;
						try
						{
							fallGoodsItem = new FallGoodsItem
							{
								GoodsID = Convert.ToInt32(itemFields[0]),
								BasePercent = basePercent,
								SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
								Binding = Convert.ToInt32(itemFields[2]),
								LuckyRate = (int)Convert.ToDouble(itemFields[3]),
								FallLevelID = Convert.ToInt32(itemFields[4]),
								ZhuiJiaID = Convert.ToInt32(itemFields[5]),
								ExcellencePropertyID = Convert.ToInt32(itemFields[6])
							};
							basePercent += fallGoodsItem.SelfPercent;
						}
						catch (Exception)
						{
							fallGoodsItem = null;
						}
						if (null == fallGoodsItem)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, item), null, true);
						}
						else
						{
							fallGoodsItemList.Add(fallGoodsItem);
						}
					}
				}
			}
			if (basePercent > 100000)
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("解析掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
			}
			return fallGoodsItemList;
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x0025B538 File Offset: 0x00259738
		public List<GoodsData> GetFixedGoodsDataList(List<FallGoodsItem> fixedFallGoodsItemList, int count)
		{
			List<GoodsData> result;
			if (null == fixedFallGoodsItemList)
			{
				result = null;
			}
			else if (count <= 0)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fixedList = new List<FallGoodsItem>();
				fixedList.AddRange(fixedFallGoodsItemList);
				List<GoodsData> goodsDataList = new List<GoodsData>();
				for (int i = 0; i < count; i++)
				{
					if (fixedFallGoodsItemList.Count <= 0)
					{
						break;
					}
					int baseOdds = 0;
					foreach (FallGoodsItem item in fixedList)
					{
						baseOdds += item.SelfPercent;
					}
					int rand = Global.GetRandomNumber(0, baseOdds);
					foreach (FallGoodsItem item in fixedList)
					{
						if (rand < item.SelfPercent)
						{
							int goodsQualtiy = 0;
							int goodsLevel = this.GetFallGoodsLevel(item.FallLevelID);
							int goodsBornIndex = 0;
							int nLuckyProp = 0;
							int luckyRate = this.GetLuckyGoodsID(item.LuckyRate);
							if (luckyRate > 0)
							{
								int nValue = GameManager.GoodsPackMgr.GetLuckyGoodsID(luckyRate);
								if (nValue >= 1)
								{
									nLuckyProp = 1;
								}
							}
							int appendPropLev = this.GetZhuiJiaGoodsLevelID(item.ZhuiJiaID);
							int GoodsExcellenceList = this.GetExcellencePropertysID(item.GoodsID, item.ExcellencePropertyID);
							string props = "";
							GoodsData goodsData = new GoodsData
							{
								Id = this.GetNextGoodsID(),
								GoodsID = item.GoodsID,
								Forge_level = goodsLevel,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Quality = goodsQualtiy,
								Props = props,
								GCount = 1,
								Binding = item.Binding,
								BornIndex = goodsBornIndex,
								Lucky = nLuckyProp,
								AppendPropLev = appendPropLev,
								ExcellenceInfo = GoodsExcellenceList
							};
							fixedList.Remove(item);
							goodsDataList.Add(goodsData);
							break;
						}
						rand -= item.SelfPercent;
					}
				}
				result = goodsDataList;
			}
			return result;
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x0025B7C0 File Offset: 0x002599C0
		public void ResetLimitTimeRange()
		{
			this._LimitTimeStartDayTime = new DateTime(2000, 1, 1);
			this._LimitTimeEndDayTime = new DateTime(2000, 1, 1);
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x0025B7E8 File Offset: 0x002599E8
		private bool JugeInLimitTimeRange()
		{
			if (2000 == this._LimitTimeStartDayTime.Year)
			{
				this._LimitTimeStartDayTime = Global.GetJieriStartDay();
			}
			if (2000 == this._LimitTimeEndDayTime.Year)
			{
				this._LimitTimeEndDayTime = Global.GetAddDaysDataTime(Global.GetJieriStartDay(), Math.Max(0, Global.GetJieriDaysNum()), true);
			}
			DateTime today = TimeUtil.NowDateTime();
			return today.Ticks >= this._LimitTimeStartDayTime.Ticks && today.Ticks < this._LimitTimeEndDayTime.Ticks;
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x0025B898 File Offset: 0x00259A98
		private List<FallGoodsItem> GetLimitTimeFallGoodsItem(int goodsPackID)
		{
			List<FallGoodsItem> result;
			if (!this.JugeInLimitTimeRange())
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = null;
				lock (this._LimitTimeFallGoodsItemsDict)
				{
					this._LimitTimeFallGoodsItemsDict.TryGetValue(goodsPackID, out fallGoodsItemList);
				}
				if (null != fallGoodsItemList)
				{
					result = fallGoodsItemList;
				}
				else
				{
					SystemXmlItem monsterGoodsItem = null;
					if (!GameManager.SystemLimitTimeMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
					{
						result = null;
					}
					else
					{
						FallGoodsItem fallGoodsItem = null;
						fallGoodsItemList = new List<FallGoodsItem>();
						string goodsData = monsterGoodsItem.GetStringValue("GoodsID");
						string[] goodsFields = goodsData.Split(new char[]
						{
							'|'
						});
						int basePercent = 0;
						for (int i = 0; i < goodsFields.Length; i++)
						{
							string item = goodsFields[i].Trim();
							if (!(item == ""))
							{
								string[] itemFields = item.Split(new char[]
								{
									','
								});
								if (itemFields.Length == 7)
								{
									fallGoodsItem = null;
									try
									{
										fallGoodsItem = new FallGoodsItem
										{
											GoodsID = Convert.ToInt32(itemFields[0]),
											BasePercent = basePercent,
											SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
											Binding = Convert.ToInt32(itemFields[2]),
											LuckyRate = (int)Convert.ToDouble(itemFields[3]),
											FallLevelID = Convert.ToInt32(itemFields[4]),
											ZhuiJiaID = Convert.ToInt32(itemFields[5]),
											ExcellencePropertyID = Convert.ToInt32(itemFields[6])
										};
										basePercent += fallGoodsItem.SelfPercent;
									}
									catch (Exception)
									{
										fallGoodsItem = null;
									}
									if (null == fallGoodsItem)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("解析节日掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, item), null, true);
									}
									else
									{
										fallGoodsItemList.Add(fallGoodsItem);
									}
								}
							}
						}
						if (basePercent > 100000)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析节日掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
						}
						lock (this._LimitTimeFallGoodsItemsDict)
						{
							this._LimitTimeFallGoodsItemsDict[goodsPackID] = fallGoodsItemList;
						}
						result = fallGoodsItemList;
					}
				}
			}
			return result;
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x0025BB4C File Offset: 0x00259D4C
		public List<FallGoodsItem> GetFixedFallGoodsItemList(int goodsPackID)
		{
			List<FallGoodsItem> fixedFallGoodsItemList = null;
			lock (this._FixedGoodsItemsDict)
			{
				this._FixedGoodsItemsDict.TryGetValue(goodsPackID, out fixedFallGoodsItemList);
			}
			List<FallGoodsItem> result;
			if (null != fixedFallGoodsItemList)
			{
				result = fixedFallGoodsItemList;
			}
			else
			{
				SystemXmlItem monsterGoodsItem = null;
				if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
				{
					result = null;
				}
				else
				{
					string goodsData = monsterGoodsItem.GetStringValue("Fixedaward");
					fixedFallGoodsItemList = this.ParseGoodsDataList(goodsPackID, goodsData);
					lock (this._FixedGoodsItemsDict)
					{
						this._FixedGoodsItemsDict[goodsPackID] = fixedFallGoodsItemList;
					}
					result = fixedFallGoodsItemList;
				}
			}
			return result;
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x0025BC3C File Offset: 0x00259E3C
		public int GetFallGoodsMaxCount(int goodsPackID)
		{
			Tuple<int, int> maxCount = null;
			lock (this._FallGoodsMaxCountDict)
			{
				if (this._FallGoodsMaxCountDict.TryGetValue(goodsPackID, out maxCount))
				{
					return maxCount.Item2;
				}
			}
			SystemXmlItem monsterGoodsItem = null;
			int result;
			if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
			{
				result = -1;
			}
			else
			{
				string strMaxList = monsterGoodsItem.GetStringValue("MaxList");
				string[] fields = strMaxList.Split(new char[]
				{
					','
				});
				if (fields.Length != 2)
				{
					maxCount = new Tuple<int, int>(0, 0);
				}
				else
				{
					maxCount = new Tuple<int, int>(Global.SafeConvertToInt32(fields[0]), Global.SafeConvertToInt32(fields[1]));
				}
				lock (this._FallGoodsMaxCountDict)
				{
					this._FallGoodsMaxCountDict[goodsPackID] = maxCount;
				}
				result = maxCount.Item2;
			}
			return result;
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x0025BD78 File Offset: 0x00259F78
		public int GetFixedFallGoodsMaxCount(int goodsPackID)
		{
			Tuple<int, int> maxCount = null;
			lock (this._FallGoodsMaxCountDict)
			{
				if (this._FallGoodsMaxCountDict.TryGetValue(goodsPackID, out maxCount))
				{
					return maxCount.Item1;
				}
			}
			SystemXmlItem monsterGoodsItem = null;
			int result;
			if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
			{
				result = -1;
			}
			else
			{
				string strMaxList = monsterGoodsItem.GetStringValue("MaxList");
				string[] fields = strMaxList.Split(new char[]
				{
					','
				});
				maxCount = new Tuple<int, int>(Global.SafeConvertToInt32(fields[0]), Global.SafeConvertToInt32(fields[1]));
				lock (this._FallGoodsMaxCountDict)
				{
					this._FallGoodsMaxCountDict[goodsPackID] = maxCount;
				}
				result = maxCount.Item1;
			}
			return result;
		}

		// Token: 0x06002A6A RID: 10858 RVA: 0x0025BE98 File Offset: 0x0025A098
		private int GetLimitTimeFallGoodsMaxCount(int goodsPackID)
		{
			int maxCount = -1;
			lock (this._LimitTimeFallGoodsMaxCountDict)
			{
				if (this._LimitTimeFallGoodsMaxCountDict.TryGetValue(goodsPackID, out maxCount))
				{
					return maxCount;
				}
			}
			SystemXmlItem monsterGoodsItem = null;
			int result;
			if (!GameManager.SystemLimitTimeMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out monsterGoodsItem))
			{
				result = -1;
			}
			else
			{
				maxCount = monsterGoodsItem.GetIntValue("MaxList", -1);
				lock (this._LimitTimeFallGoodsMaxCountDict)
				{
					this._LimitTimeFallGoodsMaxCountDict[goodsPackID] = maxCount;
				}
				result = maxCount;
			}
			return result;
		}

		// Token: 0x06002A6B RID: 10859 RVA: 0x0025BF80 File Offset: 0x0025A180
		private FallQualityItem GetFallQualityItem(int fallQualityID)
		{
			FallQualityItem fallQualityItem = null;
			lock (this._FallGoodsQualityDict)
			{
				this._FallGoodsQualityDict.TryGetValue(fallQualityID, out fallQualityItem);
			}
			FallQualityItem result;
			if (null != fallQualityItem)
			{
				result = fallQualityItem;
			}
			else
			{
				SystemXmlItem goodsQualityItem = null;
				if (!GameManager.SystemGoodsQuality.SystemXmlItemDict.TryGetValue(fallQualityID, out goodsQualityItem))
				{
					result = null;
				}
				else
				{
					fallQualityItem = new FallQualityItem
					{
						ID = fallQualityID,
						QualityBasePercent = new double[5],
						QualitySelfPercent = new double[5]
					};
					string quality = goodsQualityItem.GetStringValue("Quality");
					if (!string.IsNullOrEmpty(quality))
					{
						string[] sa = quality.Split(new char[]
						{
							'|'
						});
						if (sa.Length == 5)
						{
							fallQualityItem.QualitySelfPercent = Global.StringArray2DoubleArray(sa);
							double basePercent = 0.0;
							for (int i = 0; i < fallQualityItem.QualitySelfPercent.Length; i++)
							{
								fallQualityItem.QualityBasePercent[i] = basePercent;
								basePercent += fallQualityItem.QualitySelfPercent[i];
							}
							if (basePercent > 1.0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项的品质掉落概率溢出1.0错误, fallQualityID={0}", fallQualityID), null, true);
							}
						}
					}
					lock (this._FallGoodsQualityDict)
					{
						this._FallGoodsQualityDict[fallQualityID] = fallQualityItem;
					}
					result = fallQualityItem;
				}
			}
			return result;
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x0025C158 File Offset: 0x0025A358
		private FallLevelItem GetFallLevelItem(int fallLevelID)
		{
			FallLevelItem fallLevelItem = null;
			lock (this._FallGoodsLevelDict)
			{
				this._FallGoodsLevelDict.TryGetValue(fallLevelID, out fallLevelItem);
			}
			FallLevelItem result;
			if (null != fallLevelItem)
			{
				result = fallLevelItem;
			}
			else
			{
				SystemXmlItem goodsLevelItem = null;
				if (!GameManager.SystemGoodsLevel.SystemXmlItemDict.TryGetValue(fallLevelID, out goodsLevelItem))
				{
					result = null;
				}
				else
				{
					fallLevelItem = new FallLevelItem
					{
						ID = fallLevelID,
						LevelBasePercent = new double[21],
						LevelSelfPercent = new double[21]
					};
					string level = goodsLevelItem.GetStringValue("Level");
					if (!string.IsNullOrEmpty(level))
					{
						string[] sa = level.Split(new char[]
						{
							'|'
						});
						if (sa.Length == 21)
						{
							fallLevelItem.LevelSelfPercent = Global.StringArray2DoubleArray(sa);
							double basePercent = 0.0;
							for (int i = 0; i < fallLevelItem.LevelSelfPercent.Length; i++)
							{
								fallLevelItem.LevelBasePercent[i] = basePercent;
								basePercent += fallLevelItem.LevelSelfPercent[i];
							}
							if (basePercent > 1.0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项的级别掉落概率溢出1.0错误, fallLevelID={0}", fallLevelID), null, true);
							}
						}
					}
					lock (this._FallGoodsLevelDict)
					{
						this._FallGoodsLevelDict[fallLevelID] = fallLevelItem;
					}
					result = fallLevelItem;
				}
			}
			return result;
		}

		// Token: 0x06002A6D RID: 10861 RVA: 0x0025C334 File Offset: 0x0025A534
		private FallBornIndexItem GetFallBornIndexItem(int fallBornIndexID)
		{
			FallBornIndexItem fallBornIndexItem = null;
			lock (this._FallGoodsBornIndexDict)
			{
				this._FallGoodsBornIndexDict.TryGetValue(fallBornIndexID, out fallBornIndexItem);
			}
			FallBornIndexItem result;
			if (null != fallBornIndexItem)
			{
				result = fallBornIndexItem;
			}
			else
			{
				SystemXmlItem goodsBornIndexItem = null;
				if (!GameManager.SystemGoodsBornIndex.SystemXmlItemDict.TryGetValue(fallBornIndexID, out goodsBornIndexItem))
				{
					result = null;
				}
				else
				{
					fallBornIndexItem = new FallBornIndexItem
					{
						ID = fallBornIndexID,
						LevelBasePercent = new double[12],
						LevelSelfPercent = new double[12]
					};
					string born = goodsBornIndexItem.GetStringValue("Born");
					if (!string.IsNullOrEmpty(born))
					{
						string[] sa = born.Split(new char[]
						{
							'|'
						});
						if (sa.Length == 12)
						{
							fallBornIndexItem.LevelSelfPercent = Global.StringArray2DoubleArray(sa);
							double basePercent = 0.0;
							for (int i = 0; i < fallBornIndexItem.LevelSelfPercent.Length; i++)
							{
								fallBornIndexItem.LevelBasePercent[i] = basePercent;
								basePercent += fallBornIndexItem.LevelSelfPercent[i];
							}
							if (basePercent > 1.0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项的天生掉落概率溢出1.0错误, fallBornIndexID={0}", fallBornIndexID), null, true);
							}
						}
					}
					lock (this._FallGoodsBornIndexDict)
					{
						this._FallGoodsBornIndexDict[fallBornIndexID] = fallBornIndexItem;
					}
					result = fallBornIndexItem;
				}
			}
			return result;
		}

		// Token: 0x06002A6E RID: 10862 RVA: 0x0025C510 File Offset: 0x0025A710
		private ZhuiJiaIDItem GetZhuiJiaIDItem(int zhuiJiaID)
		{
			ZhuiJiaIDItem zhuiJiaIDItem = null;
			lock (this._ZhuiJiaIDDict)
			{
				this._ZhuiJiaIDDict.TryGetValue(zhuiJiaID, out zhuiJiaIDItem);
			}
			ZhuiJiaIDItem result;
			if (null != zhuiJiaIDItem)
			{
				result = zhuiJiaIDItem;
			}
			else
			{
				SystemXmlItem goodsZhuiJiaItem = null;
				if (!GameManager.SystemGoodsZhuiJia.SystemXmlItemDict.TryGetValue(zhuiJiaID, out goodsZhuiJiaItem))
				{
					result = null;
				}
				else
				{
					zhuiJiaIDItem = new ZhuiJiaIDItem
					{
						ID = zhuiJiaID,
						LevelBasePercent = new double[11],
						LevelSelfPercent = new double[11]
					};
					string level = goodsZhuiJiaItem.GetStringValue("ZhuiJiaLevel");
					if (!string.IsNullOrEmpty(level))
					{
						string[] sa = level.Split(new char[]
						{
							'|'
						});
						if (sa.Length == 21)
						{
							zhuiJiaIDItem.LevelSelfPercent = Global.StringArray2DoubleArray(sa);
							double basePercent = 0.0;
							for (int i = 0; i < zhuiJiaIDItem.LevelSelfPercent.Length; i++)
							{
								zhuiJiaIDItem.LevelBasePercent[i] = basePercent;
								basePercent += zhuiJiaIDItem.LevelSelfPercent[i];
							}
							if (basePercent > 1.0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("解析掉落项的级别追加概率溢出1.0错误, zhuiJiaID={0}", zhuiJiaID), null, true);
							}
						}
					}
					lock (this._ZhuiJiaIDDict)
					{
						this._ZhuiJiaIDDict[zhuiJiaID] = zhuiJiaIDItem;
					}
					result = zhuiJiaIDItem;
				}
			}
			return result;
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x0025C6EC File Offset: 0x0025A8EC
		public ExcellencePropertyGroupItem GetExcellencePropertyGroupItem(int excellencePropertyGroupID)
		{
			ExcellencePropertyGroupItem excellencePropertyGroupItem = null;
			lock (this._ExcellencePropertyGroupItemDict)
			{
				this._ExcellencePropertyGroupItemDict.TryGetValue(excellencePropertyGroupID, out excellencePropertyGroupItem);
			}
			ExcellencePropertyGroupItem result;
			if (null != excellencePropertyGroupItem)
			{
				result = excellencePropertyGroupItem;
			}
			else
			{
				SystemXmlItem goodsExcellencePropertyItem = null;
				if (!GameManager.SystemGoodsExcellenceProperty.SystemXmlItemDict.TryGetValue(excellencePropertyGroupID, out goodsExcellencePropertyItem))
				{
					result = null;
				}
				else
				{
					excellencePropertyGroupItem = new ExcellencePropertyGroupItem
					{
						ID = excellencePropertyGroupID,
						Max = goodsExcellencePropertyItem.GetIntArrayValue("MAX", ','),
						ExcellencePropertyItems = this.ParseExcellencePropertyItems(goodsExcellencePropertyItem)
					};
					lock (this._ExcellencePropertyGroupItemDict)
					{
						this._ExcellencePropertyGroupItemDict[excellencePropertyGroupID] = excellencePropertyGroupItem;
					}
					result = excellencePropertyGroupItem;
				}
			}
			return result;
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x0025C7FC File Offset: 0x0025A9FC
		private ExcellencePropertyItem[] ParseExcellencePropertyItems(SystemXmlItem goodsExcellencePropertyItem)
		{
			ExcellencePropertyItem[] excellencePropertyItems = null;
			int nBase = 0;
			string property = goodsExcellencePropertyItem.GetStringValue("ExcellenceProperty");
			if (!string.IsNullOrEmpty(property))
			{
				string[] sa = property.Split(new char[]
				{
					'|'
				});
				if (sa != null && sa.Length > 0)
				{
					excellencePropertyItems = new ExcellencePropertyItem[sa.Length];
					for (int i = 0; i < sa.Length; i++)
					{
						string[] fields = sa[i].Split(new char[]
						{
							','
						});
						if (2 == fields.Length)
						{
							excellencePropertyItems[i] = new ExcellencePropertyItem
							{
								Num = Global.SafeConvertToInt32(fields[0]),
								BasePercent = nBase,
								SelfPercent = (int)(Global.SafeConvertToDouble(fields[1]) * 100000.0)
							};
							nBase += excellencePropertyItems[i].SelfPercent;
						}
					}
				}
			}
			return excellencePropertyItems;
		}

		// Token: 0x06002A71 RID: 10865 RVA: 0x0025C90C File Offset: 0x0025AB0C
		public int ResetCachingItems()
		{
			int ret = GameManager.SystemMonsterGoodsList.ReloadLoadFromXMlFile();
			lock (this._FallGoodsItemsDict)
			{
				this._FallGoodsItemsDict.Clear();
			}
			ret = GameManager.SystemLimitTimeMonsterGoodsList.ReloadLoadFromXMlFile();
			lock (this._LimitTimeFallGoodsItemsDict)
			{
				this._LimitTimeFallGoodsItemsDict.Clear();
			}
			lock (this._FixedGoodsItemsDict)
			{
				this._FixedGoodsItemsDict.Clear();
			}
			lock (this._FallGoodsMaxCountDict)
			{
				this._FallGoodsMaxCountDict.Clear();
			}
			lock (this._LimitTimeFallGoodsMaxCountDict)
			{
				this._LimitTimeFallGoodsMaxCountDict.Clear();
			}
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				ret = GameManager.SystemGoodsQuality.ReloadLoadFromXMlFile();
				lock (this._FallGoodsQualityDict)
				{
					this._FallGoodsQualityDict.Clear();
				}
				if (ret < 0)
				{
					result = ret;
				}
				else
				{
					ret = GameManager.SystemGoodsLevel.ReloadLoadFromXMlFile();
					lock (this._FallGoodsLevelDict)
					{
						this._FallGoodsLevelDict.Clear();
					}
					if (ret < 0)
					{
						result = ret;
					}
					else
					{
						ret = GameManager.SystemGoodsBornIndex.ReloadLoadFromXMlFile();
						lock (this._FallGoodsBornIndexDict)
						{
							this._FallGoodsBornIndexDict.Clear();
						}
						if (ret < 0)
						{
							result = ret;
						}
						else
						{
							ret = GameManager.SystemGoodsZhuiJia.ReloadLoadFromXMlFile();
							lock (this._ZhuiJiaIDDict)
							{
								this._ZhuiJiaIDDict.Clear();
							}
							if (ret < 0)
							{
								result = ret;
							}
							else
							{
								ret = GameManager.SystemGoodsExcellenceProperty.ReloadLoadFromXMlFile();
								lock (this._ExcellencePropertyGroupItemDict)
								{
									this._ExcellencePropertyGroupItemDict.Clear();
								}
								if (ret < 0)
								{
									result = ret;
								}
								else
								{
									lock (this._CacheShiQuGoodsDict)
									{
										this._CacheShiQuGoodsDict.Clear();
									}
									lock (this._GlobalFallGoodsLimitDict)
									{
										this._GlobalFallGoodsLimitDict.Clear();
									}
									result = ret;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06002A72 RID: 10866 RVA: 0x0025CD24 File Offset: 0x0025AF24
		// (set) Token: 0x06002A73 RID: 10867 RVA: 0x0025CD3C File Offset: 0x0025AF3C
		public Dictionary<int, GoodsPackItem> GoodsPackDict
		{
			get
			{
				return this._GoodsPackDict;
			}
			set
			{
				this._GoodsPackDict = value;
			}
		}

		// Token: 0x06002A74 RID: 10868 RVA: 0x0025CD48 File Offset: 0x0025AF48
		private int GetFallGoodsQuality(int fallQualityID)
		{
			int goodsQuality = 0;
			int result;
			if (-1 == fallQualityID)
			{
				result = goodsQuality;
			}
			else
			{
				FallQualityItem fallQualityItem = this.GetFallQualityItem(fallQualityID);
				if (null == fallQualityItem)
				{
					result = goodsQuality;
				}
				else
				{
					int rndPercent = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < fallQualityItem.QualitySelfPercent.Length; i++)
					{
						int basePercent = (int)(fallQualityItem.QualityBasePercent[i] * 100000.0);
						int percent = (int)(fallQualityItem.QualitySelfPercent[i] * 100000.0);
						if (rndPercent > basePercent && rndPercent <= basePercent + percent)
						{
							goodsQuality = i;
							break;
						}
					}
					result = goodsQuality;
				}
			}
			return result;
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x0025CE00 File Offset: 0x0025B000
		public int GetFallGoodsLevel(int fallLevelID)
		{
			int goodsLevel = 0;
			int result;
			if (-1 == fallLevelID)
			{
				result = goodsLevel;
			}
			else
			{
				FallLevelItem fallLevelItem = this.GetFallLevelItem(fallLevelID);
				if (null == fallLevelItem)
				{
					result = goodsLevel;
				}
				else
				{
					int rndPercent = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < fallLevelItem.LevelSelfPercent.Length; i++)
					{
						int basePercent = (int)(fallLevelItem.LevelBasePercent[i] * 100000.0);
						int percent = (int)(fallLevelItem.LevelSelfPercent[i] * 100000.0);
						if (rndPercent > basePercent && rndPercent <= basePercent + percent)
						{
							goodsLevel = i;
							break;
						}
					}
					result = goodsLevel;
				}
			}
			return result;
		}

		// Token: 0x06002A76 RID: 10870 RVA: 0x0025CEB8 File Offset: 0x0025B0B8
		private int GetFallGoodsBornIndex(IObject attacker, int fallBornIndexID, int goodsID)
		{
			int goodsBornIndex = 0;
			int result;
			if (!(attacker is GameClient))
			{
				result = goodsBornIndex;
			}
			else if (!DBRoleBufferManager.ProcessFallTianSheng(attacker as GameClient))
			{
				result = goodsBornIndex;
			}
			else
			{
				goodsBornIndex = Global.GetBornIndexOnFallGoods(goodsID);
				result = goodsBornIndex;
			}
			return result;
		}

		// Token: 0x06002A77 RID: 10871 RVA: 0x0025CEFC File Offset: 0x0025B0FC
		public int GetLuckyGoodsID(int luckyPercent)
		{
			int rndPercent = Global.GetRandomNumber(1, 100001);
			int result;
			if (rndPercent <= luckyPercent * 100000)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06002A78 RID: 10872 RVA: 0x0025CF30 File Offset: 0x0025B130
		public int GetZhuiJiaGoodsLevelID(int zhuiJiaID)
		{
			int appendPropLev = 0;
			int result;
			if (-1 == zhuiJiaID)
			{
				result = appendPropLev;
			}
			else
			{
				ZhuiJiaIDItem zhuiJiaIDItem = this.GetZhuiJiaIDItem(zhuiJiaID);
				if (null == zhuiJiaIDItem)
				{
					result = appendPropLev;
				}
				else
				{
					int rndPercent = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < zhuiJiaIDItem.LevelSelfPercent.Length; i++)
					{
						int basePercent = (int)(zhuiJiaIDItem.LevelBasePercent[i] * 100000.0);
						int percent = (int)(zhuiJiaIDItem.LevelSelfPercent[i] * 100000.0);
						if (rndPercent > basePercent && rndPercent <= basePercent + percent)
						{
							appendPropLev = i;
							break;
						}
					}
					result = appendPropLev;
				}
			}
			return result;
		}

		// Token: 0x06002A79 RID: 10873 RVA: 0x0025CFE8 File Offset: 0x0025B1E8
		public int GetExcellencePropertysID(int GoodsID, int excellencePropertyGroupID)
		{
			int result;
			if (ZuoQiManager.CheckIsZuoQiByGoodsID(GoodsID))
			{
				result = excellencePropertyGroupID;
			}
			else if (RebornEquip.IsRebornEquip(GoodsID))
			{
				result = excellencePropertyGroupID;
			}
			else
			{
				int excellencePropertyID = 0;
				if (-1 == excellencePropertyGroupID)
				{
					result = excellencePropertyID;
				}
				else
				{
					ExcellencePropertyGroupItem excellencePropertyGroupItem = this.GetExcellencePropertyGroupItem(excellencePropertyGroupID);
					if (excellencePropertyGroupItem == null || excellencePropertyGroupItem.ExcellencePropertyItems == null || excellencePropertyGroupItem.Max == null || excellencePropertyGroupItem.Max.Length <= 0)
					{
						result = excellencePropertyID;
					}
					else
					{
						List<int> idList = new List<int>();
						int nNum = 0;
						int rndPercent = Global.GetRandomNumber(1, 100001);
						int i;
						for (i = 0; i < excellencePropertyGroupItem.ExcellencePropertyItems.Length; i++)
						{
							if (rndPercent > excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent && rndPercent <= excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent + excellencePropertyGroupItem.ExcellencePropertyItems[i].SelfPercent)
							{
								nNum = excellencePropertyGroupItem.ExcellencePropertyItems[i].Num;
								break;
							}
						}
						if (nNum > 0 && nNum <= excellencePropertyGroupItem.Max.Length)
						{
							int nCount = 0;
							do
							{
								int nProp = Global.GetRandomNumber(0, excellencePropertyGroupItem.Max.Length);
								if (idList.IndexOf(nProp) < 0)
								{
									idList.Add(nProp);
									nCount++;
								}
							}
							while (nCount != nNum);
						}
						i = 0;
						while (i < idList.Count && i < excellencePropertyGroupItem.Max.Length)
						{
							excellencePropertyID |= 1 << excellencePropertyGroupItem.Max[idList[i]];
							i++;
						}
						result = excellencePropertyID;
					}
				}
			}
			return result;
		}

		// Token: 0x06002A7A RID: 10874 RVA: 0x0025D1B4 File Offset: 0x0025B3B4
		public List<FallGoodsItem> GetFallGoodsItemByPercent(List<FallGoodsItem> gallGoodsItemList, int maxFallCount, int fallAlgorithm, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> result;
			if (null == gallGoodsItemList)
			{
				result = gallGoodsItemList;
			}
			else if (gallGoodsItemList.Count <= 0)
			{
				result = gallGoodsItemList;
			}
			else
			{
				List<FallGoodsItem> goodsItemList = new List<FallGoodsItem>();
				if (0 == fallAlgorithm)
				{
					bool hasRobotDropRate = robotDropRate < 1.0;
					for (int i = 0; i < gallGoodsItemList.Count; i++)
					{
						int itemDropRate = gallGoodsItemList[i].SelfPercent;
						if (hasRobotDropRate)
						{
							double rate = (double)itemDropRate * robotDropRate;
							itemDropRate = (int)rate;
						}
						int randPercent = Global.GetRandomNumber(1, 100001);
						if (randPercent <= itemDropRate)
						{
							goodsItemList.Add(gallGoodsItemList[i]);
						}
					}
					if (goodsItemList.Count > maxFallCount)
					{
						goodsItemList = Global.RandomSortList<FallGoodsItem>(goodsItemList);
						goodsItemList = goodsItemList.GetRange(0, maxFallCount);
					}
				}
				else
				{
					for (int i = 0; i < maxFallCount; i++)
					{
						int randPercent = Global.GetRandomNumber(1, 100001);
						FallGoodsItem fallGoodsItem = this.PickUpGoodsItemByPercent(gallGoodsItemList, randPercent);
						if (null != fallGoodsItem)
						{
							goodsItemList.Add(fallGoodsItem);
						}
					}
				}
				result = goodsItemList;
			}
			return result;
		}

		// Token: 0x06002A7B RID: 10875 RVA: 0x0025D2F4 File Offset: 0x0025B4F4
		private FallGoodsItem PickUpGoodsItemByPercent(List<FallGoodsItem> gallGoodsItemList, int randPercent)
		{
			FallGoodsItem fallGoodsItem = null;
			for (int i = 0; i < gallGoodsItemList.Count; i++)
			{
				if (randPercent > gallGoodsItemList[i].BasePercent && randPercent <= gallGoodsItemList[i].BasePercent + gallGoodsItemList[i].SelfPercent)
				{
					fallGoodsItem = gallGoodsItemList[i];
					break;
				}
			}
			return fallGoodsItem;
		}

		// Token: 0x06002A7C RID: 10876 RVA: 0x0025D380 File Offset: 0x0025B580
		public List<GoodsData> GetGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> fallGoodsItemList = this.GetNormalFallGoodsItem(goodsPackID);
			List<GoodsData> result;
			if (null == fallGoodsItemList)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fixedFixedFallGoodsItemList = this.GetFixedFallGoodsItemList(goodsPackID);
				List<GoodsData> fixedGoodsDataList = this.GetFixedGoodsDataList(fixedFixedFallGoodsItemList, this.GetFixedFallGoodsMaxCount(goodsPackID));
				List<FallGoodsItem> tempItemList2 = this.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 0, robotDropRate);
				if (tempItemList2.Count <= 0)
				{
					if (fixedGoodsDataList == null || fixedGoodsDataList.Count <= 0)
					{
						return null;
					}
				}
				else
				{
					tempItemList2.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
				}
				List<GoodsData> goodsDataList = new List<GoodsData>();
				if (fixedGoodsDataList != null && fixedGoodsDataList.Count > 0)
				{
					int toAddNum = GoodsPackManager.MaxFallCount - tempItemList2.Count;
					if (toAddNum > 0)
					{
						toAddNum = Global.GMin(toAddNum, fixedGoodsDataList.Count);
						for (int i = 0; i < toAddNum; i++)
						{
							GoodsData goodData = fixedGoodsDataList[i];
							goodData.Id = this.GetNextGoodsID();
							goodData.Binding = Math.Max(goodData.Binding, forceBinding);
							goodsDataList.Add(goodData);
						}
					}
				}
				for (int i = 0; i < tempItemList2.Count; i++)
				{
					int goodsQualtiy = 0;
					int goodsLevel = this.GetFallGoodsLevel(tempItemList2[i].FallLevelID);
					int goodsBornIndex = 0;
					int nLuckyProp = 0;
					int luckyRate = this.GetLuckyGoodsID(tempItemList2[i].LuckyRate);
					if (luckyRate > 0)
					{
						int nValue = GameManager.GoodsPackMgr.GetLuckyGoodsID(luckyRate);
						if (nValue >= 1)
						{
							nLuckyProp = 1;
						}
					}
					int appendPropLev = this.GetZhuiJiaGoodsLevelID(tempItemList2[i].ZhuiJiaID);
					int excellenceInfo = this.GetExcellencePropertysID(tempItemList2[i].GoodsID, tempItemList2[i].ExcellencePropertyID);
					string props = "";
					GoodsData goodsData = new GoodsData
					{
						Id = this.GetNextGoodsID(),
						GoodsID = tempItemList2[i].GoodsID,
						Using = 0,
						Forge_level = goodsLevel,
						Starttime = "1900-01-01 12:00:00",
						Endtime = "1900-01-01 12:00:00",
						Site = 0,
						Quality = goodsQualtiy,
						Props = props,
						GCount = 1,
						Binding = Math.Max(tempItemList2[i].Binding, forceBinding),
						Jewellist = "",
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = goodsBornIndex,
						Lucky = nLuckyProp,
						Strong = 0,
						ExcellenceInfo = excellenceInfo,
						AppendPropLev = appendPropLev,
						ChangeLifeLevForEquip = 0
					};
					goodsDataList.Add(goodsData);
				}
				result = goodsDataList;
			}
			return result;
		}

		// Token: 0x06002A7D RID: 10877 RVA: 0x0025D6AC File Offset: 0x0025B8AC
		private List<GoodsData> GetEraGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<GoodsData> result;
			if (!(attacker is GameClient))
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = EraManager.getInstance().GetEraFallGoodsItem(attacker as GameClient, goodsPackID);
				if (null == fallGoodsItemList)
				{
					result = null;
				}
				else
				{
					List<FallGoodsItem> tempItemList2 = this.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 0, robotDropRate);
					if (tempItemList2.Count <= 0)
					{
						result = null;
					}
					else
					{
						tempItemList2.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
						List<GoodsData> goodsDataList = new List<GoodsData>();
						for (int i = 0; i < tempItemList2.Count; i++)
						{
							int goodsQualtiy = 0;
							int goodsLevel = this.GetFallGoodsLevel(tempItemList2[i].FallLevelID);
							int goodsBornIndex = 0;
							int luckyRate = this.GetLuckyGoodsID(tempItemList2[i].LuckyRate);
							int appendPropLev = this.GetZhuiJiaGoodsLevelID(tempItemList2[i].ZhuiJiaID);
							int excellenceInfo = this.GetExcellencePropertysID(tempItemList2[i].GoodsID, tempItemList2[i].ExcellencePropertyID);
							string props = "";
							GoodsData goodsData = new GoodsData
							{
								Id = this.GetNextGoodsID(),
								GoodsID = tempItemList2[i].GoodsID,
								Using = 0,
								Forge_level = goodsLevel,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Site = 0,
								Quality = goodsQualtiy,
								Props = props,
								GCount = 1,
								Binding = Math.Max(tempItemList2[i].Binding, forceBinding),
								Jewellist = "",
								BagIndex = 0,
								AddPropIndex = 0,
								BornIndex = goodsBornIndex,
								Lucky = luckyRate,
								Strong = 0,
								ExcellenceInfo = excellenceInfo,
								AppendPropLev = appendPropLev,
								ChangeLifeLevForEquip = 0
							};
							goodsDataList.Add(goodsData);
						}
						if (goodsDataList.Count != 0)
						{
							if (!this.JudgeModifyGlobalFallGoodsLimit(goodsPackID))
							{
								return null;
							}
						}
						result = goodsDataList;
					}
				}
			}
			return result;
		}

		// Token: 0x06002A7E RID: 10878 RVA: 0x0025D908 File Offset: 0x0025BB08
		private List<GoodsData> GetLimitTimeGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> fallGoodsItemList = this.GetLimitTimeFallGoodsItem(goodsPackID);
			List<GoodsData> result;
			if (null == fallGoodsItemList)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> tempItemList2 = this.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 0, robotDropRate);
				if (tempItemList2.Count <= 0)
				{
					result = null;
				}
				else
				{
					tempItemList2.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
					List<GoodsData> goodsDataList = new List<GoodsData>();
					for (int i = 0; i < tempItemList2.Count; i++)
					{
						int goodsQualtiy = 0;
						int goodsLevel = this.GetFallGoodsLevel(tempItemList2[i].FallLevelID);
						int goodsBornIndex = 0;
						int luckyRate = this.GetLuckyGoodsID(tempItemList2[i].LuckyRate);
						int appendPropLev = this.GetZhuiJiaGoodsLevelID(tempItemList2[i].ZhuiJiaID);
						int excellenceInfo = this.GetExcellencePropertysID(tempItemList2[i].GoodsID, tempItemList2[i].ExcellencePropertyID);
						string props = "";
						GoodsData goodsData = new GoodsData
						{
							Id = this.GetNextGoodsID(),
							GoodsID = tempItemList2[i].GoodsID,
							Using = 0,
							Forge_level = goodsLevel,
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							Quality = goodsQualtiy,
							Props = props,
							GCount = 1,
							Binding = Math.Max(tempItemList2[i].Binding, forceBinding),
							Jewellist = "",
							BagIndex = 0,
							AddPropIndex = 0,
							BornIndex = goodsBornIndex,
							Lucky = luckyRate,
							Strong = 0,
							ExcellenceInfo = excellenceInfo,
							AppendPropLev = appendPropLev,
							ChangeLifeLevForEquip = 0
						};
						goodsDataList.Add(goodsData);
					}
					result = goodsDataList;
				}
			}
			return result;
		}

		// Token: 0x06002A7F RID: 10879 RVA: 0x0025DB00 File Offset: 0x0025BD00
		private bool JugeFuBenMapFall(MapGrid mapGrid, int copyMapID, int newGridX, int newGridY)
		{
			bool result;
			if (copyMapID <= 0)
			{
				result = false;
			}
			else
			{
				bool canFall = true;
				List<object> objsList = mapGrid.FindObjects(newGridX, newGridY);
				if (null != objsList)
				{
					for (int objIndex = 0; objIndex < objsList.Count; objIndex++)
					{
						if (objsList[objIndex] is GoodsPackItem)
						{
							if ((objsList[objIndex] as GoodsPackItem).CopyMapID == copyMapID)
							{
								canFall = false;
								break;
							}
						}
					}
				}
				result = canFall;
			}
			return result;
		}

		// Token: 0x06002A80 RID: 10880 RVA: 0x0025DB90 File Offset: 0x0025BD90
		private bool JugeTaskTargetFall(MapGrid mapGrid, int copyMapID, int newGridX, int newGridY, GoodsPackItem goodsPackData)
		{
			bool canFall = true;
			List<object> objsList = mapGrid.FindObjects(newGridX, newGridY);
			if (null != objsList)
			{
				for (int objIndex = 0; objIndex < objsList.Count; objIndex++)
				{
					if (objsList[objIndex] is GoodsPackItem)
					{
						if ((objsList[objIndex] as GoodsPackItem).CopyMapID == copyMapID)
						{
							if ((objsList[objIndex] as GoodsPackItem).OnlyID <= 0 || (objsList[objIndex] as GoodsPackItem).OnlyID == goodsPackData.OnlyID)
							{
								canFall = false;
								break;
							}
						}
					}
				}
			}
			return canFall;
		}

		// Token: 0x06002A81 RID: 10881 RVA: 0x0025DC50 File Offset: 0x0025BE50
		private Point FindABlankPoint(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			Point fallPoint = new Point(centerPoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), centerPoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			int gridX = (int)centerPoint.X;
			int gridY = (int)centerPoint.Y;
			for (int circleNum = 1; circleNum <= 5; circleNum++)
			{
				for (int x = gridX - circleNum; x <= gridX + circleNum; x++)
				{
					int newGridX = x;
					int newGridY = gridY - circleNum;
					string key = string.Format("{0}_{1}", newGridX, newGridY);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
						{
							if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
						}
					}
				}
				for (int x = gridX - circleNum; x <= gridX + circleNum; x++)
				{
					int newGridX = x;
					int newGridY = gridY + circleNum;
					string key = string.Format("{0}_{1}", newGridX, newGridY);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
						{
							if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
						}
					}
				}
				for (int y = gridY - circleNum + 1; y <= gridY + circleNum - 1; y++)
				{
					int newGridY = y;
					int newGridX = gridX - circleNum;
					string key = string.Format("{0}_{1}", newGridX, newGridY);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
						{
							if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
						}
					}
				}
				for (int y = gridY - circleNum + 1; y <= gridY + circleNum - 1; y++)
				{
					int newGridY = y;
					int newGridX = gridX + circleNum;
					string key = string.Format("{0}_{1}", newGridX, newGridY);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
						{
							if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								dict[key] = true;
								fallPoint = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return fallPoint;
							}
						}
					}
				}
			}
			return fallPoint;
		}

		// Token: 0x06002A82 RID: 10882 RVA: 0x0025E198 File Offset: 0x0025C398
		private Point FindABlankPointEx(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID, IObject attacker)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			Point fallPoint = new Point(centerPoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), centerPoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			int centerCridX = (int)centerPoint.X;
			int centerCridY = (int)centerPoint.Y;
			int i = 0;
			while (i < 200)
			{
				int newGridX = (int)Global.ClientViewGridArray[i] + centerCridX;
				int newGridY = (int)Global.ClientViewGridArray[i + 1] + centerCridY;
				if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
				{
					string key = string.Format("{0}_{1}", newGridX, newGridY);
					if (!dict.ContainsKey(key))
					{
						Point result;
						if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
						{
							dict[key] = true;
							result = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
						}
						else
						{
							if (!this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								goto IL_181;
							}
							dict[key] = true;
							result = new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
						}
						return result;
					}
				}
				IL_182:
				i += 2;
				continue;
				IL_181:
				goto IL_182;
			}
			if (null != attacker)
			{
				fallPoint = attacker.CurrentPos;
				centerCridX = (int)fallPoint.X;
				centerCridY = (int)fallPoint.Y;
				i = 0;
				while (i < 200)
				{
					int newGridX = (int)Global.ClientViewGridArray[i] + centerCridX;
					int newGridY = (int)Global.ClientViewGridArray[i + 1] + centerCridY;
					if (!Global.InOnlyObs(objType, mapCode, newGridX, newGridY))
					{
						string key = string.Format("{0}_{1}", newGridX, newGridY);
						if (!dict.ContainsKey(key))
						{
							if (mapGrid.CanMove(objType, newGridX, newGridY, 0, 0))
							{
								dict[key] = true;
								return new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, newGridX, newGridY))
							{
								dict[key] = true;
								return new Point((double)(newGridX * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(newGridY * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
							}
						}
					}
					IL_2D0:
					i += 2;
					continue;
					goto IL_2D0;
				}
			}
			return fallPoint;
		}

		// Token: 0x06002A83 RID: 10883 RVA: 0x0025E498 File Offset: 0x0025C698
		public Point GetFallGoodsPosition(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID, IObject attacker)
		{
			return this.FindABlankPointEx(objType, mapCode, dict, centerPoint, copyMapID, attacker);
		}

		// Token: 0x06002A84 RID: 10884 RVA: 0x0025E4BC File Offset: 0x0025C6BC
		private GoodsPackItem GetMonsterGoodsPackItem(GameClient client, int ownerRoleID, string ownerRoleName, int goodsPackID, List<int> teamRoleIDs, int mapCode, int copyMapID, int toX, int toY, int forceBinding, string monsterName, int belongTo, int fallLevel, int teamID)
		{
			int maxFallCountByID = this.GetFallGoodsMaxCount(goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> goodsDataList = this.GetGoodsDataList(client, goodsPackID, maxFallCountByID, forceBinding, 1.0);
			maxFallCountByID = this.GetLimitTimeFallGoodsMaxCount(goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> goodsDataListLimitTime = this.GetLimitTimeGoodsDataList(client, goodsPackID, maxFallCountByID, forceBinding, 1.0);
			maxFallCountByID = EraManager.getInstance().GetEraFallGoodsMaxCount(client, goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> goodsDataListEra = this.GetEraGoodsDataList(client, goodsPackID, maxFallCountByID, forceBinding, 1.0);
			GoodsPackItem result;
			if (goodsDataList == null && goodsDataListLimitTime == null && null == goodsDataListEra)
			{
				result = null;
			}
			else
			{
				if (null == goodsDataList)
				{
					goodsDataList = new List<GoodsData>();
				}
				if (null != goodsDataListLimitTime)
				{
					goodsDataList.AddRange(goodsDataListLimitTime);
				}
				if (null != goodsDataListEra)
				{
					goodsDataList.AddRange(goodsDataListEra);
				}
				GoodsPackItem goodsPackItem = new GoodsPackItem
				{
					AutoID = this.GetNextAutoID(),
					GoodsPackID = goodsPackID,
					OwnerRoleID = ownerRoleID,
					OwnerRoleName = ownerRoleName,
					GoodsPackType = 0,
					ProduceTicks = TimeUtil.NOW(),
					LockedRoleID = -1,
					GoodsDataList = goodsDataList,
					TeamRoleIDs = teamRoleIDs,
					MapCode = mapCode,
					FallPoint = new Point((double)toX, (double)toY),
					CopyMapID = copyMapID,
					KilledMonsterName = monsterName,
					BelongTo = belongTo,
					FallLevel = fallLevel,
					TeamID = teamID
				};
				lock (this._GoodsPackDict)
				{
					this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
				}
				result = goodsPackItem;
			}
			return result;
		}

		// Token: 0x06002A85 RID: 10885 RVA: 0x0025E6C0 File Offset: 0x0025C8C0
		public List<GoodsPackItem> GetMonsterGoodsPackItemList(IObject attacker, int ownerRoleID, string ownerRoleName, int goodsPackID, List<int> teamRoleIDs, int mapCode, int copyMapID, int toX, int toY, int forceBinding, string monsterName, int belongTo, int fallLevel, int teamID, int monsterType = -1, List<long> teamRoleDamages = null)
		{
			int maxFallCountByID = this.GetFallGoodsMaxCount(goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			double dropRate = 1.0;
			if (ownerRoleID > 0)
			{
				dropRate = RobotTaskValidator.getInstance().GetRobotSceneDropRate(attacker as GameClient, mapCode, dropRate, monsterType);
			}
			List<GoodsData> goodsDataList = this.GetGoodsDataList(attacker, goodsPackID, maxFallCountByID, forceBinding, dropRate);
			maxFallCountByID = this.GetLimitTimeFallGoodsMaxCount(goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> goodsDataListLimitTime = this.GetLimitTimeGoodsDataList(attacker, goodsPackID, maxFallCountByID, forceBinding, dropRate);
			maxFallCountByID = EraManager.getInstance().GetEraFallGoodsMaxCount(attacker, goodsPackID);
			if (maxFallCountByID <= 0)
			{
				maxFallCountByID = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> goodsDataListEra = this.GetEraGoodsDataList(attacker, goodsPackID, maxFallCountByID, forceBinding, 1.0);
			List<GoodsPackItem> result;
			if (goodsDataList == null && goodsDataListLimitTime == null && null == goodsDataListEra)
			{
				result = null;
			}
			else
			{
				if (null == goodsDataList)
				{
					goodsDataList = new List<GoodsData>();
				}
				if (null != goodsDataListLimitTime)
				{
					goodsDataList.AddRange(goodsDataListLimitTime);
				}
				if (null != goodsDataListEra)
				{
					goodsDataList.AddRange(goodsDataListEra);
				}
				Dictionary<string, bool> gridDict = new Dictionary<string, bool>();
				List<GoodsPackItem> goodsPackItemList = new List<GoodsPackItem>();
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					List<GoodsData> oneGoodsDataList = new List<GoodsData>();
					oneGoodsDataList.Add(goodsDataList[i]);
					GoodsPackItem goodsPackItem = new GoodsPackItem
					{
						AutoID = this.GetNextAutoID(),
						GoodsPackID = goodsPackID,
						OwnerRoleID = ownerRoleID,
						OwnerRoleName = ownerRoleName,
						GoodsPackType = 0,
						ProduceTicks = TimeUtil.NOW(),
						LockedRoleID = -1,
						GoodsDataList = oneGoodsDataList,
						TeamRoleIDs = teamRoleIDs,
						MapCode = mapCode,
						CopyMapID = copyMapID,
						KilledMonsterName = monsterName,
						BelongTo = belongTo,
						FallLevel = fallLevel,
						TeamID = teamID,
						TeamRoleDamages = teamRoleDamages
					};
					goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, gridDict, new Point((double)toX, (double)toY), copyMapID, attacker);
					goodsPackItemList.Add(goodsPackItem);
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
					}
				}
				result = goodsPackItemList;
			}
			return result;
		}

		// Token: 0x06002A86 RID: 10886 RVA: 0x0025E954 File Offset: 0x0025CB54
		private GoodsPackItem GetRoleGoodsPackItem(int ownerRoleID, string ownerRoleName, int goodsPackID, List<GoodsData> goodsDataList, int mapCode, int copyMapID, int toGridX, int toGridY, string fromRoleName)
		{
			GoodsPackItem goodsPackItem = new GoodsPackItem
			{
				AutoID = this.GetNextAutoID(),
				GoodsPackID = goodsPackID,
				OwnerRoleID = ownerRoleID,
				OwnerRoleName = ownerRoleName,
				GoodsPackType = 0,
				ProduceTicks = TimeUtil.NOW(),
				LockedRoleID = -1,
				GoodsDataList = goodsDataList,
				TeamRoleIDs = null,
				MapCode = mapCode,
				CopyMapID = copyMapID,
				KilledMonsterName = fromRoleName,
				BelongTo = -1,
				FallLevel = 0,
				TeamID = -1
			};
			Dictionary<string, bool> gridDict = new Dictionary<string, bool>();
			goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, gridDict, new Point((double)toGridX, (double)toGridY), copyMapID, null);
			lock (this._GoodsPackDict)
			{
				this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
			}
			return goodsPackItem;
		}

		// Token: 0x06002A87 RID: 10887 RVA: 0x0025EA60 File Offset: 0x0025CC60
		public List<GoodsPackItem> GetRoleGoodsPackItemList(int ownerRoleID, string ownerRoleName, List<GoodsData> goodsDataList, int mapCode, int copyMapID, int toGridX, int toGridY, string fromRoleName)
		{
			Dictionary<string, bool> gridDict = new Dictionary<string, bool>();
			List<GoodsPackItem> goodsPackItemList = new List<GoodsPackItem>();
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				List<GoodsData> oneGoodsDataList = new List<GoodsData>();
				oneGoodsDataList.Add(goodsDataList[i]);
				GoodsPackItem goodsPackItem = new GoodsPackItem
				{
					AutoID = this.GetNextAutoID(),
					GoodsPackID = this.GetNextRoleGoodsPackID(),
					OwnerRoleID = ownerRoleID,
					OwnerRoleName = ownerRoleName,
					GoodsPackType = 0,
					ProduceTicks = TimeUtil.NOW(),
					LockedRoleID = -1,
					GoodsDataList = oneGoodsDataList,
					TeamRoleIDs = null,
					MapCode = mapCode,
					CopyMapID = copyMapID,
					KilledMonsterName = fromRoleName,
					BelongTo = -1,
					FallLevel = 0,
					TeamID = -1
				};
				goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, gridDict, new Point((double)toGridX, (double)toGridY), copyMapID, null);
				goodsPackItemList.Add(goodsPackItem);
				lock (this._GoodsPackDict)
				{
					this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
				}
			}
			return goodsPackItemList;
		}

		// Token: 0x06002A88 RID: 10888 RVA: 0x0025EBCC File Offset: 0x0025CDCC
		private GoodsPackItem GetBattleGoodsPackItem(int ownerRoleID, string ownerRoleName, int goodsPackID, List<GoodsData> awardsGoodsDataList, List<GoodsData> giveGoodsDataList, int mapCode, int copyMapID, int toX, int toY)
		{
			List<GoodsData> packGoodsDataList = new List<GoodsData>();
			if (null != awardsGoodsDataList)
			{
				for (int i = 0; i < awardsGoodsDataList.Count; i++)
				{
					packGoodsDataList.Add(awardsGoodsDataList[i]);
				}
			}
			for (int i = 0; i < giveGoodsDataList.Count; i++)
			{
				packGoodsDataList.Add(new GoodsData
				{
					Id = this.GetNextGoodsID(),
					GoodsID = giveGoodsDataList[i].GoodsID,
					Using = giveGoodsDataList[i].Using,
					Forge_level = giveGoodsDataList[i].Forge_level,
					Starttime = giveGoodsDataList[i].Starttime,
					Endtime = giveGoodsDataList[i].Endtime,
					Site = giveGoodsDataList[i].Site,
					Quality = giveGoodsDataList[i].Quality,
					Props = giveGoodsDataList[i].Props,
					GCount = giveGoodsDataList[i].GCount,
					Binding = giveGoodsDataList[i].Binding,
					Jewellist = giveGoodsDataList[i].Jewellist,
					BagIndex = 0,
					AddPropIndex = 0,
					BornIndex = 0,
					Lucky = 0,
					Strong = 0,
					ExcellenceInfo = 0,
					AppendPropLev = 0,
					ChangeLifeLevForEquip = 0
				});
			}
			GoodsPackItem goodsPackItem = new GoodsPackItem
			{
				AutoID = this.GetNextAutoID(),
				GoodsPackID = goodsPackID,
				OwnerRoleID = ownerRoleID,
				OwnerRoleName = ownerRoleName,
				GoodsPackType = 0,
				ProduceTicks = TimeUtil.NOW(),
				LockedRoleID = -1,
				GoodsDataList = packGoodsDataList,
				TeamRoleIDs = null,
				MapCode = mapCode,
				FallPoint = new Point((double)toX, (double)toY),
				CopyMapID = copyMapID,
				KilledMonsterName = "",
				BelongTo = -1,
				FallLevel = 0,
				TeamID = -1
			};
			lock (this._GoodsPackDict)
			{
				this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
			}
			return goodsPackItem;
		}

		// Token: 0x06002A89 RID: 10889 RVA: 0x0025EE5C File Offset: 0x0025D05C
		private int FindGoodsID2RoleID(GoodsPackItem goodsPackItem, int goodsDbID)
		{
			int roleID = -1;
			if (null != goodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (!goodsPackItem.GoodsIDToRolesDict.TryGetValue(goodsDbID, out roleID))
						{
							roleID = -1;
						}
					}
				}
			}
			return roleID;
		}

		// Token: 0x06002A8A RID: 10890 RVA: 0x0025EEDC File Offset: 0x0025D0DC
		private void AddGoodsID2RoleID(GoodsPackItem goodsPackItem, int goodsDbID, int roleID)
		{
			if (null != goodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					goodsPackItem.GoodsIDToRolesDict[goodsDbID] = roleID;
				}
			}
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x0025EF3C File Offset: 0x0025D13C
		private void SendRandMessage(GameClient[] clientsArray, string msgText)
		{
			foreach (GameClient gc in clientsArray)
			{
				if (null != gc)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyChatBox, 200);
				}
			}
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x0025EF98 File Offset: 0x0025D198
		private void JugeGoodsID2RoleID(GameClient client, GoodsPackItem goodsPackItem, int goodsDbID, int goodsID)
		{
			int MaxRandNum = -1;
			GameClient toClient = null;
			string goodsName = Global.GetGoodsNameByID(goodsID);
			GameClient[] clientsArray = new GameClient[goodsPackItem.TeamRoleIDs.Count];
			int[] RandNumArray = new int[goodsPackItem.TeamRoleIDs.Count];
			for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
			{
				GameClient gc = GameManager.ClientMgr.FindClient(goodsPackItem.TeamRoleIDs[i]);
				if (null == gc)
				{
					clientsArray[i] = null;
					RandNumArray[i] = -1;
				}
				else
				{
					int randNum = Global.GetRandomNumber(1, 101);
					clientsArray[i] = gc;
					RandNumArray[i] = randNum;
					if (randNum > MaxRandNum)
					{
						MaxRandNum = randNum;
						toClient = gc;
					}
				}
			}
			for (int i = 0; i < clientsArray.Length; i++)
			{
				GameClient gc = clientsArray[i];
				if (null != gc)
				{
					this.SendRandMessage(clientsArray, StringUtil.substitute(GLang.GetLang(379, new object[0]), new object[]
					{
						Global.FormatRoleName(gc, gc.ClientData.RoleName),
						goodsName,
						RandNumArray[i]
					}));
				}
			}
			if (null != toClient)
			{
				this.AddGoodsID2RoleID(goodsPackItem, goodsDbID, toClient.ClientData.RoleID);
				this.SendRandMessage(clientsArray, StringUtil.substitute(GLang.GetLang(380, new object[0]), new object[]
				{
					Global.FormatRoleName(toClient, toClient.ClientData.RoleName),
					goodsName
				}));
			}
		}

		// Token: 0x06002A8D RID: 10893 RVA: 0x0025F13C File Offset: 0x0025D33C
		private void JugeGoodsID2RoleIDByDamageRandom(GameClient client, GoodsPackItem goodsPackItem, int goodsDbID, int goodsID)
		{
			GameClient toClient = null;
			string goodsName = Global.GetGoodsNameByID(goodsID);
			List<GameClient> clientsArray = new List<GameClient>(goodsPackItem.TeamRoleIDs.Count);
			List<long> RandNumArray = new List<long>(goodsPackItem.TeamRoleIDs.Count);
			for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
			{
				GameClient gc = GameManager.ClientMgr.FindClient(goodsPackItem.TeamRoleIDs[i]);
				if (null != gc)
				{
					clientsArray.Add(gc);
					RandNumArray.Add(goodsPackItem.TeamRoleDamages[i]);
					toClient = gc;
				}
			}
			long totalDamage = RandNumArray.Sum();
			long randNum = (long)((double)totalDamage * Global.GetRandom());
			for (int i = 0; i < clientsArray.Count; i++)
			{
				if (randNum <= RandNumArray[i])
				{
					toClient = clientsArray[i];
					break;
				}
				randNum -= RandNumArray[i];
			}
			if (null != toClient)
			{
				this.AddGoodsID2RoleID(goodsPackItem, goodsDbID, toClient.ClientData.RoleID);
				this.SendRandMessage(clientsArray.ToArray(), GLang.GetLang(688, new object[]
				{
					Global.FormatRoleName(toClient, toClient.ClientData.RoleName),
					goodsName
				}));
			}
		}

		// Token: 0x06002A8E RID: 10894 RVA: 0x0025F2A4 File Offset: 0x0025D4A4
		public GoodsPackItem FindGoodsPackItem(int autoID)
		{
			GoodsPackItem goodsPackItem = null;
			lock (this._GoodsPackDict)
			{
				if (!this._GoodsPackDict.TryGetValue(autoID, out goodsPackItem))
				{
					return null;
				}
			}
			return goodsPackItem;
		}

		// Token: 0x06002A8F RID: 10895 RVA: 0x0025F30C File Offset: 0x0025D50C
		public static bool IsFallTongQianGoods(int goodsID)
		{
			int fallTongQianGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("FallTongQianGoodsID", -1);
			return fallTongQianGoodsID == goodsID;
		}

		// Token: 0x06002A90 RID: 10896 RVA: 0x0025F340 File Offset: 0x0025D540
		private static bool ProcessFallTongQian(TCPOutPacketPool pool, GameClient client, int goodsID, int goodsNum, int fallLevel)
		{
			bool result;
			if (!GoodsPackManager.IsFallTongQianGoods(goodsID))
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemDropMoneyItem = null;
				if (!GameManager.SystemDropMoney.SystemXmlItemDict.TryGetValue(fallLevel, out systemDropMoneyItem))
				{
					result = true;
				}
				else
				{
					int minMoney = systemDropMoneyItem.GetIntValue("MinMoney", -1);
					int maxMoney = systemDropMoneyItem.GetIntValue("MaxMoney", -1);
					for (int i = 0; i < goodsNum; i++)
					{
						int money = Global.GetRandomNumber(minMoney, maxMoney);
						money = Global.FilterValue(client, money);
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, pool, client, money, "拾取金币", false);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002A91 RID: 10897 RVA: 0x0025F3F8 File Offset: 0x0025D5F8
		public bool AutoAddThingIntoBag(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsPackItem goodsPackItem, GoodsData goodsData)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				GameClient gc = null;
				int toRoleID = this.FindGoodsID2RoleID(goodsPackItem, goodsData.Id);
				if (-1 == toRoleID)
				{
					gc = client;
				}
				else
				{
					gc = GameManager.ClientMgr.FindClient(toRoleID);
				}
				if (null == gc)
				{
					result = false;
				}
				else if (Global.CanAddGoods(gc, goodsData.GoodsID, 1, goodsData.Binding, "1900-01-01 12:00:00", true, false) || GoodsPackManager.IsFallTongQianGoods(goodsData.GoodsID))
				{
					lock (this._GoodsPackDict)
					{
						goodsPackItem.GoodsIDDict[goodsData.Id] = true;
					}
					if (!GoodsPackManager.ProcessFallTongQian(pool, gc, goodsData.GoodsID, goodsData.GCount, goodsPackItem.FallLevel))
					{
						int nRet = Global.AddGoodsDBCommand_Hook(pool, gc, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "杀怪掉落后自动拾取", true, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
						if (0 == nRet)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, Global.ModifyGoodsLogName(goodsData), "杀怪掉落后自动拾取", Global.GetMapName(client.ClientData.MapCode), "系统", "销毁", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsData);
						}
					}
					GameManager.ClientMgr.NotifySelfGetThing(sl, pool, gc, goodsData.Id);
					SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PickUpEquipCount);
					evObj.Arg1 = goodsData.GoodsID;
					evObj.Arg2 = goodsData.GCount;
					GlobalEventSource.getInstance().fireEvent(evObj);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06002A92 RID: 10898 RVA: 0x0025F654 File Offset: 0x0025D854
		private bool CanAutoFightGetThings(GameClient client, GoodsPackItem goodsPackItem, GoodsData goodsData)
		{
			bool result;
			if (!client.ClientData.AutoFighting)
			{
				result = false;
			}
			else if (client.ClientData.AutoFightGetThings <= 0)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods) || null == systemGoods)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("处理自动挂机拾取物品到背包时，获取物品xml信息失败: GoodsID={0}", goodsData.GoodsID), null, true);
					result = false;
				}
				else
				{
					int categoriy = systemGoods.GetIntValue("Categoriy", -1);
					if (501 == categoriy)
					{
						if (4 != ((byte)client.ClientData.AutoFightGetThings & 4))
						{
							return false;
						}
					}
					else if (180 == categoriy)
					{
						if (8 != ((byte)client.ClientData.AutoFightGetThings & 8))
						{
							return false;
						}
					}
					else if (2 != ((byte)client.ClientData.AutoFightGetThings & 2))
					{
						return false;
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002A93 RID: 10899 RVA: 0x0025F76C File Offset: 0x0025D96C
		public bool AutoGetThings(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsPackItem goodsPackItem)
		{
			if (client.ClientData.AutoFighting)
			{
				if (goodsPackItem.TeamRoleIDs != null)
				{
					return false;
				}
			}
			bool result;
			if (goodsPackItem.GoodsDataList == null || goodsPackItem.GoodsDataList.Count <= 0)
			{
				result = true;
			}
			else
			{
				int getThingCount = 0;
				for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
				{
					if (this.CanAutoFightGetThings(client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
					{
						if (this.AutoAddThingIntoBag(sl, pool, client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
						{
							getThingCount++;
						}
					}
					else if (Data.AutoGetThing > 0)
					{
						if (this.AutoAddThingIntoBag(sl, pool, client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
						{
							getThingCount++;
						}
					}
				}
				result = (getThingCount >= goodsPackItem.GoodsDataList.Count);
			}
			return result;
		}

		// Token: 0x06002A94 RID: 10900 RVA: 0x0025F890 File Offset: 0x0025DA90
		public string FormatTeamRoleIDs(GoodsPackItem goodsPackItem)
		{
			string teamRoleIDs = "";
			string result;
			if (null == goodsPackItem)
			{
				result = teamRoleIDs;
			}
			else
			{
				if (goodsPackItem.TeamRoleIDs != null && goodsPackItem.TeamRoleIDs.Count > 0)
				{
					for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
					{
						if (teamRoleIDs.Length > 0)
						{
							teamRoleIDs += ",";
						}
						teamRoleIDs += goodsPackItem.TeamRoleIDs[i].ToString();
					}
				}
				result = teamRoleIDs;
			}
			return result;
		}

		// Token: 0x06002A95 RID: 10901 RVA: 0x0025F934 File Offset: 0x0025DB34
		public List<GoodsPackItem> ProcessMonster(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster)
		{
			List<GoodsPackItem> result;
			if (attacker is GameClient)
			{
				result = this.ProcessMonsterByClient(sl, pool, attacker as GameClient, monster);
			}
			else
			{
				result = this.ProcessMonsterByMonster(sl, pool, attacker as Monster, monster);
			}
			return result;
		}

		// Token: 0x06002A96 RID: 10902 RVA: 0x0025F97C File Offset: 0x0025DB7C
		public List<GoodsPackItem> ProcessMonsterByClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster)
		{
			JunTuanManager.getInstance().AddJunTuanTaskValue(client, monster, 1, 1);
			RebornManager.getInstance().ProcessRebornMonsterFallGoods(client, monster);
			List<GoodsPackItem> result;
			if (!Global.FilterFallGoods(client))
			{
				result = null;
			}
			else if (monster.MonsterInfo.FallGoodsPackID < 0)
			{
				result = null;
			}
			else
			{
				bool isTeamSharingMap = true;
				if (monster.CurrentMapCode == GameManager.ArenaBattleMgr.BattleMapCode)
				{
					isTeamSharingMap = false;
				}
				GameClient otherClient = client;
				if (3 == monster.MonsterInfo.FallBelongTo && MoYuLongXue.InMoYuMap(monster.CurrentMapCode))
				{
					int onwerID = MoYuLongXue.KillerRid(monster);
					otherClient = GameManager.ClientMgr.FindClient(onwerID);
					if (null == otherClient)
					{
						RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, onwerID), 0);
						if (null == dbRd)
						{
							LogManager.WriteLog(LogTypes.Error, "MoYuLongXue :: 道具归属权，但是查不到角色数据。", null, true);
							otherClient = client;
						}
						otherClient = new GameClient
						{
							ClientData = new SafeClientData
							{
								RoleData = dbRd
							}
						};
					}
					isTeamSharingMap = false;
				}
				if (4 == monster.MonsterInfo.FallBelongTo && ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(monster.CurrentMapCode))
				{
					otherClient = GameManager.ClientMgr.FindClient(ZhuanShengShiLian.KillerRid(client, monster));
					if (null == otherClient)
					{
						otherClient = client;
					}
				}
				SceneUIClasses sceneType = Global.GetMapSceneType(monster.CurrentMapCode);
				if (5 == monster.MonsterInfo.FallBelongTo && sceneType == SceneUIClasses.Comp)
				{
					CompMapClientContextData clientContext = CompManager.getInstance().GetBossTopDamageClientContext(monster);
					if (null == clientContext)
					{
						otherClient = client;
					}
					else
					{
						otherClient = GameManager.ClientMgr.FindClient(clientContext.RoleId);
						if (null == otherClient)
						{
							RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, clientContext.RoleId), clientContext.ServerId);
							if (null == dbRd)
							{
								LogManager.WriteLog(LogTypes.Error, "CompBoss :: 道具归属权，但是查不到角色数据。", null, true);
								otherClient = client;
							}
							otherClient = new GameClient
							{
								ClientData = new SafeClientData
								{
									RoleData = dbRd
								}
							};
						}
					}
					isTeamSharingMap = false;
				}
				int teamID = -1;
				List<int> teamRoleIDs = null;
				List<long> teamRoleDamages = null;
				if (otherClient.ClientData.TeamID > 0 && isTeamSharingMap)
				{
					TeamData td = GameManager.TeamMgr.FindData(otherClient.ClientData.TeamID);
					if (td != null && td.GetThingOpt > 0)
					{
						lock (td)
						{
							teamID = td.TeamID;
							teamRoleIDs = new List<int>();
							long maxDamage = 0L;
							for (int i = 0; i < td.TeamRoles.Count; i++)
							{
								if (td.TeamRoles[i].RoleID == otherClient.ClientData.RoleID)
								{
									teamRoleIDs.Add(td.TeamRoles[i].RoleID);
								}
								else
								{
									GameClient gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
									if (null != gc)
									{
										if (gc.ClientData.MapCode == monster.CurrentMapCode)
										{
											if (gc.ClientData.CopyMapID == monster.CurrentCopyMapID)
											{
												if (Global.InCircle(new Point((double)gc.ClientData.PosX, (double)gc.ClientData.PosY), monster.SafeCoordinate, 800.0))
												{
													if (td.GetThingOpt == 2 && GoodsPackManager.TeamShareMode_MaxDamage)
													{
														long damage = monster.GetAttackerDamage(td.TeamRoles[i].RoleID);
														if (damage > maxDamage)
														{
															maxDamage = damage;
															otherClient = gc;
														}
													}
													else
													{
														teamRoleIDs.Add(td.TeamRoles[i].RoleID);
													}
												}
											}
										}
									}
								}
							}
							if (teamRoleIDs.Count <= 1)
							{
								teamRoleIDs = null;
							}
							else if (td.GetThingOpt == 3 && GoodsPackManager.TeamShareMode_RandomByDamage)
							{
								teamRoleDamages = monster.GetAttackerDamageList(teamRoleIDs);
							}
						}
					}
				}
				int forceBinding = -1;
				Point grid = monster.CurrentGrid;
				List<GoodsPackItem> goodsPackItemList = this.GetMonsterGoodsPackItemList(otherClient, otherClient.ClientData.RoleID, Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName), monster.MonsterInfo.FallGoodsPackID, teamRoleIDs, monster.CurrentMapCode, monster.CurrentCopyMapID, (int)grid.X, (int)grid.Y, forceBinding, monster.MonsterInfo.VSName, monster.MonsterInfo.FallBelongTo, monster.MonsterInfo.VLevel, teamID, monster.MonsterType, teamRoleDamages);
				if (goodsPackItemList == null || goodsPackItemList.Count <= 0)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < goodsPackItemList.Count; i++)
					{
						this.ProcessGoodsPackItem(otherClient, monster, goodsPackItemList[i], forceBinding);
						bool bNeedSend = true;
						if (monster.MonsterInfo.ExtensionID == 1800 || monster.MonsterInfo.ExtensionID == 1900 || monster.MonsterInfo.ExtensionID == 2900 || monster.MonsterInfo.ExtensionID == 3900 || monster.MonsterInfo.ExtensionID == 4900 || monster.MonsterInfo.ExtensionID == 5900 || monster.MonsterInfo.ExtensionID == 6900 || monster.MonsterInfo.ExtensionID == 7900 || monster.MonsterInfo.ExtensionID == 8900)
						{
							bNeedSend = false;
						}
						Global.BroadcastGetGoodsHint(otherClient, goodsPackItemList[i].GoodsDataList[0], monster.MonsterInfo.VSName, monster.CurrentMapCode, bNeedSend);
					}
					result = goodsPackItemList;
				}
			}
			return result;
		}

		// Token: 0x06002A97 RID: 10903 RVA: 0x0026006C File Offset: 0x0025E26C
		public List<GoodsPackItem> ProcessMonsterByMonster(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster monster)
		{
			List<GoodsPackItem> result;
			if (monster.MonsterInfo.FallGoodsPackID < 0)
			{
				result = null;
			}
			else
			{
				int forceBinding = 0;
				Point grid = monster.CurrentGrid;
				List<GoodsPackItem> goodsPackItemList = this.GetMonsterGoodsPackItemList(attacker, -1, "", monster.MonsterInfo.FallGoodsPackID, null, attacker.MonsterZoneNode.MapCode, attacker.CopyMapID, (int)grid.X, (int)grid.Y, forceBinding, monster.MonsterInfo.VSName, monster.MonsterInfo.FallBelongTo, monster.MonsterInfo.VLevel, -1, -1, null);
				if (goodsPackItemList == null || goodsPackItemList.Count <= 0)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < goodsPackItemList.Count; i++)
					{
						this.ProcessGoodsPackItem(attacker, monster, goodsPackItemList[i], forceBinding);
					}
					result = goodsPackItemList;
				}
			}
			return result;
		}

		// Token: 0x06002A98 RID: 10904 RVA: 0x00260158 File Offset: 0x0025E358
		public void ProcessTaskDropByTargetNum(GameClient client, string goodsData, Monster monster)
		{
			if (!string.IsNullOrEmpty(goodsData))
			{
				List<FallGoodsItem> list = this.ParseGoodsDataList(0, goodsData);
				list = this.GetFallGoodsItemByPercent(list, int.MaxValue, 0, 1.0);
				List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(list);
				if (null != goodsDataList)
				{
					Dictionary<string, bool> gridDict = new Dictionary<string, bool>();
					List<GoodsPackItem> goodsPackItemList = new List<GoodsPackItem>();
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						if (Global.IsRoleOccupationMatchGoods(client, goodsDataList[i].GoodsID))
						{
							List<GoodsData> oneGoodsDataList = new List<GoodsData>();
							oneGoodsDataList.Add(goodsDataList[i]);
							GoodsPackItem goodsPackItem = new GoodsPackItem
							{
								AutoID = GameManager.GoodsPackMgr.GetNextAutoID(),
								GoodsPackID = 0,
								OwnerRoleID = client.ClientData.RoleID,
								OwnerRoleName = client.ClientData.RoleName,
								GoodsPackType = 0,
								ProduceTicks = TimeUtil.NOW(),
								LockedRoleID = -1,
								GoodsDataList = oneGoodsDataList,
								TeamRoleIDs = null,
								MapCode = ((monster == null) ? client.ClientData.MapCode : monster.CurrentMapCode),
								CopyMapID = ((monster == null) ? client.ClientData.CopyMapID : monster.CurrentCopyMapID),
								KilledMonsterName = null,
								BelongTo = 1,
								FallLevel = 0,
								TeamID = -1,
								OnlyID = client.ClientData.RoleID
							};
							double X = (monster == null) ? client.CurrentGrid.X : monster.CurrentGrid.X;
							double Y = (monster == null) ? client.CurrentGrid.Y : monster.CurrentGrid.Y;
							goodsPackItem.FallPoint = GameManager.GoodsPackMgr.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, client.ClientData.MapCode, gridDict, new Point((double)((int)X), (double)((int)Y)), client.ClientData.CopyMapID, client);
							goodsPackItemList.Add(goodsPackItem);
							lock (GameManager.GoodsPackMgr.GoodsPackDict)
							{
								GameManager.GoodsPackMgr.GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
							}
						}
					}
					for (int j = 0; j < goodsPackItemList.Count; j++)
					{
						GameManager.GoodsPackMgr.ProcessGoodsPackItem(client, client, goodsPackItemList[j], 1);
					}
				}
			}
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x00260424 File Offset: 0x0025E624
		public void ProcessGoodsPackItem(IObject attacker, IObject obj, GoodsPackItem goodsPackItem, int forceBinding)
		{
			if (null != goodsPackItem)
			{
				GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].MoveObject(-1, -1, (int)goodsPackItem.FallPoint.X, (int)goodsPackItem.FallPoint.Y, goodsPackItem);
				this.WriteFallGoodsRecords(goodsPackItem);
			}
		}

		// Token: 0x06002A9A RID: 10906 RVA: 0x00260480 File Offset: 0x0025E680
		public void ProcessRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, string enemyName, out string strDropList)
		{
			strDropList = "";
			if (Global.CanMapLostEquip(client.ClientData.MapCode))
			{
				if (Global.FilterFallGoods(client))
				{
					if (null != otherClient.ClientData.GoodsDataList)
					{
						lock (otherClient.ClientData.GoodsDataList)
						{
							if (otherClient.ClientData.GoodsDataList.Count <= 0)
							{
								return;
							}
						}
						int maxFallUsingGoodsNum = 1;
						int maxmaxFallBagGoodsNum = 3;
						if (Global.IsRedName(otherClient))
						{
							int maxFallRoleBagRate = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MaxFallRedRoleBagRate", -1));
							int maxFallRoleUsingRate = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MaxFallRedRoleUsingRate", -1));
							maxFallRoleBagRate *= otherClient.ClientData.PKPoint / 100 - 1;
							maxFallRoleUsingRate *= otherClient.ClientData.PKPoint / 100 - 1;
							List<GoodsData> goodsDataList = new List<GoodsData>();
							if (maxFallRoleBagRate > 0)
							{
								List<GoodsData> fallGoodsList = Global.GetFallGoodsList(otherClient);
								if (fallGoodsList != null && fallGoodsList.Count > 0)
								{
									int fallBagGoodsNum = 0;
									for (int i = 0; i < fallGoodsList.Count; i++)
									{
										int randNum = Global.GetRandomNumber(1, 100001);
										if (randNum <= maxFallRoleBagRate)
										{
											GoodsData goodsData = fallGoodsList[i];
											if (null != goodsData)
											{
												int oldGoodsNum = 1;
												if (Global.GetGoodsDefaultCount(goodsData.GoodsID) > 1)
												{
													oldGoodsNum = goodsData.GCount;
												}
												if (GameManager.ClientMgr.FallRoleGoods(sl, Global._TCPManager.tcpClientPool, pool, otherClient, goodsData))
												{
													fallBagGoodsNum++;
													goodsData = Global.CopyGoodsData(goodsData);
													goodsData.Id = this.GetNextGoodsID();
													goodsData.GCount = oldGoodsNum;
													goodsDataList.Add(goodsData);
												}
											}
											if (fallBagGoodsNum >= maxmaxFallBagGoodsNum)
											{
												break;
											}
										}
									}
								}
							}
							if (maxFallRoleUsingRate > 0)
							{
								List<GoodsData> usingGoodsDataList = Global.GetUsingGoodsList(otherClient, 0);
								if (usingGoodsDataList != null && usingGoodsDataList.Count > 0)
								{
									int fallUsingGoodsNum = 0;
									for (int i = 0; i < usingGoodsDataList.Count; i++)
									{
										int goodsCatetoriy = Global.GetGoodsCatetoriy(usingGoodsDataList[i].GoodsID);
										int randNum = Global.GetRandomNumber(1, 100001);
										int thisTimeMaxFallRoleUsingRate = maxFallRoleUsingRate;
										if (randNum <= thisTimeMaxFallRoleUsingRate)
										{
											GoodsData goodsData = usingGoodsDataList[i];
											if (null != goodsData)
											{
												int oldGoodsNum = goodsData.GCount;
												if (GameManager.ClientMgr.FallRoleGoods(sl, Global._TCPManager.tcpClientPool, pool, otherClient, goodsData))
												{
													fallUsingGoodsNum++;
													goodsData.Id = this.GetNextGoodsID();
													goodsData.GCount = oldGoodsNum;
													goodsDataList.Add(goodsData);
													Global.NotifyChangeEquip(Global._TCPManager, pool, otherClient, goodsData, 1);
													goodsData.Using = 0;
													otherClient.UsingEquipMgr.RefreshEquip(goodsData);
												}
											}
											if (fallUsingGoodsNum >= maxFallUsingGoodsNum)
											{
												break;
											}
										}
									}
									if (fallUsingGoodsNum > 0)
									{
										Global.RefreshEquipProp(otherClient);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, pool, otherClient);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, pool, otherClient, true, false, 7);
									}
								}
							}
							if (goodsDataList.Count > 0)
							{
								strDropList = EventLogManager.MakeGoodsDataPropString(goodsDataList);
								Point grid = otherClient.CurrentGrid;
								List<GoodsPackItem> goodsPackItemList = this.GetRoleGoodsPackItemList(client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), goodsDataList, otherClient.ClientData.MapCode, otherClient.ClientData.CopyMapID, (int)grid.X, (int)grid.Y, enemyName);
								if (goodsPackItemList != null && goodsPackItemList.Count > 0)
								{
									StringBuilder sb = new StringBuilder();
									for (int i = 0; i < goodsPackItemList.Count; i++)
									{
										this.ProcessGoodsPackItem(client, otherClient, goodsPackItemList[i], 0);
										sb.AppendFormat("{0}", Global.GetGoodsNameByID(goodsPackItemList[i].GoodsDataList[0].GoodsID));
										if (i != goodsPackItemList.Count - 1)
										{
											sb.Append(" ");
										}
									}
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(383, new object[0]), new object[]
									{
										enemyName,
										sb.ToString()
									}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002A9B RID: 10907 RVA: 0x002609E8 File Offset: 0x0025EBE8
		public void ProcessRoleAbandonGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int toGridX, int toGridY)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			goodsDataList.Add(goodsData);
			Point grid = client.CurrentGrid;
			List<GoodsPackItem> goodsPackItemList = this.GetRoleGoodsPackItemList(client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), goodsDataList, client.ClientData.MapCode, client.ClientData.CopyMapID, (int)grid.X, (int)grid.Y, Global.FormatRoleName(client, client.ClientData.RoleName));
			if (goodsPackItemList != null && goodsPackItemList.Count > 0)
			{
				for (int i = 0; i < goodsPackItemList.Count; i++)
				{
					this.ProcessGoodsPackItem(client, null, goodsPackItemList[i], 0);
				}
			}
		}

		// Token: 0x06002A9C RID: 10908 RVA: 0x00260AAC File Offset: 0x0025ECAC
		private GameClient GetBattleRandomClient(List<BattleRoleItem> battleRoleItemList)
		{
			int randNum = Global.GetRandomNumber(0, 101);
			int maxNum = 0;
			int i = 0;
			while (i < battleRoleItemList.Count && i < 10)
			{
				if ((double)randNum > battleRoleItemList[i].Percent)
				{
					break;
				}
				maxNum++;
				i++;
			}
			GameClient result;
			if (maxNum < 0)
			{
				result = null;
			}
			else
			{
				int randIndex = Global.GetRandomNumber(0, maxNum);
				result = battleRoleItemList[randIndex].Client;
			}
			return result;
		}

		// Token: 0x06002A9D RID: 10909 RVA: 0x00260B34 File Offset: 0x0025ED34
		private void AddBattleBufferAndFlags(GameClient client, int bufferType)
		{
			double[] actionParams = new double[2];
			actionParams[0] = 1440.0;
			if (0 == bufferType)
			{
				actionParams[1] = 20.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 1;
			}
			else if (1 == bufferType)
			{
				actionParams[1] = 15.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 2;
			}
			else
			{
				if (2 != bufferType)
				{
					return;
				}
				actionParams[1] = 10.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 3;
			}
			Global.UpdateBufferData(client, BufferItemTypes.AntiRole, actionParams, 0, true);
			GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
			GameManager.ClientMgr.NotifyRoleBattleNameInfo(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.UpdateBattleNum(client, 1, false);
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x00260C91 File Offset: 0x0025EE91
		public void ProcessBattle(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, List<GoodsData> giveGoodsDataList, int fallGoodsPackID, int fallNum)
		{
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x00260C98 File Offset: 0x0025EE98
		public void SendMySelfGoodsPackItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				GoodsPackItem goodsPackItem = null;
				int i = 0;
				while (i < objsList.Count && i < 30)
				{
					if (objsList[i] is GoodsPackItem)
					{
						goodsPackItem = (objsList[i] as GoodsPackItem);
						if (goodsPackItem.OnlyID <= 0 || goodsPackItem.OnlyID == client.ClientData.RoleID)
						{
							if (goodsPackItem.GoodsDataList.Count > 0)
							{
								GoodsData goodsData = goodsPackItem.GoodsDataList[0];
								if (null != goodsData)
								{
									string teamRoleIDs = this.FormatTeamRoleIDs(goodsPackItem);
									int ExcellenceInfo = goodsData.ExcellenceInfo;
									lock (RebornEquip.SuperiorDrop)
									{
										if (RebornEquip.IsRebornEquip(goodsData.GoodsID) && RebornEquip.SuperiorDrop != null)
										{
											if (RebornEquip.SuperiorDrop.ContainsKey(ExcellenceInfo))
											{
												ExcellenceInfo = RebornEquip.SuperiorDrop[ExcellenceInfo].ShowColor;
											}
										}
									}
									GameManager.ClientMgr.NotifyMySelfNewGoodsPack(sl, pool, client, (goodsPackItem.BelongTo <= 0) ? -1 : goodsPackItem.OwnerRoleID, goodsPackItem.OwnerRoleName, goodsPackItem.AutoID, goodsPackItem.GoodsPackID, goodsPackItem.MapCode, (int)goodsPackItem.FallPoint.X, (int)goodsPackItem.FallPoint.Y, goodsData.GoodsID, goodsData.GCount, goodsPackItem.ProduceTicks, goodsPackItem.TeamID, teamRoleIDs, goodsData.Lucky, ExcellenceInfo, goodsData.AppendPropLev, goodsData.Forge_level);
								}
							}
						}
					}
					i++;
				}
			}
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x00260E8C File Offset: 0x0025F08C
		public void DelMySelfGoodsPackItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GoodsPackItem)
					{
						GoodsPackItem goodsPackItem = objsList[i] as GoodsPackItem;
						if (this.CanOpenGoodsPack(goodsPackItem, client.ClientData.RoleID))
						{
							List<GoodsData> Goodslist = goodsPackItem.GoodsDataList;
							if (Goodslist != null)
							{
								for (int j = 0; j < Goodslist.Count; j++)
								{
									if (!GoodsPackManager.IsFallTongQianGoods(Goodslist[j].GoodsID) && !Global.CanAddGoods(client, Goodslist[j].GoodsID, Goodslist[j].GCount, Goodslist[j].Binding, "1900-01-01 12:00:00", true, false))
									{
										return;
									}
								}
							}
							GameManager.ClientMgr.NotifyMySelfDelGoodsPack(sl, pool, client, goodsPackItem.AutoID);
						}
					}
				}
			}
		}

		// Token: 0x06002AA1 RID: 10913 RVA: 0x00260FAC File Offset: 0x0025F1AC
		public void ProcessAllGoodsPackItems(SocketListener sl, TCPOutPacketPool pool)
		{
			List<GoodsPackItem> goodsPackItemList = new List<GoodsPackItem>();
			lock (this._GoodsPackDict)
			{
				foreach (GoodsPackItem val in this._GoodsPackDict.Values)
				{
					goodsPackItemList.Add(val);
				}
			}
			long nowTicks = TimeUtil.NOW();
			GoodsPackItem goodsPackItem = null;
			for (int i = 0; i < goodsPackItemList.Count; i++)
			{
				goodsPackItem = goodsPackItemList[i];
				if (nowTicks - goodsPackItem.ProduceTicks >= (long)(Data.PackDestroyTimeTick * 1000))
				{
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict.Remove(goodsPackItem.AutoID);
					}
					GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
				}
			}
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x0026110C File Offset: 0x0025F30C
		private bool CanOpenGoodsPack(GoodsPackItem goodsPackItem, int roleID)
		{
			bool result;
			if (goodsPackItem.OnlyID > 0 && goodsPackItem.OnlyID != roleID)
			{
				result = false;
			}
			else if (goodsPackItem.BelongTo <= 0)
			{
				result = true;
			}
			else
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - goodsPackItem.ProduceTicks >= (long)(Data.GoodsPackOvertimeTick * 1000))
				{
					result = true;
				}
				else
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (-1 != goodsPackItem.TeamRoleIDs.IndexOf(roleID))
						{
							GameClient gc = GameManager.ClientMgr.FindClient(roleID);
							if (null != gc)
							{
								bool isTeamSharingMap = true;
								if (gc.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode)
								{
									isTeamSharingMap = false;
								}
								TeamData td = GameManager.TeamMgr.FindData(gc.ClientData.TeamID);
								if (td != null)
								{
									if (td.GetThingOpt > 0 && isTeamSharingMap)
									{
										return true;
									}
								}
							}
						}
					}
					result = (goodsPackItem.OwnerRoleID < 0 || goodsPackItem.OwnerRoleID == roleID);
				}
			}
			return result;
		}

		// Token: 0x06002AA3 RID: 10915 RVA: 0x00261258 File Offset: 0x0025F458
		public void UnLockGoodsPackItem(GameClient client)
		{
			if (null != client.ClientData.LockedGoodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					if (this._GoodsPackDict.ContainsKey(client.ClientData.LockedGoodsPackItem.AutoID))
					{
						client.ClientData.LockedGoodsPackItem.LockedRoleID = -1;
					}
				}
				client.ClientData.LockedGoodsPackItem = null;
			}
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x002612F8 File Offset: 0x0025F4F8
		public List<GoodsData> GetLeftGoodsDataList(GoodsPackItem goodsPackItem)
		{
			List<GoodsData> result;
			if (goodsPackItem.GoodsDataList == null)
			{
				result = null;
			}
			else
			{
				List<GoodsData> goodsDataList = new List<GoodsData>();
				for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
				{
					if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsPackItem.GoodsDataList[i].Id))
					{
						goodsDataList.Add(goodsPackItem.GoodsDataList[i]);
					}
				}
				result = goodsDataList;
			}
			return result;
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x00261378 File Offset: 0x0025F578
		private void ProcessTeamGoodsPack(GameClient client, GoodsPackItem goodsPackItem)
		{
			if (null != goodsPackItem)
			{
				if (TimeUtil.NOW() - goodsPackItem.ProduceTicks < (long)(Data.GoodsPackOvertimeTick * 1000))
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (goodsPackItem.GoodsIDToRolesDict.Count <= 0)
						{
							if (null == this.ProhibitRollHashSet)
							{
								this.ProhibitRollHashSet = new HashSet<int>();
								try
								{
									int[] ProhibitRollGoodsIDs = GameManager.systemParamsList.GetParamValueIntArrayByName("ProhibitRoll", ',');
									if (ProhibitRollGoodsIDs != null && ProhibitRollGoodsIDs.Length > 0)
									{
										foreach (int goodsID in ProhibitRollGoodsIDs)
										{
											this.ProhibitRollHashSet.Add(goodsID);
										}
									}
								}
								catch
								{
								}
							}
							for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
							{
								int goodsID = goodsPackItem.GoodsDataList[i].GoodsID;
								if (!this.ProhibitRollHashSet.Contains(goodsID))
								{
									if (null == goodsPackItem.TeamRoleDamages)
									{
										this.JugeGoodsID2RoleID(client, goodsPackItem, goodsPackItem.GoodsDataList[i].Id, goodsID);
									}
									else
									{
										this.JugeGoodsID2RoleIDByDamageRandom(client, goodsPackItem, goodsPackItem.GoodsDataList[i].Id, goodsID);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x00261508 File Offset: 0x0025F708
		public GoodsPackListData ProcessClickOnGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID, out TCPOutPacket tcpOutPacket, int nID, int openState, bool tcpPacketData)
		{
			tcpOutPacket = null;
			int retError = 0;
			long leftTicks = 0L;
			long packTicks = -1L;
			List<GoodsData> leftGoodsDataList = null;
			GoodsPackItem goodsPackItem = null;
			lock (this._GoodsPackDict)
			{
				if (this._GoodsPackDict.TryGetValue(autoID, out goodsPackItem))
				{
					if (openState > 0)
					{
						if (goodsPackItem != null)
						{
							List<GoodsData> GoodsDataList = this.GetLeftGoodsDataList(goodsPackItem);
							if (GoodsDataList != null)
							{
								for (int i = 0; i < GoodsDataList.Count; i++)
								{
									if (!GoodsPackManager.IsFallTongQianGoods(GoodsDataList[i].GoodsID) && !Global.CanAddGoods(client, GoodsDataList[i].GoodsID, GoodsDataList[i].GCount, GoodsDataList[i].Binding, "1900-01-01 12:00:00", true, false))
									{
										return null;
									}
								}
							}
						}
						if (this.CanOpenGoodsPack(goodsPackItem, client.ClientData.RoleID))
						{
							if (-1 == goodsPackItem.LockedRoleID || goodsPackItem.LockedRoleID == client.ClientData.RoleID)
							{
								goodsPackItem.LockedRoleID = client.ClientData.RoleID;
								client.ClientData.LockedGoodsPackItem = goodsPackItem;
								leftGoodsDataList = this.GetLeftGoodsDataList(goodsPackItem);
								this.ProcessTeamGoodsPack(client, goodsPackItem);
								goodsPackItem.OpenPackTicks = TimeUtil.NOW();
								if (null != goodsPackItem.TeamRoleIDs)
								{
									long lastOpenPackTicks = 0L;
									goodsPackItem.RolesTicksDict.TryGetValue(client.ClientData.RoleID, out lastOpenPackTicks);
									packTicks = 15000L - lastOpenPackTicks;
								}
							}
							else
							{
								retError = -3;
								goodsPackItem = null;
							}
						}
						else
						{
							long nowTicks = TimeUtil.NOW();
							leftTicks = (long)(Data.GoodsPackOvertimeTick * 1000) - (nowTicks - goodsPackItem.ProduceTicks);
							if (goodsPackItem.TeamRoleIDs != null && -1 != goodsPackItem.TeamRoleIDs.IndexOf(client.ClientData.RoleID))
							{
								packTicks = -2L;
							}
							retError = -2;
							goodsPackItem = null;
							LogManager.WriteLog(LogTypes.Info, "retError : -2", null, true);
						}
					}
					else
					{
						long lastOpenPackTicks = 0L;
						goodsPackItem.RolesTicksDict.TryGetValue(client.ClientData.RoleID, out lastOpenPackTicks);
						goodsPackItem.RolesTicksDict[client.ClientData.RoleID] = lastOpenPackTicks + (TimeUtil.NOW() - goodsPackItem.OpenPackTicks);
						goodsPackItem.LockedRoleID = -1;
						client.ClientData.LockedGoodsPackItem = null;
						goodsPackItem = null;
					}
				}
				else
				{
					retError = -1;
				}
			}
			List<GoodsData> goodsDataList = null;
			if (goodsPackItem != null)
			{
				goodsDataList = leftGoodsDataList;
			}
			GoodsPackListData goodsPackListData = new GoodsPackListData
			{
				AutoID = autoID,
				GoodsDataList = goodsDataList,
				OpenState = openState,
				RetError = retError,
				LeftTicks = leftTicks,
				PackTicks = packTicks
			};
			if (tcpPacketData)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, pool, nID);
			}
			return goodsPackListData;
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x00261864 File Offset: 0x0025FA64
		private GoodsPackItem GetLockedGoodsPackItem(GameClient client, int autoID)
		{
			GoodsPackItem goodsPackItem = null;
			GoodsPackItem result;
			if (!this._GoodsPackDict.TryGetValue(autoID, out goodsPackItem))
			{
				result = null;
			}
			else if (goodsPackItem.LockedRoleID != client.ClientData.RoleID)
			{
				result = null;
			}
			else
			{
				result = goodsPackItem;
			}
			return result;
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x002618AC File Offset: 0x0025FAAC
		public void ProcessGetThing(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID, int goodsDbID, out bool bRet)
		{
			bRet = true;
			List<GoodsData> goodsDataList = null;
			GoodsPackItem goodsPackItem = null;
			lock (this._GoodsPackDict)
			{
				goodsPackItem = this.GetLockedGoodsPackItem(client, autoID);
				if (null == goodsPackItem)
				{
					return;
				}
				string killedMonsterName = goodsPackItem.KilledMonsterName;
				goodsDataList = new List<GoodsData>();
				if (-1 == goodsDbID)
				{
					for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
					{
						if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsPackItem.GoodsDataList[i].Id))
						{
							goodsDataList.Add(goodsPackItem.GoodsDataList[i]);
						}
					}
				}
				else if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsDbID))
				{
					for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
					{
						if (goodsPackItem.GoodsDataList[i].Id == goodsDbID)
						{
							goodsDataList.Add(goodsPackItem.GoodsDataList[i]);
							break;
						}
					}
				}
			}
			if (goodsDataList != null && goodsDataList.Count > 0)
			{
				GameClient gc = null;
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					int toRoleID = this.FindGoodsID2RoleID(goodsPackItem, goodsDataList[i].Id);
					if (-1 == toRoleID)
					{
						gc = client;
					}
					else
					{
						gc = GameManager.ClientMgr.FindClient(toRoleID);
					}
					if (null != gc)
					{
						if (!GoodsPackManager.IsFallTongQianGoods(goodsDataList[i].GoodsID))
						{
							if (!RebornEquip.IsRebornType(goodsDataList[i].GoodsID))
							{
								if (!Global.CanAddGoods(gc, goodsDataList[i].GoodsID, 1, goodsDataList[i].Binding, "1900-01-01 12:00:00", true, false))
								{
									bRet = false;
									break;
								}
							}
							else if (!RebornEquip.CanAddGoodsToReborn(gc, goodsDataList[i].GoodsID, 1, goodsDataList[i].Binding, "1900-01-01 12:00:00", true, false))
							{
								bRet = false;
								break;
							}
						}
						lock (this._GoodsPackDict)
						{
							goodsPackItem.GoodsIDDict[goodsDataList[i].Id] = true;
						}
						if (!GoodsPackManager.ProcessFallTongQian(pool, gc, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsPackItem.FallLevel))
						{
							int nRet = Global.AddGoodsDBCommand_Hook(pool, gc, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, goodsDataList[i].Props, goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, goodsDataList[i].Jewellist, true, 1, "杀怪掉落后手动拾取", true, goodsDataList[i].Endtime, goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, goodsDataList[i].ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
							if (0 == nRet)
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, Global.ModifyGoodsLogName(goodsDataList[i]), "杀怪掉落后手动拾取", Global.GetMapName(client.ClientData.MapCode), "系统", "销毁", goodsDataList[i].GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsDataList[i]);
							}
							if (MoYuLongXue.InMoYuMap(goodsPackItem.MapCode))
							{
								if (MoYuLongXue.IsBHGoods(goodsDataList[i].GoodsID))
								{
									string broadMsg;
									if (client.ClientData.Faction > 0)
									{
										broadMsg = string.Format(GLang.GetLang(4007, new object[0]), new object[]
										{
											client.ClientData.BHName,
											client.ClientData.RoleName,
											goodsPackItem.KilledMonsterName,
											Global.GetGoodsName(goodsDataList[i])
										});
									}
									else
									{
										broadMsg = string.Format(GLang.GetLang(4008, new object[0]), client.ClientData.RoleName, goodsPackItem.KilledMonsterName, Global.GetGoodsName(goodsDataList[i]));
									}
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, broadMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
								}
							}
							if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(goodsPackItem.MapCode))
							{
								if (ZhuanShengShiLian.IsShiLianGoods(goodsDataList[i].GoodsID))
								{
									string broadMsg = string.Format(GLang.GetLang(4012, new object[0]), client.ClientData.RoleName, Global.GetGoodsName(goodsDataList[i]));
									ZhuanShengShiLian.BroadMsg(goodsPackItem.MapCode, broadMsg);
								}
							}
							if (ThemeBoss.getInstance().IsThemeBossScene(goodsPackItem.MapCode))
							{
								if (ThemeBoss.getInstance().IsThemeBossGoods(goodsDataList[i].GoodsID))
								{
									string broadMsg = string.Format(GLang.GetLang(4015, new object[0]), Global.FormatRoleNameWithZoneId(client), Global.GetGoodsName(goodsDataList[i]));
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, broadMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
								}
							}
						}
						GameManager.ClientMgr.NotifySelfGetThing(sl, pool, gc, goodsDbID);
						SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PickUpEquipCount);
						evObj.Arg1 = goodsDataList[i].GoodsID;
						evObj.Arg2 = goodsDataList[i].GCount;
						GlobalEventSource.getInstance().fireEvent(evObj);
					}
				}
				bool nothing = false;
				lock (this._GoodsPackDict)
				{
					nothing = (goodsPackItem.GoodsIDDict.Count >= goodsPackItem.GoodsDataList.Count);
				}
				if (nothing)
				{
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict.Remove(autoID);
					}
					GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
					List<object> objsList = Global.GetAll9Clients(goodsPackItem);
					GameManager.ClientMgr.NotifyOthersDelGoodsPack(sl, pool, objsList, client.ClientData.MapCode, autoID, client.ClientData.RoleID);
				}
			}
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x0026204C File Offset: 0x0026024C
		public void ExternalRemoveGoodsPack(GoodsPackItem goodsPackItem)
		{
			GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
			List<object> objsList = Global.GetAll9Clients(goodsPackItem);
			GameManager.ClientMgr.NotifyOthersDelGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, objsList, goodsPackItem.MapCode, goodsPackItem.AutoID, -1);
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x002620AC File Offset: 0x002602AC
		public List<FallGoodsItem> GetFallGoodsItemList(int goodsPackID)
		{
			List<FallGoodsItem> fallGoodsItemList = this.GetNormalFallGoodsItem(goodsPackID);
			List<FallGoodsItem> result;
			if (null == fallGoodsItemList)
			{
				result = null;
			}
			else
			{
				result = fallGoodsItemList;
			}
			return result;
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x002620D8 File Offset: 0x002602D8
		public List<FallGoodsItem> GetRandomFallGoodsItemList(int goodsPackID, int maxFallCount, bool isGood)
		{
			List<FallGoodsItem> fallGoodsItemList = this.GetNormalFallGoodsItem(goodsPackID);
			List<FallGoodsItem> result;
			if (null == fallGoodsItemList)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> randFallGoodsItemList = Global.RandomSortList<FallGoodsItem>(fallGoodsItemList);
				if (maxFallCount > 0)
				{
					while (randFallGoodsItemList.Count > maxFallCount)
					{
						randFallGoodsItemList.RemoveAt(randFallGoodsItemList.Count - 1);
					}
				}
				for (int i = 0; i < fallGoodsItemList.Count; i++)
				{
					fallGoodsItemList[i].IsGood = isGood;
				}
				result = randFallGoodsItemList;
			}
			return result;
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x00262164 File Offset: 0x00260364
		public List<GoodsData> GetGoodsDataListFromFallGoodsItemList(List<FallGoodsItem> fallGoodsItemList)
		{
			List<GoodsData> result;
			if (fallGoodsItemList == null || fallGoodsItemList.Count <= 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> goodsDataList = new List<GoodsData>();
				for (int i = 0; i < fallGoodsItemList.Count; i++)
				{
					int goodsQualtiy = 0;
					int goodsLevel = this.GetFallGoodsLevel(fallGoodsItemList[i].FallLevelID);
					int goodsBornIndex = 0;
					int luckyRate = this.GetLuckyGoodsID(fallGoodsItemList[i].LuckyRate);
					int appendPropLev = this.GetZhuiJiaGoodsLevelID(fallGoodsItemList[i].ZhuiJiaID);
					int excellenceInfo = this.GetExcellencePropertysID(fallGoodsItemList[i].GoodsID, fallGoodsItemList[i].ExcellencePropertyID);
					string props = "";
					GoodsData goodsData = new GoodsData
					{
						Id = i,
						GoodsID = fallGoodsItemList[i].GoodsID,
						Using = 0,
						Forge_level = goodsLevel,
						Starttime = "1900-01-01 12:00:00",
						Endtime = "1900-01-01 12:00:00",
						Site = 0,
						Quality = goodsQualtiy,
						Props = props,
						GCount = 1,
						Binding = fallGoodsItemList[i].Binding,
						Jewellist = "",
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = goodsBornIndex,
						Lucky = luckyRate,
						Strong = 0,
						ExcellenceInfo = excellenceInfo,
						AppendPropLev = appendPropLev,
						ChangeLifeLevForEquip = 0
					};
					goodsDataList.Add(goodsData);
				}
				result = goodsDataList;
			}
			return result;
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x00262300 File Offset: 0x00260500
		private GoodsPackItem FindGoodsPackItemByPos(Point grid, GameClient gameClient)
		{
			MapGrid mapGrid = null;
			GoodsPackItem result;
			if (!GameManager.MapGridMgr.DictGrids.TryGetValue(gameClient.ClientData.MapCode, out mapGrid))
			{
				result = null;
			}
			else if (null == mapGrid)
			{
				result = null;
			}
			else
			{
				List<object> objsList = mapGrid.FindObjects((int)grid.X, (int)grid.Y);
				if (null != objsList)
				{
					for (int objIndex = 0; objIndex < objsList.Count; objIndex++)
					{
						if (objsList[objIndex] is GoodsPackItem)
						{
							if (gameClient.ClientData.CopyMapID <= 0)
							{
								return objsList[objIndex] as GoodsPackItem;
							}
							if ((objsList[objIndex] as GoodsPackItem).CopyMapID == gameClient.ClientData.CopyMapID)
							{
								return objsList[objIndex] as GoodsPackItem;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x00262418 File Offset: 0x00260618
		public void ProcessClickGoodsPackWhenMovingEnd(GameClient client)
		{
			GoodsPackItem goodsPackItem = this.FindGoodsPackItemByPos(client.CurrentGrid, client);
			if (null != goodsPackItem)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					GoodsPackListData goodsPackListData = GameManager.GoodsPackMgr.ProcessClickOnGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, out tcpOutPacket, 147, 1, false);
					if (null != goodsPackListData)
					{
						if (0 == goodsPackListData.RetError)
						{
							bool bRet = true;
							GameManager.GoodsPackMgr.ProcessGetThing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, -1, out bRet);
							this.UnLockGoodsPackItem(client);
							if (bRet)
							{
								this.TakeFallGoodsRecords(goodsPackItem, client);
								goodsPackListData.GoodsDataList = null;
								TCPOutPacket tcpOutPacket2 = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
								if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket2, true))
								{
								}
							}
						}
						else if (goodsPackListData.RetError == -1)
						{
							GameManager.GoodsPackMgr.ExternalRemoveGoodsPack(goodsPackItem);
						}
						else
						{
							goodsPackListData.GoodsDataList = null;
							TCPOutPacket tcpOutPacket2 = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
							if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket2, true))
							{
							}
						}
					}
				}
				finally
				{
				}
			}
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x002625B0 File Offset: 0x002607B0
		private void InitShiQuGoodsList()
		{
			lock (this._CacheShiQuGoodsDict)
			{
				if (this._CacheShiQuGoodsDict.Count <= 0)
				{
					string str = GameManager.systemParamsList.GetParamValueByName("ShiQuGoodsList");
					if (!string.IsNullOrEmpty(str))
					{
						string[] fields = str.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < fields.Length; i++)
						{
							string[] fields2 = fields[i].Split(new char[]
							{
								','
							});
							Dictionary<int, bool> dict = new Dictionary<int, bool>();
							for (int j = 0; j < fields2.Length; j++)
							{
								dict[Global.SafeConvertToInt32(fields2[j])] = true;
							}
							this._CacheShiQuGoodsDict[i] = dict;
						}
					}
				}
			}
		}

		// Token: 0x06002AB0 RID: 10928 RVA: 0x002626C8 File Offset: 0x002608C8
		private int GetPickUpShiQuGoodsType(int goodsID)
		{
			lock (this._CacheShiQuGoodsDict)
			{
				foreach (int key in this._CacheShiQuGoodsDict.Keys)
				{
					Dictionary<int, bool> dict = this._CacheShiQuGoodsDict[key];
					if (dict.ContainsKey(goodsID))
					{
						return key;
					}
				}
			}
			return -1;
		}

		// Token: 0x06002AB1 RID: 10929 RVA: 0x00262784 File Offset: 0x00260984
		private bool CanPickUpGoodss(GameClient client, GoodsPackItem goodsPackItem)
		{
			bool result;
			if (client.ClientData.AutoFightGetThings == 0)
			{
				result = false;
			}
			else if (goodsPackItem.GoodsDataList.Count <= 0)
			{
				result = false;
			}
			else
			{
				this.InitShiQuGoodsList();
				GoodsData goodsData = goodsPackItem.GoodsDataList[0];
				if (goodsPackItem.OnlyID > 0 && goodsPackItem.OnlyID != client.ClientData.RoleID)
				{
					result = false;
				}
				else
				{
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods) || null == systemGoods)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("处理拾取物品到背包时，获取物品xml信息失败: GoodsID={0}", goodsData.GoodsID), null, true);
						result = false;
					}
					else if (!GoodsPackManager.IsFallTongQianGoods(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true, false))
					{
						result = false;
					}
					else
					{
						int categoriy = systemGoods.GetIntValue("Categoriy", -1);
						if (categoriy >= 0 && categoriy < 49)
						{
							int color = Global.GetEquipColor(goodsData);
							int bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, color - 1);
							result = (1 == bitVal);
						}
						else
						{
							int shiquGoodsType = this.GetPickUpShiQuGoodsType(goodsData.GoodsID);
							int bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 24);
							if (1 == bitVal)
							{
								if (0 == shiquGoodsType)
								{
									return true;
								}
							}
							bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 25);
							if (1 == bitVal)
							{
								if (1 == shiquGoodsType)
								{
									return true;
								}
							}
							bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 26);
							if (1 == bitVal)
							{
								if (2 == shiquGoodsType)
								{
									return true;
								}
							}
							bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 27);
							if (1 == bitVal)
							{
								if (3 == shiquGoodsType)
								{
									return true;
								}
							}
							bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 28);
							if (1 == bitVal)
							{
								if (4 == shiquGoodsType)
								{
									return true;
								}
							}
							bitVal = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 29);
							if (1 == bitVal)
							{
								if (-1 == shiquGoodsType)
								{
									return true;
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06002AB2 RID: 10930 RVA: 0x00262A68 File Offset: 0x00260C68
		private List<GoodsPackItem> FindGoodsPackItemListByPos(Point grid, int girdNum, GameClient gameClient)
		{
			MapGrid mapGrid = null;
			List<GoodsPackItem> result;
			if (!GameManager.MapGridMgr.DictGrids.TryGetValue(gameClient.ClientData.MapCode, out mapGrid))
			{
				result = null;
			}
			else if (null == mapGrid)
			{
				result = null;
			}
			else
			{
				int startGridX = (int)grid.X - girdNum;
				int endGridX = (int)grid.X + girdNum;
				int startGridY = (int)grid.Y - girdNum;
				int endGridY = (int)grid.Y + girdNum;
				startGridX = Global.GMax(startGridX, 0);
				startGridY = Global.GMax(startGridY, 0);
				endGridX = Global.GMin(endGridX, mapGrid.MapGridXNum - 1);
				endGridY = Global.GMin(endGridY, mapGrid.MapGridYNum - 1);
				List<GoodsPackItem> GoodsPackItemList = new List<GoodsPackItem>();
				for (int gridX = startGridX; gridX <= endGridX; gridX++)
				{
					for (int gridY = startGridY; gridY <= endGridY; gridY++)
					{
						List<object> objsList = mapGrid.FindGoodsPackItems(gridX, gridY);
						if (null != objsList)
						{
							int objIndex = 0;
							while (objIndex < objsList.Count)
							{
								if (objsList[objIndex] is GoodsPackItem)
								{
									if (this.CanOpenGoodsPack(objsList[objIndex] as GoodsPackItem, gameClient.ClientData.RoleID))
									{
										if (this.CanPickUpGoodss(gameClient, objsList[objIndex] as GoodsPackItem))
										{
											if (gameClient.ClientData.CopyMapID > 0)
											{
												if ((objsList[objIndex] as GoodsPackItem).CopyMapID == gameClient.ClientData.CopyMapID)
												{
													GoodsPackItemList.Add(objsList[objIndex] as GoodsPackItem);
												}
											}
											else
											{
												GoodsPackItemList.Add(objsList[objIndex] as GoodsPackItem);
											}
										}
									}
								}
								IL_1BB:
								objIndex++;
								continue;
								goto IL_1BB;
							}
						}
					}
				}
				result = GoodsPackItemList;
			}
			return result;
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x00262C84 File Offset: 0x00260E84
		public void ProcessClickGoodsPackWhenMovingToOtherGrid(GameClient client, int gridNum = 1)
		{
			List<GoodsPackItem> goodsPackItemList = this.FindGoodsPackItemListByPos(client.CurrentGrid, gridNum, client);
			if (goodsPackItemList != null && goodsPackItemList.Count > 0)
			{
				lock (client.ClientData.PickUpGoodsPackMutex)
				{
					for (int i = 0; i < goodsPackItemList.Count; i++)
					{
						GoodsPackItem goodsPackItem = goodsPackItemList[i];
						TCPOutPacket tcpOutPacket = null;
						try
						{
							GoodsPackListData goodsPackListData = GameManager.GoodsPackMgr.ProcessClickOnGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, out tcpOutPacket, 147, 1, false);
							if (null != goodsPackListData)
							{
								if (0 == goodsPackListData.RetError)
								{
									bool bRet = true;
									GameManager.GoodsPackMgr.ProcessGetThing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, -1, out bRet);
									this.UnLockGoodsPackItem(client);
									if (!bRet)
									{
										break;
									}
									this.TakeFallGoodsRecords(goodsPackItem, client);
									goodsPackListData.GoodsDataList = null;
									TCPOutPacket tcpOutPacket2 = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
									if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket2, true))
									{
									}
								}
								else if (goodsPackListData.RetError == -1)
								{
									GameManager.GoodsPackMgr.ExternalRemoveGoodsPack(goodsPackItem);
								}
								else
								{
									goodsPackListData.GoodsDataList = null;
									TCPOutPacket tcpOutPacket2 = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
									if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket2, true))
									{
									}
								}
							}
						}
						finally
						{
						}
					}
				}
			}
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x00262EA0 File Offset: 0x002610A0
		private void WriteFallGoodsRecords(GoodsPackItem goodsPackItem)
		{
			GoodsData goodsData = goodsPackItem.GoodsDataList[0];
			SystemXmlItem systemGoods = Global.CanBroadcastOrEventGoods(goodsData.GoodsID);
			if (null != systemGoods)
			{
				GameManager.DBCmdMgr.AddDBCmd(10118, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}", new object[]
				{
					goodsPackItem.OwnerRoleID,
					goodsPackItem.AutoID,
					-1,
					goodsData.GoodsID,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.Quality,
					goodsData.Forge_level,
					goodsData.Jewellist,
					Global.GetMapName(goodsPackItem.MapCode),
					goodsPackItem.CurrentGrid.ToString(),
					goodsPackItem.KilledMonsterName
				}), null, 0);
				string resList = EventLogManager.NewGoodsDataPropString(goodsData);
				EventLogManager.AddFallGoodsEvent(goodsPackItem.MapCode, goodsPackItem.OwnerRoleID, goodsPackItem.KilledMonsterName, resList);
			}
		}

		// Token: 0x06002AB5 RID: 10933 RVA: 0x00262FD0 File Offset: 0x002611D0
		private void TakeFallGoodsRecords(GoodsPackItem goodsPackItem, GameClient client)
		{
			GoodsData goodsData = goodsPackItem.GoodsDataList[0];
			SystemXmlItem systemGoods = Global.CanBroadcastOrEventGoods(goodsData.GoodsID);
			if (null != systemGoods)
			{
				GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "拾取物品", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsData);
				GameManager.DBCmdMgr.AddDBCmd(10118, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}", new object[]
				{
					client.ClientData.RoleID,
					goodsPackItem.AutoID,
					1,
					goodsData.GoodsID,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.Quality,
					goodsData.Forge_level,
					goodsData.Jewellist,
					Global.GetMapName(goodsPackItem.MapCode),
					goodsPackItem.CurrentGrid.ToString(),
					goodsPackItem.KilledMonsterName
				}), null, client.ServerId);
			}
		}

		// Token: 0x040039B7 RID: 14775
		private long BaseAutoID = 0L;

		// Token: 0x040039B8 RID: 14776
		private long BaseGoodsID = 0L;

		// Token: 0x040039B9 RID: 14777
		private long BaseRoleGoodsPackID = 0L;

		// Token: 0x040039BA RID: 14778
		private int GlobalFallGoodsLimitDayID;

		// Token: 0x040039BB RID: 14779
		private Dictionary<int, int> _GlobalFallGoodsLimitDict = new Dictionary<int, int>();

		// Token: 0x040039BC RID: 14780
		private Dictionary<int, int> GlobalFallGoodsNumDict = new Dictionary<int, int>();

		// Token: 0x040039BD RID: 14781
		private Dictionary<int, List<FallGoodsItem>> _FallGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		// Token: 0x040039BE RID: 14782
		private Dictionary<int, List<FallGoodsItem>> _LimitTimeFallGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		// Token: 0x040039BF RID: 14783
		private DateTime _LimitTimeStartDayTime = new DateTime(2000, 1, 1);

		// Token: 0x040039C0 RID: 14784
		private DateTime _LimitTimeEndDayTime = new DateTime(2000, 1, 1);

		// Token: 0x040039C1 RID: 14785
		private Dictionary<int, List<FallGoodsItem>> _FixedGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		// Token: 0x040039C2 RID: 14786
		private Dictionary<int, Tuple<int, int>> _FallGoodsMaxCountDict = new Dictionary<int, Tuple<int, int>>();

		// Token: 0x040039C3 RID: 14787
		private Dictionary<int, int> _LimitTimeFallGoodsMaxCountDict = new Dictionary<int, int>();

		// Token: 0x040039C4 RID: 14788
		private Dictionary<int, FallQualityItem> _FallGoodsQualityDict = new Dictionary<int, FallQualityItem>();

		// Token: 0x040039C5 RID: 14789
		private Dictionary<int, FallLevelItem> _FallGoodsLevelDict = new Dictionary<int, FallLevelItem>();

		// Token: 0x040039C6 RID: 14790
		private Dictionary<int, FallBornIndexItem> _FallGoodsBornIndexDict = new Dictionary<int, FallBornIndexItem>();

		// Token: 0x040039C7 RID: 14791
		private Dictionary<int, ZhuiJiaIDItem> _ZhuiJiaIDDict = new Dictionary<int, ZhuiJiaIDItem>();

		// Token: 0x040039C8 RID: 14792
		private Dictionary<int, ExcellencePropertyGroupItem> _ExcellencePropertyGroupItemDict = new Dictionary<int, ExcellencePropertyGroupItem>();

		// Token: 0x040039C9 RID: 14793
		public static int TeamShareMode_Flags = 1;

		// Token: 0x040039CA RID: 14794
		public static bool TeamShareMode_MaxDamage = false;

		// Token: 0x040039CB RID: 14795
		public static bool TeamShareMode_RandomByDamage = false;

		// Token: 0x040039CC RID: 14796
		public static int MaxFallCount = 10000;

		// Token: 0x040039CD RID: 14797
		private Dictionary<int, GoodsPackItem> _GoodsPackDict = new Dictionary<int, GoodsPackItem>();

		// Token: 0x040039CE RID: 14798
		private HashSet<int> ProhibitRollHashSet;

		// Token: 0x040039CF RID: 14799
		private Dictionary<int, Dictionary<int, bool>> _CacheShiQuGoodsDict = new Dictionary<int, Dictionary<int, bool>>();
	}
}
