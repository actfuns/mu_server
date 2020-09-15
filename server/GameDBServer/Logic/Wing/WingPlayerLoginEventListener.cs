using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.Wing
{
	// Token: 0x0200018F RID: 399
	public class WingPlayerLoginEventListener : IEventListener
	{
		// Token: 0x0600071A RID: 1818 RVA: 0x00041C92 File Offset: 0x0003FE92
		private WingPlayerLoginEventListener()
		{
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00041CA0 File Offset: 0x0003FEA0
		public static WingPlayerLoginEventListener getInstnace()
		{
			return WingPlayerLoginEventListener.instance;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00041CB8 File Offset: 0x0003FEB8
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				WingPaiHangManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
			}
		}

		// Token: 0x0400092B RID: 2347
		private static WingPlayerLoginEventListener instance = new WingPlayerLoginEventListener();
	}
}
