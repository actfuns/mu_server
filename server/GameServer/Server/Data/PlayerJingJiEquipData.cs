using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiEquipData
	{
		
		[ProtoMember(1)]
		public int EquipId;

		
		[ProtoMember(2)]
		public int Forge_level;

		
		[ProtoMember(3)]
		public int ExcellenceInfo;

		
		[ProtoMember(4)]
		public int BagIndex;
	}
}
