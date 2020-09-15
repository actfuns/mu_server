using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004C2 RID: 1218
	public class MapEventMgr
	{
		// Token: 0x06001685 RID: 5765 RVA: 0x0015FE20 File Offset: 0x0015E020
		public void AddGuangMuEvent(int guangMuID, int show)
		{
			MapAIEvent guangMuEvent = new MapAIEvent
			{
				GuangMuID = guangMuID,
				Show = show
			};
			lock (this.EventQueue)
			{
				this.EventQueue.Add(guangMuEvent);
			}
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x0015FE94 File Offset: 0x0015E094
		public void PlayMapEvents(GameClient client)
		{
			lock (this.EventQueue)
			{
				foreach (object obj in this.EventQueue)
				{
					if (obj is MapAIEvent)
					{
						MapAIEvent e = (MapAIEvent)obj;
						int guangMuID = e.GuangMuID;
						int show = e.Show;
						client.sendCmd(667, string.Format("{0}:{1}", guangMuID, show), false);
					}
				}
			}
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x0015FF74 File Offset: 0x0015E174
		public void ClearAllMapEvents()
		{
			lock (this.EventQueue)
			{
				this.EventQueue.Clear();
			}
		}

		// Token: 0x0400204A RID: 8266
		private List<object> EventQueue = new List<object>();
	}
}
