using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HongBaoDeatilsData
	{
		
		[ProtoMember(1)]
		public int hongBaoStatus;

		
		[ProtoMember(2)]
		public int type;

		
		[ProtoMember(3)]
		public string sender;

		
		[ProtoMember(4)]
		public DateTime sendTime;

		
		[ProtoMember(5)]
		public string message;

		
		[ProtoMember(6)]
		public int diamondNum;

		
		[ProtoMember(7)]
		public int sumDiamondNum;

		
		[ProtoMember(8)]
		public int leftCount;

		
		[ProtoMember(9)]
		public int sumCount;

		
		[ProtoMember(10)]
		public List<SingleHongBaoRankInfo> infos;

		
		[ProtoMember(11)]
		public int hongBaoID;
	}
}
