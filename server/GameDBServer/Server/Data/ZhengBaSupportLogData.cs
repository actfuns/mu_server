using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhengBaSupportLogData
	{
		
		[ProtoMember(1)]
		public int FromRoleId;

		
		[ProtoMember(2)]
		public int FromZoneId;

		
		[ProtoMember(3)]
		public string FromRolename;

		
		[ProtoMember(4)]
		public int SupportType;

		
		[ProtoMember(5)]
		public int ToUnionGroup;

		
		[ProtoMember(6)]
		public int ToGroup;

		
		[ProtoMember(7)]
		public DateTime Time;

		
		[ProtoMember(8)]
		public int Month;

		
		[ProtoMember(9)]
		public int RankOfDay;

		
		[ProtoMember(10)]
		public int FromServerId;
	}
}
