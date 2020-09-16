using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class TCPRandKey
	{
		
		public TCPRandKey(int capacity)
		{
			this.ListRandKey = new List<int>(capacity);
			this.DictRandKey = new Dictionary<int, bool>(capacity);
		}

		
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

		
		public bool FindKey(int key)
		{
			return this.DictRandKey.ContainsKey(key);
		}

		
		public int GetKey()
		{
			int randIndex = this.Rand.Next(0, this.ListRandKey.Count);
			return this.ListRandKey[randIndex];
		}

		
		private Random Rand = null;

		
		private List<int> ListRandKey = null;

		
		private Dictionary<int, bool> DictRandKey = null;
	}
}
