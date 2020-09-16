using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	
	public class GAreaLua
	{
		
		
		
		public int ID { get; set; }

		
		
		
		public Point CenterPoint { get; set; }

		
		
		
		public int Radius { get; set; }

		
		
		
		public string LuaScriptFileName { get; set; }

		
		
		
		public AddtionType AddtionType { get; set; }

		
		
		
		public int TaskId { get; set; }

		
		
		
		public Dictionary<AreaEventType, List<int>> AreaEventDict { get; set; }
	}
}
