using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class DamageMonitor
	{
		
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

		
		public void Clear(int mapCode, int RoleID)
		{
			this.DictMonitorList.Clear();
		}

		
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

		
		private Dictionary<int, List<int>> DictMonitorList = new Dictionary<int, List<int>>();
	}
}
