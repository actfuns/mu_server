using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C3 RID: 963
	[ProtoContract]
	public class LangHunLingYuWorldData
	{
		// Token: 0x04001926 RID: 6438
		[ProtoMember(1)]
		public List<LangHunLingYuCityData> CityList = new List<LangHunLingYuCityData>();
	}
}
