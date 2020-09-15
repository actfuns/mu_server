using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000FE RID: 254
	public class XueSeChengBaoBaseEventObject : EventObject
	{
		// Token: 0x060003E5 RID: 997 RVA: 0x0003D714 File Offset: 0x0003B914
		public XueSeChengBaoBaseEventObject(int bloodCastleStatus) : base(1)
		{
			this._BloodCastleStatus = bloodCastleStatus;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0003D730 File Offset: 0x0003B930
		public static XueSeChengBaoBaseEventObject CreateStatusEvent(int status)
		{
			return new XueSeChengBaoBaseEventObject(status);
		}

		// Token: 0x0400053C RID: 1340
		public int _BloodCastleStatus = 0;
	}
}
