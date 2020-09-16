using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "JingLing", "ID")]
	public class ArmorUpInfo
	{
		
		public int ID;

		
		public int ArmorClass;

		
		public int LuckyOne;

		
		public int LuckyTwo;

		
		public double LuckyTwoRate;

		
		public double Damageabsorption;

		
		public double Armorrecovery;

		
		[TemplateMappingField(Exclude = true)]
		public int ArmorUp;

		
		[TemplateMappingField(Exclude = true)]
		public int AddAttack;

		
		[TemplateMappingField(Exclude = true)]
		public int AddDefense;

		
		[TemplateMappingField(Exclude = true)]
		public int ShenmingUP;

		
		public List<int> NeedGoods;

		
		public int NeedDiamond;

		
		[TemplateMappingField(Exclude = true)]
		public int MaxStarLevel;
	}
}
