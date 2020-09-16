using System;
using System.Runtime.InteropServices;

namespace HSGameEngine.Tools.AStarEx
{
	
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NodeFast
	{
		
		public double f;

		
		public double g;

		
		public double h;

		
		public int parentX;

		
		public int parentY;
	}
}
