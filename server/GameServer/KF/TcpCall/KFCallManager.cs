using System;
using System.Configuration;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using KF.Contract;
using KF.Remoting;
using KF.Remoting.HuanYingSiYuan.TcpStaticClient;
using Server.Tools;
using Tmsk.Contract.Tools;

namespace KF.TcpCall
{
	// Token: 0x02000895 RID: 2197
	public class KFCallManager
	{
		// Token: 0x06003D2F RID: 15663 RVA: 0x0034442C File Offset: 0x0034262C
		public static bool Start()
		{
			lock (KFCallManager.Mutex)
			{
				KFCallManager.KFClient oldClient = KFCallManager.Current;
				string host = null;
				int port = 0;
				string uri = ConfigurationManager.AppSettings.Get("KFService");
				if (!string.IsNullOrEmpty(uri))
				{
					string[] array = uri.Split(new char[]
					{
						':'
					});
					if (array.Length == 2 && int.TryParse(array[1], out port) && port > 0 && port < 65535)
					{
						host = array[0];
					}
				}
				if (!string.IsNullOrEmpty(host))
				{
					if (oldClient != null)
					{
						if (KFCallManager.Host != host || KFCallManager.Port != port)
						{
							oldClient.OnSetSocket = null;
							KFCallManager.NewKFClient(host, port);
							oldClient.Dispose();
						}
					}
					else
					{
						KFCallManager.NewKFClient(host, port);
					}
				}
				else if (null != oldClient)
				{
					oldClient.OnSetSocket = null;
					oldClient.Dispose();
				}
			}
			return true;
		}

		// Token: 0x06003D30 RID: 15664 RVA: 0x00344578 File Offset: 0x00342778
		private static void NewKFClient(string host, int port)
		{
			AutoCSer.Net.TcpInternalServer.ServerAttribute attribute = new AutoCSer.Net.TcpInternalServer.ServerAttribute();
			attribute.IsAutoClient = true;
			attribute.Host = host;
			attribute.Port = port;
			attribute.ServerSendBufferMaxSize = 33554432;
			attribute.ClientSendBufferMaxSize = 4194304;
			KuaFuClientContext clientInfo = new KuaFuClientContext();
			clientInfo.ServerId = GameManager.ServerId;
			clientInfo.Token = GameCoreInterface.getinstance().GetLocalAddressIPs();
			KFCallManager.Host = host;
			KFCallManager.Port = port;
			KFCallManager.ClientInfo = clientInfo;
			AutoCSer.Net.TcpStaticServer.Client tcpClient = KfCall.NewTcpClient(attribute, null, MyLogAdapter.GetILog(), new Func<AutoCSer.Net.TcpInternalServer.ClientSocketSender, bool>(KFCallManager.verifyMethod));
			KFCallManager.Current = new KFCallManager.KFClient(tcpClient, attribute, clientInfo, new Action<KFCallManager.KFClient>(KFCallManager.OnSetSocket));
			tcpClient.TryCreateSocket();
		}

		// Token: 0x06003D31 RID: 15665 RVA: 0x00344624 File Offset: 0x00342824
		private static bool verifyMethod(AutoCSer.Net.TcpInternalServer.ClientSocketSender sender)
		{
			ReturnValue<bool> returnValue = TcpCall.KFServiceBase.InitializeClient(sender, KFCallManager.ClientInfo);
			bool result;
			if (returnValue.Type == ReturnType.Success && returnValue.Value)
			{
				KFCallManager.ClientInfo.ClientId = TimeUtil.AgeByUnixTime(KFCallManager.ClientInfo.ClientId);
				LogManager.WriteLog(LogTypes.Running, string.Format("连接中心成功,host={0},port={1},error={2}", KFCallManager.Host, KFCallManager.Port, returnValue.Type.ToString()), null, true);
				result = (KFCallManager.IsLogin = true);
			}
			else
			{
				LogManager.WriteLog(LogTypes.Running, string.Format("连接中心失败,host={0},port={1},error={2}", KFCallManager.Host, KFCallManager.Port, returnValue.Type.ToString()), null, true);
				result = (KFCallManager.IsLogin = false);
			}
			return result;
		}

		// Token: 0x06003D32 RID: 15666 RVA: 0x003446F1 File Offset: 0x003428F1
		private static void OnSetSocket(KFCallManager.KFClient client)
		{
			client.KeepGetMessage = TcpCall.KFServiceBase.KeepGetMessage(new Action<ReturnValue<KFCallMsg>>(KFCallManager.KFCallMsgFunc));
		}

		// Token: 0x06003D33 RID: 15667 RVA: 0x0034470C File Offset: 0x0034290C
		public static void KFCallMsgFunc(ReturnValue<KFCallMsg> msg)
		{
			if (msg.Type == ReturnType.Success)
			{
				KFCallManager.MsgSource.fireEvent(msg.Value.KuaFuEventType, msg.Value);
			}
		}

		// Token: 0x04004797 RID: 18327
		private static object Mutex = new object();

		// Token: 0x04004798 RID: 18328
		public static EventSourceEx<KFCallMsg> MsgSource = new EventSourceEx<KFCallMsg>();

		// Token: 0x04004799 RID: 18329
		public static bool IsLogin;

		// Token: 0x0400479A RID: 18330
		public static string Host;

		// Token: 0x0400479B RID: 18331
		public static int Port;

		// Token: 0x0400479C RID: 18332
		public static KuaFuClientContext ClientInfo;

		// Token: 0x0400479D RID: 18333
		private static KFCallManager.KFClient Current;

		// Token: 0x02000896 RID: 2198
		private class KFClient : IDisposable
		{
			// Token: 0x06003D36 RID: 15670 RVA: 0x0034479C File Offset: 0x0034299C
			public KFClient(AutoCSer.Net.TcpStaticServer.Client client, AutoCSer.Net.TcpInternalServer.ServerAttribute attribute, KuaFuClientContext clientInfo, Action<KFCallManager.KFClient> onSetSocket)
			{
				this._Attribute = attribute;
				this.OnSetSocket = onSetSocket;
				this.tcpClient = client;
				this.tcpClient.OnSetSocket(delegate(ClientSocketBase socket)
				{
					if (socket.IsSocketVersion(ref this._Socket))
					{
						this.OnSetSocket(this);
					}
				});
			}

			// Token: 0x06003D37 RID: 15671 RVA: 0x003447EC File Offset: 0x003429EC
			~KFClient()
			{
				this.Dispose();
			}

			// Token: 0x06003D38 RID: 15672 RVA: 0x00344820 File Offset: 0x00342A20
			public void Dispose()
			{
				if (!this.IsDisposed)
				{
					try
					{
						this.IsDisposed = true;
						GC.SuppressFinalize(this);
						if (null != this.tcpClient)
						{
							this.tcpClient.Dispose();
							this.tcpClient = null;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}

			// Token: 0x0400479E RID: 18334
			private bool IsDisposed;

			// Token: 0x0400479F RID: 18335
			public AutoCSer.Net.TcpInternalServer.ServerAttribute _Attribute;

			// Token: 0x040047A0 RID: 18336
			private AutoCSer.Net.TcpStaticServer.Client tcpClient;

			// Token: 0x040047A1 RID: 18337
			private ClientSocketBase _Socket;

			// Token: 0x040047A2 RID: 18338
			public Action<KFCallManager.KFClient> OnSetSocket;

			// Token: 0x040047A3 RID: 18339
			public KeepCallback KeepGetMessage;
		}
	}
}
