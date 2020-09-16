using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class PassiveSkillModule
	{
		
		private void TryTriggerSkills(GameClient client, long nowTicks, SkillTriggerTypes type)
		{
			lock (this.mutex)
			{
				foreach (PassiveSkillData data in this.passiveSkillList.Values)
				{
					if (data.triggerType == (int)type)
					{
						long spanTicks;
						bool b = this._spanTimeDict.TryGetValue(data.skillId, out spanTicks);
						if (!b || spanTicks <= nowTicks)
						{
							int rnd = Global.GetRandomNumber(0, 100);
							if (rnd < data.triggerRate)
							{
								long coolDownTicks;
								b = this.coolDownDict.TryGetValue(data.skillId, out coolDownTicks);
								if (!b || coolDownTicks <= nowTicks)
								{
									this.coolDownDict[data.skillId] = nowTicks + (long)(data.coolDown * 1000);
									this._spanTimeDict[data.skillId] = nowTicks + (long)(data.triggerCD * 1000);
									int posX = client.ClientData.PosX;
									int posY = client.ClientData.PosY;
									SpriteAttack.AddDelayMagic(client, client.ClientData.RoleID, posX, posY, posX, posY, data.skillId);
									EventLogManager.AddRoleSkillEvent(client, SkillLogTypes.PassiveSkillTrigger, LogRecordType.IntValue2, new object[]
									{
										data.skillId,
										data.skillLevel,
										data.triggerRate,
										rnd,
										data.coolDown
									});
								}
							}
						}
					}
				}
			}
		}

		
		public void OnInjured(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks > this.NextTriggerSkillForInjuredTicks)
			{
				this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.Injured);
				this.NextTriggerSkillForInjuredTicks = nowTicks + 1000L;
			}
		}

		
		public void OnProcessMagic(GameClient client, int enemy, int enemyX, int enemyY)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.Attack);
		}

		
		public void OnKillMonster(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.KillMonster);
		}

		
		public void UpdateSkillList(List<PassiveSkillData> skillList)
		{
			if (null != skillList)
			{
				lock (this.mutex)
				{
					this.passiveSkillList.Clear();
					foreach (PassiveSkillData skill in skillList)
					{
						this.passiveSkillList[skill.skillId] = new PassiveSkillData(skill.skillId, skill.skillLevel, skill.triggerType, skill.triggerRate, skill.coolDown, skill.triggerCD);
					}
					foreach (KeyValuePair<int, PassiveSkillData> otherSkill in this.OtherPassiveSkillList)
					{
						this.passiveSkillList[otherSkill.Key] = otherSkill.Value;
					}
				}
			}
		}

		
		public void UpdateOtherSkillList(List<PassiveSkillData> skillList)
		{
			if (null != skillList)
			{
				lock (this.mutex)
				{
					foreach (KeyValuePair<int, PassiveSkillData> skill in this.OtherPassiveSkillList)
					{
						this.passiveSkillList.Remove(skill.Key);
					}
					this.OtherPassiveSkillList.Clear();
					foreach (PassiveSkillData skill2 in skillList)
					{
						this.OtherPassiveSkillList[skill2.skillId] = new PassiveSkillData(skill2.skillId, skill2.skillLevel, skill2.triggerType, skill2.triggerRate, skill2.coolDown, skill2.triggerCD);
						this.passiveSkillList[skill2.skillId] = new PassiveSkillData(skill2.skillId, skill2.skillLevel, skill2.triggerType, skill2.triggerRate, skill2.coolDown, skill2.triggerCD);
					}
				}
			}
		}

		
		public void UpdateSkillData(int magicCode, int level, int triggerType, int triggerRate, int coolDown, int spanCD)
		{
			lock (this.mutex)
			{
				PassiveSkillData data;
				if (this.passiveSkillList.TryGetValue(magicCode, out data))
				{
					data.skillLevel = level;
					data.triggerRate = triggerRate;
					data.coolDown = coolDown;
					data.triggerType = triggerType;
					data.triggerCD = spanCD;
				}
			}
		}

		
		public SkillData GetSkillData(int magicCode)
		{
			lock (this.mutex)
			{
				PassiveSkillData data;
				if (this.passiveSkillList.TryGetValue(magicCode, out data))
				{
					return data.skillData;
				}
			}
			return null;
		}

		
		private object mutex = new object();

		
		public Dictionary<int, PassiveSkillData> passiveSkillList = new Dictionary<int, PassiveSkillData>();

		
		private Dictionary<int, long> coolDownDict = new Dictionary<int, long>();

		
		private Dictionary<int, long> _spanTimeDict = new Dictionary<int, long>();

		
		public Dictionary<int, PassiveSkillData> OtherPassiveSkillList = new Dictionary<int, PassiveSkillData>();

		
		public long NextTriggerSkillForInjuredTicks;

		
		public SkillData currentSkillData = new SkillData();
	}
}
