using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "TransfigurationLevel", "ID")]
	public class BianShenStarInfo
	{
		
		public int ID;

		
		public List<int> OccupationID;

		
		public int Level;

		
		public string ProPerty;

		
		public int UpExp;

		
		public int GoodsExp;

		
		public double ExpCritRate;

		
		public double ExpCritTimes;

		
		[TemplateMappingField(SpliteChars = "|,")]
		public List<List<int>> NeedGoods;

		
		public List<int> AttackSkill;

		
		public List<int> MagicSkill;

		
		public int Duration;

		
		[TemplateMappingField(Exclude = true)]
		public int NeedDiamond;

		
		[TemplateMappingField(Exclude = true)]
		public int ZuanShiExp;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
