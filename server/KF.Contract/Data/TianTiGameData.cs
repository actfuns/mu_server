using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x0200001E RID: 30
	[Serializable]
	public class TianTiGameData : IGameData, ICloneable
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00003FB4 File Offset: 0x000021B4
		public object Clone()
		{
			return new TianTiGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		// Token: 0x04000097 RID: 151
		public int ZhanDouLi;

		// Token: 0x04000098 RID: 152
		public string RoleName;
	}
}
