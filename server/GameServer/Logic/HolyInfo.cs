using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class HolyInfo
	{
		
		public static int GetShengwuID(sbyte nSuit, sbyte nType)
		{
			return (int)(nType * 100 + nSuit);
		}

		
		public List<MagicActionItem> m_ExtraPropertyList = new List<MagicActionItem>();
	}
}
