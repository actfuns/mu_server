using System;

namespace KF.Contract.Data
{
	// Token: 0x02000013 RID: 19
	[Serializable]
	public class KuaFuFuBenRoleData
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000024A4 File Offset: 0x000006A4
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000024BB File Offset: 0x000006BB
		public int ServerId { get; protected internal set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000036 RID: 54 RVA: 0x000024C4 File Offset: 0x000006C4
		// (set) Token: 0x06000037 RID: 55 RVA: 0x000024DB File Offset: 0x000006DB
		public int RoleId { get; protected internal set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000024E4 File Offset: 0x000006E4
		// (set) Token: 0x06000039 RID: 57 RVA: 0x000024FB File Offset: 0x000006FB
		public int Side { get; protected internal set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002504 File Offset: 0x00000704
		// (set) Token: 0x0600003B RID: 59 RVA: 0x0000251B File Offset: 0x0000071B
		public int GameId { get; protected internal set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002524 File Offset: 0x00000724
		// (set) Token: 0x0600003D RID: 61 RVA: 0x0000253B File Offset: 0x0000073B
		public int ZhanDouLi { get; protected internal set; }
	}
}
