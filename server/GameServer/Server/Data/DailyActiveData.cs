using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class DailyActiveData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public long DailyActiveValues = 0L;

		
		[ProtoMember(3)]
		public long TotalKilledMonsterCount = 0L;

		
		[ProtoMember(4)]
		public long DailyActiveTotalLoginCount = 0L;

		
		[ProtoMember(5)]
		public int DailyActiveOnLineTimer = 0;

		
		[ProtoMember(6)]
		public List<ushort> DailyActiveInforFlags = null;

		
		[ProtoMember(7)]
		public int NowCompletedDailyActiveID = 0;

		
		[ProtoMember(8)]
		public int TotalKilledBossCount = 0;

		
		[ProtoMember(9)]
		public int PassNormalCopySceneNum = 0;

		
		[ProtoMember(10)]
		public int PassHardCopySceneNum = 0;

		
		[ProtoMember(11)]
		public int PassDifficultCopySceneNum = 0;

		
		[ProtoMember(12)]
		public int BuyItemInMall = 0;

		
		[ProtoMember(13)]
		public int CompleteDailyTaskCount = 0;

		
		[ProtoMember(14)]
		public int CompleteBloodCastleCount = 0;

		
		[ProtoMember(15)]
		public int CompleteDaimonSquareCount = 0;

		
		[ProtoMember(16)]
		public int CompleteBattleCount = 0;

		
		[ProtoMember(17)]
		public int EquipForge = 0;

		
		[ProtoMember(18)]
		public int EquipAppend = 0;

		
		[ProtoMember(19)]
		public int ChangeLife = 0;

		
		[ProtoMember(20)]
		public int MergeFruit = 0;

		
		[ProtoMember(21)]
		public int GetAwardFlag = 0;
	}
}
