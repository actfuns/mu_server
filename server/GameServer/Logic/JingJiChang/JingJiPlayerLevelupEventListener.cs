using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000731 RID: 1841
	public class JingJiPlayerLevelupEventListener : IEventListener
	{
		// Token: 0x06002CF8 RID: 11512 RVA: 0x0028253D File Offset: 0x0028073D
		private JingJiPlayerLevelupEventListener()
		{
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x00282548 File Offset: 0x00280748
		public static JingJiPlayerLevelupEventListener getInstance()
		{
			return JingJiPlayerLevelupEventListener.instance;
		}

		// Token: 0x06002CFA RID: 11514 RVA: 0x00282560 File Offset: 0x00280760
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 9)
			{
				PlayerLevelupEventObject levelupEvent = (PlayerLevelupEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLevelup(levelupEvent.Player);
			}
		}

		// Token: 0x04003B7D RID: 15229
		private static JingJiPlayerLevelupEventListener instance = new JingJiPlayerLevelupEventListener();
	}
}
