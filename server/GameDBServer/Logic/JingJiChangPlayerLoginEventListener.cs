using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic
{
	// Token: 0x02000144 RID: 324
	public class JingJiChangPlayerLoginEventListener : IEventListener
	{
		// Token: 0x0600057A RID: 1402 RVA: 0x0002EC5C File Offset: 0x0002CE5C
		private JingJiChangPlayerLoginEventListener()
		{
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0002EC68 File Offset: 0x0002CE68
		public static JingJiChangPlayerLoginEventListener getInstnace()
		{
			return JingJiChangPlayerLoginEventListener.instance;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0002EC80 File Offset: 0x0002CE80
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID);
			}
		}

		// Token: 0x04000823 RID: 2083
		private static JingJiChangPlayerLoginEventListener instance = new JingJiChangPlayerLoginEventListener();
	}
}
