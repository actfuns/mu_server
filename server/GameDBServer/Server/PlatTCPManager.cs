using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Share.Server;

namespace GameDBServer.Server
{
	// Token: 0x02000202 RID: 514
	internal class PlatTCPManager
	{
		// Token: 0x06000A9C RID: 2716 RVA: 0x000655C8 File Offset: 0x000637C8
		private PlatTCPManager()
		{
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x000655F8 File Offset: 0x000637F8
		public static PlatTCPManager getInstance()
		{
			return PlatTCPManager.instance;
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000A9E RID: 2718 RVA: 0x00065610 File Offset: 0x00063810
		public Dictionary<int, PlatTCPManager.NetCommandFunc> ProcessCmdFuncDict
		{
			get
			{
				return this._ProcessCmdFuncDict;
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00065628 File Offset: 0x00063828
		public void initialize(int capacity)
		{
			capacity = Math.Max(capacity, 10);
			this.socketListener = new SocketListener(capacity, 32768);
			this.inputServers = new Dictionary<Socket, int>();
			this.dictInPackets = new Dictionary<Socket, TCPInPacket>();
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.tcpOutPacketPool.initialize(capacity * 5);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this.InitCmds();
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x000656F4 File Offset: 0x000638F4
		protected bool InitCmds()
		{
			this._ProcessCmdFuncDict = new Dictionary<int, PlatTCPManager.NetCommandFunc>();
			this._ProcessCmdFuncDict.Add(1, new PlatTCPManager.NetCommandFunc(this.ProcessRechargeData));
			this._ProcessCmdFuncDict.Add(2, new PlatTCPManager.NetCommandFunc(this.ProcessPlatGift));
			return true;
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000AA1 RID: 2721 RVA: 0x00065744 File Offset: 0x00063944
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0006575C File Offset: 0x0006395C
		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00065779 File Offset: 0x00063979
		public void Stop()
		{
			this.socketListener.Stop();
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00065788 File Offset: 0x00063988
		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.inputServers)
			{
				int flag = 0;
				if (!this.inputServers.TryGetValue(s, out flag))
				{
					this.inputServers.Add(s, 1);
				}
			}
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00065810 File Offset: 0x00063A10
		private void SocketClosed(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(s))
				{
					TCPInPacket tcpInPacket = this.dictInPackets[s];
					this.tcpInPacketPool.Push(tcpInPacket);
					this.dictInPackets.Remove(s);
				}
			}
			lock (this.inputServers)
			{
				this.inputServers.Remove(s);
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x000658F8 File Offset: 0x00063AF8
		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			TCPInPacket tcpInPacket = null;
			Socket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (!this.dictInPackets.TryGetValue(s, out tcpInPacket))
				{
					tcpInPacket = this.tcpInPacketPool.Pop(s, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
					this.dictInPackets[s] = tcpInPacket;
				}
			}
			return tcpInPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x000659C4 File Offset: 0x00063BC4
		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket tcpOutPacket = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(tcpOutPacket);
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x000659F8 File Offset: 0x00063BF8
		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpInPacket = sender as TCPInPacket;
			TCPOutPacket tcpOutPacket = null;
			int flag = 0;
			bool result2;
			if (!this.inputServers.TryGetValue(tcpInPacket.CurrentSocket, out flag))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未建立会话或会话已关闭: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)), null, true);
				result2 = false;
			}
			else
			{
				long processBeginTime = TimeUtil.NowEx();
				TCPProcessCmdResults result = PlatTCPManager.ProcessCmd(tcpInPacket.CurrentSocket, this.tcpOutPacketPool, (int)tcpInPacket.PacketCmdID, tcpInPacket.GetPacketBytes(), tcpInPacket.PacketDataSize, out tcpOutPacket);
				long processTime = TimeUtil.NowEx() - processBeginTime;
				if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacket)
				{
					this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket);
				}
				else if (result == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket)), null, true);
					return false;
				}
				result2 = true;
			}
			return result2;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x00065B04 File Offset: 0x00063D04
		public static TCPProcessCmdResults ProcessCmd(Socket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			PlatTCPManager.NetCommandFunc func = null;
			TCPProcessCmdResults result;
			if (!PlatTCPManager.getInstance().ProcessCmdFuncDict.TryGetValue(nID, out func) || null == func)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未指定处理方式的命令: {0}, 关闭连接", (TCPPlatCmds)nID), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				result = func(pool, nID, data, count, out tcpOutPacket);
			}
			return result;
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00065B6C File Offset: 0x00063D6C
		private TCPProcessCmdResults ProcessRechargeData(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RechargeData rechargeData = null;
			try
			{
				rechargeData = DataHelper.BytesToObject<RechargeData>(data, 0, count);
				if (null == rechargeData)
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string strMD5Key = string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
				{
					rechargeData.Amount,
					rechargeData.UserID,
					rechargeData.ZoneID,
					rechargeData.order_no,
					rechargeData.cporder_no,
					rechargeData.Time,
					GameDBManager.serverDBInfo.ChargeKey
				});
				string sign = MD5Helper.get_md5_string(strMD5Key);
				string strcmd;
				if (sign != rechargeData.Sign)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Sign Faild : sign={0} recvsign={1} strMD5Key={2}", sign, rechargeData.Sign, strMD5Key), null, true);
					strcmd = string.Format("{0}:{1}", rechargeData.Id, -4);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBQuery.CheckOrderNo(DBManager.getInstance(), rechargeData.order_no) && (DBQuery.CheckInputLogOrderNo(DBManager.getInstance(), rechargeData.order_no) || DBQuery.CheckInputLog2OrderNo(DBManager.getInstance(), rechargeData.order_no)))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2OrderNo Faild : order_no={0}", rechargeData.order_no), null, true);
					strcmd = string.Format("{0}:{1}", rechargeData.Id, -5);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!DBWriter.Insert2OrderNo(DBManager.getInstance(), rechargeData.order_no))
				{
				}
				if (!DBWriter.Insert2InputLog(DBManager.getInstance(), rechargeData))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2InputLog Faild : order_no={0}", rechargeData.order_no), null, true);
					strcmd = string.Format("{0}:{1}", rechargeData.Id, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				TempMoneyInfo info = new TempMoneyInfo();
				info.userID = rechargeData.UserID;
				info.chargeRoleID = rechargeData.RoleID;
				info.addUserMoney = rechargeData.Amount;
				info.addUserItem = rechargeData.ItemId;
				info.chargeTm = rechargeData.ChargeTime;
				info.cc = rechargeData.cc;
				info.budanflag = rechargeData.BudanFlag;
				if (!DBWriter.Insert2TempMoney(DBManager.getInstance(), info))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2TempMoney Faild : order_no={0} userid={1} rid={2} Amount={3} ItemID={4}", new object[]
					{
						rechargeData.order_no,
						rechargeData.UserID,
						rechargeData.RoleID,
						rechargeData.Amount,
						rechargeData.ItemId
					}), null, true);
					strcmd = string.Format("{0}:{1}", rechargeData.Id, -3);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}", rechargeData.Id, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00065F3C File Offset: 0x0006413C
		private TCPProcessCmdResults ProcessPlatGift(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				PlatGiftData cmdData = DataHelper.BytesToObject<PlatGiftData>(data, 0, count);
				if (null == cmdData)
				{
					throw new Exception("数据解析失败,null == cmdData");
				}
				string strMD5Key = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
				{
					cmdData.RoleID,
					cmdData.UserID,
					cmdData.Type,
					cmdData.ID,
					cmdData.ExtraData,
					cmdData.Message,
					cmdData.Time,
					GameDBManager.serverDBInfo.ChargeKey
				});
				string sign = MD5Helper.get_md5_string(strMD5Key);
				string strcmd;
				if (sign != cmdData.Sign)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Sign Faild : sign={0} recvsign={1} strMD5Key={2}", sign, cmdData.Sign, strMD5Key), null, true);
					strcmd = string.Format("{0}:{1}", cmdData.ID, -4);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}", cmdData.ID, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x04000C72 RID: 3186
		private static PlatTCPManager instance = new PlatTCPManager();

		// Token: 0x04000C73 RID: 3187
		private TCPInPacketPool tcpInPacketPool = null;

		// Token: 0x04000C74 RID: 3188
		private TCPOutPacketPool tcpOutPacketPool = null;

		// Token: 0x04000C75 RID: 3189
		private Dictionary<Socket, int> inputServers = null;

		// Token: 0x04000C76 RID: 3190
		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		// Token: 0x04000C77 RID: 3191
		private Dictionary<int, PlatTCPManager.NetCommandFunc> _ProcessCmdFuncDict;

		// Token: 0x04000C78 RID: 3192
		private SocketListener socketListener = null;

		// Token: 0x02000203 RID: 515
		// (Invoke) Token: 0x06000AAE RID: 2734
		public delegate TCPProcessCmdResults NetCommandFunc(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket);

		// Token: 0x02000204 RID: 516
		public class RechargeConst
		{
			// Token: 0x04000C79 RID: 3193
			public const int Sucess = 1;

			// Token: 0x04000C7A RID: 3194
			public const int InputError = -2;

			// Token: 0x04000C7B RID: 3195
			public const int TempMoneyError = -3;

			// Token: 0x04000C7C RID: 3196
			public const int SignError = -4;

			// Token: 0x04000C7D RID: 3197
			public const int AllExist = -5;
		}
	}
}
