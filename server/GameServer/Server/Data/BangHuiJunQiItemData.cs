using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiJunQiItemData
	{
		
		[ProtoMember(1)]
		public int BHID = 0;

		
		[ProtoMember(2)]
		public string QiName = "";

		
		[ProtoMember(3)]
		public int QiLevel = 0;
	}
}
