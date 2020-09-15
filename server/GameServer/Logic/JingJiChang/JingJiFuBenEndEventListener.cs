using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000732 RID: 1842
	public class JingJiFuBenEndEventListener : IEventListener
	{
		// Token: 0x06002CFC RID: 11516 RVA: 0x002825A3 File Offset: 0x002807A3
		private JingJiFuBenEndEventListener()
		{
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x002825B0 File Offset: 0x002807B0
		public static JingJiFuBenEndEventListener getInstance()
		{
			return JingJiFuBenEndEventListener.instance;
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x002825C8 File Offset: 0x002807C8
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 10)
			{
				PlayerDeadEventObject playerDeadEvent = (PlayerDeadEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerDead(playerDeadEvent.getPlayer(), playerDeadEvent.getAttacker());
			}
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject monsterDeadEvent = (MonsterDeadEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForMonsterDead(monsterDeadEvent.getAttacker(), monsterDeadEvent.getMonster());
			}
		}

		// Token: 0x04003B7E RID: 15230
		private static JingJiFuBenEndEventListener instance = new JingJiFuBenEndEventListener();
	}
}
