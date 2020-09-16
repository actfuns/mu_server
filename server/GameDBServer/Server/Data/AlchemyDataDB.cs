using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AlchemyDataDB
	{
		
		public string getStringValue(Dictionary<int, int> dict)
		{
			string value = "";
			string result;
			if (null == dict)
			{
				result = value;
			}
			else
			{
				foreach (KeyValuePair<int, int> item in dict)
				{
					value += string.Format("{0},{1}|", item.Key, item.Value);
				}
				if (!string.IsNullOrEmpty(value))
				{
					value = value.Substring(0, value.Length - 1);
				}
				result = value;
			}
			return result;
		}

		
		[ProtoMember(1)]
		public AlchemyData BaseData = new AlchemyData();

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public Dictionary<int, int> HistCost = new Dictionary<int, int>();

		
		[ProtoMember(4)]
		public int ElementDayID = 0;

		
		[ProtoMember(5)]
		public string rollbackType = "";
	}
}
