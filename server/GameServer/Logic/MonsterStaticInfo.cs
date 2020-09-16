using System;

namespace GameServer.Logic
{
	
	public class MonsterStaticInfo
	{
		
		
		
		public string VSName { get; set; }

		
		
		
		public int ExtensionID { get; set; }

		
		
		
		public int VLevel { get; set; }

		
		
		
		public int VExperience { get; set; }

		
		
		
		public int VMoney { get; set; }

		
		
		
		public double VLifeMax { get; set; }

		
		
		
		public double VManaMax { get; set; }

		
		
		
		public int ToOccupation { get; set; }

		
		
		
		public int[] SpriteSpeedTickList { get; set; }

		
		
		
		public int[] EachActionFrameRange { get; set; }

		
		
		
		public int[] EffectiveFrame { get; set; }

		
		
		
		public int SeekRange { get; set; }

		
		
		
		public int EquipmentBody { get; set; }

		
		
		
		public int EquipmentWeapon { get; set; }

		
		
		
		public int MinAttack { get; set; }

		
		
		
		public int MaxAttack { get; set; }

		
		
		
		public int Defense { get; set; }

		
		
		
		public int MDefense { get; set; }

		
		
		
		public double HitV { get; set; }

		
		
		
		public double Dodge { get; set; }

		
		
		
		public double RecoverLifeV { get; set; }

		
		
		
		public double RecoverMagicV { get; set; }

		
		
		
		public double MonsterDamageThornPercent { get; set; }

		
		
		
		public double MonsterDamageThorn { get; set; }

		
		
		
		public double MonsterSubAttackInjurePercent { get; set; }

		
		
		
		public double MonsterSubAttackInjure { get; set; }

		
		
		
		public double MonsterIgnoreDefensePercent { get; set; }

		
		
		
		public double MonsterIgnoreDefenseRate { get; set; }

		
		
		
		public double MonsterLucky { get; set; }

		
		
		
		public double MonsterFatalAttack { get; set; }

		
		
		
		public double MonsterDoubleAttack { get; set; }

		
		
		
		public int FallGoodsPackID { get; set; }

		
		
		
		public int AttackType { get; set; }

		
		
		
		public int BattlePersonalJiFen { get; set; }

		
		
		
		public int BattleZhenYingJiFen { get; set; }

		
		
		
		public int DaimonSquareJiFen { get; set; }

		
		
		
		public int BloodCastJiFen { get; set; }

		
		
		
		public int WolfScore { get; set; }

		
		
		
		public int FallBelongTo { get; set; }

		
		
		
		public int[] SkillIDs { get; set; }

		
		
		
		public int Camp { get; set; }

		
		
		
		public int AIID { get; set; }

		
		
		
		public int ChangeLifeCount { get; set; }

		
		public MonsterStaticInfo Clone()
		{
			MonsterStaticInfo info = new MonsterStaticInfo();
			info.VSName = this.VSName;
			info.ExtensionID = this.ExtensionID;
			info.VLevel = this.VLevel;
			info.VExperience = this.VExperience;
			info.RebornExp = this.VExperience;
			info.ExtProps = this.ExtProps;
			info.VMoney = this.VMoney;
			info.VLifeMax = this.VLifeMax;
			info.VManaMax = this.VManaMax;
			info.ToOccupation = this.ToOccupation;
			if (null != this.SpriteSpeedTickList)
			{
				info.SpriteSpeedTickList = new int[this.SpriteSpeedTickList.Length];
				this.SpriteSpeedTickList.CopyTo(info.SpriteSpeedTickList, 0);
			}
			if (null != this.EachActionFrameRange)
			{
				info.EachActionFrameRange = new int[this.EachActionFrameRange.Length];
				this.EachActionFrameRange.CopyTo(info.EachActionFrameRange, 0);
			}
			if (null != this.EffectiveFrame)
			{
				info.EffectiveFrame = new int[this.EffectiveFrame.Length];
				this.EffectiveFrame.CopyTo(info.EffectiveFrame, 0);
			}
			info.SeekRange = this.SeekRange;
			info.EquipmentBody = this.EquipmentBody;
			info.EquipmentWeapon = this.EquipmentWeapon;
			info.MinAttack = this.MinAttack;
			info.MaxAttack = this.MaxAttack;
			info.Defense = this.Defense;
			info.MDefense = this.MDefense;
			info.HitV = this.HitV;
			info.Dodge = this.Dodge;
			info.RecoverLifeV = this.RecoverLifeV;
			info.RecoverMagicV = this.RecoverMagicV;
			info.MonsterDamageThornPercent = this.MonsterDamageThornPercent;
			info.MonsterDamageThorn = this.MonsterDamageThorn;
			info.MonsterSubAttackInjurePercent = this.MonsterSubAttackInjurePercent;
			info.MonsterSubAttackInjure = this.MonsterSubAttackInjure;
			info.MonsterIgnoreDefensePercent = this.MonsterIgnoreDefensePercent;
			info.MonsterIgnoreDefenseRate = this.MonsterIgnoreDefenseRate;
			info.MonsterLucky = this.MonsterLucky;
			info.MonsterFatalAttack = this.MonsterFatalAttack;
			info.MonsterDoubleAttack = this.MonsterDoubleAttack;
			info.FallGoodsPackID = this.FallGoodsPackID;
			info.AttackType = this.AttackType;
			info.BattlePersonalJiFen = this.BattlePersonalJiFen;
			info.BattleZhenYingJiFen = this.BattleZhenYingJiFen;
			info.DaimonSquareJiFen = this.DaimonSquareJiFen;
			info.BloodCastJiFen = this.BloodCastJiFen;
			info.FallBelongTo = this.FallBelongTo;
			if (null != this.SkillIDs)
			{
				info.SkillIDs = new int[this.SkillIDs.Length];
				this.SkillIDs.CopyTo(info.SkillIDs, 0);
			}
			info.Camp = this.Camp;
			info.AIID = this.AIID;
			info.ChangeLifeCount = this.ChangeLifeCount;
			if (this.ExtProps != null)
			{
				info.ExtProps = new double[177];
				Array.Copy(this.ExtProps, info.ExtProps, 177);
			}
			return info;
		}

		
		public int RebornExp;

		
		public double[] ExtProps;
	}
}
