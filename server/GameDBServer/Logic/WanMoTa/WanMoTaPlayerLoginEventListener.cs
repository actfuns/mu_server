using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.WanMoTa
{
	// Token: 0x0200018C RID: 396
	public class WanMoTaPlayerLoginEventListener : IEventListener
	{
		// Token: 0x060006FF RID: 1791 RVA: 0x000414E6 File Offset: 0x0003F6E6
		private WanMoTaPlayerLoginEventListener()
		{
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x000414F4 File Offset: 0x0003F6F4
		public static WanMoTaPlayerLoginEventListener getInstnace()
		{
			return WanMoTaPlayerLoginEventListener.instance;
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0004150C File Offset: 0x0003F70C
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				WanMoTaManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
			}
		}

		// Token: 0x04000924 RID: 2340
		private static WanMoTaPlayerLoginEventListener instance = new WanMoTaPlayerLoginEventListener();
	}
}
