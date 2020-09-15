using System;

namespace DotNetDetour
{
	// Token: 0x02000013 RID: 19
	[AttributeUsage(AttributeTargets.Method)]
	public class MonitorAttribute : Attribute
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00004274 File Offset: 0x00002474
		// (set) Token: 0x0600004B RID: 75 RVA: 0x0000428B File Offset: 0x0000248B
		public string NamespaceName { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00004294 File Offset: 0x00002494
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000042AB File Offset: 0x000024AB
		public string ClassName { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600004E RID: 78 RVA: 0x000042B4 File Offset: 0x000024B4
		// (set) Token: 0x0600004F RID: 79 RVA: 0x000042CB File Offset: 0x000024CB
		public Type Type { get; set; }

		// Token: 0x06000050 RID: 80 RVA: 0x000042D4 File Offset: 0x000024D4
		public MonitorAttribute(string NamespaceName, string ClassName)
		{
			this.NamespaceName = NamespaceName;
			this.ClassName = ClassName;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000042EF File Offset: 0x000024EF
		public MonitorAttribute(Type type)
		{
			this.Type = type;
		}
	}
}
