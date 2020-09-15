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
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0003D7A4 File Offset: 0x0003B9A4
		// (set) Token: 0x060003EA RID: 1002 RVA: 0x0003D7BB File Offset: 0x0003B9BB
		public int ServerId { get; protected set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x0003D7C4 File Offset: 0x0003B9C4
		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0003D7DC File Offset: 0x0003B9DC
		public int BhId
		{
			get
			{
				return this.bhId;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0003D7F4 File Offset: 0x0003B9F4
		public int ShijianType
		{
			get
			{
				return this.shijianType;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0003D80C File Offset: 0x0003BA0C
		public int Param1
		{
			get
			{
				return this.param1;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x0003D824 File Offset: 0x0003BA24
		public int Param2
		{
			get
			{
				return this.param2;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x0003D83C File Offset: 0x0003BA3C
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
