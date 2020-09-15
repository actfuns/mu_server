using System;
using KF.Contract.Interface;

namespace KF.Contract.Data.HuanYingSiYuan
{
	// Token: 0x0200000E RID: 14
	[Serializable]
	public class HuanYingSiYuanGameData : IGameData, ICloneable
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000020CC File Offset: 0x000002CC
		public object Clone()
		{
			return new HuanYingSiYuanGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000020F4 File Offset: 0x000002F4
		public static int GetZhanDouLi(IGameData gameData)
		{
			int zhanDouLi = 0;
			if (null != gameData)
			{
				HuanYingSiYuanGameData huanYingSiYuanGameData = gameData as HuanYingSiYuanGameData;
				if (null != huanYingSiYuanGameData)
				{
					zhanDouLi = huanYingSiYuanGameData.ZhanDouLi;
				}
			}
			return zhanDouLi;
		}

		// Token: 0x04000039 RID: 57
		public int ZhanDouLi;
	}
}
