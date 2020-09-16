using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	
	public class TCPSession
	{
		
		public TCPSession(TMSKSocket socket)
		{
			long[] array = new long[6];
			array[0] = TimeUtil.NOW();
			this._socketTime = array;
			this._socketState = 0;
			this._cmdID = 0;
			this._cmdCount = 0;
			this._cmdTime = 0L;
			this._timeOutCount = 0;
			this._decryptCount = 0;
            this._currentSocket = socket;
		}

		
		public void release()
		{
			this._currentSocket = null;
			lock (this._cmdWrapperQueue)
			{
				if (null != this._cmdWrapperQueue)
				{
					this._cmdWrapperQueue.Clear();
				}
			}
		}

		
		
		public TMSKSocket CurrentSocket
		{
			get
			{
				return this._currentSocket;
			}
		}

		
		
		public object Lock
		{
			get
			{
				return this._lock;
			}
		}

		
		public static void SetMaxPosCmdNumPer5Seconds(int defVal)
		{
			TCPSession.MaxPosCmdNumPer5Seconds = GameManager.PlatConfigMgr.GetGameConfigItemInt("maxposcmdnum", defVal);
		}

		
		
		
		public long[] SocketTime
		{
			get
			{
				return this._socketTime;
			}
			set
			{
				this._socketTime = value;
			}
		}

		
		public void SetSocketTime(int index)
		{
			this._socketTime[index] = TimeUtil.NOW();
			this._socketState = index;
		}

		
		
		
		public int SocketState
		{
			get
			{
				return this._socketState;
			}
			set
			{
				this._socketState = value;
			}
		}

		
		
		
		public int CmdID
		{
			get
			{
				return this._cmdID;
			}
			set
			{
				this._cmdID = value;
				this._cmdCount++;
			}
		}

		
		
		
		public long CmdTime
		{
			get
			{
				return this._cmdTime;
			}
			set
			{
				this._cmdTime = value;
			}
		}

		
		
		public int CmdCount
		{
			get
			{
				return this._cmdCount;
			}
		}

		
		
		
		public int TimeOutCount
		{
			get
			{
				return this._timeOutCount;
			}
			set
			{
				this._timeOutCount = value;
			}
		}

		
		public void TimeOutCountAdd()
		{
			this._timeOutCount++;
		}

		
		
		
		public int DecryptCount
		{
			get
			{
				return this._decryptCount;
			}
			set
			{
				this._decryptCount = value;
			}
		}

		
		public void DecryptCountAdd()
		{
			this._decryptCount++;
		}

		
		public void addTCPCmdWrapper(TCPCmdWrapper wrapper, out int posCmdNum)
		{
			posCmdNum = 0;
			lock (this._cmdWrapperQueue)
			{
				this._cmdWrapperQueue.Enqueue(wrapper);
			}
			if (611 == wrapper.NID)
			{
				this.cmdNum++;
				if (TimeUtil.NOW() - this.beginTime >= 10000L)
				{
					if (this.cmdNum >= TCPSession.MaxPosCmdNumPer5Seconds)
					{
						posCmdNum = this.cmdNum;
						this.cmdNum = 0;
						this.beginTime = TimeUtil.NOW();
					}
					else
					{
						this.cmdNum = 0;
						this.beginTime = TimeUtil.NOW();
					}
				}
			}
		}

		
		public void CheckCmdNum(int cmdID, long clientTime, out int posCmdNum)
		{
			posCmdNum = 0;
			if (611 == cmdID)
			{
				this.cmdNum++;
				long nowTicks = TimeUtil.NOW();
				this.CheckCmdTimeQueue.Enqueue(new Tuple<long, long>(nowTicks, clientTime));
				if (nowTicks - this.beginTime >= 10000L)
				{
					if (this.cmdNum >= 8)
					{
						try
						{
							long st = 0L;
							long sc = 0L;
							string msg = string.Format("检测到客户端加速,num={0},userid={1},times=", this.cmdNum, this._currentSocket.UserID);
							foreach (Tuple<long, long> t in this.CheckCmdTimeQueue)
							{
								msg = string.Concat(new object[]
								{
									msg,
									t.Item1 - st,
									',',
									t.Item2 - sc,
									'|'
								});
								st = t.Item1;
								sc = t.Item2;
							}
							LogManager.WriteLog(LogTypes.Error, msg, null, true);
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
					if (this.cmdNum >= TCPSession.MaxPosCmdNumPer5Seconds)
					{
						posCmdNum = this.cmdNum;
					}
					this.cmdNum = 0;
					this.beginTime = nowTicks;
					this.CheckCmdTimeQueue.Clear();
				}
			}
		}

		
		public TCPCmdWrapper getNextTCPCmdWrapper()
		{
			lock (this._cmdWrapperQueue)
			{
				if (this._cmdWrapperQueue.Count > 0)
				{
					return this._cmdWrapperQueue.Dequeue();
				}
			}
			return null;
		}

		
		private TMSKSocket _currentSocket = null;

		
		private Queue<TCPCmdWrapper> _cmdWrapperQueue = new Queue<TCPCmdWrapper>();

		
		private object _lock = new object();

		
		public static int MaxPosCmdNumPer5Seconds = GameManager.PlatConfigMgr.GetGameConfigItemInt("maxposcmdnum", 8);

		
		public static int MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 500);

		
		public static int MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);

		
		public bool IsGM = false;

		
		public int IsGuest;

		
		public int gmPriority;

		
		public long LastLogoutServerTicks;

		
		private int cmdNum = 0;

		
		private long beginTime = TimeUtil.NOW();

		
		private Queue<Tuple<long, long>> CheckCmdTimeQueue = new Queue<Tuple<long, long>>();

		
		private long[] _socketTime;

		
		private int _socketState;

		
		private int _cmdID;

		
		private int _cmdCount;

		
		private long _cmdTime;

		
		private int _timeOutCount;

		
		private int _decryptCount;

		
		public bool? InIpWhiteList;

		
		public bool? InUseridWhiteList;

		
		public int IsAdult;
	}
}
