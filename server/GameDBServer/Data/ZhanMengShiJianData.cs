using System;
using GameDBServer.DB;
using ProtoBuf;

namespace GameDBServer.Data
{
	
	[ProtoContract]
	public class ZhanMengShiJianData
	{
		
		[DBMapping(ColumnName = "pkId")]
		public int PKId = 0;

		
		[ProtoMember(1)]
		[DBMapping(ColumnName = "bhId")]
		public int BHID = 0;

		
		[ProtoMember(2)]
		[DBMapping(ColumnName = "shijianType")]
		public int ShiJianType = 0;

		
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(3)]
		public string RoleName = "";

		
		[DBMapping(ColumnName = "createTime")]
		[ProtoMember(4)]
		public string CreateTime = "";

		
		[DBMapping(ColumnName = "subValue1")]
		[ProtoMember(5)]
		public int SubValue1 = -1;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "subValue2")]
		public int SubValue2 = -1;

		
		[ProtoMember(7)]
		[DBMapping(ColumnName = "subValue3")]
		public int SubValue3 = -1;

		
		[DBMapping(ColumnName = "subSzValue1")]
		[ProtoMember(8)]
		public string SubSzValue1 = "";
	}
}
