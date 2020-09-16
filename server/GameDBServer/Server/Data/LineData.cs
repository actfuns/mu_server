﻿using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LineData
	{
		
		[ProtoMember(1)]
		public int LineID = 0;

		
		[ProtoMember(2)]
		public string GameServerIP = "";

		
		[ProtoMember(3)]
		public int GameServerPort = 0;

		
		[ProtoMember(4)]
		public int OnlineCount = 0;
	}
}
