using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiData
	{
		
		public TianTi5v5ZhanDuiData Clone()
		{
			TianTi5v5ZhanDuiData data = base.MemberwiseClone() as TianTi5v5ZhanDuiData;
			if (null != this.teamerList)
			{
				data.teamerList = new List<TianTi5v5ZhanDuiRoleData>();
				foreach (TianTi5v5ZhanDuiRoleData role in this.teamerList)
				{
					data.teamerList.Add(role.Clone());
				}
			}
			return data;
		}

		
		[ProtoMember(1)]
		public int ZhanDuiID;

		
		[ProtoMember(2)]
		public string XuanYan;

		
		[ProtoMember(3)]
		public string ZhanDuiName;

		
		[ProtoMember(4)]
		public int LeaderRoleID;

		
		[ProtoMember(5)]
		public int DuanWeiId;

		
		[ProtoMember(6)]
		public int DuanWeiJiFen;

		
		[ProtoMember(7)]
		public int DuanWeiRank;

		
		[ProtoMember(8)]
		public long ZhanDouLi;

		
		[ProtoMember(9)]
		public int LianSheng;

		
		[ProtoMember(10)]
		public int SuccessCount;

		
		[ProtoMember(11)]
		public int FightCount;

		
		[ProtoMember(12)]
		public int MonthDuanWeiRank;

		
		[ProtoMember(13)]
		public List<TianTi5v5ZhanDuiRoleData> teamerList = new List<TianTi5v5ZhanDuiRoleData>();

		
		[ProtoMember(14)]
		public string TeamerRidList;

		
		[ProtoMember(15)]
		public DateTime LastFightTime;

		
		[ProtoMember(16)]
		public string LeaderRoleName;

		
		[ProtoMember(17)]
		public int ZoneID;

		
		[ProtoMember(18)]
		public int ZorkWin;

		
		[ProtoMember(19)]
		public int ZorkWinStreak;

		
		[ProtoMember(20)]
		public int ZorkBossInjure;

		
		[ProtoMember(21)]
		public int ZorkJiFen;

		
		[ProtoMember(22)]
		public DateTime ZorkLastFightTime;

		
		[ProtoMember(23)]
		public int EscapeJiFen;

		
		[ProtoMember(24)]
		public DateTime EscapeLastFightTime;

		
		[ProtoMember(25)]
		public int ZhanDuiDataModeType;
	}
}
