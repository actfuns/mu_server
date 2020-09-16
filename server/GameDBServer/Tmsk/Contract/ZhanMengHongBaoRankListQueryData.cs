using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	
	[ProtoContract]
	public class ZhanMengHongBaoRankListQueryData
	{
		
		[ProtoMember(1)]
		public int Bhid;

		
		[ProtoMember(2)]
		public int Type;

		
		[ProtoMember(3)]
		public DateTime StartTime;

		
		[ProtoMember(4)]
		public int Count;
	}
}
