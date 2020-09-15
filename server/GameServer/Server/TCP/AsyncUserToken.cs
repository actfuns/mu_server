using System;

namespace Server.TCP
{
	// Token: 0x020008CE RID: 2254
	public class AsyncUserToken : IDisposable
	{
		// Token: 0x06004047 RID: 16455 RVA: 0x003BB760 File Offset: 0x003B9960
		public void Dispose()
		{
			this.CurrentSocket = null;
			this.Tag = null;
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06004048 RID: 16456 RVA: 0x003BB774 File Offset: 0x003B9974
		// (set) Token: 0x06004049 RID: 16457 RVA: 0x003BB78B File Offset: 0x003B998B
		public TMSKSocket CurrentSocket { get; set; }

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x0600404A RID: 16458 RVA: 0x003BB794 File Offset: 0x003B9994
		// (set) Token: 0x0600404B RID: 16459 RVA: 0x003BB7AB File Offset: 0x003B99AB
		public object Tag { get; set; }

		// Token: 0x04004F35 RID: 20277
		public SendBuffer _SendBuffer;
	}
}
