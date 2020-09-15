using System;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000CC RID: 204
	public class CaiJiEventObject : EventObjectEx
	{
		// Token: 0x0600038D RID: 909 RVA: 0x0003CE53 File Offset: 0x0003B053
		public CaiJiEventObject(object source, object target) : base(10002)
		{
			this.Source = source;
			this.Target = target;
		}

		// Token: 0x040004DF RID: 1247
		public object Source;

		// Token: 0x040004E0 RID: 1248
		public object Target;
	}
}
