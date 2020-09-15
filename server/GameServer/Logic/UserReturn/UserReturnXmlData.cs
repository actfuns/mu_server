using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B4 RID: 1204
	[ProtoContract]
	public class UserReturnXmlData
	{
		// Token: 0x04001FF6 RID: 8182
		[ProtoMember(1)]
		public List<string> XmlNameList;

		// Token: 0x04001FF7 RID: 8183
		[ProtoMember(2)]
		public List<string> XmlList;
	}
}
