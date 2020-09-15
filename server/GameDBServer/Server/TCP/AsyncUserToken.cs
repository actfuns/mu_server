using System;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x02000212 RID: 530
	public class AsyncUserToken : IDisposable
	{
		// Token: 0x06000C48 RID: 3144 RVA: 0x0009F0A8 File Offset: 0x0009D2A8
		public void Dispose()
		{
			this.CurrentSocket = null;
			this.Tag = null;
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x0009F0BC File Offset: 0x0009D2BC
		// (set) Token: 0x06000C4A RID: 3146 RVA: 0x0009F0D3 File Offset: 0x0009D2D3
		public Socket CurrentSocket { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000C4B RID: 3147 RVA: 0x0009F0DC File Offset: 0x0009D2DC
		// (set) Token: 0x06000C4C RID: 3148 RVA: 0x0009F0F3 File Offset: 0x0009D2F3
		public object Tag { get; set; }
	}
}
