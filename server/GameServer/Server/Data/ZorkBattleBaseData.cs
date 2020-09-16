using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZorkBattleBaseData
	{
		
		[ProtoMember(1)]
		public List<int> listAnalysisData = new List<int>();

		
		[ProtoMember(2)]
		public Dictionary<int, int> ArchievementAwardDict = new Dictionary<int, int>();

		
		[ProtoMember(3)]
		public int TeamDuanWei;
	}
}
