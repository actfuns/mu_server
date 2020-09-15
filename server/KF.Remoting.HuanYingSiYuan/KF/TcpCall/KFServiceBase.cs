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
	// Token: 0x0200005D RID: 93
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class KFServiceBase
	{
		// Token: 0x06000430 RID: 1072 RVA: 0x0003670D File Offset: 0x0003490D
		public static void TimerProc()
		{
			ClientAgentManager.Instance().SendAsyncKuaFuMsg();
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x0003671C File Offset: 0x0003491C
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

		// Token: 0x06000432 RID: 1074 RVA: 0x00036850 File Offset: 0x00034A50
		[AutoCSer.Net.TcpStaticServer.KeepCallbackMethod(IsClientAsynchronous = false, IsClientAwaiter = false)]
		public static void KeepGetMessage(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, Func<ReturnValue<KFCallMsg>, bool> onMessage)
		{
			ClientAgent agent = socket.ClientObject as ClientAgent;
			if (null != agent)
			{
				agent.KFCallMsg = onMessage;
			}
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x0003687C File Offset: 0x00034A7C
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int ExecuteCommand(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, string[] args)
		{
			return -11004;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00036894 File Offset: 0x00034A94
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

		// Token: 0x04000249 RID: 585
		public const string ClientSegmentationCopyPath = "GameServer\\Remoting\\";

		// Token: 0x0200005E RID: 94
		internal static class TcpStaticServer
		{
			// Token: 0x06000435 RID: 1077 RVA: 0x00036900 File Offset: 0x00034B00
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M12(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, string[] args)
			{
				return KFServiceBase.ExecuteCommand(_sender_, args);
			}

			// Token: 0x06000436 RID: 1078 RVA: 0x0003691C File Offset: 0x00034B1C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M13(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, KuaFuClientContext clientInfo)
			{
				return KFServiceBase.InitializeClient(_sender_, clientInfo);
			}

			// Token: 0x06000437 RID: 1079 RVA: 0x00036935 File Offset: 0x00034B35
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M14(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, Func<ReturnValue<KFCallMsg>, bool> _onReturn_)
			{
				KFServiceBase.KeepGetMessage(_sender_, _onReturn_);
			}

			// Token: 0x06000438 RID: 1080 RVA: 0x00036940 File Offset: 0x00034B40
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M15(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, int serverId, Dictionary<int, int> mapClientCountDict)
			{
				KFServiceBase.UpdateKuaFuMapClientCount(_sender_, serverId, mapClientCountDict);
			}
		}
	}
}
