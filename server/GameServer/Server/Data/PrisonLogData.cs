using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PrisonLogData
	{
		
		[ProtoMember(1)]
		public int ID = 0;

		
		[ProtoMember(2)]
		public string Name1;

		
		[ProtoMember(3)]
		public string Name2;

		
		[ProtoMember(4)]
		public int JiangLiType = 0;

		
		[ProtoMember(5)]
		public int JiangLiCount = 0;
	}
}
