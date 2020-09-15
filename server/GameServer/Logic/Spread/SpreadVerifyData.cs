using System;

namespace GameServer.Logic.Spread
{
	// Token: 0x02000452 RID: 1106
	public class SpreadVerifyData
	{
		// Token: 0x04001DD8 RID: 7640
		public string VerifyCode = "";

		// Token: 0x04001DD9 RID: 7641
		public string Tel = "";

		// Token: 0x04001DDA RID: 7642
		public int TelCode = 0;

		// Token: 0x04001DDB RID: 7643
		public DateTime VerifyTime = DateTime.Now;

		// Token: 0x04001DDC RID: 7644
		public DateTime TelTime = DateTime.Now;
	}
}
