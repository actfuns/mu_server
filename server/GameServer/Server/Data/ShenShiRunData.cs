using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class ShenShiRunData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, int> ParentMagicCode = new Dictionary<int, int>();

		
		public Dictionary<int, FuWenHoleItem> FuWenHoleDict = new Dictionary<int, FuWenHoleItem>();

		
		public Dictionary<int, FuWenItem> FuWenDict = new Dictionary<int, FuWenItem>();

		
		public Dictionary<int, FuWenGodItem> FuWenGodDict = new Dictionary<int, FuWenGodItem>();

		
		public List<FuWenRandomItem> FuWenRandomList = new List<FuWenRandomItem>();

		
		public List<FuWenRandomItem> FuWenPayRandomList = new List<FuWenRandomItem>();

		
		public List<FuWenRandomItem> HuoDongFuWenRandomList = new List<FuWenRandomItem>();

		
		public List<FuWenRandomItem> HuoDongFuWenPayRandomList = new List<FuWenRandomItem>();
	}
}
