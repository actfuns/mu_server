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
	
	public class ServerMonitorManager
	{
		
		public ServerMonitorManager()
		{
			this._PrevCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
			this._LastCalcCpuMs = TimeUtil.NOW();
			this._LastReportMs = TimeUtil.NOW();
		}

		
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

		
		public void SetNeedReload()
		{
			this._BNeedReLoad = true;
		}

		
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

		
		public const string ReportUrlCfgKey = "server_monitor_report";

		
		private TimeSpan _PrevCpuTime = TimeSpan.Zero;

		
		private long _LastCalcCpuMs;

		
		private long _LastReportMs;

		
		private bool _BIsReporting = false;

		
		private bool _BNeedReLoad = true;

		
		private string _ReportToUrl = string.Empty;

		
		private int _ReportIntervalSec = 5;
	}
}
