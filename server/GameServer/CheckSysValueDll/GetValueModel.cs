using System;
using System.Collections.Generic;
using ProtoBuf;

namespace CheckSysValueDll
{
	// Token: 0x020008EE RID: 2286
	[ProtoContract]
	public class GetValueModel
	{
		// Token: 0x04005003 RID: 20483
		[ProtoMember(1)]
		public string TypeName;

		// Token: 0x04005004 RID: 20484
		[ProtoMember(2)]
		public string SeachName;

		// Token: 0x04005005 RID: 20485
		[ProtoMember(3)]
		public List<SeachData> SeachDataList;
	}
}
