using System;
using System.Collections.Generic;
using ProtoBuf;

namespace CheckSysValueDll
{
	// Token: 0x020008ED RID: 2285
	[ProtoContract]
	public class CheckValueResultItem
	{
		// Token: 0x04005000 RID: 20480
		[ProtoMember(1)]
		public string TypeName;

		// Token: 0x04005001 RID: 20481
		[ProtoMember(2)]
		public object StrValue;

		// Token: 0x04005002 RID: 20482
		[ProtoMember(3)]
		public List<string> Childs;
	}
}
