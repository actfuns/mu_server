using System;

namespace DotNetDetour
{
	// Token: 0x02000013 RID: 19
	[AttributeUsage(AttributeTargets.Method)]
	public class MonitorAttribute : Attribute
	{
		// Token: 0x17000002 RID: 2
		
		
		public string NamespaceName { get; set; }

		// Token: 0x17000003 RID: 3
		
		
		public string ClassName { get; set; }

		// Token: 0x17000004 RID: 4
		
		
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
