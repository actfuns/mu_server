using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BiaoCheData
	{
		
		[ProtoMember(1)]
		public int OwnerRoleID = 0;

		
		[ProtoMember(2)]
		public int BiaoCheID = 0;

		
		[ProtoMember(3)]
		public string BiaoCheName = "";

		
		[ProtoMember(4)]
		public int YaBiaoID = 0;

		
		[ProtoMember(5)]
		public int MapCode = 0;

		
		[ProtoMember(6)]
		public int PosX = 0;

		
		[ProtoMember(7)]
		public int PosY = 0;

		
		[ProtoMember(8)]
		public int Direction = 0;

		
		[ProtoMember(9)]
		public int LifeV = 0;

		
		[ProtoMember(10)]
		public int CutLifeV = 0;

		
		[ProtoMember(11)]
		public long StartTime = 0L;

		
		[ProtoMember(12)]
		public int BodyCode = 0;

		
		[ProtoMember(13)]
		public int PicCode = 0;

		
		[ProtoMember(14)]
		public int CurrentLifeV = 0;

		
		[ProtoMember(15)]
		public string OwnerRoleName = "";
	}
}
