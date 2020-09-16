using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoCSer.Metadata;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Contract;
using KF.Remoting;
using Server.Tools;

namespace KF.TcpCall
{
	
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class KFServiceBase
	{
		
		public static void TimerProc()
		{
			ClientAgentManager.Instance().SendAsyncKuaFuMsg();
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, IsVerifyMethod = true, IsClientAwaiter = false)]
		public static bool InitializeClient(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, KuaFuClientContext clientInfo)
		{
			bool isLogin = false;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool bFirstInit = false;
					int ret = ClientAgentManager.Instance().InitializeClient(clientInfo, out bFirstInit);
					if (ret > 0)
					{
						isLogin = true;
						socket.ClientObject = ClientAgentManager.Instance().GetCurrentClientAgent(clientInfo.ServerId);
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
			}
			return isLogin;
		}

		
		[AutoCSer.Net.TcpStaticServer.KeepCallbackMethod(IsClientAsynchronous = false, IsClientAwaiter = false)]
		public static void KeepGetMessage(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, Func<ReturnValue<KFCallMsg>, bool> onMessage)
		{
			ClientAgent agent = socket.ClientObject as ClientAgent;
			if (null != agent)
			{
				agent.KFCallMsg = onMessage;
			}
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int ExecuteCommand(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, string[] args)
		{
			return -11004;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ServerTask = ServerTaskType.Queue, IsClientSendOnly = true, IsClientAwaiter = false)]
		public static void UpdateKuaFuMapClientCount(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, int serverId, Dictionary<int, int> mapClientCountDict)
		{
			ClientAgent agent = socket.ClientObject as ClientAgent;
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				KuaFuServerManager.UpdateKuaFuLineData(agent.ClientInfo.ServerId, mapClientCountDict);
				ClientAgentManager.Instance().SetMainlinePayload(agent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
			}
		}

		
		public const string ClientSegmentationCopyPath = "GameServer\\Remoting\\";

		
		internal static class TcpStaticServer
		{
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M12(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, string[] args)
			{
				return KFServiceBase.ExecuteCommand(_sender_, args);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M13(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, KuaFuClientContext clientInfo)
			{
				return KFServiceBase.InitializeClient(_sender_, clientInfo);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M14(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, Func<ReturnValue<KFCallMsg>, bool> _onReturn_)
			{
				KFServiceBase.KeepGetMessage(_sender_, _onReturn_);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M15(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, int serverId, Dictionary<int, int> mapClientCountDict)
			{
				KFServiceBase.UpdateKuaFuMapClientCount(_sender_, serverId, mapClientCountDict);
			}
		}
	}
}
