using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiMissionMainData
	{
		
		[ProtoMember(1)]
		public List<YaoSaiMissionData> MissionDataList;

		
		[ProtoMember(2)]
		public int ExcuteMissionCount;

		
		[ProtoMember(3)]
		public DateTime FreeRefreshTime;
	}
}
