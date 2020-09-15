using System;

namespace GameServer.Logic
{
	// Token: 0x020007DB RID: 2011
	public class MagicActionItem
	{
		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060038CD RID: 14541 RVA: 0x00306740 File Offset: 0x00304940
		// (set) Token: 0x060038CE RID: 14542 RVA: 0x00306757 File Offset: 0x00304957
		public MagicActionIDs MagicActionID { get; set; }

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x060038CF RID: 14543 RVA: 0x00306760 File Offset: 0x00304960
		// (set) Token: 0x060038D0 RID: 14544 RVA: 0x00306777 File Offset: 0x00304977
		public double[] MagicActionParams { get; set; }
	}
}
