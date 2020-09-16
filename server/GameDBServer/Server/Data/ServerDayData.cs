using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ServerDayData
	{
		
		[ProtoMember(1)]
		public int Dayid = 0;

		
		[ProtoMember(2)]
		public string CDate;

		
		[ProtoMember(3)]
		public int WorldLevel = 0;
	}
}
