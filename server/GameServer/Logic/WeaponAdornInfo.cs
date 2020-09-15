using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007B8 RID: 1976
	public class WeaponAdornInfo
	{
		// Token: 0x04003F72 RID: 16242
		public int nOccupationLimit;

		// Token: 0x04003F73 RID: 16243
		public WeaponTypeAndACTInfo tagWeaponTypeInfo = new WeaponTypeAndACTInfo();

		// Token: 0x04003F74 RID: 16244
		public List<WeaponTypeAndACTInfo> listCoexistType = new List<WeaponTypeAndACTInfo>();
	}
}
