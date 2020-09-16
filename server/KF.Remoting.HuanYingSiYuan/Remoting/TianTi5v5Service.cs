using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace Remoting
{
	
	public class TianTi5v5Service : IDisposable
	{
		
		public void Dispose()
		{
		}

		
		private static int ZhanDuiDataSortCompare(TianTi5v5ZhanDuiData x, TianTi5v5ZhanDuiData y)
		{
			int ret = y.DuanWeiJiFen - x.DuanWeiJiFen;
			int result;
			if (ret != 0)
			{
				result = ret;
			}
			else
			{
				result = x.ZhanDuiID - y.ZhanDuiID;
			}
			return result;
		}

		
		public static bool InitConfig()
		{
			TianTi5v5Service.RankData.MaxPaiMingRank = 201;
			try
			{
				string fileName = "Config/TeamDuanWeiAward.xml";
				string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
				XElement xml = ConfigHelper.Load(fullPathFileName);
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement node in nodes)
				{
					int StarRank = (int)ConfigHelper.GetElementAttributeValueLong(node, "StarRank", 201L);
					TianTi5v5Service.RankData.MaxPaiMingRank = Math.Max(TianTi5v5Service.RankData.MaxPaiMingRank, StarRank);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			Dictionary<int, TianTi5v5ZhanDuiData> dict = new Dictionary<int, TianTi5v5ZhanDuiData>();
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.LoadZhanDuiRankData(TianTi5v5Service.RankData, TimeUtil.NowDateTime());
				if (!TianTi5v5Service.Persistence.LoadZhanDuiData(dict))
				{
					return false;
				}
				TianTi5v5Service.ZhanDuiDict = dict;
			}
			TianTi5v5Service.pTianTiPiPeiCfg = new List<TianTi5v5Service.PiPeiCfg>
			{
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 0,
					MinTime = 0,
					MaxTime = 5
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 1,
					MinTime = 5,
					MaxTime = 10
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 3,
					MinTime = 10,
					MaxTime = 15
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 25,
					MinTime = 15,
					MaxTime = 60
				}
			};
			return true;
		}

		
		public static void LoadZhanDuiRankData(TianTi5v5RankData rankData, DateTime now)
		{
			try
			{
				DateTime modifyDate = now;
				List<TianTi5v5ZhanDuiData> dayList = new List<TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> monthList = new List<TianTi5v5ZhanDuiData>();
				int dayID = TimeUtil.GetOffsetDay(now);
				int monthDayID = TimeUtil.GetOffsetMonthDayID(now);
				TianTi5v5Service.Persistence.LoadZhanDuiRankList(monthList, monthDayID);
				if (now.Day != 1)
				{
					TianTi5v5Service.Persistence.LoadZhanDuiRankList(dayList, dayID);
				}
				lock (TianTi5v5Service.Mutex)
				{
					rankData.ModifyTime = modifyDate;
					rankData.DayPaiHangList = dayList;
					rankData.MonthPaiHangList = monthList;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static void UpdateZhanDuiRankData(DateTime now, bool forceUpdateMonthRank = false)
		{
			bool updateMonthRank = now.Day == 1 || forceUpdateMonthRank;
			int dayID = TimeUtil.GetOffsetDay(now);
			DateTime monthStartDateTime = new DateTime(now.Year, now.Month, 1);
			if (now.Day == 1)
			{
				monthStartDateTime = monthStartDateTime.AddMonths(-1);
			}
			List<TianTi5v5ZhanDuiData> list = TianTi5v5Service.ZhanDuiDict.Values.ToList<TianTi5v5ZhanDuiData>();
			list.RemoveAll((TianTi5v5ZhanDuiData x) => x.LastFightTime < monthStartDateTime || x.DuanWeiJiFen == 0);
			list.Sort(new Comparison<TianTi5v5ZhanDuiData>(TianTi5v5Service.ZhanDuiDataSortCompare));
			int maxRankCount = TianTi5v5Service.RankData.MaxPaiMingRank;
			List<TianTi5v5ZhanDuiData> rankList = new List<TianTi5v5ZhanDuiData>();
			for (int i = 0; i < list.Count; i++)
			{
				TianTi5v5ZhanDuiData data = list[i];
				if (i < maxRankCount)
				{
					data.DuanWeiRank = i + 1;
					if (updateMonthRank)
					{
						data.MonthDuanWeiRank = data.DuanWeiRank;
					}
					rankList.Add(data.Clone());
					if (updateMonthRank)
					{
						data.DuanWeiRank = maxRankCount + 1;
					}
				}
				else
				{
					data.DuanWeiRank = maxRankCount + 1;
					if (updateMonthRank)
					{
						data.MonthDuanWeiRank = data.DuanWeiRank;
					}
				}
			}
			TianTi5v5Service.Persistence.UpdateZhanDuiRankData(list, dayID, TianTi5v5Service.RankData.MaxPaiMingRank, updateMonthRank);
			TianTi5v5Service.Persistence.UpdateZhanDuiDayRank(rankList, dayID, TianTi5v5Service.RankData.MaxPaiMingRank, updateMonthRank);
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.RankData.ModifyTime = now;
				if (now.Day == 1)
				{
					TianTi5v5Service.RankData.MonthPaiHangList = rankList;
					TianTi5v5Service.RankData.DayPaiHangList = new List<TianTi5v5ZhanDuiData>();
				}
				else
				{
					TianTi5v5Service.RankData.DayPaiHangList = list;
				}
			}
		}

		
		public static void PaiHangCopy(DateTime now)
		{
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.RankData.ModifyTime = now;
				TianTi5v5Service.RankData.MonthPaiHangList = TianTi5v5Service.RankData.DayPaiHangList;
			}
		}

		
		public static int CreateZhanDui(int serverID, TianTi5v5ZhanDuiData pData)
		{
			int ret2 = pData.ZhanDuiID;
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -3;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					if (pData.ZhanDuiID == 0)
					{
						if (TianTi5v5Service.ZhanDuiDict.Values.Any((TianTi5v5ZhanDuiData x) => StringComparer.CurrentCultureIgnoreCase.Compare(x.ZhanDuiName, pData.ZhanDuiName) == 0))
						{
							return -4023;
						}
						ret2 = TianTi5v5Service.Persistence.InitZhanDui(pData);
						if (ret2 <= 0)
						{
							return -15;
						}
						pData.ZhanDuiID = ret2;
						TianTi5v5Service.ZhanDuiDict[pData.ZhanDuiID] = pData;
						return ret2;
					}
					else
					{
						TianTi5v5Service.Persistence.UpdateZhanDui(pData);
						TianTi5v5Service.ZhanDuiDict[pData.ZhanDuiID] = pData;
					}
				}
				result = ret2;
			}
			return result;
		}

		
		public static int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -1;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					TianTi5v5ZhanDuiData zhanDuiData;
					if (!TianTi5v5Service.ZhanDuiDict.TryGetValue((int)teamID, out zhanDuiData))
					{
						return -15;
					}
					zhanDuiData.XuanYan = xuanYan;
				}
				TianTi5v5Service.Persistence.UpdateZhanDuiXuanYan(teamID, xuanYan);
				result = 0;
			}
			return result;
		}

		
		public static int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			int result = 0;
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData curZhanDui;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(data.ZhanDuiID, out curZhanDui))
				{
					if (modeType == ZhanDuiDataModeTypes.ZhanDuiInfo)
					{
						curZhanDui.ZhanDuiID = data.ZhanDuiID;
						curZhanDui.XuanYan = data.XuanYan;
						curZhanDui.ZhanDuiName = data.ZhanDuiName;
						curZhanDui.LeaderRoleID = data.LeaderRoleID;
						curZhanDui.ZhanDouLi = data.ZhanDouLi;
						curZhanDui.teamerList = data.teamerList;
						curZhanDui.TeamerRidList = data.TeamerRidList;
						curZhanDui.LeaderRoleName = data.LeaderRoleName;
						curZhanDui.ZoneID = data.ZoneID;
					}
					else if (modeType == ZhanDuiDataModeTypes.TianTiFightData)
					{
						curZhanDui.DuanWeiId = data.DuanWeiId;
						curZhanDui.DuanWeiJiFen = data.DuanWeiJiFen;
						curZhanDui.DuanWeiRank = data.DuanWeiRank;
						curZhanDui.ZhanDouLi = data.ZhanDouLi;
						curZhanDui.LianSheng = data.LianSheng;
						curZhanDui.SuccessCount = data.SuccessCount;
						curZhanDui.FightCount = data.FightCount;
						curZhanDui.MonthDuanWeiRank = data.MonthDuanWeiRank;
						curZhanDui.LastFightTime = data.LastFightTime;
						using (List<TianTi5v5ZhanDuiRoleData>.Enumerator enumerator = curZhanDui.teamerList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TianTi5v5ZhanDuiRoleData role = enumerator.Current;
								TianTi5v5ZhanDuiRoleData newRoleInfo = data.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == role.RoleID);
								if (null != newRoleInfo)
								{
									role.MonthFightCounts = newRoleInfo.MonthFightCounts;
									role.TodayFightCount = newRoleInfo.TodayFightCount;
									role.MonthFigntCount = newRoleInfo.MonthFigntCount;
									role.ZhanLi = newRoleInfo.ZhanLi;
									role.RoleOcc = newRoleInfo.RoleOcc;
									role.ZhuanSheng = newRoleInfo.ZhuanSheng;
									role.Level = newRoleInfo.Level;
									role.RebornLevel = newRoleInfo.RebornLevel;
									role.ModelData = newRoleInfo.ModelData;
								}
							}
						}
					}
					result = TianTi5v5Service.Persistence.UpdateZhanDui(curZhanDui);
				}
			}
			return result;
		}

		
		public static void UpdateZorkZhanDuiData(TianTi5v5ZhanDuiData data)
		{
			TianTi5v5Service.Persistence.UpdateZorkZhanDui(data);
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData curZhanDui;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(data.ZhanDuiID, out curZhanDui))
				{
					curZhanDui.ZorkJiFen = data.ZorkJiFen;
					curZhanDui.ZorkLastFightTime = data.ZorkLastFightTime;
				}
			}
		}

		
		public static TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID)
		{
			TianTi5v5ZhanDuiData data = null;
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out data);
			}
			return data;
		}

		
		public static void CalZorkBattleRankTeamJiFen(List<KFZorkRankInfo> rankList)
		{
			lock (TianTi5v5Service.Mutex)
			{
				List<TianTi5v5ZhanDuiData> zhanduiList = TianTi5v5Service.ZhanDuiDict.Values.ToList<TianTi5v5ZhanDuiData>();
				if (zhanduiList.Count != 0)
				{
					zhanduiList.RemoveAll((TianTi5v5ZhanDuiData x) => x.ZorkJiFen == 0);
					zhanduiList.Sort(delegate(TianTi5v5ZhanDuiData left, TianTi5v5ZhanDuiData right)
					{
						int result;
						if (left.ZorkJiFen > right.ZorkJiFen)
						{
							result = -1;
						}
						else if (left.ZorkJiFen < right.ZorkJiFen)
						{
							result = 1;
						}
						else if (left.ZorkLastFightTime > right.ZorkLastFightTime)
						{
							result = 1;
						}
						else if (left.ZorkLastFightTime < right.ZorkLastFightTime)
						{
							result = -1;
						}
						else if (left.ZhanDuiID > right.ZhanDuiID)
						{
							result = -1;
						}
						else if (left.ZhanDuiID < right.ZhanDuiID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					zhanduiList = zhanduiList.GetRange(0, Math.Min(30, zhanduiList.Count));
					foreach (TianTi5v5ZhanDuiData item in zhanduiList)
					{
						KFZorkRankInfo rankinfo = new KFZorkRankInfo
						{
							Key = item.ZhanDuiID,
							Value = item.ZorkJiFen,
							StrParam1 = KuaFuServerManager.FormatName(item.ZoneID, item.ZhanDuiName)
						};
						rankList.Add(rankinfo);
					}
				}
			}
		}

		
		public static void ClearAllZhanDuiZorkData()
		{
			lock (TianTi5v5Service.Mutex)
			{
				foreach (TianTi5v5ZhanDuiData zhandui in TianTi5v5Service.ZhanDuiDict.Values)
				{
					if (zhandui.ZorkJiFen > 0)
					{
						zhandui.ZorkJiFen = 0;
						TianTi5v5Service.Persistence.UpdateZorkZhanDui(zhandui);
					}
				}
				TianTi5v5Service.Persistence.ClearZorkBattleRoleData();
			}
		}

		
		public static int DeleteZhanDui(int serverID, int roleid, int zhanDuiID)
		{
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -100;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					TianTi5v5ZhanDuiData zhanDuiData;
					if (!TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out zhanDuiData))
					{
						result = 0;
					}
					else
					{
						int ret = TianTi5v5Service.Persistence.DeleteZhanDui((long)zhanDuiID);
						if (ret >= 0)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("DeleteZhanDui,zhanduiid={0},roleid={1},teamName={2}", zhanDuiID, roleid, zhanDuiData.ZhanDuiName), null, true);
							TianTi5v5Service.ZhanDuiDict.Remove(zhanDuiID);
						}
						result = ret;
					}
				}
			}
			return result;
		}

		
		public static TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			TianTi5v5RankData tianTiRankData = new TianTi5v5RankData();
			lock (TianTi5v5Service.Mutex)
			{
				tianTiRankData.ModifyTime = TianTi5v5Service.RankData.ModifyTime;
				tianTiRankData.MaxPaiMingRank = TianTi5v5Service.RankData.MaxPaiMingRank;
				if (modifyTime < TianTi5v5Service.RankData.ModifyTime && null != TianTi5v5Service.RankData.DayPaiHangList)
				{
					tianTiRankData.DayPaiHangList = new List<TianTi5v5ZhanDuiData>(TianTi5v5Service.RankData.DayPaiHangList);
				}
				if (modifyTime < TianTi5v5Service.RankData.ModifyTime && null != TianTi5v5Service.RankData.MonthPaiHangList)
				{
					tianTiRankData.MonthPaiHangList = new List<TianTi5v5ZhanDuiData>(TianTi5v5Service.RankData.MonthPaiHangList);
				}
			}
			return tianTiRankData;
		}

		
		public static List<int> GetZhanDuiMemberIDs(int zhanDuiID)
		{
			List<int> roleList = new List<int>();
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData data;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out data))
				{
					roleList = data.teamerList.ConvertAll<int>((TianTi5v5ZhanDuiRoleData x) => x.RoleID);
				}
			}
			return roleList;
		}

		
		public static int ZhanDuiRoleSignUp(int serverId, int gameType, int zhanDuiID, long zhanLi = 1L, int groupIndex = 1)
		{
			int result = 1;
			Lazy<KF5v5PiPeiTeam> lazy = new Lazy<KF5v5PiPeiTeam>(() => new KF5v5PiPeiTeam
			{
				TeamID = zhanDuiID,
				GroupIndex = groupIndex,
				ServerID = serverId,
				ZhanDouLi = zhanLi,
				StateEndTicks = TimeUtil.NOW()
			});
			KF5v5PiPeiTeam piPeiTeam = TianTi5v5Service.PiPeiDict.GetOrAdd(zhanDuiID, (int x) => lazy.Value);
			int oldGameId = 0;
			lock (piPeiTeam)
			{
				oldGameId = piPeiTeam.GameId;
				piPeiTeam.GameId = 0;
				piPeiTeam.State = 1;
				piPeiTeam.ServerID = serverId;
				piPeiTeam.ZhanDouLi = zhanLi;
				piPeiTeam.GroupIndex = groupIndex;
				piPeiTeam.StateEndTicks = Global.NowTime.Ticks;
				int rndSecs = Global.GetRandomNumber(0, 10);
				LogManager.WriteLog(LogTypes.Info, string.Format("组队竞技战队随机延时,zhanduiid={0}, duanwei={1},time={2}", zhanDuiID, groupIndex, rndSecs), null, true);
			}
			if (oldGameId > 0)
			{
				TianTi5v5Service.RemoveRoleFromFuBen(oldGameId, zhanDuiID);
			}
			return result;
		}

		
		public static int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID)
		{
			KF5v5PiPeiTeam piPeiTeam;
			if (TianTi5v5Service.PiPeiDict.TryGetValue(zhanDuiID, out piPeiTeam))
			{
				if (state == 6 || state == 0)
				{
					int oldGameId = 0;
					lock (piPeiTeam)
					{
						oldGameId = piPeiTeam.GameId;
						piPeiTeam.GameId = 0;
						piPeiTeam.State = 0;
						piPeiTeam.ServerID = serverId;
						piPeiTeam.StateEndTicks = Global.NowTime.Ticks;
					}
					if (oldGameId > 0)
					{
						TianTi5v5Service.RemoveRoleFromFuBen(oldGameId, zhanDuiID);
					}
				}
			}
			return 0;
		}

		
		public static KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			KuaFu5v5FuBenData kuaFuFuBenData = null;
			KuaFu5v5FuBenData result;
			if (TianTi5v5Service.FuBenDataDict.TryGetValue(gameId, out kuaFuFuBenData) && kuaFuFuBenData.State < GameFuBenState.End)
			{
				result = kuaFuFuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public static void ClearRolePairFightCount()
		{
			lock (TianTi5v5Service.RolePairFightCountDict)
			{
				TianTi5v5Service.RolePairFightCountDict.Clear();
			}
		}

		
		public static void AddRolePairFightCount(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			int roleId = 0;
			int roleId2 = 0;
			if (KuaFu5v5FuBenData.ZhanDuiDict.Count >= 2)
			{
				foreach (int id in KuaFu5v5FuBenData.ZhanDuiDict.Keys)
				{
					if (roleId == 0)
					{
						roleId = id;
					}
					else
					{
						roleId2 = id;
					}
				}
				long rolePairKey = ListExt.MakeRolePairKey(roleId, roleId2);
				lock (TianTi5v5Service.RolePairFightCountDict)
				{
					int fightCount;
					if (!TianTi5v5Service.RolePairFightCountDict.TryGetValue(rolePairKey, out fightCount))
					{
						TianTi5v5Service.RolePairFightCountDict[rolePairKey] = 1;
					}
					else
					{
						TianTi5v5Service.RolePairFightCountDict[rolePairKey] = fightCount + 1;
					}
				}
			}
		}

		
		public static bool CanAddFuBenRole(int x, int y)
		{
			long rolePairKey = ListExt.MakeRolePairKey(x, y);
			lock (TianTi5v5Service.RolePairFightCountDict)
			{
				int fightCount;
				if (!TianTi5v5Service.RolePairFightCountDict.TryGetValue(rolePairKey, out fightCount) || fightCount < TianTi5v5Service.Persistence.MaxRolePairFightCount)
				{
					return true;
				}
			}
			return false;
		}

		
		public static RangeKey GetAssignRange(int baseValue, long startTicks, long waitTicks1, long waitTicks3, long waitTicksAll)
		{
			int rangeIndex;
			if (startTicks > waitTicks3)
			{
				if (startTicks > waitTicks1)
				{
					rangeIndex = 0;
				}
				else
				{
					rangeIndex = 1;
				}
			}
			else if (startTicks > waitTicksAll)
			{
				rangeIndex = 2;
			}
			else
			{
				rangeIndex = 3;
			}
			int expend = TianTi5v5Service.AssignRangeArray[rangeIndex];
			return new RangeKey(baseValue - expend, baseValue + expend, null);
		}

		
		public static void ThreadProc(object state)
		{
			if (TianTi5v5Service.ThreadInit)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					if (now > TianTi5v5Service.CheckRoleTimerProcTime)
					{
						TianTi5v5Service.CheckRoleTimerProcTime = now.AddSeconds(1.428);
						int signUpCnt;
						int startCnt;
						TianTi5v5Service.CheckRoleTimerProc(now, out signUpCnt, out startCnt);
						ClientAgentManager.Instance().SetGameTypeLoad(TianTi5v5Service.GameType, signUpCnt, startCnt);
					}
					if (now > TianTi5v5Service.SaveServerStateProcTime)
					{
						TianTi5v5Service.SaveServerStateProcTime = now.AddSeconds(30.0);
						if (now.Hour >= 3 && now.Hour < 4)
						{
							TianTi5v5Service.ClearRolePairFightCount();
							TianTi5v5Service.UpdateZhanDuiRankData(now, false);
							ZhanDuiZhengBa_K.LoadSyncData(now, false);
						}
					}
					if (now > TianTi5v5Service.CheckGameFuBenTime)
					{
						TianTi5v5Service.CheckGameFuBenTime = now.AddSeconds(1000.0);
						TianTi5v5Service.CheckGameFuBenTimerProc(now);
					}
					TianTi5v5Service.Persistence.DelayWriteDataProc();
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		
		private static void CheckRoleTimerProc(DateTime now, out int signUpCnt, out int startCount)
		{
			signUpCnt = 0;
			startCount = 0;
			bool assgionGameFuBen = true;
			long maxRemoveRoleTicks = now.AddHours(-1.0).Ticks;
			long waitTicks = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecs1)).Ticks;
			long waitTicks2 = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecs3)).Ticks;
			long waitTicksAll = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecsAll)).Ticks;
			long waitTicksMax = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.WaitForJoinMaxSecs)).Ticks;
			TianTi5v5Service.ProcessPiPeiList.Clear();
			List<int> removeList = new List<int>();
			foreach (KF5v5PiPeiTeam kuaFuRoleData in TianTi5v5Service.PiPeiDict.Values)
			{
				int oldGameId = 0;
				lock (kuaFuRoleData)
				{
					if (kuaFuRoleData.State == 0 || kuaFuRoleData.State > 5)
					{
						if (kuaFuRoleData.StateEndTicks < maxRemoveRoleTicks)
						{
							kuaFuRoleData.State = 0;
							removeList.Add(kuaFuRoleData.TeamID);
							continue;
						}
					}
					else if (kuaFuRoleData.State == 3 || kuaFuRoleData.State == 4)
					{
						if (kuaFuRoleData.StateEndTicks < now.Ticks)
						{
							kuaFuRoleData.State = 0;
							removeList.Add(kuaFuRoleData.TeamID);
							oldGameId = kuaFuRoleData.GameId;
						}
					}
					else if (kuaFuRoleData.State == 1)
					{
						if (kuaFuRoleData.StateEndTicks < waitTicksMax)
						{
							kuaFuRoleData.State = 0;
							removeList.Add(kuaFuRoleData.TeamID);
							continue;
						}
					}
				}
				if (kuaFuRoleData.State == 1)
				{
					signUpCnt++;
					if (assgionGameFuBen)
					{
						RangeKey range = TianTi5v5Service.GetAssignRange(kuaFuRoleData.GroupIndex, kuaFuRoleData.StateEndTicks, waitTicks, waitTicks2, waitTicksAll);
						assgionGameFuBen = TianTi5v5Service.AssignGameFuben(kuaFuRoleData, range, now);
					}
				}
				else if (kuaFuRoleData.State == 2)
				{
					signUpCnt++;
				}
				else if (kuaFuRoleData.State == 5)
				{
					startCount++;
				}
				if (oldGameId > 0)
				{
					TianTi5v5Service.RemoveRoleFromFuBen(oldGameId, kuaFuRoleData.TeamID);
				}
			}
			foreach (int id in removeList)
			{
				KF5v5PiPeiTeam kuaFuRoleDataTemp;
				if (TianTi5v5Service.PiPeiDict.TryRemove(id, out kuaFuRoleDataTemp))
				{
					if (kuaFuRoleDataTemp.State == 1)
					{
						TianTi5v5Service.PiPeiDict.TryAdd(id, kuaFuRoleDataTemp);
					}
				}
			}
		}

		
		public static bool AssignGameFuben(KF5v5PiPeiTeam kuaFuRoleData, RangeKey range, DateTime now)
		{
			int roleCount = 0;
			DateTime stateEndTime = now.AddSeconds((double)TianTi5v5Service.EnterGameSecs);
			List<KuaFuFuBenRoleData> updateRoleDataList = new List<KuaFuFuBenRoleData>();
			KuaFu5v5FuBenData KuaFu5v5FuBenData = new KuaFu5v5FuBenData();
			int side = 0;
			int removeZhanDuiIDFromPiPeiList = 0;
			KF5v5PiPeiTeam team = kuaFuRoleData;
			if (Consts.TianTiRoleCountTotal > 1)
			{
				foreach (Tuple<KF5v5PiPeiTeam, int, int, int> z in TianTi5v5Service.ProcessPiPeiList)
				{
					if (z.Item2 >= range.Left && z.Item2 <= range.Right)
					{
						if (kuaFuRoleData.GroupIndex >= z.Item3 && kuaFuRoleData.GroupIndex <= z.Item4)
						{
							if (TianTi5v5Service.CanAddFuBenRole(kuaFuRoleData.TeamID, z.Item1.TeamID))
							{
								removeZhanDuiIDFromPiPeiList = z.Item1.TeamID;
								team = z.Item1;
								if (KuaFu5v5FuBenData.AddZhanDui(team.TeamID, ref roleCount, ref side))
								{
									TianTi5v5ZhanDuiData teamData;
									if (TianTi5v5Service.ZhanDuiDict.TryGetValue(team.TeamID, out teamData))
									{
										foreach (TianTi5v5ZhanDuiRoleData role in teamData.teamerList)
										{
											KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
											{
												ServerId = team.ServerID,
												RoleId = role.RoleID,
												Side = side
											};
											KuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, team.TeamID);
										}
									}
								}
							}
						}
					}
				}
				if (removeZhanDuiIDFromPiPeiList == 0)
				{
					TianTi5v5Service.ProcessPiPeiList.Add(new Tuple<KF5v5PiPeiTeam, int, int, int>(kuaFuRoleData, kuaFuRoleData.GroupIndex, range.Left, range.Right));
					return true;
				}
				TianTi5v5Service.ProcessPiPeiList.RemoveAll((Tuple<KF5v5PiPeiTeam, int, int, int> x) => x.Item1.TeamID == removeZhanDuiIDFromPiPeiList);
				TianTi5v5Service.ProcessPiPeiList.RemoveAll((Tuple<KF5v5PiPeiTeam, int, int, int> x) => x.Item1.TeamID == kuaFuRoleData.TeamID);
			}
			team = kuaFuRoleData;
			if (KuaFu5v5FuBenData.AddZhanDui(team.TeamID, ref roleCount, ref side))
			{
				TianTi5v5ZhanDuiData teamData;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(team.TeamID, out teamData))
				{
					foreach (TianTi5v5ZhanDuiRoleData role in teamData.teamerList)
					{
						KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
						{
							ServerId = team.ServerID,
							RoleId = role.RoleID,
							Side = side
						};
						KuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, team.TeamID);
					}
				}
			}
			try
			{
				int kfSrvId = 0;
				int gameId = TianTi5v5Service.Persistence.GetNextGameId();
				bool createSuccess = ClientAgentManager.Instance().AssginKfFuben(TianTi5v5Service.GameType, (long)gameId, roleCount, out kfSrvId);
				if (createSuccess)
				{
					KuaFu5v5FuBenData.ServerId = kfSrvId;
					KuaFu5v5FuBenData.GameId = gameId;
					KuaFu5v5FuBenData.GameType = (int)TianTi5v5Service.GameType;
					KuaFu5v5FuBenData.EndTime = Global.NowTime.AddMinutes(8.0);
					KuaFu5v5FuBenData.LoginInfo = KuaFuServerManager.GetKuaFuLoginInfo(kuaFuRoleData.ServerID, kfSrvId);
					TianTi5v5Service.AddGameFuBen(KuaFu5v5FuBenData);
					TianTi5v5Service.Persistence.LogCreateTianTiFuBen(KuaFu5v5FuBenData.GameId, KuaFu5v5FuBenData.ServerId, 0, roleCount);
					foreach (int zhanDuiID in KuaFu5v5FuBenData.ZhanDuiDict.Keys)
					{
						KF5v5PiPeiTeam kuaFuRoleDataTemp;
						if (TianTi5v5Service.PiPeiDict.TryGetValue(zhanDuiID, out kuaFuRoleDataTemp))
						{
							kuaFuRoleDataTemp.State = 3;
							kuaFuRoleDataTemp.StateEndTicks = stateEndTime.Ticks;
							kuaFuRoleDataTemp.GameId = KuaFu5v5FuBenData.GameId;
						}
					}
					KuaFu5v5FuBenData.State = GameFuBenState.Start;
					TianTi5v5Service.NotifyFuBenRoleEnterGame(KuaFu5v5FuBenData);
					TianTi5v5Service.AddRolePairFightCount(KuaFu5v5FuBenData);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		
		public static void NotifyFuBenRoleEnterGame(KuaFu5v5FuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					List<int> serverIDs = new List<int>();
					foreach (KuaFuFuBenRoleData role in fuBenData.RoleDict.Values)
					{
						if (!serverIDs.Contains(role.ServerId))
						{
							serverIDs.Add(role.ServerId);
							AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.KuaFu5v5UpdateAndNotifyEnterGame, new object[]
							{
								fuBenData
							});
							ClientAgentManager.Instance().PostAsyncEvent(role.ServerId, TianTi5v5Service.ChannelGameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		public static void AddGameFuBen(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			TianTi5v5Service.FuBenDataDict[KuaFu5v5FuBenData.GameId] = KuaFu5v5FuBenData;
		}

		
		public static void RemoveGameFuBen(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			int gameId = KuaFu5v5FuBenData.GameId;
			KuaFu5v5FuBenData KuaFu5v5FuBenDataTemp;
			if (TianTi5v5Service.FuBenDataDict.TryRemove(gameId, out KuaFu5v5FuBenDataTemp))
			{
				KuaFu5v5FuBenDataTemp.State = GameFuBenState.End;
			}
			ClientAgentManager.Instance().RemoveKfFuben(TianTi5v5Service.GameType, KuaFu5v5FuBenData.ServerId, (long)KuaFu5v5FuBenData.GameId);
			lock (KuaFu5v5FuBenData)
			{
				foreach (KeyValuePair<int, int> kv in KuaFu5v5FuBenData.ZhanDuiDict)
				{
					KF5v5PiPeiTeam team;
					if (TianTi5v5Service.PiPeiDict.TryGetValue(kv.Key, out team))
					{
						if (team.GameId == gameId && team.State >= 3)
						{
							team.State = 0;
						}
					}
				}
			}
		}

		
		public static void RemoveRoleFromFuBen(int gameId, int zhanDuiID)
		{
			if (gameId > 0)
			{
				KuaFu5v5FuBenData fuBenData;
				if (TianTi5v5Service.FuBenDataDict.TryGetValue(gameId, out fuBenData))
				{
					lock (fuBenData)
					{
						fuBenData.State = GameFuBenState.End;
						int count = fuBenData.RemoveKuaFuFuBenZhanDui(zhanDuiID);
						if (fuBenData.CanRemove())
						{
							TianTi5v5Service.RemoveGameFuBen(fuBenData);
						}
					}
				}
			}
		}

		
		public static int GetZhanLiAssignRangeID(long startTicks, DateTime now)
		{
			long waitTicks = Math.Max(0L, now.Ticks - startTicks);
			int result;
			if (null == TianTi5v5Service.pTianTiPiPeiCfg)
			{
				result = -1;
			}
			else
			{
				foreach (TianTi5v5Service.PiPeiCfg item in TianTi5v5Service.pTianTiPiPeiCfg)
				{
					if (waitTicks >= (long)(item.MinTime * 1000) * 10000L && waitTicks < (long)(item.MaxTime * 1000) * 10000L)
					{
						return item.ID;
					}
				}
				result = -1;
			}
			return result;
		}

		
		private static void CheckGameFuBenTimerProc(DateTime now)
		{
			if (TianTi5v5Service.FuBenDataDict.Count > 0)
			{
				DateTime canRemoveTime = now.AddMinutes(-40.0);
				foreach (KuaFu5v5FuBenData fuBenData in TianTi5v5Service.FuBenDataDict.Values)
				{
					lock (fuBenData)
					{
						if (fuBenData.CanRemove())
						{
							TianTi5v5Service.RemoveGameFuBen(fuBenData);
						}
						else if (fuBenData.EndTime < canRemoveTime)
						{
							TianTi5v5Service.RemoveGameFuBen(fuBenData);
						}
					}
				}
			}
		}

		
		public static int UpdateEscapeZhanDui(int zhanDuiID, int jiFen, DateTime fightTime)
		{
			TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(zhanDuiID);
			int result;
			if (zhanDuiData != null)
			{
				if (jiFen != -2147483648)
				{
					zhanDuiData.EscapeJiFen = jiFen;
				}
				zhanDuiData.EscapeLastFightTime = fightTime;
				TianTi5v5Service.Persistence.UpdateEscapeZhanDui(zhanDuiData);
				result = zhanDuiData.EscapeJiFen;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		private const double CheckGameFuBenInterval = 1000.0;

		
		private const double CheckRoleTimerProcInterval = 1.428;

		
		private const double SaveServerStateProcInterval = 30.0;

		
		private static object Mutex = new object();

		
		private static bool ThreadInit = true;

		
		private static GameTypes GameType = GameTypes.TianTi5v5;

		
		private static GameTypes ChannelGameType = GameTypes.TianTi;

		
		private static int EnterGameSecs = 30;

		
		private static DateTime CheckGameFuBenTime;

		
		private static DateTime CheckRoleTimerProcTime;

		
		private static DateTime SaveServerStateProcTime;

		
		private static List<Tuple<KF5v5PiPeiTeam, int, int, int>> ProcessPiPeiList = new List<Tuple<KF5v5PiPeiTeam, int, int, int>>();

		
		public static int[] AssignRangeArray = new int[]
		{
			0,
			1,
			2,
			100
		};

		
		public static int[][] AssignRangeArray2 = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				100
			}
		};

		
		public static List<TianTi5v5Service.PiPeiCfg> pTianTiPiPeiCfg = new List<TianTi5v5Service.PiPeiCfg>();

		
		private static SortedList<long, int> RolePairFightCountDict = new SortedList<long, int>();

		
		public static ConcurrentDictionary<int, KF5v5PiPeiTeam> PiPeiDict = new ConcurrentDictionary<int, KF5v5PiPeiTeam>();

		
		private static ConcurrentDictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new ConcurrentDictionary<int, KuaFu5v5FuBenData>(1, 4096);

		
		private static Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		
		private static Dictionary<int, TianTi5v5ZhanDuiRoleData> ZhanDuiRoleDataDict = new Dictionary<int, TianTi5v5ZhanDuiRoleData>();

		
		private static TianTi5v5RankData RankData = new TianTi5v5RankData();

		
		public static TianTiPersistence Persistence = TianTiPersistence.Instance;

		
		public class PiPeiCfg
		{
			
			public int ID;

			
			public int MinTime;

			
			public int MaxTime;

			
			public int Zhanli;

			
			public int PiPeiType;

			
			public double dPiPeiPercent;
		}
	}
}
