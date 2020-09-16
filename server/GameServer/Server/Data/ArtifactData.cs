using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ArtifactData
	{
		
		[ProtoMember(1)]
		public int ArtifactID = 0;

		
		[ProtoMember(2)]
		public string ArtifactName = "";

		
		[ProtoMember(3)]
		public int NewEquitID = 0;

		
		[ProtoMember(4)]
		public int NeedEquitID = 0;

		
		[ProtoMember(5)]
		public Dictionary<int, int> NeedMaterial = null;

		
		[ProtoMember(6)]
		public int NeedGoldBind = 0;

		
		[ProtoMember(7)]
		public int NeedZaiZao = 0;

		
		[ProtoMember(8)]
		public Dictionary<int, int> FailMaterial = null;

		
		[ProtoMember(9)]
		public int SuccessRate = 0;
	}
}
