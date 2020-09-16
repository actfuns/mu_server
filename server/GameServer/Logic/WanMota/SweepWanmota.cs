using System;
using System.Collections.Generic;
using System.Timers;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.WanMota
{
	
	public class SweepWanmota
	{
		
		
		
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

		
		public SweepWanmota(GameClient client)
		{
			this.sweepClient = client;
		}

		
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

		
		private GameClient sweepClient = null;

		
		public int nSweepingOrder;

		
		public int nSweepingMaxOrder;

		
		private Timer _WanMoTaSweepingTimer = null;
	}
}
