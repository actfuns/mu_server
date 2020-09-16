using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class FashionNamagerData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, FashionTabData> FashionTabDict = new Dictionary<int, FashionTabData>();

		
		public Dictionary<int, FashionData> FashingDict = new Dictionary<int, FashionData>();

		
		public Dictionary<KeyValuePair<int, int>, FashionBagData> FashionBagDict = new Dictionary<KeyValuePair<int, int>, FashionBagData>();

		
		public Dictionary<int, int> SpecialTitleDict = new Dictionary<int, int>();

		
		public int LuoLanChengZhuRoleID = 0;
	}
}
