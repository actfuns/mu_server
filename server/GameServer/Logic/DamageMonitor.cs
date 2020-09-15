using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200028F RID: 655
	public class DamageMonitor
	{
		// Token: 0x06000983 RID: 2435 RVA: 0x00096F34 File Offset: 0x00095134
		public void Set(int mapCode, int RoleID)
		{
			if (!this.Get(mapCode, RoleID))
			{
				if (this.DictMonitorList.ContainsKey(mapCode))
				{
					this.DictMonitorList[mapCode].Add(RoleID);
				}
				else
				{
					List<int> MonitorList = new List<int>();
					MonitorList.Add(RoleID);
					this.DictMonitorList[mapCode] = MonitorList;
				}
			}
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00096F9C File Offset: 0x0009519C
		public void Remove(int mapCode, int RoleID)
		{
			if (this.Get(mapCode, RoleID))
			{
				if (this.DictMonitorList.ContainsKey(mapCode))
				{
					this.DictMonitorList[mapCode].Remove(RoleID);
				}
			}
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x00096FE4 File Offset: 0x000951E4
		public bool Get(int mapCode, int RoleID)
		{
			if (this.DictMonitorList.ContainsKey(mapCode))
			{
				if (this.DictMonitorList[mapCode].IndexOf(RoleID) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0009702A File Offset: 0x0009522A
		public void Clear(int mapCode, int RoleID)
		{
			this.DictMonitorList.Clear();
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0009703C File Offset: 0x0009523C
		public void Out(GameClient client)
		{
			if (null != client)
			{
				if (this.Get(1, client.ClientData.RoleID))
				{
					long nowTicks = TimeUtil.NOW();
					string DamageInfo = string.Format("个人伤害统计,ticks={6}, mapcode={0}, roleid={1}, magiccode={2}, damage={3}, posx={4}, posy={5},vipexp={7}", new object[]
					{
						client.ClientData.MapCode,
						client.ClientData.RoleID,
						client.CheckCheatData.LastMagicCode,
						client.CheckCheatData.LastDamage,
						client.ClientData.PosX,
						client.ClientData.PosY,
						nowTicks,
						client.ClientData.VipExp
					});
					if (client.CheckCheatData.LastEnemyID > 0)
					{
						DamageInfo += string.Format("LastEnemyID={0} LastEnemyName={1} LastEnemyPosX={2} LastEnemyPosY={3} dist={4:00} ", new object[]
						{
							client.CheckCheatData.LastEnemyID,
							client.CheckCheatData.LastEnemyName,
							client.CheckCheatData.LastEnemyPos.X,
							client.CheckCheatData.LastEnemyPos.Y,
							Global.GetTwoPointDistance(client.CurrentPos, client.CheckCheatData.LastEnemyPos)
						});
					}
					for (int i = 0; i < 12; i++)
					{
						if (Global.GetIntSomeBit(client.CheckCheatData.LastDamageType, i) == 1)
						{
							DamageInfo += string.Format("damagetype={0}", (DamageType)i);
						}
					}
					client.CheckCheatData.LastMagicCode = 0;
					client.CheckCheatData.LastDamage = 0L;
					client.CheckCheatData.LastDamageType = 0;
					client.CheckCheatData.LastEnemyID = 0;
					client.CheckCheatData.LastEnemyName = "";
					client.CheckCheatData.LastEnemyPos.X = 0.0;
					client.CheckCheatData.LastEnemyPos.Y = 0.0;
					LogManager.WriteLog(LogTypes.Error, DamageInfo, null, true);
				}
			}
		}

		// Token: 0x0400103D RID: 4157
		private Dictionary<int, List<int>> DictMonitorList = new Dictionary<int, List<int>>();
	}
}
