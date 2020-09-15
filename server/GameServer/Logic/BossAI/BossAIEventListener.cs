using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D6 RID: 1494
	public class BossAIEventListener : IEventListener
	{
		// Token: 0x06001BD0 RID: 7120 RVA: 0x001A1BC4 File Offset: 0x0019FDC4
		private BossAIEventListener()
		{
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x001A1BD0 File Offset: 0x0019FDD0
		public static BossAIEventListener getInstance()
		{
			return BossAIEventListener.instance;
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x001A1BE8 File Offset: 0x0019FDE8
		public void processEvent(EventObject eventObject)
		{
			Monster monster = null;
			GameClient gameClient = null;
			List<BossAIItem> execBossAIItemList = new List<BossAIItem>();
			if (eventObject.getEventType() == 16)
			{
				monster = (eventObject as MonsterBirthOnEventObject).getMonster();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 0);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									monster.RecBossAI(bossAIItemList[i]);
									execBossAIItemList.Add(bossAIItemList[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 11)
			{
				monster = (eventObject as MonsterDeadEventObject).getMonster();
				gameClient = (eventObject as MonsterDeadEventObject).getAttacker();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 3);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									monster.RecBossAI(bossAIItemList[i]);
									execBossAIItemList.Add(bossAIItemList[i]);
								}
							}
						}
					}
					List<BossAIItem> allDeadBossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 5);
					if (null != allDeadBossAIItemList)
					{
						for (int i = 0; i < allDeadBossAIItemList.Count; i++)
						{
							if (monster.CanExecBossAI(allDeadBossAIItemList[i]))
							{
								bool toContinue = false;
								List<int> monsterIDList = (allDeadBossAIItemList[i].Condition as AllDeadCondition).MonsterIDList;
								for (int j = 0; j < monsterIDList.Count; j++)
								{
									List<object> findMonsters = GameManager.MonsterMgr.FindMonsterByExtensionID(monster.CurrentCopyMapID, monsterIDList[j]);
									if (findMonsters.Count > 0)
									{
										toContinue = true;
										break;
									}
								}
								if (!toContinue)
								{
									monster.RecBossAI(allDeadBossAIItemList[i]);
									execBossAIItemList.Add(allDeadBossAIItemList[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 17)
			{
				monster = (eventObject as MonsterInjuredEventObject).getMonster();
				gameClient = (eventObject as MonsterInjuredEventObject).getAttacker();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 2);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									monster.RecBossAI(bossAIItemList[i]);
									execBossAIItemList.Add(bossAIItemList[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 19)
			{
				monster = (eventObject as MonsterAttackedEventObject).getMonster();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 4);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									monster.RecBossAI(bossAIItemList[i]);
									execBossAIItemList.Add(bossAIItemList[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 18)
			{
				monster = (eventObject as MonsterBlooadChangedEventObject).getMonster();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 1);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									double currentLifeVPercent = monster.VLife / monster.MonsterInfo.VLifeMax;
									bool canExecActions = currentLifeVPercent >= (bossAIItemList[i].Condition as BloodChangedCondition).MinLifePercent && currentLifeVPercent <= (bossAIItemList[i].Condition as BloodChangedCondition).MaxLifePercent;
									if (canExecActions)
									{
										monster.RecBossAI(bossAIItemList[i]);
										execBossAIItemList.Add(bossAIItemList[i]);
									}
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 20)
			{
				monster = (eventObject as MonsterLivingTimeEventObject).getMonster();
				int AIID = monster.MonsterInfo.AIID;
				if (AIID > 0)
				{
					List<BossAIItem> bossAIItemList = BossAICachingMgr.FindCachingItem(AIID, 6);
					if (null != bossAIItemList)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < bossAIItemList.Count; i++)
							{
								if (monster.CanExecBossAI(bossAIItemList[i]))
								{
									bool canExecActions = monster.GetMonsterLivingTicks() >= (bossAIItemList[i].Condition as LivingTimeCondition).LivingMinutes * 60L * 1000L;
									if (canExecActions)
									{
										monster.RecBossAI(bossAIItemList[i]);
										execBossAIItemList.Add(bossAIItemList[i]);
									}
								}
							}
						}
					}
				}
			}
			if (null != execBossAIItemList)
			{
				for (int i = 0; i < execBossAIItemList.Count; i++)
				{
					BossAIItem bossAIItem = execBossAIItemList[i];
					List<MagicActionItem> magicActionItemList = null;
					if (GameManager.SystemMagicActionMgr.BossAIActionsDict.TryGetValue(bossAIItem.ID, out magicActionItemList) && null != magicActionItemList)
					{
						for (int j = 0; j < magicActionItemList.Count; j++)
						{
							MagicAction.ProcessAction(monster, gameClient, magicActionItemList[j].MagicActionID, magicActionItemList[j].MagicActionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							if (!string.IsNullOrEmpty(bossAIItem.Desc))
							{
								GameManager.ClientMgr.BroadSpecialHintText(monster.CurrentMapCode, monster.CurrentCopyMapID, bossAIItem.Desc);
							}
						}
					}
				}
			}
		}

		// Token: 0x04002A10 RID: 10768
		private static BossAIEventListener instance = new BossAIEventListener();
	}
}
