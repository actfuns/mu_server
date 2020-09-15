using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020008C3 RID: 2243
	public class TCPRandKey
	{
		// Token: 0x06003FE5 RID: 16357 RVA: 0x003B9CBE File Offset: 0x003B7EBE
		public TCPRandKey(int capacity)
		{
			this.ListRandKey = new List<int>(capacity);
			this.DictRandKey = new Dictionary<int, bool>(capacity);
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x003B9CF8 File Offset: 0x003B7EF8
		public void Init(int count, int randSeed)
		{
			this.Rand = new Random(randSeed);
			for (int i = 0; i < count; i++)
			{
				int key = this.Rand.Next(0, int.MaxValue);
				this.ListRandKey.Add(key);
				this.DictRandKey.Add(key, true);
			}
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x003B9D54 File Offset: 0x003B7F54
		public bool FindKey(int key)
		{
			return this.DictRandKey.ContainsKey(key);
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x003B9D74 File Offset: 0x003B7F74
		public int GetKey()
		{
			int randIndex = this.Rand.Next(0, this.ListRandKey.Count);
			return this.ListRandKey[randIndex];
		}

		// Token: 0x04004EF8 RID: 20216
		private Random Rand = null;

		// Token: 0x04004EF9 RID: 20217
		private List<int> ListRandKey = null;

		// Token: 0x04004EFA RID: 20218
		private Dictionary<int, bool> DictRandKey = null;
	}
}
