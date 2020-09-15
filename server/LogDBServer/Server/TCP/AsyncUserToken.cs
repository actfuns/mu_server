using System;
using System.Net.Sockets;

namespace Server.TCP
{
	// Token: 0x02000036 RID: 54
	public class AsyncUserToken : IDisposable
	{
		// Token: 0x0600011C RID: 284 RVA: 0x00006F88 File Offset: 0x00005188
		public void Dispose()
		{
			this.CurrentSocket = null;
			this.Tag = null;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00006F9C File Offset: 0x0000519C
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00006FB3 File Offset: 0x000051B3
		public Socket CurrentSocket { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00006FBC File Offset: 0x000051BC
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00006FD3 File Offset: 0x000051D3
		public object Tag { get; set; }
	}
}
