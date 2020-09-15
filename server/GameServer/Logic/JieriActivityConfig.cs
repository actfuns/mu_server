using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000701 RID: 1793
	public class JieriActivityConfig
	{
		// Token: 0x06002B1D RID: 11037 RVA: 0x00266CB8 File Offset: 0x00264EB8
		public bool InList(int type)
		{
			return this.ConfigDict.ContainsKey(type);
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x00266CE4 File Offset: 0x00264EE4
		public string GetFileName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ConfigDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x00266D1C File Offset: 0x00264F1C
		public string GetActivityName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ActivityNameDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04003A25 RID: 14885
		public Dictionary<int, string> ConfigDict = new Dictionary<int, string>();

		// Token: 0x04003A26 RID: 14886
		public Dictionary<int, string> ActivityNameDict = new Dictionary<int, string>();

		// Token: 0x04003A27 RID: 14887
		public List<int> openList = new List<int>();
	}
}
