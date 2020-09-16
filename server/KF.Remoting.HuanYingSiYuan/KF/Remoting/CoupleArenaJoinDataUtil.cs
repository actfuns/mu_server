using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Data;

namespace KF.Remoting
{
	
	internal class CoupleArenaJoinDataUtil
	{
		
		public CoupleArenaJoinData Create()
		{
			CoupleArenaJoinData joinData = new CoupleArenaJoinData();
			joinData.JoinSeq = this.CurrJoinSeq;
			this.CurrJoinSeq += 1U;
			return joinData;
		}

		
		public List<CoupleArenaJoinData> GetJoinList()
		{
			return this.JoinDataDict.Values.ToList<CoupleArenaJoinData>();
		}

		
		public void Reset()
		{
			this.JoinDataDict.Clear();
			this.RoleJoinSeqDict.Clear();
		}

		
		public void AddJoinData(CoupleArenaJoinData joinData)
		{
			if (joinData != null)
			{
				this.JoinDataDict.Add(joinData.JoinSeq, joinData);
				this.RoleJoinSeqDict[joinData.RoleId1] = joinData.JoinSeq;
				this.RoleJoinSeqDict[joinData.RoleId2] = joinData.JoinSeq;
			}
		}

		
		public void DelJoinData(CoupleArenaJoinData joinData)
		{
			if (joinData != null)
			{
				if (this.JoinDataDict.Remove(joinData.JoinSeq))
				{
					this.RoleJoinSeqDict.Remove(joinData.RoleId1);
					this.RoleJoinSeqDict.Remove(joinData.RoleId2);
				}
			}
		}

		
		public CoupleArenaJoinData GetJoinData(int roleId)
		{
			uint joinSeq;
			if (this.RoleJoinSeqDict.TryGetValue(roleId, out joinSeq))
			{
				CoupleArenaJoinData joinData;
				if (this.JoinDataDict.TryGetValue(joinSeq, out joinData))
				{
					return joinData;
				}
			}
			return null;
		}

		
		private uint CurrJoinSeq = 1U;

		
		private Dictionary<uint, CoupleArenaJoinData> JoinDataDict = new Dictionary<uint, CoupleArenaJoinData>();

		
		private Dictionary<int, uint> RoleJoinSeqDict = new Dictionary<int, uint>();
	}
}
