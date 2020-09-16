using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMemberData
	{
		
		[ProtoMember(1)]
		public int ZoneID = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public string RoleName = "";

		
		[ProtoMember(4)]
		public int Occupation = 0;

		
		[ProtoMember(5)]
		public int BHZhiwu = 0;

		
		[ProtoMember(6)]
		public string ChengHao = "";

		
		[ProtoMember(7)]
		public int BangGong = 0;

		
		[ProtoMember(8)]
		public int Level = 0;

		
		[ProtoMember(9)]
		public int XueWeiNum = 0;

		
		[ProtoMember(10)]
		public int SkillLearnedNum = 0;

		
		[ProtoMember(11)]
		public int OnlineState = 0;

		
		[ProtoMember(12)]
		public int BangHuiMemberCombatForce = 0;

		
		[ProtoMember(13)]
		public int BangHuiMemberChangeLifeLev = 0;

		
		[ProtoMember(14)]
		public int JunTuanZhiWu;

		
		[ProtoMember(15)]
		public int YaoSaiBossState;

		
		[ProtoMember(16)]
		public int YaoSaiJianYuState;

		
		[ProtoMember(18)]
		public long LogOffTime;
	}
}
