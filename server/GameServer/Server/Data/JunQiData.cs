using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JunQiData
	{
		
		[ProtoMember(1)]
		public int JunQiID = 0;

		
		[ProtoMember(2)]
		public string QiName = "";

		
		[ProtoMember(3)]
		public int JunQiLevel = 0;

		
		[ProtoMember(4)]
		public int ZoneID = 0;

		
		[ProtoMember(5)]
		public int BHID = 0;

		
		[ProtoMember(6)]
		public string BHName = "";

		
		[ProtoMember(7)]
		public int QiZuoNPC = 0;

		
		[ProtoMember(8)]
		public int MapCode = 0;

		
		[ProtoMember(9)]
		public int PosX = 0;

		
		[ProtoMember(10)]
		public int PosY = 0;

		
		[ProtoMember(11)]
		public int Direction = 0;

		
		[ProtoMember(12)]
		public int LifeV = 0;

		
		[ProtoMember(13)]
		public int CutLifeV = 0;

		
		[ProtoMember(14)]
		public long StartTime = 0L;

		
		[ProtoMember(15)]
		public int BodyCode = 0;

		
		[ProtoMember(16)]
		public int PicCode = 0;

		
		[ProtoMember(17)]
		public int CurrentLifeV = 0;
	}
}
