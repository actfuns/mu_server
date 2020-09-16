using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class JingLingYuanSuInfo
	{
		
		public int ID;

		
		public int YuanSuType;

		
		public int ShuXingType;

		
		public int QiangHuaLevel;

		
		public int JieXingCurrency;

		
		public double Success;

		
		public string Attribute;

		
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> NeedGoods;

		
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> Failtofail;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtProps = new double[177];
	}
}
