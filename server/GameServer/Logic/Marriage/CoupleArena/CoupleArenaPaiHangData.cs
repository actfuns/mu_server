using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000365 RID: 869
	[ProtoContract]
	public class CoupleArenaPaiHangData
	{
		// Token: 0x04001702 RID: 5890
		[ProtoMember(1)]
		public List<CoupleArenaCoupleJingJiData> PaiHang;
	}
}
