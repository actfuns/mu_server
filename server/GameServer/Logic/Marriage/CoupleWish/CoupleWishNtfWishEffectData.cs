using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x0200036E RID: 878
	[ProtoContract]
	public class CoupleWishNtfWishEffectData
	{
		// Token: 0x04001731 RID: 5937
		[ProtoMember(1)]
		public KuaFuRoleMiniData From;

		// Token: 0x04001732 RID: 5938
		[ProtoMember(2)]
		public List<KuaFuRoleMiniData> To;

		// Token: 0x04001733 RID: 5939
		[ProtoMember(3)]
		public int WishType;

		// Token: 0x04001734 RID: 5940
		[ProtoMember(4)]
		public string WishTxt;

		// Token: 0x04001735 RID: 5941
		[ProtoMember(5)]
		public int GetBinJinBi;

		// Token: 0x04001736 RID: 5942
		[ProtoMember(6)]
		public int GetBindZuanShi;

		// Token: 0x04001737 RID: 5943
		[ProtoMember(7)]
		public int GetExp;
	}
}
