using System;
using System.Collections.Generic;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000105 RID: 261
	public abstract class SceneEventSource : ISceneEventSource
	{
		// Token: 0x060003FC RID: 1020 RVA: 0x0003DCA4 File Offset: 0x0003BEA4
		public void registerListener(int eventType, int sceneType, IEventListenerEx listener)
		{
			lock (this.Event2Scenelisteners)
			{
				Dictionary<int, List<IEventListenerEx>> dict;
				if (!this.Event2Scenelisteners.TryGetValue(eventType, out dict))
				{
					dict = new Dictionary<int, List<IEventListenerEx>>();
					this.Event2Scenelisteners.Add(eventType, dict);
				}
				List<IEventListenerEx> listenerList = null;
				if (!dict.TryGetValue(sceneType, out listenerList))
				{
					listenerList = new List<IEventListenerEx>();
					dict.Add(sceneType, listenerList);
				}
				listenerList.Add(listener);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0003DD40 File Offset: 0x0003BF40
		public void removeListener(int eventType, int sceneType, IEventListenerEx listener)
		{
			lock (this.Event2Scenelisteners)
			{
				Dictionary<int, List<IEventListenerEx>> dict;
				if (!this.Event2Scenelisteners.TryGetValue(eventType, out dict))
				{
					dict = new Dictionary<int, List<IEventListenerEx>>();
					this.Event2Scenelisteners.Add(eventType, dict);
				}
				List<IEventListenerEx> listenerList = null;
				if (dict.TryGetValue(sceneType, out listenerList))
				{
					listenerList.Remove(listener);
				}
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0003DDD0 File Offset: 0x0003BFD0
		public bool fireEvent(EventObjectEx eventObj, int sceneType)
		{
			int eventType;
			bool result;
			if (eventObj == null || (eventType = eventObj.EventType) == -1)
			{
				result = eventObj.Result;
			}
			else
			{
				List<IEventListenerEx> copylistenerList = null;
				List<IEventListenerEx> listenerList = null;
				lock (this.Event2Scenelisteners)
				{
					Dictionary<int, List<IEventListenerEx>> dict;
					if (!this.Event2Scenelisteners.TryGetValue(eventType, out dict))
					{
						return eventObj.Result;
					}
					if (!dict.TryGetValue(sceneType, out listenerList))
					{
						return eventObj.Result;
					}
					copylistenerList = listenerList.GetRange(0, listenerList.Count);
				}
				this.dispatchEvent(eventObj, copylistenerList);
				result = eventObj.Result;
			}
			return result;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0003DEA8 File Offset: 0x0003C0A8
		public void dispatchEvent(EventObjectEx eventObj, List<IEventListenerEx> listenerList)
		{
			foreach (IEventListenerEx listener in listenerList)
			{
				try
				{
					listener.processEvent(eventObj);
					if (eventObj.Handled)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.EventType, ex.ToString()), null, true);
				}
			}
		}

		// Token: 0x0400058E RID: 1422
		protected Dictionary<int, Dictionary<int, List<IEventListenerEx>>> Event2Scenelisteners = new Dictionary<int, Dictionary<int, List<IEventListenerEx>>>();
	}
}
