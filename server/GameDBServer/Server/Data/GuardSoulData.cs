using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GuardSoulData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int Type = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public int EquipSlot = -1;
	}
}
