using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	
	public class JingJiFuBenEndEventListener : IEventListener
	{
		
		private JingJiFuBenEndEventListener()
		{
		}

		
		public static JingJiFuBenEndEventListener getInstance()
		{
			return JingJiFuBenEndEventListener.instance;
		}

		
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

		
		private static JingJiFuBenEndEventListener instance = new JingJiFuBenEndEventListener();
	}
}
