using System;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;

namespace KF.Contract
{
	// Token: 0x02000003 RID: 3
	public class AuthorizationContext : IAuthorizeRemotingConnection
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		bool IAuthorizeRemotingConnection.IsConnectingEndPointAuthorized(EndPoint endPoint)
		{
			Console.WriteLine("新客户IP : " + endPoint);
			return true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207C File Offset: 0x0000027C
		bool IAuthorizeRemotingConnection.IsConnectingIdentityAuthorized(IIdentity identity)
		{
			Console.WriteLine("新客户用户名 : " + identity.Name);
			return true;
		}
	}
}
