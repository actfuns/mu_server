using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010E RID: 270
	[ProtoContract]
	public class AlchemyDataDB
	{
		// Token: 0x06000480 RID: 1152 RVA: 0x00024704 File Offset: 0x00022904
		public string getStringValue(Dictionary<int, int> dict)
		{
			string value = "";
			string result;
			if (null == dict)
			{
				result = value;
			}
			else
			{
				foreach (KeyValuePair<int, int> item in dict)
				{
					value += string.Format("{0},{1}|", item.Key, item.Value);
				}
				if (!string.IsNullOrEmpty(value))
				{
					value = value.Substring(0, value.Length - 1);
				}
				result = value;
			}
			return result;
		}

		// Token: 0x04000748 RID: 1864
		[ProtoMember(1)]
		public AlchemyData BaseData = new AlchemyData();

		// Token: 0x04000749 RID: 1865
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x0400074A RID: 1866
		[ProtoMember(3)]
		public Dictionary<int, int> HistCost = new Dictionary<int, int>();

		// Token: 0x0400074B RID: 1867
		[ProtoMember(4)]
		public int ElementDayID = 0;

		// Token: 0x0400074C RID: 1868
		[ProtoMember(5)]
		public string rollbackType = "";
	}
}
