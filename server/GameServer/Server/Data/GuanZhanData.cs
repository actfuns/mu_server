using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007A9 RID: 1961
	[ProtoContract]
	public class GuanZhanData
	{
		// Token: 0x04003F21 RID: 16161
		[ProtoMember(1)]
		public List<string> SideName = new List<string>();

		// Token: 0x04003F22 RID: 16162
		[ProtoMember(2)]
		public Dictionary<int, List<GuanZhanRoleMiniData>> RoleMiniDataDict = new Dictionary<int, List<GuanZhanRoleMiniData>>();
	}
}
