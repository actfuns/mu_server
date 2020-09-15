using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Data;

namespace KF.Remoting
{
	// Token: 0x0200000B RID: 11
	internal class CoupleArenaJoinDataUtil
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00004C20 File Offset: 0x00002E20
		public CoupleArenaJoinData Create()
		{
			CoupleArenaJoinData joinData = new CoupleArenaJoinData();
			joinData.JoinSeq = this.CurrJoinSeq;
			this.CurrJoinSeq += 1U;
			return joinData;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004C54 File Offset: 0x00002E54
		public List<CoupleArenaJoinData> GetJoinList()
		{
			return this.JoinDataDict.Values.ToList<CoupleArenaJoinData>();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004C76 File Offset: 0x00002E76
		public void Reset()
		{
			this.JoinDataDict.Clear();
			this.RoleJoinSeqDict.Clear();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004C94 File Offset: 0x00002E94
		public void AddJoinData(CoupleArenaJoinData joinData)
		{
			if (joinData != null)
			{
				this.JoinDataDict.Add(joinData.JoinSeq, joinData);
				this.RoleJoinSeqDict[joinData.RoleId1] = joinData.JoinSeq;
				this.RoleJoinSeqDict[joinData.RoleId2] = joinData.JoinSeq;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004CF4 File Offset: 0x00002EF4
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

		// Token: 0x0600005A RID: 90 RVA: 0x00004D50 File Offset: 0x00002F50
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

		// Token: 0x04000036 RID: 54
		private uint CurrJoinSeq = 1U;

		// Token: 0x04000037 RID: 55
		private Dictionary<uint, CoupleArenaJoinData> JoinDataDict = new Dictionary<uint, CoupleArenaJoinData>();

		// Token: 0x04000038 RID: 56
		private Dictionary<int, uint> RoleJoinSeqDict = new Dictionary<int, uint>();
	}
}
