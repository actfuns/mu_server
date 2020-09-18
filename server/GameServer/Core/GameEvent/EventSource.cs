using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	
	public abstract class EventSource
	{
		
		public void registerListener(int eventType, IEventListener listener)
		{
			lock (this.Mutex)
			{
				if (eventType >= 66)
				{
					List<IEventListener> listenerList;
					if (!this.listeners.TryGetValue(eventType, out listenerList))
					{
						listenerList = new List<IEventListener>();
						this.listeners.Add(eventType, listenerList);
					}
					listenerList.Add(listener);
				}
				else
				{
					List<IEventListener> listenerList = this.listenerArray[eventType];
					if (null == listenerList)
					{
						listenerList = new List<IEventListener>();
					}
					else
					{
						listenerList = new List<IEventListener>(listenerList);
					}
					this.listenerArray[eventType] = listenerList;
					listenerList.Add(listener);
				}
			}
		}

		
		public void removeListener(int eventType, IEventListener listener)
		{
			lock (this.Mutex)
			{
				if (eventType >= 66)
				{
					List<IEventListener> listenerList = null;
					if (this.listeners.TryGetValue(eventType, out listenerList))
					{
						listenerList.Remove(listener);
					}
				}
				else
				{
					List<IEventListener> listenerList = this.listenerArray[eventType];
					if (null == listenerList)
					{
						listenerList = new List<IEventListener>();
					}
					else
					{
						listenerList = new List<IEventListener>(listenerList);
					}
					this.listenerArray[eventType] = listenerList;
					listenerList.Remove(listener);
				}
			}
		}

		
		public void fireEvent(EventObject eventObj)
		{
			if (null != eventObj)
			{
				if (eventObj.AsyncEvent)
				{
					lock (this.AsyncEventQueue)
					{
						this.AsyncEventQueue.Enqueue(eventObj);
						return;
					}
				}
				this.fireEventInternal(eventObj);
			}
		}

		
		public void fireEventInternal(EventObject eventObj)
		{
			int eventType = eventObj.getEventType();
			if (eventType >= 66)
			{
				List<IEventListener> copylistenerList = null;
				List<IEventListener> listenerList = null;
				lock (this.Mutex)
				{
					if (!this.listeners.TryGetValue(eventObj.getEventType(), out listenerList))
					{
						return;
					}
					copylistenerList = listenerList.GetRange(0, listenerList.Count);
				}
				this.dispatchEvent(eventObj, copylistenerList);
			}
			else
			{
				List<IEventListener> listenerList;
				lock (this.Mutex)
				{
					listenerList = this.listenerArray[eventType];
				}
				if (null != listenerList)
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

		
		public void DispatchEventAsync()
		{
			for (;;)
			{
				EventObject eventObj = null;
				lock (this.AsyncEventQueue)
				{
					if (this.AsyncEventQueue.Count <= 0)
					{
						break;
					}
					eventObj = this.AsyncEventQueue.Dequeue();
				}
				if (null != eventObj)
				{
					this.fireEventInternal(eventObj);
				}
			}
		}

		
		protected object Mutex = new object();

		
		protected List<IEventListener>[] listenerArray = new List<IEventListener>[66];

		
		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();

		
		protected Queue<EventObject> AsyncEventQueue = new Queue<EventObject>();
	}
}
