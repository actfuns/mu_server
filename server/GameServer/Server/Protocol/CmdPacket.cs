using System;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x02000864 RID: 2148
	public class CmdPacket
	{
		// Token: 0x06003CA4 RID: 15524 RVA: 0x0034176C File Offset: 0x0033F96C
		public CmdPacket(int nID, byte[] data, int count)
		{
			this.CmdID = nID;
			this.Data = new byte[count];
			DataHelper.CopyBytes(this.Data, 0, data, 0, count);
		}

		// Token: 0x04004718 RID: 18200
		public int CmdID;

		// Token: 0x04004719 RID: 18201
		public byte[] Data;
	}
}
