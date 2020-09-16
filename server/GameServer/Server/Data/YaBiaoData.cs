using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaBiaoData
	{
		
		[ProtoMember(1)]
		public int YaBiaoID = 0;

		
		[ProtoMember(2)]
		public long StartTime = 0L;

		
		[ProtoMember(3)]
		public int State = 0;

		
		[ProtoMember(4)]
		public int LineID = 0;

		
		[ProtoMember(5)]
		public int TouBao = 0;

		
		[ProtoMember(6)]
		public int YaBiaoDayID = 0;

		
		[ProtoMember(7)]
		public int YaBiaoNum = 0;

		
		[ProtoMember(8)]
		public int TakeGoods = 0;
	}
}
