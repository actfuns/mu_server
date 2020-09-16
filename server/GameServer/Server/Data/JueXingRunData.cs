using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class JueXingRunData
	{
		
		public object Mutex = new object();

		
		public bool MoHuaOpen = false;

		
		public Dictionary<int, JueXingShiItem> JueXingShiDict = new Dictionary<int, JueXingShiItem>();

		
		public Dictionary<int, TaoZhuang> TaoZhuangDict = new Dictionary<int, TaoZhuang>();

		
		public Dictionary<int, AwakenLevelItem> AwakenLevelDict = new Dictionary<int, AwakenLevelItem>();

		
		public Dictionary<int, int> AwakenRecoveryDict = new Dictionary<int, int>();

		
		public int ExcellencePropLimit = 6;

		
		public int SuitIDLimit = 11;
	}
}
