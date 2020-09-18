using System;

namespace DotNetDetour
{
	
	[AttributeUsage(AttributeTargets.Method)]
	public class MonitorAttribute : Attribute
	{
		
		
		
		public string NamespaceName { get; set; }

		
		
		
		public string ClassName { get; set; }

		
		
		
		public Type Type { get; set; }

		
		public MonitorAttribute(string NamespaceName, string ClassName)
		{
			this.NamespaceName = NamespaceName;
			this.ClassName = ClassName;
		}

		
		public MonitorAttribute(Type type)
		{
			this.Type = type;
		}
	}
}
