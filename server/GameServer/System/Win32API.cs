using System;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x020008FC RID: 2300
	public static class Win32API
	{
		// Token: 0x06004296 RID: 17046
		[DllImport("Tmsk.Tools.dll")]
		public static extern ushort GenRandKey(int val);

		// Token: 0x06004297 RID: 17047
		[DllImport("Tmsk.Tools.dll")]
		public static extern ulong OpenKey(ushort randKey, ulong oldSortKey);

		// Token: 0x06004298 RID: 17048
		[DllImport("Tmsk.Tools.dll")]
		public static extern void CloseKey(IntPtr key);

		// Token: 0x06004299 RID: 17049
		[DllImport("Tmsk.Tools.dll")]
		public unsafe static extern void SortBytes(byte* data, int srcIndex, int length, ulong key);

		// Token: 0x0600429A RID: 17050
		[DllImport("Tmsk.Tools.dll")]
		public static extern ushort OpenMagic(ushort randKey, ushort baseVal);
	}
}
