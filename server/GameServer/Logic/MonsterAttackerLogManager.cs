using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	
	public class MonsterAttackerLogManager : SingletonTemplate<MonsterAttackerLogManager>, IEventListener
	{
		
		private MonsterAttackerLogManager()
		{
			GlobalEventSource.getInstance().registerListener(11, this);
		}

		
		public void LoadRecordMonsters()
		{
			lock (this.Mutex)
			{
				this.NeedRecordLogMonsters.Clear();
				int[] monsterIds = GameManager.systemParamsList.GetParamValueIntArrayByName("LogAttackBoss", ',');
				int i = 0;
				while (monsterIds != null && i < monsterIds.Length)
				{
					if (!this.NeedRecordLogMonsters.Contains(monsterIds[i]))
					{
						this.NeedRecordLogMonsters.Add(monsterIds[i]);
					}
					i++;
				}
			}
		}

		
		public bool IsNeedRecordAttackLog(int monsterId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.NeedRecordLogMonsters.Contains(monsterId);
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEvent = eventObject as MonsterDeadEventObject;
				if (null != monsterDeadEvent)
				{
					Monster i = monsterDeadEvent.getMonster();
					if (i != null)
					{
						if (this.IsNeedRecordAttackLog(i.MonsterInfo.ExtensionID))
						{
							string log = i.BuildAttackerLog();
							LogManager.WriteLog(LogTypes.Attack, log, null, true);
						}
					}
				}
			}
		}

		
		public void AddRoleRelifeLog(RoleRelifeLog log)
		{
			if (log != null)
			{
				if (log.hpModify || log.mpModify)
				{
					bool bNeedLog = false;
					lock (this.Mutex)
					{
						bNeedLog = this.NeedRecordRelifeRoles.Contains(log.RoleId);
					}
					if (bNeedLog)
					{
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat("回血日志， roleid={0}, rolename={1}, mapcode={2}, reason={3}", new object[]
						{
							log.RoleId,
							log.Rolename,
							log.MapCode,
							log.Reason
						});
						if (log.hpModify)
						{
							sb.AppendFormat(" ,oldHp={0}, newHp={1}, addHp={2}", log.oldHp, log.newHp, log.newHp - log.oldHp);
						}
						if (log.mpModify)
						{
							sb.AppendFormat(" ,oldMp={0}, newMp={1}, addMp={2}", log.oldMp, log.newMp, log.newMp - log.oldMp);
						}
						LogManager.WriteLog(LogTypes.Attack, sb.ToString(), null, true);
					}
				}
			}
		}

		
		public void SetLogRoleRelife(int roleId, bool bLog = true)
		{
			lock (this.Mutex)
			{
				if (bLog && !this.NeedRecordRelifeRoles.Contains(roleId))
				{
					this.NeedRecordRelifeRoles.Add(roleId);
				}
				if (!bLog)
				{
					this.NeedRecordRelifeRoles.Remove(roleId);
				}
			}
		}

		
		private object Mutex = new object();

		
		private HashSet<int> NeedRecordLogMonsters = new HashSet<int>();

		
		private HashSet<int> NeedRecordRelifeRoles = new HashSet<int>();
	}
}
