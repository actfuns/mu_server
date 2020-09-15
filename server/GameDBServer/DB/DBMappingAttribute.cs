using System;

namespace GameDBServer.DB
{
	// Token: 0x020000ED RID: 237
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DBMappingAttribute : Attribute
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0001FD28 File Offset: 0x0001DF28
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x0001FD3F File Offset: 0x0001DF3F
		public string ColumnName { get; set; }
	}
}
