using System;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.MerlinMagicBook
{
	// Token: 0x02000387 RID: 903
	public class MerlinInjureManager
	{
		// Token: 0x06000F71 RID: 3953 RVA: 0x000F26B8 File Offset: 0x000F08B8
		public int CalcMerlinInjure(IObject attacker, IObject defender, int nBaseInjure, ref EMerlinSecretAttrType eref)
		{
			eref = EMerlinSecretAttrType.EMSAT_None;
			try
			{
				if (!(attacker is GameClient) && !(attacker is Robot))
				{
					return 0;
				}
				if (!(defender is GameClient) && !(defender is Robot) && !(defender is Monster))
				{
					return 0;
				}
				if (nBaseInjure <= 0)
				{
					return 0;
				}
				double percent = 1.0;
				eref = this.GetMerlinInjureType(attacker, defender, ref percent);
				if (eref == EMerlinSecretAttrType.EMSAT_None)
				{
					return 0;
				}
				return this.TriggerEffect(attacker, defender, nBaseInjure, eref, percent);
			}
			catch (Exception ex)
			{
				if (attacker is GameClient)
				{
					GameClient client = attacker as GameClient;
					if (null != client)
					{
						DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
			return 0;
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x000F27C0 File Offset: 0x000F09C0
		private EMerlinSecretAttrType GetMerlinInjureType(IObject attacker, IObject defender, ref double percent)
		{
			try
			{
				double dSpeedDownRate = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_SpeedDownP);
				double dFrozenRate = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_FrozenP);
				double dBlowRate = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_BlowP);
				double dPalsyRate = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_PalsyP);
				double[] rateArr = new double[]
				{
					dSpeedDownRate,
					dFrozenRate,
					dBlowRate,
					dPalsyRate
				};
				switch (RoleAlgorithm.GetRateIndexPercent(rateArr))
				{
				case 0:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 58);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_SpeedDownP;
				case 1:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 56);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_FrozenP;
				case 2:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 59);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_BlowP;
				case 3:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 57);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_PalsyP;
				default:
					return EMerlinSecretAttrType.EMSAT_None;
				}
			}
			catch (Exception ex)
			{
				if (attacker is GameClient)
				{
					GameClient client = attacker as GameClient;
					if (null != client)
					{
						DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
			return EMerlinSecretAttrType.EMSAT_None;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x000F2980 File Offset: 0x000F0B80
		public double GetMerlinInjurePercent(IObject attacker, IObject defender, EMerlinSecretAttrType eType)
		{
			double val = 0.0;
			try
			{
				switch (eType)
				{
				case EMerlinSecretAttrType.EMSAT_FrozenP:
					val += RoleAlgorithm.GetFrozenPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_PalsyP:
					val += RoleAlgorithm.GetPalsyPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_SpeedDownP:
					val += RoleAlgorithm.GetSpeedDownPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_BlowP:
					val += RoleAlgorithm.GetBlowPercent(attacker);
					break;
				}
				if (defender is Robot || defender is GameClient)
				{
					switch (eType)
					{
					case EMerlinSecretAttrType.EMSAT_FrozenP:
						val -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 97) : (defender as Robot).DeFrozenPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_PalsyP:
						val -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 98) : (defender as Robot).DePalsyPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_SpeedDownP:
						val -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 99) : (defender as Robot).DeSpeedDownPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_BlowP:
						val -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 100) : (defender as Robot).DeBlowPercent);
						break;
					}
				}
				return (val > 0.0) ? val : 0.0;
			}
			catch (Exception ex)
			{
				if (attacker is GameClient)
				{
					GameClient client = attacker as GameClient;
					if (null != client)
					{
						DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
			return (val > 0.0) ? val : 0.0;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x000F2B5C File Offset: 0x000F0D5C
		private int TriggerEffect(IObject attacker, IObject defender, int nBaseInjure, EMerlinSecretAttrType eType, double percent)
		{
			int nInjure = 0;
			try
			{
				switch (eType)
				{
				case EMerlinSecretAttrType.EMSAT_FrozenP:
				{
					nInjure = (int)((double)nBaseInjure * 0.5);
					double[] param = new double[]
					{
						0.99,
						2.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_FROZEN, param, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_PalsyP:
				{
					nInjure = (int)((double)nBaseInjure * 0.5);
					double[] param = new double[]
					{
						0.99,
						1.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_PALSY, param, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_SpeedDownP:
				{
					nInjure = (int)((double)nBaseInjure * 0.5);
					double[] param = new double[]
					{
						0.3,
						4.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, param, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_BlowP:
					nInjure = nBaseInjure;
					break;
				default:
					return 0;
				}
				return nInjure;
			}
			catch (Exception ex)
			{
				if (attacker is GameClient)
				{
					GameClient client = attacker as GameClient;
					if (null != client)
					{
						DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
			return nInjure;
		}
	}
}
