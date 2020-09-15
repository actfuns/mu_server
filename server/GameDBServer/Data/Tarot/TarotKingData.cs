using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	// Token: 0x020000B8 RID: 184
	[ProtoContract]
	public class TarotKingData
	{
		// Token: 0x0600019C RID: 412 RVA: 0x00008C24 File Offset: 0x00006E24
		public TarotKingData()
		{
			this.AddtionDict = new Dictionary<int, int>();
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008C4C File Offset: 0x00006E4C
		public TarotKingData(string data)
		{
			string[] info = data.Split(new char[]
			{
				'_'
			});
			if (info.Length == 3)
			{
				this.StartTime = Convert.ToInt64(info[0]);
				this.BufferSecs = Convert.ToInt64(info[1]);
				string[] addDictInfo = info[2].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (addDictInfo.Length == 3)
				{
					this.AddtionDict = new Dictionary<int, int>();
					foreach (string addPart in addDictInfo)
					{
						string[] addPartData = addPart.Split(new char[]
						{
							'@'
						}, StringSplitOptions.RemoveEmptyEntries);
						this.AddtionDict.Add(Convert.ToInt32(addPartData[0]), Convert.ToInt32(addPartData[1]));
					}
				}
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008D48 File Offset: 0x00006F48
		public string GetDataStrInfo()
		{
			string addStr = string.Empty;
			if (this.AddtionDict != null && this.AddtionDict.Count == 3)
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

		// Token: 0x040004D8 RID: 1240
		[ProtoMember(1)]
		public long StartTime = 0L;

		// Token: 0x040004D9 RID: 1241
		[ProtoMember(2)]
		public long BufferSecs = 0L;

		// Token: 0x040004DA RID: 1242
		[ProtoMember(3)]
		public Dictionary<int, int> AddtionDict;
	}
}
