using System;
using GameServer.Logic;
using GameServer.Logic.Ornament;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class OrnamentGoalEventObject : EventObject
	{
		
		public OrnamentGoalEventObject(GameClient Client, OrnamentGoalType FuncType, params int[] args) : base(37)
		{
			this.Reset();
			this.Client = Client;
			this.FuncType = FuncType;
			if (args.Length > 0)
			{
				this.Arg1 = args[0];
			}
			if (args.Length > 1)
			{
				this.Arg1 = args[1];
			}
			if (args.Length > 2)
			{
				this.Arg1 = args[2];
			}
			if (args.Length > 3)
			{
				this.Arg1 = args[3];
			}
		}

		
		public void Reset()
		{
			this.Client = null;
			this.FuncType = OrnamentGoalType.OGT_UseGoods;
			this.Arg1 = 0;
			this.Arg2 = 0;
			this.Arg3 = 0;
			this.Arg4 = 0;
		}

		
		public GameClient Client;

		
		public OrnamentGoalType FuncType;

		
		public int Arg1;

		
		public int Arg2;

		
		public int Arg3;

		
		public int Arg4;
	}
}
