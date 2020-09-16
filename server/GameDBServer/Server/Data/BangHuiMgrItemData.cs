using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMgrItemData
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
		public int BHZhiwu = 0;

		
		[ProtoMember(6)]
		public string ChengHao = "";

		
		[ProtoMember(7)]
		public int BangGong = 0;

		
		[ProtoMember(8)]
		public int Level = 0;
	}
}
