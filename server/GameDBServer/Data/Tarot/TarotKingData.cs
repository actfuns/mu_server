using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	
	[ProtoContract]
	public class TarotKingData
	{
		
		public TarotKingData()
		{
			this.AddtionDict = new Dictionary<int, int>();
		}

		
		public TarotKingData(string data)
		{
			string[] info = data.Split(new char[]
			{
				'_'
			});
			if (info.Length == 3)
			{
				this.StartTime = Convert.ToInt64(info[0]);
				this.BufferSecs = Convert.ToInt64(info[1]);
				string[] addDictInfo = info[2].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (addDictInfo.Length == 3)
				{
					this.AddtionDict = new Dictionary<int, int>();
					foreach (string addPart in addDictInfo)
					{
						string[] addPartData = addPart.Split(new char[]
						{
							'@'
						}, StringSplitOptions.RemoveEmptyEntries);
						this.AddtionDict.Add(Convert.ToInt32(addPartData[0]), Convert.ToInt32(addPartData[1]));
					}
				}
			}
		}

		
		public string GetDataStrInfo()
		{
			string addStr = string.Empty;
			if (this.AddtionDict != null && this.AddtionDict.Count == 3)
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
