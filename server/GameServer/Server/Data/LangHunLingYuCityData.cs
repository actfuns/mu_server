using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuCityData
	{
		
		[ProtoMember(1)]
		public int CityId;

		
		[ProtoMember(2)]
		public int CityLevel;

		
		[ProtoMember(3)]
		public BangHuiMiniData Owner;

		
		[ProtoMember(4)]
		public List<BangHuiMiniData> AttackerList = new List<BangHuiMiniData>();
	}
}
