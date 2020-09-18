using System;
using GameServer.Logic;
using GameServer.Logic.ActivityNew.SevenDay;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class SevenDayGoalEventObject : EventObject
	{
		
		public SevenDayGoalEventObject() : base(32)
		{
			this.Reset();
		}

		
		public void Reset()
		{
			this.Client = null;
			this.FuncType = ESevenDayGoalFuncType.Unknown;
			this.Arg1 = 0;
			this.Arg2 = 0;
			this.Arg3 = 0;
			this.Arg4 = 0;
		}

		
		public GameClient Client;

		
		public ESevenDayGoalFuncType FuncType;

		
		public int Arg1;

		
		public int Arg2;

		
		public int Arg3;

		
		public int Arg4;
	}
}
