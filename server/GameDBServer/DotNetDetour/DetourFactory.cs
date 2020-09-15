using System;
using DotNetDetour.DetourWays;

namespace DotNetDetour
{
	// Token: 0x0200000A RID: 10
	public class DetourFactory
	{
		// Token: 0x06000030 RID: 48 RVA: 0x000031A8 File Offset: 0x000013A8
		public static IDetour CreateDetourEngine()
		{
			IDetour result;
			if (IntPtr.Size == 4)
			{
				result = new NativeDetourFor32Bit();
			}
			else
			{
				if (IntPtr.Size != 8)
				{
					throw new NotImplementedException();
				}
				result = new NativeDetourFor64Bit();
			}
			return result;
		}
	}
}
