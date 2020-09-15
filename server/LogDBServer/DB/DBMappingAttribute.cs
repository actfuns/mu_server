using System;

namespace LogDBServer.DB
{
	// Token: 0x02000012 RID: 18
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DBMappingAttribute : Attribute
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003888 File Offset: 0x00001A88
		// (set) Token: 0x06000045 RID: 69 RVA: 0x0000389F File Offset: 0x00001A9F
		public string ColumnName { get; set; }
	}
}
