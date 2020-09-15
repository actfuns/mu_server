using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A7 RID: 167
	[ProtoContract]
	public class RoleParamsData
	{
		// Token: 0x0400046A RID: 1130
		[ProtoMember(1)]
		public string ParamName = "";

		// Token: 0x0400046B RID: 1131
		[ProtoMember(2)]
		public string ParamValue = "";

		// Token: 0x0400046C RID: 1132
		public long UpdateFaildTicks;

		// Token: 0x0400046D RID: 1133
		public RoleParamType ParamType;
	}
}
