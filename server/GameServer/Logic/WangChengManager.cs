using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007B0 RID: 1968
	public class WangChengManager
	{
		// Token: 0x060033A4 RID: 13220 RVA: 0x002DCBC8 File Offset: 0x002DADC8
		public static void UpdateWangZuBHNameFromDBServer(int bhid)
		{
			WangChengManager.WangZuBHid = bhid;
			if (bhid > 0)
			{
				BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, 0);
				if (null != bangHuiMiniData)
				{
					WangChengManager.WangZuBHName = bangHuiMiniData.BHName;
				}
			}
		}

		// Token: 0x060033A5 RID: 13221 RVA: 0x002DCC08 File Offset: 0x002DAE08
		public static int GetWangZuBHid()
		{
			return WangChengManager.WangZuBHid;
		}

		// Token: 0x060033A6 RID: 13222 RVA: 0x002DCC20 File Offset: 0x002DAE20
		public static string GetWangZuBHName()
		{
			return WangChengManager.WangZuBHName;
		}

		// Token: 0x060033A7 RID: 13223 RVA: 0x002DCC38 File Offset: 0x002DAE38
		public static void ParseWeekDaysTimes()
		{
			string WangChengZhanWeekDays_str = GameManager.systemParamsList.GetParamValueByName("WangChengZhanWeekDays");
			if (!string.IsNullOrEmpty(WangChengZhanWeekDays_str))
			{
				string[] WangChengZhanWeekDays_fields = WangChengZhanWeekDays_str.Split(new char[]
				{
					','
				});
				int[] weekDays = new int[WangChengZhanWeekDays_fields.Length];
				for (int i = 0; i < WangChengZhanWeekDays_fields.Length; i++)
				{
					weekDays[i] = Global.SafeConvertToInt32(WangChengZhanWeekDays_fields[i]);
				}
				if (weekDays.Length > 0 && weekDays[0] >= 0)
				{
					WangChengManager.WangChengZhanWeekDaysByConfig = true;
					WangChengManager.WangChengZhanWeekDays = weekDays;
				}
			}
			string wangChengZhanFightingDayTimes_str = GameManager.systemParamsList.GetParamValueByName("WangChengZhanFightingDayTimes");
			WangChengManager.WangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(wangChengZhanFightingDayTimes_str);
			WangChengManager.MaxTakingHuangGongSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTakingHuangGongSecs", -1);
			WangChengManager.MaxTakingHuangGongSecs *= 1000;
			Global.UpdateWangChengZhanWeekDays(true);
			WangChengManager.NotifyAllWangChengMapInfoData();
		}

		// Token: 0x060033A8 RID: 13224 RVA: 0x002DCD1C File Offset: 0x002DAF1C
		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == WangChengManager.WangChengZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < WangChengManager.WangChengZhanWeekDays.Length; i++)
				{
					if (WangChengManager.WangChengZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060033A9 RID: 13225 RVA: 0x002DCD70 File Offset: 0x002DAF70
		public static bool IsInWangChengFightingTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			int weekDayID = (int)now.DayOfWeek;
			bool result;
			if (!WangChengManager.IsDayOfWeek(weekDayID))
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(now, WangChengManager.WangChengZhanFightingDayTimes, out endMinute, false);
			}
			return result;
		}

		// Token: 0x060033AA RID: 13226 RVA: 0x002DCDB4 File Offset: 0x002DAFB4
		public static bool IsWangChengZhanOver()
		{
			return !WangChengManager.WaitingHuangChengResult;
		}

		// Token: 0x060033AB RID: 13227 RVA: 0x002DCDD0 File Offset: 0x002DAFD0
		public static bool IsInCityWarBattling(GameClient client)
		{
			if (client.ClientData.MapCode == Global.GetHuangGongMapCode())
			{
				if (WangChengZhanStates.None != WangChengManager.WangChengZhanState)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060033AC RID: 13228 RVA: 0x002DCE10 File Offset: 0x002DB010
		public static bool IsInBattling()
		{
			return WangChengZhanStates.None != WangChengManager.WangChengZhanState;
		}

		// Token: 0x060033AD RID: 13229 RVA: 0x002DCE34 File Offset: 0x002DB034
		public static void ProcessWangChengZhanResult()
		{
			Global.UpdateWangChengZhanWeekDays(false);
			if (WangChengZhanStates.None == WangChengManager.WangChengZhanState)
			{
				if (WangChengManager.IsInWangChengFightingTime())
				{
					WangChengManager.WangChengZhanState = WangChengZhanStates.Fighting;
					WangChengManager.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
					WangChengManager.WaitingHuangChengResult = true;
					WangChengManager.NotifyAllWangChengMapInfoData();
					Global.BroadcastHuangChengBattleStart();
				}
			}
			else if (WangChengManager.IsInWangChengFightingTime())
			{
				bool ret = WangChengManager.TryGenerateNewHuangChengBangHui();
				if (ret)
				{
					WangChengManager.HandleHuangChengResultEx(false);
					WangChengManager.NotifyAllWangChengMapInfoData();
				}
				else
				{
					WangChengManager.ProcessTimeAddRoleExp();
				}
			}
			else
			{
				WangChengManager.WangChengZhanState = WangChengZhanStates.None;
				WangChengManager.WaitingHuangChengResult = false;
				WangChengManager.TryGenerateNewHuangChengBangHui();
				WangChengManager.HandleHuangChengResultEx(true);
				WangChengManager.NotifyAllWangChengMapInfoData();
			}
		}

		// Token: 0x060033AE RID: 13230 RVA: 0x002DCEE8 File Offset: 0x002DB0E8
		public static bool TryGenerateNewHuangChengBangHui()
		{
			int newBHid = WangChengManager.GetTheOnlyOneBangHui();
			bool result;
			if (newBHid <= 0 || WangChengManager.WangZuBHid == newBHid)
			{
				WangChengManager.LastTheOnlyOneBangHui = -1;
				result = false;
			}
			else if (WangChengManager.LastTheOnlyOneBangHui != newBHid)
			{
				WangChengManager.LastTheOnlyOneBangHui = newBHid;
				WangChengManager.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
				result = false;
			}
			else
			{
				if (WangChengManager.LastTheOnlyOneBangHui > 0)
				{
					long ticks = TimeUtil.NOW();
					if (ticks - WangChengManager.BangHuiTakeHuangGongTicks > (long)WangChengManager.MaxTakingHuangGongSecs)
					{
						WangChengManager.WangZuBHid = WangChengManager.LastTheOnlyOneBangHui;
						WangChengManager.UpdateWangZuBHNameFromDBServer(newBHid);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060033AF RID: 13231 RVA: 0x002DCF88 File Offset: 0x002DB188
		public static int GetTheOnlyOneBangHui()
		{
			List<GameClient> lsClients = GameManager.ClientMgr.GetMapAliveClientsEx(Global.GetHuangGongMapCode(), true);
			int newBHid = -1;
			int existBHid = -1;
			for (int i = 0; i < lsClients.Count; i++)
			{
				GameClient client = lsClients[i];
				if (existBHid != -1)
				{
					if (client.ClientData.Faction > 0 && client.ClientData.Faction != existBHid)
					{
						newBHid = -1;
						break;
					}
				}
				else if (client.ClientData.Faction > 0)
				{
					existBHid = client.ClientData.Faction;
					newBHid = existBHid;
				}
			}
			return newBHid;
		}

		// Token: 0x060033B0 RID: 13232 RVA: 0x002DD040 File Offset: 0x002DB240
		public static void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x002DD060 File Offset: 0x002DB260
		private static void HandleWangChengFailed()
		{
			JunQiManager.HandleLingDiZhanResultByMapCode(6, Global.GetHuangGongMapCode(), 0, true, false);
			Global.BroadcastWangChengFailedHint();
			JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
		}

		// Token: 0x060033B2 RID: 13234 RVA: 0x002DD080 File Offset: 0x002DB280
		private static void HandleHuangChengResultEx(bool isBattleOver = false)
		{
			if (WangChengManager.WangZuBHid <= 0)
			{
				if (isBattleOver)
				{
					WangChengManager.HandleWangChengFailed();
				}
			}
			else
			{
				JunQiManager.HandleLingDiZhanResultByMapCode(6, Global.GetHuangGongMapCode(), WangChengManager.WangZuBHid, true, false);
				Global.BroadcastHuangChengOkHintEx(WangChengManager.WangZuBHName, isBattleOver);
				JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				if (isBattleOver)
				{
					HuodongCachingMgr.UpdateHeFuWCKingBHID(WangChengManager.WangZuBHid);
				}
			}
		}

		// Token: 0x060033B3 RID: 13235 RVA: 0x002DD0E8 File Offset: 0x002DB2E8
		public static void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		// Token: 0x060033B4 RID: 13236 RVA: 0x002DD10C File Offset: 0x002DB30C
		public static WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return WangChengManager.FormatWangChengMapInfoData();
		}

		// Token: 0x060033B5 RID: 13237 RVA: 0x002DD124 File Offset: 0x002DB324
		public static WangChengMapInfoData FormatWangChengMapInfoData()
		{
			string nextBattleTime = GLang.GetLang(43, new object[0]);
			long endTime = 0L;
			if (WangChengZhanStates.None == WangChengManager.WangChengZhanState)
			{
				nextBattleTime = WangChengManager.GetNextCityBattleTime();
			}
			else
			{
				endTime = WangChengManager.GetBattleEndMs();
			}
			return new WangChengMapInfoData
			{
				FightingEndTime = endTime,
				FightingState = (WangChengManager.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = nextBattleTime,
				WangZuBHName = WangChengManager.WangZuBHName,
				WangZuBHid = WangChengManager.WangZuBHid
			};
		}

		// Token: 0x060033B6 RID: 13238 RVA: 0x002DD1AC File Offset: 0x002DB3AC
		public static Dictionary<int, int> GetWarRequstMap(string warReqString)
		{
			Dictionary<int, int> warRequstMap = new Dictionary<int, int>();
			string[] reqItems = warReqString.Split(new char[]
			{
				','
			});
			for (int i = 0; i < reqItems.Length; i++)
			{
				string[] item = reqItems[i].Split(new char[]
				{
					'_'
				});
				if (item.Length == 2)
				{
					int bhid = int.Parse(item[0]);
					int day = int.Parse(item[1]);
					if (!warRequstMap.ContainsKey(bhid))
					{
						if (day >= TimeUtil.NowDateTime().DayOfYear || day + 120 <= TimeUtil.NowDateTime().DayOfYear)
						{
							warRequstMap.Add(bhid, day);
						}
					}
				}
			}
			return warRequstMap;
		}

		// Token: 0x060033B7 RID: 13239 RVA: 0x002DD28C File Offset: 0x002DB48C
		public static string GeWarRequstString(Dictionary<int, int> warRequstMap)
		{
			string nowWarRequest = "";
			for (int i = 0; i < warRequstMap.Count; i++)
			{
				if (nowWarRequest.Length > 0)
				{
					nowWarRequest += ",";
				}
				nowWarRequest += string.Format("{0}_{1}", warRequstMap.ElementAt(i).Key, warRequstMap.ElementAt(i).Value);
			}
			return nowWarRequest;
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x002DD318 File Offset: 0x002DB518
		public static int SetCityWarRequestToDBServer(int lingDiID, string nowWarRequest)
		{
			int retCode = -200;
			string strcmd = string.Format("{0}:{1}", lingDiID, nowWarRequest);
			string[] fields = Global.ExecuteDBCmd(10098, strcmd, 0);
			int result;
			if (fields == null || fields.Length != 5)
			{
				result = retCode;
			}
			else
			{
				retCode = Global.SafeConvertToInt32(fields[0]);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				result = retCode;
			}
			return result;
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x002DD378 File Offset: 0x002DB578
		public static bool IsExistCityWarToday()
		{
			int day = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(6);
			bool result;
			if (null == lingDiItem)
			{
				result = false;
			}
			else
			{
				Dictionary<int, int> warRequestMap = WangChengManager.GetWarRequstMap(lingDiItem.WarRequest);
				result = warRequestMap.ContainsValue(day);
			}
			return result;
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x002DD3D4 File Offset: 0x002DB5D4
		public static void UpdateWangChengZhanWeekDays(int[] weekDays)
		{
			if (!WangChengManager.WangChengZhanWeekDaysByConfig)
			{
				WangChengManager.WangChengZhanWeekDays = weekDays;
			}
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x002DD3F4 File Offset: 0x002DB5F4
		protected static void RemoveTodayInWarRequest()
		{
			int day = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(6);
			if (null != lingDiItem)
			{
				Dictionary<int, int> warRequestMap = WangChengManager.GetWarRequstMap(lingDiItem.WarRequest);
				if (warRequestMap.ContainsValue(day))
				{
					for (int i = 0; i < warRequestMap.Count; i++)
					{
						if (warRequestMap.Values.ElementAt(i) == day)
						{
							warRequestMap.Remove(warRequestMap.Keys.ElementAt(i));
							break;
						}
					}
					string nowWarRequest = WangChengManager.GeWarRequstString(warRequestMap);
					WangChengManager.SetCityWarRequestToDBServer(6, nowWarRequest);
				}
			}
		}

		// Token: 0x060033BC RID: 13244 RVA: 0x002DD49C File Offset: 0x002DB69C
		public static long GetBattleEndMs()
		{
			DateTime now = TimeUtil.NowDateTime();
			int hour = now.Hour;
			int minute = now.Minute;
			int nowMinite = hour * 60 + minute;
			int endMinute = 0;
			Global.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), WangChengManager.WangChengZhanFightingDayTimes, out endMinute, true);
			return now.AddMinutes((double)Math.Max(0, endMinute - nowMinite)).Ticks / 10000L;
		}

		// Token: 0x060033BD RID: 13245 RVA: 0x002DD554 File Offset: 0x002DB754
		public static string GetNextCityBattleTime()
		{
			string unKown = GLang.GetLang(43, new object[0]);
			int day = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(6);
			string result;
			if (null == lingDiItem)
			{
				result = unKown;
			}
			else
			{
				Dictionary<int, int> warRequestMap = WangChengManager.GetWarRequstMap(lingDiItem.WarRequest);
				List<DateTime> lsDays = new List<DateTime>();
				for (int i = 0; i < warRequestMap.Count; i++)
				{
					DateTime dt = TimeUtil.NowDateTime();
					int span = warRequestMap.Values.ElementAt(i) - day;
					if (span >= 0)
					{
						dt = dt.AddDays((double)span);
					}
					else
					{
						int yearNext = dt.Year + 1;
						dt = DateTime.Parse(string.Format("{0}-01-01", yearNext)).AddDays((double)(warRequestMap.Values.ElementAt(i) - 1));
					}
					lsDays.Add(dt);
				}
				lsDays.Sort(delegate(DateTime l, DateTime r)
				{
					int result2;
					if (l.Ticks < r.Ticks)
					{
						result2 = -1;
					}
					else if (l.Ticks > r.Ticks)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				if (lsDays.Count > 0)
				{
					DateTime nextDate = lsDays[0];
					if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
					{
						return lsDays[0].ToString("yyyy-MM-dd " + string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].FromHour, WangChengManager.WangChengZhanFightingDayTimes[0].FromMinute));
					}
				}
				result = unKown;
			}
			return result;
		}

		// Token: 0x060033BE RID: 13246 RVA: 0x002DD750 File Offset: 0x002DB950
		public static bool GetNextCityBattleTimeAndBangHui(out int dayID, out int bangHuiID)
		{
			dayID = -1;
			bangHuiID = -1;
			int day = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(6);
			bool result;
			if (null == lingDiItem)
			{
				result = false;
			}
			else
			{
				Dictionary<int, int> warRequestMap = WangChengManager.GetWarRequstMap(lingDiItem.WarRequest);
				List<DateTime> lsDays = new List<DateTime>();
				for (int i = 0; i < warRequestMap.Count; i++)
				{
					DateTime dt = TimeUtil.NowDateTime();
					int span = warRequestMap.Values.ElementAt(i) - day;
					if (span >= 0)
					{
						dt = dt.AddDays((double)span);
					}
					else
					{
						int yearNext = dt.Year + 1;
						dt = DateTime.Parse(string.Format("{0}-01-01", yearNext)).AddDays((double)(warRequestMap.Values.ElementAt(i) - 1));
					}
					lsDays.Add(dt);
				}
				lsDays.Sort(delegate(DateTime l, DateTime r)
				{
					int result2;
					if (l.Ticks < r.Ticks)
					{
						result2 = -1;
					}
					else if (l.Ticks > r.Ticks)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				if (lsDays.Count > 0)
				{
					DateTime nextDate = lsDays[0];
					if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
					{
						dayID = nextDate.DayOfYear;
						for (int i = 0; i < warRequestMap.Count; i++)
						{
							if (dayID == warRequestMap.Values.ElementAt(i))
							{
								bangHuiID = warRequestMap.Keys.ElementAt(i);
								return true;
							}
						}
						return false;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060033BF RID: 13247 RVA: 0x002DD908 File Offset: 0x002DBB08
		public static bool GetNextCityBattleTimeAndBangHui(out string dayTime, out string bangHuiName)
		{
			dayTime = GLang.GetLang(43, new object[0]);
			bangHuiName = GLang.GetLang(568, new object[0]);
			int warDay;
			int bangHuiID;
			return WangChengManager.GetNextCityBattleTimeAndBangHui(out warDay, out bangHuiID) && WangChengManager.GetWarTimeStringAndBHName(warDay, bangHuiID, out dayTime, out bangHuiName);
		}

		// Token: 0x060033C0 RID: 13248 RVA: 0x002DD9A4 File Offset: 0x002DBBA4
		public static string GetCityBattleTimeAndBangHuiListString()
		{
			string result;
			if (WangChengManager.WangChengZhanFightingDayTimes == null || WangChengManager.WangChengZhanFightingDayTimes.Length <= 0)
			{
				result = "";
			}
			else
			{
				int day = TimeUtil.NowDateTime().DayOfYear;
				BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(6);
				if (null == lingDiItem)
				{
					result = "";
				}
				else
				{
					Dictionary<int, int> warRequestMap = WangChengManager.GetWarRequstMap(lingDiItem.WarRequest);
					List<DateTime> lsDays = new List<DateTime>();
					for (int i = 0; i < warRequestMap.Count; i++)
					{
						DateTime dt = TimeUtil.NowDateTime();
						int span = warRequestMap.Values.ElementAt(i) - day;
						if (span >= 0)
						{
							dt = dt.AddDays((double)span);
						}
						else
						{
							int yearNext = dt.Year + 1;
							dt = DateTime.Parse(string.Format("{0}-01-01", yearNext)).AddDays((double)(warRequestMap.Values.ElementAt(i) - 1));
						}
						lsDays.Add(dt);
					}
					lsDays.Sort(delegate(DateTime l, DateTime r)
					{
						int result2;
						if (l.Ticks < r.Ticks)
						{
							result2 = -1;
						}
						else if (l.Ticks > r.Ticks)
						{
							result2 = 1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					string timeBangHuiString = "";
					int index = 0;
					while (index < lsDays.Count && index < 10)
					{
						int dayID = lsDays[index].DayOfYear;
						for (int i = 0; i < warRequestMap.Count; i++)
						{
							if (dayID == warRequestMap.Values.ElementAt(i))
							{
								int bangHuiID = warRequestMap.Keys.ElementAt(i);
								string strTime;
								string strBH;
								WangChengManager.GetWarTimeStringAndBHName(dayID, bangHuiID, out strTime, out strBH);
								if (timeBangHuiString.Length > 0)
								{
									timeBangHuiString += ",";
								}
								timeBangHuiString += string.Format("{0},{1}", strTime, strBH);
								break;
							}
						}
						index++;
					}
					result = timeBangHuiString;
				}
			}
			return result;
		}

		// Token: 0x060033C1 RID: 13249 RVA: 0x002DDBC4 File Offset: 0x002DBDC4
		private static bool GetWarTimeStringAndBHName(int warDay, int bangHuiID, out string dayTime, out string bangHuiName)
		{
			dayTime = GLang.GetLang(43, new object[0]);
			bangHuiName = GLang.GetLang(568, new object[0]);
			BangHuiMiniData bhData = Global.GetBangHuiMiniData(bangHuiID, 0);
			if (null != bhData)
			{
				bangHuiName = bhData.BHName;
			}
			int day = TimeUtil.NowDateTime().DayOfYear;
			DateTime dt = TimeUtil.NowDateTime();
			int span = warDay - day;
			if (span >= 0)
			{
				dt = dt.AddDays((double)span);
			}
			else
			{
				int yearNext = dt.Year + 1;
				dt = DateTime.Parse(string.Format("{0}-01-01", yearNext)).AddDays((double)(warDay - 1));
			}
			if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
			{
				string dayTime2 = dt.ToString("yyyy-MM-dd " + string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].FromHour, WangChengManager.WangChengZhanFightingDayTimes[0].FromMinute));
				string dayTime3 = string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].EndHour, WangChengManager.WangChengZhanFightingDayTimes[0].EndMinute);
				dayTime = string.Format(GLang.GetLang(569, new object[0]), dayTime2, dayTime3);
			}
			else
			{
				dayTime = dt.ToString("yyyy-MM-dd");
			}
			return true;
		}

		// Token: 0x060033C2 RID: 13250 RVA: 0x002DDD34 File Offset: 0x002DBF34
		private static void ProcessTimeAddRoleExp()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - WangChengManager.LastAddBangZhanAwardsTicks >= 10000L)
			{
				WangChengManager.LastAddBangZhanAwardsTicks = ticks;
				List<object> objsList = GameManager.ClientMgr.GetMapClients(Global.GetHuangGongMapCode());
				if (null != objsList)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (c != null)
						{
							BangZhanAwardsMgr.ProcessBangZhanAwards(c);
						}
					}
				}
			}
		}

		// Token: 0x04003F52 RID: 16210
		private static bool WaitingHuangChengResult = false;

		// Token: 0x04003F53 RID: 16211
		private static long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		// Token: 0x04003F54 RID: 16212
		private static string WangZuBHName = "";

		// Token: 0x04003F55 RID: 16213
		private static int WangZuBHid = -1;

		// Token: 0x04003F56 RID: 16214
		public static object ApplyWangChengWarMutex = new object();

		// Token: 0x04003F57 RID: 16215
		private static int MaxTakingHuangGongSecs = 1200;

		// Token: 0x04003F58 RID: 16216
		private static bool WangChengZhanWeekDaysByConfig = false;

		// Token: 0x04003F59 RID: 16217
		private static int[] WangChengZhanWeekDays = null;

		// Token: 0x04003F5A RID: 16218
		private static DateTimeRange[] WangChengZhanFightingDayTimes = null;

		// Token: 0x04003F5B RID: 16219
		public static WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		// Token: 0x04003F5C RID: 16220
		private static int LastTheOnlyOneBangHui = -1;

		// Token: 0x04003F5D RID: 16221
		private static long LastAddBangZhanAwardsTicks = 0L;
	}
}
