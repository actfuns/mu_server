using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RingRankingInfo
	{
		
		public PlayerRingRankingData getPlayerRingRankingData()
		{
			return new PlayerRingRankingData(this);
		}

		
		private PlayerRingRankingData rankingData;

		
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleid")]
		public int nRoleID;

		
		[DBMapping(ColumnName = "rolename")]
		[ProtoMember(2)]
		public string strRoleName;

		
		[DBMapping(ColumnName = "goodwilllevel")]
		[ProtoMember(3)]
		public int byGoodwilllevel = 0;

		
		[ProtoMember(4)]
		[DBMapping(ColumnName = "goodwillstar")]
		public int byGoodwillstar = 0;

		
		[DBMapping(ColumnName = "ringid")]
		[ProtoMember(5)]
		public int nRingID;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "changtime")]
		public string strAddTime = "";
	}
}
