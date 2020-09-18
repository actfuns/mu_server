using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.MerlinMagicBook
{
	
	public class MerlinPlayerLoginEventListener : IEventListener
	{
		
		private MerlinPlayerLoginEventListener()
		{
		}

		
		public static MerlinPlayerLoginEventListener getInstnace()
		{
			return MerlinPlayerLoginEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				MerlinRankManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
			}
		}

		
		private static MerlinPlayerLoginEventListener instance = new MerlinPlayerLoginEventListener();
	}
}
