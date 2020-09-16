using System;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using GameServer.Server;
using KF.Remoting;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	
	public class TestReceiveModel : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static TestReceiveModel getInstance()
		{
			return TestReceiveModel.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			return true;
		}

		
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

		
		public bool showdown()
		{
			KFCallManager.MsgSource.removeListener(10041, this.NotifyEnterHandler);
			return true;
		}

		
		public void start()
		{
			this.receiveSuNum = 0;
			this.receiveErrNum = 0;
			this.receiveStopNum = 0;
			this.startTime = DateTime.Now;
			this.endTime = this.startTime;
			this.flag = true;
		}

		
		public void stop()
		{
			if (this.endTime == this.startTime)
			{
				this.endTime = DateTime.Now;
			}
			this.flag = false;
		}

		
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

		
		private static TestReceiveModel instance = new TestReceiveModel();

		
		public DateTime endTime;

		
		public DateTime startTime;

		
		private bool flag = false;

		
		public int receiveSuNum = 0;

		
		public int receiveErrNum = 0;

		
		public int receiveStopNum = 0;

		
		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;
	}
}
