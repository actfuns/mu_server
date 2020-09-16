using System;
using GameServer.Core.Executor;
using GameServer.Interface;

namespace GameServer.Logic.NewBufferExt
{
	
	public class DelayInjuredBufferItem : IBufferItem
	{
		
		public int ObjectID = 0;

		
		public int TimeSlotSecs = 0;

		
		public int SubLifeV = 0;

		
		public long StartSubLifeNoShowTicks = TimeUtil.NOW();
	}
}
