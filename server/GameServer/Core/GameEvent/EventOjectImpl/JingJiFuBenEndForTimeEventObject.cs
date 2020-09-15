using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E0 RID: 224
	public class JingJiFuBenEndForTimeEventObject : EventObject
	{
		// Token: 0x060003A9 RID: 937 RVA: 0x0003D163 File Offset: 0x0003B363
		public JingJiFuBenEndForTimeEventObject(int fubenId) : base(1)
		{
			this.fubenId = fubenId;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0003D178 File Offset: 0x0003B378
		public int getFuBenId()
		{
			return this.fubenId;
		}

		// Token: 0x04000515 RID: 1301
		private int fubenId;
	}
}
