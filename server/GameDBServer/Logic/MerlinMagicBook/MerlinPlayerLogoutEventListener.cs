using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.MerlinMagicBook
{
	// Token: 0x02000156 RID: 342
	public class MerlinPlayerLogoutEventListener : IEventListener
	{
		// Token: 0x060005D4 RID: 1492 RVA: 0x00032FED File Offset: 0x000311ED
		private MerlinPlayerLogoutEventListener()
		{
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00032FF8 File Offset: 0x000311F8
		public static MerlinPlayerLogoutEventListener getInstnace()
		{
			return MerlinPlayerLogoutEventListener.instance;
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00033010 File Offset: 0x00031210
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;
				MerlinRankManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
			}
		}

		// Token: 0x04000853 RID: 2131
		private static MerlinPlayerLogoutEventListener instance = new MerlinPlayerLogoutEventListener();
	}
}
