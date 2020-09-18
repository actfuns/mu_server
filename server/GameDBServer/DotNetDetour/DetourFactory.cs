using System;
using DotNetDetour.DetourWays;

namespace DotNetDetour
{
	
	public class DetourFactory
	{
		
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
