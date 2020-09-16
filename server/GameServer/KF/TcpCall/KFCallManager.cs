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
	
	public class KFCallManager
	{
		
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

		
		private static void OnSetSocket(KFCallManager.KFClient client)
		{
			client.KeepGetMessage = TcpCall.KFServiceBase.KeepGetMessage(new Action<ReturnValue<KFCallMsg>>(KFCallManager.KFCallMsgFunc));
		}

		
		public static void KFCallMsgFunc(ReturnValue<KFCallMsg> msg)
		{
			if (msg.Type == ReturnType.Success)
			{
				KFCallManager.MsgSource.fireEvent(msg.Value.KuaFuEventType, msg.Value);
			}
		}

		
		private static object Mutex = new object();

		
		public static EventSourceEx<KFCallMsg> MsgSource = new EventSourceEx<KFCallMsg>();

		
		public static bool IsLogin;

		
		public static string Host;

		
		public static int Port;

		
		public static KuaFuClientContext ClientInfo;

		
		private static KFCallManager.KFClient Current;

		
		private class KFClient : IDisposable
		{
			
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

			
			~KFClient()
			{
				this.Dispose();
			}

			
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

			
			private bool IsDisposed;

			
			public AutoCSer.Net.TcpInternalServer.ServerAttribute _Attribute;

			
			private AutoCSer.Net.TcpStaticServer.Client tcpClient;

			
			private ClientSocketBase _Socket;

			
			public Action<KFCallManager.KFClient> OnSetSocket;

			
			public KeepCallback KeepGetMessage;
		}
	}
}
