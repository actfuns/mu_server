using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class HuiJiManagerData
	{
		
		public object Mutex = new object();

		
		public bool IsGongNengOpend;

		
		public int[] EmblemFull;

		
		public double[] EmblemShengXing;

		
		public TemplateLoader<Dictionary<int, EmblemStarInfo>> EmblemStarDict = new TemplateLoader<Dictionary<int, EmblemStarInfo>>();

		
		public TemplateLoader<Dictionary<int, EmblemUpInfo>> EmblemUpDict = new TemplateLoader<Dictionary<int, EmblemUpInfo>>();
	}
}
