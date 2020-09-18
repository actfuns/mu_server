using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class BianShenManagerData
	{
		
		public object Mutex = new object();

		
		public bool IsGongNengOpend;

		
		public int FreeNum;

		
		public int TransfigurationBuff;

		
		public int BianShenCDSecs;

		
		public List<List<int>> NeedGoods;

		
		public int[] BianShenFull;

		
		public TemplateLoader<Dictionary<int, FashionEffectInfo>> FashionEffectInfoDict = new TemplateLoader<Dictionary<int, FashionEffectInfo>>();

		
		public TemplateLoader<Dictionary<int, BianShenStarInfo>> BianShenStarDict = new TemplateLoader<Dictionary<int, BianShenStarInfo>>();

		
		public Dictionary<int, List<BianShenStarInfo>> BianShenUpDict = new Dictionary<int, List<BianShenStarInfo>>();
	}
}
