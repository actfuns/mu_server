using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.WanMoTa
{
	
	public class WanMoTaPlayerLogoutEventListener : IEventListener
	{
		
		private WanMoTaPlayerLogoutEventListener()
		{
		}

		
		public static WanMoTaPlayerLogoutEventListener getInstnace()
		{
			return WanMoTaPlayerLogoutEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				WanMoTaManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
			}
		}

		
		private static WanMoTaPlayerLogoutEventListener instance = new WanMoTaPlayerLogoutEventListener();
	}
}
