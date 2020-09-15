using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000094 RID: 148
	[ProtoContract]
	public class EscapeBattleSideScore
	{
		// Token: 0x06000269 RID: 617 RVA: 0x0002A27C File Offset: 0x0002847C
		public EscapeBattleSideScore Clone()
		{
			return base.MemberwiseClone() as EscapeBattleSideScore;
		}

		// Token: 0x04000388 RID: 904
		[ProtoMember(1)]
		public List<EscapeBattleRoleInfo> BattleRoleList = new List<EscapeBattleRoleInfo>();

		// Token: 0x04000389 RID: 905
		[ProtoMember(2)]
		public List<EscapeBattleTeamInfo> BattleTeamList = new List<EscapeBattleTeamInfo>();

		// Token: 0x0400038A RID: 906
		[ProtoMember(3)]
		public EscapeBattleAreaInfo targetSafeArea = new EscapeBattleAreaInfo();

		// Token: 0x0400038B RID: 907
		[ProtoMember(4)]
		public EscapeBattleAreaInfo safeArea = new EscapeBattleAreaInfo();

		// Token: 0x0400038C RID: 908
		[ProtoMember(5)]
		public DateTime AreaChangeTm;

		// Token: 0x0400038D RID: 909
		[ProtoMember(6)]
		public int ReliveCount;

		// Token: 0x0400038E RID: 910
		[ProtoMember(7)]
		public EscapeBattleGameSceneStatuses eStatus;
	}
}
