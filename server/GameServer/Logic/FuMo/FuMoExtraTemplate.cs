using System;
using System.Collections.Generic;

namespace GameServer.Logic.FuMo
{
	
	public class FuMoExtraTemplate
	{
		
		
		
		public int ID { get; set; }

		
		
		
		public string Name { get; set; }

		
		
		
		public List<int> Condition { get; set; }

		
		
		
		public string Type { get; set; }

		
		
		
		public Dictionary<double, double> Parameter { get; set; }
	}
}
