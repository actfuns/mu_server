using System;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;

namespace KF.Contract
{
	
	public class AuthorizationContext : IAuthorizeRemotingConnection
	{
		
		bool IAuthorizeRemotingConnection.IsConnectingEndPointAuthorized(EndPoint endPoint)
		{
			Console.WriteLine("新客户IP : " + endPoint);
			return true;
		}

		
		bool IAuthorizeRemotingConnection.IsConnectingIdentityAuthorized(IIdentity identity)
		{
			Console.WriteLine("新客户用户名 : " + identity.Name);
			return true;
		}
	}
}
