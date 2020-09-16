using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WanMotaInfo
	{
		
		public PlayerWanMoTaRankingData getPlayerWanMoTaRankingData()
		{
			return new PlayerWanMoTaRankingData(this);
		}

		
		private PlayerWanMoTaRankingData rankingData;

		
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleID")]
		public int nRoleID;

		
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(2)]
		public string strRoleName;

		
		[DBMapping(ColumnName = "flushTime")]
		[ProtoMember(3)]
		public long lFlushTime = 0L;

		
		[DBMapping(ColumnName = "passLayerCount")]
		[ProtoMember(4)]
		public int nPassLayerCount = 0;

		
		[ProtoMember(5)]
		[DBMapping(ColumnName = "sweepLayer")]
		public int nSweepLayer = 0;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "sweepReward")]
		public string strSweepReward = "";

		
		[ProtoMember(7)]
		[DBMapping(ColumnName = "sweepBeginTime")]
		public long lSweepBeginTime = 0L;

		
		[ProtoMember(8)]
		public int nTopPassLayerCount;
	}
}
