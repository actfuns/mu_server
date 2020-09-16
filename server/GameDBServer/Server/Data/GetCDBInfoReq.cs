using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GetCDBInfoReq
	{
		
		[ProtoMember(2)]
		public string PTID;

		
		[ProtoMember(3)]
		public string ServerID = "";

		
		[ProtoMember(1)]
		public string Gamecode = "";
	}
}
