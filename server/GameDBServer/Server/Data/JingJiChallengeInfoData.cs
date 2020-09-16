using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiChallengeInfoData
	{
		
		[DBMapping(ColumnName = "pkId")]
		[ProtoMember(1)]
		public int pkId;

		
		[ProtoMember(2)]
		[DBMapping(ColumnName = "roleId")]
		public int roleId;

		
		[ProtoMember(3)]
		[DBMapping(ColumnName = "zhanbaoType")]
		public int zhanbaoType;

		
		[ProtoMember(4)]
		[DBMapping(ColumnName = "challengeName")]
		public string challengeName;

		
		[DBMapping(ColumnName = "value")]
		[ProtoMember(5)]
		public int value;

		
		[DBMapping(ColumnName = "createTime")]
		public string createTime;
	}
}
