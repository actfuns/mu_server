using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompZhiWuData
	{
		
		[ProtoMember(1)]
		public List<KFCompRoleData> CompRoleData = new List<KFCompRoleData>();
	}
}
