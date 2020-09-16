using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WingData
	{
		
		[ProtoMember(1)]
		public int DbID = 0;

		
		[ProtoMember(2)]
		public int WingID = 0;

		
		[ProtoMember(3)]
		public int ForgeLevel = 0;

		
		[ProtoMember(4)]
		public long AddDateTime = 0L;

		
		[ProtoMember(5)]
		public int JinJieFailedNum = 0;

		
		[ProtoMember(6)]
		public int Using = 0;

		
		[ProtoMember(7)]
		public int StarExp = 0;

		
		[ProtoMember(8)]
		public int ZhuLingNum = 0;

		
		[ProtoMember(9)]
		public int ZhuHunNum = 0;
	}
}
