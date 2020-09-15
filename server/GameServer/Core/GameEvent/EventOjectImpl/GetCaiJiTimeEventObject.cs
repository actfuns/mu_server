using System;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000CD RID: 205
	public class GetCaiJiTimeEventObject : EventObjectEx
	{
		// Token: 0x0600038E RID: 910 RVA: 0x0003CE71 File Offset: 0x0003B071
		public GetCaiJiTimeEventObject(object source, object target) : base(10003)
		{
			this.Source = source;
			this.Target = target;
		}

		// Token: 0x040004E1 RID: 1249
		public int GatherTime;

		// Token: 0x040004E2 RID: 1250
		public object Source;

		// Token: 0x040004E3 RID: 1251
		public object Target;
	}
}
