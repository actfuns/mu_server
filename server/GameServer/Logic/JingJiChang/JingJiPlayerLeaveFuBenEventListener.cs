using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	
	public class JingJiPlayerLeaveFuBenEventListener : IEventListener
	{
		
		private JingJiPlayerLeaveFuBenEventListener()
		{
		}

		
		public static JingJiPlayerLeaveFuBenEventListener getInstance()
		{
			return JingJiPlayerLeaveFuBenEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject _eventObject = eventObject as PlayerLeaveFuBenEventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerLeaveFuBen(_eventObject.getPlayer());
			}
		}

		
		private static JingJiPlayerLeaveFuBenEventListener instance = new JingJiPlayerLeaveFuBenEventListener();
	}
}
