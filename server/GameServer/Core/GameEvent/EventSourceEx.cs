using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	
	public class EventSourceEx<T>
	{
		
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

		
		public void fireEvent(int eventType, T eventObj)
		{
			if (null != eventObj)
			{
				this.fireEventInternal(eventType, eventObj);
			}
		}

		
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

		
		protected object Mutex = new object();

		
		protected Dictionary<int, List<EventSourceEx<T>.HandlerData>> listeners = new Dictionary<int, List<EventSourceEx<T>.HandlerData>>();

		
		public class HandlerData
		{
			
			public int ID;

			
			public int EventType;

			
			public Func<T, bool> Handler;

			
			public List<int> BeforeList;

			
			public List<int> AfterList;
		}
	}
}
