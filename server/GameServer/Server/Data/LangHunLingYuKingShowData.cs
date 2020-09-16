using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuKingShowData
	{
		
		[ProtoMember(1)]
		public int AdmireCount;

		
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;
	}
}
