using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000103 RID: 259
	public abstract class EventSource
	{
		// Token: 0x060003F2 RID: 1010 RVA: 0x0003D860 File Offset: 0x0003BA60
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

		// Token: 0x060003F3 RID: 1011 RVA: 0x0003D918 File Offset: 0x0003BB18
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

		// Token: 0x060003F4 RID: 1012 RVA: 0x0003D9C4 File Offset: 0x0003BBC4
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

		// Token: 0x060003F5 RID: 1013 RVA: 0x0003DA3C File Offset: 0x0003BC3C
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

		// Token: 0x060003F6 RID: 1014 RVA: 0x0003DB2C File Offset: 0x0003BD2C
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

		// Token: 0x060003F7 RID: 1015 RVA: 0x0003DBB8 File Offset: 0x0003BDB8
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

		// Token: 0x04000589 RID: 1417
		protected object Mutex = new object();

		// Token: 0x0400058A RID: 1418
		protected List<IEventListener>[] listenerArray = new List<IEventListener>[66];

		// Token: 0x0400058B RID: 1419
		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();

		// Token: 0x0400058C RID: 1420
		protected Queue<EventObject> AsyncEventQueue = new Queue<EventObject>();
	}
}
