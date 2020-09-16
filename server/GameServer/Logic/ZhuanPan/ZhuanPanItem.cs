using System;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	
	[ProtoContract]
	public class ZhuanPanItem
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public string GoodsID;

		
		[ProtoMember(3)]
		public int AwardLevel;

		
		[ProtoMember(4)]
		public int GongGao;

		
		[ProtoMember(5)]
		public int AwardLabel;
	}
}
