using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EraRankData
	{
		
		[ProtoMember(1)]
		public int RankValue;

		
		[ProtoMember(2)]
		public int JunTuanID;

		
		[ProtoMember(3)]
		public string JunTuanName;

		
		[ProtoMember(4)]
		public byte EraStage;

		
		[ProtoMember(5)]
		public int EraStageProcess;
	}
}
