using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ChengJiuData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public long ChengJiuPoints = 0L;

		
		[ProtoMember(3)]
		public long TotalKilledMonsterNum = 0L;

		
		[ProtoMember(4)]
		public long TotalLoginNum = 0L;

		
		[ProtoMember(5)]
		public int ContinueLoginNum = 0;

		
		[ProtoMember(6)]
		public List<ushort> ChengJiuFlags = null;

		
		[ProtoMember(7)]
		public int NowCompletedChengJiu = 0;

		
		[ProtoMember(8)]
		public long TotalKilledBossNum = 0L;

		
		[ProtoMember(9)]
		public long CompleteNormalCopyMapCount = 0L;

		
		[ProtoMember(10)]
		public long CompleteHardCopyMapCount = 0L;

		
		[ProtoMember(11)]
		public long CompleteDifficltCopyMapCount = 0L;

		
		[ProtoMember(12)]
		public long GuildChengJiu = 0L;

		
		[ProtoMember(13)]
		public long JunXianChengJiu = 0L;
	}
}
