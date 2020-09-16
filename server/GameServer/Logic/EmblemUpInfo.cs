using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "EmblemUp", "ID")]
	public class EmblemUpInfo
	{
		
		public int ID;

		
		public int EmblemLevel;

		
		public int LuckyOne;

		
		public int LuckyTwo;

		
		public double LuckyTwoRate;

		
		public int DurationTime;

		
		public int CDTime;

		
		public double SubAttackInjurePercent;

		
		public double SPAttackInjurePercent;

		
		public double AttackInjurePercent;

		
		public double ElementAttackInjurePercent;

		
		public int LifeV;

		
		public int AddAttack;

		
		public int AddDefense;

		
		public int DecreaseInjureValue;

		
		public List<int> NeedGoods;

		
		public int NeedDiamond;

		
		[TemplateMappingField(Exclude = true)]
		public int MaxStarLevel;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropTempValues = new double[177];
	}
}
