using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuWorldData
	{
		
		[ProtoMember(1)]
		public List<LangHunLingYuCityData> CityList = new List<LangHunLingYuCityData>();
	}
}
