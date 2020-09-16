using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuKingShowDataHist
	{
		
		[ProtoMember(1)]
		public int AdmireCount;

		
		[ProtoMember(2)]
		public DateTime CompleteTime;

		
		[ProtoMember(3)]
		public string BHName;

		
		[ProtoMember(4)]
		public RoleData4Selector RoleData4Selector;
	}
}
