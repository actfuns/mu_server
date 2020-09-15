using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x020002F6 RID: 758
	public class TaoZhuang
	{
		// Token: 0x04001390 RID: 5008
		public int ID;

		// Token: 0x04001391 RID: 5009
		public int Type;

		// Token: 0x04001392 RID: 5010
		public List<int> AwakenList;

		// Token: 0x04001393 RID: 5011
		public int TaoZhuangProps1Num;

		// Token: 0x04001394 RID: 5012
		public double[] TaoZhuangProps1 = new double[177];

		// Token: 0x04001395 RID: 5013
		public int TaoZhuangProps2Num;

		// Token: 0x04001396 RID: 5014
		public double[] TaoZhuangProps2 = new double[177];

		// Token: 0x04001397 RID: 5015
		public int TaoZhuangProps3Num;

		// Token: 0x04001398 RID: 5016
		public double[] TaoZhuangProps3 = new double[177];

		// Token: 0x04001399 RID: 5017
		public int WeaponMasterNum;

		// Token: 0x0400139A RID: 5018
		public int WeaponMasterType;

		// Token: 0x0400139B RID: 5019
		public List<List<int>> PassiveSkill;

		// Token: 0x0400139C RID: 5020
		public List<List<int>> PassiveEffect;
	}
}
