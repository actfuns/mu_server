using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiSaveData
	{
		
		[ProtoMember(1)]
		public bool isWin;

		
		[ProtoMember(2)]
		public int roleId;

		
		[ProtoMember(3)]
		public int level;

		
		[ProtoMember(4)]
		public int changeLiveCount;

		
		[ProtoMember(5)]
		public long nextChallengeTime;

		
		[ProtoMember(6)]
		public double[] baseProps;

		
		[ProtoMember(7)]
		public double[] extProps;

		
		[ProtoMember(8)]
		public List<PlayerJingJiEquipData> equipDatas;

		
		[ProtoMember(9)]
		public List<PlayerJingJiSkillData> skillDatas;

		
		[ProtoMember(10)]
		public int combatForce = 0;

		
		[ProtoMember(11)]
		public int robotId;

		
		[ProtoMember(12)]
		public WingData wingData;

		
		[ProtoMember(13)]
		public long settingFlags;

		
		[ProtoMember(14)]
		public int Occupation;

		
		[ProtoMember(16)]
		public SkillEquipData ShenShiEuipSkill;

		
		[ProtoMember(17)]
		public List<int> PassiveEffectList;

		
		[ProtoMember(18)]
		public int SubOccupation;
	}
}
