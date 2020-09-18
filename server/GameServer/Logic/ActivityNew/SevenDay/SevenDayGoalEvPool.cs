using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	
	public static class SevenDayGoalEvPool
	{
		
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

		
		private static object Mutex = new object();

		
		private static Queue<SevenDayGoalEventObject> freeEvList = new Queue<SevenDayGoalEventObject>();
	}
}
