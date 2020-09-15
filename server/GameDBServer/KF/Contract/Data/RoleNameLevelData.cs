using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x020000BE RID: 190
	[ProtoContract]
	public class RoleNameLevelData
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x00008F45 File Offset: 0x00007145
		public RoleNameLevelData()
		{
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00008F50 File Offset: 0x00007150
		public RoleNameLevelData(int zhuanSheng, int level, string roleName, bool zhiWu, int occupation)
		{
			this.ZhuanSheng = zhuanSheng;
			this.Level = level;
			this.RoleName = roleName;
			this.ZhiWu = zhiWu;
			this.Occupation = occupation;
		}

		// Token: 0x0400050C RID: 1292
		[ProtoMember(1)]
		public int ZhuanSheng;

		// Token: 0x0400050D RID: 1293
		[ProtoMember(2)]
		public int Level;

		// Token: 0x0400050E RID: 1294
		[ProtoMember(3)]
		public string RoleName;

		// Token: 0x0400050F RID: 1295
		[ProtoMember(4)]
		public bool ZhiWu;

		// Token: 0x04000510 RID: 1296
		[ProtoMember(5)]
		public int Occupation;
	}
}
