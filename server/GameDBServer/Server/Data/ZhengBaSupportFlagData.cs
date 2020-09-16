using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhengBaSupportFlagData
	{
		
		[ProtoMember(1)]
		public int UnionGroup;

		
		[ProtoMember(2)]
		public int Group;

		
		[ProtoMember(3)]
		public bool IsOppose;

		
		[ProtoMember(4)]
		public bool IsSupport;

		
		[ProtoMember(5)]
		public bool IsYaZhu;

		
		[ProtoMember(6)]
		public int SupportDay;
	}
}
