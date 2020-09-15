using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.WanMoTa
{
	// Token: 0x0200018D RID: 397
	public class WanMoTaPlayerLogoutEventListener : IEventListener
	{
		// Token: 0x06000703 RID: 1795 RVA: 0x0004155D File Offset: 0x0003F75D
		private WanMoTaPlayerLogoutEventListener()
		{
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00041568 File Offset: 0x0003F768
		public static WanMoTaPlayerLogoutEventListener getInstnace()
		{
			return WanMoTaPlayerLogoutEventListener.instance;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00041580 File Offset: 0x0003F780
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				WanMoTaManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
			}
		}

		// Token: 0x04000925 RID: 2341
		private static WanMoTaPlayerLogoutEventListener instance = new WanMoTaPlayerLogoutEventListener();
	}
}
