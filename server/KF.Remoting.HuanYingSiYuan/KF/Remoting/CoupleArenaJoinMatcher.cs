using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Data;

namespace KF.Remoting
{
	// Token: 0x0200000A RID: 10
	internal class CoupleArenaJoinMatcher
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00004944 File Offset: 0x00002B44
		public void AddJoinData(int duanweiType, int duanWeiLevel, CoupleArenaJoinData joinData)
		{
			this.AddJoinData(this.MakeChannel(duanweiType, duanWeiLevel), joinData);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004957 File Offset: 0x00002B57
		public void AddGlobalJoinData(CoupleArenaJoinData joinData)
		{
			this.AddJoinData(this.GlobalChannel, joinData);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004B5C File Offset: 0x00002D5C
		public IEnumerable<List<CoupleArenaJoinData>> GetAllMatch()
		{
			foreach (int channel in this.JoinDatasDict.Keys.ToList<int>())
			{
				yield return this.GetJoinDataList(channel);
			}
			yield break;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004B80 File Offset: 0x00002D80
		private void AddJoinData(int channel, CoupleArenaJoinData joinData)
		{
			if (joinData != null)
			{
				this.GetJoinDataList(channel).Add(joinData);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004BAC File Offset: 0x00002DAC
		private List<CoupleArenaJoinData> GetJoinDataList(int unionDuanwei)
		{
			List<CoupleArenaJoinData> result;
			if (!this.JoinDatasDict.TryGetValue(unionDuanwei, out result))
			{
				result = new List<CoupleArenaJoinData>();
				this.JoinDatasDict.Add(unionDuanwei, result);
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004BE8 File Offset: 0x00002DE8
		private int MakeChannel(int duanweiType, int duanweiLevel)
		{
			return duanweiType * 1000 + duanweiLevel;
		}

		// Token: 0x04000034 RID: 52
		private Dictionary<int, List<CoupleArenaJoinData>> JoinDatasDict = new Dictionary<int, List<CoupleArenaJoinData>>();

		// Token: 0x04000035 RID: 53
		private readonly int GlobalChannel = -1;
	}
}
