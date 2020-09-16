using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000CF RID: 207
	public class ClientRegionEventObject : EventObject
	{
		// Token: 0x17000007 RID: 7
		
		
		public GameClient Client { get; private set; }

		// Token: 0x17000008 RID: 8
		
		
		public int EventType { get; private set; }

		// Token: 0x17000009 RID: 9
		
		
		public int Flag { get; private set; }

		// Token: 0x1700000A RID: 10
		
		
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
