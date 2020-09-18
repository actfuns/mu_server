using System;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class CaiJiEventObject : EventObjectEx
	{
		
		public CaiJiEventObject(object source, object target) : base(10002)
		{
			this.Source = source;
			this.Target = target;
		}

		
		public object Source;

		
		public object Target;
	}
}
