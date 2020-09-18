using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class JingJiFuBenEndForTimeEventObject : EventObject
	{
		
		public JingJiFuBenEndForTimeEventObject(int fubenId) : base(1)
		{
			this.fubenId = fubenId;
		}

		
		public int getFuBenId()
		{
			return this.fubenId;
		}

		
		private int fubenId;
	}
}
