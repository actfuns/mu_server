using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class BangHuiNumLevelMgr
	{
		
		public static void RecalcBangHuiNumLevel(DBManager dbMgr)
		{
			long ticks = DateTime.Now.Ticks;
			if (ticks - BangHuiNumLevelMgr.LastQueryTimeTicks >= BangHuiNumLevelMgr.MaxQueryTimeSlotTicks)
			{
				BangHuiNumLevelMgr.LastQueryTimeTicks = ticks;
				GameDBManager.BangHuiListMgr.RefreshBangHuiListData(dbMgr);
				BangHuiListData bangHuiListData = GameDBManager.BangHuiListMgr.GetBangHuiListData(dbMgr, -1, 0, 10000);
				if (bangHuiListData != null && null != bangHuiListData.BangHuiItemDataList)
				{
					List<BangHuiNumLevelItem> bangHuiNumLevelItemList = new List<BangHuiNumLevelItem>();
					for (int i = 0; i < bangHuiListData.BangHuiItemDataList.Count; i++)
					{
						int totalNum;
						int totalLevel;
						long totalCombatforce;
						if (DBQuery.QueryBHMemberSumData(bangHuiListData.BangHuiItemDataList[i].BHID, out totalNum, out totalLevel, out totalCombatforce))
						{
							bangHuiNumLevelItemList.Add(new BangHuiNumLevelItem
							{
								BHID = bangHuiListData.BangHuiItemDataList[i].BHID,
								TotalNum = totalNum,
								TotalLevel = totalLevel,
								TotalCombatForce = (int)Math.Min(totalCombatforce, 2147483647L)
							});
						}
					}
					for (int i = 0; i < bangHuiNumLevelItemList.Count; i++)
					{
						DBWriter.UpdateBangHuiNumLevel(dbMgr, bangHuiNumLevelItemList[i].BHID, bangHuiNumLevelItemList[i].TotalNum, bangHuiNumLevelItemList[i].TotalLevel, bangHuiNumLevelItemList[i].TotalCombatForce);
					}
				}
			}
		}

		
		public static long MaxQueryTimeSlotTicks = 600000000L;

		
		private static long LastQueryTimeTicks = 0L;
	}
}
