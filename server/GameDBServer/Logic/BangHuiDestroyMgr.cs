using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Tmsk.Tools.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000118 RID: 280
	public class BangHuiDestroyMgr
	{
		// Token: 0x060004A8 RID: 1192 RVA: 0x00025F64 File Offset: 0x00024164
		public static void ProcessDestroyBangHui(DBManager dbMgr)
		{
			DateTime dateTime = DateTime.Now;
			int dayID = dateTime.DayOfYear;
			DayOfWeek dayOfWeek = dateTime.DayOfWeek;
			int nowTimer = dateTime.Hour * 60 + dateTime.Minute;
			if (dayID != BangHuiDestroyMgr.LastCheckDestroyDayID)
			{
				BangHuiDestroyMgr.LastCheckDestroyDayID = dayID;
				BangHuiDestroyMgr.LastCheckDestroyTimer = nowTimer;
			}
			else if (nowTimer >= BangHuiDestroyMgr.DestroyTimer && BangHuiDestroyMgr.LastCheckDestroyTimer < BangHuiDestroyMgr.DestroyTimer && dayOfWeek == DayOfWeek.Sunday)
			{
				BangHuiDestroyMgr.LastCheckDestroyDayID = dayID;
				BangHuiDestroyMgr.LastCheckDestroyTimer = nowTimer;
				BangHuiDestroyMgr.HandleDestroyBangHuis(dbMgr);
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00026018 File Offset: 0x00024218
		private static void HandleDestroyBangHuis(DBManager dbMgr)
		{
			int moneyPerLevel = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-per-qilevel", 10000);
			if (moneyPerLevel > 0)
			{
				DBWriter.SubBangHuiTongQianByQiLevel(dbMgr, moneyPerLevel);
				int maxLevel = GameDBManager.GameConfigMgr.GetGameConfigItemInt("juntuanbanghuimax", 8);
				string goldjoin = GameDBManager.GameConfigMgr.GetGameConfigItemStr("bhmatch_goldjoin", "");
				List<int> goldjoinList = ConfigHelper.String2IntList(goldjoin, '|');
				List<int> noMoneyBangHuiList = DBQuery.GetNoMoneyBangHuiList(dbMgr, maxLevel);
				for (int i = 0; i < noMoneyBangHuiList.Count; i++)
				{
					int bhid = noMoneyBangHuiList[i];
					if (!goldjoinList.Exists((int x) => x == bhid))
					{
						BangHuiDestroyMgr.DoDestroyBangHui(dbMgr, bhid);
						string gmCmdData = string.Format("-autodestroybh {0}", bhid);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
					}
				}
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00026110 File Offset: 0x00024310
		public static void DoDestroyBangHui(DBManager dbMgr, int bhid)
		{
			lock (Global.BangHuiMutex)
			{
				DBWriter.DeleteBangHui(dbMgr, bhid);
				GameDBManager.BangHuiJunQiMgr.RemoveBangHuiJunQi(bhid);
				DBWriter.ClearAllRoleBangHuiInfo(dbMgr, bhid);
				List<DBRoleInfo> dbRoleInfoList = dbMgr.DBRoleMgr.GetCachingDBRoleInfoListByFaction(bhid);
				if (null != dbRoleInfoList)
				{
					for (int i = 0; i < dbRoleInfoList.Count; i++)
					{
						dbRoleInfoList[i].Faction = 0;
						dbRoleInfoList[i].BHName = "";
						dbRoleInfoList[i].BHZhiWu = 0;
					}
				}
			}
			DBWriter.ClearBHLingDiByID(dbMgr, bhid);
			GameDBManager.BangHuiLingDiMgr.ClearBangHuiLingDi(bhid);
			ZhanMengShiJianManager.getInstance().onZhanMengJieSan(bhid);
			string gmCmdData = string.Format("-synclingdi", new object[0]);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00026210 File Offset: 0x00024410
		public static void ClearBangHuiLingDi(DBManager dbMgr, int bhid)
		{
			lock (Global.BangHuiMutex)
			{
				GameDBManager.BangHuiJunQiMgr.RemoveBangHuiJunQi(bhid);
			}
			DBWriter.ClearBHLingDiByID(dbMgr, bhid);
			GameDBManager.BangHuiLingDiMgr.ClearBangHuiLingDi(bhid);
			string gmCmdData = string.Format("-synclingdi", new object[0]);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
		}

		// Token: 0x0400077E RID: 1918
		private static int LastCheckDestroyDayID = DateTime.Now.DayOfYear;

		// Token: 0x0400077F RID: 1919
		private static int LastCheckDestroyTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		// Token: 0x04000780 RID: 1920
		private static int DestroyTimer = 21;
	}
}
