using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class XueSeChengBaoBaseEventObject : EventObject
	{
		
		public XueSeChengBaoBaseEventObject(int bloodCastleStatus) : base(1)
		{
			this._BloodCastleStatus = bloodCastleStatus;
		}

		
		public static XueSeChengBaoBaseEventObject CreateStatusEvent(int status)
		{
			return new XueSeChengBaoBaseEventObject(status);
		}

		
		public int _BloodCastleStatus = 0;
	}
}
