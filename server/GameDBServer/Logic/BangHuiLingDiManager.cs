using System;
using System.Collections.Generic;
using System.Globalization;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class BangHuiLingDiManager
	{
		
		public void LoadBangHuiLingDiItemsDictFromDB(DBManager dbMgr)
		{
			DBQuery.QueryBHLingDiInfoDict(dbMgr, this._BangHuiLingDiItemsDict);
			for (int i = 1; i < 7; i++)
			{
				if (!this._BangHuiLingDiItemsDict.ContainsKey(i))
				{
					BangHuiLingDiInfoData BangHuiLingDiInfoData = new BangHuiLingDiInfoData
					{
						LingDiID = i,
						ZoneID = 0,
						BHName = "",
						LingDiTax = 0,
						TakeDayID = 0,
						TakeDayNum = 0,
						YestodayTax = 0,
						TaxDayID = 0,
						TodayTax = 0,
						TotalTax = 0
					};
					this._BangHuiLingDiItemsDict[BangHuiLingDiInfoData.LingDiID] = BangHuiLingDiInfoData;
				}
			}
		}

		
		public BangHuiLingDiInfoData FindBangHuiLingDiByID(int lingDiID)
		{
			BangHuiLingDiInfoData BangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out BangHuiLingDiInfoData))
				{
					return null;
				}
			}
			return BangHuiLingDiInfoData;
		}

		
		public void ClearBangHuiLingDi(int bhid)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData val in this._BangHuiLingDiItemsDict.Values)
				{
					if (val.BHID == bhid)
					{
						val.BHID = 0;
						val.ZoneID = 0;
						val.BHName = "";
						val.LingDiTax = 0;
						val.TakeDayID = 0;
						val.TakeDayNum = 0;
						val.YestodayTax = 0;
						val.TaxDayID = 0;
						val.TodayTax = 0;
						val.TotalTax = 0;
					}
				}
			}
		}

		
		public void OnChangeBangHuiName(int bhid, string oldName, string newName)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData val in this._BangHuiLingDiItemsDict.Values)
				{
					if (val.BHID == bhid)
					{
						val.BHName = newName;
					}
				}
			}
		}

		
		public void ClearBangHuiLingDiByID(int lingDiID)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData val in this._BangHuiLingDiItemsDict.Values)
				{
					if (val.LingDiID == lingDiID)
					{
						val.TotalTax = 0;
						break;
					}
				}
			}
		}

		
		public BangHuiLingDiInfoData ClearLingDiBangHuiInfo(int lingDiID)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				bangHuiLingDiInfoData.BHID = 0;
				bangHuiLingDiInfoData.ZoneID = 0;
				bangHuiLingDiInfoData.BHName = "";
				bangHuiLingDiInfoData.LingDiTax = 0;
				bangHuiLingDiInfoData.TakeDayID = 0;
				bangHuiLingDiInfoData.TakeDayNum = 0;
				bangHuiLingDiInfoData.YestodayTax = 0;
				bangHuiLingDiInfoData.TaxDayID = 0;
				bangHuiLingDiInfoData.TodayTax = 0;
				bangHuiLingDiInfoData.TotalTax = 0;
			}
			return bangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData AddBangHuiLingDi(int bhid, int zoneID, string bhName, int lingDiID)
		{
			BangHuiLingDiInfoData BangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.ContainsKey(lingDiID))
				{
					BangHuiLingDiInfoData = new BangHuiLingDiInfoData
					{
						LingDiID = lingDiID,
						BHID = bhid,
						ZoneID = zoneID,
						BHName = bhName,
						LingDiTax = 0,
						TakeDayID = 0,
						TakeDayNum = 0,
						YestodayTax = 0,
						TaxDayID = 0,
						TodayTax = 0,
						TotalTax = 0
					};
					this._BangHuiLingDiItemsDict[BangHuiLingDiInfoData.LingDiID] = BangHuiLingDiInfoData;
				}
				else
				{
					BangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[lingDiID];
					if (BangHuiLingDiInfoData.BHID != bhid)
					{
						BangHuiLingDiInfoData.LingDiTax = 0;
						BangHuiLingDiInfoData.TakeDayID = 0;
						BangHuiLingDiInfoData.TakeDayNum = 0;
						BangHuiLingDiInfoData.YestodayTax = 0;
						BangHuiLingDiInfoData.TaxDayID = 0;
						BangHuiLingDiInfoData.TodayTax = 0;
						BangHuiLingDiInfoData.TotalTax = 0;
					}
					BangHuiLingDiInfoData.BHID = bhid;
					BangHuiLingDiInfoData.ZoneID = zoneID;
					BangHuiLingDiInfoData.BHName = bhName;
				}
			}
			return BangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData UpdateBangHuiLingDiTax(int bhid, int lingDiID, int tax)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				bangHuiLingDiInfoData.LingDiTax = tax;
			}
			return bangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData UpdateBangHuiLingDiWarRequest(int lingDiID, string warRequest)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				bangHuiLingDiInfoData.WarRequest = warRequest;
			}
			return bangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData AddLingDiTaxMoney(int bhid, int lingDiID, int addMoney)
		{
			int dayID = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				bangHuiLingDiInfoData.TotalTax += addMoney;
				if (bangHuiLingDiInfoData.TaxDayID == dayID)
				{
					bangHuiLingDiInfoData.TodayTax += addMoney;
				}
				else
				{
					bangHuiLingDiInfoData.YestodayTax = bangHuiLingDiInfoData.TodayTax;
					bangHuiLingDiInfoData.TaxDayID = dayID;
					bangHuiLingDiInfoData.TodayTax = addMoney;
				}
			}
			return bangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData TakeLingDiTaxMoney(int bhid, int lingDiID, int takeMoney)
		{
			int dayID = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				if (dayID == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						return null;
					}
				}
				if ((double)takeMoney > (double)bangHuiLingDiInfoData.TotalTax * 0.25)
				{
					return null;
				}
				bangHuiLingDiInfoData.TakeDayID = dayID;
				bangHuiLingDiInfoData.TakeDayNum = 1;
				bangHuiLingDiInfoData.TotalTax = Math.Max(bangHuiLingDiInfoData.TotalTax - takeMoney, 0);
			}
			return bangHuiLingDiInfoData;
		}

		
		public BangHuiLingDiInfoData TakeLingDiDailyAward(int bhid, int lingDiID)
		{
			int dayID = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				if (dayID == bangHuiLingDiInfoData.AwardFetchDay)
				{
					return null;
				}
				bangHuiLingDiInfoData.AwardFetchDay = dayID;
			}
			return bangHuiLingDiInfoData;
		}

		
		public TCPOutPacket GetBangHuiLingDiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			Dictionary<int, BangHuiLingDiItemData> bangHuiLingDiItemDataDict = new Dictionary<int, BangHuiLingDiItemData>();
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (int key in this._BangHuiLingDiItemsDict.Keys)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[key];
					bangHuiLingDiItemDataDict[key] = new BangHuiLingDiItemData
					{
						LingDiID = bangHuiLingDiInfoData.LingDiID,
						BHID = bangHuiLingDiInfoData.BHID,
						ZoneID = bangHuiLingDiInfoData.ZoneID,
						BHName = bangHuiLingDiInfoData.BHName,
						LingDiTax = bangHuiLingDiInfoData.LingDiTax,
						WarRequest = bangHuiLingDiInfoData.WarRequest,
						AwardFetchDay = bangHuiLingDiInfoData.AwardFetchDay
					};
				}
			}
			return DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiItemData>>(bangHuiLingDiItemDataDict, pool, cmdID);
		}

		
		public TCPOutPacket GetBangHuiLingDiInfosDictTCPOutPacket(TCPOutPacketPool pool, int bhid, int cmdID)
		{
			Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfoDataDict = new Dictionary<int, BangHuiLingDiInfoData>();
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (int key in this._BangHuiLingDiItemsDict.Keys)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[key];
					if (bhid == bangHuiLingDiInfoData.BHID)
					{
						bangHuiLingDiInfoDataDict[key] = new BangHuiLingDiInfoData
						{
							LingDiID = bangHuiLingDiInfoData.LingDiID,
							BHID = bangHuiLingDiInfoData.BHID,
							ZoneID = bangHuiLingDiInfoData.ZoneID,
							BHName = bangHuiLingDiInfoData.BHName,
							LingDiTax = bangHuiLingDiInfoData.LingDiTax,
							TakeDayID = bangHuiLingDiInfoData.TakeDayID,
							TakeDayNum = bangHuiLingDiInfoData.TakeDayNum,
							YestodayTax = bangHuiLingDiInfoData.YestodayTax,
							TaxDayID = bangHuiLingDiInfoData.TaxDayID,
							TodayTax = bangHuiLingDiInfoData.TodayTax,
							TotalTax = bangHuiLingDiInfoData.TotalTax
						};
					}
				}
			}
			return DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(bangHuiLingDiInfoDataDict, pool, cmdID);
		}

		
		private static int WeekOfYear()
		{
			GregorianCalendar gc = new GregorianCalendar();
			return gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		
		public void ProcessClearYangZhouTotalTax(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks;
			if (nowTicks - this.LastClearYangZhouTotalTaxTicks >= 600000000L)
			{
				this.LastClearYangZhouTotalTaxTicks = nowTicks;
				int thisWeekID = BangHuiLingDiManager.WeekOfYear();
				if (thisWeekID != this.ThisWeekID)
				{
					this.ThisWeekID = thisWeekID;
					this.ClearBangHuiLingDiByID(3);
					DBWriter.ClearBHLingDiTotalTaxByID(dbMgr, 3);
				}
			}
		}

		
		private Dictionary<int, BangHuiLingDiInfoData> _BangHuiLingDiItemsDict = new Dictionary<int, BangHuiLingDiInfoData>();

		
		private int ThisWeekID = BangHuiLingDiManager.WeekOfYear();

		
		private long LastClearYangZhouTotalTaxTicks = DateTime.Now.Ticks;
	}
}
