using System;

namespace GameServer.Logic
{
	// Token: 0x020007D0 RID: 2000
	public class BufferPropItem
	{
		// Token: 0x0600386D RID: 14445 RVA: 0x00302CC4 File Offset: 0x00300EC4
		public BufferPropItem()
		{
			this.ResetProps();
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x0600386E RID: 14446 RVA: 0x00302D1C File Offset: 0x00300F1C
		public double[] BaseProps
		{
			get
			{
				return this._BaseProps;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x0600386F RID: 14447 RVA: 0x00302D34 File Offset: 0x00300F34
		public long[] BasePropsTick
		{
			get
			{
				return this._BasePropsTick;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06003870 RID: 14448 RVA: 0x00302D4C File Offset: 0x00300F4C
		public double[] ExtProps
		{
			get
			{
				return this._ExtProps;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06003871 RID: 14449 RVA: 0x00302D64 File Offset: 0x00300F64
		public long[] ExtPropsTick
		{
			get
			{
				return this._ExtPropsTick;
			}
		}

		// Token: 0x06003872 RID: 14450 RVA: 0x00302D7C File Offset: 0x00300F7C
		public void ResetProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this._BaseProps[i] = 0.0;
				this._BasePropsTick[i] = 0L;
			}
			for (int i = 0; i < 177; i++)
			{
				this._ExtProps[i] = 0.0;
				this._ExtPropsTick[i] = 0L;
			}
		}

		// Token: 0x04004158 RID: 16728
		private double[] _BaseProps = new double[4];

		// Token: 0x04004159 RID: 16729
		private long[] _BasePropsTick = new long[4];

		// Token: 0x0400415A RID: 16730
		private double[] _ExtProps = new double[177];

		// Token: 0x0400415B RID: 16731
		private long[] _ExtPropsTick = new long[177];
	}
}
