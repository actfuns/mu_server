using System;
using System.Collections.Generic;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	
	public class S2KFCommunication
	{
		
		public static void start(string cmd = null)
		{
			try
			{
				if (S2KFCommunication.stage)
				{
					Console.WriteLine("已经开启过了");
				}
				S2KFCommunication.endNum = 0;
				Console.WriteLine("输入1.开启线程数 2,中心返回消息长度, 3.时间间隔毫秒(0 运行5s自动结束)  例子：/“5,1024,10/”");
				string[] files = Console.ReadLine().Split(new char[]
				{
					','
				});
				int thread = Convert.ToInt32(files[0].Trim());
				int uptime = Convert.ToInt32(files[2].Trim());
				ThreadTimerModel.MsgLen = Convert.ToInt32(files[1].Trim());
				S2KFCommunication.objList = new List<ThreadTimerModel>(thread);
				for (int i = 0; i < thread; i++)
				{
					S2KFCommunication.objList.Add(new ThreadTimerModel(i));
				}
				int runTime = 0;
				if (uptime < 1)
				{
					Console.WriteLine("输入运行秒数");
					runTime = Convert.ToInt32(Console.ReadLine());
				}
				foreach (ThreadTimerModel item in S2KFCommunication.objList)
				{
					item.Start(uptime, runTime);
				}
				TestReceiveModel.getInstance().start();
				S2KFCommunication.stage = true;
				Console.WriteLine("压测开启");
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("开启失败 Exception");
			}
		}

		
		public static void SetEnd()
		{
			S2KFCommunication.endNum++;
			if (S2KFCommunication.endNum == S2KFCommunication.objList.Count)
			{
				S2KFCommunication.stop(null);
			}
		}

		
		public static void stop(string cmd = null)
		{
			try
			{
				foreach (ThreadTimerModel item in S2KFCommunication.objList)
				{
					item.Stop();
				}
				TestReceiveModel.getInstance().stop();
				while (!TcpCall.TestS2KFCommunication.SendData(1, false).IsReturn)
				{
				}
				S2KFCommunication.stage = false;
				Console.WriteLine("压测关闭");
				double time = 0.0;
				int receiveNum = 0;
				int sendSuNum = 0;
				int sendErrNum = 0;
				foreach (ThreadTimerModel item in S2KFCommunication.objList)
				{
					item.print();
					sendSuNum += item.sendSuNum;
					sendErrNum += item.sendErrNum;
					time += (item.endTime - item.startTime).TotalMilliseconds;
				}
				string str = string.Format("统计,发送失败={1},发送成功={2}, Time={4},平均={5}", new object[]
				{
					0,
					sendErrNum,
					sendSuNum,
					receiveNum,
					time / (double)S2KFCommunication.objList.Count,
					time / (double)S2KFCommunication.objList.Count / (double)(sendSuNum + sendErrNum)
				});
				Console.ForegroundColor = ConsoleColor.Red;
				SysConOut.WriteLine(str);
				Console.ForegroundColor = ConsoleColor.White;
				TestReceiveModel.getInstance().print();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("关闭失败 Exception");
			}
		}

		
		private static int endNum = 0;

		
		private static bool stage = false;

		
		private static List<ThreadTimerModel> objList;
	}
}
