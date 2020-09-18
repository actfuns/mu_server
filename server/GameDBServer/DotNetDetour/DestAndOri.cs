using System;
using System.Reflection;

namespace DotNetDetour
{
	
	internal class DestAndOri : IEquatable<DestAndOri>
	{
		
		public bool Equals(DestAndOri other)
		{
			return this.Dest == other.Dest && this.Ori == other.Ori;
		}

		
		public MethodInfo Dest;

		
		public MethodInfo Ori;

		
		public MethodInfo Src;
	}
}
