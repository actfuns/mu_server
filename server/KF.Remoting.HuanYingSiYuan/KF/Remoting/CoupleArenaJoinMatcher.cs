using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Data;

namespace KF.Remoting
{
	
	internal class CoupleArenaJoinMatcher
	{
		
		public void AddJoinData(int duanweiType, int duanWeiLevel, CoupleArenaJoinData joinData)
		{
			this.AddJoinData(this.MakeChannel(duanweiType, duanWeiLevel), joinData);
		}

		
		public void AddGlobalJoinData(CoupleArenaJoinData joinData)
		{
			this.AddJoinData(this.GlobalChannel, joinData);
		}

		
		public IEnumerable<List<CoupleArenaJoinData>> GetAllMatch()
		{
			foreach (int channel in this.JoinDatasDict.Keys.ToList<int>())
			{
				yield return this.GetJoinDataList(channel);
			}
			yield break;
		}

		
		private void AddJoinData(int channel, CoupleArenaJoinData joinData)
		{
			if (joinData != null)
			{
				this.GetJoinDataList(channel).Add(joinData);
			}
		}

		
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

		
		private int MakeChannel(int duanweiType, int duanweiLevel)
		{
			return duanweiType * 1000 + duanweiLevel;
		}

		
		private Dictionary<int, List<CoupleArenaJoinData>> JoinDatasDict = new Dictionary<int, List<CoupleArenaJoinData>>();

		
		private readonly int GlobalChannel = -1;
	}
}
