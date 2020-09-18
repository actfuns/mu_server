using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class ZhanMengShijianBaseEventObject : EventObject
	{
		
		public ZhanMengShijianBaseEventObject(string roleName, int bhId, int shijianType, int param1, int param2, int param3, int serverId) : base(0)
		{
			this.roleName = roleName;
			this.bhId = bhId;
			this.shijianType = shijianType;
			this.param1 = param1;
			this.param2 = param2;
			this.param3 = param3;
			this.ServerId = serverId;
		}

		
		
		
		public int ServerId { get; protected set; }

		
		
		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		
		
		public int BhId
		{
			get
			{
				return this.bhId;
			}
		}

		
		
		public int ShijianType
		{
			get
			{
				return this.shijianType;
			}
		}

		
		
		public int Param1
		{
			get
			{
				return this.param1;
			}
		}

		
		
		public int Param2
		{
			get
			{
				return this.param2;
			}
		}

		
		
		public int Param3
		{
			get
			{
				return this.param3;
			}
		}

		
		protected string roleName;

		
		protected int bhId;

		
		protected int shijianType;

		
		protected int param1;

		
		protected int param2;

		
		protected int param3;
	}
}
