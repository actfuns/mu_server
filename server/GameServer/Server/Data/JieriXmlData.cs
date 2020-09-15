using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200055E RID: 1374
	[ProtoContract]
	public class JieriXmlData : ICompressed
	{
		// Token: 0x04002507 RID: 9479
		[ProtoMember(1)]
		public List<string> XmlList = null;

		// Token: 0x04002508 RID: 9480
		[ProtoMember(2)]
		public int Version;
	}
}
