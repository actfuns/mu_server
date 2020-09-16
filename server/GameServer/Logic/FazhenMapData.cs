using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class FazhenMapData
	{
		
		public int CopyMapID = 0;

		
		public int MapCode = 0;

		
		public Dictionary<int, SingleFazhenTelegateData> Telegates = new Dictionary<int, SingleFazhenTelegateData>();
	}
}
