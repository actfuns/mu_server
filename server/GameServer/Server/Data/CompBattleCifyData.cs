using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompBattleCifyData
	{
		
		[ProtoMember(1)]
		public int CityID;

		
		[ProtoMember(2)]
		public int RoleNum;

		
		[ProtoMember(3)]
		public Dictionary<int, int> StrongholdDict = new Dictionary<int, int>();

		
		[ProtoMember(4)]
		public List<CompBattleZhuJiangInfo> ZhuJiangList = new List<CompBattleZhuJiangInfo>();

		
		[ProtoMember(5)]
		public int OwnCompType;
	}
}
