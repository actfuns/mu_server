using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x02000100 RID: 256
	public class ZhanMengShijianBaseEventObject : EventObject
	{
		// Token: 0x060003E8 RID: 1000 RVA: 0x0003D754 File Offset: 0x0003B954
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

		// Token: 0x1700000C RID: 12
		
		
		public int ServerId { get; protected set; }

		// Token: 0x1700000D RID: 13
		
		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		// Token: 0x1700000E RID: 14
		
		public int BhId
		{
			get
			{
				return this.bhId;
			}
		}

		// Token: 0x1700000F RID: 15
		
		public int ShijianType
		{
			get
			{
				return this.shijianType;
			}
		}

		// Token: 0x17000010 RID: 16
		
		public int Param1
		{
			get
			{
				return this.param1;
			}
		}

		// Token: 0x17000011 RID: 17
		
		public int Param2
		{
			get
			{
				return this.param2;
			}
		}

		// Token: 0x17000012 RID: 18
		
		public int Param3
		{
			get
			{
				return this.param3;
			}
		}

		// Token: 0x0400053D RID: 1341
		protected string roleName;

		// Token: 0x0400053E RID: 1342
		protected int bhId;

		// Token: 0x0400053F RID: 1343
		protected int shijianType;

		// Token: 0x04000540 RID: 1344
		protected int param1;

		// Token: 0x04000541 RID: 1345
		protected int param2;

		// Token: 0x04000542 RID: 1346
		protected int param3;
	}
}
