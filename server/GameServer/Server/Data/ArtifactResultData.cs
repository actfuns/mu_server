using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ArtifactResultData
	{
		
		[ProtoMember(1)]
		public int State = 0;

		
		[ProtoMember(2)]
		public int EquipDbID = 0;

		
		[ProtoMember(3)]
		public int Bind = 0;
	}
}
