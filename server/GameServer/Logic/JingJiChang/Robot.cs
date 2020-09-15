using System;
using System.Collections.Generic;
using GameServer.Logic.JingJiChang.FSM;
using Server.Data;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000736 RID: 1846
	public class Robot : Monster
	{
		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x00285B80 File Offset: 0x00283D80
		// (set) Token: 0x06002DAA RID: 11690 RVA: 0x00285B98 File Offset: 0x00283D98
		public double FrozenPercent
		{
			get
			{
				return this._FrozenPercent;
			}
			set
			{
				this._FrozenPercent = value;
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06002DAB RID: 11691 RVA: 0x00285BA4 File Offset: 0x00283DA4
		// (set) Token: 0x06002DAC RID: 11692 RVA: 0x00285BBC File Offset: 0x00283DBC
		public double PalsyPercent
		{
			get
			{
				return this._PalsyPercent;
			}
			set
			{
				this._PalsyPercent = value;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06002DAD RID: 11693 RVA: 0x00285BC8 File Offset: 0x00283DC8
		// (set) Token: 0x06002DAE RID: 11694 RVA: 0x00285BE0 File Offset: 0x00283DE0
		public double SpeedDownPercent
		{
			get
			{
				return this._SpeedDownPercent;
			}
			set
			{
				this._SpeedDownPercent = value;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06002DAF RID: 11695 RVA: 0x00285BEC File Offset: 0x00283DEC
		// (set) Token: 0x06002DB0 RID: 11696 RVA: 0x00285C04 File Offset: 0x00283E04
		public double BlowPercent
		{
			get
			{
				return this._BlowPercent;
			}
			set
			{
				this._BlowPercent = value;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06002DB1 RID: 11697 RVA: 0x00285C10 File Offset: 0x00283E10
		// (set) Token: 0x06002DB2 RID: 11698 RVA: 0x00285C28 File Offset: 0x00283E28
		public double DeFrozenPercent
		{
			get
			{
				return this._DeFrozenPercent;
			}
			set
			{
				this._DeFrozenPercent = value;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06002DB3 RID: 11699 RVA: 0x00285C34 File Offset: 0x00283E34
		// (set) Token: 0x06002DB4 RID: 11700 RVA: 0x00285C4C File Offset: 0x00283E4C
		public double DePalsyPercent
		{
			get
			{
				return this._DePalsyPercent;
			}
			set
			{
				this._DePalsyPercent = value;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06002DB5 RID: 11701 RVA: 0x00285C58 File Offset: 0x00283E58
		// (set) Token: 0x06002DB6 RID: 11702 RVA: 0x00285C70 File Offset: 0x00283E70
		public double DeSpeedDownPercent
		{
			get
			{
				return this._DeSpeedDownPercent;
			}
			set
			{
				this._DeSpeedDownPercent = value;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06002DB7 RID: 11703 RVA: 0x00285C7C File Offset: 0x00283E7C
		// (set) Token: 0x06002DB8 RID: 11704 RVA: 0x00285C94 File Offset: 0x00283E94
		public double DeBlowPercent
		{
			get
			{
				return this._DeBlowPercent;
			}
			set
			{
				this._DeBlowPercent = value;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06002DB9 RID: 11705 RVA: 0x00285CA0 File Offset: 0x00283EA0
		// (set) Token: 0x06002DBA RID: 11706 RVA: 0x00285CB8 File Offset: 0x00283EB8
		public double WaterPenetration
		{
			get
			{
				return this._WaterPenetration;
			}
			set
			{
				this._WaterPenetration = value;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06002DBB RID: 11707 RVA: 0x00285CC4 File Offset: 0x00283EC4
		// (set) Token: 0x06002DBC RID: 11708 RVA: 0x00285CDC File Offset: 0x00283EDC
		public double FirePenetration
		{
			get
			{
				return this._FirePenetration;
			}
			set
			{
				this._FirePenetration = value;
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06002DBD RID: 11709 RVA: 0x00285CE8 File Offset: 0x00283EE8
		// (set) Token: 0x06002DBE RID: 11710 RVA: 0x00285D00 File Offset: 0x00283F00
		public double WindPenetration
		{
			get
			{
				return this._WindPenetration;
			}
			set
			{
				this._WindPenetration = value;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06002DBF RID: 11711 RVA: 0x00285D0C File Offset: 0x00283F0C
		// (set) Token: 0x06002DC0 RID: 11712 RVA: 0x00285D24 File Offset: 0x00283F24
		public double SoilPenetration
		{
			get
			{
				return this._SoilPenetration;
			}
			set
			{
				this._SoilPenetration = value;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06002DC1 RID: 11713 RVA: 0x00285D30 File Offset: 0x00283F30
		// (set) Token: 0x06002DC2 RID: 11714 RVA: 0x00285D48 File Offset: 0x00283F48
		public double IcePenetration
		{
			get
			{
				return this._IcePenetration;
			}
			set
			{
				this._IcePenetration = value;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06002DC3 RID: 11715 RVA: 0x00285D54 File Offset: 0x00283F54
		// (set) Token: 0x06002DC4 RID: 11716 RVA: 0x00285D6C File Offset: 0x00283F6C
		public double LightningPenetration
		{
			get
			{
				return this._LightningPenetration;
			}
			set
			{
				this._LightningPenetration = value;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x00285D78 File Offset: 0x00283F78
		// (set) Token: 0x06002DC6 RID: 11718 RVA: 0x00285D90 File Offset: 0x00283F90
		public double DeWaterPenetration
		{
			get
			{
				return this._DeWaterPenetration;
			}
			set
			{
				this._DeWaterPenetration = value;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06002DC7 RID: 11719 RVA: 0x00285D9C File Offset: 0x00283F9C
		// (set) Token: 0x06002DC8 RID: 11720 RVA: 0x00285DB4 File Offset: 0x00283FB4
		public double DeFirePenetration
		{
			get
			{
				return this._DeFirePenetration;
			}
			set
			{
				this._DeFirePenetration = value;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06002DC9 RID: 11721 RVA: 0x00285DC0 File Offset: 0x00283FC0
		// (set) Token: 0x06002DCA RID: 11722 RVA: 0x00285DD8 File Offset: 0x00283FD8
		public double DeWindPenetration
		{
			get
			{
				return this._DeWindPenetration;
			}
			set
			{
				this._DeWindPenetration = value;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06002DCB RID: 11723 RVA: 0x00285DE4 File Offset: 0x00283FE4
		// (set) Token: 0x06002DCC RID: 11724 RVA: 0x00285DFC File Offset: 0x00283FFC
		public double DeSoilPenetration
		{
			get
			{
				return this._DeSoilPenetration;
			}
			set
			{
				this._DeSoilPenetration = value;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06002DCD RID: 11725 RVA: 0x00285E08 File Offset: 0x00284008
		// (set) Token: 0x06002DCE RID: 11726 RVA: 0x00285E20 File Offset: 0x00284020
		public double DeIcePenetration
		{
			get
			{
				return this._DeIcePenetration;
			}
			set
			{
				this._DeIcePenetration = value;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06002DCF RID: 11727 RVA: 0x00285E2C File Offset: 0x0028402C
		// (set) Token: 0x06002DD0 RID: 11728 RVA: 0x00285E44 File Offset: 0x00284044
		public double DeLightningPenetration
		{
			get
			{
				return this._DeLightningPenetration;
			}
			set
			{
				this._DeLightningPenetration = value;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06002DD1 RID: 11729 RVA: 0x00285E50 File Offset: 0x00284050
		// (set) Token: 0x06002DD2 RID: 11730 RVA: 0x00285E68 File Offset: 0x00284068
		public double ElementPenetration
		{
			get
			{
				return this._ElementPenetration;
			}
			set
			{
				this._ElementPenetration = value;
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06002DD3 RID: 11731 RVA: 0x00285E74 File Offset: 0x00284074
		// (set) Token: 0x06002DD4 RID: 11732 RVA: 0x00285E8C File Offset: 0x0028408C
		public int WaterAttack
		{
			get
			{
				return this._WaterAttack;
			}
			set
			{
				this._WaterAttack = value;
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06002DD5 RID: 11733 RVA: 0x00285E98 File Offset: 0x00284098
		// (set) Token: 0x06002DD6 RID: 11734 RVA: 0x00285EB0 File Offset: 0x002840B0
		public int FireAttack
		{
			get
			{
				return this._FireAttack;
			}
			set
			{
				this._FireAttack = value;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06002DD7 RID: 11735 RVA: 0x00285EBC File Offset: 0x002840BC
		// (set) Token: 0x06002DD8 RID: 11736 RVA: 0x00285ED4 File Offset: 0x002840D4
		public int WindAttack
		{
			get
			{
				return this._WindAttack;
			}
			set
			{
				this._WindAttack = value;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06002DD9 RID: 11737 RVA: 0x00285EE0 File Offset: 0x002840E0
		// (set) Token: 0x06002DDA RID: 11738 RVA: 0x00285EF8 File Offset: 0x002840F8
		public int SoilAttack
		{
			get
			{
				return this._SoilAttack;
			}
			set
			{
				this._SoilAttack = value;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06002DDB RID: 11739 RVA: 0x00285F04 File Offset: 0x00284104
		// (set) Token: 0x06002DDC RID: 11740 RVA: 0x00285F1C File Offset: 0x0028411C
		public int IceAttack
		{
			get
			{
				return this._IceAttack;
			}
			set
			{
				this._IceAttack = value;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06002DDD RID: 11741 RVA: 0x00285F28 File Offset: 0x00284128
		// (set) Token: 0x06002DDE RID: 11742 RVA: 0x00285F40 File Offset: 0x00284140
		public int LightningAttack
		{
			get
			{
				return this._LightningAttack;
			}
			set
			{
				this._LightningAttack = value;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06002DDF RID: 11743 RVA: 0x00285F4C File Offset: 0x0028414C
		// (set) Token: 0x06002DE0 RID: 11744 RVA: 0x00285F64 File Offset: 0x00284164
		public int Lucky
		{
			get
			{
				return this._lucky;
			}
			set
			{
				this._lucky = value;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06002DE1 RID: 11745 RVA: 0x00285F70 File Offset: 0x00284170
		// (set) Token: 0x06002DE2 RID: 11746 RVA: 0x00285F88 File Offset: 0x00284188
		public int FatalValue
		{
			get
			{
				return this._fatalValue;
			}
			set
			{
				this._fatalValue = value;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06002DE3 RID: 11747 RVA: 0x00285F94 File Offset: 0x00284194
		// (set) Token: 0x06002DE4 RID: 11748 RVA: 0x00285FAC File Offset: 0x002841AC
		public int DoubleValue
		{
			get
			{
				return this._doubleValue;
			}
			set
			{
				this._doubleValue = value;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06002DE5 RID: 11749 RVA: 0x00285FB8 File Offset: 0x002841B8
		// (set) Token: 0x06002DE6 RID: 11750 RVA: 0x00285FD0 File Offset: 0x002841D0
		public double DeLucky
		{
			get
			{
				return this._deLucky;
			}
			set
			{
				this._deLucky = value;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06002DE7 RID: 11751 RVA: 0x00285FDC File Offset: 0x002841DC
		// (set) Token: 0x06002DE8 RID: 11752 RVA: 0x00285FF4 File Offset: 0x002841F4
		public double DeFatalValue
		{
			get
			{
				return this._deFatalValue;
			}
			set
			{
				this._deFatalValue = value;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06002DE9 RID: 11753 RVA: 0x00286000 File Offset: 0x00284200
		// (set) Token: 0x06002DEA RID: 11754 RVA: 0x00286018 File Offset: 0x00284218
		public double DeDoubleValue
		{
			get
			{
				return this._deDoubleValue;
			}
			set
			{
				this._deDoubleValue = value;
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06002DEB RID: 11755 RVA: 0x00286024 File Offset: 0x00284224
		// (set) Token: 0x06002DEC RID: 11756 RVA: 0x0028603C File Offset: 0x0028423C
		public double RuthlessValue
		{
			get
			{
				return this._ruthlessValue;
			}
			set
			{
				this._ruthlessValue = value;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06002DED RID: 11757 RVA: 0x00286048 File Offset: 0x00284248
		// (set) Token: 0x06002DEE RID: 11758 RVA: 0x00286060 File Offset: 0x00284260
		public double ColdValue
		{
			get
			{
				return this._coldValue;
			}
			set
			{
				this._coldValue = value;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06002DEF RID: 11759 RVA: 0x0028606C File Offset: 0x0028426C
		// (set) Token: 0x06002DF0 RID: 11760 RVA: 0x00286084 File Offset: 0x00284284
		public double SavageValue
		{
			get
			{
				return this._savageValue;
			}
			set
			{
				this._savageValue = value;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x00286090 File Offset: 0x00284290
		// (set) Token: 0x06002DF2 RID: 11762 RVA: 0x002860A8 File Offset: 0x002842A8
		public double DeRuthlessValue
		{
			get
			{
				return this._deRuthlessValue;
			}
			set
			{
				this._deRuthlessValue = value;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06002DF3 RID: 11763 RVA: 0x002860B4 File Offset: 0x002842B4
		// (set) Token: 0x06002DF4 RID: 11764 RVA: 0x002860CC File Offset: 0x002842CC
		public double DeColdValue
		{
			get
			{
				return this._deColdValue;
			}
			set
			{
				this._deColdValue = value;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06002DF5 RID: 11765 RVA: 0x002860D8 File Offset: 0x002842D8
		// (set) Token: 0x06002DF6 RID: 11766 RVA: 0x002860F0 File Offset: 0x002842F0
		public double DeSavageValue
		{
			get
			{
				return this._deSavageValue;
			}
			set
			{
				this._deSavageValue = value;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06002DF7 RID: 11767 RVA: 0x002860FC File Offset: 0x002842FC
		// (set) Token: 0x06002DF8 RID: 11768 RVA: 0x00286114 File Offset: 0x00284314
		public int Sex
		{
			get
			{
				return this._sex;
			}
			set
			{
				this._sex = value;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06002DF9 RID: 11769 RVA: 0x00286120 File Offset: 0x00284320
		// (set) Token: 0x06002DFA RID: 11770 RVA: 0x00286138 File Offset: 0x00284338
		public int PlayerId
		{
			get
			{
				return this._playerId;
			}
			set
			{
				this._playerId = value;
			}
		}

		// Token: 0x06002DFB RID: 11771 RVA: 0x00286144 File Offset: 0x00284344
		public Robot(GameClient player, RoleDataMini roleDataMini)
		{
			base.MonsterInfo = new MonsterStaticInfo();
			base.MonsterType = 1801;
			this.roleDataMini = roleDataMini;
			this.FSM = new FinishStateMachine(player, this);
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x0028623C File Offset: 0x0028443C
		public RoleDataMini getRoleDataMini()
		{
			return this.roleDataMini;
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06002DFD RID: 11773 RVA: 0x00286254 File Offset: 0x00284454
		// (set) Token: 0x06002DFE RID: 11774 RVA: 0x00286271 File Offset: 0x00284471
		public new List<int> PassiveEffectList
		{
			get
			{
				return this.roleDataMini.PassiveEffectList;
			}
			set
			{
				this.roleDataMini.PassiveEffectList = value;
			}
		}

		// Token: 0x06002DFF RID: 11775 RVA: 0x00286280 File Offset: 0x00284480
		public void onUpdate()
		{
			this.FSM.onUpdate();
		}

		// Token: 0x06002E00 RID: 11776 RVA: 0x0028628F File Offset: 0x0028448F
		public void startAttack()
		{
			this.FSM.switchState(AIState.ATTACK);
		}

		// Token: 0x06002E01 RID: 11777 RVA: 0x0028629F File Offset: 0x0028449F
		public void stopAttack()
		{
			this.FSM.switchState(AIState.RETURN);
		}

		// Token: 0x04003BF2 RID: 15346
		private FinishStateMachine FSM = null;

		// Token: 0x04003BF3 RID: 15347
		private RoleDataMini roleDataMini = null;

		// Token: 0x04003BF4 RID: 15348
		public Dictionary<int, int> skillInfos = new Dictionary<int, int>();

		// Token: 0x04003BF5 RID: 15349
		private int _sex;

		// Token: 0x04003BF6 RID: 15350
		private int _playerId;

		// Token: 0x04003BF7 RID: 15351
		private int _lucky = 0;

		// Token: 0x04003BF8 RID: 15352
		private int _fatalValue = 0;

		// Token: 0x04003BF9 RID: 15353
		private int _doubleValue = 0;

		// Token: 0x04003BFA RID: 15354
		private double _deLucky = 0.0;

		// Token: 0x04003BFB RID: 15355
		private double _deFatalValue = 0.0;

		// Token: 0x04003BFC RID: 15356
		private double _deDoubleValue = 0.0;

		// Token: 0x04003BFD RID: 15357
		private double _ruthlessValue = 0.0;

		// Token: 0x04003BFE RID: 15358
		private double _coldValue = 0.0;

		// Token: 0x04003BFF RID: 15359
		private double _savageValue = 0.0;

		// Token: 0x04003C00 RID: 15360
		private double _deRuthlessValue = 0.0;

		// Token: 0x04003C01 RID: 15361
		private double _deColdValue = 0.0;

		// Token: 0x04003C02 RID: 15362
		private double _deSavageValue = 0.0;

		// Token: 0x04003C03 RID: 15363
		private double _FrozenPercent;

		// Token: 0x04003C04 RID: 15364
		private double _PalsyPercent;

		// Token: 0x04003C05 RID: 15365
		private double _SpeedDownPercent;

		// Token: 0x04003C06 RID: 15366
		private double _BlowPercent;

		// Token: 0x04003C07 RID: 15367
		private double _DeFrozenPercent;

		// Token: 0x04003C08 RID: 15368
		private double _DePalsyPercent;

		// Token: 0x04003C09 RID: 15369
		private double _DeSpeedDownPercent;

		// Token: 0x04003C0A RID: 15370
		private double _DeBlowPercent;

		// Token: 0x04003C0B RID: 15371
		private int _WaterAttack;

		// Token: 0x04003C0C RID: 15372
		private int _FireAttack;

		// Token: 0x04003C0D RID: 15373
		private int _WindAttack;

		// Token: 0x04003C0E RID: 15374
		private int _SoilAttack;

		// Token: 0x04003C0F RID: 15375
		private int _IceAttack;

		// Token: 0x04003C10 RID: 15376
		private int _LightningAttack;

		// Token: 0x04003C11 RID: 15377
		private double _WaterPenetration;

		// Token: 0x04003C12 RID: 15378
		private double _FirePenetration;

		// Token: 0x04003C13 RID: 15379
		private double _WindPenetration;

		// Token: 0x04003C14 RID: 15380
		private double _SoilPenetration;

		// Token: 0x04003C15 RID: 15381
		private double _IcePenetration;

		// Token: 0x04003C16 RID: 15382
		private double _LightningPenetration;

		// Token: 0x04003C17 RID: 15383
		private double _DeWaterPenetration;

		// Token: 0x04003C18 RID: 15384
		private double _DeFirePenetration;

		// Token: 0x04003C19 RID: 15385
		private double _DeWindPenetration;

		// Token: 0x04003C1A RID: 15386
		private double _DeSoilPenetration;

		// Token: 0x04003C1B RID: 15387
		private double _DeIcePenetration;

		// Token: 0x04003C1C RID: 15388
		private double _DeLightningPenetration;

		// Token: 0x04003C1D RID: 15389
		private double _ElementPenetration;
	}
}
