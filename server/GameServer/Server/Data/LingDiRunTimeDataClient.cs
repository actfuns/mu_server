using System;

namespace Server.Data
{
	
	public class LingDiRunTimeDataClient
	{
		
		public object Mutex = new object();

		
		public bool[] DoubleOpenState;

		
		public DoubleOpenItem[] DoubleOpenTime;
	}
}
