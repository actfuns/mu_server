using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class DayRechargeRankManager
	{
		
		public List<InputKingPaiHangData> GetRankByDay(DBManager dbMgr, int day)
		{
			List<InputKingPaiHangData> ranklist = null;
			int currDay = Global.GetOffsetDay(DateTime.Now);
			List<InputKingPaiHangData> result;
			if (day > currDay)
			{
				result = null;
			}
			else
			{
				if (day < currDay)
				{
					lock (this.RechargeRankDict)
					{
						if (this.RechargeRankDict.ContainsKey(day))
						{
							ranklist = this.RechargeRankDict[day];
							return ranklist;
						}
					}
				}
				List<int> minGateValueList = new List<int>();
				for (int i = 0; i < 4; i++)
				{
					minGateValueList.Add(1);
				}
				DateTime now = Global.GetRealDate(day);
				string startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				ranklist = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, startTime, endTime, minGateValueList, 4);
				if (null == ranklist)
				{
					result = null;
				}
				else
				{
					foreach (InputKingPaiHangData item in ranklist)
					{
						Global.GetUserMaxLevelRole(dbMgr, item.UserID, out item.MaxLevelRoleName, out item.MaxLevelRoleZoneID);
					}
					lock (this.RechargeRankDict)
					{
						this.RechargeRankDict[day] = ranklist;
					}
					result = ranklist;
				}
			}
			return result;
		}

		
		public int GetRoleRankByDay(DBManager dbMgr, string userid, int day)
		{
			List<InputKingPaiHangData> ranklist = this.GetRankByDay(dbMgr, day);
			int result;
			if (null == ranklist)
			{
				result = 0;
			}
			else
			{
				int rank = 0;
				foreach (InputKingPaiHangData item in ranklist)
				{
					if (string.Compare(userid, item.UserID) == 0)
					{
						return rank + 1;
					}
					rank++;
				}
				result = 0;
			}
			return result;
		}

		
		private const int HeFuRankCount = 4;

		
		private Dictionary<int, List<InputKingPaiHangData>> RechargeRankDict = new Dictionary<int, List<InputKingPaiHangData>>();
	}
}
