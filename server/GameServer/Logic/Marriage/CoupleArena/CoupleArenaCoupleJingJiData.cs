using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaCoupleJingJiData
	{
		
		[ProtoMember(1)]
		public int ManRoleId;

		
		[ProtoMember(2)]
		public int ManZoneId;

		
		[ProtoMember(3)]
		public RoleData4Selector ManSelector;

		
		[ProtoMember(4)]
		public int WifeRoleId;

		
		[ProtoMember(5)]
		public int WifeZoneId;

		
		[ProtoMember(6)]
		public RoleData4Selector WifeSelector;

		
		[ProtoMember(7)]
		public int TotalFightTimes;

		
		[ProtoMember(8)]
		public int WinFightTimes;

		
		[ProtoMember(9)]
		public int LianShengTimes;

		
		[ProtoMember(10)]
		public int DuanWeiType;

		
		[ProtoMember(11)]
		public int DuanWeiLevel;

		
		[ProtoMember(12)]
		public int JiFen;

		
		[ProtoMember(13)]
		public int Rank;

		
		public int IsDivorced;
	}
}
