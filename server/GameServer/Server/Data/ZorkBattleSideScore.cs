using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x02000849 RID: 2121
	[ProtoContract]
	public class ZorkBattleSideScore
	{
		// Token: 0x06003BBA RID: 15290 RVA: 0x0032F238 File Offset: 0x0032D438
		public ZorkBattleSideScore Clone()
		{
			return base.MemberwiseClone() as ZorkBattleSideScore;
		}

		// Token: 0x04004646 RID: 17990
		[ProtoMember(1)]
		public List<ZorkBattleRoleInfo> ZorkBattleRoleList = new List<ZorkBattleRoleInfo>();

		// Token: 0x04004647 RID: 17991
		[ProtoMember(2)]
		public Dictionary<int, string> MosterNextTimeDict = new Dictionary<int, string>();

		// Token: 0x04004648 RID: 17992
		[ProtoMember(3)]
		public List<ZorkBattleTeamInfo> ZorkBattleTeamList = new List<ZorkBattleTeamInfo>();

		// Token: 0x04004649 RID: 17993
		[ProtoMember(4)]
		public int BossBuffID;
	}
}
