﻿using System;

namespace Server.Protocol
{
	
	internal enum TCPCmdPacketSize
	{
		
		MAX_SIZE = 131072,
		
		LIMIT_SIZE = 1048576,
		
		RECV_MAX_SIZE = 6144
	}
}
