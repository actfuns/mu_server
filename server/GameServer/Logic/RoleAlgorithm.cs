using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x020007C5 RID: 1989
	public class RoleAlgorithm
	{
		// Token: 0x0600341D RID: 13341 RVA: 0x002E18E0 File Offset: 0x002DFAE0
		static RoleAlgorithm()
		{
			RoleAlgorithm.ExtListArray = new List<ExtPropIndexes>[177];
			RoleAlgorithm.BaseListArray = new List<ExtPropIndexes>[4];
			RoleAlgorithm.CreateNewExtArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.ExtListArray);
			RoleAlgorithm.CreateNewBaseArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.BaseListArray);
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x002E4C40 File Offset: 0x002E2E40
		public static void CreateNewExtArray(Dictionary<ExtPropIndexes, ExtPropItem> oldDic, List<ExtPropIndexes>[] ExtArray)
		{
			foreach (ExtPropIndexes key in oldDic.Keys)
			{
				if (oldDic[key].ExtProp != ExtPropIndexes.Max)
				{
					if (ExtArray[(int)oldDic[key].ExtProp] == null)
					{
						ExtArray[(int)oldDic[key].ExtProp] = new List<ExtPropIndexes>();
					}
					ExtArray[(int)oldDic[key].ExtProp].Add(key);
				}
				if (oldDic[key].ExtPropPercent != ExtPropIndexes.Max)
				{
					if (ExtArray[(int)oldDic[key].ExtPropPercent] == null)
					{
						ExtArray[(int)oldDic[key].ExtPropPercent] = new List<ExtPropIndexes>();
					}
					ExtArray[(int)oldDic[key].ExtPropPercent].Add(key);
				}
			}
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x002E4D50 File Offset: 0x002E2F50
		public static void CreateNewBaseArray(Dictionary<ExtPropIndexes, ExtPropItem> oldDic, List<ExtPropIndexes>[] BaseArray)
		{
			foreach (ExtPropIndexes key in oldDic.Keys)
			{
				if (oldDic[key].UnitProp != UnitPropIndexes.Max)
				{
					if (BaseArray[(int)oldDic[key].UnitProp] == null)
					{
						BaseArray[(int)oldDic[key].UnitProp] = new List<ExtPropIndexes>();
					}
					BaseArray[(int)oldDic[key].UnitProp].Add(key);
				}
			}
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x002E4DFC File Offset: 0x002E2FFC
		public static bool NeedNotifyClient(ExtPropIndexes attribute)
		{
			return RoleAlgorithm.NotifyList.Contains(attribute);
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x002E4E1C File Offset: 0x002E301C
		public static double GetPureExtProp(GameClient client, int extProp)
		{
			double dValue = 0.0;
			ExtPropItem extPropItem = null;
			RoleAlgorithm.roleExtPropDic.TryGetValue((ExtPropIndexes)extProp, out extPropItem);
			double result;
			if (extPropItem == null)
			{
				result = 0.0;
			}
			else
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				dValue += roleBasePropItem.arrRoleExtProp[extProp];
				double addVal = 0.0;
				switch (extPropItem.UnitProp)
				{
				case UnitPropIndexes.Strength:
					addVal = RoleAlgorithm.GetStrength(client, true);
					break;
				case UnitPropIndexes.Intelligence:
					addVal = RoleAlgorithm.GetIntelligence(client, true);
					break;
				case UnitPropIndexes.Dexterity:
					addVal = RoleAlgorithm.GetDexterity(client, true);
					break;
				case UnitPropIndexes.Constitution:
					addVal = RoleAlgorithm.GetConstitution(client, true);
					break;
				case UnitPropIndexes.Max:
					addVal = 0.0;
					break;
				}
				if (extPropItem.UnitProp != UnitPropIndexes.Max)
				{
					addVal *= extPropItem.Coefficient[nOcc];
				}
				dValue += addVal;
				dValue += client.ClientData.EquipProp.ExtProps[extProp] + client.RoleBuffer.GetExtProp(extProp) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[extProp] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[extProp];
				dValue += client.AllThingsMultipliedBuffer.GetExtProp(extProp);
				dValue += client.ClientData.PropsCacheManager.GetExtProp(extProp);
				dValue += client.RoleMultipliedBuffer.GetExtProp(extProp);
				dValue += client.RoleOnceBuffer.GetExtProp(extProp);
				if (extPropItem.ExtProp != ExtPropIndexes.Max)
				{
					dValue += RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtProp);
				}
				if (extPropItem.ExcellentProp != null)
				{
					for (int i = 0; i < extPropItem.ExcellentProp.Length; i++)
					{
						dValue += client.ClientData.ExcellenceProp[(int)extPropItem.ExcellentProp[i]];
					}
				}
				if (extPropItem.BufferProp != null)
				{
					for (int i = 0; i < extPropItem.BufferProp.Length; i++)
					{
						dValue += DBRoleBufferManager.ProcessTimeAddProp(client, extPropItem.BufferProp[i]);
					}
				}
				if (extProp == 17)
				{
					dValue += client.ClientData.LuckProp * 0.01;
				}
				if (extPropItem.ExtPropPercent != ExtPropIndexes.Max)
				{
					double addPercent = RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtPropPercent);
					dValue *= 1.0 + addPercent;
				}
				result = dValue;
			}
			return result;
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x002E50A8 File Offset: 0x002E32A8
		public static double GetExtProp(GameClient client, int extProp)
		{
			double dValue = 0.0;
			ExtPropItem extPropItem = null;
			RoleAlgorithm.roleExtPropDic.TryGetValue((ExtPropIndexes)extProp, out extPropItem);
			double result;
			if (extPropItem == null)
			{
				result = 0.0;
			}
			else
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				dValue += roleBasePropItem.arrRoleExtProp[extProp];
				double addVal = 0.0;
				switch (extPropItem.UnitProp)
				{
				case UnitPropIndexes.Strength:
					addVal = RoleAlgorithm.GetStrength(client, true);
					break;
				case UnitPropIndexes.Intelligence:
					addVal = RoleAlgorithm.GetIntelligence(client, true);
					break;
				case UnitPropIndexes.Dexterity:
					addVal = RoleAlgorithm.GetDexterity(client, true);
					break;
				case UnitPropIndexes.Constitution:
					addVal = RoleAlgorithm.GetConstitution(client, true);
					break;
				case UnitPropIndexes.Max:
					addVal = 0.0;
					break;
				}
				if (extPropItem.UnitProp != UnitPropIndexes.Max)
				{
					addVal *= extPropItem.Coefficient[nOcc];
				}
				dValue += addVal;
				dValue += client.ClientData.EquipProp.ExtProps[extProp] + client.RoleBuffer.GetExtProp(extProp) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[extProp] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[extProp];
				dValue += client.AllThingsMultipliedBuffer.GetExtProp(extProp);
				dValue += client.ClientData.PropsCacheManager.GetExtProp(extProp);
				dValue += client.RoleMultipliedBuffer.GetExtProp(extProp);
				dValue += client.RoleOnceBuffer.GetExtProp(extProp);
				if (extPropItem.ExtProp != ExtPropIndexes.Max)
				{
					dValue += RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtProp);
				}
				if (extPropItem.ExcellentProp != null)
				{
					for (int i = 0; i < extPropItem.ExcellentProp.Length; i++)
					{
						dValue += client.ClientData.ExcellenceProp[(int)extPropItem.ExcellentProp[i]];
					}
				}
				dValue *= extPropItem.PropCoef;
				if (extPropItem.BufferProp != null)
				{
					for (int i = 0; i < extPropItem.BufferProp.Length; i++)
					{
						dValue += DBRoleBufferManager.ProcessTimeAddProp(client, extPropItem.BufferProp[i]);
					}
				}
				if (extProp == 17)
				{
					dValue += client.ClientData.LuckProp;
				}
				if (extPropItem.ExtPropPercent != ExtPropIndexes.Max)
				{
					double addPercent = RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtPropPercent);
					dValue *= 1.0 + addPercent;
				}
				dValue += client.ClientData.PurePropsCacheManager.GetExtProp(extProp);
				double modpct = client.ClientData.PctPropsCacheManager.GetExtProp(extProp);
				if (modpct > 0.0)
				{
					dValue *= modpct;
				}
				result = dValue;
			}
			return result;
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x002E5374 File Offset: 0x002E3574
		public static double GetBaseExtProp(GameClient client, ExtPropItem extPropItem)
		{
			double val = 0.0;
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return val;
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x002E53B0 File Offset: 0x002E35B0
		public static double GetStrength(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropStrength + client.RoleBuffer.GetBaseProp(0) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[0] + client.ClientData.PropsCacheManager.GetBaseProp(0);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
			}
			return dValue;
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x002E5424 File Offset: 0x002E3624
		public static double GetIntelligence(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropIntelligence + client.RoleBuffer.GetBaseProp(1) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[1] + client.ClientData.PropsCacheManager.GetBaseProp(1);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
			}
			return dValue;
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x002E5498 File Offset: 0x002E3698
		public static double GetDexterity(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropDexterity + client.RoleBuffer.GetBaseProp(2) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[2] + client.ClientData.PropsCacheManager.GetBaseProp(2);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
			}
			return dValue;
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x002E550C File Offset: 0x002E370C
		public static double GetConstitution(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropConstitution + client.RoleBuffer.GetBaseProp(3) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[3] + client.ClientData.PropsCacheManager.GetBaseProp(3);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
			}
			return dValue;
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x002E5610 File Offset: 0x002E3810
		public static double GetMagicSkillIncrease(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(33, delegate
			{
				double dValue = 0.0;
				int nOcc = Global.CalcOriginalOccupationID(client);
				EOccupationType eOcc = (EOccupationType)nOcc;
				if (EOccupationType.EOT_Magician == eOcc || EOccupationType.EOT_MagicSword == eOcc || EOccupationType.EOT_Summoner == eOcc)
				{
					RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
					dValue = RoleAlgorithm.GetStrength(client, true) / 100000.0;
					dValue += roleBasePropItem.MagicSkillIncreasePercent;
				}
				return dValue;
			});
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x002E56DC File Offset: 0x002E38DC
		public static double GetPhySkillIncrease(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(31, delegate
			{
				double dValue = 0.0;
				int nOcc = Global.CalcOriginalOccupationID(client);
				EOccupationType eOcc = (EOccupationType)nOcc;
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				if (eOcc == EOccupationType.EOT_Warrior || EOccupationType.EOT_Bow == eOcc || EOccupationType.EOT_MagicSword == eOcc)
				{
					dValue = RoleAlgorithm.GetIntelligence(client, true) / 100000.0;
				}
				return dValue + roleBasePropItem.PhySkillIncreasePercent;
			});
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x002E571C File Offset: 0x002E391C
		public static double GetAttackSpeed(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.AttackSpeed;
		}

		// Token: 0x0600342B RID: 13355 RVA: 0x002E57A4 File Offset: 0x002E39A4
		public static double GetAttackSpeedServer(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(1, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				return roleBasePropItem.AttackSpeed;
			});
		}

		// Token: 0x0600342C RID: 13356 RVA: 0x002E58EC File Offset: 0x002E3AEC
		public static double GetFatalAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(35, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[35] + client.RoleBuffer.GetExtProp(35) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[35] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[35];
				val += client.ClientData.PropsCacheManager.GetExtProp(35);
				val += client.ClientData.ExcellenceProp[0];
				val += client.ClientData.ExcellenceProp[18];
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPFATALATTACK);
				val *= 100.0;
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.FatalAttack);
				return val + client.RoleMultipliedBuffer.GetExtProp(35);
			});
		}

		// Token: 0x0600342D RID: 13357 RVA: 0x002E592C File Offset: 0x002E3B2C
		public static double GetDeFatalAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(52);
			val += client.ClientData.ExcellenceProp[30];
			return val * 100.0;
		}

		// Token: 0x0600342E RID: 13358 RVA: 0x002E597C File Offset: 0x002E3B7C
		public static double GetFatalHurt(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(90);
		}

		// Token: 0x0600342F RID: 13359 RVA: 0x002E59B0 File Offset: 0x002E3BB0
		public static double GetDeLuckyAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(51);
			val += client.ClientData.ExcellenceProp[29];
			return val * 100.0;
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x002E5A00 File Offset: 0x002E3C00
		public static double GetFatalAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterFatalAttack * 100.0;
		}

		// Token: 0x06003431 RID: 13361 RVA: 0x002E5B24 File Offset: 0x002E3D24
		public static double GetDoubleAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(36, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[36] + client.RoleBuffer.GetExtProp(36) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[36] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[36];
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDOUBLEATTACK);
				val += client.ClientData.ExcellenceProp[23];
				val += client.ClientData.PropsCacheManager.GetExtProp(36);
				val *= 100.0;
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DoubleAttack);
				return val + client.RoleMultipliedBuffer.GetExtProp(36);
			});
		}

		// Token: 0x06003432 RID: 13362 RVA: 0x002E5B64 File Offset: 0x002E3D64
		public static double GetDeDoubleAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(53);
			val += client.ClientData.ExcellenceProp[31];
			return val * 100.0;
		}

		// Token: 0x06003433 RID: 13363 RVA: 0x002E5BB4 File Offset: 0x002E3DB4
		public static double GetSavagePercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(61);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003434 RID: 13364 RVA: 0x002E5C00 File Offset: 0x002E3E00
		public static double GetDeSavagePercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(64);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003435 RID: 13365 RVA: 0x002E5C4C File Offset: 0x002E3E4C
		public static double GetColdPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(62);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x002E5C98 File Offset: 0x002E3E98
		public static double GetDeColdPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(65);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x002E5CE4 File Offset: 0x002E3EE4
		public static double GetRuthlessPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(63);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003438 RID: 13368 RVA: 0x002E5D30 File Offset: 0x002E3F30
		public static double GetDeRuthlessPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(66);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		// Token: 0x06003439 RID: 13369 RVA: 0x002E5D7C File Offset: 0x002E3F7C
		public static double GetDoubleAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterDoubleAttack * 100.0;
		}

		// Token: 0x0600343A RID: 13370 RVA: 0x002E5E68 File Offset: 0x002E4068
		public static double GetMoveSpeed(GameClient client)
		{
			double result;
			if (client.RoleBuffer.GetExtProp(47) > 0.1)
			{
				result = 0.0;
			}
			else
			{
				result = client.propsCacheModule.GetExtPropsValue(2, delegate
				{
					double val = 1.0;
					val = val * (1.0 + client.ClientData.EquipProp.ExtProps[2]) * (1.0 + client.RoleBuffer.GetExtProp(2));
					val += client.ClientData.PropsCacheManager.GetExtProp(2);
					val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MoveSpeed);
					val += client.RoleMultipliedBuffer.GetExtProp(2);
					if (val < 0.0)
					{
						val = 0.0;
					}
					return val;
				});
			}
			return result;
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x002E5FE0 File Offset: 0x002E41E0
		public static double GetDamageThornPercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(29, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[29] + client.RoleBuffer.GetExtProp(29) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[29] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[29];
				val += client.AllThingsMultipliedBuffer.GetExtProp(29);
				val += client.ClientData.PropsCacheManager.GetExtProp(29);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThornPercent);
				val += client.RoleMultipliedBuffer.GetExtProp(29);
				val += client.ClientData.ExcellenceProp[11];
				return val + client.ClientData.ExcellenceProp[28];
			});
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x002E6020 File Offset: 0x002E4220
		public static double GetDamageThornPercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterDamageThornPercent;
		}

		// Token: 0x0600343D RID: 13373 RVA: 0x002E6114 File Offset: 0x002E4314
		public static double GetDamageThorn(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(30, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[30] + client.RoleBuffer.GetExtProp(30) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[30] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[30];
				val += client.ClientData.PropsCacheManager.GetExtProp(30);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThorn);
				val += client.RoleMultipliedBuffer.GetExtProp(30);
				return Global.GMax(0.0, val);
			});
		}

		// Token: 0x0600343E RID: 13374 RVA: 0x002E6154 File Offset: 0x002E4354
		public static double GetDamageThorn(Monster monster)
		{
			double val = monster.MonsterInfo.MonsterDamageThorn;
			return Global.GMax(0.0, val);
		}

		// Token: 0x0600343F RID: 13375 RVA: 0x002E623C File Offset: 0x002E443C
		public static double GetStrong(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(0, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[0] + client.RoleBuffer.GetExtProp(0) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[0] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[0];
				val += client.AllThingsMultipliedBuffer.GetExtProp(0);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Strong);
				return val + client.RoleMultipliedBuffer.GetExtProp(0);
			});
		}

		// Token: 0x06003440 RID: 13376 RVA: 0x002E6278 File Offset: 0x002E4478
		public static double GetStrong(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x06003441 RID: 13377 RVA: 0x002E6294 File Offset: 0x002E4494
		public static double GetMinADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 3);
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x002E62B0 File Offset: 0x002E44B0
		public static double GetMinADefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.Defense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x002E62EC File Offset: 0x002E44EC
		public static double GetMaxADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 4);
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x002E63C8 File Offset: 0x002E45C8
		public static double GetIncreasePhyDefense(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(42, delegate
			{
				double addPercent = DBRoleBufferManager.ProcessAddTempDefense(client);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(42);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(92);
				addPercent += client.RoleBuffer.GetExtProp(42);
				addPercent += client.ClientData.EquipProp.ExtProps[92];
				return addPercent + (client.ClientData.ExcellenceProp[13] + client.ClientData.ExcellenceProp[27]);
			});
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x002E6408 File Offset: 0x002E4608
		public static double GetMaxADefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.Defense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		// Token: 0x06003446 RID: 13382 RVA: 0x002E6444 File Offset: 0x002E4644
		public static double GetMinMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 5);
		}

		// Token: 0x06003447 RID: 13383 RVA: 0x002E6520 File Offset: 0x002E4720
		public static double GetIncreaseMagDefense(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(43, delegate
			{
				double addPercent = DBRoleBufferManager.ProcessAddTempDefense(client);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(43);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(92);
				addPercent += client.RoleBuffer.GetExtProp(43);
				addPercent += client.ClientData.EquipProp.ExtProps[92];
				return addPercent + (client.ClientData.ExcellenceProp[13] + client.ClientData.ExcellenceProp[27]);
			});
		}

		// Token: 0x06003448 RID: 13384 RVA: 0x002E6560 File Offset: 0x002E4760
		public static double GetMinMDefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.MDefense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x002E659C File Offset: 0x002E479C
		public static double GetMaxMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 6);
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x002E65B8 File Offset: 0x002E47B8
		public static double GetMaxMDefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.MDefense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		// Token: 0x0600344B RID: 13387 RVA: 0x002E65F4 File Offset: 0x002E47F4
		public static double GetMinAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 7);
		}

		// Token: 0x0600344C RID: 13388 RVA: 0x002E6610 File Offset: 0x002E4810
		public static double GetMinAttackV(Monster monster)
		{
			double attackVal = (double)monster.MonsterInfo.MinAttack;
			return attackVal * (1.0 + monster.TempPropsBuffer.GetExtProp(11));
		}

		// Token: 0x0600344D RID: 13389 RVA: 0x002E664C File Offset: 0x002E484C
		public static double GetMaxAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 8);
		}

		// Token: 0x0600344E RID: 13390 RVA: 0x002E6744 File Offset: 0x002E4944
		public static double GetIncreasePhyAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(11, delegate
			{
				double addPercent = DBRoleBufferManager.ProcessAddTempAttack(client);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(11);
				addPercent += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreasePhyAttack);
				addPercent += client.RoleBuffer.GetExtProp(11);
				addPercent += client.ClientData.ExcellenceProp[4];
				addPercent += client.ClientData.ExcellenceProp[3];
				addPercent += client.ClientData.ExcellenceProp[24];
				addPercent += client.ClientData.EquipProp.ExtProps[91];
				return addPercent + client.ClientData.PropsCacheManager.GetExtProp(91);
			});
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x002E6784 File Offset: 0x002E4984
		public static double GetMaxAttackV(Monster monster)
		{
			int attackVal = monster.MonsterInfo.MaxAttack;
			return (double)attackVal;
		}

		// Token: 0x06003450 RID: 13392 RVA: 0x002E67A4 File Offset: 0x002E49A4
		public static double GetMinMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 9);
		}

		// Token: 0x06003451 RID: 13393 RVA: 0x002E67C0 File Offset: 0x002E49C0
		public static double GetMinMagicAttackV(Monster monster)
		{
			double attackVal = (double)monster.MonsterInfo.MinAttack;
			return attackVal * (1.0 + monster.TempPropsBuffer.GetExtProp(12));
		}

		// Token: 0x06003452 RID: 13394 RVA: 0x002E67FC File Offset: 0x002E49FC
		public static double GetMaxMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 10);
		}

		// Token: 0x06003453 RID: 13395 RVA: 0x002E68F4 File Offset: 0x002E4AF4
		public static double GetIncreaseMagAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(12, delegate
			{
				double addPercent = DBRoleBufferManager.ProcessAddTempAttack(client);
				addPercent += client.ClientData.PropsCacheManager.GetExtProp(12);
				addPercent += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreaseMagAttack);
				addPercent += client.RoleBuffer.GetExtProp(12);
				addPercent += client.ClientData.ExcellenceProp[4];
				addPercent += client.ClientData.ExcellenceProp[3];
				addPercent += client.ClientData.ExcellenceProp[24];
				addPercent += client.ClientData.EquipProp.ExtProps[91];
				return addPercent + client.ClientData.PropsCacheManager.GetExtProp(91);
			});
		}

		// Token: 0x06003454 RID: 13396 RVA: 0x002E6934 File Offset: 0x002E4B34
		public static double GetMaxMagicAttackV(Monster monster)
		{
			int attackVal = monster.MonsterInfo.MaxAttack;
			return (double)attackVal;
		}

		// Token: 0x06003455 RID: 13397 RVA: 0x002E6BAC File Offset: 0x002E4DAC
		public static double GetMaxLifeV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(13, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				EOccupationType eOcc = (EOccupationType)nOcc;
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				double val = roleBasePropItem.LifeV;
				if (EOccupationType.EOT_Warrior == eOcc)
				{
					val += RoleAlgorithm.GetConstitution(client, true) * 5.0;
				}
				else if (EOccupationType.EOT_Magician == eOcc)
				{
					val += RoleAlgorithm.GetConstitution(client, true) * 3.6;
				}
				else if (EOccupationType.EOT_Bow == eOcc)
				{
					val += RoleAlgorithm.GetConstitution(client, true) * 4.2;
				}
				else if (EOccupationType.EOT_MagicSword == eOcc)
				{
					val += RoleAlgorithm.GetConstitution(client, true) * 4.4;
				}
				else if (EOccupationType.EOT_Summoner == eOcc)
				{
					val += RoleAlgorithm.GetConstitution(client, true) * 3.4;
				}
				val = val + client.ClientData.EquipProp.ExtProps[13] + client.RoleBuffer.GetExtProp(13) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[13] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[13];
				val += client.AllThingsMultipliedBuffer.GetExtProp(13);
				val += client.ClientData.PropsCacheManager.GetExtProp(13);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifeV);
				val += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxLifeV);
				val += client.RoleMultipliedBuffer.GetExtProp(13);
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXHPVALUE);
				double addPercent = RoleAlgorithm.GetMaxLifePercentV(client);
				val *= Math.Max(0.0, 1.0 + addPercent);
				return val + client.ClientData.PurePropsCacheManager.GetExtProp(13);
			});
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x002E6D30 File Offset: 0x002E4F30
		public static double GetMaxLifePercentV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(14, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[14] + client.RoleBuffer.GetExtProp(14) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[14] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[14];
				val += client.AllThingsMultipliedBuffer.GetExtProp(14);
				val += client.ClientData.PropsCacheManager.GetExtProp(14);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifePercent);
				val += client.RoleMultipliedBuffer.GetExtProp(14);
				val += client.ClientData.ExcellenceProp[8];
				val += client.ClientData.ExcellenceProp[20];
				val += DBRoleBufferManager.ProcessUpLifeLimit(client);
				double modpct = client.ClientData.PctPropsCacheManager.GetExtProp(14);
				if (modpct > 0.0)
				{
					val *= modpct;
				}
				return val;
			});
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x002E6E64 File Offset: 0x002E5064
		public static double GetLifeStealV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(44, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[44] + client.RoleBuffer.GetExtProp(44) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[44] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[44];
				val += client.AllThingsMultipliedBuffer.GetExtProp(44);
				val += client.ClientData.PropsCacheManager.GetExtProp(44);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeSteal);
				val += client.RoleMultipliedBuffer.GetExtProp(44);
				double per = RoleAlgorithm.GetLifeStealPercentV(client);
				return val * (1.0 + per);
			});
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x002E6EA4 File Offset: 0x002E50A4
		public static double GetLifeStealPercentV(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(67);
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x002E6ED8 File Offset: 0x002E50D8
		public static double GetPotionPercentV(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(68);
		}

		// Token: 0x0600345A RID: 13402 RVA: 0x002E6FE8 File Offset: 0x002E51E8
		public static double GetAddAttackV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(45, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[45] + client.RoleBuffer.GetExtProp(45) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[45] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[45];
				val += client.AllThingsMultipliedBuffer.GetExtProp(45);
				val += client.ClientData.PropsCacheManager.GetExtProp(45);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttack);
				return val + client.RoleMultipliedBuffer.GetExtProp(45);
			});
		}

		// Token: 0x0600345B RID: 13403 RVA: 0x002E7104 File Offset: 0x002E5304
		public static double GetAddDefenseV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(46, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[46] + client.RoleBuffer.GetExtProp(46) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[46] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[46];
				val += client.AllThingsMultipliedBuffer.GetExtProp(46);
				val += client.ClientData.PropsCacheManager.GetExtProp(46);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddDefense);
				return val + client.RoleMultipliedBuffer.GetExtProp(46);
			});
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x002E7144 File Offset: 0x002E5344
		public static double GetAddAttackPercent(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(91);
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x002E7178 File Offset: 0x002E5378
		public static double GetAddDefensePercent(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(92);
		}

		// Token: 0x0600345E RID: 13406 RVA: 0x002E72E4 File Offset: 0x002E54E4
		public static double GetMaxMagicV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(15, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				double val = roleBasePropItem.MagicV;
				val = val + client.ClientData.EquipProp.ExtProps[15] + client.RoleBuffer.GetExtProp(15) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[15] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[15];
				val += client.AllThingsMultipliedBuffer.GetExtProp(15);
				val += client.ClientData.PropsCacheManager.GetExtProp(15);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicV);
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXMPVALUE);
				val += client.RoleMultipliedBuffer.GetExtProp(15);
				double addPercent = RoleAlgorithm.GetMaxMagicPercent(client);
				return val * Math.Max(0.0, 1.0 + addPercent);
			});
		}

		// Token: 0x0600345F RID: 13407 RVA: 0x002E7444 File Offset: 0x002E5644
		public static double GetLuckV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(17, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[17] + client.RoleBuffer.GetExtProp(17) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[17] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[17];
				val += client.ClientData.ExcellenceProp[17];
				val += client.ClientData.PropsCacheManager.GetExtProp(17);
				val *= 100.0;
				val += client.AllThingsMultipliedBuffer.GetExtProp(17);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Lucky);
				val += client.RoleMultipliedBuffer.GetExtProp(17);
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPLUCKYATTACK);
				return val + client.ClientData.LuckProp;
			});
		}

		// Token: 0x06003460 RID: 13408 RVA: 0x002E7484 File Offset: 0x002E5684
		public static double GetLuckV(Monster monster)
		{
			return monster.MonsterInfo.MonsterLucky;
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x002E7658 File Offset: 0x002E5858
		public static double GetHitV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(18, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				double val = roleBasePropItem.HitV;
				val += RoleAlgorithm.GetDexterity(client, true) * 0.5;
				val += client.RoleBuffer.GetExtProp(18);
				val += client.ClientData.EquipProp.ExtProps[18] + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[18] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[18];
				val += client.AllThingsMultipliedBuffer.GetExtProp(18);
				val += client.ClientData.PropsCacheManager.GetExtProp(18);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.HitV);
				val += client.RoleMultipliedBuffer.GetExtProp(18);
				double addPercent = client.ClientData.ExcellenceProp[6] + client.ClientData.ExcellenceProp[19] + client.RoleBuffer.GetExtProp(54) + client.ClientData.PropsCacheManager.GetExtProp(54);
				val *= 1.0 + addPercent;
				return val + client.ClientData.PurePropsCacheManager.GetExtProp(18);
			});
		}

		// Token: 0x06003462 RID: 13410 RVA: 0x002E7698 File Offset: 0x002E5898
		public static double GetHitPercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[6] + client.ClientData.ExcellenceProp[19] + client.RoleBuffer.GetExtProp(54) + client.ClientData.PropsCacheManager.GetExtProp(54);
		}

		// Token: 0x06003463 RID: 13411 RVA: 0x002E76F4 File Offset: 0x002E58F4
		public static double GetHitV(Monster monster)
		{
			double val = monster.MonsterInfo.HitV;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(18));
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x002E78E4 File Offset: 0x002E5AE4
		public static double GetDodgeV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(19, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				double val = roleBasePropItem.Dodge;
				val += RoleAlgorithm.GetDexterity(client, true) * 0.5;
				val += client.ClientData.EquipProp.ExtProps[19] + client.RoleBuffer.GetExtProp(19) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[19] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[19];
				val += client.AllThingsMultipliedBuffer.GetExtProp(19);
				val += client.ClientData.PropsCacheManager.GetExtProp(19);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Dodge);
				val += client.RoleMultipliedBuffer.GetExtProp(19);
				double addPercent = client.ClientData.ExcellenceProp[12] + client.ClientData.ExcellenceProp[25] + client.RoleBuffer.GetExtProp(55) + client.ClientData.PropsCacheManager.GetExtProp(55);
				val *= 1.0 + addPercent;
				return val + client.ClientData.PurePropsCacheManager.GetExtProp(19);
			});
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x002E7924 File Offset: 0x002E5B24
		public static double GetDodgePercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[12] + client.ClientData.ExcellenceProp[25] + client.RoleBuffer.GetExtProp(55) + client.ClientData.PropsCacheManager.GetExtProp(55);
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x002E7980 File Offset: 0x002E5B80
		public static double GetDodgeV(Monster monster)
		{
			return monster.MonsterInfo.Dodge;
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x002E79A0 File Offset: 0x002E5BA0
		public static double GetLifeRecoverValPercentV(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.RecoverLifeV;
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x002E7AC4 File Offset: 0x002E5CC4
		public static double GetLifeRecoverAddPercentV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(20, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[20] + client.RoleBuffer.GetExtProp(20) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[20] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[20];
				val += client.AllThingsMultipliedBuffer.GetExtProp(20);
				val += client.ClientData.PropsCacheManager.GetExtProp(20);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeRecoverPercent);
				val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDLIFERECOVERPERCENT);
				return val + client.RoleMultipliedBuffer.GetExtProp(20);
			});
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x002E7B04 File Offset: 0x002E5D04
		public static double GetLifeRecoverAddPercentOnlySandR(GameClient client)
		{
			double addrate = 0.0;
			return addrate + client.ClientData.PropsCacheManager.GetExtProp(88);
		}

		// Token: 0x0600346A RID: 13418 RVA: 0x002E7B38 File Offset: 0x002E5D38
		public static double GetLifeRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverLifeV;
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x002E7B58 File Offset: 0x002E5D58
		public static double GetMagicRecoverValPercentV(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.RecoverMagicV;
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x002E7B94 File Offset: 0x002E5D94
		public static double GetMagicRecoverAddPercentV(GameClient client)
		{
			return 0.0;
		}

		// Token: 0x0600346D RID: 13421 RVA: 0x002E7BB0 File Offset: 0x002E5DB0
		public static double GetMagicRecoverAddPercentOnlySandR(GameClient client)
		{
			double addrate = 0.0;
			return addrate + client.ClientData.PropsCacheManager.GetExtProp(89);
		}

		// Token: 0x0600346E RID: 13422 RVA: 0x002E7BE4 File Offset: 0x002E5DE4
		public static double GetMagicRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverMagicV;
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x002E7CF0 File Offset: 0x002E5EF0
		public static double GetSubAttackInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(24, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[24] + client.RoleBuffer.GetExtProp(24) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[24] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[24];
				val += client.AllThingsMultipliedBuffer.GetExtProp(24);
				val += client.ClientData.PropsCacheManager.GetExtProp(24);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjurePercent);
				val += client.RoleMultipliedBuffer.GetExtProp(24);
				return Math.Max(0.0, val);
			});
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x002E7D30 File Offset: 0x002E5F30
		public static double GetInjurePenetrationPercent(GameClient client)
		{
			double val = client.ClientData.PropsCacheManager.GetExtProp(93);
			return Math.Max(0.0, val);
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x002E7D64 File Offset: 0x002E5F64
		public static double GetSubAttackInjurePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjurePercent;
		}

		// Token: 0x06003472 RID: 13426 RVA: 0x002E7E5C File Offset: 0x002E605C
		public static double GetSubAttackInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(25, delegate
			{
				double val = 0.0;
				val = val + client.ClientData.EquipProp.ExtProps[25] + client.RoleBuffer.GetExtProp(25) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[25] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[25];
				val += client.AllThingsMultipliedBuffer.GetExtProp(25);
				val += client.ClientData.PropsCacheManager.GetExtProp(25);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjure);
				return val + client.RoleMultipliedBuffer.GetExtProp(25);
			});
		}

		// Token: 0x06003473 RID: 13427 RVA: 0x002E7E9C File Offset: 0x002E609C
		public static double GetSubAttackInjureValue(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjure;
		}

		// Token: 0x06003474 RID: 13428 RVA: 0x002E7F98 File Offset: 0x002E6198
		public static double GetMaxMagicPercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(16, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[16] + client.RoleBuffer.GetExtProp(16) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[16] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[16];
				val += client.AllThingsMultipliedBuffer.GetExtProp(16);
				val += client.ClientData.PropsCacheManager.GetExtProp(16);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicPercent);
				return val + client.RoleMultipliedBuffer.GetExtProp(16);
			});
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x002E80E0 File Offset: 0x002E62E0
		public static double GetIgnoreDefensePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(28, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[28] + client.RoleBuffer.GetExtProp(28) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[28] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[28];
				val += client.AllThingsMultipliedBuffer.GetExtProp(28);
				val += client.ClientData.PropsCacheManager.GetExtProp(28);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefensePercent);
				val += client.RoleMultipliedBuffer.GetExtProp(28);
				val += client.ClientData.ExcellenceProp[14];
				return val + client.ClientData.ExcellenceProp[26];
			});
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x002E8120 File Offset: 0x002E6320
		public static double GetIgnoreDefensePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefensePercent;
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x002E8248 File Offset: 0x002E6448
		public static double GetDecreaseInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(37, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[37] + client.RoleBuffer.GetExtProp(37) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[37] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[37];
				val += client.AllThingsMultipliedBuffer.GetExtProp(37);
				val += client.ClientData.PropsCacheManager.GetExtProp(37);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjurePercent);
				val += client.RoleMultipliedBuffer.GetExtProp(37);
				val += client.ClientData.ExcellenceProp[10];
				return val + client.ClientData.ExcellenceProp[22];
			});
		}

		// Token: 0x06003478 RID: 13432 RVA: 0x002E8288 File Offset: 0x002E6488
		public static double GetDecreaseInjurePercent(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x06003479 RID: 13433 RVA: 0x002E8380 File Offset: 0x002E6580
		public static double GetDecreaseInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(38, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[38] + client.RoleBuffer.GetExtProp(38) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[38] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[38];
				val += client.AllThingsMultipliedBuffer.GetExtProp(38);
				val += client.ClientData.PropsCacheManager.GetExtProp(38);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjureValue);
				return val + client.RoleMultipliedBuffer.GetExtProp(38);
			});
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x002E83C0 File Offset: 0x002E65C0
		public static double GetDecreaseInjureValue(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x0600347B RID: 13435 RVA: 0x002E84B8 File Offset: 0x002E66B8
		public static double GetCounteractInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(39, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[39] + client.RoleBuffer.GetExtProp(39) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[39] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[39];
				val += client.AllThingsMultipliedBuffer.GetExtProp(39);
				val += client.ClientData.PropsCacheManager.GetExtProp(39);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjurePercent);
				return val + client.RoleMultipliedBuffer.GetExtProp(39);
			});
		}

		// Token: 0x0600347C RID: 13436 RVA: 0x002E84F8 File Offset: 0x002E66F8
		public static double GetCounteractInjurePercent(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x0600347D RID: 13437 RVA: 0x002E85F0 File Offset: 0x002E67F0
		public static double GetCounteractInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(40, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[40] + client.RoleBuffer.GetExtProp(40) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[40] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[40];
				val += client.AllThingsMultipliedBuffer.GetExtProp(40);
				val += client.ClientData.PropsCacheManager.GetExtProp(40);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjureValue);
				return val + client.RoleMultipliedBuffer.GetExtProp(40);
			});
		}

		// Token: 0x0600347E RID: 13438 RVA: 0x002E8630 File Offset: 0x002E6830
		public static double GetCounteractInjureValue(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x0600347F RID: 13439 RVA: 0x002E8794 File Offset: 0x002E6994
		public static double GetAddAttackInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(26, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[26] + client.RoleBuffer.GetExtProp(26) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[26] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[26];
				val += client.AllThingsMultipliedBuffer.GetExtProp(26);
				val += client.ClientData.PropsCacheManager.GetExtProp(26);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjurePercent);
				val += client.ClientData.ExcellenceProp[5];
				val += client.ClientData.ExcellenceProp[21];
				val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.AddAttackInjurePercent);
				val += client.RoleMultipliedBuffer.GetExtProp(26);
				double modpct = client.ClientData.PctPropsCacheManager.GetExtProp(26);
				if (modpct > 0.0)
				{
					val *= modpct;
				}
				return val;
			});
		}

		// Token: 0x06003480 RID: 13440 RVA: 0x002E87D4 File Offset: 0x002E69D4
		public static double GetAddAttackInjurePercent(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x06003481 RID: 13441 RVA: 0x002E88D0 File Offset: 0x002E6AD0
		public static double GetAddAttackInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(27, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[27] + client.RoleBuffer.GetExtProp(27) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[27] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[27];
				val += client.AllThingsMultipliedBuffer.GetExtProp(27);
				val += client.ClientData.PropsCacheManager.GetExtProp(27);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjure);
				return val + client.RoleMultipliedBuffer.GetExtProp(27);
			});
		}

		// Token: 0x06003482 RID: 13442 RVA: 0x002E8910 File Offset: 0x002E6B10
		public static double GetAddAttackInjureValue(Monster monster)
		{
			return 0.0;
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x002E8A20 File Offset: 0x002E6C20
		public static double GetIgnoreDefenseRate(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(41, delegate
			{
				double val = 0.0;
				val += client.ClientData.EquipProp.ExtProps[41] + client.RoleBuffer.GetExtProp(41) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[41] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[41];
				val += client.AllThingsMultipliedBuffer.GetExtProp(41);
				val += client.ClientData.PropsCacheManager.GetExtProp(41);
				val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefenseRate);
				val += client.ClientData.ExcellenceProp[7];
				return val + client.RoleMultipliedBuffer.GetExtProp(41);
			});
		}

		// Token: 0x06003484 RID: 13444 RVA: 0x002E8A60 File Offset: 0x002E6C60
		public static double GetIgnoreDefenseRate(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefenseRate;
		}

		// Token: 0x06003485 RID: 13445 RVA: 0x002E8A80 File Offset: 0x002E6C80
		public static double GetFrozenPercent(IObject obj)
		{
			double dVal = 0.0;
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (null != client)
				{
					dVal += client.ClientData.EquipProp.ExtProps[56];
					dVal += client.ClientData.PropsCacheManager.GetExtProp(56);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					dVal = robot.FrozenPercent;
					if (dVal > 1.0)
					{
						dVal /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(dVal, 0.0);
		}

		// Token: 0x06003486 RID: 13446 RVA: 0x002E8B7C File Offset: 0x002E6D7C
		public static double GetPalsyPercent(IObject obj)
		{
			double dVal = 0.0;
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (null != client)
				{
					dVal += client.ClientData.EquipProp.ExtProps[57];
					dVal += client.ClientData.PropsCacheManager.GetExtProp(57);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					dVal = robot.PalsyPercent;
					if (dVal > 1.0)
					{
						dVal /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(dVal, 0.0);
		}

		// Token: 0x06003487 RID: 13447 RVA: 0x002E8C78 File Offset: 0x002E6E78
		public static double GetSpeedDownPercent(IObject obj)
		{
			double dVal = 0.0;
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (null != client)
				{
					dVal += client.ClientData.EquipProp.ExtProps[58];
					dVal += client.ClientData.PropsCacheManager.GetExtProp(58);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					dVal = robot.SpeedDownPercent;
					if (dVal > 1.0)
					{
						dVal /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(dVal, 0.0);
		}

		// Token: 0x06003488 RID: 13448 RVA: 0x002E8D74 File Offset: 0x002E6F74
		public static double GetBlowPercent(IObject obj)
		{
			double dVal = 0.0;
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (null != client)
				{
					dVal += client.ClientData.EquipProp.ExtProps[59];
					dVal += client.ClientData.PropsCacheManager.GetExtProp(59);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					dVal = robot.BlowPercent;
					if (dVal > 1.0)
					{
						dVal /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(dVal, 0.0);
		}

		// Token: 0x06003489 RID: 13449 RVA: 0x002E8E70 File Offset: 0x002E7070
		public static double GetAutoRevivePercent(object obj)
		{
			double dVal = 0.0;
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (null != client)
				{
					dVal += client.ClientData.PropsCacheManager.GetExtProp(60);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return dVal;
		}

		// Token: 0x0600348A RID: 13450 RVA: 0x002E8F20 File Offset: 0x002E7120
		public static double GetExtPropValue(GameClient client, ExtPropIndexes extPropIndex)
		{
			double val = 0.0;
			val += client.ClientData.EquipProp.ExtProps[(int)extPropIndex];
			return val + client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
		}

		// Token: 0x0600348B RID: 13451 RVA: 0x002E8F68 File Offset: 0x002E7168
		public static double CalRebornAttackInjureValue(IObject obj, IObject objTarget, ExtPropIndexes propIdx, ref int damageType)
		{
			double Attack = 0.0;
			double Defense = 0.0;
			double PenetratePercent = 0.0;
			double AbsorbPercent = 0.0;
			double WeakPercent = 0.0;
			double DoubleAttackPercent = 0.0;
			double DoubleAttackInjure = 0.0;
			double DoubleAttackResistance = 0.0;
			double DoubleAttackInjureResistance = 0.0;
			bool flag = true;
			if (RebornEquip.ExtPropIndexMap == null || !RebornEquip.ExtPropIndexMap.ContainsKey((int)propIdx) || RebornEquip.ExtPropIndexMap[(int)propIdx] == null || RebornEquip.ExtPropIndexMap[(int)propIdx].Count != 2)
			{
				flag = false;
			}
			if (obj is GameClient)
			{
				Attack = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx);
				PenetratePercent = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 2);
				DoubleAttackPercent = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 5);
				DoubleAttackInjure = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 6);
				Attack += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornAttack);
				PenetratePercent += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornPenetratePercent);
				DoubleAttackPercent += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornDoubleAttackPercent);
				DoubleAttackInjure += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornDoubleAttackInjure);
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				Attack = monster.DynamicData.ExtProps[(int)propIdx];
				PenetratePercent = monster.DynamicData.ExtProps[(int)(propIdx + 2)];
				DoubleAttackPercent = monster.DynamicData.ExtProps[(int)(propIdx + 5)];
				DoubleAttackInjure = monster.DynamicData.ExtProps[(int)(propIdx + 6)];
				Attack += monster.DynamicData.ExtProps[157];
				PenetratePercent += monster.DynamicData.ExtProps[159];
				DoubleAttackPercent += monster.DynamicData.ExtProps[162];
				DoubleAttackInjure += monster.DynamicData.ExtProps[163];
			}
			if (objTarget is GameClient)
			{
				Defense = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 1);
				AbsorbPercent = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 3);
				WeakPercent = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 4);
				Defense += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDefense);
				AbsorbPercent += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornAbsorbPercent);
				WeakPercent += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornWeakPercent);
				if (flag)
				{
					DoubleAttackResistance = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, (ExtPropIndexes)RebornEquip.ExtPropIndexMap[(int)propIdx][0]);
					DoubleAttackInjureResistance = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, (ExtPropIndexes)RebornEquip.ExtPropIndexMap[(int)propIdx][1]);
					DoubleAttackResistance += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDoubleAttackResistance);
					DoubleAttackInjureResistance += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDoubleAttackInjureResistance);
				}
			}
			else if (objTarget is Monster)
			{
				Defense = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 1)];
				AbsorbPercent = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 3)];
				WeakPercent = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 4)];
				Defense += (objTarget as Monster).DynamicData.ExtProps[158];
				AbsorbPercent += (objTarget as Monster).DynamicData.ExtProps[160];
				WeakPercent += (objTarget as Monster).DynamicData.ExtProps[161];
				if (flag)
				{
					DoubleAttackResistance = (objTarget as Monster).DynamicData.ExtProps[RebornEquip.ExtPropIndexMap[(int)propIdx][0]];
					DoubleAttackInjureResistance = (objTarget as Monster).DynamicData.ExtProps[RebornEquip.ExtPropIndexMap[(int)propIdx][1]];
					DoubleAttackResistance += (objTarget as Monster).DynamicData.ExtProps[175];
					DoubleAttackInjureResistance += (objTarget as Monster).DynamicData.ExtProps[176];
				}
			}
			bool DoubleAttack = false;
			double r = (double)Global.GetRandomNumber(1, 101);
			if (r <= (DoubleAttackPercent - DoubleAttackResistance) * 100.0)
			{
				DoubleAttack = true;
			}
			double result;
			if (DoubleAttack)
			{
				result = Math.Max(0.0, Attack - Defense) * (2.0 + Math.Min(Math.Max(0.0, DoubleAttackInjure - DoubleAttackInjureResistance), 1.0));
				damageType = 11;
			}
			else
			{
				result = Math.Max(0.0, Attack - Defense);
			}
			double InjurePct = Math.Max(-1.0, PenetratePercent - AbsorbPercent);
			InjurePct = Math.Min(InjurePct, 0.0);
			WeakPercent = Math.Max(0.0, 1.0 + WeakPercent);
			return result * (1.0 + InjurePct) * WeakPercent;
		}

		// Token: 0x0600348C RID: 13452 RVA: 0x002E94A4 File Offset: 0x002E76A4
		private static double GetAttackPower(IObject obj, int damage, int val, int luck, int nFatalValue, out int nDamageType, int nMaxAttackValue, double subSpPercent = 0.0)
		{
			nDamageType = 0;
			if (val < 0)
			{
				val = 0;
			}
			int r = Global.GetRandomNumber(1, 101);
			int result;
			if (r <= nFatalValue)
			{
				double dValue = 0.2;
				GameClient client = obj as GameClient;
				if (client != null)
				{
					dValue *= 1.0 + RoleAlgorithm.GetFatalHurt(client);
					dValue += DBRoleBufferManager.ProcessSpecialAttackValueBuff(client, 65);
					client.CheckCheatData.LastDamageType = Global.SetIntSomeBit(3, client.CheckCheatData.LastDamageType, true);
				}
				result = (int)((double)nMaxAttackValue * (1.0 + dValue * (1.0 - subSpPercent)));
				nDamageType = 3;
			}
			else if (r <= luck)
			{
				result = damage + val;
				double dValue = 0.0;
				GameClient client = obj as GameClient;
				if (client != null)
				{
					dValue = DBRoleBufferManager.ProcessSpecialAttackValueBuff(client, 64);
					client.CheckCheatData.LastDamageType = Global.SetIntSomeBit(4, client.CheckCheatData.LastDamageType, true);
				}
				result += (int)((double)result * dValue * (1.0 - subSpPercent));
				nDamageType = 4;
			}
			else
			{
				result = damage + Global.GetRandomNumber(0, val + 1);
				if (obj is GameClient)
				{
					GameClient client = obj as GameClient;
					client.CheckCheatData.LastDamageType = Global.SetIntSomeBit(0, client.CheckCheatData.LastDamageType, true);
				}
			}
			return (double)result;
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x002E9620 File Offset: 0x002E7820
		public static double CalcInjureValue(IObject obj, IObject objTarget, double damage, ref int damageType, double elementInjurePercnet)
		{
			double result = damage;
			double ruthlessValue = 0.0;
			double coldValue = 0.0;
			double savageValue = 0.0;
			double doubleValue = 0.0;
			if (obj is Robot)
			{
				Robot robot = obj as Robot;
				ruthlessValue = robot.RuthlessValue;
				ruthlessValue -= (double)((int)RoleAlgorithm.GetDeRuthlessPercent(objTarget as GameClient));
				coldValue = robot.ColdValue;
				coldValue -= (double)((int)RoleAlgorithm.GetDeColdPercent(objTarget as GameClient));
				savageValue = robot.SavageValue;
				savageValue -= (double)((int)RoleAlgorithm.GetDeSavagePercent(objTarget as GameClient));
				doubleValue = (double)robot.DoubleValue;
				doubleValue -= (double)((int)RoleAlgorithm.GetDeDoubleAttack(objTarget as GameClient));
			}
			else if (obj is GameClient)
			{
				ruthlessValue = (double)((int)RoleAlgorithm.GetRuthlessPercent(obj as GameClient));
				coldValue = (double)((int)RoleAlgorithm.GetColdPercent(obj as GameClient));
				savageValue = (double)((int)RoleAlgorithm.GetSavagePercent(obj as GameClient));
				doubleValue = (double)((int)RoleAlgorithm.GetDoubleAttack(obj as GameClient));
				if (objTarget is GameClient)
				{
					ruthlessValue -= (double)((int)RoleAlgorithm.GetDeRuthlessPercent(objTarget as GameClient));
					coldValue -= (double)((int)RoleAlgorithm.GetDeColdPercent(objTarget as GameClient));
					savageValue -= (double)((int)RoleAlgorithm.GetDeSavagePercent(objTarget as GameClient));
					doubleValue -= (double)((int)RoleAlgorithm.GetDeDoubleAttack(objTarget as GameClient));
				}
				else if (objTarget is Robot)
				{
					Robot robot = objTarget as Robot;
					ruthlessValue -= robot.DeRuthlessValue;
					coldValue -= robot.DeColdValue;
					savageValue -= robot.DeSavageValue;
					doubleValue -= robot.DeDoubleValue;
				}
			}
			ruthlessValue = Math.Max(0.0, ruthlessValue);
			coldValue = Math.Max(0.0, coldValue);
			savageValue = Math.Max(0.0, savageValue);
			doubleValue = Math.Max(0.0, doubleValue);
			double[] rateArr = new double[]
			{
				ruthlessValue,
				coldValue,
				savageValue,
				doubleValue
			};
			switch (RoleAlgorithm.GetRateIndex(rateArr, 100))
			{
			case 0:
			{
				result = damage * 1.5;
				double[] array = new double[2];
				array[0] = (double)((int)(result * 0.1));
				double[] param = array;
				MagicAction.ProcessAction(obj, objTarget, MagicActionIDs.MU_ADD_HP, param, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
				damageType = 6;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			case 1:
			{
				result = damage * 2.0;
				double[] param = new double[]
				{
					0.5,
					4.0
				};
				MagicAction.ProcessAction(obj, objTarget, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, param, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
				damageType = 7;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			case 2:
				result = damage * 3.0;
				damageType = 8;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			case 3:
			{
				double dValue = 1.0;
				if (obj is GameClient)
				{
					dValue += DBRoleBufferManager.ProcessSpecialAttackValueBuff(obj as GameClient, 66);
				}
				if (objTarget is GameClient)
				{
					dValue *= 1.0 - RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.SPAttackInjurePercent);
				}
				result = (double)((int)(damage * (1.0 + dValue)));
				damageType = 2;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			}
			result += (double)GameManager.ElementsAttackMgr.CalcAllElementDamage(obj, objTarget) * elementInjurePercnet;
			return result * (1.0 + SingletonTemplate<CoupleArenaManager>.Instance().CalcBuffHurt(obj, objTarget));
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x002E9AD4 File Offset: 0x002E7CD4
		public static int GetRateIndexPercent(double[] rateArr)
		{
			int index = -1;
			int result2;
			if (rateArr == null || rateArr.Length <= 0)
			{
				result2 = index;
			}
			else
			{
				double result = 0.0;
				double r = Global.GetRandom();
				for (int i = 0; i < rateArr.Length; i++)
				{
					result += rateArr[i];
					if (result > r)
					{
						return i;
					}
				}
				result2 = index;
			}
			return result2;
		}

		// Token: 0x0600348F RID: 13455 RVA: 0x002E9B44 File Offset: 0x002E7D44
		public static int GetRateIndex(double[] rateArr, int max)
		{
			int index = -1;
			int result2;
			if (rateArr == null || rateArr.Length <= 0)
			{
				result2 = index;
			}
			else
			{
				double result = 0.0;
				double r = (double)Global.GetRandomNumber(0, max);
				for (int i = 0; i < rateArr.Length; i++)
				{
					result += rateArr[i];
					if (result > r)
					{
						return i;
					}
				}
				result2 = index;
			}
			return result2;
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x002E9BB4 File Offset: 0x002E7DB4
		private static double GetDefensePower(int baseDefense, int val)
		{
			if (val < 0)
			{
				val = 0;
			}
			return (double)(baseDefense + Global.GetRandomNumber(0, val + 1));
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x002E9BE0 File Offset: 0x002E7DE0
		private static double GetDefenseValue(int minDefense, int maxDefense)
		{
			return RoleAlgorithm.GetDefensePower(minDefense, maxDefense - minDefense);
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x002E9BFC File Offset: 0x002E7DFC
		public static double CalcAttackValue(IObject obj, int minAttackV, int maxAttackV, int lucky, int nFatalValue, out int nDamageType, double subSpPercent = 0.0)
		{
			nDamageType = 0;
			return RoleAlgorithm.GetAttackPower(obj, minAttackV, maxAttackV - minAttackV, lucky, nFatalValue, out nDamageType, maxAttackV, subSpPercent);
		}

		// Token: 0x06003493 RID: 13459 RVA: 0x002E9C24 File Offset: 0x002E7E24
		public static double GetRealInjuredValue(long attackV, long defenseV)
		{
			return (double)((int)Math.Max(attackV - defenseV, (long)((int)Math.Max((double)attackV * 0.1, 5.0))));
		}

		// Token: 0x06003494 RID: 13460 RVA: 0x002E9C68 File Offset: 0x002E7E68
		public static double GetRealHitRate(double hitV, double dodgeV)
		{
			double result;
			if (dodgeV <= 0.0)
			{
				result = 1.0;
			}
			else
			{
				int rndNum = Global.GetRandomNumber(0, 101);
				int nHit = (int)(hitV / (hitV + dodgeV / 10.0) * 100.0);
				int value = Global.GMax(nHit, 3);
				result = (double)((rndNum <= value) ? 1 : 0);
			}
			return result;
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x002E9CD0 File Offset: 0x002E7ED0
		public static double GetRealHitRate(double DodgePercent)
		{
			double result;
			if (DodgePercent <= 0.0)
			{
				result = 1.0;
			}
			else
			{
				int rndNum = Global.GetRandomNumber(0, 101);
				int value = (int)(DodgePercent * 100.0);
				result = (double)((rndNum <= value) ? 0 : 1);
			}
			return result;
		}

		// Token: 0x06003496 RID: 13462 RVA: 0x002E9D20 File Offset: 0x002E7F20
		public static int CallAttackArmor(IObject attacker, IObject obj, ref int injure1, ref int injure2)
		{
			int currentArmorV = -1;
			int totalInjure = injure1 + injure2;
			if (totalInjure > 0 && (attacker is GameClient || attacker is Robot))
			{
				if (obj is GameClient)
				{
					GameClient client = obj as GameClient;
					if (client.ClientData.CurrentArmorV > 0)
					{
						int subInjure = (int)Global.GMin((double)client.ClientData.CurrentArmorV, (double)totalInjure * client.ClientData.ArmorPercent);
						int newInjure = totalInjure - subInjure;
						currentArmorV = Global.GMax(client.ClientData.CurrentArmorV - subInjure, 0);
						client.ClientData.CurrentArmorV = currentArmorV;
						injure2 = (int)((long)injure2 * (long)newInjure / (long)totalInjure);
						injure1 = newInjure - injure2;
					}
				}
				else if (obj is Robot)
				{
					Robot robot = obj as Robot;
					currentArmorV = (int)robot.DynamicData.ExtProps[119];
					if (currentArmorV > 0)
					{
						double ArmorPercent = robot.DynamicData.ExtProps[120];
						int subInjure = (int)Global.GMin((double)currentArmorV, (double)totalInjure * ArmorPercent);
						int newInjure = totalInjure - subInjure;
						currentArmorV = Global.GMax(currentArmorV - subInjure, 0);
						robot.DynamicData.ExtProps[119] = (double)currentArmorV;
						injure2 = (int)((long)injure2 * (long)newInjure / (long)totalInjure);
						injure1 = newInjure - injure2;
					}
				}
			}
			return currentArmorV;
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x002E9E90 File Offset: 0x002E8090
		public static int CalcAttackInjure(int attackType, IObject obj, int injured, IObject attacker = null)
		{
			double subPercent = 0.0;
			double subValue = 0.0;
			double DecreasePercent = 0.0;
			double DecreaseValue = 0.0;
			double CounteractPercent = 0.0;
			double CounteractValue = 0.0;
			double ctPercent = 0.0;
			if (attacker is GameClient)
			{
				ctPercent = RoleAlgorithm.GetInjurePenetrationPercent(attacker as GameClient);
			}
			else if (attacker is Monster)
			{
				ctPercent = (attacker as Monster).DynamicData.ExtProps[93];
			}
			if (obj is GameClient)
			{
				subPercent = RoleAlgorithm.GetSubAttackInjurePercent(obj as GameClient);
				subValue = RoleAlgorithm.GetSubAttackInjureValue(obj as GameClient);
				DecreasePercent = RoleAlgorithm.GetDecreaseInjurePercent(obj as GameClient);
				DecreaseValue = RoleAlgorithm.GetDecreaseInjureValue(obj as GameClient);
				CounteractPercent = RoleAlgorithm.GetCounteractInjurePercent(obj as GameClient);
				CounteractValue = RoleAlgorithm.GetCounteractInjureValue(obj as GameClient);
			}
			else if (obj is Monster)
			{
				subPercent = RoleAlgorithm.GetSubAttackInjurePercent(obj as Monster);
				subValue = RoleAlgorithm.GetSubAttackInjureValue(obj as Monster);
				DecreasePercent = RoleAlgorithm.GetDecreaseInjurePercent(obj as Monster);
				DecreaseValue = RoleAlgorithm.GetDecreaseInjureValue(obj as Monster);
				CounteractPercent = RoleAlgorithm.GetCounteractInjurePercent(obj as Monster);
				CounteractValue = RoleAlgorithm.GetCounteractInjureValue(obj as Monster);
			}
			if (obj is GameClient || obj is Robot)
			{
				double min = Data.ExtPropThreshold[24].Item1;
				double max = Data.ExtPropThreshold[24].Item2;
				subPercent = Math.Max(min, subPercent - ctPercent);
				subPercent = Math.Min(max, subPercent);
			}
			injured -= (int)(subValue + DecreaseValue + CounteractValue);
			injured = (int)((double)injured * (1.0 - subPercent) * (1.0 - DecreasePercent) * (1.0 - CounteractPercent));
			return injured;
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x002EA080 File Offset: 0x002E8280
		public static int GetDefenseByCalcingIgnoreDefense(int attackType, IObject self, int defense, ref int burst)
		{
			double ignorePercent = 0.0;
			if (self is GameClient)
			{
				ignorePercent = RoleAlgorithm.GetIgnoreDefensePercent(self as GameClient);
			}
			else if (self is Monster)
			{
				ignorePercent = RoleAlgorithm.GetIgnoreDefensePercent(self as Monster);
			}
			int result;
			if (ignorePercent <= 0.0)
			{
				result = defense;
			}
			else
			{
				int rndNum = Global.GetRandomNumber(0, 101);
				int value = (int)(ignorePercent * 100.0);
				if (rndNum <= value)
				{
					burst = 1;
					result = 0;
				}
				else
				{
					result = defense;
				}
			}
			return result;
		}

		// Token: 0x06003499 RID: 13465 RVA: 0x002EA11C File Offset: 0x002E831C
		private static bool ClientIgnorePhyAttack(GameClient client, ref int burst)
		{
			double ignorePhyAttackPercent = RoleAlgorithm.GetExtPropValue(client, ExtPropIndexes.IgnorePhyAttackPercent);
			bool result;
			if (ignorePhyAttackPercent > 0.0 && ignorePhyAttackPercent >= Global.GetRandom())
			{
				burst = 9;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600349A RID: 13466 RVA: 0x002EA160 File Offset: 0x002E8360
		private static bool ClientIgnoreMagicAttack(GameClient client, ref int burst)
		{
			double ignorePhyAttackPercent = RoleAlgorithm.GetExtPropValue(client, ExtPropIndexes.IgnoreMagyAttackPercent);
			bool result;
			if (ignorePhyAttackPercent > 0.0 && ignorePhyAttackPercent >= Global.GetRandom())
			{
				burst = 10;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600349B RID: 13467 RVA: 0x002EA1A4 File Offset: 0x002E83A4
		public static void AttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(monster);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinAttackV(client);
			int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(client);
			int lucky = (int)RoleAlgorithm.GetLuckV(client);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky -= (int)robot.DeLucky;
				nFatalValue -= (int)robot.DeFatalValue;
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(monster);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(monster);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
				burst = 1;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, client, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			injure += (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetPhySkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 119);
			if (null != bufferData)
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
				{
					injure = (int)((double)injure * (1.0 + (double)bufferData.BufferVal / 1000.0));
				}
			}
			injure = RoleAlgorithm.CalcAttackInjure(0, monster, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, monster, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, monster, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, monster, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = monster.GetObjectID();
				client.CheckCheatData.LastEnemyName = monster.MonsterInfo.VSName;
				client.CheckCheatData.LastEnemyPos = monster.CurrentPos;
			}
		}

		// Token: 0x0600349C RID: 13468 RVA: 0x002EA444 File Offset: 0x002E8644
		public static void AttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnorePhyAttack(client, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(client);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinAttackV(monster);
			int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(monster);
			int lucky = (int)RoleAlgorithm.GetLuckV(monster);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(monster);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
				lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(client);
				nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(client);
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(client);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(client);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			if (monster is Robot)
			{
				injure /= 2;
			}
			injure = RoleAlgorithm.CalcAttackInjure(0, client, injure, monster);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(monster, client, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, client, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, client, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
			}
		}

		// Token: 0x0600349D RID: 13469 RVA: 0x002EA660 File Offset: 0x002E8860
		public static void AttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate = 1.0, int addVlue = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinAttackV(monster);
			int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(monster);
			int lucky = (int)RoleAlgorithm.GetLuckV(monster);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(monster);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(enemy);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure, null);
			double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, enemy, magicCode);
			double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, enemy, magicCode);
			injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
			if (injure <= 0)
			{
				injure = -1;
			}
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x002EA820 File Offset: 0x002E8A20
		public static void AttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnorePhyAttack(enemy, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinAttackV(client);
			int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(client);
			int lucky = (int)RoleAlgorithm.GetLuckV(client);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);
			lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(enemy);
			nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(enemy);
			double subSpPercent = RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.SPAttackInjurePercent);
			int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, subSpPercent);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(enemy);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
				burst = 1;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, client, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			if (defenseV < 0)
			{
				defenseV = 0;
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 - RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.AttackInjurePercent)));
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetPhySkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, enemy, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, enemy, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
				BufferData bufferData = Global.GetBufferDataByID(enemy, 119);
				if (null != bufferData)
				{
					long nowTicks = TimeUtil.NOW();
					if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)bufferData.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = enemy.ClientData.RoleID;
				client.CheckCheatData.LastEnemyName = enemy.ClientData.RoleName;
				client.CheckCheatData.LastEnemyPos = enemy.CurrentPos;
			}
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x002EAAE8 File Offset: 0x002E8CE8
		public static void MAttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(monster);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(client);
			int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
			int lucky = (int)RoleAlgorithm.GetLuckV(client);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky -= (int)robot.DeLucky;
				nFatalValue -= (int)robot.DeFatalValue;
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(monster);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(monster);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
				burst = 1;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(1, client, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetMagicSkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, monster, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, monster, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, monster, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, monster, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
				BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 119);
				if (null != bufferData)
				{
					long nowTicks = TimeUtil.NOW();
					if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)bufferData.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = monster.GetObjectID();
				client.CheckCheatData.LastEnemyName = monster.MonsterInfo.VSName;
				client.CheckCheatData.LastEnemyPos = monster.CurrentPos;
			}
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x002EAD84 File Offset: 0x002E8F84
		public static void MAttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnoreMagicAttack(client, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(client);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					injure = 0;
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
			int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
			int lucky = 0;
			int nFatalValue = 0;
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
				lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(client);
				nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(client);
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(client);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(client);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, client, injure, monster);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(monster, client, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, client, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, client, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
			}
		}

		// Token: 0x060034A1 RID: 13473 RVA: 0x002EAF7C File Offset: 0x002E917C
		public static void MAttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate = 1.0, int addVlue = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
			int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
			int lucky = 0;
			int nFatalValue = 0;
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
			}
			int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure, null);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, enemy, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
			}
		}

		// Token: 0x060034A2 RID: 13474 RVA: 0x002EB12C File Offset: 0x002E932C
		public static void MAttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnoreMagicAttack(enemy, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (hit <= 0)
				{
					return;
				}
			}
			int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(client);
			int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
			int lucky = (int)RoleAlgorithm.GetLuckV(client);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);
			lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(enemy);
			nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(enemy);
			double subSpPercent = RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.SPAttackInjurePercent);
			int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, out burst, subSpPercent);
			attackV = (int)((double)attackV * attackPercent);
			int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
			int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
			int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);
			if (ignoreDefenseAndDodge)
			{
				defenseV = 0;
				burst = 1;
			}
			else
			{
				defenseV = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(1, client, defenseV, ref burst);
			}
			if (defenseV > 0)
			{
				defenseV = (int)((double)defenseV * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			if (defenseV < 0)
			{
				defenseV = 0;
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)attackV, (long)defenseV);
			injure = (int)((double)injure * (1.0 - RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.AttackInjurePercent)));
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetMagicSkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, enemy, (double)injure, ref burst, injurePercnet);
				double shenShiAddPercent = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, enemy, magicCode);
				double shenShiAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + shenShiAddPercent) + (double)addVlue + shenShiAddInjure);
				BufferData bufferData = Global.GetBufferDataByID(enemy, 119);
				if (null != bufferData)
				{
					long nowTicks = TimeUtil.NOW();
					if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)bufferData.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = enemy.ClientData.RoleID;
				client.CheckCheatData.LastEnemyName = enemy.ClientData.RoleName;
				client.CheckCheatData.LastEnemyPos = enemy.CurrentPos;
			}
		}

		// Token: 0x060034A3 RID: 13475 RVA: 0x002EB3F0 File Offset: 0x002E95F0
		public static double GetRoleNegativeRate(GameClient client, double baseVal, ExtPropIndexes extPropIndex)
		{
			double val = client.ClientData.EquipProp.ExtProps[(int)extPropIndex] + client.RoleBuffer.GetExtProp((int)extPropIndex);
			val += client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, extPropIndex);
			val += client.RoleMultipliedBuffer.GetExtProp((int)extPropIndex);
			return val + baseVal;
		}

		// Token: 0x060034A4 RID: 13476 RVA: 0x002EB460 File Offset: 0x002E9660
		public static double GetRoleStateDingSheng(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[47] + client.RoleBuffer.GetExtProp(47);
			val += client.ClientData.PropsCacheManager.GetExtProp(47);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateDingShen);
			val = client.RoleMultipliedBuffer.GetExtProp(47, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x002EB4EC File Offset: 0x002E96EC
		public static double GetRoleStateMoveSpeed(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[48] + client.RoleBuffer.GetExtProp(48);
			val += client.ClientData.PropsCacheManager.GetExtProp(48);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateMoveSpeed);
			val = client.RoleMultipliedBuffer.GetExtProp(48, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x002EB578 File Offset: 0x002E9778
		public static double GetRoleStateJiTui(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[49] + client.RoleBuffer.GetExtProp(49);
			val += client.ClientData.PropsCacheManager.GetExtProp(49);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateJiTui);
			val = client.RoleMultipliedBuffer.GetExtProp(49, val);
			return val + baseVal;
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x002EB5EC File Offset: 0x002E97EC
		public static double GetRoleStateHunMi(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[50] + client.RoleBuffer.GetExtProp(50);
			val += client.ClientData.PropsCacheManager.GetExtProp(50);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateHunMi);
			val = client.RoleMultipliedBuffer.GetExtProp(50, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		// Token: 0x04003FBD RID: 16317
		public static List<ExtPropIndexes>[] ExtListArray;

		// Token: 0x04003FBE RID: 16318
		public static List<ExtPropIndexes>[] BaseListArray;

		// Token: 0x04003FBF RID: 16319
		public static Dictionary<ExtPropIndexes, ExtPropItem> roleExtPropDic = new Dictionary<ExtPropIndexes, ExtPropItem>
		{
			{
				ExtPropIndexes.Strong,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AttackSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MoveSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MinDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreasePhyDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.528,
						0.384,
						0.45599999999999996,
						0.48,
						0.0,
						0.336
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					}
				}
			},
			{
				ExtPropIndexes.MaxDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreasePhyDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.88,
						0.64,
						0.76,
						0.8,
						0.0,
						0.56
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddDefense
					}
				}
			},
			{
				ExtPropIndexes.MinMDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreaseMagDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.36,
						0.504,
						0.432,
						0.45599999999999996,
						0.0,
						0.528
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddMDefense
					}
				}
			},
			{
				ExtPropIndexes.MaxMDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreaseMagDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.6,
						0.84,
						0.72,
						0.76,
						0.0,
						0.88
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					}
				}
			},
			{
				ExtPropIndexes.MinAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreasePhyAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.45599999999999996,
						0.0,
						0.48,
						0.504,
						0.0,
						0.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					}
				}
			},
			{
				ExtPropIndexes.MaxAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreasePhyAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.76,
						0.0,
						0.8,
						0.84,
						0.0,
						0.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddAttack
					}
				}
			},
			{
				ExtPropIndexes.MinMAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreaseMagAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						0.0,
						0.528,
						0.0,
						0.552,
						0.0,
						0.6
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					}
				}
			},
			{
				ExtPropIndexes.MaxMAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreaseMagAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						0.0,
						0.88,
						0.0,
						0.92,
						0.0,
						1.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddMAttack
					}
				}
			},
			{
				ExtPropIndexes.IncreasePhyAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP3,
						ExcellencePorp.EXCELLENCEPORP4,
						ExcellencePorp.EXCELLENCEPORP24
					}
				}
			},
			{
				ExtPropIndexes.IncreaseMagAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP3,
						ExcellencePorp.EXCELLENCEPORP4,
						ExcellencePorp.EXCELLENCEPORP24
					}
				}
			},
			{
				ExtPropIndexes.MaxLifeV,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.MaxLifePercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Constitution,
					Coefficient = new double[]
					{
						5.0,
						3.6,
						4.2,
						4.4,
						0.0,
						3.4
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDMAXHPVALUE
					}
				}
			},
			{
				ExtPropIndexes.MaxLifePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP8,
						ExcellencePorp.EXCELLENCEPORP20
					}
				}
			},
			{
				ExtPropIndexes.MaxMagicV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.MaxMagicPercent,
					PropCoef = 1.0,
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDMAXMPVALUE
					}
				}
			},
			{
				ExtPropIndexes.MaxMagicPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Lucky,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.HitV,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.HitPercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.5,
						0.5,
						0.5,
						0.5,
						0.5,
						0.5
					}
				}
			},
			{
				ExtPropIndexes.Dodge,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.DodgePercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.5,
						0.5,
						0.5,
						0.5,
						0.5,
						0.5
					}
				}
			},
			{
				ExtPropIndexes.LifeRecoverPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDLIFERECOVERPERCENT
					}
				}
			},
			{
				ExtPropIndexes.LifeRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MagicRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SubAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SubAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP5,
						ExcellencePorp.EXCELLENCEPORP21
					}
				}
			},
			{
				ExtPropIndexes.AddAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDefensePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP14,
						ExcellencePorp.EXCELLENCEPORP26
					}
				}
			},
			{
				ExtPropIndexes.DamageThornPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP11,
						ExcellencePorp.EXCELLENCEPORP28
					}
				}
			},
			{
				ExtPropIndexes.DamageThorn,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.PhySkillIncreasePercent,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						1E-05,
						0.0,
						1E-05,
						1E-05,
						0.0,
						0.0
					}
				}
			},
			{
				ExtPropIndexes.MagicSkillIncreasePercent,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.0,
						1E-05,
						0.0,
						1E-05,
						0.0,
						0.0001
					}
				}
			},
			{
				ExtPropIndexes.FatalAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP0,
						ExcellencePorp.EXCELLENCEPORP18
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.ADDTEMPFATALATTACK
					}
				}
			},
			{
				ExtPropIndexes.DoubleAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP23
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.ADDTEMPDOUBLEATTACK
					}
				}
			},
			{
				ExtPropIndexes.DecreaseInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP10,
						ExcellencePorp.EXCELLENCEPORP22
					}
				}
			},
			{
				ExtPropIndexes.DecreaseInjureValue,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.CounteractInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.CounteractInjureValue,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDefenseRate,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP7
					}
				}
			},
			{
				ExtPropIndexes.IncreasePhyDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddDefensePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP13,
						ExcellencePorp.EXCELLENCEPORP27
					}
				}
			},
			{
				ExtPropIndexes.IncreaseMagDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddDefensePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP13,
						ExcellencePorp.EXCELLENCEPORP27
					}
				}
			},
			{
				ExtPropIndexes.LifeSteal,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.LifeStealPercent,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateDingShen,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateMoveSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateJiTui,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateHunMi,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeLucky,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP29
					}
				}
			},
			{
				ExtPropIndexes.DeFatalAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP30
					}
				}
			},
			{
				ExtPropIndexes.DeDoubleAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP31
					}
				}
			},
			{
				ExtPropIndexes.HitPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP6,
						ExcellencePorp.EXCELLENCEPORP19
					}
				}
			},
			{
				ExtPropIndexes.DodgePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP12,
						ExcellencePorp.EXCELLENCEPORP25
					}
				}
			},
			{
				ExtPropIndexes.FrozenPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.PalsyPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SpeedDownPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.BlowPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AutoRevivePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SavagePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.ColdPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.RuthlessPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeSavagePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeColdPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeRuthlessPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.LifeStealPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Potion,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FirePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IcePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeFirePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeWaterPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeLightningPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeSoilPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeIcePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeWindPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Holywater,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RecoverLifeV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RecoverMagicV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Fatalhurt,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddDefensePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.InjurePenetrationPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnorePhyAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreMagyAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeFrozenPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DePalsyPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeSpeedDownPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeBlowPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Toughness,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SPAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WeaponEffect,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorMax,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NaturePenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDamageThornPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			}
		};

		// Token: 0x04003FC0 RID: 16320
		public static List<ExtPropIndexes> NotifyList = new List<ExtPropIndexes>
		{
			ExtPropIndexes.MinAttack,
			ExtPropIndexes.MaxAttack,
			ExtPropIndexes.MinDefense,
			ExtPropIndexes.MaxDefense,
			ExtPropIndexes.MinMAttack,
			ExtPropIndexes.MaxMAttack,
			ExtPropIndexes.MinMDefense,
			ExtPropIndexes.MaxMDefense,
			ExtPropIndexes.HitV,
			ExtPropIndexes.Dodge,
			ExtPropIndexes.AddAttackInjure,
			ExtPropIndexes.DecreaseInjureValue,
			ExtPropIndexes.MaxLifeV,
			ExtPropIndexes.MaxMagicV,
			ExtPropIndexes.LifeSteal,
			ExtPropIndexes.FireAttack,
			ExtPropIndexes.WaterAttack,
			ExtPropIndexes.LightningAttack,
			ExtPropIndexes.SoilAttack,
			ExtPropIndexes.IceAttack,
			ExtPropIndexes.WindAttack,
			ExtPropIndexes.AddAttack,
			ExtPropIndexes.IncreasePhyAttack,
			ExtPropIndexes.IncreaseMagAttack,
			ExtPropIndexes.AddAttackPercent,
			ExtPropIndexes.IncreasePhyDefense,
			ExtPropIndexes.AddDefense,
			ExtPropIndexes.AddDefensePercent,
			ExtPropIndexes.IncreaseMagDefense,
			ExtPropIndexes.HitPercent,
			ExtPropIndexes.DodgePercent,
			ExtPropIndexes.MaxLifePercent,
			ExtPropIndexes.MaxMagicPercent,
			ExtPropIndexes.LifeStealPercent,
			ExtPropIndexes.ArmorMax
		};
	}
}
