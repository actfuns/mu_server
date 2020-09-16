using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PrisonMainData
	{
		
		[ProtoMember(1)]
		public PrisonRoleData roleData = new PrisonRoleData();

		
		[ProtoMember(2)]
		public int MineFuLuState = 0;

		
		[ProtoMember(3)]
		public long RevoltCD = 0L;

		
		[ProtoMember(4)]
		public int JieJiuCount = 0;

		
		[ProtoMember(5)]
		public int ZhengFuCount = 0;

		
		[ProtoMember(6)]
		public int ZhengFuLeftCount = 0;

		
		[ProtoMember(7)]
		public int LaoDongCount = 0;

		
		[ProtoMember(8)]
		public List<PrisonFuLuData> fuLuDataList = new List<PrisonFuLuData>();
	}
}
