using System;

namespace GameServer.Logic
{
	// Token: 0x020006B6 RID: 1718
	public class EquipPropItem
	{
		// Token: 0x06002050 RID: 8272 RVA: 0x001BD751 File Offset: 0x001BB951
		public EquipPropItem()
		{
			this.ResetProps();
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06002051 RID: 8273 RVA: 0x001BD780 File Offset: 0x001BB980
		public double[] BaseProps
		{
			get
			{
				return this._BaseProps;
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06002052 RID: 8274 RVA: 0x001BD798 File Offset: 0x001BB998
		public double[] ExtProps
		{
			get
			{
				return this._ExtProps;
			}
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x001BD7B0 File Offset: 0x001BB9B0
		public void ResetProps()
		{
			for (int i = 0; i < 5; i++)
			{
				this._BaseProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this._ExtProps[i] = 0.0;
			}
		}

		// Token: 0x0400365B RID: 13915
		private double[] _BaseProps = new double[5];

		// Token: 0x0400365C RID: 13916
		private double[] _ExtProps = new double[177];
	}
}
