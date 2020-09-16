using System;

namespace GameServer.Logic.TuJian
{
	
	internal class _AttrValue
	{
		
		public _AttrValue Add(_AttrValue other)
		{
			if (other != null)
			{
				this.MinDefense += other.MinDefense;
				this.MaxDefense += other.MaxDefense;
				this.MinMDefense += other.MinMDefense;
				this.MaxMDefense += other.MaxMDefense;
				this.MinAttack += other.MinAttack;
				this.MaxAttack += other.MaxAttack;
				this.MinMAttack += other.MinMAttack;
				this.MaxMAttack += other.MaxMAttack;
				this.HitV += other.HitV;
				this.Dodge += other.Dodge;
				this.MaxLifeV += other.MaxLifeV;
			}
			return this;
		}

		
		public int MinDefense = 0;

		
		public int MaxDefense = 0;

		
		public int MinMDefense = 0;

		
		public int MaxMDefense = 0;

		
		public int MinAttack = 0;

		
		public int MaxAttack = 0;

		
		public int MinMAttack = 0;

		
		public int MaxMAttack = 0;

		
		public int HitV = 0;

		
		public int Dodge = 0;

		
		public int MaxLifeV = 0;
	}
}
