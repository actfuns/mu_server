using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.Wing
{
	
	public class WingPlayerLoginEventListener : IEventListener
	{
		
		private WingPlayerLoginEventListener()
		{
		}

		
		public static WingPlayerLoginEventListener getInstnace()
		{
			return WingPlayerLoginEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				WingPaiHangManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
			}
		}

		
		private static WingPlayerLoginEventListener instance = new WingPlayerLoginEventListener();
	}
}
