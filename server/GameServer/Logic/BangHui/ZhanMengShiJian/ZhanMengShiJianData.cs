using System;
using ProtoBuf;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	
	[ProtoContract]
	public class ZhanMengShiJianData
	{
		
		public int PKId = 0;

		
		[ProtoMember(1)]
		public int BHID = 0;

		
		[ProtoMember(2)]
		public int ShiJianType = 0;

		
		[ProtoMember(3)]
		public string RoleName = "";

		
		[ProtoMember(4)]
		public string CreateTime = "";

		
		[ProtoMember(5)]
		public int SubValue1 = -1;

		
		[ProtoMember(6)]
		public int SubValue2 = -1;

		
		[ProtoMember(7)]
		public int SubValue3 = -1;

		
		[ProtoMember(8)]
		public string SubSzValue1 = "";
	}
}
