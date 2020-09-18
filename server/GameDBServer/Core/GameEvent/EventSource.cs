using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameDBServer.Core.GameEvent
{
	
	public abstract class EventSource
	{
		
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

		
		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();
	}
}
