using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class WangChengManager
	{
		
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

		
		public static int GetWangZuBHid()
		{
			return WangChengManager.WangZuBHid;
		}

		
		public static string GetWangZuBHName()
		{
			return WangChengManager.WangZuBHName;
		}

		
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

		
		public static bool IsWangChengZhanOver()
		{
			return !WangChengManager.WaitingHuangChengResult;
		}

		
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

		
		public static bool IsInBattling()
		{
			return WangChengZhanStates.None != WangChengManager.WangChengZhanState;
		}

		
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

		
		public static void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		
		private static void HandleWangChengFailed()
		{
			JunQiManager.HandleLingDiZhanResultByMapCode(6, Global.GetHuangGongMapCode(), 0, true, false);
			Global.BroadcastWangChengFailedHint();
			JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
		}

		
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

		
		public static void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		
		public static WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return WangChengManager.FormatWangChengMapInfoData();
		}

		
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

		
		public static void UpdateWangChengZhanWeekDays(int[] weekDays)
		{
			if (!WangChengManager.WangChengZhanWeekDaysByConfig)
			{
				WangChengManager.WangChengZhanWeekDays = weekDays;
			}
		}

		
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

		
		public static bool GetNextCityBattleTimeAndBangHui(out string dayTime, out string bangHuiName)
		{
			dayTime = GLang.GetLang(43, new object[0]);
			bangHuiName = GLang.GetLang(568, new object[0]);
			int warDay;
			int bangHuiID;
			return WangChengManager.GetNextCityBattleTimeAndBangHui(out warDay, out bangHuiID) && WangChengManager.GetWarTimeStringAndBHName(warDay, bangHuiID, out dayTime, out bangHuiName);
		}

		
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

		
		private static bool WaitingHuangChengResult = false;

		
		private static long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		
		private static string WangZuBHName = "";

		
		private static int WangZuBHid = -1;

		
		public static object ApplyWangChengWarMutex = new object();

		
		private static int MaxTakingHuangGongSecs = 1200;

		
		private static bool WangChengZhanWeekDaysByConfig = false;

		
		private static int[] WangChengZhanWeekDays = null;

		
		private static DateTimeRange[] WangChengZhanFightingDayTimes = null;

		
		public static WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		
		private static int LastTheOnlyOneBangHui = -1;

		
		private static long LastAddBangZhanAwardsTicks = 0L;
	}
}
