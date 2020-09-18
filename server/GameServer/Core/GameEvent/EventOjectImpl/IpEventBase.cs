using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class IpEventBase : EventObject
	{
		
		public IpEventBase(int eventType) : base(eventType, true)
		{
		}

		
		public IpEventBase(int eventType, long _ipAsInt, string _userid) : base(eventType)
		{
			this.ipAsInt = _ipAsInt;
			this.userid = _userid;
		}

		
		public long getIpAsInt()
		{
			return this.ipAsInt;
		}

		
		public string getUserID()
		{
			return this.userid;
		}

		
		public int getRoleID()
		{
			return this.roleid;
		}

		
		protected long ipAsInt;

		
		protected string userid;

		
		protected int roleid;
	}
}
