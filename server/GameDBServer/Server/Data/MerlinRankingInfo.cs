using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MerlinRankingInfo
	{
		
		public PlayerMerlinRankingData getPlayerMerlinRankingData()
		{
			return new PlayerMerlinRankingData(this);
		}

		
		private PlayerMerlinRankingData rankingData;

		
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleID")]
		public int nRoleID;

		
		[ProtoMember(2)]
		[DBMapping(ColumnName = "roleName")]
		public string strRoleName;

		
		[DBMapping(ColumnName = "occupation")]
		[ProtoMember(3)]
		public int nOccupation;

		
		[ProtoMember(4)]
		[DBMapping(ColumnName = "level")]
		public int nLevel = 0;

		
		[ProtoMember(5)]
		[DBMapping(ColumnName = "starNum")]
		public int nStarNum = 0;

		
		[DBMapping(ColumnName = "addTime")]
		[ProtoMember(6)]
		public string strAddTime = "";
	}
}
