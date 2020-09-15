using System;
using System.Runtime.InteropServices;

namespace DotNetDetour
{
	// Token: 0x02000015 RID: 21
	public class NativeAPI
	{
		// Token: 0x06000052 RID: 82
		[DllImport("kernel32")]
		public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, Protection flNewProtect, out uint lpflOldProtect);
	}
}
