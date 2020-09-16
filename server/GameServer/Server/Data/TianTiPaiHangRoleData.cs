using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTiPaiHangRoleData
	{
		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int Occupation;

		
		[ProtoMember(4)]
		public int ZoneId;

		
		[ProtoMember(5)]
		public int ZhanLi;

		
		[ProtoMember(6)]
		public int DuanWeiId;

		
		[ProtoMember(7)]
		public int DuanWeiJiFen;

		
		[ProtoMember(8)]
		public int DuanWeiRank;

		
		[ProtoMember(9)]
		public RoleData4Selector RoleData4Selector;

		
		[ProtoMember(10)]
		public int ZhengBaGrade;

		
		[ProtoMember(11)]
		public int ZhengBaGroup;

		
		[ProtoMember(12)]
		public int ZhengBaState;
	}
}
