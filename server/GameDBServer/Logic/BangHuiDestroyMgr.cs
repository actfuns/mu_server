using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Tmsk.Tools.Tools;

namespace GameDBServer.Logic
{
	
	public class BangHuiDestroyMgr
	{
		
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

		
		private static int LastCheckDestroyDayID = DateTime.Now.DayOfYear;

		
		private static int LastCheckDestroyTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		
		private static int DestroyTimer = 21;
	}
}
