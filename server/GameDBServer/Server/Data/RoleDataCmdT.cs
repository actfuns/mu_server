using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006B RID: 107
	[ProtoContract]
	public class RoleDataCmdT<T>
	{
		// Token: 0x06000105 RID: 261 RVA: 0x0000681C File Offset: 0x00004A1C
		public RoleDataCmdT()
		{
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006827 File Offset: 0x00004A27
		public RoleDataCmdT(int roleId, T v)
		{
			this.RoleID = roleId;
			this.Value = v;
		}

		// Token: 0x0400024B RID: 587
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400024C RID: 588
		[ProtoMember(2, IsRequired = true)]
		public T Value;
	}
}
