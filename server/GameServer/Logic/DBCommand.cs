using System;

namespace GameServer.Logic
{
	// Token: 0x0200061F RID: 1567
	public class DBCommand
	{
		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06001FC4 RID: 8132 RVA: 0x001B7A60 File Offset: 0x001B5C60
		// (set) Token: 0x06001FC5 RID: 8133 RVA: 0x001B7A77 File Offset: 0x001B5C77
		public int DBCommandID { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06001FC6 RID: 8134 RVA: 0x001B7A80 File Offset: 0x001B5C80
		// (set) Token: 0x06001FC7 RID: 8135 RVA: 0x001B7A97 File Offset: 0x001B5C97
		public string DBCommandText { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06001FC8 RID: 8136 RVA: 0x001B7AA0 File Offset: 0x001B5CA0
		// (remove) Token: 0x06001FC9 RID: 8137 RVA: 0x001B7ADC File Offset: 0x001B5CDC
		public event DBCommandEventHandler DBCommandEvent;

		// Token: 0x06001FCA RID: 8138 RVA: 0x001B7B18 File Offset: 0x001B5D18
		public void DoDBCommandEvent(DBCommandEventArgs e)
		{
			if (null != this.DBCommandEvent)
			{
				this.DBCommandEvent(this, e);
			}
		}

		// Token: 0x04002CC3 RID: 11459
		public int ServerId;
	}
}
