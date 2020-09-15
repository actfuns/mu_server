using System;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B5 RID: 1205
	public class ReturnActivityInfo
	{
		// Token: 0x04001FF8 RID: 8184
		public int ID = 0;

		// Token: 0x04001FF9 RID: 8185
		public int ActivityID = 0;

		// Token: 0x04001FFA RID: 8186
		public DateTime TimeBegin = DateTime.MinValue;

		// Token: 0x04001FFB RID: 8187
		public DateTime TimeEnd = DateTime.MinValue;

		// Token: 0x04001FFC RID: 8188
		public DateTime TimeBeginNoLogin = DateTime.MinValue;

		// Token: 0x04001FFD RID: 8189
		public DateTime TimeEndNoLogin = DateTime.MinValue;

		// Token: 0x04001FFE RID: 8190
		public int Level = 0;

		// Token: 0x04001FFF RID: 8191
		public int Vip = 4;

		// Token: 0x04002000 RID: 8192
		public bool IsOpen = false;

		// Token: 0x04002001 RID: 8193
		public string ActivityDay = "";

		// Token: 0x04002002 RID: 8194
		public string TimeBeginStr = "";

		// Token: 0x04002003 RID: 8195
		public string TimeEndStr = "";
	}
}
