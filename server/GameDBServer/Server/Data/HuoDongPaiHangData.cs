using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HuoDongPaiHangData
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int ZoneID;

		
		[ProtoMember(4)]
		public int Type;

		
		[ProtoMember(5)]
		public int PaiHang;

		
		[ProtoMember(6)]
		public string PaiHangTime;

		
		[ProtoMember(7)]
		public int PaiHangValue;
	}
}
