using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000112 RID: 274
	[ProtoContract]
	public class ActivitiesData
	{
		// Token: 0x040005C2 RID: 1474
		[ProtoMember(1)]
		public string ActivitiesXmlString = "";
	}
}
