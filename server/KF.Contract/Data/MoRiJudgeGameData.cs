using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x0200001D RID: 29
	[Serializable]
	public class MoRiJudgeGameData : IGameData, ICloneable
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00003F84 File Offset: 0x00002184
		public object Clone()
		{
			return new TianTiGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		// Token: 0x04000096 RID: 150
		public int ZhanDouLi;
	}
}
