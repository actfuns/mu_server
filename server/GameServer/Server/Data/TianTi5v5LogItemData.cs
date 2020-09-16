using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTi5v5LogItemData
	{
		
		[ProtoMember(1)]
		public int ZoneId1;

		
		[ProtoMember(2)]
		public string RoleName1;

		
		[ProtoMember(3)]
		public int ZoneId2;

		
		[ProtoMember(4)]
		public string RoleName2;

		
		[ProtoMember(5)]
		public int Success;

		
		[ProtoMember(6)]
		public int DuanWeiJiFenAward;

		
		[ProtoMember(7)]
		public int RongYaoAward;

		
		[ProtoMember(8)]
		public int RoleId;

		
		[ProtoMember(9)]
		public DateTime EndTime;
	}
}
