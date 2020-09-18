using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiRoleData
	{
		
		public TianTi5v5ZhanDuiRoleData Clone()
		{
			return base.MemberwiseClone() as TianTi5v5ZhanDuiRoleData;
		}

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int RoleOcc;

		
		[ProtoMember(5)]
		public string RoleName = "";

		
		[ProtoMember(6)]
		public long ZhanLi = 0L;

		
		[ProtoMember(7)]
		public int ZhuanSheng = 0;

		
		[ProtoMember(8)]
		public int Level = 0;

		
		[ProtoMember(9)]
		public byte[] ModelData;

		
		[ProtoMember(10)]
		public int OnlineState;

		
		[ProtoMember(11)]
		public byte[] PlayerJingJiMirrorData;

		
		[ProtoMember(12)]
		public int ZoneID;

		
		[ProtoMember(13)]
		public int TodayFightCount;

		
		[ProtoMember(14)]
		public int LastFightDayId;

		
		[ProtoMember(15)]
		public int MonthFigntCount;

		
		[ProtoMember(16)]
		public int MonthAwardsFlags;

		
		[ProtoMember(17)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;

		
		[ProtoMember(18)]
		public int[] MonthFightCounts;

		
		[ProtoMember(19)]
		public int RebornLevel = 0;
	}
}
