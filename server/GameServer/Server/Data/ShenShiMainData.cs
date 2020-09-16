using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShenShiMainData
	{
		
		[ProtoMember(1)]
		public int FuWenTabId;

		
		[ProtoMember(2)]
		public DateTime NextFreeTime;
	}
}
