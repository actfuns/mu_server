using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000017 RID: 23
	[ProtoContract]
	public class RoleDataCmdT<T>
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00006066 File Offset: 0x00004266
		public RoleDataCmdT()
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00006071 File Offset: 0x00004271
		public RoleDataCmdT(int roleId, T v)
		{
			this.RoleID = roleId;
			this.Value = v;
		}

		// Token: 0x04000092 RID: 146
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000093 RID: 147
		[ProtoMember(2, IsRequired = true)]
		public T Value;
	}
}
