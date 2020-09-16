using System;
using Server.Data;

namespace GameServer.Logic
{
	
	public class PassiveSkillData
	{
		
		public PassiveSkillData()
		{
		}

		
		public PassiveSkillData(int _skillId, int _skillLevel, int _triggerType, int _triggerRate, int _coolDown, int _spanTime)
		{
			this.skillId = _skillId;
			this.skillLevel = _skillLevel;
			this.triggerType = _triggerType;
			this.triggerRate = _triggerRate;
			this.coolDown = _coolDown;
			this.triggerCD = _spanTime;
			this.skillData.SkillID = this.skillId;
			this.skillData.SkillLevel = this.skillLevel;
		}

		
		public int skillId;

		
		public int skillLevel;

		
		public int triggerRate;

		
		public int triggerType;

		
		public int coolDown;

		
		public int triggerCD;

		
		public SkillData skillData = new SkillData();
	}
}
