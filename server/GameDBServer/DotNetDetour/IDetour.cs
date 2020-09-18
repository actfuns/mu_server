using System;
using System.Reflection;

namespace DotNetDetour
{
	
	public interface IDetour
	{
		
		void Patch(MethodInfo src, MethodInfo dest, MethodInfo ori);
	}
}
