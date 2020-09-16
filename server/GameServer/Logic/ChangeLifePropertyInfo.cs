using System;

namespace GameServer.Logic
{
	
	public class ChangeLifePropertyInfo
	{
		
		public void AddFrom(ChangeLifePropertyInfo info)
		{
			this.AddPhyAttackMinValue += info.AddPhyAttackMinValue;
			this.AddPhyAttackMaxValue += info.AddPhyAttackMaxValue;
			this.AddMagAttackMinValue += info.AddMagAttackMinValue;
			this.AddMagAttackMaxValue += info.AddMagAttackMaxValue;
			this.AddPhyDefenseMinValue += info.AddPhyDefenseMinValue;
			this.AddPhyDefenseMaxValue += info.AddPhyDefenseMaxValue;
			this.AddMagDefenseMinValue += info.AddMagDefenseMinValue;
			this.AddMagDefenseMaxValue += info.AddMagDefenseMaxValue;
			this.AddHitPropValue += info.AddHitPropValue;
			this.AddDodgePropValue += info.AddDodgePropValue;
			this.AddMaxLifePropValue += info.AddMaxLifePropValue;
		}

		
		public int PhyAttackMin = 0;

		
		public int PhyAttackMax = 0;

		
		public int MagAttackMin = 0;

		
		public int MagAttackMax = 0;

		
		public int PhyDefenseMin = 0;

		
		public int PhyDefenseMax = 0;

		
		public int MagDefenseMin = 0;

		
		public int MagDefenseMax = 0;

		
		public int HitProp = 0;

		
		public int DodgeProp = 0;

		
		public int MaxLifeProp = 0;

		
		public int AddPhyAttackMinValue = 0;

		
		public int AddPhyAttackMaxValue = 0;

		
		public int AddMagAttackMinValue = 0;

		
		public int AddMagAttackMaxValue = 0;

		
		public int AddPhyDefenseMinValue = 0;

		
		public int AddPhyDefenseMaxValue = 0;

		
		public int AddMagDefenseMinValue = 0;

		
		public int AddMagDefenseMaxValue = 0;

		
		public int AddHitPropValue = 0;

		
		public int AddDodgePropValue = 0;

		
		public int AddMaxLifePropValue = 0;
	}
}
