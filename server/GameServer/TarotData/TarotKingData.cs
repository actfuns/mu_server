using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.TarotData
{
	// Token: 0x02000199 RID: 409
	[ProtoContract]
	public class TarotKingData
	{
		// Token: 0x060004DD RID: 1245 RVA: 0x000428E6 File Offset: 0x00040AE6
		public TarotKingData()
		{
			this.AddtionDict = new Dictionary<int, int>();
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0004290C File Offset: 0x00040B0C
		public string GetDataStrInfo()
		{
			string addStr = string.Empty;
			if (this.AddtionDict.Count == 3)
			{
				foreach (KeyValuePair<int, int> addtion in this.AddtionDict)
				{
					object obj = addStr;
					addStr = string.Concat(new object[]
					{
						obj,
						addtion.Key,
						"@",
						addtion.Value,
						","
					});
				}
			}
			return string.Format("{0}_{1}_{2}", this.StartTime, this.BufferSecs, addStr);
		}

		// Token: 0x04000904 RID: 2308
		[ProtoMember(1)]
		public long StartTime = 0L;

		// Token: 0x04000905 RID: 2309
		[ProtoMember(2)]
		public long BufferSecs = 0L;

		// Token: 0x04000906 RID: 2310
		[ProtoMember(3)]
		public Dictionary<int, int> AddtionDict;
	}
}
