using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058B RID: 1419
	[ProtoContract]
	public class RoleParamsData
	{
		// Token: 0x040027FF RID: 10239
		[ProtoMember(1)]
		public string ParamName = "";

		// Token: 0x04002800 RID: 10240
		[ProtoMember(2)]
		public string ParamValue = "";
	}
}
