using System;

namespace GameServer.Server
{
	// Token: 0x020008C0 RID: 2240
	public class CmdHandler
	{
		// Token: 0x04004EE4 RID: 20196
		public uint CmdFlags;

		// Token: 0x04004EE5 RID: 20197
		public short MinParamCount;

		// Token: 0x04004EE6 RID: 20198
		public short MaxParamCount;

		// Token: 0x04004EE7 RID: 20199
		public ICmdProcessor CmdProcessor;
	}
}
