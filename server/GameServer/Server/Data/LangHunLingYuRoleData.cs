using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuRoleData
	{
		
		[ProtoMember(1)]
		public int SignUpState;

		
		[ProtoMember(2)]
		public List<int> GetDayAwardsState;

		
		[ProtoMember(3)]
		public List<LangHunLingYuCityData> SelfCityList = new List<LangHunLingYuCityData>();

		
		[ProtoMember(4)]
		public List<LangHunLingYuCityData> OtherCityList = new List<LangHunLingYuCityData>();
	}
}
