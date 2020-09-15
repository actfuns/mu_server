using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;
using Tmsk.Tools;

namespace GameServer.Logic
{
	// Token: 0x020008F2 RID: 2290
	public class ServerMonitorManager
	{
		// Token: 0x06004214 RID: 16916 RVA: 0x003C6148 File Offset: 0x003C4348
		public ServerMonitorManager()
		{
			this._PrevCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
			this._LastCalcCpuMs = TimeUtil.NOW();
			this._LastReportMs = TimeUtil.NOW();
		}

		// Token: 0x06004215 RID: 16917 RVA: 0x003C61B0 File Offset: 0x003C43B0
		private bool GetCpuAndMem(out double cpuLoad, out double memMb)
		{
			cpuLoad = 0.0;
			memMb = 0.0;
			try
			{
				long nowMs = TimeUtil.NOW();
				double intervalMs = (double)(nowMs - this._LastCalcCpuMs);
				TimeSpan curCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
				if (intervalMs > 0.0)
				{
					cpuLoad = (curCpuTime - this._PrevCpuTime).TotalMilliseconds * 1.0 / intervalMs / (double)Environment.ProcessorCount;
					cpuLoad = Math.Min(cpuLoad, 1.0);
				}
				memMb = (double)Process.GetCurrentProcess().WorkingSet64 / 1048576.0;
				this._LastCalcCpuMs = nowMs;
				this._PrevCpuTime = curCpuTime;
			}
			catch (Exception)
			{
				cpuLoad = 0.0;
				memMb = 0.0;
				return false;
			}
			return true;
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x003C62A0 File Offset: 0x003C44A0
		private void RefreshReportConfig()
		{
			if (this._BNeedReLoad)
			{
				this._BNeedReLoad = false;
				string report = GameManager.GameConfigMgr.GetGameConfigItemStr("server_monitor_report", "");
				string[] fields = report.Split(new char[]
				{
					','
				});
				string tmpReportUrl = string.Empty;
				int tmpReportInterval = 3;
				if (fields.Length >= 1)
				{
					tmpReportUrl = fields[0];
				}
				if (fields.Length >= 2)
				{
					if (!int.TryParse(fields[1], out tmpReportInterval))
					{
						tmpReportInterval = 5;
					}
				}
				tmpReportInterval = Math.Max(3, tmpReportInterval);
				this._ReportToUrl = tmpReportUrl;
				this._ReportIntervalSec = tmpReportInterval;
			}
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x003C6347 File Offset: 0x003C4547
		public void SetNeedReload()
		{
			this._BNeedReLoad = true;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x003C6384 File Offset: 0x003C4584
		public void CheckReport()
		{
			try
			{
				this.RefreshReportConfig();
				if (!string.IsNullOrEmpty(this._ReportToUrl))
				{
					long nowMs = TimeUtil.NOW();
					if (nowMs - this._LastReportMs >= (long)(this._ReportIntervalSec * 1000))
					{
						if (!this._BIsReporting)
						{
							this._BIsReporting = true;
							this._LastReportMs = nowMs;
							StringBuilder sb = new StringBuilder();
							sb.Append(this._ReportToUrl).Append("?");
							sb.AppendFormat("serverid={0}&", GameCoreInterface.getinstance().GetLocalServerId());
							sb.AppendFormat("platform={0}&", GameCoreInterface.getinstance().GetPlatformType().ToString());
							double cpuLoad;
							double memMb;
							this.GetCpuAndMem(out cpuLoad, out memMb);
							sb.AppendFormat("cpu={0}&", cpuLoad);
							sb.AppendFormat("mem={0}&", memMb);
							sb.AppendFormat("roleCount={0}&", GameManager.ClientMgr.GetClientCount());
							sb.AppendFormat("procCmdCount={0}&", TCPCmdHandler.TotalHandledCmdsNum);
							sb.AppendFormat("cmdAvgProcMs={0}&", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0);
							sb.AppendFormat("cmdMaxProcMs={0}&", TCPCmdHandler.MaxUsedTicksByCmdID);
							sb.AppendFormat("dbConnCount={0}&", Global._TCPManager.tcpClientPool.GetPoolCount());
							sb.AppendFormat("lastFlushMonsterToNow={0}", GameManager.LastFlushMonsterMs * 10000L);
							new Task(delegate()
							{
								WebHelper.RequestByGet(sb.ToString(), 2000, 30000);
								this._BIsReporting = false;
							}).Start();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "ServerAnalysisManager.CheckReport() failed!", ex, true);
			}
		}

		// Token: 0x0400500B RID: 20491
		public const string ReportUrlCfgKey = "server_monitor_report";

		// Token: 0x0400500C RID: 20492
		private TimeSpan _PrevCpuTime = TimeSpan.Zero;

		// Token: 0x0400500D RID: 20493
		private long _LastCalcCpuMs;

		// Token: 0x0400500E RID: 20494
		private long _LastReportMs;

		// Token: 0x0400500F RID: 20495
		private bool _BIsReporting = false;

		// Token: 0x04005010 RID: 20496
		private bool _BNeedReLoad = true;

		// Token: 0x04005011 RID: 20497
		private string _ReportToUrl = string.Empty;

		// Token: 0x04005012 RID: 20498
		private int _ReportIntervalSec = 5;
	}
}
