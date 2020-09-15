using System;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using GameServer.Server;
using KF.Remoting;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	// Token: 0x020008F9 RID: 2297
	public class TestReceiveModel : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x0600427D RID: 17021 RVA: 0x003C9874 File Offset: 0x003C7A74
		public static TestReceiveModel getInstance()
		{
			return TestReceiveModel.instance;
		}

		// Token: 0x0600427E RID: 17022 RVA: 0x003C988C File Offset: 0x003C7A8C
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600427F RID: 17023 RVA: 0x003C98A0 File Offset: 0x003C7AA0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06004280 RID: 17024 RVA: 0x003C98B4 File Offset: 0x003C7AB4
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06004281 RID: 17025 RVA: 0x003C98C8 File Offset: 0x003C7AC8
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			return true;
		}

		// Token: 0x06004282 RID: 17026 RVA: 0x003C98DC File Offset: 0x003C7ADC
		public bool startup()
		{
			this.NotifyEnterHandler = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 0,
				EventType = 10041,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10041, this.NotifyEnterHandler);
			return true;
		}

		// Token: 0x06004283 RID: 17027 RVA: 0x003C9938 File Offset: 0x003C7B38
		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			try
			{
				int kuaFuEventType = msg.KuaFuEventType;
				if (kuaFuEventType == 10041)
				{
					if (!this.flag)
					{
						this.receiveStopNum++;
						TcpCall.TestS2KFCommunication.SendData(1, false);
						if (this.receiveStopNum % 100 == 0)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							SysConOut.WriteLine(string.Format("结束服务器接收异步消息 {0}", this.receiveStopNum));
							Console.ForegroundColor = ConsoleColor.White;
						}
						return true;
					}
					string data = msg.Get<string>();
					if (null != data)
					{
						this.receiveSuNum++;
					}
					else
					{
						this.receiveErrNum++;
					}
					if (this.receiveErrNum + this.receiveSuNum == 50000)
					{
						this.endTime = DateTime.Now;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]跨服消息{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x003C9A64 File Offset: 0x003C7C64
		public bool showdown()
		{
			KFCallManager.MsgSource.removeListener(10041, this.NotifyEnterHandler);
			return true;
		}

		// Token: 0x06004285 RID: 17029 RVA: 0x003C9A8D File Offset: 0x003C7C8D
		public void start()
		{
			this.receiveSuNum = 0;
			this.receiveErrNum = 0;
			this.receiveStopNum = 0;
			this.startTime = DateTime.Now;
			this.endTime = this.startTime;
			this.flag = true;
		}

		// Token: 0x06004286 RID: 17030 RVA: 0x003C9AC4 File Offset: 0x003C7CC4
		public void stop()
		{
			if (this.endTime == this.startTime)
			{
				this.endTime = DateTime.Now;
			}
			this.flag = false;
		}

		// Token: 0x06004287 RID: 17031 RVA: 0x003C9B00 File Offset: 0x003C7D00
		public void print()
		{
			string str = string.Format("接收统计,接收失败={1},成功={2}, Time={5},平均={6}", new object[]
			{
				0,
				this.receiveErrNum,
				this.receiveSuNum,
				0,
				0,
				(this.endTime - this.startTime).TotalMilliseconds,
				Math.Truncate(100.0 * (this.endTime - this.startTime).TotalMilliseconds / (double)(this.receiveErrNum + this.receiveSuNum)) / 100.0
			});
			Console.ForegroundColor = ConsoleColor.Red;
			SysConOut.WriteLine(str);
			Console.ForegroundColor = ConsoleColor.White;
		}

		// Token: 0x04005034 RID: 20532
		private static TestReceiveModel instance = new TestReceiveModel();

		// Token: 0x04005035 RID: 20533
		public DateTime endTime;

		// Token: 0x04005036 RID: 20534
		public DateTime startTime;

		// Token: 0x04005037 RID: 20535
		private bool flag = false;

		// Token: 0x04005038 RID: 20536
		public int receiveSuNum = 0;

		// Token: 0x04005039 RID: 20537
		public int receiveErrNum = 0;

		// Token: 0x0400503A RID: 20538
		public int receiveStopNum = 0;

		// Token: 0x0400503B RID: 20539
		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;
	}
}
