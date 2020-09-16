using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WarnInfo
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int ID = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public string Desc = "";

		
		[ProtoMember(3, IsRequired = true)]
		public int TimeSec = 0;

		
		[ProtoMember(4, IsRequired = true)]
		public int Operate = 0;
	}
}
