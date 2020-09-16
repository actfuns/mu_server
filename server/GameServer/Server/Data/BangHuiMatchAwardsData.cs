using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMatchAwardsData
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
		public string MvpRoleName;

		
		[ProtoMember(6)]
		public int MvpOccupation;

		
		[ProtoMember(7)]
		public int MvpRoleSex;

		
		[ProtoMember(8)]
		public string SuccessBHName;
	}
}
