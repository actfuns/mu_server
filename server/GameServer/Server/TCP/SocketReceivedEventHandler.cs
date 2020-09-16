using System;
using System.Net.Sockets;

namespace Server.TCP
{
	
	
	public delegate bool SocketReceivedEventHandler(object sender, SocketAsyncEventArgs e);
}
