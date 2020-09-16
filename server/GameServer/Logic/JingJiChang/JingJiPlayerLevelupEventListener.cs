using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	
	public class JingJiPlayerLevelupEventListener : IEventListener
	{
		
		private JingJiPlayerLevelupEventListener()
		{
		}

		
		public static JingJiPlayerLevelupEventListener getInstance()
		{
			return JingJiPlayerLevelupEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 9)
			{
				PlayerLevelupEventObject levelupEvent = (PlayerLevelupEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLevelup(levelupEvent.Player);
			}
		}

		
		private static JingJiPlayerLevelupEventListener instance = new JingJiPlayerLevelupEventListener();
	}
}
