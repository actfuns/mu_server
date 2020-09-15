using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AF RID: 1455
	[ProtoContract]
	public class MallSaleData
	{
		// Token: 0x04002905 RID: 10501
		[ProtoMember(1)]
		public string MallXmlString = "";

		// Token: 0x04002906 RID: 10502
		[ProtoMember(2)]
		public string MallTabXmlString = "";

		// Token: 0x04002907 RID: 10503
		[ProtoMember(3)]
		public string QiangGouXmlString = "";
	}
}
