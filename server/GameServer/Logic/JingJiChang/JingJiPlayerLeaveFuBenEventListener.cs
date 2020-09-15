using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x02000734 RID: 1844
	public class JingJiPlayerLeaveFuBenEventListener : IEventListener
	{
		// Token: 0x06002D04 RID: 11524 RVA: 0x002826B2 File Offset: 0x002808B2
		private JingJiPlayerLeaveFuBenEventListener()
		{
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x002826C0 File Offset: 0x002808C0
		public static JingJiPlayerLeaveFuBenEventListener getInstance()
		{
			return JingJiPlayerLeaveFuBenEventListener.instance;
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x002826D8 File Offset: 0x002808D8
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject _eventObject = eventObject as PlayerLeaveFuBenEventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerLeaveFuBen(_eventObject.getPlayer());
			}
		}

		// Token: 0x04003B80 RID: 15232
		private static JingJiPlayerLeaveFuBenEventListener instance = new JingJiPlayerLeaveFuBenEventListener();
	}
}
