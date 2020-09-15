using System;
using System.Collections.Generic;
using Server.Tools;

namespace LogDBServer.Core.GameEvent
{
	// Token: 0x0200000A RID: 10
	public abstract class EventSource
	{
		// Token: 0x06000025 RID: 37 RVA: 0x0000284C File Offset: 0x00000A4C
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

		// Token: 0x06000026 RID: 38 RVA: 0x000028C4 File Offset: 0x00000AC4
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

		// Token: 0x06000027 RID: 39 RVA: 0x0000292C File Offset: 0x00000B2C
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

		// Token: 0x06000028 RID: 40 RVA: 0x00002978 File Offset: 0x00000B78
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
					LogManager.WriteLog(LogTypes.Error, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.getEventType(), ex));
				}
			}
		}

		// Token: 0x04000015 RID: 21
		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();
	}
}
