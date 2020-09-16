using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ArtifactSuitData
	{
		
		[ProtoMember(1)]
		public int SuitID = 0;

		
		[ProtoMember(2)]
		public string SuitName = "";

		
		[ProtoMember(3)]
		public List<int> EquipIDList = null;

		
		[ProtoMember(4)]
		public bool IsMulti = false;

		
		[ProtoMember(5)]
		public Dictionary<int, Dictionary<string, string>> SuitAttr = null;
	}
}
