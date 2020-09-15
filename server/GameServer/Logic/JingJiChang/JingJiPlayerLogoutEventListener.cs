using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000733 RID: 1843
	public class JingJiPlayerLogoutEventListener : IEventListener
	{
		// Token: 0x06002D00 RID: 11520 RVA: 0x0028264A File Offset: 0x0028084A
		private JingJiPlayerLogoutEventListener()
		{
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x00282658 File Offset: 0x00280858
		public static JingJiPlayerLogoutEventListener getInstance()
		{
			return JingJiPlayerLogoutEventListener.instance;
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x00282670 File Offset: 0x00280870
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject _eventObject = (PlayerLogoutEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerLogout(_eventObject.getPlayer());
			}
		}

		// Token: 0x04003B7F RID: 15231
		private static JingJiPlayerLogoutEventListener instance = new JingJiPlayerLogoutEventListener();
	}
}
