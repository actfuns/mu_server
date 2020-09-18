using System;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class GetCaiJiTimeEventObject : EventObjectEx
	{
		
		public GetCaiJiTimeEventObject(object source, object target) : base(10003)
		{
			this.Source = source;
			this.Target = target;
		}

		
		public int GatherTime;

		
		public object Source;

		
		public object Target;
	}
}
