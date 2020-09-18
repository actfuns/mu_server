using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class ArmorManagerData
	{
		
		public object Mutex = new object();

		
		public double[] HudunBaoji;

		
		public TemplateLoader<Dictionary<int, ArmorStarInfo>> ArmorStarDict = new TemplateLoader<Dictionary<int, ArmorStarInfo>>();

		
		public TemplateLoader<Dictionary<int, ArmorUpInfo>> ArmorUpDict = new TemplateLoader<Dictionary<int, ArmorUpInfo>>();
	}
}
