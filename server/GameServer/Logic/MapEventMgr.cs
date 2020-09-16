using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class MapEventMgr
	{
		
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

		
		public void ClearAllMapEvents()
		{
			lock (this.EventQueue)
			{
				this.EventQueue.Clear();
			}
		}

		
		private List<object> EventQueue = new List<object>();
	}
}
