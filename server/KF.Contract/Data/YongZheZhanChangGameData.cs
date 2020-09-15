using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000025 RID: 37
	[Serializable]
	public class YongZheZhanChangGameData : IGameData, ICloneable
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00004014 File Offset: 0x00002214
		public object Clone()
		{
			return new YongZheZhanChangGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		// Token: 0x040000B7 RID: 183
		public int ZhanDouLi;

		// Token: 0x040000B8 RID: 184
		public string RoleName;
	}
}
