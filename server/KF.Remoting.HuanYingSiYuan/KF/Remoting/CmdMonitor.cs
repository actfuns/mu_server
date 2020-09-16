using System;
using System.Collections.Generic;
using System.Linq;
using AutoCSer;
using GameServer.Core.Executor;
using KF.Remoting.HuanYingSiYuan.TcpStaticServer;
using Server.Tools;

namespace KF.Remoting
{
	
	public static class CmdMonitor
	{
		
		static CmdMonitor()
		{
			CmdMonitor.Reset();
		}

		
		public static void Reset()
		{
			CmdMonitor.StartTime = TimeUtil.NowDateTime();
			for (int i = 0; i < CmdMonitor.cmdMoniter.Length; i++)
			{
				CmdMonitor.cmdMoniter[i] = new PorcessCmdMoniter(i, 0L);
			}
		}

		
		public static string GetCmdName(PorcessCmdMoniter m)
		{
			string result;
			if (m.cmd >= 800 && !string.IsNullOrEmpty(m.cmdName))
			{
				result = m.cmdName.Replace("/KuaFuService/", "").Replace(".soap", "/");
			}
			else
			{
				if (CmdMonitor.array != null && m.cmd >= 0 && m.cmd < CmdMonitor.array.Length)
				{
					string key = CmdMonitor.array[m.cmd].Key;
					int idx0 = key.IndexOf('(');
					int idx = key.LastIndexOf(')');
					if (idx0 > 0 && idx0 < idx && idx < key.Length - 1)
					{
						string name = key.Remove(idx0, idx - idx0 + 1);
						int idx2 = name.LastIndexOf('.');
						if (idx2 > 0)
						{
							m.cmdName = name.Insert(idx0, ".").Substring(idx2 + 1);
						}
					}
				}
				if (string.IsNullOrEmpty(m.cmdName))
				{
					m.cmdName = m.cmd.ToString();
				}
				result = m.cmdName;
			}
			return result;
		}

		
		public static void RecordCmdDetail(string cmdName, long processTime, long dataSize, TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_OK)
		{
			int cmdId;
			lock (CmdMonitor.mutex)
			{
				if (!CmdMonitor.cmdMapper.TryGetValue(cmdName, out cmdId))
				{
					cmdId = CmdMonitor.cmdMapper.Count + 800;
					CmdMonitor.cmdMapper[cmdName] = cmdId;
				}
			}
			CmdMonitor.cmdMoniter[cmdId].cmdName = cmdName;
			CmdMonitor.cmdMoniter[cmdId].onProcessNoWait(processTime, dataSize, result);
		}

		
		public static void RecordCmdDetail(string cmdName, long processTime, long dataSize, long outSize, TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_OK)
		{
			int cmdId;
			lock (CmdMonitor.mutex)
			{
				if (!CmdMonitor.cmdMapper.TryGetValue(cmdName, out cmdId))
				{
					cmdId = CmdMonitor.cmdMapper.Count + 800;
					CmdMonitor.cmdMapper[cmdName] = cmdId;
				}
			}
			CmdMonitor.cmdMoniter[cmdId].cmdName = cmdName;
			CmdMonitor.cmdMoniter[cmdId].onProcessNoWait(processTime, dataSize, result);
			CmdMonitor.cmdMoniter[cmdId].OnOutputData(outSize);
		}

		
		public static void RecordCmdDetail(int cmdId, long processTime, long dataSize, TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_OK)
		{
			CmdMonitor.cmdMoniter[cmdId].onProcessNoWait(processTime, dataSize, result);
		}

		
		public static void RecordCmdDetail2(int cmdId, long processTime, long waitTime)
		{
			CmdMonitor.cmdMoniter[cmdId].onProcess(processTime, waitTime);
		}

		
		public static void RecordCmdOutputDataSize(int cmdId, long dataSize)
		{
			CmdMonitor.cmdMoniter[cmdId].OnOutputData(dataSize);
		}

		
		public static void ShowServerTCPInfo(object obj)
		{
			try
			{
				string[] cmd = obj as string[];
				bool clear = cmd.Contains("/c");
				bool detail = cmd.Contains("/d");
				DateTime now = TimeUtil.NowDateTime();
				SysConOut.WriteLine(string.Format("当前时间:{0},统计时长:{1}", now.ToString("yyyy-MM-dd HH:mm:ss"), (now - CmdMonitor.StartTime).ToString()));
				if (clear)
				{
					detail = true;
					CmdMonitor.StartTime = now;
				}
				SysConOut.WriteLine(string.Format("总处理指令个数 {0}", CmdMonitor.TotalHandledCmdsNum));
				SysConOut.WriteLine(string.Format("指令处理耗时详情", new object[0]));
				try
				{
					if (detail)
					{
						if (Console.WindowWidth < 160)
						{
							Console.WindowWidth = 160;
						}
					}
					else if (Console.WindowWidth >= 88)
					{
						Console.WindowWidth = 88;
					}
				}
				catch
				{
				}
				int count = 0;
				lock (CmdMonitor.mutex)
				{
					foreach (PorcessCmdMoniter i in CmdMonitor.cmdMoniter)
					{
						if (i.processNum != 0)
						{
							Console.ForegroundColor = count % 5 + ConsoleColor.Green;
							if (detail)
							{
								if (count++ == 0)
								{
									SysConOut.WriteLine(string.Format("{0, -48}{1, 6}{2, 7}{3, 7} {4, 7} {5, 4} {6, 4} {7, 5}", new object[]
									{
										"消息",
										"已处理次数",
										"平均处理时长",
										"总计消耗时长",
										"总计字节数",
										"发送次数",
										"发送字节数",
										"失败/成功/数据"
									}));
								}
								string info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##} {4, 13:0.##} {5, 8} {6, 12} {7, 4}/{8}/{9}", new object[]
								{
									CmdMonitor.GetCmdName(i),
									i.processNum,
									TimeUtil.TimeMS(i.avgProcessTime(), 2),
									TimeUtil.TimeMS(i.processTotalTime, 2),
									i.GetTotalBytes(),
									i.SendNum,
									i.OutPutBytes,
									i.Num_Faild,
									i.Num_OK,
									i.Num_WithData
								});
								SysConOut.WriteLine(info);
							}
							else
							{
								if (count++ == 0)
								{
									SysConOut.WriteLine(string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", new object[]
									{
										"消息",
										"已处理次数",
										"平均处理时长",
										"总计消耗时长"
									}));
								}
								string info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", new object[]
								{
									CmdMonitor.GetCmdName(i),
									i.processNum,
									TimeUtil.TimeMS(i.avgProcessTime(), 2),
									TimeUtil.TimeMS(i.processTotalTime, 2)
								});
								SysConOut.WriteLine(info);
							}
							if (clear)
							{
								i.Reset();
							}
						}
					}
					Console.ForegroundColor = ConsoleColor.White;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		private static object mutex = new object();

		
		public static long TotalHandledCmdsNum;

		
		public static long processCmdNum = 0L;

		
		public static long processTotalTime = 0L;

		
		public static PorcessCmdMoniter[] cmdMoniter = new PorcessCmdMoniter[1024];

		
		public static DateTime StartTime;

		
		private static Dictionary<string, int> cmdMapper = new Dictionary<string, int>();

		
		private static KeyValue<string, int>[] array = KfCall._identityCommandNames_();
	}
}
