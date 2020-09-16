using System;
using System.Threading;
using AutoCSer.Net.TcpServer;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	
	public class ThreadTimerModel
	{
		
		public void print()
		{
			string str = string.Format("ID={0},发送失败={1},成功={2}, Time={5},平均={6}", new object[]
			{
				this.threadID,
				this.sendErrNum,
				this.sendSuNum,
				0,
				0,
				(this.endTime - this.startTime).TotalMilliseconds,
				Math.Truncate(100.0 * (this.endTime - this.startTime).TotalMilliseconds / (double)(this.sendSuNum + this.sendErrNum)) / 100.0
			});
			Console.WriteLine(str);
		}

		
		public ThreadTimerModel(int ID)
		{
			this.threadID = ID;
		}

		
		public void Start(int upTime, int runTime = 0)
		{
			this.startTime = DateTime.Now.AddMilliseconds(-1.0);
			if (upTime == 0)
			{
				this.RunTime = runTime;
				this.upDateTimer = new Timer(new TimerCallback(ThreadTimerModel.ZeroUpDateTick), this, 1, 3600000);
			}
			else
			{
				this.upDateTimer = new Timer(new TimerCallback(ThreadTimerModel.UpDateTick), this, 1, upTime);
			}
		}

		
		public void Stop()
		{
			this.endTime = DateTime.Now;
			this.upDateTimer.Change(-1, -1);
		}

		
		private static void UpDateTick(object sender)
		{
			try
			{
				ReturnValue<string> msgData = TcpCall.TestS2KFCommunication.SendData(ThreadTimerModel.MsgLen, true);
				ThreadTimerModel model = sender as ThreadTimerModel;
				if (!msgData.IsReturn)
				{
					model.sendErrNum++;
				}
				else
				{
					model.sendSuNum++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		
		private static void ZeroUpDateTick(object sender)
		{
			try
			{
				ThreadTimerModel model = sender as ThreadTimerModel;
				model.startTime = DateTime.Now;
				while ((model.endTime - model.startTime).TotalMilliseconds < (double)(1000 * model.RunTime))
				{
					model.endTime = DateTime.Now;
					try
					{
						if (!TcpCall.TestS2KFCommunication.SendData(ThreadTimerModel.MsgLen, true).IsReturn)
						{
							model.sendErrNum++;
						}
						else
						{
							model.sendSuNum++;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						Console.WriteLine("UpDateTick Exception");
					}
				}
				S2KFCommunication.SetEnd();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		
		public static int MsgLen;

		
		private int threadID;

		
		public DateTime endTime;

		
		public DateTime startTime;

		
		public Timer upDateTimer = null;

		
		public int sendSuNum = 0;

		
		public int sendErrNum = 0;

		
		private int RunTime = 0;
	}
}
