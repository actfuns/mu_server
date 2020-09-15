using System;

namespace GameServer.Logic
{
	// Token: 0x02000765 RID: 1893
	public class MonsterFlags
	{
		// Token: 0x06003088 RID: 12424 RVA: 0x002B2288 File Offset: 0x002B0488
		public void Copy(MonsterFlags flags)
		{
			if (null != flags)
			{
				this.InjureEvent = flags.InjureEvent;
			}
		}

		// Token: 0x04003D40 RID: 15680
		public static readonly MonsterFlags AllFlags = new MonsterFlags
		{
			InjureEvent = true
		};

		// Token: 0x04003D41 RID: 15681
		public bool InjureEvent;
	}
}
