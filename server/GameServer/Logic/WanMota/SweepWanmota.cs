using System;
using System.Collections.Generic;
using System.Timers;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.WanMota
{
	// Token: 0x020007B1 RID: 1969
	public class SweepWanmota
	{
		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060033C8 RID: 13256 RVA: 0x002DDE34 File Offset: 0x002DC034
		// (set) Token: 0x060033C9 RID: 13257 RVA: 0x002DDE4C File Offset: 0x002DC04C
		public Timer WanMoTaSweepingTimer
		{
			get
			{
				return this._WanMoTaSweepingTimer;
			}
			set
			{
				this._WanMoTaSweepingTimer = value;
			}
		}

		// Token: 0x060033CA RID: 13258 RVA: 0x002DDE56 File Offset: 0x002DC056
		public SweepWanmota(GameClient client)
		{
			this.sweepClient = client;
		}

		// Token: 0x060033CB RID: 13259 RVA: 0x002DDE78 File Offset: 0x002DC078
		public void BeginSweeping()
		{
			if (null == this.WanMoTaSweepingTimer)
			{
				this.WanMoTaSweepingTimer = new Timer(2000.0);
				this.WanMoTaSweepingTimer.Elapsed += this.Sweeping;
				this.WanMoTaSweepingTimer.Interval = 2000.0;
				this.WanMoTaSweepingTimer.Enabled = true;
			}
		}

		// Token: 0x060033CC RID: 13260 RVA: 0x002DDEE8 File Offset: 0x002DC0E8
		public void StopSweeping()
		{
			if (null != this.WanMoTaSweepingTimer)
			{
				lock (this.WanMoTaSweepingTimer)
				{
					this.WanMoTaSweepingTimer.Enabled = false;
					this.WanMoTaSweepingTimer.Stop();
					this.WanMoTaSweepingTimer = null;
				}
			}
		}

		// Token: 0x060033CD RID: 13261 RVA: 0x002DDF60 File Offset: 0x002DC160
		private void Sweeping(object source, ElapsedEventArgs e)
		{
			lock (this.sweepClient)
			{
				WanMotaCopySceneManager.GetWanmotaSweepReward(this.sweepClient, WanMotaCopySceneManager.nWanMoTaFirstFuBenOrder + this.nSweepingOrder - 1);
				this.nSweepingOrder++;
				if (this.nSweepingOrder > this.nSweepingMaxOrder)
				{
					this.StopSweeping();
					List<SingleLayerRewardData> listRewardData = SweepWanMotaManager.SummarySweepRewardInfo(this.sweepClient);
					List<SingleLayerRewardData> WanMoTaLayerRewardList = this.sweepClient.ClientData.LayerRewardData.WanMoTaLayerRewardList;
					this.sweepClient.ClientData.LayerRewardData.WanMoTaLayerRewardList = listRewardData;
					if (-1 == WanMoTaDBCommandManager.UpdateSweepAwardDBCommand(this.sweepClient, 0))
					{
						LogManager.WriteLog(LogTypes.Error, "扫荡奖励汇总后，写到数据库失败", null, true);
						this.sweepClient.ClientData.LayerRewardData.WanMoTaLayerRewardList = WanMoTaLayerRewardList;
					}
					else
					{
						this.sweepClient.ClientData.WanMoTaProp.nSweepLayer = 0;
						SweepWanMotaManager.UpdataSweepInfo(this.sweepClient, listRewardData);
					}
				}
				else
				{
					this.sweepClient.ClientData.WanMoTaProp.nSweepLayer = this.nSweepingOrder;
					WanMoTaDBCommandManager.UpdateSweepAwardDBCommand(this.sweepClient, this.sweepClient.ClientData.WanMoTaProp.nSweepLayer);
				}
			}
		}

		// Token: 0x04003F61 RID: 16225
		private GameClient sweepClient = null;

		// Token: 0x04003F62 RID: 16226
		public int nSweepingOrder;

		// Token: 0x04003F63 RID: 16227
		public int nSweepingMaxOrder;

		// Token: 0x04003F64 RID: 16228
		private Timer _WanMoTaSweepingTimer = null;
	}
}
