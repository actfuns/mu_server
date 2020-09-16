using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriCZKingData
	{
		
		[ProtoMember(1)]
		public int YuanBao;

		
		[ProtoMember(2)]
		public List<InputKingPaiHangData> ListPaiHang;

		
		[ProtoMember(3)]
		public int State;

		
		[ProtoMember(4)]
		public List<InputKingPaiHangData> ListPaiHangYestoday;
	}
}
