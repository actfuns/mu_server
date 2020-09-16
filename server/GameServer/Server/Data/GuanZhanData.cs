using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GuanZhanData
	{
		
		[ProtoMember(1)]
		public List<string> SideName = new List<string>();

		
		[ProtoMember(2)]
		public Dictionary<int, List<GuanZhanRoleMiniData>> RoleMiniDataDict = new Dictionary<int, List<GuanZhanRoleMiniData>>();
	}
}
