using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZuoQiMainData
	{
		
		[ProtoMember(1)]
		public List<MountData> MountList;

		
		[ProtoMember(2)]
		public DateTime NextFreeTime;

		
		[ProtoMember(3)]
		public int MountLevel;
	}
}
