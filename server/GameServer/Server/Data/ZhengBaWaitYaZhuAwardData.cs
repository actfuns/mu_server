using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhengBaWaitYaZhuAwardData
	{
		
		[ProtoMember(1)]
		public int Month;

		
		[ProtoMember(2)]
		public int RankOfDay;

		
		[ProtoMember(3)]
		public int FromRoleId;

		
		[ProtoMember(4)]
		public int UnionGroup;

		
		[ProtoMember(5)]
		public int Group;
	}
}
