using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TempItemChargeInfo
	{
		
		[ProtoMember(1)]
		public int ID = 0;

		
		[ProtoMember(2)]
		public string userID = "";

		
		[ProtoMember(3)]
		public int chargeRoleID = 0;

		
		[ProtoMember(4)]
		public int addUserMoney = 0;

		
		[ProtoMember(5)]
		public int zhigouID = 0;

		
		[ProtoMember(6)]
		public string chargeTime = "";

		
		[ProtoMember(7)]
		public byte isDel = 0;
	}
}
