using System;
using System.Collections.Generic;

namespace GameServer.Logic.FuMo
{
	
	public class FuMoRandomTemplate
	{
		
		
		
		public int ID { get; set; }

		
		
		
		public string Name { get; set; }

		
		
		
		public string Type { get; set; }

		
		
		
		public Dictionary<double, double> Parameter { get; set; }

		
		
		
		public int BeginNum { get; set; }

		
		
		
		public int EndNum { get; set; }
	}
}
