using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiBagData
	{
		
		[ProtoMember(1)]
		public int Goods1Num = 0;

		
		[ProtoMember(2)]
		public int Goods2Num = 0;

		
		[ProtoMember(3)]
		public int Goods3Num = 0;

		
		[ProtoMember(4)]
		public int Goods4Num = 0;

		
		[ProtoMember(5)]
		public int Goods5Num = 0;

		
		[ProtoMember(6)]
		public int TongQian = 0;

		
		[ProtoMember(7)]
		public List<BangGongHistData> BbangGongHistList = null;
	}
}
