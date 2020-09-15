using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	// Token: 0x020001AE RID: 430
	public static class SevenDayGoalEvPool
	{
		// Token: 0x0600052D RID: 1325 RVA: 0x00049038 File Offset: 0x00047238
		public static SevenDayGoalEventObject Alloc(GameClient client, ESevenDayGoalFuncType funcType)
		{
			SevenDayGoalEventObject evObj = null;
			lock (SevenDayGoalEvPool.Mutex)
			{
				if (SevenDayGoalEvPool.freeEvList.Count > 0)
				{
					evObj = SevenDayGoalEvPool.freeEvList.Dequeue();
				}
			}
			if (evObj == null)
			{
				evObj = new SevenDayGoalEventObject();
			}
			evObj.Reset();
			evObj.Client = client;
			evObj.FuncType = funcType;
			return evObj;
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x000490D4 File Offset: 0x000472D4
		public static void Free(SevenDayGoalEventObject evObj)
		{
			if (evObj != null)
			{
				evObj.Reset();
				lock (SevenDayGoalEvPool.Mutex)
				{
					SevenDayGoalEvPool.freeEvList.Enqueue(evObj);
				}
			}
		}

		// Token: 0x040009AF RID: 2479
		private static object Mutex = new object();

		// Token: 0x040009B0 RID: 2480
		private static Queue<SevenDayGoalEventObject> freeEvList = new Queue<SevenDayGoalEventObject>();
	}
}
