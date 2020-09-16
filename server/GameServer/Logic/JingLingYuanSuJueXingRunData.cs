using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class JingLingYuanSuJueXingRunData
	{
		
		public object Mutex = new object();

		
		public TemplateLoader<Dictionary<int, JingLingYuanSuInfo>> YuanSuInfoDict = new TemplateLoader<Dictionary<int, JingLingYuanSuInfo>>();

		
		public TemplateLoader<Dictionary<int, JingLingYuanSuShuXingInfo>> ShuXingInfoDict = new TemplateLoader<Dictionary<int, JingLingYuanSuShuXingInfo>>();
	}
}
