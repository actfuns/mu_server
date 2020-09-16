using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.TarotData
{
	
	[ProtoContract]
	public class TarotKingData
	{
		
		public TarotKingData()
		{
			this.AddtionDict = new Dictionary<int, int>();
		}

		
		public string GetDataStrInfo()
		{
			string addStr = string.Empty;
			if (this.AddtionDict.Count == 3)
			{
				foreach (KeyValuePair<int, int> addtion in this.AddtionDict)
				{
					object obj = addStr;
					addStr = string.Concat(new object[]
					{
						obj,
						addtion.Key,
						"@",
						addtion.Value,
						","
					});
				}
			}
			return string.Format("{0}_{1}_{2}", this.StartTime, this.BufferSecs, addStr);
		}

		
		[ProtoMember(1)]
		public long StartTime = 0L;

		
		[ProtoMember(2)]
		public long BufferSecs = 0L;

		
		[ProtoMember(3)]
		public Dictionary<int, int> AddtionDict;
	}
}
