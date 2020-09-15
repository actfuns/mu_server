using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	// Token: 0x0200000B RID: 11
	public class EventSourceEx<T>
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00005B74 File Offset: 0x00003D74
		public void registerListener(int eventType, EventSourceEx<T>.HandlerData listener)
		{
			lock (this.Mutex)
			{
				List<EventSourceEx<T>.HandlerData> listenerList;
				if (!this.listeners.TryGetValue(eventType, out listenerList))
				{
					listenerList = new List<EventSourceEx<T>.HandlerData>();
					this.listeners.Add(eventType, listenerList);
				}
				lock (listenerList)
				{
					listenerList.Add(listener);
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00005C50 File Offset: 0x00003E50
		public void removeListener(int eventType, EventSourceEx<T>.HandlerData listener)
		{
			lock (this.Mutex)
			{
				List<EventSourceEx<T>.HandlerData> listenerList = null;
				if (this.listeners.TryGetValue(eventType, out listenerList))
				{
					lock (listenerList)
					{
						listenerList.RemoveAll((EventSourceEx<T>.HandlerData x) => x.Handler == listener.Handler);
					}
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00005D14 File Offset: 0x00003F14
		public void fireEvent(int eventType, T eventObj)
		{
			if (null != eventObj)
			{
				this.fireEventInternal(eventType, eventObj);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00005D40 File Offset: 0x00003F40
		public void fireEventInternal(int eventType, T eventObj)
		{
			List<EventSourceEx<T>.HandlerData> listenerList = null;
			lock (this.Mutex)
			{
				if (!this.listeners.TryGetValue(eventType, out listenerList))
				{
					return;
				}
			}
			lock (listenerList)
			{
				this.dispatchEvent(eventType, eventObj, listenerList);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00005DE0 File Offset: 0x00003FE0
		private void dispatchEvent(int eventType, T eventObj, List<EventSourceEx<T>.HandlerData> listenerList)
		{
			foreach (EventSourceEx<T>.HandlerData listener in listenerList)
			{
				try
				{
					listener.Handler(eventObj);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("事件处理错误: type={0},handler={1},ex={2}", eventType, listener.Handler.GetType().FullName, ex), null, true);
				}
			}
		}

		// Token: 0x04000043 RID: 67
		protected object Mutex = new object();

		// Token: 0x04000044 RID: 68
		protected Dictionary<int, List<EventSourceEx<T>.HandlerData>> listeners = new Dictionary<int, List<EventSourceEx<T>.HandlerData>>();

		// Token: 0x0200000C RID: 12
		public class HandlerData
		{
			// Token: 0x04000045 RID: 69
			public int ID;

			// Token: 0x04000046 RID: 70
			public int EventType;

			// Token: 0x04000047 RID: 71
			public Func<T, bool> Handler;

			// Token: 0x04000048 RID: 72
			public List<int> BeforeList;

			// Token: 0x04000049 RID: 73
			public List<int> AfterList;
		}
	}
}
