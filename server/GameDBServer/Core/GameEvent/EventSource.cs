using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameDBServer.Core.GameEvent
{
	// Token: 0x02000022 RID: 34
	public abstract class EventSource
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00004B24 File Offset: 0x00002D24
		public void registerListener(int eventType, IEventListener listener)
		{
			lock (this.listeners)
			{
				List<IEventListener> listenerList = null;
				if (!this.listeners.TryGetValue(eventType, out listenerList))
				{
					listenerList = new List<IEventListener>();
					this.listeners.Add(eventType, listenerList);
				}
				listenerList.Add(listener);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004B9C File Offset: 0x00002D9C
		public void removeListener(int eventType, IEventListener listener)
		{
			lock (this.listeners)
			{
				List<IEventListener> listenerList = null;
				if (this.listeners.TryGetValue(eventType, out listenerList))
				{
					listenerList.Remove(listener);
				}
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004C04 File Offset: 0x00002E04
		public void fireEvent(EventObject eventObj)
		{
			if (eventObj != null && eventObj.getEventType() != -1)
			{
				List<IEventListener> listenerList = null;
				if (this.listeners.TryGetValue(eventObj.getEventType(), out listenerList))
				{
					this.dispatchEvent(eventObj, listenerList);
				}
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004C50 File Offset: 0x00002E50
		private void dispatchEvent(EventObject eventObj, List<IEventListener> listenerList)
		{
			foreach (IEventListener listener in listenerList)
			{
				try
				{
					listener.processEvent(eventObj);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.getEventType(), ex), null, true);
				}
			}
		}

		// Token: 0x0400005E RID: 94
		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();
	}
}
