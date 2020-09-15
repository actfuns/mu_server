using System;
using System.Reflection;

namespace DotNetDetour
{
	// Token: 0x0200000B RID: 11
	public interface IDetour
	{
		// Token: 0x06000032 RID: 50
		void Patch(MethodInfo src, MethodInfo dest, MethodInfo ori);
	}
}
