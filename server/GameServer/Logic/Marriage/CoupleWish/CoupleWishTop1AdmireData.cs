using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleWish
{
	
	[ProtoContract]
	public class CoupleWishTop1AdmireData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int DbCoupleId;

		
		[ProtoMember(2)]
		public RoleData4Selector ManSelector;

		
		[ProtoMember(3)]
		public RoleData4Selector WifeSelector;

		
		[ProtoMember(4)]
		public int BeAdmireCount;

		
		[ProtoMember(5)]
		public int MyAdmireCount;
	}
}
