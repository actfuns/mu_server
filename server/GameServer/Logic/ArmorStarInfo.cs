using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class ArmorStarInfo
	{
		
		public int ID;

		
		public int ArmorupStage;

		
		public int StarLevel;

		
		public int ArmorUp;

		
		public int AddAttack;

		
		public int AddDefense;

		
		public int ShenmingUP;

		
		public int StarExp;

		
		public int GoodsExp;

		
		public int ZuanShiExp;

		
		public List<int> NeedGoods;

		
		public int NeedDiamond;

		
		[TemplateMappingField(Exclude = true)]
		public ArmorUpInfo ArmorUpInfo;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
