using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.Wing
{
	
	public class WingPlayerLogoutEventListener : IEventListener
	{
		
		private WingPlayerLogoutEventListener()
		{
		}

		
		public static WingPlayerLogoutEventListener getInstnace()
		{
			return WingPlayerLogoutEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				WingPaiHangManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
			}
		}

		
		private static WingPlayerLogoutEventListener instance = new WingPlayerLogoutEventListener();
	}
}
