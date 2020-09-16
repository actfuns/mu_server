using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KingOfBattleAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int BindJinBi;

		
		[ProtoMember(3)]
		public long Exp;

		
		[ProtoMember(4)]
		public List<AwardsItemData> AwardsItemDataList;

		
		[ProtoMember(5)]
		public int SideScore1;

		
		[ProtoMember(6)]
		public int SideScore2;

		
		[ProtoMember(7)]
		public int SelfScore;

		
		[ProtoMember(8)]
		public string MvpRoleName;

		
		[ProtoMember(9)]
		public int MvpOccupation;

		
		[ProtoMember(10)]
		public int MvpRoleSex;
	}
}
