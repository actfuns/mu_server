using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.Wing
{
	// Token: 0x02000190 RID: 400
	public class WingPlayerLogoutEventListener : IEventListener
	{
		// Token: 0x0600071E RID: 1822 RVA: 0x00041D09 File Offset: 0x0003FF09
		private WingPlayerLogoutEventListener()
		{
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00041D14 File Offset: 0x0003FF14
		public static WingPlayerLogoutEventListener getInstnace()
		{
			return WingPlayerLogoutEventListener.instance;
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00041D2C File Offset: 0x0003FF2C
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				WingPaiHangManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
			}
		}

		// Token: 0x0400092C RID: 2348
		private static WingPlayerLogoutEventListener instance = new WingPlayerLogoutEventListener();
	}
}
