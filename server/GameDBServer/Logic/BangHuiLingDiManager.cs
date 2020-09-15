using System;
using System.Collections.Generic;
using System.Globalization;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001A4 RID: 420
	public class BangHuiLingDiManager
	{
		// Token: 0x060008E2 RID: 2274 RVA: 0x00052CF0 File Offset: 0x00050EF0
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

		// Token: 0x060008E3 RID: 2275 RVA: 0x00052D98 File Offset: 0x00050F98
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

		// Token: 0x060008E4 RID: 2276 RVA: 0x00052E00 File Offset: 0x00051000
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

		// Token: 0x060008E5 RID: 2277 RVA: 0x00052EF0 File Offset: 0x000510F0
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

		// Token: 0x060008E6 RID: 2278 RVA: 0x00052F9C File Offset: 0x0005119C
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

		// Token: 0x060008E7 RID: 2279 RVA: 0x00053048 File Offset: 0x00051248
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

		// Token: 0x060008E8 RID: 2280 RVA: 0x000530FC File Offset: 0x000512FC
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

		// Token: 0x060008E9 RID: 2281 RVA: 0x00053234 File Offset: 0x00051434
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

		// Token: 0x060008EA RID: 2282 RVA: 0x000532B8 File Offset: 0x000514B8
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

		// Token: 0x060008EB RID: 2283 RVA: 0x00053328 File Offset: 0x00051528
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

		// Token: 0x060008EC RID: 2284 RVA: 0x00053404 File Offset: 0x00051604
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

		// Token: 0x060008ED RID: 2285 RVA: 0x00053504 File Offset: 0x00051704
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

		// Token: 0x060008EE RID: 2286 RVA: 0x000535B0 File Offset: 0x000517B0
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

		// Token: 0x060008EF RID: 2287 RVA: 0x000536D4 File Offset: 0x000518D4
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

		// Token: 0x060008F0 RID: 2288 RVA: 0x00053858 File Offset: 0x00051A58
		private static int WeekOfYear()
		{
			GregorianCalendar gc = new GregorianCalendar();
			return gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00053880 File Offset: 0x00051A80
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

		// Token: 0x040009A1 RID: 2465
		private Dictionary<int, BangHuiLingDiInfoData> _BangHuiLingDiItemsDict = new Dictionary<int, BangHuiLingDiInfoData>();

		// Token: 0x040009A2 RID: 2466
		private int ThisWeekID = BangHuiLingDiManager.WeekOfYear();

		// Token: 0x040009A3 RID: 2467
		private long LastClearYangZhouTotalTaxTicks = DateTime.Now.Ticks;
	}
}
