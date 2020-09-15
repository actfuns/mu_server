using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	// Token: 0x02000009 RID: 9
	internal class CoupleArenaDivorceRecord
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00004894 File Offset: 0x00002A94
		public void Add(int roleId1, int roleId2)
		{
			long key = this.GetUnionCouple(roleId1, roleId2);
			if (!this.keySet.Contains(key))
			{
				this.keySet.Add(key);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000048C8 File Offset: 0x00002AC8
		public bool IsDivorce(int roleId1, int roleId2)
		{
			long key = this.GetUnionCouple(roleId1, roleId2);
			return this.keySet.Contains(key);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000048EF File Offset: 0x00002AEF
		public void Reset()
		{
			this.keySet.Clear();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004900 File Offset: 0x00002B00
		private long GetUnionCouple(int a1, int a2)
		{
			int min = Math.Min(a1, a2);
			int max = Math.Max(a1, a2);
			long v = (long)min;
			v <<= 32;
			return v | (long)((ulong)max);
		}

		// Token: 0x04000033 RID: 51
		private HashSet<long> keySet = new HashSet<long>();
	}
}
