using System;
using System.Collections.Concurrent;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200077C RID: 1916
	public class ProcessSessionTask : ScheduleTask
	{
		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06003122 RID: 12578 RVA: 0x002B8CCC File Offset: 0x002B6ECC
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x002B8CE4 File Offset: 0x002B6EE4
		public ProcessSessionTask(TCPSession session)
		{
			this.beginTime = TimeUtil.NowEx();
			this.session = session;
		}

		// Token: 0x06003124 RID: 12580 RVA: 0x002B8D1C File Offset: 0x002B6F1C
		public void run()
		{
			int cmdID = 0;
			long processTime = 0L;
			long processWaitTime = 0L;
			if (Monitor.TryEnter(this.session.Lock))
			{
				try
				{
					long processBeginTime = TimeUtil.NowEx();
					processWaitTime = processBeginTime - this.beginTime;
					TCPCmdWrapper wrapper = this.session.getNextTCPCmdWrapper();
					if (null != wrapper)
					{
						try
						{
							TCPCmdHandler.ProcessCmd(wrapper.TcpMgr, wrapper.TMSKSocket, wrapper.TcpClientPool, wrapper.TcpRandKey, wrapper.Pool, wrapper.NID, wrapper.Data, wrapper.Count);
						}
						catch (Exception ex)
						{
							DataHelper.WriteFormatExceptionLog(ex, string.Format("指令处理错误：{0},{1}", Global.GetDebugHelperInfo(wrapper.TMSKSocket), (TCPGameServerCmds)wrapper.NID), false, false);
						}
						ProcessSessionTask.processCmdNum += 1L;
						processTime = TimeUtil.NowEx() - processBeginTime;
						ProcessSessionTask.processTotalTime += processWaitTime + processTime;
						cmdID = wrapper.NID;
						wrapper.release();
						wrapper = null;
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					Monitor.Exit(this.session.Lock);
				}
				if (cmdID > 0)
				{
					TCPManager.RecordCmdDetail2(cmdID, processTime, processWaitTime);
				}
			}
			else
			{
				TCPManager.getInstance().taskExecutor.scheduleExecute(this, 5L);
			}
		}

		// Token: 0x04003D9C RID: 15772
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x04003D9D RID: 15773
		public static long processCmdNum = 0L;

		// Token: 0x04003D9E RID: 15774
		public static long processTotalTime = 0L;

		// Token: 0x04003D9F RID: 15775
		public static ConcurrentDictionary<int, PorcessCmdMoniter> cmdMoniter = new ConcurrentDictionary<int, PorcessCmdMoniter>();

		// Token: 0x04003DA0 RID: 15776
		public static DateTime StartTime = TimeUtil.NowDateTime();

		// Token: 0x04003DA1 RID: 15777
		private TCPSession session = null;

		// Token: 0x04003DA2 RID: 15778
		private long beginTime = 0L;
	}
}
