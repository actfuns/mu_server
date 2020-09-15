using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006EB RID: 1771
	public class HuangChengManager
	{
		// Token: 0x06002AC3 RID: 10947 RVA: 0x00263574 File Offset: 0x00261774
		public static void LoadHuangDiRoleIDFromDBServer(int bhid)
		{
			if (bhid > 0)
			{
				string[] fields = Global.ExecuteDBCmd(10076, string.Format("{0}", bhid), 0);
				if (fields != null && fields.Length == 3)
				{
					HuangChengManager.HuangDiRoleID = Global.SafeConvertToInt32(fields[0]);
					HuangChengManager.HuangDiRoleName = fields[1];
					HuangChengManager.HuangDiBHName = fields[2];
				}
			}
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x002635DC File Offset: 0x002617DC
		public static int GetHuangDiRoleID()
		{
			return HuangChengManager.HuangDiRoleID;
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x002635F4 File Offset: 0x002617F4
		public static string GetHuangDiRoleName()
		{
			return HuangChengManager.HuangDiRoleName;
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x0026360C File Offset: 0x0026180C
		public static string GetHuangDiBHName()
		{
			return HuangChengManager.HuangDiBHName;
		}

		// Token: 0x06002AC7 RID: 10951 RVA: 0x00263624 File Offset: 0x00261824
		public static int ProcessTakeSheLiZhiYuan(int roleID, string roleName, string bhName, bool sendToOtherLine = true)
		{
			int oldHuangDiRoleID = HuangChengManager.HuangDiRoleID;
			HuangChengManager.HuangDiRoleID = roleID;
			HuangChengManager.HuangDiRoleName = roleName;
			HuangChengManager.HuangDiBHName = bhName;
			HuangChengManager.HuangDiRoleTicks = TimeUtil.NOW();
			if (sendToOtherLine)
			{
				HuangChengManager.NotifySyncHuanDiRoleInfo(oldHuangDiRoleID, roleID, roleName, bhName);
			}
			return oldHuangDiRoleID;
		}

		// Token: 0x06002AC8 RID: 10952 RVA: 0x00263670 File Offset: 0x00261870
		public static void NotifySyncHuanDiRoleInfo(int oldHuangDiRoleID, int roleID, string roleName, string bhName)
		{
			string gmCmdData = string.Format("-synchuangdi {0} {1} {2} {3}", new object[]
			{
				oldHuangDiRoleID,
				roleID,
				roleName,
				bhName
			});
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				-1,
				"",
				0,
				"",
				0,
				gmCmdData,
				0,
				0,
				GameManager.ServerLineID
			}), null, 0);
		}

		// Token: 0x06002AC9 RID: 10953 RVA: 0x00263720 File Offset: 0x00261920
		public static void ParseWeekDaysTimes()
		{
			string huangChengZhanWeekDays_str = GameManager.systemParamsList.GetParamValueByName("HuangChengZhanWeekDays");
			if (!string.IsNullOrEmpty(huangChengZhanWeekDays_str))
			{
				string[] huangChengZhanWeekDays_fields = huangChengZhanWeekDays_str.Split(new char[]
				{
					','
				});
				int[] weekDays = new int[huangChengZhanWeekDays_fields.Length];
				for (int i = 0; i < huangChengZhanWeekDays_fields.Length; i++)
				{
					weekDays[i] = Global.SafeConvertToInt32(huangChengZhanWeekDays_fields[i]);
				}
				HuangChengManager.HuangChengZhanWeekDays = weekDays;
			}
			string huangChengZhanFightingDayTimes_str = GameManager.systemParamsList.GetParamValueByName("HuangChengZhanFightingDayTimes");
			HuangChengManager.HuangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(huangChengZhanFightingDayTimes_str);
			HuangChengManager.MaxHavingSheLiZhiYuanSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxHavingSheLiZhiYuanSecs", -1);
			HuangChengManager.MaxHavingSheLiZhiYuanSecs *= 1000;
		}

		// Token: 0x06002ACA RID: 10954 RVA: 0x002637D8 File Offset: 0x002619D8
		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == HuangChengManager.HuangChengZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < HuangChengManager.HuangChengZhanWeekDays.Length; i++)
				{
					if (HuangChengManager.HuangChengZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x0026382C File Offset: 0x00261A2C
		public static bool IsInHuangChengFightingTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			int weekDayID = (int)now.DayOfWeek;
			bool result;
			if (!HuangChengManager.IsDayOfWeek(weekDayID))
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(now, HuangChengManager.HuangChengZhanFightingDayTimes, out endMinute, false);
			}
			return result;
		}

		// Token: 0x06002ACC RID: 10956 RVA: 0x00263870 File Offset: 0x00261A70
		public static bool CanTakeSheLiZhiYuan()
		{
			return HuangChengManager.HuangDiRoleID <= 0;
		}

		// Token: 0x06002ACD RID: 10957 RVA: 0x00263898 File Offset: 0x00261A98
		public static bool IsHuangChengZhanOver()
		{
			return !HuangChengManager.WaitingHuangChengResult;
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x002638B4 File Offset: 0x00261AB4
		public static void ProcessHuangChengZhanResult()
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				if (HuangChengZhanStates.None == HuangChengManager.HuangChengZhanState)
				{
					if (HuangChengManager.IsInHuangChengFightingTime())
					{
						HuangChengManager.HuangChengZhanState = HuangChengZhanStates.Fighting;
						HuangChengManager.HuangDiRoleTicks = TimeUtil.NOW();
						HuangChengManager.WaitingHuangChengResult = true;
						HuangChengManager.NotifyAllHuangChengMapInfoData();
						HuangChengManager.HandleOutMapHuangDiRoleChanging();
					}
				}
				else if (HuangChengManager.IsInHuangChengFightingTime())
				{
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.HandleOutMapHuangDiRoleChanging();
						if (HuangChengManager.HuangDiRoleID > 0)
						{
							long ticks = TimeUtil.NOW();
							if (ticks - HuangChengManager.HuangDiRoleTicks > (long)HuangChengManager.MaxHavingSheLiZhiYuanSecs)
							{
								HuangChengManager.WaitingHuangChengResult = false;
								HuangChengManager.HandleHuangChengResult();
								HuangChengManager.NotifyAllHuangChengMapInfoData();
							}
						}
					}
				}
				else
				{
					HuangChengManager.HuangChengZhanState = HuangChengZhanStates.None;
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.WaitingHuangChengResult = false;
						HuangChengManager.HandleHuangChengResult();
						HuangChengManager.NotifyAllHuangChengMapInfoData();
					}
				}
			}
		}

		// Token: 0x06002ACF RID: 10959 RVA: 0x002639B0 File Offset: 0x00261BB0
		public static void NotifyAllHuangChengMapInfoData()
		{
			HuangChengMapInfoData huangChengMapInfoData = HuangChengManager.FormatHuangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllHuangChengMapInfoData(Global.GetHuangChengMapCode(), huangChengMapInfoData);
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x002639D5 File Offset: 0x00261BD5
		private static void HandleHuangChengFailed()
		{
			JunQiManager.HandleLingDiZhanResultByMapCode(2, Global.GetHuangChengMapCode(), 0, true, false);
			HuangChengManager.ProcessHuangChengFightingEndAwards(-1);
			Global.BroadcastHuangChengFailedHint();
			JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
		}

		// Token: 0x06002AD1 RID: 10961 RVA: 0x002639FC File Offset: 0x00261BFC
		private static void HandleHuangChengResult()
		{
			if (HuangChengManager.HuangDiRoleID <= 0)
			{
				HuangChengManager.HandleHuangChengFailed();
			}
			else
			{
				GameClient otherClient = GameManager.ClientMgr.FindClient(HuangChengManager.HuangDiRoleID);
				if (null == otherClient)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else if (otherClient.ClientData.Faction <= 0)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else if (otherClient.ClientData.BHZhiWu != 1)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else
				{
					JunQiManager.HandleLingDiZhanResultByMapCode(2, Global.GetHuangChengMapCode(), otherClient.ClientData.Faction, true, false);
					HuangChengManager.ProcessHuangChengFightingEndAwards(otherClient.ClientData.Faction);
					Global.BroadcastHuangChengOkHint(otherClient);
					JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				}
			}
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x00263AB4 File Offset: 0x00261CB4
		public static void HandleOutMapHuangDiRoleChanging()
		{
			if (HuangChengManager.HuangDiRoleID > 0)
			{
				GameClient client = GameManager.ClientMgr.FindClient(HuangChengManager.HuangDiRoleID);
				if (client == null || client.ClientData.MapCode != Global.GetHuangChengMapCode())
				{
					HuangChengManager.HandleDeadHuangDiRoleChanging(null);
				}
			}
		}

		// Token: 0x06002AD3 RID: 10963 RVA: 0x00263B08 File Offset: 0x00261D08
		public static void HandleLeaveMapHuangDiRoleChanging(GameClient client)
		{
			if (HuangChengManager.HuangDiRoleID > 0)
			{
				if (client.ClientData.RoleID == HuangChengManager.HuangDiRoleID)
				{
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.HandleDeadHuangDiRoleChanging(null);
					}
				}
			}
		}

		// Token: 0x06002AD4 RID: 10964 RVA: 0x00263B54 File Offset: 0x00261D54
		public static void HandleDeadHuangDiRoleChanging(GameClient client)
		{
			if (null != client)
			{
				if (client.ClientData.RoleID != HuangChengManager.HuangDiRoleID)
				{
					return;
				}
				if (2 != JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode))
				{
					return;
				}
				if (!HuangChengManager.IsInHuangChengFightingTime())
				{
					return;
				}
				if (!HuangChengManager.WaitingHuangChengResult)
				{
					return;
				}
			}
			int oldHuangDiRoleID = 0;
			lock (HuangChengManager.SheLiZhiYuanMutex)
			{
				oldHuangDiRoleID = HuangChengManager.ProcessTakeSheLiZhiYuan(0, "", "", true);
			}
			if (oldHuangDiRoleID > 0)
			{
				GameClient oldClient = GameManager.ClientMgr.FindClient(oldHuangDiRoleID);
				if (null != oldClient)
				{
					Global.RemoveBufferData(oldClient, 14);
				}
			}
			GameManager.ClientMgr.NotifyAllChgHuangDiRoleIDMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, oldHuangDiRoleID, HuangChengManager.GetHuangDiRoleID());
			HuangChengManager.NotifyAllHuangChengMapInfoData();
		}

		// Token: 0x06002AD5 RID: 10965 RVA: 0x00263C68 File Offset: 0x00261E68
		public static HuangChengMapInfoData GetHuangChengMapInfoData(GameClient client)
		{
			int lingDiID = JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode);
			HuangChengMapInfoData result;
			if (lingDiID != 2)
			{
				result = null;
			}
			else
			{
				result = HuangChengManager.FormatHuangChengMapInfoData();
			}
			return result;
		}

		// Token: 0x06002AD6 RID: 10966 RVA: 0x00263CA0 File Offset: 0x00261EA0
		public static HuangChengMapInfoData FormatHuangChengMapInfoData()
		{
			return new HuangChengMapInfoData
			{
				FightingEndTime = HuangChengManager.HuangDiRoleTicks,
				HuangDiRoleID = HuangChengManager.HuangDiRoleID,
				HuangDiRoleName = HuangChengManager.HuangDiRoleName,
				HuangDiBHName = HuangChengManager.HuangDiBHName,
				FightingState = (HuangChengManager.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = "",
				WangZuBHid = -1
			};
		}

		// Token: 0x06002AD7 RID: 10967 RVA: 0x00263D0C File Offset: 0x00261F0C
		private static int GetExperienceAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 1000000;
			}
			else
			{
				result = 500000;
			}
			return result;
		}

		// Token: 0x06002AD8 RID: 10968 RVA: 0x00263D34 File Offset: 0x00261F34
		private static int GetBangGongAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 100;
			}
			else
			{
				result = 50;
			}
			return result;
		}

		// Token: 0x06002AD9 RID: 10969 RVA: 0x00263D58 File Offset: 0x00261F58
		private static void ProcessRoleExperienceAwards(GameClient client, bool success)
		{
			int experience = HuangChengManager.GetExperienceAwards(client, success);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)experience, true, false, false, "none");
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x00263D84 File Offset: 0x00261F84
		private static void ProcessRoleBangGongAwards(GameClient client, bool success)
		{
			int bangGong = HuangChengManager.GetBangGongAwards(client, success);
			if (bangGong > 0)
			{
				if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref bangGong, AddBangGongTypes.None, 0))
				{
					if (0 != bangGong)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "皇城战结束时的奖励", "系统", client.ClientData.RoleName, "增加", bangGong, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
					}
				}
				GameManager.SystemServerEvents.AddEvent(string.Format("角色获取帮贡, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.BangGong,
					bangGong
				}), EventLevels.Record);
			}
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x00263E94 File Offset: 0x00262094
		private static bool CanGetAWards(GameClient client, long nowTicks)
		{
			bool result;
			if (nowTicks - client.ClientData.EnterMapTicks < (long)HuangChengManager.MaxHavingSheLiZhiYuanSecs)
			{
				result = false;
			}
			else if (client.ClientData.Faction <= 0)
			{
				result = false;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetAnyLingDiItemDataByBHID(client.ClientData.Faction);
				result = (null != bangHuiLingDiItemData);
			}
			return result;
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x00263F00 File Offset: 0x00262100
		private static void ProcessHuangChengFightingEndAwards(int huangDiBHID)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(Global.GetHuangChengMapCode());
			if (null != objsList)
			{
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CurrentLifeV > 0)
						{
							if (HuangChengManager.CanGetAWards(c, nowTicks))
							{
								HuangChengManager.ProcessRoleExperienceAwards(c, huangDiBHID == c.ClientData.Faction);
								HuangChengManager.ProcessRoleBangGongAwards(c, huangDiBHID == c.ClientData.Faction);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x00263FC0 File Offset: 0x002621C0
		public static int NewXuanFeiSafeNum(int roleID)
		{
			int result;
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				int randNum = Global.GetRandomNumber(0, 1000001);
				HuangChengManager.XuanFeiSafeDict[roleID] = randNum;
				result = randNum;
			}
			return result;
		}

		// Token: 0x06002ADE RID: 10974 RVA: 0x00264024 File Offset: 0x00262224
		public static int FindXuanFeiSafeNum(int roleID)
		{
			int result;
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				int randNum = 0;
				if (!HuangChengManager.XuanFeiSafeDict.TryGetValue(roleID, out randNum))
				{
					result = -1;
				}
				else
				{
					result = randNum;
				}
			}
			return result;
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x00264088 File Offset: 0x00262288
		public static void RemoveXuanFeiSafeNum(int roleID)
		{
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				HuangChengManager.XuanFeiSafeDict.Remove(roleID);
			}
		}

		// Token: 0x040039D6 RID: 14806
		public static object SheLiZhiYuanMutex = new object();

		// Token: 0x040039D7 RID: 14807
		private static bool WaitingHuangChengResult = false;

		// Token: 0x040039D8 RID: 14808
		private static long HuangDiRoleTicks = TimeUtil.NOW();

		// Token: 0x040039D9 RID: 14809
		private static int HuangDiRoleID = 0;

		// Token: 0x040039DA RID: 14810
		private static string HuangDiRoleName = "";

		// Token: 0x040039DB RID: 14811
		private static string HuangDiBHName = "";

		// Token: 0x040039DC RID: 14812
		private static int MaxHavingSheLiZhiYuanSecs = 1200;

		// Token: 0x040039DD RID: 14813
		private static int[] HuangChengZhanWeekDays = null;

		// Token: 0x040039DE RID: 14814
		private static DateTimeRange[] HuangChengZhanFightingDayTimes = null;

		// Token: 0x040039DF RID: 14815
		public static HuangChengZhanStates HuangChengZhanState = HuangChengZhanStates.None;

		// Token: 0x040039E0 RID: 14816
		private static Dictionary<int, int> XuanFeiSafeDict = new Dictionary<int, int>();
	}
}
