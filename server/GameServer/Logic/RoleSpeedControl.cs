using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class RoleSpeedControl
	{
		
		
		
		public int LastPosX { get; set; }

		
		
		
		public int LastPosY { get; set; }

		
		
		
		public double LastSlowRate
		{
			get
			{
				long ticks = TimeUtil.NOW();
				int punishSecs = GameManager.GameConfigMgr.GetGameConfigItemInt("punish-speed-secs", 5);
				double result;
				if (ticks - this._LastSlowRateTicks < (long)(punishSecs * 1000))
				{
					result = this._LastSlowRate;
				}
				else
				{
					result = 0.0;
				}
				return result;
			}
			set
			{
				this._LastSlowRateTicks = TimeUtil.NOW();
				this._LastSlowRate = value;
			}
		}

		
		
		
		public long LastDelayTicks { get; set; }

		
		public void AddRoleSpeed(int mapCode, int x, int y, double overflowSpeed)
		{
			RoleSpeedItem roleSpeedItem = new RoleSpeedItem
			{
				MapCode = mapCode,
				X = x,
				Y = y,
				OverflowSpeed = overflowSpeed
			};
			this.RoleSpeedItemList.Add(roleSpeedItem);
		}

		
		public int GetRoleSpeedCount()
		{
			return this.RoleSpeedItemList.Count;
		}

		
		public RoleSpeedItem GetFirstRoleSpeed()
		{
			RoleSpeedItem result;
			if (this.RoleSpeedItemList.Count <= 0)
			{
				result = null;
			}
			else
			{
				result = this.RoleSpeedItemList[0];
			}
			return result;
		}

		
		public void ClearRoleSpeed()
		{
			this.LastPosX = 0;
			this.LastPosY = 0;
			this.RoleSpeedItemList.Clear();
		}

		
		private double _LastSlowRate = 0.0;

		
		private long _LastSlowRateTicks = 0L;

		
		private List<RoleSpeedItem> RoleSpeedItemList = new List<RoleSpeedItem>();
	}
}
