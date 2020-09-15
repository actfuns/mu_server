using System;
using System.Reflection;

namespace DotNetDetour
{
	// Token: 0x02000011 RID: 17
	internal class DestAndOri : IEquatable<DestAndOri>
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003D58 File Offset: 0x00001F58
		public bool Equals(DestAndOri other)
		{
			return this.Dest == other.Dest && this.Ori == other.Ori;
		}

		// Token: 0x04000035 RID: 53
		public MethodInfo Dest;

		// Token: 0x04000036 RID: 54
		public MethodInfo Ori;

		// Token: 0x04000037 RID: 55
		public MethodInfo Src;
	}
}
