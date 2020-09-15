using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic
{
	// Token: 0x02000145 RID: 325
	public class JingJiChangPlayerLogoutEventListener : IEventListener
	{
		// Token: 0x0600057E RID: 1406 RVA: 0x0002ECC6 File Offset: 0x0002CEC6
		private JingJiChangPlayerLogoutEventListener()
		{
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0002ECD4 File Offset: 0x0002CED4
		public static JingJiChangPlayerLogoutEventListener getInstnace()
		{
			return JingJiChangPlayerLogoutEventListener.instance;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0002ECEC File Offset: 0x0002CEEC
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLogin(logoutEvent.RoleInfo.RoleID);
			}
		}

		// Token: 0x04000824 RID: 2084
		private static JingJiChangPlayerLogoutEventListener instance = new JingJiChangPlayerLogoutEventListener();
	}
}
