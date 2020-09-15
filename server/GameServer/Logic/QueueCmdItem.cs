using System;

namespace GameServer.Logic
{
	// Token: 0x020005EA RID: 1514
	public class QueueCmdItem
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x001AB608 File Offset: 0x001A9808
		// (set) Token: 0x06001CBD RID: 7357 RVA: 0x001AB61F File Offset: 0x001A981F
		public int CmdID { get; set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06001CBE RID: 7358 RVA: 0x001AB628 File Offset: 0x001A9828
		// (set) Token: 0x06001CBF RID: 7359 RVA: 0x001AB63F File Offset: 0x001A983F
		public long ExecTicks { get; set; }
	}
}
