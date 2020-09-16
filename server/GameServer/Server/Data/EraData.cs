using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EraData
	{
		
		[ProtoMember(1)]
		public int EraID;

		
		[ProtoMember(2)]
		public byte EraStage;

		
		[ProtoMember(3)]
		public int EraStageProcess;

		
		[ProtoMember(4)]
		public byte FastEraStage;

		
		[ProtoMember(5)]
		public int FastEraStateProcess;

		
		[ProtoMember(6)]
		public List<EraTaskData> EraTaskList = new List<EraTaskData>();

		
		[ProtoMember(7)]
		public List<EraRankData> EraRankList = new List<EraRankData>();

		
		[ProtoMember(8)]
		public Dictionary<int, int> EraAwardStateDict = new Dictionary<int, int>();
	}
}
