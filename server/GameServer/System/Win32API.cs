using System;
using System.Runtime.InteropServices;

namespace System
{
	
	public static class Win32API
	{
		
		[DllImport("Tmsk.Tools.dll")]
		public static extern ushort GenRandKey(int val);

		
		[DllImport("Tmsk.Tools.dll")]
		public static extern ulong OpenKey(ushort randKey, ulong oldSortKey);

		
		[DllImport("Tmsk.Tools.dll")]
		public static extern void CloseKey(IntPtr key);

		
		[DllImport("Tmsk.Tools.dll")]
		public unsafe static extern void SortBytes(byte* data, int srcIndex, int length, ulong key);

		
		[DllImport("Tmsk.Tools.dll")]
		public static extern ushort OpenMagic(ushort randKey, ushort baseVal);
	}
}
