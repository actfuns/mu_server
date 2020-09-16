using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Tmsk.Contract
{
	
	[ProtoContract]
	public class HongBaoSendData
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
		public int leftZuanShi;

		
		[ProtoMember(7)]
		public int sumDiamondNum;

		
		[ProtoMember(8)]
		public int leftCount;

		
		[ProtoMember(9)]
		public int sumCount;

		
		[ProtoMember(10)]
		public DateTime endTime;

		
		[ProtoMember(11)]
		public int hongBaoID;

		
		[ProtoMember(12)]
		public List<HongBaoRecvData> RecvList;

		
		[ProtoMember(13)]
		public int senderID;

		
		[ProtoMember(14)]
		public int bhid;
	}
}
