using System;
using System.Collections.Generic;

namespace GameServer.Logic.ZhuanPan
{
	
	public class ZhuanPanData
	{
		
		public object Mutex = new object();

		
		public bool ZhuanPanOpen;

		
		public List<List<int>> ZhuanPanConstArray;

		
		public int ZhuanPanFree;

		
		public int ZhuanPanZuanShiFuLi;

		
		public DateTime BeginTime;

		
		public DateTime EndTime;

		
		public List<ZhuanPanItem> ZhuanPanItemXmlList;

		
		public Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> ZhuanPanAwardXmlDict;

		
		public List<ZhuanPanGongGaoData> GongGaoList = new List<ZhuanPanGongGaoData>();
	}
}
