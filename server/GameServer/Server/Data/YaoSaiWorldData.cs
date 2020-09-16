using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiWorldData
	{
		
		[ProtoMember(1)]
		public int state;

		
		[ProtoMember(2)]
		public PrisonRoleData Mine = new PrisonRoleData();

		
		[ProtoMember(3)]
		public PrisonRoleData Master = new PrisonRoleData();

		
		[ProtoMember(4)]
		public int zhenfuCount;

		
		[ProtoMember(5)]
		public int zhenfuLeftCount;

		
		[ProtoMember(6)]
		public int jiejiuCount;

		
		[ProtoMember(7)]
		public List<PrisonFuLuData> FuLuList = new List<PrisonFuLuData>();
	}
}
