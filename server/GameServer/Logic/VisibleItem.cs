using System;

namespace GameServer.Logic
{
	// Token: 0x020005EF RID: 1519
	public class VisibleItem
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06001CE7 RID: 7399 RVA: 0x001AB890 File Offset: 0x001A9A90
		// (set) Token: 0x06001CE8 RID: 7400 RVA: 0x001AB8A7 File Offset: 0x001A9AA7
		public ObjectTypes ItemType { get; set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06001CE9 RID: 7401 RVA: 0x001AB8B0 File Offset: 0x001A9AB0
		// (set) Token: 0x06001CEA RID: 7402 RVA: 0x001AB8C7 File Offset: 0x001A9AC7
		public int ItemID { get; set; }
	}
}
