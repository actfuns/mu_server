using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangGongHistData
	{
		
		[ProtoMember(1)]
		public int ZoneID = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public string RoleName = "";

		
		[ProtoMember(4)]
		public int Occupation = 0;

		
		[ProtoMember(5)]
		public int RoleLevel = 0;

		
		[ProtoMember(6)]
		public int BHZhiWu = 0;

		
		[ProtoMember(7)]
		public string BHChengHao = "";

		
		[ProtoMember(8)]
		public int Goods1Num = 0;

		
		[ProtoMember(9)]
		public int Goods2Num = 0;

		
		[ProtoMember(10)]
		public int Goods3Num = 0;

		
		[ProtoMember(11)]
		public int Goods4Num = 0;

		
		[ProtoMember(12)]
		public int Goods5Num = 0;

		
		[ProtoMember(13)]
		public int TongQian = 0;

		
		[ProtoMember(14)]
		public int BangGong = 0;
	}
}
