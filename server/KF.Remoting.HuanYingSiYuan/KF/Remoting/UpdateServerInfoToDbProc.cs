using System;

namespace KF.Remoting
{
	
	
	public delegate void UpdateServerInfoToDbProc(int serverId, string ip, int port, int dbPort, int logdbPort, int flags);
}
