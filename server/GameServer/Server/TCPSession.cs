using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	// Token: 0x020008C4 RID: 2244
	public class TCPSession
	{
		// Token: 0x06003FE9 RID: 16361 RVA: 0x003B9DAC File Offset: 0x003B7FAC
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
			base..ctor();
			this._currentSocket = socket;
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x003B9E4C File Offset: 0x003B804C
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

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06003FEB RID: 16363 RVA: 0x003B9EB4 File Offset: 0x003B80B4
		public TMSKSocket CurrentSocket
		{
			get
			{
				return this._currentSocket;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06003FEC RID: 16364 RVA: 0x003B9ECC File Offset: 0x003B80CC
		public object Lock
		{
			get
			{
				return this._lock;
			}
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x003B9EE4 File Offset: 0x003B80E4
		public static void SetMaxPosCmdNumPer5Seconds(int defVal)
		{
			TCPSession.MaxPosCmdNumPer5Seconds = GameManager.PlatConfigMgr.GetGameConfigItemInt("maxposcmdnum", defVal);
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06003FEE RID: 16366 RVA: 0x003B9EFC File Offset: 0x003B80FC
		// (set) Token: 0x06003FEF RID: 16367 RVA: 0x003B9F14 File Offset: 0x003B8114
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

		// Token: 0x06003FF0 RID: 16368 RVA: 0x003B9F1E File Offset: 0x003B811E
		public void SetSocketTime(int index)
		{
			this._socketTime[index] = TimeUtil.NOW();
			this._socketState = index;
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06003FF1 RID: 16369 RVA: 0x003B9F38 File Offset: 0x003B8138
		// (set) Token: 0x06003FF2 RID: 16370 RVA: 0x003B9F50 File Offset: 0x003B8150
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

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06003FF3 RID: 16371 RVA: 0x003B9F5C File Offset: 0x003B815C
		// (set) Token: 0x06003FF4 RID: 16372 RVA: 0x003B9F74 File Offset: 0x003B8174
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

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06003FF5 RID: 16373 RVA: 0x003B9F8C File Offset: 0x003B818C
		// (set) Token: 0x06003FF6 RID: 16374 RVA: 0x003B9FA4 File Offset: 0x003B81A4
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

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06003FF7 RID: 16375 RVA: 0x003B9FB0 File Offset: 0x003B81B0
		public int CmdCount
		{
			get
			{
				return this._cmdCount;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06003FF8 RID: 16376 RVA: 0x003B9FC8 File Offset: 0x003B81C8
		// (set) Token: 0x06003FF9 RID: 16377 RVA: 0x003B9FE0 File Offset: 0x003B81E0
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

		// Token: 0x06003FFA RID: 16378 RVA: 0x003B9FEA File Offset: 0x003B81EA
		public void TimeOutCountAdd()
		{
			this._timeOutCount++;
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06003FFB RID: 16379 RVA: 0x003B9FFC File Offset: 0x003B81FC
		// (set) Token: 0x06003FFC RID: 16380 RVA: 0x003BA014 File Offset: 0x003B8214
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

		// Token: 0x06003FFD RID: 16381 RVA: 0x003BA01E File Offset: 0x003B821E
		public void DecryptCountAdd()
		{
			this._decryptCount++;
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x003BA030 File Offset: 0x003B8230
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

		// Token: 0x06003FFF RID: 16383 RVA: 0x003BA108 File Offset: 0x003B8308
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

		// Token: 0x06004000 RID: 16384 RVA: 0x003BA2BC File Offset: 0x003B84BC
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

		// Token: 0x04004EFB RID: 20219
		private TMSKSocket _currentSocket = null;

		// Token: 0x04004EFC RID: 20220
		private Queue<TCPCmdWrapper> _cmdWrapperQueue = new Queue<TCPCmdWrapper>();

		// Token: 0x04004EFD RID: 20221
		private object _lock = new object();

		// Token: 0x04004EFE RID: 20222
		public static int MaxPosCmdNumPer5Seconds = GameManager.PlatConfigMgr.GetGameConfigItemInt("maxposcmdnum", 8);

		// Token: 0x04004EFF RID: 20223
		public static int MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 500);

		// Token: 0x04004F00 RID: 20224
		public static int MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);

		// Token: 0x04004F01 RID: 20225
		public bool IsGM = false;

		// Token: 0x04004F02 RID: 20226
		public int IsGuest;

		// Token: 0x04004F03 RID: 20227
		public int gmPriority;

		// Token: 0x04004F04 RID: 20228
		public long LastLogoutServerTicks;

		// Token: 0x04004F05 RID: 20229
		private int cmdNum = 0;

		// Token: 0x04004F06 RID: 20230
		private long beginTime = TimeUtil.NOW();

		// Token: 0x04004F07 RID: 20231
		private Queue<Tuple<long, long>> CheckCmdTimeQueue = new Queue<Tuple<long, long>>();

		// Token: 0x04004F08 RID: 20232
		private long[] _socketTime;

		// Token: 0x04004F09 RID: 20233
		private int _socketState;

		// Token: 0x04004F0A RID: 20234
		private int _cmdID;

		// Token: 0x04004F0B RID: 20235
		private int _cmdCount;

		// Token: 0x04004F0C RID: 20236
		private long _cmdTime;

		// Token: 0x04004F0D RID: 20237
		private int _timeOutCount;

		// Token: 0x04004F0E RID: 20238
		private int _decryptCount;

		// Token: 0x04004F0F RID: 20239
		public bool? InIpWhiteList;

		// Token: 0x04004F10 RID: 20240
		public bool? InUseridWhiteList;

		// Token: 0x04004F11 RID: 20241
		public int IsAdult;
	}
}
