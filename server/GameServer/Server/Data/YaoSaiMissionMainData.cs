using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000802 RID: 2050
	[ProtoContract]
	public class YaoSaiMissionMainData
	{
		// Token: 0x040043E6 RID: 17382
		[ProtoMember(1)]
		public List<YaoSaiMissionData> MissionDataList;

		// Token: 0x040043E7 RID: 17383
		[ProtoMember(2)]
		public int ExcuteMissionCount;

		// Token: 0x040043E8 RID: 17384
		[ProtoMember(3)]
		public DateTime FreeRefreshTime;
	}
}
