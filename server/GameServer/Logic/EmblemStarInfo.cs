using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "EmblemStar", "ID")]
	public class EmblemStarInfo
	{
		
		public int ID;

		
		public int EmblemLevel;

		
		public int EmblemStar;

		
		public int LifeV;

		
		public int AddAttack;

		
		public int AddDefense;

		
		public int DecreaseInjureValue;

		
		public int StarExp;

		
		public int GoodsExp;

		
		public int ZuanShiExp;

		
		public List<int> NeedGoods;

		
		public int NeedDiamond;

		
		[TemplateMappingField(Exclude = true)]
		public EmblemUpInfo EmblemUpInfo;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
