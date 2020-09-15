using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020001E6 RID: 486
	public class PassiveSkillModule
	{
		// Token: 0x06000617 RID: 1559 RVA: 0x00055B80 File Offset: 0x00053D80
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

		// Token: 0x06000618 RID: 1560 RVA: 0x00055D98 File Offset: 0x00053F98
		public void OnInjured(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks > this.NextTriggerSkillForInjuredTicks)
			{
				this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.Injured);
				this.NextTriggerSkillForInjuredTicks = nowTicks + 1000L;
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00055DD8 File Offset: 0x00053FD8
		public void OnProcessMagic(GameClient client, int enemy, int enemyX, int enemyY)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.Attack);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00055DF8 File Offset: 0x00053FF8
		public void OnKillMonster(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.KillMonster);
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00055E18 File Offset: 0x00054018
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

		// Token: 0x0600061C RID: 1564 RVA: 0x00055F50 File Offset: 0x00054150
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

		// Token: 0x0600061D RID: 1565 RVA: 0x000560E8 File Offset: 0x000542E8
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

		// Token: 0x0600061E RID: 1566 RVA: 0x0005616C File Offset: 0x0005436C
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

		// Token: 0x04000AA0 RID: 2720
		private object mutex = new object();

		// Token: 0x04000AA1 RID: 2721
		public Dictionary<int, PassiveSkillData> passiveSkillList = new Dictionary<int, PassiveSkillData>();

		// Token: 0x04000AA2 RID: 2722
		private Dictionary<int, long> coolDownDict = new Dictionary<int, long>();

		// Token: 0x04000AA3 RID: 2723
		private Dictionary<int, long> _spanTimeDict = new Dictionary<int, long>();

		// Token: 0x04000AA4 RID: 2724
		public Dictionary<int, PassiveSkillData> OtherPassiveSkillList = new Dictionary<int, PassiveSkillData>();

		// Token: 0x04000AA5 RID: 2725
		public long NextTriggerSkillForInjuredTicks;

		// Token: 0x04000AA6 RID: 2726
		public SkillData currentSkillData = new SkillData();
	}
}
