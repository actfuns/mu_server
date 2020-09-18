using System;
using System.Windows;

namespace GameServer.Interface
{
	
	public interface IPosition
	{
		
		
		
		Point Center { get; set; }

		
		
		
		Point Coordinate { get; set; }

		
		
		
		int Z { get; set; }
	}
}
