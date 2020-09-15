using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000788 RID: 1928
	public class RoleSpeedControl
	{
		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06003226 RID: 12838 RVA: 0x002CB398 File Offset: 0x002C9598
		// (set) Token: 0x06003227 RID: 12839 RVA: 0x002CB3AF File Offset: 0x002C95AF
		public int LastPosX { get; set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06003228 RID: 12840 RVA: 0x002CB3B8 File Offset: 0x002C95B8
		// (set) Token: 0x06003229 RID: 12841 RVA: 0x002CB3CF File Offset: 0x002C95CF
		public int LastPosY { get; set; }

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x0600322A RID: 12842 RVA: 0x002CB3D8 File Offset: 0x002C95D8
		// (set) Token: 0x0600322B RID: 12843 RVA: 0x002CB42D File Offset: 0x002C962D
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

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x0600322C RID: 12844 RVA: 0x002CB444 File Offset: 0x002C9644
		// (set) Token: 0x0600322D RID: 12845 RVA: 0x002CB45B File Offset: 0x002C965B
		public long LastDelayTicks { get; set; }

		// Token: 0x0600322E RID: 12846 RVA: 0x002CB464 File Offset: 0x002C9664
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

		// Token: 0x0600322F RID: 12847 RVA: 0x002CB4A8 File Offset: 0x002C96A8
		public int GetRoleSpeedCount()
		{
			return this.RoleSpeedItemList.Count;
		}

		// Token: 0x06003230 RID: 12848 RVA: 0x002CB4C8 File Offset: 0x002C96C8
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

		// Token: 0x06003231 RID: 12849 RVA: 0x002CB4FD File Offset: 0x002C96FD
		public void ClearRoleSpeed()
		{
			this.LastPosX = 0;
			this.LastPosY = 0;
			this.RoleSpeedItemList.Clear();
		}

		// Token: 0x04003E61 RID: 15969
		private double _LastSlowRate = 0.0;

		// Token: 0x04003E62 RID: 15970
		private long _LastSlowRateTicks = 0L;

		// Token: 0x04003E63 RID: 15971
		private List<RoleSpeedItem> RoleSpeedItemList = new List<RoleSpeedItem>();
	}
}
