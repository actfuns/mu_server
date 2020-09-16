using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RebornEquipData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int HoleID = 0;

		
		[ProtoMember(3)]
		public int Level = 0;

		
		[ProtoMember(4)]
		public int Able = 0;
	}
}
