using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaPaiHangData
	{
		
		[ProtoMember(1)]
		public List<CoupleArenaCoupleJingJiData> PaiHang;
	}
}
