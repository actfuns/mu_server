using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MountData
	{
		
		[ProtoMember(1)]
		public int GoodsID;

		
		[ProtoMember(2)]
		public bool IsNew;
	}
}
