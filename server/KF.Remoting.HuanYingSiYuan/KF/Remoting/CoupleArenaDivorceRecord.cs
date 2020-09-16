using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	internal class CoupleArenaDivorceRecord
	{
		
		public void Add(int roleId1, int roleId2)
		{
			long key = this.GetUnionCouple(roleId1, roleId2);
			if (!this.keySet.Contains(key))
			{
				this.keySet.Add(key);
			}
		}

		
		public bool IsDivorce(int roleId1, int roleId2)
		{
			long key = this.GetUnionCouple(roleId1, roleId2);
			return this.keySet.Contains(key);
		}

		
		public void Reset()
		{
			this.keySet.Clear();
		}

		
		private long GetUnionCouple(int a1, int a2)
		{
			int min = Math.Min(a1, a2);
			int max = Math.Max(a1, a2);
			long v = (long)min;
			v <<= 32;
			return v | (long)((ulong)max);
		}

		
		private HashSet<long> keySet = new HashSet<long>();
	}
}
