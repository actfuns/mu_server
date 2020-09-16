using System;

namespace HSGameEngine.Tools.AStar
{
	
	public enum PathFinderNodeType
	{
		
		Start = 1,
		
		End,
		
		Open = 4,
		
		Close = 8,
		
		Current = 16,
		
		Path = 32
	}
}
