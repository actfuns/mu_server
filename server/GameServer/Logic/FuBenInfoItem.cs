using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020006C4 RID: 1732
	public class FuBenInfoItem
	{
		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060020D8 RID: 8408 RVA: 0x001C2C80 File Offset: 0x001C0E80
		// (set) Token: 0x060020D9 RID: 8409 RVA: 0x001C2CC8 File Offset: 0x001C0EC8
		public long StartTicks
		{
			get
			{
				long startTicks;
				lock (this)
				{
					startTicks = this._StartTicks;
				}
				return startTicks;
			}
			set
			{
				lock (this)
				{
					this._StartTicks = value;
				}
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060020DA RID: 8410 RVA: 0x001C2D10 File Offset: 0x001C0F10
		// (set) Token: 0x060020DB RID: 8411 RVA: 0x001C2D58 File Offset: 0x001C0F58
		public long EndTicks
		{
			get
			{
				long endTicks;
				lock (this)
				{
					endTicks = this._EndTicks;
				}
				return endTicks;
			}
			set
			{
				lock (this)
				{
					this._EndTicks = value;
				}
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060020DC RID: 8412 RVA: 0x001C2DA0 File Offset: 0x001C0FA0
		// (set) Token: 0x060020DD RID: 8413 RVA: 0x001C2DE8 File Offset: 0x001C0FE8
		public int nDieCount
		{
			get
			{
				int nDieCount;
				lock (this)
				{
					nDieCount = this._nDieCount;
				}
				return nDieCount;
			}
			set
			{
				lock (this)
				{
					this._nDieCount = value;
				}
			}
		}

		// Token: 0x040036A0 RID: 13984
		public int FuBenSeqID = 0;

		// Token: 0x040036A1 RID: 13985
		private long _StartTicks = 0L;

		// Token: 0x040036A2 RID: 13986
		public long _EndTicks = 0L;

		// Token: 0x040036A3 RID: 13987
		public int GoodsBinding = 0;

		// Token: 0x040036A4 RID: 13988
		public int FuBenID = 0;

		// Token: 0x040036A5 RID: 13989
		public int _nDieCount = 0;

		// Token: 0x040036A6 RID: 13990
		public int nDayOfYear = TimeUtil.NowDateTime().DayOfYear;

		// Token: 0x040036A7 RID: 13991
		public double AwardRate = 1.0;
	}
}
