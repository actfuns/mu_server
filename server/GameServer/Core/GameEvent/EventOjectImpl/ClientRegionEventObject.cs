using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000CF RID: 207
	public class ClientRegionEventObject : EventObject
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000390 RID: 912 RVA: 0x0003CEAC File Offset: 0x0003B0AC
		// (set) Token: 0x06000391 RID: 913 RVA: 0x0003CEC3 File Offset: 0x0003B0C3
		public GameClient Client { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000392 RID: 914 RVA: 0x0003CECC File Offset: 0x0003B0CC
		// (set) Token: 0x06000393 RID: 915 RVA: 0x0003CEE3 File Offset: 0x0003B0E3
		public int EventType { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000394 RID: 916 RVA: 0x0003CEEC File Offset: 0x0003B0EC
		// (set) Token: 0x06000395 RID: 917 RVA: 0x0003CF03 File Offset: 0x0003B103
		public int Flag { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000396 RID: 918 RVA: 0x0003CF0C File Offset: 0x0003B10C
		// (set) Token: 0x06000397 RID: 919 RVA: 0x0003CF23 File Offset: 0x0003B123
		public int AreaLuaID { get; private set; }

		// Token: 0x06000398 RID: 920 RVA: 0x0003CF2C File Offset: 0x0003B12C
		public ClientRegionEventObject(GameClient client, int eventType, int flag, int areaLuaID) : base(31)
		{
			this.Client = client;
			this.EventType = eventType;
			this.Flag = flag;
			this.AreaLuaID = areaLuaID;
		}
	}
}
