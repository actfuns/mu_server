using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002FC RID: 764
	[ProtoContract]
	public class JueXingShiData
	{
		// Token: 0x06000C2A RID: 3114 RVA: 0x000BE8C0 File Offset: 0x000BCAC0
		public TaoZhuangData GetAttackTaoZhuang()
		{
			return this.TaoZhuangList.Find((TaoZhuangData _g) => _g.ID == this.AttackEquip);
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x000BE90C File Offset: 0x000BCB0C
		public TaoZhuangData GetDefenseTaoZhuang()
		{
			return this.TaoZhuangList.Find((TaoZhuangData _g) => _g.ID == this.DefenseEquip);
		}

		// Token: 0x040013C0 RID: 5056
		[ProtoMember(1)]
		public int AttackEquip;

		// Token: 0x040013C1 RID: 5057
		[ProtoMember(2)]
		public int DefenseEquip;

		// Token: 0x040013C2 RID: 5058
		[ProtoMember(3)]
		public List<TaoZhuangData> TaoZhuangList;

		// Token: 0x040013C3 RID: 5059
		[ProtoMember(4)]
		public int JueXingJie;

		// Token: 0x040013C4 RID: 5060
		[ProtoMember(5)]
		public int JueXingJi;
	}
}
