using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WingRankingInfo
	{
		
		public PlayerWingRankingData getPlayerWingRankingData()
		{
			return new PlayerWingRankingData(this);
		}

		
		private PlayerWingRankingData rankingData;

		
		[DBMapping(ColumnName = "rid")]
		[ProtoMember(1)]
		public int nRoleID;

		
		[DBMapping(ColumnName = "rname")]
		[ProtoMember(2)]
		public string strRoleName;

		
		[DBMapping(ColumnName = "occupation")]
		[ProtoMember(3)]
		public int nOccupation;

		
		[DBMapping(ColumnName = "wingid")]
		[ProtoMember(4)]
		public int nWingID = 0;

		
		[DBMapping(ColumnName = "forgeLevel")]
		[ProtoMember(5)]
		public int nStarNum = 0;

		
		[DBMapping(ColumnName = "addtime")]
		[ProtoMember(6)]
		public string strAddTime = "";
	}
}
