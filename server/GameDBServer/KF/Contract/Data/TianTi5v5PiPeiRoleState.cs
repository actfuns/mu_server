using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x020000BD RID: 189
	[ProtoContract]
	[Serializable]
	public class TianTi5v5PiPeiRoleState
	{
		// Token: 0x04000509 RID: 1289
		[ProtoMember(1)]
		public string RoleName;

		// Token: 0x0400050A RID: 1290
		[ProtoMember(2)]
		public int Occupation;

		// Token: 0x0400050B RID: 1291
		[ProtoMember(3)]
		public int State;
	}
}
