using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiData
	{
		
		[ProtoMember(1)]
		public int roleId;

		
		[ProtoMember(2)]
		public string roleName;

		
		[ProtoMember(3)]
		public int level;

		
		[ProtoMember(4)]
		public int changeLiveCount;

		
		[ProtoMember(5)]
		public int occupationId;

		
		[ProtoMember(6)]
		public int winCount = 0;

		
		[ProtoMember(7)]
		public int ranking = -1;

		
		[ProtoMember(8)]
		public long nextRewardTime;

		
		[ProtoMember(9)]
		public long nextChallengeTime;

		
		[ProtoMember(10)]
		public double[] baseProps;

		
		[ProtoMember(11)]
		public double[] extProps;

		
		[ProtoMember(12)]
		public List<PlayerJingJiEquipData> equipDatas;

		
		[ProtoMember(13)]
		public List<PlayerJingJiSkillData> skillDatas;

		
		[ProtoMember(14)]
		public int combatForce = 0;

		
		[ProtoMember(15)]
		public int sex;

		
		[ProtoMember(16)]
		public string name;

		
		[ProtoMember(17)]
		public int zoneId;

		
		[ProtoMember(18)]
		public int MaxWinCnt;

		
		[ProtoMember(19)]
		public WingData wingData;

		
		[ProtoMember(20)]
		public long settingFlags;

		
		[ProtoMember(21)]
		public int AdmiredCount;

		
		[ProtoMember(22)]
		public List<int> OccupationList;

		
		[ProtoMember(23)]
		public int JunTuanId;

		
		[ProtoMember(24)]
		public string JunTuanName;

		
		[ProtoMember(25)]
		public int JunTuanZhiWu;

		
		[ProtoMember(26)]
		public int LingDi;

		
		[ProtoMember(27)]
		public RoleHuiJiData HuiJiData;

		
		[ProtoMember(28)]
		public SkillEquipData ShenShiEquipData;

		
		[ProtoMember(29)]
		public List<int> PassiveEffectList;

		
		[ProtoMember(30)]
		public int CompType;

		
		[ProtoMember(31)]
		public byte CompZhiWu;

		
		[ProtoMember(32)]
		public int SubOccupation;
	}
}
