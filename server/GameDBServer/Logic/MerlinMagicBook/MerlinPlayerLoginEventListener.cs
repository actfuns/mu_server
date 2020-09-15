using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.MerlinMagicBook
{
	// Token: 0x02000155 RID: 341
	public class MerlinPlayerLoginEventListener : IEventListener
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x00032F79 File Offset: 0x00031179
		private MerlinPlayerLoginEventListener()
		{
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00032F84 File Offset: 0x00031184
		public static MerlinPlayerLoginEventListener getInstnace()
		{
			return MerlinPlayerLoginEventListener.instance;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00032F9C File Offset: 0x0003119C
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;
				MerlinRankManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
			}
		}

		// Token: 0x04000852 RID: 2130
		private static MerlinPlayerLoginEventListener instance = new MerlinPlayerLoginEventListener();
	}
}
