using System;
using System.Collections.Generic;
using GameServer.Logic.JingJiChang.FSM;
using Server.Data;

namespace GameServer.Logic.JingJiChang
{
	
	public class Robot : Monster
	{
		
		
		
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

		
		public Robot(GameClient player, RoleDataMini roleDataMini)
		{
			base.MonsterInfo = new MonsterStaticInfo();
			base.MonsterType = 1801;
			this.roleDataMini = roleDataMini;
			this.FSM = new FinishStateMachine(player, this);
		}

		
		public RoleDataMini getRoleDataMini()
		{
			return this.roleDataMini;
		}

		
		
		
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

		
		public void onUpdate()
		{
			this.FSM.onUpdate();
		}

		
		public void startAttack()
		{
			this.FSM.switchState(AIState.ATTACK);
		}

		
		public void stopAttack()
		{
			this.FSM.switchState(AIState.RETURN);
		}

		
		private FinishStateMachine FSM = null;

		
		private RoleDataMini roleDataMini = null;

		
		public Dictionary<int, int> skillInfos = new Dictionary<int, int>();

		
		private int _sex;

		
		private int _playerId;

		
		private int _lucky = 0;

		
		private int _fatalValue = 0;

		
		private int _doubleValue = 0;

		
		private double _deLucky = 0.0;

		
		private double _deFatalValue = 0.0;

		
		private double _deDoubleValue = 0.0;

		
		private double _ruthlessValue = 0.0;

		
		private double _coldValue = 0.0;

		
		private double _savageValue = 0.0;

		
		private double _deRuthlessValue = 0.0;

		
		private double _deColdValue = 0.0;

		
		private double _deSavageValue = 0.0;

		
		private double _FrozenPercent;

		
		private double _PalsyPercent;

		
		private double _SpeedDownPercent;

		
		private double _BlowPercent;

		
		private double _DeFrozenPercent;

		
		private double _DePalsyPercent;

		
		private double _DeSpeedDownPercent;

		
		private double _DeBlowPercent;

		
		private int _WaterAttack;

		
		private int _FireAttack;

		
		private int _WindAttack;

		
		private int _SoilAttack;

		
		private int _IceAttack;

		
		private int _LightningAttack;

		
		private double _WaterPenetration;

		
		private double _FirePenetration;

		
		private double _WindPenetration;

		
		private double _SoilPenetration;

		
		private double _IcePenetration;

		
		private double _LightningPenetration;

		
		private double _DeWaterPenetration;

		
		private double _DeFirePenetration;

		
		private double _DeWindPenetration;

		
		private double _DeSoilPenetration;

		
		private double _DeIcePenetration;

		
		private double _DeLightningPenetration;

		
		private double _ElementPenetration;
	}
}
