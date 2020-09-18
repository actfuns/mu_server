using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Logic.MerlinMagicBook;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Logic.Wing;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class PaiHangManager
	{
		
		private static void LoadPaiHangLists(DBManager dbMgr)
		{
			PaiHangManager.RoleEquipPaiHangList = DBQuery.GetRoleEquipPaiHang(dbMgr);
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("paihang-jingmailevel", 0) > 0)
			{
				PaiHangManager.RoleXueWeiNumPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "JingMaiLevel");
			}
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("paihang-wuxuelevel", 0) > 0)
			{
				PaiHangManager.RoleSkillLevelPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "WuXueLevel");
			}
			PaiHangManager.RoleHorseJiFenPaiHangList = DBQuery.GetRoleHorseJiFenPaiHang(dbMgr);
			PaiHangManager.RoleLevelPaiHangList = DBQuery.GetRoleLevelPaiHang(dbMgr);
			PaiHangManager.RoleYinLiangPaiHangList = DBQuery.GetRoleYinLiangPaiHang(dbMgr);
			PaiHangManager.RoleLianZhanPaiHangList = DBQuery.GetRoleLianZhanPaiHang(dbMgr);
			PaiHangManager.RoleKillBossPaiHangList = DBQuery.GetRoleKillBossPaiHang(dbMgr);
			PaiHangManager.RoleBattleNumPaiHangList = DBQuery.GetRoleBattleNumPaiHang(dbMgr);
			PaiHangManager.RoleHeroIndexPaiHangList = DBQuery.GetRoleHeroIndexPaiHang(dbMgr);
			PaiHangManager.RoleGoldPaiHangList = DBQuery.GetRoleGoldPaiHang(dbMgr);
			PaiHangManager.CombatForcePaiHangList = DBQuery.GetRoleCombatForcePaiHang(dbMgr);
			PaiHangManager.UserMoneyPaiHangList = DBQuery.GetUserMoneyPaiHang(dbMgr);
			PaiHangManager.RoleChengJiuPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "ChengJiuLevel");
			PaiHangManager.RoleShengWangPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "ShengWangLevel");
			PaiHangManager.RoleGuardStatuePaiHangList = DBQuery.GetRoleGuardStatuePaiHang(dbMgr);
			PaiHangManager.RoleHolyItemPaiHangList = DBQuery.GetRoleHolyItemPaiHang(dbMgr);
			PaiHangManager.StorePaiHangForHuoDong(dbMgr);
		}

		
		public static void ProcessPaiHang(DBManager dbMgr, bool force = false)
		{
			if (force)
			{
				PaiHangManager.LoadPaiHangLists(dbMgr);
			}
			else
			{
				DateTime dateTime = DateTime.Now;
				long nowTicks = dateTime.Ticks / 10000L;
				if (nowTicks - PaiHangManager.LastUpdatePaiHangTickTimer >= 1800000L)
				{
					PaiHangManager.LastUpdatePaiHangTickTimer = nowTicks;
					PaiHangManager.LoadPaiHangLists(dbMgr);
				}
				else
				{
					int dayID = dateTime.DayOfYear;
					if (dayID != PaiHangManager.LastCheckPaiHangDayID)
					{
						PaiHangManager.LastCheckPaiHangDayID = dayID;
						string gmCmdData = string.Format("-updatepaihangbang", new object[0]);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
					}
				}
			}
		}

		
		public static PaiHangData GetPaiHangData(int paiHangType, int pageShowNum = -1)
		{
			List<PaiHangItemData> paiHangItemList = null;
			switch (paiHangType)
			{
			case 1:
				paiHangItemList = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				paiHangItemList = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				paiHangItemList = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				paiHangItemList = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				paiHangItemList = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				paiHangItemList = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				paiHangItemList = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				paiHangItemList = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				paiHangItemList = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				paiHangItemList = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				paiHangItemList = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				paiHangItemList = PaiHangManager.CombatForcePaiHangList;
				break;
			case 13:
				paiHangItemList = JingJiChangManager.getInstance().getRankingList(0);
				break;
			case 14:
				paiHangItemList = WanMoTaManager.getInstance().getRankingList(0);
				break;
			case 15:
				paiHangItemList = WingPaiHangManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 16:
				paiHangItemList = RingPaiHangManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 17:
				paiHangItemList = MerlinRankManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 18:
				paiHangItemList = PaiHangManager.UserMoneyPaiHangList;
				break;
			case 19:
				paiHangItemList = PaiHangManager.RoleChengJiuPaiHangList;
				break;
			case 20:
				paiHangItemList = PaiHangManager.RoleShengWangPaiHangList;
				break;
			case 21:
				paiHangItemList = PaiHangManager.RoleGuardStatuePaiHangList;
				break;
			case 22:
				paiHangItemList = PaiHangManager.RoleHolyItemPaiHangList;
				break;
			}
			return new PaiHangData
			{
				PaiHangType = paiHangType,
				PaiHangList = paiHangItemList
			};
		}

		
		protected static void StorePaiHangForHuoDong(DBManager dbMgr)
		{
			PaiHangManager.StorePaiHangPos(dbMgr, 5, 5, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 8, 6, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 3, 7, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 2, 8, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 8, 36, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 5, 33, 10);
		}

		
		protected static void StorePaiHangPos(DBManager dbMgr, int paiHangType, int huoDongType, int maxPaiHang = 10)
		{
			List<PaiHangItemData> paiHangItemList = null;
			switch (paiHangType)
			{
			case 1:
				paiHangItemList = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				paiHangItemList = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				paiHangItemList = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				paiHangItemList = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				paiHangItemList = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				paiHangItemList = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				paiHangItemList = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				paiHangItemList = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				paiHangItemList = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				paiHangItemList = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				paiHangItemList = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				paiHangItemList = PaiHangManager.CombatForcePaiHangList;
				break;
			}
			if (null != paiHangItemList)
			{
				string paiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				int i = 0;
				while (i < paiHangItemList.Count && i < maxPaiHang)
				{
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref paiHangItemList[i].RoleID);
					if (null != dbRoleInfo)
					{
						int phvalue = paiHangItemList[i].Val1;
						DBWriter.AddHongDongPaiHangRecord(dbMgr, paiHangItemList[i].RoleID, dbRoleInfo.RoleName, dbRoleInfo.ZoneID, huoDongType, i + 1, paiHangTime, phvalue);
					}
					i++;
				}
			}
		}

		
		public static int GetPaiHangPosByRoleID(int paiHangType, int roleID)
		{
			List<PaiHangItemData> paiHangItemList = null;
			switch (paiHangType)
			{
			case 1:
				paiHangItemList = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				paiHangItemList = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				paiHangItemList = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				paiHangItemList = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				paiHangItemList = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				paiHangItemList = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				paiHangItemList = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				paiHangItemList = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				paiHangItemList = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				paiHangItemList = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				paiHangItemList = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				paiHangItemList = PaiHangManager.CombatForcePaiHangList;
				break;
			case 19:
				paiHangItemList = PaiHangManager.RoleChengJiuPaiHangList;
				break;
			case 20:
				paiHangItemList = PaiHangManager.RoleShengWangPaiHangList;
				break;
			case 21:
				paiHangItemList = PaiHangManager.RoleGuardStatuePaiHangList;
				break;
			case 22:
				paiHangItemList = PaiHangManager.RoleHolyItemPaiHangList;
				break;
			}
			int result;
			if (null == paiHangItemList)
			{
				result = -1;
			}
			else
			{
				int i = 0;
				while (i < paiHangItemList.Count && i < 10)
				{
					if (paiHangItemList[i].RoleID == roleID)
					{
						return i;
					}
					i++;
				}
				result = -1;
			}
			return result;
		}

		
		public static Dictionary<int, int> CalcPaiHangPosDictRoleID(int roleID)
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			for (int i = 1; i < 23; i++)
			{
				dict[i] = PaiHangManager.GetPaiHangPosByRoleID(i, roleID);
			}
			return dict;
		}

		
		public static void OnChangeName(int roleid, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				Action<List<PaiHangItemData>> _nameUpdater = delegate(List<PaiHangItemData> _itemList)
				{
					if (_itemList != null)
					{
						try
						{
							List<PaiHangItemData> items = _itemList.FindAll((PaiHangItemData _item) => _item.RoleID == roleid);
							if (items != null)
							{
								foreach (PaiHangItemData item in items)
								{
									item.RoleName = newName;
								}
							}
						}
						catch (Exception)
						{
						}
					}
				};
				_nameUpdater(PaiHangManager.RoleEquipPaiHangList);
				_nameUpdater(PaiHangManager.RoleXueWeiNumPaiHangList);
				_nameUpdater(PaiHangManager.RoleSkillLevelPaiHangList);
				_nameUpdater(PaiHangManager.RoleHorseJiFenPaiHangList);
				_nameUpdater(PaiHangManager.RoleLevelPaiHangList);
				_nameUpdater(PaiHangManager.RoleYinLiangPaiHangList);
				_nameUpdater(PaiHangManager.RoleLianZhanPaiHangList);
				_nameUpdater(PaiHangManager.RoleKillBossPaiHangList);
				_nameUpdater(PaiHangManager.RoleBattleNumPaiHangList);
				_nameUpdater(PaiHangManager.RoleHeroIndexPaiHangList);
				_nameUpdater(PaiHangManager.RoleGoldPaiHangList);
				_nameUpdater(PaiHangManager.CombatForcePaiHangList);
				_nameUpdater(PaiHangManager.RoleChengJiuPaiHangList);
				_nameUpdater(PaiHangManager.RoleShengWangPaiHangList);
				PaiHangManager._UpdateName_t_huodongpaihang(roleid, oldName, newName);
				JingJiChangManager.getInstance().OnChangeName(roleid, oldName, newName);
				WanMoTaManager.getInstance().OnChangeName(roleid, oldName, newName);
				WingPaiHangManager.getInstance().OnChangeName(roleid, oldName, newName);
				RingPaiHangManager.getInstance().OnChangeName(roleid, oldName, newName);
				MerlinRankManager.getInstance().OnChangeName(roleid, oldName, newName);
			}
		}

		
		private static void _UpdateName_t_huodongpaihang(int roleid, string oldName, string newName)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_huodongpaihang SET rname='{0}' WHERE rid={1}", newName, roleid);
				if (!conn.ExecuteNonQueryBool(sql, 0))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名，更新t_huodongpaihang失败, roleId={0}, oldName={1}, newName={2}", roleid, oldName, newName), null, true);
				}
			}
		}

		
		private const long UpdatePaiHangIntervalTimer = 1800000L;

		
		private static int LastCheckPaiHangDayID = DateTime.Now.DayOfYear;

		
		private static int LastCheckPaiHangTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		
		private static int PaiHangTimer = 420;

		
		private static long LastUpdatePaiHangTickTimer = 0L;

		
		private static List<PaiHangItemData> RoleEquipPaiHangList = null;

		
		private static List<PaiHangItemData> RoleXueWeiNumPaiHangList = null;

		
		private static List<PaiHangItemData> RoleSkillLevelPaiHangList = null;

		
		private static List<PaiHangItemData> RoleHorseJiFenPaiHangList = null;

		
		private static List<PaiHangItemData> RoleLevelPaiHangList = null;

		
		private static List<PaiHangItemData> RoleYinLiangPaiHangList = null;

		
		private static List<PaiHangItemData> RoleLianZhanPaiHangList = null;

		
		private static List<PaiHangItemData> RoleKillBossPaiHangList = null;

		
		private static List<PaiHangItemData> RoleBattleNumPaiHangList = null;

		
		private static List<PaiHangItemData> RoleHeroIndexPaiHangList = null;

		
		private static List<PaiHangItemData> RoleGoldPaiHangList = null;

		
		private static List<PaiHangItemData> UserMoneyPaiHangList = null;

		
		private static List<PaiHangItemData> RoleChengJiuPaiHangList = null;

		
		private static List<PaiHangItemData> RoleShengWangPaiHangList = null;

		
		private static List<PaiHangItemData> RoleGuardStatuePaiHangList = null;

		
		private static List<PaiHangItemData> RoleHolyItemPaiHangList = null;

		
		private static List<PaiHangItemData> CombatForcePaiHangList = null;
	}
}
