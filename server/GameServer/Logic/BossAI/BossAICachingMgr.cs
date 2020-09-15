using System;
using System.Collections.Generic;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D5 RID: 1493
	public static class BossAICachingMgr
	{
		// Token: 0x06001BCB RID: 7115 RVA: 0x001A17E4 File Offset: 0x0019F9E4
		public static List<BossAIItem> FindCachingItem(int AIID, int triggerType)
		{
			string key = string.Format("{0}_{1}", AIID, triggerType);
			List<BossAIItem> bossAIItemList = null;
			List<BossAIItem> result;
			if (!BossAICachingMgr._BossAICachingDict.TryGetValue(key, out bossAIItemList))
			{
				result = null;
			}
			else
			{
				result = bossAIItemList;
			}
			return result;
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x001A1828 File Offset: 0x0019FA28
		private static ITriggerCondition ParseCondition(int ID, int triggerType, string condition)
		{
			ITriggerCondition triggerCondition = null;
			if (triggerType == 0)
			{
				triggerCondition = new BirthOnCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 1)
			{
				triggerCondition = new BloodChangedCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				string[] fields = condition.Split(new char[]
				{
					'-'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("服务器端配置的Boss AI项，条件配置错误 ID={0}", ID), null, true);
					return null;
				}
				(triggerCondition as BloodChangedCondition).MinLifePercent = Global.SafeConvertToDouble(fields[0]);
				(triggerCondition as BloodChangedCondition).MaxLifePercent = Global.SafeConvertToDouble(fields[1]);
			}
			else if (triggerType == 2)
			{
				triggerCondition = new InjuredCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 3)
			{
				triggerCondition = new DeadCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 4)
			{
				triggerCondition = new AttackedCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 5)
			{
				triggerCondition = new AllDeadCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				string[] fields = condition.Split(new char[]
				{
					','
				});
				for (int i = 0; i < fields.Length; i++)
				{
					(triggerCondition as AllDeadCondition).MonsterIDList.Add(Global.SafeConvertToInt32(fields[i]));
				}
			}
			else if (triggerType == 6)
			{
				triggerCondition = new LivingTimeCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				(triggerCondition as LivingTimeCondition).LivingMinutes = (long)Global.SafeConvertToInt32(condition);
			}
			return triggerCondition;
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x001A1A10 File Offset: 0x0019FC10
		private static BossAIItem ParseBossAICachingItem(SystemXmlItem systemXmlItem)
		{
			BossAIItem bossAIItem = new BossAIItem
			{
				ID = systemXmlItem.GetIntValue("ID", -1),
				AIID = systemXmlItem.GetIntValue("AIID", -1),
				TriggerNum = systemXmlItem.GetIntValue("TriggerNum", -1),
				TriggerCD = systemXmlItem.GetIntValue("TriggerCD", -1),
				TriggerType = systemXmlItem.GetIntValue("TriggerType", -1),
				Desc = systemXmlItem.GetStringValue("Description")
			};
			bossAIItem.Condition = BossAICachingMgr.ParseCondition(bossAIItem.ID, bossAIItem.TriggerType, systemXmlItem.GetStringValue("Condition"));
			BossAIItem result;
			if (null == bossAIItem.Condition)
			{
				result = null;
			}
			else
			{
				result = bossAIItem;
			}
			return result;
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x001A1AD4 File Offset: 0x0019FCD4
		public static void LoadBossAICachingItems(SystemXmlItems systemBossAI)
		{
			Dictionary<string, List<BossAIItem>> bossAICachingDict = new Dictionary<string, List<BossAIItem>>();
			foreach (int key in systemBossAI.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemBossAI.SystemXmlItemDict[key];
				BossAIItem bossAIItem = BossAICachingMgr.ParseBossAICachingItem(systemXmlItem);
				if (null != bossAIItem)
				{
					string strKey = string.Format("{0}_{1}", bossAIItem.AIID, bossAIItem.TriggerType);
					List<BossAIItem> bossAIItemList = null;
					if (!bossAICachingDict.TryGetValue(strKey, out bossAIItemList))
					{
						bossAIItemList = new List<BossAIItem>();
						bossAICachingDict[strKey] = bossAIItemList;
					}
					bossAIItemList.Add(bossAIItem);
				}
			}
			BossAICachingMgr._BossAICachingDict = bossAICachingDict;
		}

		// Token: 0x04002A0F RID: 10767
		private static Dictionary<string, List<BossAIItem>> _BossAICachingDict = new Dictionary<string, List<BossAIItem>>();
	}
}
