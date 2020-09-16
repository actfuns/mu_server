using System;
using System.Collections.Generic;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class PassiveEffectManager
	{
		
		public static double GetPassiveEffectAddPercent(IObject attacker, int triggerType, int effectType)
		{
			double ret = 0.0;
			try
			{
				List<int> extPropsList;
				if (attacker is GameClient)
				{
					extPropsList = attacker.PassiveEffectList;
				}
				else
				{
					if (!(attacker is Robot))
					{
						return 0.0;
					}
					extPropsList = (attacker as Robot).PassiveEffectList;
				}
				if (null == extPropsList)
				{
					return ret;
				}
				foreach (int one in extPropsList)
				{
					PassiveEffectData effectData;
					if (!PassiveEffectManager.passiveExtDict.TryGetValue(one, out effectData))
					{
						SystemXmlItem systemMagic;
						if (!GameManager.SystemPassiveMgr.SystemXmlItemDict.TryGetValue(one, out systemMagic))
						{
							continue;
						}
						effectData = new PassiveEffectData
						{
							skillId = one,
							triggerRate = systemMagic.GetIntValue("Rate", -1),
							triggerType = systemMagic.GetIntValue("Type", -1)
						};
						PassiveEffectManager.passiveExtDict[one] = effectData;
					}
					if (triggerType == effectData.triggerType)
					{
						int rnd = Global.GetRandomNumber(0, 100);
						if (rnd < effectData.triggerRate)
						{
							List<MagicActionItem> magicActionItemList = null;
							if (!GameManager.SystemPassiveEffectMgr.MagicActionsDict.TryGetValue(one, out magicActionItemList) || null == magicActionItemList)
							{
								return ret;
							}
							foreach (MagicActionItem magicItem in magicActionItemList)
							{
								double _params = 0.0;
								double _params2 = 0.0;
								if (magicItem.MagicActionParams.Length > 0)
								{
									_params = magicItem.MagicActionParams[0];
								}
								if (magicItem.MagicActionParams.Length > 1)
								{
									_params2 = magicItem.MagicActionParams[1];
								}
								switch (magicItem.MagicActionID)
								{
								case MagicActionIDs.MU_ADD_OWN_ELEMENT_DAMAGE:
									if (effectType == 1)
									{
										ret += _params;
									}
									break;
								case MagicActionIDs.MU_ADD_OWN_ELEMENT_REDUCTION:
									if (effectType == 2)
									{
										if (attacker is GameClient)
										{
											GameClient client = attacker as GameClient;
											if (Convert.ToInt32((double)client.ClientData.LifeV * _params) >= client.ClientData.CurrentLifeV)
											{
												ret += _params2;
											}
										}
										else if (attacker is Robot)
										{
											Robot robot = attacker as Robot;
											if (robot.MonsterInfo.VLifeMax * _params >= robot.VLife)
											{
												ret += _params2;
											}
										}
									}
									break;
								case MagicActionIDs.MU_REDUCE_TARGET_ELEMENT_REDUCTION:
									if (effectType == 3)
									{
										ret += _params;
									}
									break;
								case MagicActionIDs.MU_ADD_OWN_DAMAGE_REBOUND:
									if (effectType == 4)
									{
										ret += _params;
									}
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("被动效果获取异常：rid={0},triggerType={1},effectType={2}", 0, triggerType, effectType), null, true);
			}
			return ret;
		}

		
		private object mutex = new object();

		
		private Dictionary<int, List<MagicActionItem>> _MagicActionsDict = null;

		
		public static Dictionary<int, PassiveEffectData> passiveExtDict = new Dictionary<int, PassiveEffectData>();
	}
}
