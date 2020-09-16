using System;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	
	[ProtoContract]
	public class ZhuanPanGongGaoData
	{
		
		[ProtoMember(1)]
		public int ZoneId;

		
		[ProtoMember(2)]
		public int Rid;

		
		[ProtoMember(3)]
		public string RoleName;

		
		[ProtoMember(4)]
		public string GoodsId;

		
		[ProtoMember(5)]
		public int GoodsIndex;
	}
}
