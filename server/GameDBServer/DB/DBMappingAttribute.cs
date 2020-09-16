using System;

namespace GameDBServer.DB
{
	// Token: 0x020000ED RID: 237
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DBMappingAttribute : Attribute
	{
		// Token: 0x170000BA RID: 186
		
		
		public string ColumnName { get; set; }
	}
}
