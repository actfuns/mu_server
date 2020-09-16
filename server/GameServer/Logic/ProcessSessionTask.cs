using System;
using System.Collections.Concurrent;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class ProcessSessionTask : ScheduleTask
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public ProcessSessionTask(TCPSession session)
		{
			this.beginTime = TimeUtil.NowEx();
			this.session = session;
		}

		
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

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		public static long processCmdNum = 0L;

		
		public static long processTotalTime = 0L;

		
		public static ConcurrentDictionary<int, PorcessCmdMoniter> cmdMoniter = new ConcurrentDictionary<int, PorcessCmdMoniter>();

		
		public static DateTime StartTime = TimeUtil.NowDateTime();

		
		private TCPSession session = null;

		
		private long beginTime = 0L;
	}
}
