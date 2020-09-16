using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiItemData
	{
		
		[ProtoMember(1)]
		public int BHID = 0;

		
		[ProtoMember(2)]
		public string BHName = "";

		
		[ProtoMember(3)]
		public int ZoneID = 0;

		
		[ProtoMember(4)]
		public int BZRoleID = 0;

		
		[ProtoMember(5)]
		public string BZRoleName = "";

		
		[ProtoMember(6)]
		public int BZOccupation = 0;

		
		[ProtoMember(7)]
		public int TotalNum = 0;

		
		[ProtoMember(8)]
		public int TotalLevel = 0;

		
		[ProtoMember(9)]
		public int QiLevel = 0;

		
		[ProtoMember(10)]
		public int IsVerfiy = 0;

		
		[ProtoMember(11)]
		public int TotalCombatForce = 0;
	}
}
