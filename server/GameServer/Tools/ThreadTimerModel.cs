using System;
using System.Threading;
using AutoCSer.Net.TcpServer;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	// Token: 0x020008F7 RID: 2295
	public class ThreadTimerModel
	{
		// Token: 0x06004272 RID: 17010 RVA: 0x003C915C File Offset: 0x003C735C
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

		// Token: 0x06004273 RID: 17011 RVA: 0x003C922A File Offset: 0x003C742A
		public ThreadTimerModel(int ID)
		{
			this.threadID = ID;
		}

		// Token: 0x06004274 RID: 17012 RVA: 0x003C9258 File Offset: 0x003C7458
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

		// Token: 0x06004275 RID: 17013 RVA: 0x003C92D4 File Offset: 0x003C74D4
		public void Stop()
		{
			this.endTime = DateTime.Now;
			this.upDateTimer.Change(-1, -1);
		}

		// Token: 0x06004276 RID: 17014 RVA: 0x003C92F4 File Offset: 0x003C74F4
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

		// Token: 0x06004277 RID: 17015 RVA: 0x003C9374 File Offset: 0x003C7574
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

		// Token: 0x04005029 RID: 20521
		public static int MsgLen;

		// Token: 0x0400502A RID: 20522
		private int threadID;

		// Token: 0x0400502B RID: 20523
		public DateTime endTime;

		// Token: 0x0400502C RID: 20524
		public DateTime startTime;

		// Token: 0x0400502D RID: 20525
		public Timer upDateTimer = null;

		// Token: 0x0400502E RID: 20526
		public int sendSuNum = 0;

		// Token: 0x0400502F RID: 20527
		public int sendErrNum = 0;

		// Token: 0x04005030 RID: 20528
		private int RunTime = 0;
	}
}
