using System;
using System.Text;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;

namespace GameServer.Logic.ElementsAttack
{
	// Token: 0x0200029B RID: 667
	public class ElementsAttackManager
	{
		// Token: 0x060009EA RID: 2538 RVA: 0x0009DDA8 File Offset: 0x0009BFA8
		public string CalcElementInjureLog(IObject attacker, IObject defender, double injurePercent)
		{
			StringBuilder sb = new StringBuilder();
			double dmgAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 1);
			double dmgSubPercent = PassiveEffectManager.GetPassiveEffectAddPercent(defender, 5, 2);
			double penetAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 3);
			for (int i = 1; i <= 6; i++)
			{
				double nElementInjure = (double)this.CalcElementDamage(attacker, defender, (EElementDamageType)i, dmgAddPercent, dmgSubPercent, penetAddPercent) * injurePercent;
				sb.AppendFormat("{0}:{1}  ", ElementsAttackManager.ElementAttrName[i], nElementInjure);
			}
			return sb.ToString();
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0009DE2C File Offset: 0x0009C02C
		public int CalcAllElementDamage(IObject attacker, IObject defender)
		{
			int result;
			if (!(attacker is GameClient) && !(attacker is Robot))
			{
				result = 0;
			}
			else if (!(defender is GameClient) && !(defender is Robot) && !(defender is Monster))
			{
				result = 0;
			}
			else
			{
				double dmgAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 1);
				double dmgSubPercent = PassiveEffectManager.GetPassiveEffectAddPercent(defender, 5, 2);
				double penetAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 3);
				int nElementInjure = 0;
				for (int i = 1; i <= 6; i++)
				{
					nElementInjure += this.CalcElementDamage(attacker, defender, (EElementDamageType)i, dmgAddPercent, dmgSubPercent, penetAddPercent);
				}
				result = nElementInjure;
			}
			return result;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0009DED0 File Offset: 0x0009C0D0
		private int CalcElementDamage(IObject attacker, IObject defender, EElementDamageType eEDT, double dmgAddPercent = 0.0, double dmgSubPercent = 0.0, double penetAddPercent = 0.0)
		{
			double AtkPenetration = this.GetElementDamagePenetration(attacker, eEDT) + penetAddPercent;
			double DefPenetration = this.GetDeElementDamagePenetration(defender, eEDT);
			double AtkEnhance = this.GetElementEnhance(attacker, eEDT);
			double DefReduce = this.GetDeElementReduce(defender, eEDT);
			double rate = 1.0 + (AtkPenetration - DefPenetration);
			double factor = Global.GMax(0.0, 1.0 + AtkEnhance - DefReduce);
			factor = Global.GMin(2.0, factor);
			rate = Global.GMax(0.01, rate);
			rate = Global.GMin(1.0, rate);
			rate *= factor;
			int nElementInjure = this.GetElementAttack(attacker, eEDT);
			nElementInjure = (int)((double)nElementInjure * rate);
			if (attacker.ObjectType == ObjectTypes.OT_CLIENT)
			{
				double percent = 1.0 + RoleAlgorithm.GetExtPropValue(attacker as GameClient, ExtPropIndexes.ElementInjurePercent);
				if (defender.ObjectType == ObjectTypes.OT_CLIENT)
				{
					percent -= RoleAlgorithm.GetExtPropValue(defender as GameClient, ExtPropIndexes.ElementAttackInjurePercent);
					percent -= dmgSubPercent;
				}
				percent += dmgAddPercent;
				nElementInjure = (int)((double)nElementInjure * percent);
			}
			return (nElementInjure > 0) ? nElementInjure : 0;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0009DFFC File Offset: 0x0009C1FC
		public double GetElementDamagePenetration(IObject attacker, EElementDamageType eEDT)
		{
			double val = 0.0;
			if (attacker is GameClient)
			{
				GameClient attackClient = attacker as GameClient;
				if (null == attackClient)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val += attackClient.ClientData.EquipProp.ExtProps[75];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(75);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				case EElementDamageType.EEDT_Water:
					val += attackClient.ClientData.EquipProp.ExtProps[76];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(76);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				case EElementDamageType.EEDT_Lightning:
					val += attackClient.ClientData.EquipProp.ExtProps[77];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(77);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				case EElementDamageType.EEDT_Soil:
					val += attackClient.ClientData.EquipProp.ExtProps[78];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(78);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				case EElementDamageType.EEDT_Ice:
					val += attackClient.ClientData.EquipProp.ExtProps[79];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(79);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				case EElementDamageType.EEDT_Wind:
					val += attackClient.ClientData.EquipProp.ExtProps[80];
					val += attackClient.ClientData.PropsCacheManager.GetExtProp(80);
					val += RoleAlgorithm.GetExtProp(attackClient, 118);
					break;
				}
			}
			else if (attacker is Robot)
			{
				Robot attackRobot = attacker as Robot;
				if (null == attackRobot)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val = attackRobot.FirePenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Water:
					val = attackRobot.WaterPenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Lightning:
					val = attackRobot.LightningPenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Soil:
					val = attackRobot.SoilPenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Ice:
					val = attackRobot.IcePenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Wind:
					val = attackRobot.WindPenetration;
					val += (attacker as Robot).ElementPenetration;
					break;
				}
			}
			return Math.Max(val, 0.0);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0009E2C8 File Offset: 0x0009C4C8
		public double GetDeElementDamagePenetration(IObject defender, EElementDamageType eEDT)
		{
			double val = 0.0;
			if (defender is GameClient)
			{
				GameClient defenderClient = defender as GameClient;
				if (null == defenderClient)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val += defenderClient.ClientData.EquipProp.ExtProps[81];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(81);
					break;
				case EElementDamageType.EEDT_Water:
					val += defenderClient.ClientData.EquipProp.ExtProps[82];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(82);
					break;
				case EElementDamageType.EEDT_Lightning:
					val += defenderClient.ClientData.EquipProp.ExtProps[83];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(83);
					break;
				case EElementDamageType.EEDT_Soil:
					val += defenderClient.ClientData.EquipProp.ExtProps[84];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(84);
					break;
				case EElementDamageType.EEDT_Ice:
					val += defenderClient.ClientData.EquipProp.ExtProps[85];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(85);
					break;
				case EElementDamageType.EEDT_Wind:
					val += defenderClient.ClientData.EquipProp.ExtProps[86];
					val += defenderClient.ClientData.PropsCacheManager.GetExtProp(86);
					break;
				}
			}
			else if (defender is Robot)
			{
				Robot defenderRobot = defender as Robot;
				if (null == defenderRobot)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val = defenderRobot.DeFirePenetration;
					break;
				case EElementDamageType.EEDT_Water:
					val = defenderRobot.DeWaterPenetration;
					break;
				case EElementDamageType.EEDT_Lightning:
					val = defenderRobot.DeLightningPenetration;
					break;
				case EElementDamageType.EEDT_Soil:
					val = defenderRobot.DeSoilPenetration;
					break;
				case EElementDamageType.EEDT_Ice:
					val = defenderRobot.DeIcePenetration;
					break;
				case EElementDamageType.EEDT_Wind:
					val = defenderRobot.DeWindPenetration;
					break;
				}
			}
			return Math.Max(val, 0.0);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0009E4F4 File Offset: 0x0009C6F4
		public int GetElementAttack(IObject attacker, EElementDamageType eEDT)
		{
			int val = 0;
			if (attacker is GameClient)
			{
				GameClient attackClient = attacker as GameClient;
				if (null == attackClient)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[69];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(69);
					break;
				case EElementDamageType.EEDT_Water:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[70];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(70);
					break;
				case EElementDamageType.EEDT_Lightning:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[71];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(71);
					break;
				case EElementDamageType.EEDT_Soil:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[72];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(72);
					break;
				case EElementDamageType.EEDT_Ice:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[73];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(73);
					break;
				case EElementDamageType.EEDT_Wind:
					val += (int)attackClient.ClientData.EquipProp.ExtProps[74];
					val += (int)attackClient.ClientData.PropsCacheManager.GetExtProp(74);
					break;
				}
			}
			else if (attacker is Robot)
			{
				Robot attackRobot = attacker as Robot;
				if (null == attackRobot)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val = attackRobot.FireAttack;
					break;
				case EElementDamageType.EEDT_Water:
					val = attackRobot.WaterAttack;
					break;
				case EElementDamageType.EEDT_Lightning:
					val = attackRobot.LightningAttack;
					break;
				case EElementDamageType.EEDT_Soil:
					val = attackRobot.SoilAttack;
					break;
				case EElementDamageType.EEDT_Ice:
					val = attackRobot.IceAttack;
					break;
				case EElementDamageType.EEDT_Wind:
					val = attackRobot.WindAttack;
					break;
				}
			}
			return Math.Max(val, 0);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0009E71C File Offset: 0x0009C91C
		public double GetElementEnhance(IObject attacker, EElementDamageType eEDT)
		{
			double val = 0.0;
			if (attacker is GameClient)
			{
				GameClient attackClient = attacker as GameClient;
				if (null == attackClient)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val = RoleAlgorithm.GetExtProp(attackClient, 106);
					break;
				case EElementDamageType.EEDT_Water:
					val = RoleAlgorithm.GetExtProp(attackClient, 107);
					break;
				case EElementDamageType.EEDT_Lightning:
					val = RoleAlgorithm.GetExtProp(attackClient, 108);
					break;
				case EElementDamageType.EEDT_Soil:
					val = RoleAlgorithm.GetExtProp(attackClient, 109);
					break;
				case EElementDamageType.EEDT_Ice:
					val = RoleAlgorithm.GetExtProp(attackClient, 110);
					break;
				case EElementDamageType.EEDT_Wind:
					val = RoleAlgorithm.GetExtProp(attackClient, 111);
					break;
				}
			}
			return val;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0009E7CC File Offset: 0x0009C9CC
		public double GetDeElementReduce(IObject attacker, EElementDamageType eEDT)
		{
			double val = 0.0;
			if (attacker is GameClient)
			{
				GameClient attackClient = attacker as GameClient;
				if (null == attackClient)
				{
					return val;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					val = RoleAlgorithm.GetExtProp(attackClient, 112);
					break;
				case EElementDamageType.EEDT_Water:
					val = RoleAlgorithm.GetExtProp(attackClient, 113);
					break;
				case EElementDamageType.EEDT_Lightning:
					val = RoleAlgorithm.GetExtProp(attackClient, 114);
					break;
				case EElementDamageType.EEDT_Soil:
					val = RoleAlgorithm.GetExtProp(attackClient, 115);
					break;
				case EElementDamageType.EEDT_Ice:
					val = RoleAlgorithm.GetExtProp(attackClient, 116);
					break;
				case EElementDamageType.EEDT_Wind:
					val = RoleAlgorithm.GetExtProp(attackClient, 117);
					break;
				}
			}
			return val;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x0009E87C File Offset: 0x0009CA7C
		public double GetJJCRobotExtProps(int nIndex, double[] extProps)
		{
			double result;
			if (nIndex > extProps.Length - 1)
			{
				result = 0.0;
			}
			else
			{
				result = extProps[nIndex];
			}
			return result;
		}

		// Token: 0x0400106E RID: 4206
		public const bool LogElementInjure = false;

		// Token: 0x0400106F RID: 4207
		public static readonly string[] ElementAttrName = new string[]
		{
			"unknown",
			"火伤",
			"水伤",
			"雷伤",
			"土伤",
			"冰伤",
			"风伤"
		};
	}
}
