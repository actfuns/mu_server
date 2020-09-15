using System;

namespace GameServer.Logic
{
	// Token: 0x02000763 RID: 1891
	public class MonsterStaticInfo
	{
		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06002FEA RID: 12266 RVA: 0x002AEBC8 File Offset: 0x002ACDC8
		// (set) Token: 0x06002FEB RID: 12267 RVA: 0x002AEBDF File Offset: 0x002ACDDF
		public string VSName { get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06002FEC RID: 12268 RVA: 0x002AEBE8 File Offset: 0x002ACDE8
		// (set) Token: 0x06002FED RID: 12269 RVA: 0x002AEBFF File Offset: 0x002ACDFF
		public int ExtensionID { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06002FEE RID: 12270 RVA: 0x002AEC08 File Offset: 0x002ACE08
		// (set) Token: 0x06002FEF RID: 12271 RVA: 0x002AEC1F File Offset: 0x002ACE1F
		public int VLevel { get; set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x002AEC28 File Offset: 0x002ACE28
		// (set) Token: 0x06002FF1 RID: 12273 RVA: 0x002AEC3F File Offset: 0x002ACE3F
		public int VExperience { get; set; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06002FF2 RID: 12274 RVA: 0x002AEC48 File Offset: 0x002ACE48
		// (set) Token: 0x06002FF3 RID: 12275 RVA: 0x002AEC5F File Offset: 0x002ACE5F
		public int VMoney { get; set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06002FF4 RID: 12276 RVA: 0x002AEC68 File Offset: 0x002ACE68
		// (set) Token: 0x06002FF5 RID: 12277 RVA: 0x002AEC7F File Offset: 0x002ACE7F
		public double VLifeMax { get; set; }

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06002FF6 RID: 12278 RVA: 0x002AEC88 File Offset: 0x002ACE88
		// (set) Token: 0x06002FF7 RID: 12279 RVA: 0x002AEC9F File Offset: 0x002ACE9F
		public double VManaMax { get; set; }

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06002FF8 RID: 12280 RVA: 0x002AECA8 File Offset: 0x002ACEA8
		// (set) Token: 0x06002FF9 RID: 12281 RVA: 0x002AECBF File Offset: 0x002ACEBF
		public int ToOccupation { get; set; }

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06002FFA RID: 12282 RVA: 0x002AECC8 File Offset: 0x002ACEC8
		// (set) Token: 0x06002FFB RID: 12283 RVA: 0x002AECDF File Offset: 0x002ACEDF
		public int[] SpriteSpeedTickList { get; set; }

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06002FFC RID: 12284 RVA: 0x002AECE8 File Offset: 0x002ACEE8
		// (set) Token: 0x06002FFD RID: 12285 RVA: 0x002AECFF File Offset: 0x002ACEFF
		public int[] EachActionFrameRange { get; set; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06002FFE RID: 12286 RVA: 0x002AED08 File Offset: 0x002ACF08
		// (set) Token: 0x06002FFF RID: 12287 RVA: 0x002AED1F File Offset: 0x002ACF1F
		public int[] EffectiveFrame { get; set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06003000 RID: 12288 RVA: 0x002AED28 File Offset: 0x002ACF28
		// (set) Token: 0x06003001 RID: 12289 RVA: 0x002AED3F File Offset: 0x002ACF3F
		public int SeekRange { get; set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06003002 RID: 12290 RVA: 0x002AED48 File Offset: 0x002ACF48
		// (set) Token: 0x06003003 RID: 12291 RVA: 0x002AED5F File Offset: 0x002ACF5F
		public int EquipmentBody { get; set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06003004 RID: 12292 RVA: 0x002AED68 File Offset: 0x002ACF68
		// (set) Token: 0x06003005 RID: 12293 RVA: 0x002AED7F File Offset: 0x002ACF7F
		public int EquipmentWeapon { get; set; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06003006 RID: 12294 RVA: 0x002AED88 File Offset: 0x002ACF88
		// (set) Token: 0x06003007 RID: 12295 RVA: 0x002AED9F File Offset: 0x002ACF9F
		public int MinAttack { get; set; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06003008 RID: 12296 RVA: 0x002AEDA8 File Offset: 0x002ACFA8
		// (set) Token: 0x06003009 RID: 12297 RVA: 0x002AEDBF File Offset: 0x002ACFBF
		public int MaxAttack { get; set; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600300A RID: 12298 RVA: 0x002AEDC8 File Offset: 0x002ACFC8
		// (set) Token: 0x0600300B RID: 12299 RVA: 0x002AEDDF File Offset: 0x002ACFDF
		public int Defense { get; set; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x0600300C RID: 12300 RVA: 0x002AEDE8 File Offset: 0x002ACFE8
		// (set) Token: 0x0600300D RID: 12301 RVA: 0x002AEDFF File Offset: 0x002ACFFF
		public int MDefense { get; set; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600300E RID: 12302 RVA: 0x002AEE08 File Offset: 0x002AD008
		// (set) Token: 0x0600300F RID: 12303 RVA: 0x002AEE1F File Offset: 0x002AD01F
		public double HitV { get; set; }

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06003010 RID: 12304 RVA: 0x002AEE28 File Offset: 0x002AD028
		// (set) Token: 0x06003011 RID: 12305 RVA: 0x002AEE3F File Offset: 0x002AD03F
		public double Dodge { get; set; }

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06003012 RID: 12306 RVA: 0x002AEE48 File Offset: 0x002AD048
		// (set) Token: 0x06003013 RID: 12307 RVA: 0x002AEE5F File Offset: 0x002AD05F
		public double RecoverLifeV { get; set; }

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06003014 RID: 12308 RVA: 0x002AEE68 File Offset: 0x002AD068
		// (set) Token: 0x06003015 RID: 12309 RVA: 0x002AEE7F File Offset: 0x002AD07F
		public double RecoverMagicV { get; set; }

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06003016 RID: 12310 RVA: 0x002AEE88 File Offset: 0x002AD088
		// (set) Token: 0x06003017 RID: 12311 RVA: 0x002AEE9F File Offset: 0x002AD09F
		public double MonsterDamageThornPercent { get; set; }

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06003018 RID: 12312 RVA: 0x002AEEA8 File Offset: 0x002AD0A8
		// (set) Token: 0x06003019 RID: 12313 RVA: 0x002AEEBF File Offset: 0x002AD0BF
		public double MonsterDamageThorn { get; set; }

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x0600301A RID: 12314 RVA: 0x002AEEC8 File Offset: 0x002AD0C8
		// (set) Token: 0x0600301B RID: 12315 RVA: 0x002AEEDF File Offset: 0x002AD0DF
		public double MonsterSubAttackInjurePercent { get; set; }

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x0600301C RID: 12316 RVA: 0x002AEEE8 File Offset: 0x002AD0E8
		// (set) Token: 0x0600301D RID: 12317 RVA: 0x002AEEFF File Offset: 0x002AD0FF
		public double MonsterSubAttackInjure { get; set; }

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x0600301E RID: 12318 RVA: 0x002AEF08 File Offset: 0x002AD108
		// (set) Token: 0x0600301F RID: 12319 RVA: 0x002AEF1F File Offset: 0x002AD11F
		public double MonsterIgnoreDefensePercent { get; set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06003020 RID: 12320 RVA: 0x002AEF28 File Offset: 0x002AD128
		// (set) Token: 0x06003021 RID: 12321 RVA: 0x002AEF3F File Offset: 0x002AD13F
		public double MonsterIgnoreDefenseRate { get; set; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06003022 RID: 12322 RVA: 0x002AEF48 File Offset: 0x002AD148
		// (set) Token: 0x06003023 RID: 12323 RVA: 0x002AEF5F File Offset: 0x002AD15F
		public double MonsterLucky { get; set; }

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06003024 RID: 12324 RVA: 0x002AEF68 File Offset: 0x002AD168
		// (set) Token: 0x06003025 RID: 12325 RVA: 0x002AEF7F File Offset: 0x002AD17F
		public double MonsterFatalAttack { get; set; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06003026 RID: 12326 RVA: 0x002AEF88 File Offset: 0x002AD188
		// (set) Token: 0x06003027 RID: 12327 RVA: 0x002AEF9F File Offset: 0x002AD19F
		public double MonsterDoubleAttack { get; set; }

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06003028 RID: 12328 RVA: 0x002AEFA8 File Offset: 0x002AD1A8
		// (set) Token: 0x06003029 RID: 12329 RVA: 0x002AEFBF File Offset: 0x002AD1BF
		public int FallGoodsPackID { get; set; }

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x0600302A RID: 12330 RVA: 0x002AEFC8 File Offset: 0x002AD1C8
		// (set) Token: 0x0600302B RID: 12331 RVA: 0x002AEFDF File Offset: 0x002AD1DF
		public int AttackType { get; set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x0600302C RID: 12332 RVA: 0x002AEFE8 File Offset: 0x002AD1E8
		// (set) Token: 0x0600302D RID: 12333 RVA: 0x002AEFFF File Offset: 0x002AD1FF
		public int BattlePersonalJiFen { get; set; }

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x0600302E RID: 12334 RVA: 0x002AF008 File Offset: 0x002AD208
		// (set) Token: 0x0600302F RID: 12335 RVA: 0x002AF01F File Offset: 0x002AD21F
		public int BattleZhenYingJiFen { get; set; }

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06003030 RID: 12336 RVA: 0x002AF028 File Offset: 0x002AD228
		// (set) Token: 0x06003031 RID: 12337 RVA: 0x002AF03F File Offset: 0x002AD23F
		public int DaimonSquareJiFen { get; set; }

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06003032 RID: 12338 RVA: 0x002AF048 File Offset: 0x002AD248
		// (set) Token: 0x06003033 RID: 12339 RVA: 0x002AF05F File Offset: 0x002AD25F
		public int BloodCastJiFen { get; set; }

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06003034 RID: 12340 RVA: 0x002AF068 File Offset: 0x002AD268
		// (set) Token: 0x06003035 RID: 12341 RVA: 0x002AF07F File Offset: 0x002AD27F
		public int WolfScore { get; set; }

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06003036 RID: 12342 RVA: 0x002AF088 File Offset: 0x002AD288
		// (set) Token: 0x06003037 RID: 12343 RVA: 0x002AF09F File Offset: 0x002AD29F
		public int FallBelongTo { get; set; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06003038 RID: 12344 RVA: 0x002AF0A8 File Offset: 0x002AD2A8
		// (set) Token: 0x06003039 RID: 12345 RVA: 0x002AF0BF File Offset: 0x002AD2BF
		public int[] SkillIDs { get; set; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x0600303A RID: 12346 RVA: 0x002AF0C8 File Offset: 0x002AD2C8
		// (set) Token: 0x0600303B RID: 12347 RVA: 0x002AF0DF File Offset: 0x002AD2DF
		public int Camp { get; set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600303C RID: 12348 RVA: 0x002AF0E8 File Offset: 0x002AD2E8
		// (set) Token: 0x0600303D RID: 12349 RVA: 0x002AF0FF File Offset: 0x002AD2FF
		public int AIID { get; set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600303E RID: 12350 RVA: 0x002AF108 File Offset: 0x002AD308
		// (set) Token: 0x0600303F RID: 12351 RVA: 0x002AF11F File Offset: 0x002AD31F
		public int ChangeLifeCount { get; set; }

		// Token: 0x06003040 RID: 12352 RVA: 0x002AF128 File Offset: 0x002AD328
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

		// Token: 0x04003CF4 RID: 15604
		public int RebornExp;

		// Token: 0x04003CF5 RID: 15605
		public double[] ExtProps;
	}
}
