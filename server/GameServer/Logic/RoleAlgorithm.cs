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
	
	public class RoleAlgorithm
	{
		
		static RoleAlgorithm()
		{
			RoleAlgorithm.ExtListArray = new List<ExtPropIndexes>[177];
			RoleAlgorithm.BaseListArray = new List<ExtPropIndexes>[4];
			RoleAlgorithm.CreateNewExtArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.ExtListArray);
			RoleAlgorithm.CreateNewBaseArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.BaseListArray);
		}

		
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

		
		public static bool NeedNotifyClient(ExtPropIndexes attribute)
		{
			return RoleAlgorithm.NotifyList.Contains(attribute);
		}

		
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

		
		public static double GetBaseExtProp(GameClient client, ExtPropItem extPropItem)
		{
			double val = 0.0;
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return val;
		}

		
		public static double GetStrength(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropStrength + client.RoleBuffer.GetBaseProp(0) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[0] + client.ClientData.PropsCacheManager.GetBaseProp(0);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
			}
			return dValue;
		}

		
		public static double GetIntelligence(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropIntelligence + client.RoleBuffer.GetBaseProp(1) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[1] + client.ClientData.PropsCacheManager.GetBaseProp(1);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
			}
			return dValue;
		}

		
		public static double GetDexterity(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropDexterity + client.RoleBuffer.GetBaseProp(2) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[2] + client.ClientData.PropsCacheManager.GetBaseProp(2);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
			}
			return dValue;
		}

		
		public static double GetConstitution(GameClient client, bool bAddBuff = true)
		{
			double dValue = (double)client.ClientData.PropConstitution + client.RoleBuffer.GetBaseProp(3) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[3] + client.ClientData.PropsCacheManager.GetBaseProp(3);
			if (bAddBuff)
			{
				dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
			}
			return dValue;
		}

		
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

		
		public static double GetAttackSpeed(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.AttackSpeed;
		}

		
		public static double GetAttackSpeedServer(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(1, delegate
			{
				int nOcc = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
				return roleBasePropItem.AttackSpeed;
			});
		}

		
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

		
		public static double GetDeFatalAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(52);
			val += client.ClientData.ExcellenceProp[30];
			return val * 100.0;
		}

		
		public static double GetFatalHurt(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(90);
		}

		
		public static double GetDeLuckyAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(51);
			val += client.ClientData.ExcellenceProp[29];
			return val * 100.0;
		}

		
		public static double GetFatalAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterFatalAttack * 100.0;
		}

		
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

		
		public static double GetDeDoubleAttack(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(53);
			val += client.ClientData.ExcellenceProp[31];
			return val * 100.0;
		}

		
		public static double GetSavagePercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(61);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetDeSavagePercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(64);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetColdPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(62);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetDeColdPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(65);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetRuthlessPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(63);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetDeRuthlessPercent(GameClient client)
		{
			double val = 0.0;
			val += client.ClientData.PropsCacheManager.GetExtProp(66);
			val *= 100.0;
			return Math.Max(val, 0.0);
		}

		
		public static double GetDoubleAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterDoubleAttack * 100.0;
		}

		
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

		
		public static double GetDamageThornPercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterDamageThornPercent;
		}

		
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

		
		public static double GetDamageThorn(Monster monster)
		{
			double val = monster.MonsterInfo.MonsterDamageThorn;
			return Global.GMax(0.0, val);
		}

		
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

		
		public static double GetStrong(Monster monster)
		{
			return 0.0;
		}

		
		public static double GetMinADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 3);
		}

		
		public static double GetMinADefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.Defense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		
		public static double GetMaxADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 4);
		}

		
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

		
		public static double GetMaxADefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.Defense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		
		public static double GetMinMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 5);
		}

		
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

		
		public static double GetMinMDefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.MDefense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		
		public static double GetMaxMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 6);
		}

		
		public static double GetMaxMDefenseV(Monster monster)
		{
			double val = (double)monster.MonsterInfo.MDefense;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		
		public static double GetMinAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 7);
		}

		
		public static double GetMinAttackV(Monster monster)
		{
			double attackVal = (double)monster.MonsterInfo.MinAttack;
			return attackVal * (1.0 + monster.TempPropsBuffer.GetExtProp(11));
		}

		
		public static double GetMaxAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 8);
		}

		
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

		
		public static double GetMaxAttackV(Monster monster)
		{
			int attackVal = monster.MonsterInfo.MaxAttack;
			return (double)attackVal;
		}

		
		public static double GetMinMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 9);
		}

		
		public static double GetMinMagicAttackV(Monster monster)
		{
			double attackVal = (double)monster.MonsterInfo.MinAttack;
			return attackVal * (1.0 + monster.TempPropsBuffer.GetExtProp(12));
		}

		
		public static double GetMaxMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 10);
		}

		
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

		
		public static double GetMaxMagicAttackV(Monster monster)
		{
			int attackVal = monster.MonsterInfo.MaxAttack;
			return (double)attackVal;
		}

		
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

		
		public static double GetLifeStealPercentV(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(67);
		}

		
		public static double GetPotionPercentV(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(68);
		}

		
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

		
		public static double GetAddAttackPercent(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(91);
		}

		
		public static double GetAddDefensePercent(GameClient client)
		{
			double val = 0.0;
			return val + client.ClientData.PropsCacheManager.GetExtProp(92);
		}

		
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

		
		public static double GetLuckV(Monster monster)
		{
			return monster.MonsterInfo.MonsterLucky;
		}

		
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

		
		public static double GetHitPercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[6] + client.ClientData.ExcellenceProp[19] + client.RoleBuffer.GetExtProp(54) + client.ClientData.PropsCacheManager.GetExtProp(54);
		}

		
		public static double GetHitV(Monster monster)
		{
			double val = monster.MonsterInfo.HitV;
			return val * (1.0 + monster.TempPropsBuffer.GetExtProp(18));
		}

		
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

		
		public static double GetDodgePercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[12] + client.ClientData.ExcellenceProp[25] + client.RoleBuffer.GetExtProp(55) + client.ClientData.PropsCacheManager.GetExtProp(55);
		}

		
		public static double GetDodgeV(Monster monster)
		{
			return monster.MonsterInfo.Dodge;
		}

		
		public static double GetLifeRecoverValPercentV(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.RecoverLifeV;
		}

		
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

		
		public static double GetLifeRecoverAddPercentOnlySandR(GameClient client)
		{
			double addrate = 0.0;
			return addrate + client.ClientData.PropsCacheManager.GetExtProp(88);
		}

		
		public static double GetLifeRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverLifeV;
		}

		
		public static double GetMagicRecoverValPercentV(GameClient client)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
			return roleBasePropItem.RecoverMagicV;
		}

		
		public static double GetMagicRecoverAddPercentV(GameClient client)
		{
			return 0.0;
		}

		
		public static double GetMagicRecoverAddPercentOnlySandR(GameClient client)
		{
			double addrate = 0.0;
			return addrate + client.ClientData.PropsCacheManager.GetExtProp(89);
		}

		
		public static double GetMagicRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverMagicV;
		}

		
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

		
		public static double GetInjurePenetrationPercent(GameClient client)
		{
			double val = client.ClientData.PropsCacheManager.GetExtProp(93);
			return Math.Max(0.0, val);
		}

		
		public static double GetSubAttackInjurePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjurePercent;
		}

		
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

		
		public static double GetSubAttackInjureValue(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjure;
		}

		
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

		
		public static double GetIgnoreDefensePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefensePercent;
		}

		
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

		
		public static double GetDecreaseInjurePercent(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetDecreaseInjureValue(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetCounteractInjurePercent(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetCounteractInjureValue(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetAddAttackInjurePercent(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetAddAttackInjureValue(Monster monster)
		{
			return 0.0;
		}

		
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

		
		public static double GetIgnoreDefenseRate(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefenseRate;
		}

		
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

		
		public static double GetExtPropValue(GameClient client, ExtPropIndexes extPropIndex)
		{
			double val = 0.0;
			val += client.ClientData.EquipProp.ExtProps[(int)extPropIndex];
			return val + client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
		}

		
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

		
		private static double GetDefensePower(int baseDefense, int val)
		{
			if (val < 0)
			{
				val = 0;
			}
			return (double)(baseDefense + Global.GetRandomNumber(0, val + 1));
		}

		
		private static double GetDefenseValue(int minDefense, int maxDefense)
		{
			return RoleAlgorithm.GetDefensePower(minDefense, maxDefense - minDefense);
		}

		
		public static double CalcAttackValue(IObject obj, int minAttackV, int maxAttackV, int lucky, int nFatalValue, out int nDamageType, double subSpPercent = 0.0)
		{
			nDamageType = 0;
			return RoleAlgorithm.GetAttackPower(obj, minAttackV, maxAttackV - minAttackV, lucky, nFatalValue, out nDamageType, maxAttackV, subSpPercent);
		}

		
		public static double GetRealInjuredValue(long attackV, long defenseV)
		{
			return (double)((int)Math.Max(attackV - defenseV, (long)((int)Math.Max((double)attackV * 0.1, 5.0))));
		}

		
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

		
		public static double GetRoleNegativeRate(GameClient client, double baseVal, ExtPropIndexes extPropIndex)
		{
			double val = client.ClientData.EquipProp.ExtProps[(int)extPropIndex] + client.RoleBuffer.GetExtProp((int)extPropIndex);
			val += client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, extPropIndex);
			val += client.RoleMultipliedBuffer.GetExtProp((int)extPropIndex);
			return val + baseVal;
		}

		
		public static double GetRoleStateDingSheng(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[47] + client.RoleBuffer.GetExtProp(47);
			val += client.ClientData.PropsCacheManager.GetExtProp(47);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateDingShen);
			val = client.RoleMultipliedBuffer.GetExtProp(47, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		
		public static double GetRoleStateMoveSpeed(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[48] + client.RoleBuffer.GetExtProp(48);
			val += client.ClientData.PropsCacheManager.GetExtProp(48);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateMoveSpeed);
			val = client.RoleMultipliedBuffer.GetExtProp(48, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		
		public static double GetRoleStateJiTui(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[49] + client.RoleBuffer.GetExtProp(49);
			val += client.ClientData.PropsCacheManager.GetExtProp(49);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateJiTui);
			val = client.RoleMultipliedBuffer.GetExtProp(49, val);
			return val + baseVal;
		}

		
		public static double GetRoleStateHunMi(GameClient client, double baseVal)
		{
			double val = client.ClientData.EquipProp.ExtProps[50] + client.RoleBuffer.GetExtProp(50);
			val += client.ClientData.PropsCacheManager.GetExtProp(50);
			val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateHunMi);
			val = client.RoleMultipliedBuffer.GetExtProp(50, val);
			val += baseVal;
			return val + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		
		public static List<ExtPropIndexes>[] ExtListArray;

		
		public static List<ExtPropIndexes>[] BaseListArray;

		
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
