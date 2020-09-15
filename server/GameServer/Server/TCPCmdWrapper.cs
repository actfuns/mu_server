using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Server
{
	// Token: 0x020008B8 RID: 2232
	public class TCPCmdWrapper
	{
		// Token: 0x06003DD1 RID: 15825 RVA: 0x0034C50C File Offset: 0x0034A70C
		public TCPCmdWrapper(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count)
		{
			this.tcpMgr = tcpMgr;
			this.socket = socket;
			this.tcpClientPool = tcpClientPool;
			this.tcpRandKey = tcpRandKey;
			this.pool = pool;
			this.nID = nID;
			this.data = data;
			this.count = count;
			lock (TCPCmdWrapper.CountLock)
			{
				TCPCmdWrapper.TotalWrapperCount++;
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06003DD2 RID: 15826 RVA: 0x0034C5A4 File Offset: 0x0034A7A4
		public TCPManager TcpMgr
		{
			get
			{
				return this.tcpMgr;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06003DD3 RID: 15827 RVA: 0x0034C5BC File Offset: 0x0034A7BC
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06003DD4 RID: 15828 RVA: 0x0034C5D4 File Offset: 0x0034A7D4
		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06003DD5 RID: 15829 RVA: 0x0034C5EC File Offset: 0x0034A7EC
		public int NID
		{
			get
			{
				return this.nID;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06003DD6 RID: 15830 RVA: 0x0034C604 File Offset: 0x0034A804
		public TCPOutPacketPool Pool
		{
			get
			{
				return this.pool;
			}
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06003DD7 RID: 15831 RVA: 0x0034C61C File Offset: 0x0034A81C
		public TCPRandKey TcpRandKey
		{
			get
			{
				return this.tcpRandKey;
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06003DD8 RID: 15832 RVA: 0x0034C634 File Offset: 0x0034A834
		public TCPClientPool TcpClientPool
		{
			get
			{
				return this.tcpClientPool;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06003DD9 RID: 15833 RVA: 0x0034C64C File Offset: 0x0034A84C
		public TMSKSocket TMSKSocket
		{
			get
			{
				return this.socket;
			}
		}

		// Token: 0x06003DDA RID: 15834 RVA: 0x0034C664 File Offset: 0x0034A864
		public static int GetTotalCount()
		{
			int totalCount = 0;
			lock (TCPCmdWrapper.CountLock)
			{
				totalCount = TCPCmdWrapper.TotalWrapperCount;
			}
			return totalCount;
		}

		// Token: 0x06003DDB RID: 15835 RVA: 0x0034C6B8 File Offset: 0x0034A8B8
		public static TCPCmdWrapper Get(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count)
		{
			TCPCmdWrapper wrapper = null;
			lock (TCPCmdWrapper.CachedWrapperList)
			{
				if (TCPCmdWrapper.CachedWrapperList.Count > 0)
				{
					wrapper = TCPCmdWrapper.CachedWrapperList.Dequeue();
					wrapper.tcpMgr = tcpMgr;
					wrapper.socket = socket;
					wrapper.tcpClientPool = tcpClientPool;
					wrapper.tcpRandKey = tcpRandKey;
					wrapper.pool = pool;
					wrapper.nID = nID;
					wrapper.data = data;
					wrapper.count = count;
				}
			}
			if (null == wrapper)
			{
				wrapper = new TCPCmdWrapper(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, nID, data, count);
			}
			return wrapper;
		}

		// Token: 0x06003DDC RID: 15836 RVA: 0x0034C788 File Offset: 0x0034A988
		public void release()
		{
			lock (TCPCmdWrapper.CachedWrapperList)
			{
				if (TCPCmdWrapper.CachedWrapperList.Count < TCPCmdWrapper.MaxCachedWrapperCount)
				{
					TCPCmdWrapper.CachedWrapperList.Enqueue(this);
					return;
				}
			}
			this.tcpMgr = null;
			this.socket = null;
			this.tcpClientPool = null;
			this.tcpRandKey = null;
			this.pool = null;
			this.data = null;
			lock (TCPCmdWrapper.CountLock)
			{
				TCPCmdWrapper.TotalWrapperCount--;
			}
		}

		// Token: 0x040047DE RID: 18398
		public static int MaxCachedWrapperCount = 1000;

		// Token: 0x040047DF RID: 18399
		private static Queue<TCPCmdWrapper> CachedWrapperList = new Queue<TCPCmdWrapper>();

		// Token: 0x040047E0 RID: 18400
		private static object CountLock = new object();

		// Token: 0x040047E1 RID: 18401
		private static int TotalWrapperCount = 0;

		// Token: 0x040047E2 RID: 18402
		private TCPManager tcpMgr;

		// Token: 0x040047E3 RID: 18403
		private TMSKSocket socket;

		// Token: 0x040047E4 RID: 18404
		private TCPClientPool tcpClientPool;

		// Token: 0x040047E5 RID: 18405
		private TCPRandKey tcpRandKey;

		// Token: 0x040047E6 RID: 18406
		private TCPOutPacketPool pool;

		// Token: 0x040047E7 RID: 18407
		private int nID;

		// Token: 0x040047E8 RID: 18408
		private byte[] data;

		// Token: 0x040047E9 RID: 18409
		private int count;
	}
}
