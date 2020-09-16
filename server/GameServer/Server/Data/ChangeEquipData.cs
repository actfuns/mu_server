using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ChangeEquipData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public GoodsData EquipGoodsData = null;

		
		[ProtoMember(3)]
		public WingData UsingWinData = null;
	}
}
