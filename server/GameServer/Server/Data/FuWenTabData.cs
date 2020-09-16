using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuWenTabData
	{
		
		[ProtoMember(1)]
		public int TabID;

		
		[ProtoMember(2)]
		public string Name;

		
		[ProtoMember(3)]
		public List<int> FuWenEquipList;

		
		[ProtoMember(4)]
		public List<int> ShenShiActiveList;

		
		[ProtoMember(5)]
		public int SkillEquip;

		
		[ProtoMember(6)]
		public int OwnerID;
	}
}
